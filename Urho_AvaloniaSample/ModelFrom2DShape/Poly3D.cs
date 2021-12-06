using System;
using System.Collections.Generic;
using System.Diagnostics;
using LineLayer3D;
using SceneSource;
using Urho;
using U = OU.Utils;
using U2 = Global.Utils;

namespace ModelFrom2DShape
{
    /// <summary>
    /// TBD: Factor out Base class or helper to create VertexBuffer, IndexBuffer, and Geometry.
    /// </summary>
    public class Poly3D //: LineLayer3D.LineLayer3D
    {
		#region --- enums CwCorners, QuadRotate ----------------------------------------
		public enum CwCorner
		{
			TL = 0, TR = 1, BR = 2, BL = 3
		}
		/// <summary>
		/// To get stone texture upright, yet maintain vertex order for smoothing.
		/// </summary>
		public enum QuadVOrder
		{
			Default,
			ZigZag,
			ZigZagSwapped,
			Wall1,
			Wall2,
			WallTop,
			WallBottom,
			WallStart,
			WallEnd
		}
		#endregion

		#region --- static ----------------------------------------
		// ONE-TIME WORK.
		static Poly3D()
		{
			InitCornerLookups();
		}

		static private Vector2[] CwCornersAsUVs(CwCorner[] corners)
		{
			var vecs = new List<Vector2>(4);

			foreach (var corner in corners) {
				vecs.Add(CwCornerUVs[(int)corner]);
			}

			return vecs.ToArray();
		}

		/// <summary>
		/// Array to locate a given corner within 4 incoming vertices, based on corresponding CwCorner.
		/// </summary>
		/// <param name="corners"></param>
		/// <returns></returns>
		static private int[] CwCornersAsLookups(CwCorner[] corners)
		{
			var lookups = new int[4];

			for (int ii = 0; ii < corners.Length; ii++) {
				int i = (int)corners[ii];
				lookups[i] = ii;
			}

			// Verify.
			for (int ii2 = 0; ii2 < corners.Length; ii2++) {
				int lookup = lookups[ii2];
				CwCorner corner = corners[lookup];
				if (!((int)corner == ii2))
					throw new InvalidProgramException("CwCornerAsLookup");
			}

			// E.g. given {0: TL,  1: BL,  2: TR,  3: BR}, this is {0, 2, 3, 1},
			// which means: TL @ [0], TR @ [2], BR @ [3], BL @ [1].
			return lookups;
		}

		static private void VerifyCwCornerVecs(CwCorner[] corners, int[] lookups, Vector2[] uvs)
		{
			// Verify.
			for (int ii = 0; ii < lookups.Length; ii++) {
				int lookup = lookups[ii];
				Vector2 uv = uvs[lookup];
				Vector2 cornerVec = CwCornerUVs[ii];
				if (cornerVec != uv)
					throw new InvalidProgramException("VerifyCwCornerVecs");
			}

			for (int ii2 = 0; ii2 < lookups.Length; ii2++) {
				CwCorner corner = corners[ii2];
				Vector2 cornerVec = CwCornerUVs[(int)corner];
				Vector2 uv = uvs[ii2];
				if (cornerVec != uv)
					throw new InvalidProgramException("VerifyCwCornerVecs");
				U.Test();
			}
		}

		static private void FindCorner(int ii, CwCorner[] corners, int[] lookups, Vector3[] vertices,
										  out Vector3 vertex, out Vector2 uv)
		{
			vertex = FindCornerVertex(ii, lookups, vertices);
			uv = FindCornerUV(ii, corners);
		}

		private static Vector3 FindCornerVertex(int ii, int[] lookups, Vector3[] vertices)
		{
			Vector3 vertex;
			int lookup = lookups[ii];
			vertex = vertices[lookup];
			return vertex;
		}

		private static Vector2 FindCornerUV(int ii, CwCorner[] corners)
		{
			Vector2 uv;
			CwCorner corner = corners[ii];
			uv = CwCornerUVs[(int)corner];
			return uv;
		}

