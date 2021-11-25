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
        public VertexBuffer VB;
        public IndexBuffer IB;
        public Geometry Geom;

        public Poly3D()
        {

        }

        public void Init(UrhoObject something)
        {
            VB = new VertexBuffer(something.Context, false);
            IB = new IndexBuffer(something.Context, false);
            Geom = new Geometry();
        }

        /// <summary>
        /// For a situation where everything needs to be recalculated.
        /// Clear buffers? Release what?
        /// </summary>
        internal void Clear()
        {
            throw new NotImplementedException("Poly3d.Clear");
        }

        internal void AddQuad(U.Pair<Vector3> wallPair0, U.Pair<Vector3> wallPair1)
        {

        }
    }
}
