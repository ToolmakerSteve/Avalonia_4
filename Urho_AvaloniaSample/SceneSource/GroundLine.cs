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

namespace SceneSource
{
    /// <summary>
    /// Aka "Wall". A line drawn along the ground, to be converted into a 3D model.
    /// TBD: Option for Point3Ds, to represent a variable altitude above/below ground?
    /// </summary>
    public class GroundLine : SourceItem
    {
        //private static ElementMask ElemMask = ElementMask.Position | ElementMask.Normal;

        const bool AddOnlyNewQuads = true;
        public const bool SingleGeometry = false;
        public const bool SingleGeometryTEST = false;   // TMS: Temporary changes.


        #region --- data, new ----------------------------------------
        public DistD Width { get; set; }
        public DistD Height { get; set; }
        public DistD BaseAltitude { get; set; }
        public bool HasNormals { get; private set; }
        public bool HasUVs { get; private set; }

        public Geo.IContext Context { get; set; }
        /// <summary>
        /// Within coord system of a scene, "float" precision is sufficient;
        /// TBD: Make "float" version of Distance2D.
        /// </summary>
        public List<Dist2D> Points { get; private set; }
		public bool DoDeferPoint { get; set; }
		private bool HasDeferredPoint;
		private Dist2D DeferredPoint;

        private float WidthMetersF => (float)Width.Meters;
        //USE_TopMetersF private float HeightMetersF => (float)Height.Value;
        private float TopMetersF => (float)(Height + BaseAltitude).Meters;
        // Project to ground. When at 0, go slightly below ground, to avoid "crack" at base.
        private float BottomMetersF => (float)(BaseAltitude.Value == 0 ? -0.5 : BaseAltitude.Meters);

        // Top & Sides: Separate polys needed, so can adjust "previous" wall segment.
        // TBD: Start/EndPolys could be combined.
        private Poly3D TopPoly, BtmPoly, FirstSidePoly, SecondSidePoly, StartPoly, EndPoly;

        public GroundLine(bool hasUV = true, bool hasNormals = true) : this(DistD.Zero, DistD.Zero, Geo.NoContext.It, hasUV, hasNormals)
        {
        }

        /// <summary>
        /// ASSUME METERS.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="context"></param>
        public GroundLine(double width, double height, Geo.IContext context = null, bool hasUV = true, bool hasNormals = true)
                : this(new DistD(width), new DistD(height), context)
        {
        }

        public GroundLine(DistD width, DistD height, Geo.IContext context = null, bool hasUV = true, bool hasNormals = true)
        {
            Width = width;
            Height = height;
            HasUVs = hasUV;
            HasNormals = hasNormals;

            // Initialized to altitude zero.
            BaseAltitude = DistD.Zero;

            if (context == null)
                context = Geo.NoContext.It;
            Context = context;

            Points = new List<Dist2D>();
        }
        #endregion

        #region --- OnUpdate ----------------------------------------
        internal void OnUpdate()
        {
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
        }
        #endregion

        #region --- public methods ----------------------------------------
        /// <summary>
        /// ASSUMES in same Context.
        /// </summary>
        /// <param name="pt"></param>
        public void AddPoint(Dist2D pt)
        {
			FlushWithSmooth(pt);

			if (DoDeferPoint) {
				DeferredPoint = pt;
				HasDeferredPoint = true;
			} else {
				Points.Add(pt);
			}
			//Debug.WriteLine($"--- wall pt={pt.Round(3)} rel={(pt - Points[0]).Round(3)} ---");
		}

        public void AddPoint(Geo.Point2D geoPt)
        {
            if (geoPt.Context != Context)
                throw new NotImplementedException("AddPoint from a different Context");

            AddPoint(geoPt.Pt);
        }

        private Node EnsureWallNode()
        {
            var it = AvaloniaSample.AvaloniaSample.It;
            if (it.WallsNode == null)
                it.WallsNode = it.Scene.CreateChild("Walls");
            if (it.CurrentWallNode == null)
                it.CurrentWallNode = it.WallsNode.CreateChild($"Wall{it.WallsNode.GetNumChildren()+1}");

            return it.CurrentWallNode;
        }

