﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using LineLayer3D;
using SceneSource;
using Urho;
using U = OU.Utils;

namespace ModelFrom2DShape
{
    /// <summary>
    /// TBD: Factor out Base class or helper to create VertexBuffer, IndexBuffer, and Geometry.
    /// </summary>
    public class Poly3D //: LineLayer3D.LineLayer3D
    {
		#region --- Data ----------------------------------------
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
		#endregion

		#region --- new, Init, Clear, Update.. ----------------------------------------

		public Poly3D()
        {

        }

        public void Init(UrhoObject something, bool hasNormals, bool hasUVs)
        {
            const uint FloatsForVertexPosition = 3;
            const uint FloatsForVertexNormal = 3;
            const uint FloatsForVertexUV = 2;

            VBuffer = new VertexBuffer(something.Context, false);
            IBuffer = new IndexBuffer(something.Context, false);
            Geom = new Geometry();

            var eleMask = ElementMask.Position;
            uint  floatsPerVertex = FloatsForVertexPosition;

            if (hasNormals)
            {
                HasNormals = true;
                eleMask |= ElementMask.Normal;
                floatsPerVertex += FloatsForVertexNormal;
            }
            if (hasUVs)
            {
                HasUVs = true;
                eleMask |= ElementMask.TexCoord1;
                floatsPerVertex += FloatsForVertexUV;
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
			//_boundingBox = new BoundingBox(minV - new Vector3(1,1,1), maxV + new Vector3(1, 1, 1));
			return _boundingBox;
		}
		#endregion


		private float _textureScale = 1.0f / 8;//0.5f;

        /// <summary>
        /// TODO: Normals. (Also change ElemMask passed in by client.)
        /// </summary>
        /// <param name="wallPair0"></param>
        /// <param name="wallPair1"></param>
        internal void AddQuad(U.Pair<Vector3> wallPair0, U.Pair<Vector3> wallPair1, ref Vector3? normal, bool invertNorm)
        {
            NumQuads++;

            Vector3 newNormal = -U.Normal(wallPair0.First, wallPair0.Second, wallPair1.First);
            if (invertNorm)
                newNormal *= -1;

            Vector3 avgNormal = (normal.HasValue) ? (0.5f * (newNormal + normal.Value)) : newNormal;
            if (GroundLine.SingleGeometryTEST)
            {
                avgNormal = newNormal;   // for test, normals are not related.
            }
            if (NumQuads >= 2 && !GroundLine.SingleGeometryTEST)
            {
                // Update the edge of previous quad.
                // REASON: Adding this quad changes calc of perpendicular between the two quads.
                if (invertNorm)
				{
                    UpdateRecentVertex(wallPair0.First, -1, 3, avgNormal);
                    UpdateRecentVertex(wallPair0.Second, -2, 2, avgNormal);
                }
                else
                {
                    UpdateRecentVertex(wallPair0.First, -2, 2, avgNormal);
                    UpdateRecentVertex(wallPair0.Second, -1, 3, avgNormal);
                }
            }

            Vector3 vert = wallPair0.First - wallPair0.Second;
            float dy = vert.LengthFast;
            Vector3 Horiz = wallPair0.First - wallPair1.First;
            float len = Horiz.LengthFast;

			float deltaU = _textureScale * len;
			Vector2 uvScale = new Vector2(deltaU, _textureScale * dy);
			CurrentEndU = CurrentStartU + deltaU;

            // Add vertices for quad.
            if (invertNorm)
            {
                AppendVertex(wallPair0.Second, 0, avgNormal, uvScale);
                AppendVertex(wallPair0.First, 1, avgNormal, uvScale);
                AppendVertex(wallPair1.Second, 3, newNormal, uvScale);
                AppendVertex(wallPair1.First, 2, newNormal, uvScale);
            }
            else
            {
                AppendVertex(wallPair0.First, 0, avgNormal, uvScale);
                AppendVertex(wallPair0.Second, 1, avgNormal, uvScale);
                AppendVertex(wallPair1.First, 2, newNormal, uvScale);
                AppendVertex(wallPair1.Second, 3, newNormal, uvScale);
            }
            normal = newNormal;

            // Add indices for quad.
            AppendQuadIndices();


            // TODO: Need to set normals BEFORE increment these values!
            uint nAddedVertices = 4;  // From above.
            _usedVertices += nAddedVertices;
            //_usedVFloats += (uint)NFloatsPerVertex() + nAddedVertices;

            // TODO: Do we have to do this whenever we change the data?
            // TODO: If adding many quads, should do this AFTER ALL quads added.
            UpdateBufferData();
			// Accumulate, for next quad.
			CurrentStartU = CurrentEndU;

		}

        static private Vector2[] s_UVPerQuad = new Vector2[4]
        {
            new Vector2(0,0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)
        };

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
            uint floatIndex = (uint)(_usedVFloats + (FloatsPerVertex * relIndex));
            UpdateVertex(position, uvIdx, floatIndex, adjNormal, Vector2.Zero, false);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="position"></param>
		/// <param name="uvIdx"></param>
		/// <param name="iVFloat"></param>
		/// <param name="normal"></param>
		/// <param name="uvScale">Used when updateUV=true</param>
		/// <param name="updateUVAndNormals"></param>
        private void UpdateVertex(Vector3 position, uint uvIdx, uint iVFloat, Vector3 normal, Vector2 uvScale, bool updateUVAndNormals = true)
        {
            VData[iVFloat++] = position.X;
            VData[iVFloat++] = position.Y;
            VData[iVFloat++] = position.Z;

            if (HasNormals)
            {
				if (updateUVAndNormals) {
					VData[iVFloat++] = normal.X;
					VData[iVFloat++] = normal.Y;
					VData[iVFloat++] = normal.Z;
				} else {   // Skip over floats for normal.
					iVFloat += 3;
				}
			}

			if (HasUVs)
            {
                if (updateUVAndNormals)
                {
                    Vector2 uv = s_UVPerQuad[uvIdx];
                    VData[iVFloat++] = CurrentStartU + uv.X * uvScale.X;
                    VData[iVFloat++] = uv.Y * uvScale.Y;
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
		/// Two triangles forming a quad for a line segment.
		/// </summary>
		//protected int[] _indicesSegment = new int[] { 0, 2, 3, 3, 1, 0 };
		protected int[] _indicesSegment = new int[] { 3, 2, 0, 0, 1, 3 };


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

        private void UpdateBufferData()
        {
            VBuffer.SetData(VData);
            IBuffer.SetData(IData);
            Geom.SetVertexBuffer(0, VBuffer);
            Geom.IndexBuffer = IBuffer;
            Geom.SetDrawRange(PrimitiveType.TriangleList, 0, _usedIndices, false);

			DumpVData();
        }

		private void DumpVData()
		{
			Debug.WriteLine($"\n----- VData n={_numVertices} -----");
			for (int iVertex = 0; iVertex < _numVertices; iVertex++) {
				GetVertexData(iVertex, out Vector3 position, out Vector3 normal, out Vector2 uv);
				Debug.WriteLine($"{position}, {normal}, {uv}");
			}
			Debug.WriteLine($"-----  -----\n");
		}

		private void GetVertexData(int iVertex, out Vector3 position, out Vector3 normal, out Vector2 uv)
		{
			uint offset = (uint)(iVertex * FloatsPerVertex);
			position = new Vector3(VData[offset], VData[offset + 1], VData[offset + 2]);
			normal = new Vector3(VData[offset + 3], VData[offset + 4], VData[offset + 5]);
			uv = new Vector2(VData[offset + 6], VData[offset + 7]);
		}

		private void EnsureIndexCapacity(uint numIndices)
        {
            if (numIndices > _numIndices)
            {
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


		private IEnumerable<Vector3> Positions()
		{
			uint offset = 0;
			for (int iVertex = 0; iVertex < _numVertices; iVertex++) {
				Vector3 vec = new Vector3(
					VData[offset], VData[offset+1], VData[offset+2]);
				yield return vec;
				offset += FloatsPerVertex;
			}
		}
    }
}
