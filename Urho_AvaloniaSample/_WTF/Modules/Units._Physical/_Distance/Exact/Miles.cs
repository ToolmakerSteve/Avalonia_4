using System;
using System.Collections.Generic;
using System.Text;

namespace WTF.Units.Exact.Distances
{
    /// <summary>
    /// A physical distance whose Units is Miles.
    /// E.g. "Value=3" means "3 Miles".
    /// NOTE: If Distance.DefaultUnit = Miles, can use class Distance instead of class Miles.
    /// Therefore, most libraries will probably create "Distance"s, for generality.
    /// </summary>
    public struct Miles
    {
        public readonly static Miles Zero = new Miles(0);
        static private Distance.UnitsType s_Units = Distance.UnitsType.Miles;


        #region --- data, new ----------------------------------------
        public double Value;
        public Distance.UnitsType Units => s_Units;


        public Miles(double value)
        {
            Value = value;
        }


        public override string ToString()
        {
            return "Miles = " + Value;
        }
        #endregion

        static public Miles FromDistance(Distance dist) { return new Miles(dist.Meters); }
        public double InMiles => Value;
        public Distance ToDistance => Distance.FromMeters(Value);
        public double AsDefaultUnits => Distance.ConvertUnits(Value, Distance.UnitsType.Miles, Distance.DefaultUnits);

        #region === Implicit Conversion ===============================================
        static public implicit operator Distance(Miles meters)
        {
            return meters.ToDistance;
        }
        static public implicit operator Miles(Distance dist)
        {
            return FromDistance(dist);
        }
        #endregion === Implicit Conversion ===============================================

    }

}
