using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>Double-precision two-dimensional (XY) unitless vector value.   Same as a Vector2, but with double-precision.</summary>
    public struct double2
    {
        #region --- data ----------------------------------------
        public double X;
        public double Y;
        #endregion


        #region --- new ----------------------------------------
        public double2(double x, double y)
        {
            X = x;
            Y = y;
        }
        #endregion
    }
}