		static public Vector2[] CwCornerUVs;

		// This is in same order as CwCorner values.
		static private CwCorner[] s_Corners_Default = new[] { CwCorner.TL, CwCorner.TR, CwCorner.BR, CwCorner.BL };
		static private CwCorner[] s_Corners_ZigZag = new[] { CwCorner.TL, CwCorner.TR, CwCorner.BL, CwCorner.BR };
		static private CwCorner[] s_Corners_ZigZagSwapped = new[] { CwCorner.TR, CwCorner.TL, CwCorner.BR, CwCorner.BL };
		static private CwCorner[] s_Corners_Wall1 = new[] { CwCorner.TL, CwCorner.BL, CwCorner.TR, CwCorner.BR };
		static private CwCorner[] s_Corners_Wall2 = new[] { CwCorner.TR, CwCorner.BR, CwCorner.TL, CwCorner.BL };
		// Zig-zag, x-flipped.
		static private CwCorner[] s_Corners_WallStart = new[] { CwCorner.TR, CwCorner.TL, CwCorner.BR, CwCorner.BL };
		// Zig-zag.
		static private CwCorner[] s_Corners_WallEnd = new[] { CwCorner.TL, CwCorner.TR, CwCorner.BL, CwCorner.BR };
		// Zig-zag turned sideways.
		static private CwCorner[] s_Corners_WallTop = new[] { CwCorner.BL, CwCorner.TL, CwCorner.BR, CwCorner.TR };
		// Zig-zag turned sideways, x-flipped.
		static private CwCorner[] s_Corners_WallBottom = new[] { CwCorner.BR, CwCorner.TR, CwCorner.BL, CwCorner.TL };

		static private int[] s_LookupCorners_Default;
		static private int[] s_LookupCorners_ZigZag;
		static private int[] s_LookupCorners_ZigZagSwapped;
		static private int[] s_LookupCorners_Wall1;
		static private int[] s_LookupCorners_Wall2;
		static private int[] s_LookupCorners_WallStart;
		static private int[] s_LookupCorners_WallEnd;
		static private int[] s_LookupCorners_WallTop;
		static private int[] s_LookupCorners_WallBottom;

		static private Vector2[] s_QuadUVs_Default;
		//// TBD: No longer needed, because we re-order vertices, so quad is always in Default order.
		//static private Vector2[] s_QuadUVs_ZigZag;
		//static private Vector2[] s_QuadUVs_Wall1;
		//static private Vector2[] s_QuadUVs_Wall2;
		//static private Vector2[] s_QuadUVs_WallStart;
		//static private Vector2[] s_QuadUVs_WallEnd;

		private static void InitCornerLookups()
		{
			var uvTL = new Vector2(0, 0);
			var uvTR = new Vector2(1, 0);
			var uvBR = new Vector2(1, 1);
			var uvBL = new Vector2(0, 1);
			// Must match CWCorners. Used to calc other orders.
			CwCornerUVs = new[] { uvTL, uvTR, uvBR, uvBL };

			// Specify in clockwise order. TL=00 TR=10 BR=11 BL=01
			s_LookupCorners_Default = CwCornersAsLookups(s_Corners_Default);
			//s_QuadUVs_Default = new[] { vTL, vTR, vBR, vBL };
			s_QuadUVs_Default = CwCornersAsUVs(s_Corners_Default);
			VerifyCwCornerVecs(s_Corners_Default, s_LookupCorners_Default, new[] { uvTL, uvTR, uvBR, uvBL });

			s_LookupCorners_ZigZag = CwCornersAsLookups(s_Corners_ZigZag);
			s_LookupCorners_ZigZagSwapped = CwCornersAsLookups(s_Corners_ZigZagSwapped);
			s_LookupCorners_Wall1 = CwCornersAsLookups(s_Corners_Wall1);
			s_LookupCorners_Wall2 = CwCornersAsLookups(s_Corners_Wall2);
			s_LookupCorners_WallStart = CwCornersAsLookups(s_Corners_WallStart);
			s_LookupCorners_WallEnd = CwCornersAsLookups(s_Corners_WallEnd);
			s_LookupCorners_WallTop = CwCornersAsLookups(s_Corners_WallTop);
			s_LookupCorners_WallBottom = CwCornersAsLookups(s_Corners_WallBottom);
		}
		#endregion

