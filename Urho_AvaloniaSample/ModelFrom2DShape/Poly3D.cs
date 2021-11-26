using System;
using System.Collections.Generic;
using LineLayer3D;
using Urho;
using U = Global.Utils;

namespace ModelFrom2DShape
{
    /// <summary>
    /// TBD: Factor out Base class or helper to create VertexBuffer, IndexBuffer, and Geometry.
    /// </summary>
    public class Poly3D //: LineLayer3D.LineLayer3D
    {
        private VertexBuffer VBuffer;
        private IndexBuffer IBuffer;
        public Geometry Geom;
        private ElementMask ElemMask;
        private float[] VData;
        private short[] IData;
        public uint FloatsPerVertex { get; private set; }
        public bool HasNormals { get; private set; }
        public bool HasUVs { get; private set; }

        private int _numQuads;
        public int NumQuads
        {
            get => _numQuads;
            set
            {
                _numQuads = value;
                // TODO: Optimize so don't have to increase on every quad.
                EnsureVertexCapacity(NVerticesForQuads(_numQuads));
                EnsureIndexCapacity(NIndicesForQuads(_numQuads));
            }
        }

        // Capacity.
        private uint _numVertices;
        private uint _numIndices;
        // Contain data. Append starting here, up to capacity.
        private uint _usedVertices;
        private uint _usedVFloats;
        private uint _usedIndices;



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

        /// <summary>
        /// TODO: Normals. (Also change ElemMask passed in by client.)
        /// </summary>
        /// <param name="wallPair0"></param>
        /// <param name="wallPair1"></param>
        internal void AddQuad(U.Pair<Vector3> wallPair0, U.Pair<Vector3> wallPair1)
        {
            NumQuads++;

            if (NumQuads >= 2)
            {
                // Update the edge of previous quad.
                // REASON: Adding this quad changes calc of perpendicular between the two quads.
                UpdateRecentVertex(wallPair0.First, -2, 2);
                UpdateRecentVertex(wallPair0.Second, -1, 3);
            }

            // Calc normal to quad.  "-": For top of wall, this points Y up.
            Vector3 normal = -U.Normal(wallPair0.First, wallPair0.Second, wallPair1.First);
            // Add vertices for quad.
            AppendVertex(wallPair0.First, 0, normal);
            AppendVertex(wallPair0.Second, 1, normal);
            AppendVertex(wallPair1.First, 2, normal);
            AppendVertex(wallPair1.Second, 3, normal);

            // Add indices for quad.
            AppendQuadIndices();


            // TODO: Need to set normals BEFORE increment these values!
            uint nAddedVertices = 4;  // From above.
            _usedVertices += nAddedVertices;
            //_usedVFloats += (uint)NFloatsPerVertex() + nAddedVertices;

            // TODO: Do we have to do this whenever we change the data?
            // TODO: If adding many quads, should do this AFTER ALL quads added.
            UpdateBufferData();
        }

        static private Vector2[] s_UVPerQuad = new Vector2[4]
        {
            new Vector2(0,0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)
        };

        private void AppendVertex(Vector3 position, uint uvIdx, Vector3 normal)
        {
            if (_usedVertices >= _numVertices)
                throw new InvalidProgramException("AppendVertex - must extend capacity beforehand");

            UpdateVertex(position, uvIdx, _usedVFloats, normal);

            _usedVFloats += FloatsPerVertex;
        }

        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="relIndex">-1 for previous vertex, -2 for one before that.</param>
        private void UpdateRecentVertex(Vector3 position, int relIndex, uint uvIdx)
        {
            uint floatIndex = (uint)(_usedVFloats + (FloatsPerVertex * relIndex));
            UpdateVertex(position, uvIdx, floatIndex, new Vector3(), false);
        }

        private void UpdateVertex(Vector3 position, uint uvIdx, uint iVFloat, Vector3 normal, bool updateNormal = true)
        {
            VData[iVFloat++] = position.X;
            VData[iVFloat++] = position.Y;
            VData[iVFloat++] = position.Z;

            if (HasNormals)
            {
                if (updateNormal)
                {
                    VData[iVFloat++] = normal.X;
                    VData[iVFloat++] = normal.Y;
                    VData[iVFloat++] = normal.Z;
                }
                else
                {   // Skip over floats for normal.
                    iVFloat += 3;
                } 
            }

            if (HasUVs)
            {
                Vector2 uv = s_UVPerQuad[uvIdx];
                VData[iVFloat++] = uv.X;
                VData[iVFloat++] = uv.Y;
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
        protected int[] _indicesSegment = new int[] { 0, 2, 3, 3, 1, 0 };


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

    }
}
