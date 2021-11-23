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

        /// <summary>Converts from Distance2D, which drops the units and spatial-context.</summary>
        /// <remarks>For situations where the units of X and Y "cancel out" (e.g. a ratio, or scalar value).</remarks>
        public double2 FromDistance(Distance2D pt)
        {
            return new double2(pt.X.Value, pt.Y.Value);
        }
        #endregion
    }
}