		#region --- Data ----------------------------------------
		// EXPLAIN: Good values?  Maybe want to set via INVERSE of this.
		public float TextureScale = 1.0f / 4;//8;

		public BoundingBox BoundingBox {
			get {
				// TBD OPTIMIZE: Could expand as add points, so don't have to calculate from scratch.
				return UpdateBoundingBox();
			}
		}
		private BoundingBox _boundingBox;

		private VertexBuffer VBuffer;
        private IndexBuffer IBuffer;
        public Geometry Geom;
        private ElementMask ElemMask;
        private float[] VData;
        private short[] IData;
        public uint FloatsPerVertex { get; private set; }
        public bool HasNormals { get; private set; }
		public bool HasUVs { get; private set; }
		public bool HasTangents { get; private set; }
		// For current wall segment.
		public float CurrentStartU, CurrentEndU;

        private int _numQuads;
        public int NumQuads
        {
            get => _numQuads;
            private set
            {
                _numQuads = value;
                // TODO: Optimize so don't have to increase on every quad.
                EnsureVertexCapacity(NVerticesForQuads(_numQuads));
                EnsureIndexCapacity(NIndicesForQuads(_numQuads));
            }
        }

		internal bool HasContents => NumQuads > 0;

		private int _numWallSegments;
        public int NumWallSegments
        {
            get => _numWallSegments;
            set
            {
                _numWallSegments = value;
            }
        }

        // Capacity.
        private uint _numVertices;
        private uint _numIndices;
        // Contain data. Append starting here, up to capacity.
        private uint _usedVertices;
        private uint _usedVFloats;
        private uint _usedIndices;

		const uint PositionOffset = 0;   // REQUiRED to be first. (e.g. by Tangent code).
		private uint NormalOffset, UVOffset, TangentOffset;
		#endregion

		#region --- new, Init, Clear, Update.. ----------------------------------------
		public Poly3D()
        {

        }

        public void Init(UrhoObject something, bool hasNormals, bool hasUVs, bool hasTangents)
        {
            const uint FloatsForVertexNormal = 3;
            const uint FloatsForVertexUV = 2;

            VBuffer = new VertexBuffer(something.Context, false);
            IBuffer = new IndexBuffer(something.Context, false);
            Geom = new Geometry();

            var eleMask = ElementMask.Position;
            uint  floatsPerVertex = U2.FloatsForVertexPosition;

            if (hasNormals)
            {
                HasNormals = true;
				eleMask |= ElementMask.Normal;
				NormalOffset = floatsPerVertex;   // Current value.
				floatsPerVertex += U2.FloatsForVertexNormal;
            }
			if (hasUVs) {
				HasUVs = true;
				eleMask |= ElementMask.TexCoord1;
				UVOffset = floatsPerVertex;   // Current value.
				floatsPerVertex += U2.FloatsForVertexUV;
			}
			if (hasTangents) {
				HasTangents = true;
				eleMask |= ElementMask.Tangent;
				TangentOffset = floatsPerVertex;   // Current value.
				floatsPerVertex += U2.FloatsForVertexTangent;
			}
			FloatsPerVertex = floatsPerVertex;
            ElemMask = eleMask;
        }


        /// <summary>
        /// For a situation where everything needs to be recalculated.
        /// Clear buffers? Release what?
        /// </summary>
        internal void Clear(bool doTruncate = false, bool doRelease = false)
        {
            _usedVertices = 0;
            _usedVFloats = 0;
            _usedIndices = 0;
            _numQuads = 0;

			CurrentStartU = 0;

            if (doTruncate)
            {
                // TODO: Truncate arrays.
                //_numVertices = 0;
                //_numIndices = 0;
            }

            if (doRelease)
            {
                // TODO: Release what?
            }
        }

