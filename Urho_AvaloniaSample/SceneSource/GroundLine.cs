﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Global;
using ModelFrom2DShape;
using Urho;
using static OU.DistD;
using OU;
using U = OU.Utils;
using U2 = Global.Utils;

namespace SceneSource
{
	/// <summary>
	/// Aka "Wall". A line drawn along the ground, to be converted into a 3D model.
	/// TBD: Option for Point3Ds, to represent a variable altitude above/below ground?
	/// </summary>
	public class GroundLine : SourceItem
	{
		enum SegmentShape
		{
			Quad,
			// Triangle with zero width at end of segment.
			CollapsedEnd,
			// Triangle with zero width at start of segment.
			CollapsedStart
		}
		// true to create new wall BEFORE mouse down.
		// TBD: Need code to throw away any "unused" wall, when done editing.
		public const bool PreCreateWall = false;//true;
		// false to smooth while creating the quads.
		// Will change to true once I have "fix-up" code on mouse up,
		// that re-calculates the quads, to smooth curves and add bend transitions.
		public const bool AddOnlyNewQuads = false;//true;

		public const float MinWallSegmentLength = GroundLine.SingleGeometryTEST ? 5 : 0.5f;//1f;//0.5f;   // TBD: Good value.
																						   // To better follow ground unevenness. TBD: Should examine ground underneath, adapt as needed.
		public static float MaxWallSegmentLength = Math.Max(5, MinWallSegmentLength + U.VerySmallF);

		// false: To save time, do this on mouse up. Can do it better then anyway.
		const bool SmoothWhileAddPoints = false;
		const bool CastShadows = true; //true;
		public const bool SingleGeometry = false;//false;   // TODO
		public const bool SingleGeometryTEST = false;//false;   // TMS: Temporary changes.

		#region --- data, new ----------------------------------------
		public DistD Width { get; set; }
		public DistD Height { get; set; }
		public DistD BaseAltitude { get; set; }
		public bool HasNormals { get; private set; }
		public bool HasUVs { get; private set; }
		public bool HasTangents { get; private set; }

		private Model _model;

		public Geo.IContext Context { get; set; }
		/// <summary>
		/// CAUTION: Other classes are not allowed to alter Points directly;
		/// call "AddPoint" instead. TBD Future: "RemovePointAt" needed.
		/// Within coord system of a scene, "float" precision is sufficient;
		/// TBD: Make "float" version of Distance2D.
		/// </summary>
		public List<Dist2D> Points { get; private set; }
		public bool DoDeferPoint { get; set; }
		private bool _hasDeferredPoint;
		private Dist2D _deferredPoint;

		private float WidthMetersF => (float)Width.Meters;
		//USE_TopMetersF private float HeightMetersF => (float)Height.Value;
		private float TopMetersF => (float)(Height + BaseAltitude).Meters;
		// Project to ground. When at 0, go slightly below ground, to avoid "crack" at base.
		private float BottomMetersF => (float)(BaseAltitude.Value == 0 ? -0.5 : BaseAltitude.Meters);

		// Top & Sides: Separate polys needed, so can adjust "previous" wall segment.
		// TBD: Start/EndPolys could be combined.
		private Poly3D TopPoly, BottomPoly, FirstSidePoly, SecondSidePoly, StartPoly, EndPoly;

		public GroundLine(bool hasUV = true, bool hasNormals = true) : this(DistD.Zero, DistD.Zero, Geo.NoContext.It, hasUV, hasNormals)
		{
		}

		/// <summary>
		/// ASSUME METERS.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="context"></param>
		public GroundLine(double width, double height, Geo.IContext context = null, bool hasUV = true, bool hasNormals = true, bool hasTangents = true)
				: this(new DistD(width), new DistD(height), context, hasUV, hasNormals, hasTangents)
		{
		}

		public GroundLine(DistD width, DistD height, Geo.IContext context = null, bool hasUV = true, bool hasNormals = true, bool hasTangents = true)
		{
			Width = width;
			Height = height;
			HasUVs = hasUV;
			HasNormals = hasNormals;
			HasTangents = hasTangents;

			// Initialized to altitude zero.
			BaseAltitude = DistD.Zero;
			//BaseAltitude = (DistD)3;   // tmstest

			if (context == null)
				context = Geo.NoContext.It;
			Context = context;

			Points = new List<Dist2D>();
		}
		#endregion

		#region --- OnUpdate ----------------------------------------
		private bool _hasDummyData;

		internal void PreCreate()
		{
			Node wallNode = EnsureWallNode();
			StaticModel wall = EnsureModel(wallNode);
			// This may be the first slow step.
			// HOWEVER, doing it only reduces the later "glitch" from 46 ms to 40-42 ms.
			// It seems that the first update that actually includes contents causes a delay.
			// IDEA: Put some dummy contents into buffers. If its offscreen, will it still help "consume" this "first time" delay?
			// I did make sure BoundingBox was large. OR did that get cleared somewhere?
			EnsureGeometry(wall);
			if (true) {
				_AddPointNow(new Dist2D(-55.5f, -9.2f, null));
				_AddPointNow(new Dist2D(-54.7f, -9.0f, null));
				// Add it to the scene. Hopefully this "consumes" the "first draw glitch".
				OnUpdate();
				// AFTER OnUpdate, which may remove dummy data if this is set.
				_hasDummyData = true;
			}
		}

		internal void OnUpdate(bool bake = false)
		{
			RemoveDummyData();

			// TBD: Ideally, add some representation when there is only one point.
			// Given that we don't know what direction wall will go in,
			// this will have to represent "a point".
			// It could have the height and width of the wall. "Cylinder" would be ideal. "Rectangular Prism" would be okay.
			if (Points.Count < 2)
				return;

			Node wallNode = EnsureWallNode();
			StaticModel wall = EnsureModel(wallNode);

			var it = AvaloniaSample.AvaloniaSample.It;
			CreateGeometryFromPoints(wallNode, wall, it.Terrain);

			UpdateBufferData(bake);
			// TBD OPTIMIZE: Could expand as add points, so don't have to calculate from scratch.
			UpdateBoundingBox();

		}

