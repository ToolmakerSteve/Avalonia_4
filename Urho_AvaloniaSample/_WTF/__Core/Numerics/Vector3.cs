using System;
using System.Collections.Generic;
using System.Text;

namespace WTF.Numerics
{
    // SAME AS Numerics - Vector2
    /// <summary>Double-precision two-dimensional (XY) unitless vector value.   Same as a Vector2, but with double-precision.</summary>
    public struct Vector3
    {
        public class Array : List<Vector3> // NOTE: this <Vector2> would be auto-gen'ed to the specified preferred library -- like Urho... or defaults to WTF.Numerics.Vector2
        {
        }

        #region --- data ----------------------------------------
        public float X;
        public float Y;
        #endregion


        #region --- new ----------------------------------------
        public Vector3(float x, float y)
        {
            X = x;
            Y = y;
        }
        #endregion



    }
}