        public StaticModel EnsureModel(Node node)
        {
            StaticModel sModel = node.GetComponent<StaticModel>();
            if (sModel == null)
            {
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
            if (TopPoly == null)
            {
                TopPoly = CreateAndInitPoly(sModel);
                if (SingleGeometry)
                {
                    // HACK as long as we have !SingleGeometry option.
                    BtmPoly = TopPoly;
                    FirstSidePoly = TopPoly;
                    SecondSidePoly = TopPoly;
                    StartPoly = TopPoly;
                    EndPoly = TopPoly;
                }
                else
                {
                    BtmPoly = CreateAndInitPoly(sModel);
                    FirstSidePoly = CreateAndInitPoly(sModel);
                    SecondSidePoly = CreateAndInitPoly(sModel);
                    StartPoly = CreateAndInitPoly(sModel);
                    EndPoly = CreateAndInitPoly(sModel);
                }
            }

            var model = sModel.Model;
            if (model == null)
            {
                model = new Model();
                model.NumGeometries = SingleGeometry ? 1 : 6;
                model.SetGeometry(0, 0, TopPoly.Geom);
                if (!SingleGeometry)
                {
                    model.SetGeometry(1, 0, BtmPoly.Geom);
                    model.SetGeometry(2, 0, FirstSidePoly.Geom);
                    model.SetGeometry(3, 0, SecondSidePoly.Geom);
                    model.SetGeometry(4, 0, StartPoly.Geom);
                    model.SetGeometry(5, 0, EndPoly.Geom);
                }
                model.BoundingBox = new BoundingBox(-10000, 10000);
                sModel.Model = model;

                var res = AvaloniaSample.AvaloniaSample.It.ResourceCache;

                //sModel.CastShadows = true;
                //Material mat = Material.FromColor(Color.Magenta, false);
                Material mat = res.GetMaterial("Materials/StoneTiledH.xml");
                mat.CullMode = CullMode.Cw; // CullMode.Cw;
											//mat.SetShaderParameter("AmbientColor", Color.White);
											//mat.PixelShaderDefines("")
				AvaloniaSample.AvaloniaSample.It.MaybeSetWireframeMaterial(mat);

				sModel.CastShadows = true;
                sModel.SetMaterial(mat);
            }
        }

        private Poly3D CreateAndInitPoly(StaticModel sModel)
        {
            var poly = new Poly3D();
            poly.Init(sModel, HasNormals, HasUVs);
            return poly;
        }

        public void CreateGeometryFromPoints(Node node, StaticModel model, Terrain terrain)
        {
            if (Points.Count < 2)
                return;

            //Debug.WriteLine("\n\n------- CreateGeometryFromPoints -------");
            EnsureAndMaybeClearGeometry(node, model);

            // TODO: TO calc good perpendicular (to give wall its width), need THREE points (except at ends).
            // TODO: Need to detect "closed shape", for good perpendicular when wraps.

            bool firstPoint = true;
            bool firstPerpendicular = true;
            // Initial values not used; avoid uninitialized warning.
            // These are on wall's center line.
            Vector3 cl0 = new Vector3();
            Vector3 cl1 = new Vector3();
            Vector2 perp0 = new Vector2();
            Vector2 perp1 = new Vector2();
            Vector3?[] normals = new Vector3?[4];

            foreach (Dist2D srcPt in Points)
            {
                // On wall's center line.
                Vector3 cl2 = Global.Utils.PlaceOnTerrain(terrain, srcPt.ToVector2(), (float)BaseAltitude.Value);
                if (firstPoint)
                {
                    cl1 = cl2;
                    // TODO: Can't calc normal yet.
                    firstPoint = false;
                }
                else
                {
                    // Make quad between cl0 and cl1.
                    if (firstPerpendicular)
                    {
                        // We had no way to compute perpendicular at pt0; now we can.
                        firstPerpendicular = false;
                        // EXPLAIN: We're only on the second point, so cl0=cl1.
                        // There is only one perpendicular possible, from that point to cl2.
                        // Put it in perp0; this will get transferred to perp0 at end of loop.
                        // So it is perp0 for NEXT iteration.
                        perp1 = CalcPerpendicularXZ(cl0, cl2);
                    }
                    else
                    {
                        // GUESS that a good perpendicular at cl1 is tangent to the neighboring points.
                        // TBD: Adjust for relative distances to those points?
                        perp1 = CalcPerpendicularXZ(cl0, cl2);

                        AddWallSegment(cl0, cl1, perp0, perp1, terrain, normals);
                    }
                }

                cl0 = cl1;
                cl1 = cl2;
                perp0 = perp1;
            }

            // Final quad.
            AddWallSegment(cl0, cl1, perp0, perp1, terrain, normals);

            if (Points.Count == 2)
            {
                // Now that we know wall direction, create StartPoly.
                //CreateStartPoly(cl0, cl1, perp0, perp1, terrain);
            }

            FinishGeometry();
        }

        private void EnsureAndMaybeClearGeometry(Node node, StaticModel model)
        {
            _currentWallSegmentCount = 0;
            EnsureGeometry(model);

            if (!AddOnlyNewQuads)
            {
                // Recreating all quads each time.
                TopPoly.Clear();
                // TODO: Other polys also.
                throw new NotImplementedException("EnsureAndMaybeClearGeometry - clear all polys");
            }
        }

        private void FinishGeometry()
        {
            _prevWallSegmentCount = _currentWallSegmentCount;
        }

        private void AddWallSegment(Vector3 cl0, Vector3 cl1, Vector2 perp0, Vector2 perp1, Terrain terrain, Vector3?[] normals)
        {
            if (SingleGeometryTEST && _currentWallSegmentCount >= 1)
                return;   // TEST - only one segment

            _currentWallSegmentCount++;

            Vector3? normTop = normals[0];
            Vector3? normBtm = normals[1];
            Vector3? normSide1 = normals[2];
            Vector3? normSide2 = normals[3];

            // On top of wall.
            U.Pair<Vector3> wallPair0 = WallPerpendicularOnTerrain(cl0, WidthMetersF, perp0, TopMetersF, terrain);
            U.Pair<Vector3> wallPair1 = WallPerpendicularOnTerrain(cl1, WidthMetersF, perp1, TopMetersF, terrain);
            // Wall Segment: Top of wall.
            AddQuad(TopPoly, wallPair0, wallPair1, ref normTop);

            U.Pair<Vector3> groundPair0 = ProjectToTerrain(wallPair0, terrain, BottomMetersF);
            U.Pair<Vector3> groundPair1 = ProjectToTerrain(wallPair1, terrain, BottomMetersF);

            // Wall Segment: Top of wall.
            AddQuad(BtmPoly, groundPair0, groundPair1, ref normBtm, true);


            // Wall Segment: First side of wall.
            // Must specify such that the second pair is at far end - these get adjusted when next quad is added.
            // Swapped order w/i each pair, to flip normal.
            U.Pair<Vector3> groundFirstSide0 = new U.Pair<Vector3>(groundPair0.First, wallPair0.First);
            U.Pair<Vector3> groundFirstSide1 = new U.Pair<Vector3>(groundPair1.First, wallPair1.First);
            AddQuad(FirstSidePoly, groundFirstSide0, groundFirstSide1, ref normSide1);

            // Wall Segment: Second side of wall.
            // Must specify such that the second pair is at far end - these get adjusted when next quad is added.
            // Swapped order w/i each pair, to flip normal.
            U.Pair<Vector3> groundSecondSide0 = new U.Pair<Vector3>(wallPair0.Second, groundPair0.Second);
            U.Pair<Vector3> groundSecondSide1 = new U.Pair<Vector3>(wallPair1.Second, groundPair1.Second);
            AddQuad(SecondSidePoly, groundSecondSide0, groundSecondSide1, ref normSide2);

            normals[0] = normTop;
            normals[1] = normBtm;
            normals[2] = normSide1;
            normals[3] = normSide2;


            if (Points.Count == 2)
            {
                // Now that we know wall direction, create StartPoly.
                CreateStartPoly(wallPair0, groundPair0, terrain);
            }
            CreateEndPoly(wallPair1, groundPair1, terrain);
        }

		internal void Flush()
		{
			if (HasDeferredPoint) {
				Points.Add(DeferredPoint);
				HasDeferredPoint = false;
			}
		}

		internal void FlushWithSmooth(Dist2D futurePoint)
		{
			if (HasDeferredPoint) {
				if (Points.Count > 0) {
					// Smooth DeferredPoint to lessen ripples.
					// TBD: Good algorithm? Limit distance moved by smooth?
					// TBD: Fit curve through more points. Ideally do that later, so have more future points.
					Dist2D neighborAvg = U.Average(Points[Points.LastIndex()], futurePoint);
					DeferredPoint = U.Lerp(DeferredPoint, neighborAvg, 0.7);
				}

				Flush();
			}
		}

		internal bool HasContents()
		{
			return Points.Count > 0 || HasDeferredPoint;
		}


		private void CreateStartPoly(U.Pair<Vector3> wallPair0, U.Pair<Vector3> groundPair0, Terrain terrain)
        {
            if (!GroundLine.SingleGeometryTEST)
                StartPoly.Clear();
            // Swapped to flip normal.
            Vector3? normal = null;
            AddQuad(StartPoly, groundPair0, wallPair0, ref normal);
        }

        private void CreateEndPoly(U.Pair<Vector3> wallPair1, U.Pair<Vector3> groundPair1, Terrain terrain)
        {
            if (!GroundLine.SingleGeometryTEST)
            EndPoly.Clear();
            Vector3? normal = null;
            AddQuad(EndPoly, wallPair1, groundPair1, ref normal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallPair"></param>
        /// <param name="terrain"></param>
        /// <param name="relAltitude">relative altitude: distance above terrain.</param>
        /// <returns></returns>
        private U.Pair<Vector3> ProjectToTerrain(U.Pair<Vector3> wallPair, Terrain terrain, float relAltitude)
        {
            Vector3 groundFirst = ProjectToTerrain(wallPair.First, terrain, relAltitude);
            Vector3 groundSecond = ProjectToTerrain(wallPair.Second, terrain, relAltitude);
            return new U.Pair<Vector3>(groundFirst, groundSecond);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="terrain"></param>
        /// <param name="relAltitude">relative altitude: distance above terrain.</param>
        /// <returns></returns>
        private Vector3 ProjectToTerrain(Vector3 vec, Terrain terrain, float relAltitude)
        {
            float altitude = terrain.GetHeight(vec) + relAltitude;
            return U.WithAltitude(vec, altitude);
        }

        private int _prevWallSegmentCount = 0;
        private int _currentWallSegmentCount = 0;

        private void AddQuad(Poly3D poly, U.Pair<Vector3> wallPair0, U.Pair<Vector3> wallPair1, ref Vector3? normal, bool invertNorm = false)
        {
            //Debug.WriteLine($"--- ({wallPair0}, {wallPair1} ---");
            //throw new NotImplementedException();

            // ">": Only add if it is a new one. (unless !AddOnlyNewQuads)
            if (_currentWallSegmentCount > _prevWallSegmentCount || !AddOnlyNewQuads)
            {
                poly.AddQuad(wallPair0, wallPair1, ref normal, invertNorm);
            }
        }
        #endregion

        #region --- private methods ----------------------------------------
        /// <summary>
        /// Returns a unit vector, perpendicular to pa--pb, and lying in ground plane.
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        private Vector2 CalcPerpendicularXZ(Vector3 pa, Vector3 pb)
        {
            // CAUTION: Altitude is in Y.
            Vector2 pa2 = pa.XZ();
            Vector2 pb2 = pb.XZ();

            Vector2 delta = pb2 - pa2;
            Vector2 perpendicularUnit = U.RotateByDegrees(delta, 90);
            perpendicularUnit.Normalize();
            return perpendicularUnit;
        }


        /// <summary>
        /// </summary>
        /// <param name="wallCenter"></param>
        /// <param name="wallWidth"></param>
        /// <param name="perpendicularUnit">REQUIRE LENGTH=1</param>
        /// <param name="wallTop">Distance above terrain</param>
        /// <param name="terrain"></param>
        /// <returns>Two points above terrain, perpendicular to wall center, "wallWidth" apart.</returns>
        private U.Pair<Vector3> WallPerpendicularOnTerrain(
                    Vector3 wallCenter, float wallWidth, Vector2 perpendicularUnit, float wallTop, Terrain terrain)
        {
            float halfWidth = wallWidth / 2.0f;
            Vector3 halfPerp = (perpendicularUnit * halfWidth).FromXZ();
            Vector3 first = wallCenter - halfPerp;
            Vector3 second = wallCenter + halfPerp;
            first.Y = terrain.GetHeight(first) + wallTop;
            second.Y = terrain.GetHeight(second) + wallTop;

            return new U.Pair<Vector3>(first, second);
        }
        #endregion

    }
}
