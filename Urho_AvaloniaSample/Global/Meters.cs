using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public partial struct Distance
    {
        /// <summary>
        /// A physical distance whose Units is Meters.
        /// E.g. "Value=3" means "3 Meters".
        /// NOTE: If Distance.DefaultUnit = Meters, can use class Distance instead of class Meters.
        /// Therefore, most libraries will probably create "Distance"s, for generality.
        /// </summary>
        public struct Meters
        {
            public readonly static Meters Zero = new Meters(0);
            public readonly static Meters One = new Meters(1);
            static private Distance.UnitsType s_Units = Distance.UnitsType.Meters;


            #region --- data, new ----------------------------------------
            public double Value;
            public Distance.UnitsType Units => s_Units;


            public Meters(double value)
            {
                Value = value;
            }


            public override string ToString()
            {
                return "Meters = " + Value;
            }
            #endregion

            static public Meters FromDistance(Distance dist) { return new Meters(dist.InMeters); }
            public double InMeters => Value;
            public Distance ToDistance => Distance.FromMeters(Value);
            public double AsDefaultUnits => Distance.ConvertUnits(Value, Distance.UnitsType.Meters, Distance.DefaultUnits);

            #region === Implicit Conversion ===============================================
            static public implicit operator Distance(Meters meters)
            {
                return meters.ToDistance;
            }
            static public implicit operator Meters(Distance dist)
            {
                return FromDistance(dist);
            }
            #endregion === Implicit Conversion ===============================================

        }
    }

}