		public void UpdateBufferData(bool bake = false)
		{
			TopPoly.UpdateBufferData(bake);
			if (!SingleGeometry) {
				BottomPoly.UpdateBufferData(bake);
				FirstSidePoly.UpdateBufferData(bake);
				SecondSidePoly.UpdateBufferData(bake);
				// These are only one quad each. Start doesn't change as wall extends, but End does.
				StartPoly.UpdateBufferData(bake);
				EndPoly.UpdateBufferData(bake);
			}
		}

		/// <summary>
		/// Call UpdateBufferData FIRST.
		/// TBD OPTIMIZE: Could expand as add points, so don't have to calculate from scratch.
		/// </summary>
		private void UpdateBoundingBox()
		{
			var bb = TopPoly.BoundingBox;
			if (!SingleGeometry) {
				bb.Merge(BottomPoly.BoundingBox);
				bb.Merge(FirstSidePoly.BoundingBox);
				bb.Merge(SecondSidePoly.BoundingBox);
				bb.Merge(StartPoly.BoundingBox);
				bb.Merge(EndPoly.BoundingBox);
			}

			_model.BoundingBox = bb;
		}
		#endregion

		#region --- Ensure.., HasContents, Clear, CreateGeometryFromPoints ----------------------------------------
		private Node EnsureWallNode()
		{
			var it = AvaloniaSample.AvaloniaSample.It;
			if (it.WallsNode == null)
				it.WallsNode = it.Scene.CreateChild("Walls");
			if (it.CurrentWallNode == null)
				it.CurrentWallNode = it.WallsNode.CreateChild($"Wall{it.WallsNode.GetNumChildren() + 1}");

			return it.CurrentWallNode;
		}

		public StaticModel EnsureModel(Node node)
		{
			StaticModel sModel = node.GetComponent<StaticModel>();
			if (sModel == null) {
				sModel = node.CreateComponent<StaticModel>();
			}
			return sModel;
		}

		/// <summary>
		/// Sets TopPoly, FirstSidePoly, SecondSidePoly.
		/// </summary>
		/// <param name="sModel"></param>
		public void EnsureGeometry(StaticModel sModel)
		{
			if (TopPoly == null) {
				TopPoly = CreateAndInitPoly(sModel);
				if (SingleGeometry) {
					// HACK to make !SingleGeometry flag possible.
					BottomPoly = TopPoly;
					FirstSidePoly = TopPoly;
					SecondSidePoly = TopPoly;
					StartPoly = TopPoly;
					EndPoly = TopPoly;
				} else {
					BottomPoly = CreateAndInitPoly(sModel);
					FirstSidePoly = CreateAndInitPoly(sModel);
					SecondSidePoly = CreateAndInitPoly(sModel);
					StartPoly = CreateAndInitPoly(sModel);
					EndPoly = CreateAndInitPoly(sModel);
				}
			}

			_model = sModel.Model;
			if (_model == null) {
				_model = new Model();
				_model.NumGeometries = SingleGeometry ? 1 : 6;
				_model.SetGeometry(0, 0, TopPoly.Geom);
				if (!SingleGeometry) {
					_model.SetGeometry(1, 0, BottomPoly.Geom);
					_model.SetGeometry(2, 0, FirstSidePoly.Geom);
					_model.SetGeometry(3, 0, SecondSidePoly.Geom);
					_model.SetGeometry(4, 0, StartPoly.Geom);
					_model.SetGeometry(5, 0, EndPoly.Geom);
				}
				// Make sure it is visible, until do better calculation later in code.
				_model.BoundingBox = new BoundingBox(-10000, 10000);
				sModel.Model = _model;
				sModel.CastShadows = CastShadows;
				sModel.SetMaterial(WallMaterial);
			}
		}

		private static Material WallMaterial {
			get {
				if (_wallMaterial == null) {
					var res = AvaloniaSample.AvaloniaSample.It.ResourceCache;
					// Clone to alter parameters, in case we use this material elsewhere.
					//_wallMaterial = res.GetMaterial("Materials/StoneWall4.xml").Clone();
					//_wallMaterial = Material.FromColor(Color.Magenta, false);
					//_wallMaterial = res.GetMaterial("Materials/StoneWall4Normal.xml").Clone();
					_wallMaterial = res.GetMaterial(AvaloniaSample.AvaloniaSample.BoxMaterialName()).Clone();

					//_wallMaterial.SetShaderParameter("AmbientColor", Color.White);
					//_wallMaterial.SetShaderParameter("AmbientColor", Color.Gray);
					//_wallMaterial.SetShaderParameter("AmbientColor", Color.Magenta);
					//_wallMaterial.SetShaderParameter("AmbientColor", Color.Black);

					//_wallMaterial.PixelShaderDefines("")
					// Commented out this line: Designed to work with "Ccw", like Box model.
					//NO _wallMaterial.CullMode = CullMode.Cw; // CullMode.Cw;

					if (AvaloniaSample.AvaloniaSample.WireframeMaterialIsWall)
						AvaloniaSample.AvaloniaSample.It.SetWireframeMaterial(_wallMaterial);
				}
				return _wallMaterial;
			}
		}
		private static Material _wallMaterial;


		internal bool HasContents()
		{
			return Points.Count > 0 || _hasDeferredPoint;
		}

		internal void Clear()
		{
			Points.Clear();
			_hasDeferredPoint = false;
		}


