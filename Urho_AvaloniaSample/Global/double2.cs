using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// "unitless" multipliers that may be different in the three axes.
    /// See usage in Distance2D.
    /// </summary>
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

        /// <summary>
        /// For situations where the units of X and Y "cancel out".
        /// (All that matters is their "ratio".)
        /// This is often the case in trigonometric calculations.
        /// Be careful to only use where valid to do so.
        /// </summary>
        /// <param name="pt"></param>
        public double2(Distance2D pt)
        {
            X = pt.X.Value;
            Y = pt.Y.Value;
        }
        #endregion
    }
}