		// TBD OPTIMIZE: Could expand as add points, so don't have to calculate from scratch.
		public BoundingBox UpdateBoundingBox()
		{
			// May need this when expanding incrementally, to detect it hasn't been set.
			//bool isEmpty = _boundingBox.Max == _boundingBox.Min;

			if (NumQuads == 0 || _numVertices == 0)
				// TBD: What should bounds be?
				return _boundingBox;

			Vector3 minV, maxV;
			U.InitMinMax(out minV, out maxV);

			foreach (var vec in Positions()) {
				U.AccumMinMax(vec, ref minV, ref maxV);
			}

			_boundingBox = new BoundingBox(minV, maxV);
			//// tmstest: Does it help lighting to be larger?
			_boundingBox = new BoundingBox(minV - new Vector3(1,1,1), maxV + new Vector3(1, 1, 1));
			return _boundingBox;
		}

		internal void ResetU()
		{
			CurrentStartU = 0;
		}
		#endregion


		#region --- AddQuad, AppendVertex, Ensure..Capacity, UpdateBufferData ----------------------------------------
		/// <summary>
		/// TODO: Normals. (Also change ElemMask passed in by client.)
		/// </summary>
		/// <param name="wallPair0"></param>
		/// <param name="wallPair1"></param>
		/// <param name="quadVOrder"></param>
		/// <param name="normal"></param>
		/// <param name="invertNorm"></param>
		/// <param name="invertU"></param>
		/// <param name="invertWinding"></param>
		/// <param name="doUpdateBuffers">default true. Can be false until last quad. Or call UpdateBufferData directly.</param>
		internal void AddQuad(U.Pair<Vector3> wallPair0, U.Pair<Vector3> wallPair1, QuadVOrder quadVOrder,
							  ref Vector3? normal, bool invertNorm, bool invertU, bool invertWinding,
							  bool doUpdateBuffers = true)
        {
            NumQuads++;

			Vector3[] vertices0 = new[] {wallPair0.First, wallPair0.Second, wallPair1.First, wallPair1.Second};
			// Re-arrange to form quad. Clockwise order.
			Vector3[] verticesQ = ReorderVertices(vertices0, quadVOrder);
			var vTL = verticesQ[0];
			var vTR = verticesQ[1];
			var vBR = verticesQ[2];
			var vBL = verticesQ[3];

			// TL, TR, BL - 3 corners forming "L". Clockwise.
			// "-" EXPLAIN: Pointing out instead of pointing in. (OR I may have it reversed elsewhere.)
			Vector3 newNormal = -U.Normal(vTL, vTR, vBL);
			if (invertNorm)
				newNormal *= -1;

			Vector3 avgNormal = (normal.HasValue) ? (0.5f * (newNormal + normal.Value)) : newNormal;
            if (GroundLine.SingleGeometryTEST)
            {
                avgNormal = newNormal;   // for test, normals are not related.
            }
			//// TODO: Change logic in GroundLine, so quads come in smoothed.
   //         if (NumQuads >= 2 && !GroundLine.SingleGeometryTEST)
   //         {
   //             // Update the edge of previous quad.
   //             // REASON: Adding this quad changes calc of perpendicular between the two quads.
   // //            if (invertNorm)
			//	//{
   // //                UpdateRecentVertex(wallPair0.First, -1, 3, avgNormal);
   // //                UpdateRecentVertex(wallPair0.Second, -2, 2, avgNormal);
   // //            }
   // //            else
   //             {
   //                 UpdateRecentVertex(wallPair0.First, -2, 2, avgNormal);
   //                 UpdateRecentVertex(wallPair0.Second, -1, 3, avgNormal);
   //             }
   //         }

			float dy1 = (vBL - vTL).LengthFast;
			float dy2 = (vBR - vTR).LengthFast;
            float dy = Math.Max(dy1, dy2);
			float dx1 = (vTR - vTL).LengthFast;
			float dx2 = (vBR - vBL).LengthFast;
			float dx = Math.Max(dx1, dx2);

			float deltaU = TextureScale * dx;
			// Moving backwards in texture.
			if (invertU) {
				//deltaU = -deltaU;
				// Adjust CurrentStartU.
				CurrentStartU -= deltaU;
				// For next segment.
				CurrentEndU = CurrentStartU;
			}

			Vector2 uvScale = new Vector2(deltaU, TextureScale * dy);
			if (!invertU)
				CurrentEndU = CurrentStartU + (invertU ? -deltaU : deltaU);
			//CurrentEndU = CurrentStartU + deltaU;

			// Add vertices for quad.
			var finalNorm = avgNormal;
			//var finalNorm = invertNorm ? -avgNormal : avgNormal;
			// invertWinding also must invertNorm?
			//var finalNorm = invertNorm ^ invertWinding ? -avgNormal : avgNormal;
			if (invertWinding) {
				AppendVertex(vTR, 0, finalNorm, uvScale);
				AppendVertex(vTL, 1, finalNorm, uvScale);
				AppendVertex(vBL, 2, finalNorm, uvScale);
				AppendVertex(vBR, 3, finalNorm, uvScale);
			} else {
				AppendVertex(vTL, 0, finalNorm, uvScale);
				AppendVertex(vTR, 1, finalNorm, uvScale);
				AppendVertex(vBR, 2, finalNorm, uvScale);
				AppendVertex(vBL, 3, finalNorm, uvScale);
			}
			normal = newNormal;

            // Add indices for quad.
            AppendQuadIndices();


            uint nAddedVertices = 4;  // From above.
            _usedVertices += nAddedVertices;

			// Accumulate, for next quad.
			CurrentStartU = CurrentEndU;

			// NOTE: Only need to do this AFTER ALL quads added.
			if (doUpdateBuffers)
				UpdateBufferData();
		}

