﻿using System;
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


        public Poly3D()
        {

        }

        public void Init(UrhoObject something, ElementMask mask)
        {
            VBuffer = new VertexBuffer(something.Context, false);
            IBuffer = new IndexBuffer(something.Context, false);
            Geom = new Geometry();
            ElemMask = mask;
        }

        /// <summary>
        /// For a situation where everything needs to be recalculated.
        /// Clear buffers? Release what?
        /// </summary>
        internal void Clear()
        {
            throw new NotImplementedException("Poly3d.Clear");
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
                UpdateRecentVertex(wallPair0.First, -2);
                UpdateRecentVertex(wallPair0.Second, -1);
            }

            // Add vertices for quad.
            AppendVertex(wallPair0.First);
            AppendVertex(wallPair0.Second);
            AppendVertex(wallPair1.First);
            AppendVertex(wallPair1.Second);

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

        private void AppendVertex(Vector3 position)
        {
            if (_usedVertices >= _numVertices)
                throw new InvalidProgramException("AppendVertex - must extend capacity beforehand");

            UpdateVertex(position, _usedVFloats);

            //// TODO: Need to set normals BEFORE increment !
            _usedVFloats += (uint)NFloatsPerVertex();
        }

        /// <summary>
        /// </summary>
        /// <param name="position"></param>
        /// <param name="relIndex">-1 for previous vertex, -2 for one before that.</param>
        private void UpdateRecentVertex(Vector3 position, int relIndex)
        {
            int relVFloatIndex = relIndex * NFloatsPerVertex();
            UpdateVertex(position, (uint)(_usedVFloats + relVFloatIndex));
        }

        private void UpdateVertex(Vector3 position, uint iVFloat)
        {
            VData[iVFloat + 0] = position.X;
            VData[iVFloat + 1] = position.Y;
            VData[iVFloat + 2] = position.Z;

            if (ElemMask.HasFlag(ElementMask.Normal))
            {   // TODO
                //Vector3 normal;
                //VData[iVFloat + 3] = normal.X;
                //VData[iVFloat + 4] = normal.Y;
                //VData[iVFloat + 5] = normal.Z;
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


        private int _numQuads;
        public int NumQuads {
            get =>_numQuads;
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

                var nFloats = _numVertices * NFloatsPerVertex();

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


        const int FloatsForVertexPosition = 3;
        const int FloatsForVertexNormal = 3;

        private int NFloatsPerVertex()
        {
            return NFloatsPerVertex(ElemMask);
        }

        static private int NFloatsPerVertex(ElementMask mask)
        {
            int n = 0;

            // TBD: Any helper method to determine size given ElementMask bits?
            if (mask.HasFlag(ElementMask.Position))
                n += FloatsForVertexPosition;
            if (mask.HasFlag(ElementMask.Normal))
                n += FloatsForVertexNormal;

            return n;
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
