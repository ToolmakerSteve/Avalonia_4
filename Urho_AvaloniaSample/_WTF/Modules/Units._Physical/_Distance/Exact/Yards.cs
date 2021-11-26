using System;
using System.Collections.Generic;
using System.Text;

namespace WTF.Units.Exact.Distances
{
    /// <summary>
    /// A physical distance whose Units is Yards.
    /// E.g. "Value=3" means "3 Meters".
    /// NOTE: If Distance.DefaultUnit = Meters, can use class Distance instead of class Meters.
    /// Therefore, most libraries will probably create "Distance"s, for generality.
    /// </summary>
    public struct Yards
    {
        public readonly static Yards Zero = new Yards(0);
        static private Distance.UnitsType s_Units = Distance.UnitsType.Meters;


        #region --- data, new ----------------------------------------
        public double Value;
        public Distance.UnitsType Units => s_Units;


        public Yards(double value)
        {
            Value = value;
        }


        public override string ToString()
        {
            return "Meters = " + Value;
        }
        #endregion

        static public Yards FromDistance(Distance dist) { return new Yards(dist.ToUnits(Distance.UnitsType.Meters)); }
        public double InMeters => Value;
        public Distance ToDistance => Distance.FromMeters(Value);
        public double AsDefaultUnits => Distance.ConvertUnits(Value, Distance.UnitsType.Meters, Distance.DefaultUnits);

        #region === Implicit Conversion ===============================================
        static public implicit operator Distance(Yards meters)
        {
            return meters.ToDistance;
        }
        static public implicit operator Yards(Distance dist)
        {
            return FromDistance(dist);
        }
        #endregion === Implicit Conversion ===============================================

    }

}
