using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// "unitless" multipliers that may be different in the three axes.
    /// See usage in Point3D.
    /// </summary>
    public struct Unitless3D
    {
        #region --- data ----------------------------------------
        public double X;
        public double Y;
        public double Z;
        #endregion


        #region --- new ----------------------------------------
        public Unitless3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// For situations where the units of X and Y "cancel out".
        /// (All that matters is their "ratio".)
        /// This is often the case in trigonometric calculations.
        /// Be careful to only use where valid to do so.
        /// </summary>
        /// <param name="pt"></param>
        public Unitless3D(Point3D pt)
        {
            X = pt.X.Value;
            Y = pt.Y.Value;
            Z = pt.Z.Value;
        }
        #endregion
    }
}