		public void CreateGeometryFromPoints(Node node, StaticModel model, Terrain terrain)
		{
			if (Points.Count < 2)
				return;

			//DumpPoints();

			//Debug.WriteLine("\n\n------- CreateGeometryFromPoints -------");
			EnsureAndMaybeClearGeometry(node, model);

			// TODO: TO calc good perpendicular (to give wall its width), need THREE points (except at ends).
			// TODO: Need to detect "closed shape", for good perpendicular when wraps.

			bool firstPoint = true;
			bool firstPerpendicular = true;
			// Initial values not used; avoid uninitialized warning.
			// These are on wall's center line.
			var cl0 = new Vec2();
			var cl1 = new Vec2();
			Vec2 perp0 = new Vec2();
			Vec2 perp1 = new Vec2();
			Vec3?[] normals = new Vec3?[4];

			foreach (Dist2D srcPt in Points) {
				// On wall's center line. TBD: Should this be Vec2 in ground plane?
				//Vec3 cl2 = Global.Utils.PlaceOnTerrain(terrain, srcPt.ToVector2(), (float)BaseAltitude.Value);
				Vec2 cl2 = (Vec2)srcPt;
				if (firstPoint) {
					cl1 = cl2;
					// TODO: Can't calc normal yet.
					firstPoint = false;
				} else {
					// Make quad between cl0 and cl1.
					if (firstPerpendicular) {
						// We had no way to compute perpendicular at pt0; now we can.
						firstPerpendicular = false;
						// EXPLAIN: We're only on the second point, so cl0=cl1.
						// There is only one perpendicular possible, from that point to cl2.
						// Put it in perp0; this will get transferred to perp0 at end of loop.
						// So it is perp0 for NEXT iteration.
						perp1 = CalcPerpendicular(cl0, cl2);
					} else {
						// GUESS that a good perpendicular at cl1 is tangent to the neighboring points.
						// TBD: Adjust for relative distances to those points?
						perp1 = CalcPerpendicular(cl0, cl2);

						// Find sharp bends, add special segment.
						// TODO: Currently this returns "false" when freehand drawing,
						// because logic doesn't handle large bends on short segments.
						if (IsBendLarge(cl0, cl1, cl2, out float degreesFromStraight,
										out float midlineHeadingDegrees, out float heading01Degrees, out float heading12Degrees)) {
							Vec2 joinPtAfter = AddWallSegmentsForBend(terrain, cl0, cl1, cl2, perp0, perp1, degreesFromStraight,
																	  midlineHeadingDegrees, heading01Degrees, heading12Degrees, normals);
							// Adjust next segment to start after bend.
							cl1 = joinPtAfter;
						} else {
							// TODO: If this segment is preceded or followed by a bend,
							// the matching end-elevation should be adjusted (to average with neighbor segment).
							AddWallSegments(cl0, cl1, perp0, perp1, terrain, normals);
						}
					}
				}

				cl0 = cl1;
				cl1 = cl2;
				perp0 = perp1;
			}

			// Final quad.
			if (SingleGeometryTEST) {
				//// Hardcoded segment.
				//float x0 = 0;//-30;
				//float z0 = 0;//-40;
				//float dz = 10;//-20;//-5;
				//cl0 = new Vec3(x0, 1, z0);
				//cl1 = new Vec3(x0, 1, z0+dz);
				////U.Swap(ref cl0, ref cl1);   // To see what changes.
				//perp0 = CalcPerpendicular(cl0, cl1);
				//perp1 = perp0;
			}
			AddWallSegments(cl0, cl1, perp0, perp1, terrain, normals);

			FinishGeometry();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="cl0">Previous point.</param>
		/// <param name="cl1">Bend point (on center-line)</param>
		/// <param name="cl2">Next point.</param>
		/// <param name="perp0"></param>
		/// <param name="perp1"></param>
		/// <param name="degreesFromStraight"></param>
		/// <param name="midlineHeadingDegrees"></param>
		/// <param name="normals"></param>
		/// <returns></returns>
		private Vec2 AddWallSegmentsForBend(Terrain terrain, Vec2 cl0, Vec2 cl1, Vec2 cl2, Vec2 perp0, Vec2 perp1,
											float degreesFromStraight, float midlineHeadingDegrees,
											float heading01Degrees, float heading12Degrees, Vec3?[] normals)
		{
			// NOTE: First parameter is the point NEAR to bend.
			// TODO HACK: We "know" AddWallSegment ignores y, so we simply use zero.
			// Better would be to change all the Vec3s to Vec2s.
			Vec2 joinPtBefore = CalcJoinPoint(cl1, cl0, degreesFromStraight);
			Vec2 joinPtAfter = CalcJoinPoint(cl1, cl2, degreesFromStraight);
			// TODO: perp1 is wrong. Works for long segment because it is ignored.
			AddWallSegments(cl0, joinPtBefore, perp0, perp1, terrain, normals);
			Vec2 bendPerp0 = CalcPerpendicular(cl0, cl1);
			Vec2 bendPerp1 = CalcPerpendicular(cl1, cl2);
			// VERSION: Sharp algorithm requires >90, otherwise extension points will "cross'.
			float shallowBendDegrees = 91;
			// Add bend segment. TODO: Need a special shape; this is an approximation to that.
			// Find where inner walls intersect.
			Vec2 innerIntersection = CalcInnerIntersection(cl0, cl1, cl2, joinPtBefore, joinPtAfter,
														   bendPerp0, bendPerp1,
														   out Vec2 outsidePtBefore, out Vec2 outsidePtAfter);
			if (degreesFromStraight < shallowBendDegrees) {
				// This shape would get very thin for sharp bend (doubling-back).
				//AddWallSegment(joinPtBefore, joinPtAfter, bendPerp0, bendPerp1, terrain, normals);
				AddShallowBendSegment(innerIntersection, outsidePtBefore, outsidePtAfter,
									  terrain, normals);
			} else {
				// Move each outside point forward by wall width.
				Vec2 outsidePtBeforeExtended = U.MoveOnAngleDegrees(outsidePtBefore, heading01Degrees, (float)Width);
				// "-": Move back towards bend.
				Vec2 outsidePtAfterExtendedBack = U.MoveOnAngleDegrees(outsidePtAfter, heading12Degrees, -(float)Width);
				Vec2 outsideMidPtXZ = U.Average(outsidePtBeforeExtended, outsidePtAfterExtendedBack);
				_AddWallSegmentXZs(innerIntersection, outsidePtBefore, outsideMidPtXZ, outsidePtBeforeExtended, terrain, normals);
				_AddWallSegmentXZs(outsideMidPtXZ, outsidePtAfterExtendedBack, innerIntersection, outsidePtAfter, terrain, normals);
			}

			return joinPtAfter;
		}

		private void AddShallowBendSegment(Vec2 innerIntersection, Vec2 outsidePtBefore, Vec2 outsidePtAfter,
										   Terrain terrain, Vec3?[] normals)
		{
			// To specify how UVs should be assigned, we make this "triangle" into a quad with very close endpoints on inside.
			Vec2 unitDelta = (outsidePtAfter - outsidePtBefore).Normalized();
			Vec2 tinyDelta = 0.001f * unitDelta;
			// Move these parallel to outside edge, an invisibly small amount.
			Vec2 insidePtBeforeXZ = innerIntersection - tinyDelta;
			Vec2 insidePtAfterXZ = innerIntersection + tinyDelta;
			_AddWallSegmentXZs(insidePtBeforeXZ, outsidePtBefore, insidePtAfterXZ, outsidePtAfter, terrain, normals);
		}

		/// <summary>
		/// Like AddWallSegment, but given XZs rather than Vec3.
		/// </summary>
		/// <param name="insidePtBeforeXZ"></param>
		/// <param name="outsidePtBeforeXZ"></param>
		/// <param name="insidePtAfterXZ"></param>
		/// <param name="outsidePtAfterXZ"></param>
		/// <param name="terrain"></param>
		/// <param name="normals"></param>
		private void _AddWallSegmentXZs(Vec2 insidePtBeforeXZ, Vec2 outsidePtBeforeXZ,
										Vec2 insidePtAfterXZ, Vec2 outsidePtAfterXZ, Terrain terrain, Vec3?[] normals)
		{
			// TODO: What is best way to deal with uneven ground near bend?
			// --- this approach forces inside to be flat and outside to be flat. But there can be "gap" with neighboring segments.
			//var wallTopInsidePts = ProjectPointsToWallTop(insidePtBeforeXZ, insidePtAfterXZ, TopMetersF, terrain);
			//var wallTopOutsidePts = ProjectPointsToWallTop(outsidePtBefore, outsidePtAfter, TopMetersF, terrain);
			//var wallPair0 = new U.Pair<Vec3>(wallTopInsidePts.First, wallTopOutsidePts.First);
			//var wallPair1 = new U.Pair<Vec3>(wallTopInsidePts.Second, wallTopOutsidePts.Second);
			// --- this approach (approximately) matches each edge with the neighboring segment --
			var wallPair0 = ProjectPointsToWallTop(insidePtBeforeXZ, outsidePtBeforeXZ, TopMetersF, terrain);
			var wallPair1 = ProjectPointsToWallTop(insidePtAfterXZ, outsidePtAfterXZ, TopMetersF, terrain);
			_AddWallSegment(wallPair0, wallPair1, terrain, normals);
		}

		private Vec2 CalcInnerIntersection(Vec2 cl0, Vec2 cl1, Vec2 cl2,
										   Vec2 joinPtBefore, Vec2 joinPtAfter, Vec2 bendPerp0, Vec2 bendPerp1,
										   out Vec2 outsidePtBefore, out Vec2 outsidePtAfter)
		{
			// HACK: Rather than figure out WHICH wall sides are closest, calculate both sides,
			// find closest ones.
			U.Pair<Vec3> sidesBefore = WallPerpendicularOnTerrain(joinPtBefore, WidthMetersF, bendPerp0, 0, null);
			U.Pair<Vec3> sidesAfter = WallPerpendicularOnTerrain(joinPtAfter, WidthMetersF, bendPerp1, 0, null);
			U.Pair<Vec2> closestSidePts = ClosestSidePtsXZ(sidesBefore, sidesAfter,
														   out outsidePtBefore, out outsidePtAfter);
			//float error = (closestSidePts.Second - closestSidePts.First).Length();
			//if (error > 0.1)
			//	U.Dubious();

			Vec2 innerIntersection = U.Average(closestSidePts.First, closestSidePts.Second);
			return innerIntersection;
		}

		private U.Pair<Vec2> ClosestSidePtsXZ(U.Pair<Vec3> a, U.Pair<Vec3> b,
											  out Vec2 farPtA, out Vec2 farPtB)
		{
			Vec2 a1 = a.First.XZ();
			var a2 = a.Second.XZ();
			var b1 = b.First.XZ();
			var b2 = b.Second.XZ();

			// Accumulate closest solution.
			float minDistSq = float.MaxValue;
			Vec2 bestA = a1;   // Overridden in first call below.
			Vec2 bestB = b1;
			AccumMinDistSq(a1, b1, ref minDistSq, ref bestA, ref bestB);
			AccumMinDistSq(a1, b2, ref minDistSq, ref bestA, ref bestB);
			AccumMinDistSq(a2, b1, ref minDistSq, ref bestA, ref bestB);
			AccumMinDistSq(a2, b2, ref minDistSq, ref bestA, ref bestB);

			// The far points are the ones not chosen.
			farPtA = bestA == a1 ? a2 : a1;
			farPtB = bestB == b1 ? b2 : b1;
			
			return new U.Pair<Vec2>(bestA, bestB);
		}

		private void AccumMinDistSq(Vec2 a, Vec2 b,
								    ref float minDistSq, ref Vec2 bestA, ref Vec2 bestB)
		{
			float distSq = (b - a).LengthSquared();
			if (distSq < minDistSq) {
				minDistSq = distSq;
				bestA = a;
				bestB = b;
			}
		}

		private void DumpPoints()
		{
			//Debug.WriteLine($"\n----- Points n={Points.Count} -----");
			for (int iPoint = 0; iPoint < Points.Count; iPoint++) {
				//Debug.WriteLine($"{iPoint}: {Points[iPoint]}");
			}
			//Debug.WriteLine($"-----  -----\n");
		}

		private void EnsureAndMaybeClearGeometry(Node node, StaticModel model)
		{
			_currentWallSegmentCount = 0;
			EnsureGeometry(model);

			if (!AddOnlyNewQuads) {
				// Recreating all quads each time.
				TopPoly.Clear();
				if (!SingleGeometry) {
					BottomPoly.Clear();
					FirstSidePoly.Clear();
					SecondSidePoly.Clear();
					StartPoly.Clear();
					EndPoly.Clear();
				}
			}
		}

		private void FinishGeometry()
		{
			_prevWallSegmentCount = _currentWallSegmentCount;
		}

		private void AddWallSegments(Vec2 cl0, Vec2 cl1, Vec2 perp0, Vec2 perp1, Terrain terrain, Vec3?[] normals)
		{
			// Decide whether to break into smaller pieces, to better follow terrain.
			float segmentLength = (cl1 - cl0).Length(); //TBD: LengthFast;
			if (segmentLength > MaxWallSegmentLength) {
				// Smallest number of segments such that no segment is greater than Max.
				int nSegments = (int)Math.Ceiling(segmentLength / MaxWallSegmentLength);

				// IGNORE perp0 & perp1: those were calculated assuming this segment and its neighbors form a curve.
				// Rather, this section of wall is straight (point-to-point clicks).
				perp0 = CalcPerpendicular(cl0, cl1);
				perp1 = perp0;

				int lastI = nSegments - 1;
				// First "tween" (in-between) segment starts here.
				Vec2 startTweenPt = cl0;
				for (int iStep = 0; iStep < nSegments; iStep++) {
					//float startTweenWgt = iStep / (float)nSegments;
					float endTweenWgt = (iStep + 1) / (float)nSegments;
					Vec2 endTweenPt;
					if (iStep == lastI) {
						// Use the exact value of final point.
						endTweenPt = cl1;
					} else
						endTweenPt = U.Lerp(cl0, cl1, endTweenWgt);
					AddWallSegment(startTweenPt, endTweenPt, perp0, perp1, terrain, normals);
					// Prep next.
					startTweenPt = endTweenPt;
				}
			} else {
				// Single segment.
				AddWallSegment(cl0, cl1, perp0, perp1, terrain, normals);
			}
		}

		/// <summary>
		/// CAUTION: No longer tells Poly3D to UpdateBufferData.
		/// Call UpdateBufferData directly, on all polys, after all segments added.
		/// </summary>
		/// <param name="cl0">Center-line point at start of segment</param>
		/// <param name="cl1">Center-line point at end of segment</param>
		/// <param name="perp0"></param>
		/// <param name="perp1"></param>
		/// <param name="terrain"></param>
		/// <param name="normals"></param>
		private void AddWallSegment(Vec2 cl0, Vec2 cl1, Vec2 perp0, Vec2 perp1,
									Terrain terrain, Vec3?[] normals, SegmentShape shape = SegmentShape.Quad)
		{
			if (SingleGeometryTEST && _currentWallSegmentCount >= 1)
				return;   // TEST - only one segment

			_currentWallSegmentCount++;

			// On top of wall.
			U.Pair<Vec3> wallPair0 = WallPerpendicularOnTerrain(cl0, WidthMetersF, perp0, TopMetersF, terrain);
			U.Pair<Vec3> wallPair1 = WallPerpendicularOnTerrain(cl1, WidthMetersF, perp1, TopMetersF, terrain);
			_AddWallSegment(wallPair0, wallPair1, terrain, normals);
		}

		/// <summary>
		/// </summary>
		/// <param name="wallPair0">Start edge of quad</param>
		/// <param name="wallPair1">End edge of quad</param>
		/// <param name="terrain"></param>
		/// <param name="normals"></param>
		private void _AddWallSegment(U.Pair<Vec3> wallPair0, U.Pair<Vec3> wallPair1,
									 Terrain terrain, Vec3?[] normals)
		{

			// Accumulate previous values. For averaging adjacent segment normals.
			Vec3? normTop = normals[0];
			Vec3? normBtm = normals[1];
			Vec3? normSide1 = normals[2];
			Vec3? normSide2 = normals[3];

			//if (shape == SegmentShape.CollapsedStart) {
			//	var wallTop0 = ProjectToTerrain(cl0, terrain, TopMetersF);
			//	wallPair0 = new U.Pair<Vec3>(wallTop0, wallTop0);
			//} else if (shape == SegmentShape.CollapsedEnd) {
			//	var wallTop1 = ProjectToTerrain(cl1, terrain, TopMetersF);
			//	wallPair1 = new U.Pair<Vec3>(wallTop1, wallTop1);
			//}

			// Wall Segment: Top of wall.
			AddQuad(TopPoly, wallPair0, wallPair1, Poly3D.QuadVOrder.WallTop, ref normTop, false, false, false);
			//AddQuad(TopPoly, wallPair0, wallPair1, Poly3D.QuadVOrder.WallTop, ref normTop, false, true, false);

			U.Pair<Vec3> groundPair0 = ProjectToTerrain(wallPair0, terrain, BottomMetersF);
			U.Pair<Vec3> groundPair1 = ProjectToTerrain(wallPair1, terrain, BottomMetersF);
			//if (shape == SegmentShape.CollapsedStart) {
			//	var wallBottom0 = ProjectToTerrain(cl0, terrain, BottomMetersF);
			//	groundPair0 = new U.Pair<Vec3>(wallBottom0, wallBottom0);
			//} else if (shape == SegmentShape.CollapsedEnd) {
			//	var wallBottom1 = ProjectToTerrain(cl1, terrain, BottomMetersF);
			//	groundPair1 = new U.Pair<Vec3>(wallBottom1, wallBottom1);
			//}

			// Wall Segment: Bottom of wall.
			//AddQuad(BottomPoly, groundPair0, groundPair1, Poly3D.QuadVOrder.WallBottom, ref normBtm);
			AddQuad(BottomPoly, groundPair0, groundPair1, Poly3D.QuadVOrder.WallTop, ref normBtm, false, true, true);

			// Wall Segment: First side of wall.
			// Must specify such that the second pair is at far end - these get adjusted when next quad is added.
			U.Pair<Vec3> groundFirstSide0 = new U.Pair<Vec3>(wallPair0.First, groundPair0.First);
			U.Pair<Vec3> groundFirstSide1 = new U.Pair<Vec3>(wallPair1.First, groundPair1.First);
			// QuadVOrder re-orders the vertices to expected order and orientation (top left first).
			AddQuad(FirstSidePoly, groundFirstSide0, groundFirstSide1, Poly3D.QuadVOrder.Wall1, ref normSide1);

			// Wall Segment: Second side of wall.
			// Must specify such that the second pair is at far end - these get adjusted when next quad is added.
			U.Pair<Vec3> groundSecondSide0 = new U.Pair<Vec3>(wallPair0.Second, groundPair0.Second);
			U.Pair<Vec3> groundSecondSide1 = new U.Pair<Vec3>(wallPair1.Second, groundPair1.Second);
			//AddQuad(SecondSidePoly, groundSecondSide0, groundSecondSide1, Poly3D.QuadVOrder.Wall2, ref normSide2, true);
			AddQuad(SecondSidePoly, groundSecondSide0, groundSecondSide1, Poly3D.QuadVOrder.Wall1, ref normSide2, true, true, true);

			normals[0] = normTop;
			normals[1] = normBtm;
			normals[2] = normSide1;
			normals[3] = normSide2;


			if (Points.Count >= 2 && (SingleGeometry || !StartPoly.HasContents)) {
				// Now that we know wall direction, create StartPoly.
				CreateStartPoly(wallPair0, groundPair0, terrain);
			}
			CreateEndPoly(wallPair1, groundPair1, terrain);
		}
		#endregion

		#region --- AddPoint, Flush ----------------------------------------
		/// <summary>
		/// May defer adding the point.
		/// </summary>
		/// <param name="geoPt"></param>
		public void AddPoint(Geo.Point2D geoPt)
		{
			if (geoPt.Context != Context)
				throw new NotImplementedException("AddPoint from a different Context");

			AddPoint(geoPt.Pt);
		}

		/// <summary>
		/// May defer adding the point.
		/// ASSUMES pt is in same GeoContext.
		/// </summary>
		/// <param name="pt"></param>
		public void AddPoint(Dist2D pt)
		{
			//return;   // tmstest: Can we get a lockup when no walls are drawn?
			FlushWithSmooth(pt);

			if (DoDeferPoint) {
				_deferredPoint = pt;
				_hasDeferredPoint = true;
			} else {
				_AddPointNow(pt);
			}
			//Debug.WriteLine($"--- wall pt={pt.Round(3)} rel={(pt - Points[0]).Round(3)} ---");
		}

		/// <summary>
		/// Perform the Add NOW. This is the ONLY place that should do "Points.Add".
		/// Most callers should call "AddPoint" instead.
		/// </summary>
		/// <param name="pt"></param>
		private void _AddPointNow(Dist2D pt)
		{
			//TBD_HIGHER_LEVEL var rawMoves = AvaloniaSample.AvaloniaSample.It.CacheMouseMoves();
			RemoveDummyData();

			if (Points.Count > 0 && pt.NearlyEquals(Points.LastElement()))
				// Don't allow adjacent points to be (nearly) identical.
				return;

			Points.Add(pt);

			if (SmoothWhileAddPoints)
				SmoothRecentPoints();
			// No, fix later when create quads. Need different triangulation at bend.
			//FixRecentPoints();
		}

		private void RemoveDummyData()
		{
			if (_hasDummyData) {
				Points.Clear();
				// TODO: What else do we need to clear??
				_hasDummyData = false;
			}
		}

		private int _iSmoothed = -1;
		static float MaxSmoothLength = MinWallSegmentLength * 2f;//1.6f;
		private List<Dist2D> _tangents = new List<Dist2D>(); 

		private void SmoothRecentPoints()
		{
			// Need 3 points to smooth middle one.
			if (Points.Count >= 3 && _iSmoothed < Points.LastIndex()) {
				ExtendTangents();

				var p1 = Points.NearEndElement(-3);
				var p2 = Points.NearEndElement(-2);
				var p3 = Points.LastElement();

				// ONLY smooth if points are close together.
				float lengthBefore = (float)(p2 - p1).Length;
				float lengthAfter = (float)(p3 - p2).Length;
				if (lengthBefore < MaxSmoothLength && lengthAfter < MaxSmoothLength) {
					// OK to smooth.

					// Smooth at p2.
					Points[Points.Count - 2] = SmoothPoint(p1, p2, p3);
				}

				// Technically, we smoothed at NearEndElement(-2).
				// But this matches the if-test done at start of this method.
				_iSmoothed = Points.LastIndex();
			}
		}

		private void ExtendTangents()
		{
			int iLastIndex = Points.LastIndex();
			if (iLastIndex == 0)
				// Can't calculate any tangents.
				return;

			// "_iSmoothed-2": re-calc one or two tangents - may have changed due to added point.
			for (int i = Math.Max(_iSmoothed - 2, 0); i <= iLastIndex; i++) {
				var p2 = Points[i];
				// "p2": If there is no point before, use current point as start of tangent.
				var p1 = i > 0 ? Points[i - 1] : p2;
				// "p2": If there is no point after, use current point as end of tangent.
				var p3 = i < iLastIndex ? Points[i + 1] : p2;
				var tanDelta = p3 - p1;
				var tanLength = (double)tanDelta.Length;
				if (tanLength.NearlyEquals(0)) {
					// TODO: What should we do?
				} else {
					tanDelta = tanDelta.Normalized();
				}
				_tangents.SetOrAdd(i, tanDelta);
			}
		}

		// Highest index that has been checked for adding a corner.
		private int _iCornerChecked = -1;
		const float SmallBendDegreesLimit = 30;   // TBD: Good value?

		private void FixRecentPoints()
		{
			// Takes 3 points to form a corner.
			if (Points.Count >= 3 && _iCornerChecked < Points.LastIndex()) {
				// Check for recent corner.
				var p1 = Points.NearEndElement(-3);
				var p2 = Points.NearEndElement(-2);
				var p3 = Points.LastElement();
				if (IsBendLarge((Vec2)p1, (Vec2)p2, (Vec2)p3, out float degreesFromStraight,
								out float midlineHeadingDegrees, out float heading01Degrees, out float heading12Degrees)) {
					// Add a new point, to have a segment within-which the bend is handled.
					// NOTE: First parameter is the point NEAR to bend.
					Dist2D joinPt = CalcJoinPoint(p2, p1, degreesFromStraight);
					// "2": Insert before p2.
					Points.InsertBeforeLastN(2, joinPt);

					// Move p2 so (p2,p3) starts outside the join area.
					// NOTE: First parameter is the point NEAR to bend.
					joinPt = CalcJoinPoint(p2, p3, degreesFromStraight);
					if (true) {
						// "1": Insert after p2.
						Points.InsertBeforeLastN(1, joinPt);
					} else {
						// "-2": replace p2.
						Points[Points.Count - 2] = joinPt;
					}
				}

				// Technically, we checked a corner at NearEndElement(-2).
				// But this matches the if-test done at start of this method.
				_iCornerChecked = Points.LastIndex();
			}
		}

		private Vec2 CalcJoinPoint(Vec2 pNearBend, Vec2 pFarFromBend, float degreesFromStraight)
		{
			return (Vec2)CalcJoinPoint((Dist2D)pNearBend, (Dist2D)pFarFromBend, degreesFromStraight);
		}

		/// <summary>
		/// </summary>
		/// <param name="pNearBend">The endpoint of segment that is NEAR to bend.</param>
		/// <param name="pFarFromBend">The endpoing of segment that is FAR from bend.</param>
		/// <returns></returns>
		private Dist2D CalcJoinPoint(Dist2D pNearBend, Dist2D pFarFromBend, float degreesFromStraight)
		{
			float segmentLength = (float)(pFarFromBend - pNearBend).Length.Value;
			// Calculate based on bend angle, so that inside of corner is zero length.
			// An imaginary mid-line between the two segments forms a right triangle
			// with angle Beta at the bend point. "/2" because that mid-line splits the angle.
			float betaDegrees = (180 - degreesFromStraight) / 2;
			// right triangle. midline is hypotenuse,
			// one triangle edge has length "a * sin(beta)", but is also known to be "width/2".
			// Solve for that "a", which is hypotenuse length.
			float betaRadians = U.AsRadians(betaDegrees);
			float midlineLength = (float)(Width / (2 * Math.Sin(betaRadians)));
			// The OTHER side of that right triangle is distance to back away from bend point,
			// such that the inside wall edges intersect.
			float backoffLength = (float)(midlineLength * Math.Cos(betaRadians));
			float joinFraction = backoffLength / segmentLength;
			// End of short join segment.
			var joinPt = U.Lerp(pNearBend, pFarFromBend, joinFraction);
			return joinPt;
		}

		private bool IsBendLarge(Vec2 p1, Vec2 p2, Vec2 p3,
										out float degreesFromStraight, out float midlineHeadingDegrees,
										out float heading01Degrees, out float heading12Degrees)
		{
			float SafeLength = 2 * (float)Width;
			// "p3 -p1" tests for shortness due to doubling back on self (which needs a greater length).
			if ((p2 - p1).Length() < SafeLength || (p3 - p2).Length() < SafeLength ||
				(p3 - p1).Length() < SafeLength) {
				degreesFromStraight = 0;
				midlineHeadingDegrees = 0;
				heading01Degrees = 0;
				heading12Degrees = 0;
				// TODO: Currently bend logic can't handle short segments.
				return false;
			}

			// In range [-180,+180]. Zero when the points are all on a straight line.
			float signedDegrees = U2.CalcBendDegrees(p1, p2, p3, out heading01Degrees, out heading12Degrees);
			//degreesFromStraight = (float)(180 - Math.Abs(signedDegrees));
			degreesFromStraight = (float)Math.Abs(signedDegrees);

			// Average the two headings. But reverse outgoing heading.
			midlineHeadingDegrees = (heading01Degrees - heading12Degrees) * 0.5f;

			bool bendIsLarge = degreesFromStraight > SmallBendDegreesLimit;
			return bendIsLarge;
		}

		internal void Flush()
		{
			if (_hasDeferredPoint) {
				_AddPointNow(_deferredPoint);
				_hasDeferredPoint = false;
			}
		}

		internal void FlushWithSmooth(Dist2D futurePoint)
		{
			if (_hasDeferredPoint) {
				if (Points.Count > 0) {
					_deferredPoint = SmoothPoint(Points[Points.LastIndex()],
												 _deferredPoint, futurePoint);
				}

				Flush();
			}
		}

		/// <summary>
		/// Smooth pt2 (based on its neighbors) to lessen ripples.
		/// TBD: Good algorithm? Limit distance moved by smooth?
		/// TBD: Fit curve through more points. Ideally do that later, so have more future points.
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <param name="pt3"></param>
		/// <returns></returns>
		private Dist2D SmoothPoint(Dist2D pt1, Dist2D pt2, Dist2D pt3)
		{
			Dist2D neighborAvg = U.Average(pt1, pt3);
			// In range [0, 1]. Larger value smooths more (moves closer to average of neighbors).
			double neighborWgt = 0.7;
			var smoothedPt2 = U.Lerp(pt2, neighborAvg, neighborWgt);
			return smoothedPt2;
		}
		#endregion

		#region --- private methods ----------------------------------------
		private Poly3D CreateAndInitPoly(StaticModel sModel)
		{
			var poly = new Poly3D();
			poly.Init(sModel, HasNormals, HasUVs, HasTangents);
			poly.TextureScale = U2.TextureScaleFor(AvaloniaSample.AvaloniaSample.BoxTexture);
			return poly;
		}

		private void CreateStartPoly(U.Pair<Vec3> wallPair0, U.Pair<Vec3> groundPair0, Terrain terrain)
		{
			if (!GroundLine.SingleGeometryTEST)
				StartPoly.Clear();
			Vec3? normal = null;
			//AddQuad(StartPoly, wallPair0, groundPair0, Poly3D.QuadVOrder.Default, ref normal, true, true);
			AddQuad(StartPoly, wallPair0, groundPair0, Poly3D.QuadVOrder.WallStart, ref normal, true, false, false);
		}

		private void CreateEndPoly(U.Pair<Vec3> wallPair1, U.Pair<Vec3> groundPair1, Terrain terrain)
		{
			if (!GroundLine.SingleGeometryTEST)
				EndPoly.Clear();
			Vec3? normal = null;
			AddQuad(EndPoly, wallPair1, groundPair1, Poly3D.QuadVOrder.WallEnd, ref normal, true, false, false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="wallPair"></param>
		/// <param name="terrain"></param>
		/// <param name="relAltitude">relative altitude: distance above terrain.</param>
		/// <returns></returns>
		private U.Pair<Vec3> ProjectToTerrain(U.Pair<Vec3> wallPair, Terrain terrain, float relAltitude)
		{
			Vec3 groundFirst = ProjectToTerrain(wallPair.First, terrain, relAltitude);
			Vec3 groundSecond = ProjectToTerrain(wallPair.Second, terrain, relAltitude);
			return new U.Pair<Vec3>(groundFirst, groundSecond);
		}

		/// <summary>
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="terrain"></param>
		/// <param name="relAltitude">relative altitude: distance above terrain.</param>
		/// <returns></returns>
		private Vec3 ProjectToTerrain(Vec3 vec, Terrain terrain, float relAltitude)
		{
			float altitude = U2.GetTerrainHeight(terrain, vec) + relAltitude;
			return U.WithAltitude(vec, altitude);
		}

		/// <summary>
		/// </summary>
		/// <param name="vec"></param>
		/// <param name="terrain"></param>
		/// <param name="relAltitude">relative altitude: distance above terrain.</param>
		/// <returns></returns>
		private Vec3 ProjectToTerrain(Vec2 vecXZ, Terrain terrain, float relAltitude)
		{
			var vec = vecXZ.AsXZ();
			float altitude = U2.GetTerrainHeight(terrain, vecXZ.AsXZ()) + relAltitude;
			return U.WithAltitude(vec, altitude);
		}

		private int _prevWallSegmentCount = 0;
		private int _currentWallSegmentCount = 0;

		/// <summary>
		/// CAUTION: No longer tells Poly3D to UpdateBufferData.
		/// Call UpdateBufferData directly, after all quads added.
		/// </summary>
		/// <param name="poly"></param>
		/// <param name="wallPair0"></param>
		/// <param name="wallPair1"></param>
		/// <param name="quadVOrder"></param>
		/// <param name="normal"></param>
		/// <param name="invertNorm"></param>
		/// <param name="invertU"></param>
		/// <param name="invertWinding"></param>
		private void AddQuad(Poly3D poly, U.Pair<Vec3> wallPair0, U.Pair<Vec3> wallPair1,
							 Poly3D.QuadVOrder quadVOrder, ref Vec3? normal,
							 bool invertNorm = false, bool invertU = false, bool invertWinding = false)
		{
			//if (!ReferenceEquals(poly, FirstSidePoly)) return;   // ttt

			if (SingleGeometryTEST)
				poly.ResetU();
			//Debug.WriteLine($"--- ({wallPair0}, {wallPair1} ---");
			//throw new NotImplementedException();

			// ">": Only add if it is a new one. (unless !AddOnlyNewQuads)
			if (_currentWallSegmentCount > _prevWallSegmentCount || !AddOnlyNewQuads) {
				poly.AddQuad(wallPair0, wallPair1, quadVOrder, ref normal, invertNorm, invertU, invertWinding, false);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="pa"></param>
		/// <param name="pb"></param>
		/// <returns>A unit vector, perpendicular to pa--pb, and lying in ground plane.</returns>
		private Vec2 CalcPerpendicularXZ(Vec3 pa, Vec3 pb)
		{
			// CAUTION: Altitude is in Y.
			return CalcPerpendicular(pa.XZ(), pb.XZ());
		}

		/// <summary>
		/// </summary>
		/// <param name="pa"></param>
		/// <param name="pb"></param>
		/// <returns>A unit vector, perpendicular to pa--pb.</returns>
		private Vec2 CalcPerpendicular(Vec2 pa, Vec2 pb)
		{
			Vec2 delta = pb - pa;
			Vec2 perpendicularUnit = U.RotateByDegrees(delta, 90);
			return perpendicularUnit.Normalized();
		}


		/// <summary>
		/// </summary>
		/// <param name="wallCenter"></param>
		/// <param name="wallWidth"></param>
		/// <param name="perpendicularUnit">REQUIRE LENGTH=1</param>
		/// <param name="wallTop">Distance above terrain</param>
		/// <param name="terrain"></param>
		/// <returns>Two points above terrain, perpendicular to wall center, "wallWidth" apart.</returns>
		private U.Pair<Vec3> WallPerpendicularOnTerrain(
                    Vec2 wallCenter, float wallWidth, Vec2 perpendicularUnit, float wallTop, Terrain terrain)
		{
			float halfWidth = wallWidth / 2.0f;
			Vec2 halfPerp = perpendicularUnit * halfWidth;
			Vec2 firstXZ = wallCenter - halfPerp;
			Vec2 secondXZ = wallCenter + halfPerp;
			return ProjectPointsToWallTop(firstXZ, secondXZ, wallTop, terrain);
		}

		private static U.Pair<Vec3> ProjectPointsToWallTop(Vec2 firstXZ, Vec2 secondXZ, float wallTop, Terrain terrain)
		{
			float firstAltitude, secondAltitude;
			if (false) {
				// This results in an "uneven top" cross-section, if ground slopes perpendicular to the wall.
				firstAltitude = U2.GetTerrainHeight(terrain, firstXZ) + wallTop;
				secondAltitude = U2.GetTerrainHeight(terrain, secondXZ) + wallTop;
			} else {
				float terrainHeight;
				if (false) {
					terrainHeight = U.Average(U2.GetTerrainHeight(terrain, firstXZ),
											  U2.GetTerrainHeight(terrain, secondXZ));
				} else {
					// To make it easier to match at bend, we take height on centerline point.
					terrainHeight = U2.GetTerrainHeight(terrain, U.Average(firstXZ, secondXZ));
				}
				float wallAltitude = terrainHeight + wallTop;
				firstAltitude = wallAltitude;
				secondAltitude = wallAltitude;
			}

			return new U.Pair<Vec3>(U2.FromXZ(firstXZ, firstAltitude),
									U2.FromXZ(secondXZ, secondAltitude));
		}
		#endregion

	}
}
