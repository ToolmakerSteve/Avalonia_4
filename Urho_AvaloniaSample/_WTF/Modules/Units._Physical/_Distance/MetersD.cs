using System;
using System.Collections.Generic;
using System.Text;

namespace WTF.Units
{
    /// <summary>
    /// A physical distance whose Units is Meters.
    /// E.g. "Value=3" means "3 Meters".
    /// NOTE: If Distance.DefaultUnit = Meters, can use class Distance instead of class Meters.
    /// Therefore, most libraries will probably create "Distance"s, for generality.
    /// </summary>
    public struct MetersD
    {
        public readonly static MetersD Zero = new MetersD(0);
        static public readonly Distance.UnitsType UnitsType = Distance.UnitsType.Meters;


        #region --- data, new ----------------------------------------
        public double Meters;
        public Distance.UnitsType Units => UnitsType;


        public MetersD(double value)
        {
            Meters = value;
        }


        public override string ToString()
        {
            return "Meters = " + Meters;
        }
        #endregion

        static public MetersD FromDistance(Distance dist) { return new MetersD(dist.Meters); }
        public Distance ToDistance => Distance.FromMeters(Meters);
        public double ToDefaultUnits => Distance.ConvertUnits(Meters, Distance.UnitsType.Meters, Distance.DefaultUnits);

        #region === Implicit Conversion ===============================================
        static public implicit operator Distance(MetersD meters)
        {
            return meters.ToDistance;
        }
        static public implicit operator MetersD(Distance dist)
        {
            return FromDistance(dist);
        }
        #endregion === Implicit Conversion ===============================================

    }

}