		static private int[] GetLookupCorners(QuadVOrder quadVOrder)
		{
			switch (quadVOrder) {
				case QuadVOrder.Wall1:
					return s_LookupCorners_Wall1;
				case QuadVOrder.Wall2:
					return s_LookupCorners_Wall2;
				case QuadVOrder.WallStart:
					return s_LookupCorners_WallStart;
				case QuadVOrder.WallEnd:
					return s_LookupCorners_WallEnd;
				case QuadVOrder.WallTop:
					return s_LookupCorners_WallTop;
				case QuadVOrder.WallBottom:
					return s_LookupCorners_WallBottom;
				case QuadVOrder.ZigZag:
					return s_LookupCorners_ZigZag;
				case QuadVOrder.ZigZagSwapped:
					return s_LookupCorners_ZigZagSwapped;
			}

			return s_LookupCorners_Default;
		}
		static private Vector3[] ReorderVertices(Vector3[] vertices0, QuadVOrder quadVOrder)
		{
			var lookups = GetLookupCorners(quadVOrder);

			Vector3[] vertices = new Vector3[4];
			for (int ii = 0; ii < lookups.Length; ii++) {
				var lookup = lookups[ii];
				//vertices[lookup] = vertices0[ii];
				vertices[ii] = vertices0[lookup];
			}

			return vertices;
		}

		//static private Vector2[] GetUVsPerQuad(QuadVOrder quadVOrder)
		//{
		//	switch (quadVOrder) {
		//		case QuadVOrder.Wall1:
		//			return s_QuadUVs_Wall1;
		//		case QuadVOrder.Wall2:
		//			return s_QuadUVs_Wall2;
		//	}

		//	return s_QuadUVs_Default;
		//}


		private void AppendVertex(Vector3 position, uint uvIdx, Vector3 normal, Vector2 uvScale)
        {
            if (_usedVertices >= _numVertices)
                throw new InvalidProgramException("AppendVertex - must extend capacity beforehand");

            UpdateVertex(position, uvIdx, _usedVFloats, normal, uvScale);

            _usedVFloats += FloatsPerVertex;
        }

