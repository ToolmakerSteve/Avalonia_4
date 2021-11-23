using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public partial struct Distance
    {
        /// <summary>
        /// Value has Units "Distance.DefaultUnit SQUARED".
        /// TBD: Could generalize to "Distance.ToThePower".
        /// Squared is quite common in geometric calculations, so want this to be efficient and compact.
        /// </summary>
        public struct Squared
        {
            #region --- static methods -----------------------------------------
            public static Squared FromDefaultUnitsSquared(double value)
            {
                return new Squared(value);
            }
            #endregion


            #region --- data -----------------------------------------
            /// <summary>Has Units "Distance.DefaultUnit SQUARED"</summary>
            public double Value;
            #endregion


            #region --- new -----------------------------------------
            /// <summary>
            /// private: Instead call static "FromDefaultUnitsSquared".
            /// </summary>
            /// <param name="value"></param>
            private Squared(double value)
            {
                Value = value;
            }
            #endregion


            #region --- operators -----------------------------------------
            static public Squared operator +(Squared a, Squared b)
            {
                return FromDefaultUnitsSquared(a.Value + b.Value);
            }

            static public Squared operator -(Squared a, Squared b)
            {
                return FromDefaultUnitsSquared(a.Value - b.Value);
            }

            static public Squared operator -(Squared a)
            {
                return FromDefaultUnitsSquared(-a.Value);
            }

            static public Squared operator *(Squared a, double b)
            {
                return FromDefaultUnitsSquared(a.Value * b);
            }

            static public Squared operator *(double a, Squared b)
            {
                return FromDefaultUnitsSquared(a * b.Value);
            }

            /// <summary>
            /// EXPLAIN: "units * units" / "units" => "units".
            /// That is, one of the "units" in "Squared" gets "canceled out" by the division.`
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            /// <returns></returns>
            static public Distance operator /(Squared a, Distance b)
            {
                return Distance.FromDefaultUnits(a.Value / b.Value);
            }
            #endregion


            #region --- public methods -----------------------------------------
            public Distance Sqrt()
            {
                return Distance.FromDefaultUnits(Math.Sqrt(Value));
            }
            #endregion
        }

    }
}