		/// <summary>
		/// </summary>
		/// <param name="position"></param>
		/// <param name="relIndex">-1 for previous vertex, -2 for one before that.</param>
		private void UpdateRecentVertex(Vector3 position, int relIndex, uint uvIdx, Vector3 adjNormal)
        {
			return;   // TODO: No longer valid; vertices have been re-ordered to be quad.
            uint floatIndex = (uint)(_usedVFloats + (FloatsPerVertex * relIndex));
            UpdateVertex(position, uvIdx, floatIndex, adjNormal, null);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="uvIdx"></param>
		/// <param name="iVFloat"></param>
		/// <param name="normal"></param>
		/// <param name="uvScale">Used when updateUV=true</param>
		/// <param name="updateUV"></param>
        private void UpdateVertex(Vector3 position, uint uvIdx, uint iVFloat,
								  Vector3 normal, Vector2? uvScale)
        {
            VData[iVFloat++] = position.X;
            VData[iVFloat++] = position.Y;
            VData[iVFloat++] = position.Z;

            if (HasNormals)
            {
				if (true) {
					VData[iVFloat++] = normal.X;
					VData[iVFloat++] = normal.Y;
					VData[iVFloat++] = normal.Z;
				} else {   // Skip over floats for normal.
					iVFloat += 3;
				}
			}

			if (HasUVs)
            {
                if (uvScale.HasValue)
                {
					//Vector2 uv = GetUVsPerQuad(quadRotate)[uvIdx];
					// TBD: Why aren't these COUNTER-clockwise, like our culling mode?
					Vector2 uv = s_QuadUVs_Default[uvIdx];  // Clockwise; same as CwCorner.
					VData[iVFloat++] = CurrentStartU + uv.X * uvScale.Value.X;
                    VData[iVFloat++] = uv.Y * uvScale.Value.Y;
                }
                else
                {   // Skip over floats for uv.
                    iVFloat += 2;
                }
            }
        }

        private void AppendQuadIndices()
        {
            uint len = (uint)_indicesSegment.Length;

            for (uint ii = 0; ii < len; ii++)
            {
                IData[_usedIndices + ii] = (short)(_usedVertices + _indicesSegment[ii]);
            }
            _usedIndices += len;
        }

		/// <summary>
		/// Two triangles forming a quad for a line segment. Clockwise. Because our vertices are clockwise.
		/// (Even though we are doing CullMode.Ccw. Somewhere normal is flipped.)
		/// TBD: Would lighting behave better if these were Ccw?
		/// Actually, it depends which direction wall is being drawn/extended...
		/// </summary>
		protected int[] _indicesSegment = new int[] { 0, 1, 3, 1, 2, 3 };
		// CCW.
		//protected int[] _indicesSegment = new int[] { 3, 1, 0, 3, 2, 1 };


		private uint NVerticesForQuads(int numQuads)
        {
            // "4": 4 corners per quad.
            return (uint)(4 * numQuads);
        }

        private uint NIndicesForQuads(int numQuads)
        {
            // "6": 2 triangles per quad.
            return (uint)(6 * numQuads);
        }

        /// <summary>
        /// TODO: Optimize so don't have to increase on every quad.
        /// </summary>
        /// <param name="numVertices"></param>
        private void EnsureVertexCapacity(uint numVertices)
        {
            if (numVertices > _numVertices)
            {
                _numVertices = numVertices;
                VBuffer.SetSize(_numVertices, ElemMask, false);

                var nFloats = _numVertices * FloatsPerVertex;

                float[] oldData = VData;

                VData = new float[nFloats];

                if (oldData != null)
                    Array.Copy(oldData, VData, oldData.Length);

                // NO, do this AFTER filling VData with correct content.
                //VBuffer.SetData(VData);
            }
        }

		private void EnsureIndexCapacity(uint numIndices)
		{
			if (numIndices > _numIndices) {
				_numIndices = numIndices;
				IBuffer.SetSize(_numIndices, false, false);

				short[] oldIndices = IData;

				IData = new short[_numIndices];

				if (oldIndices != null)
					Array.Copy(oldIndices, IData, oldIndices.Length);

				// NO, do this AFTER filling IData with correct content.
				//IBuffer.SetData(IData);
			}
		}
		
		public void UpdateBufferData(bool bake = false)
        {
			if (!HasContents)
				return;

			if (bake)
				CalcTangents();
			//DumpData();   // tmstest

			VBuffer.SetData(VData);
            IBuffer.SetData(IData);
            Geom.SetVertexBuffer(0, VBuffer);
            Geom.IndexBuffer = IBuffer;
            Geom.SetDrawRange(PrimitiveType.TriangleList, 0, _usedIndices, false);
        }
		
		internal void CalcTangents()
		{
			if (HasTangents)
				Tangent1.GenerateTangents(VData, FloatsPerVertex, IData, 0, _numIndices, NormalOffset, UVOffset, TangentOffset);
			else
				U.DoNothing();
		}
		#endregion

		#region --- DumpData, GetVertex.., Positions ----------------------------------------
		private void DumpData()
		{
			Debug.WriteLine($"\n----- VData n={_numVertices} -----");
			for (int iVertex = 0; iVertex < _numVertices; iVertex++) {
				GetVertexData(iVertex, out Vector3 position, out Vector3 normal,
							  out Vector2 uv, out Vector4 tangent);
				Debug.WriteLine($"{position}, {normal}, {uv}, {tangent}");
			}
			Debug.WriteLine($"\n----- IData n={_numIndices} -----");
			for (int iIndex = 0; iIndex < _numIndices; iIndex++) {
				float index = IData[iIndex];
				Debug.WriteLine($"{index}");
			}
			Debug.WriteLine($"-----  -----\n");
		}

		private void GetVertexData(int iVertex, out Vector3 position, out Vector3 normal,
								   out Vector2 uv, out Vector4 tangent)
		{
			position = GetVertexPosition(iVertex);
			normal = GetVertexNormal(iVertex);
			uv = GetVertexUV(iVertex);
			tangent = GetVertexTangent(iVertex);
		}

		public Vector3 GetVertexPosition(int iVertex)
		{
			return U2.AsVector3(VData, (uint)(FloatsPerVertex * iVertex + PositionOffset));
		}

		public Vector3 GetVertexNormal(int iVertex)
		{
			if (HasNormals)
				return U2.AsVector3(VData, (uint)(FloatsPerVertex * iVertex + NormalOffset));
			else
				return new Vector3();
		}

		public Vector2 GetVertexUV(int iVertex)
		{
			if (HasUVs)
				return U2.AsVector2(VData, (uint)(FloatsPerVertex * iVertex + UVOffset));
			else
				return new Vector2();
		}

		public Vector4 GetVertexTangent(int iVertex)
		{
			if (HasTangents)
				return U2.AsVector4(VData, (uint)(FloatsPerVertex * iVertex + TangentOffset));
			else
				return new Vector4();
		}


		private IEnumerable<Vector3> Positions()
		{
			uint offset = PositionOffset;
			for (int iVertex = 0; iVertex < _numVertices; iVertex++) {
				Vector3 vec = new Vector3(
					VData[offset], VData[offset + 1], VData[offset + 2]);
				yield return vec;
				offset += FloatsPerVertex;
			}
		}

		private IEnumerable<Vector4> Tangents()
		{
			uint offset = TangentOffset;
			for (int iVertex = 0; iVertex < _numVertices; iVertex++) {
				Vector4 tan = new Vector4(
					VData[offset], VData[offset + 1], VData[offset + 2], VData[offset + 3]);
				yield return tan;
				offset += FloatsPerVertex;
			}
		}
		#endregion
	}
}
