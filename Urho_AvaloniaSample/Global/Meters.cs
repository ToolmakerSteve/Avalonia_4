using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// A physical distance whose Units is Meters.
    /// E.g. "Value=3" means "3 Meters".
    /// </summary>
    public struct Meters
    {
        public readonly static Meters Zero = new Meters(0);
        static private Distance.UnitsType s_Units = Distance.UnitsType.Meters;

        #region "-- data, new --"
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

        public double AsMeters => Value;
        public Distance ToDistance => Distance.FromMeters(Value);
        public double AsDefaultUnits => Distance.ConvertUnits(Value, Distance.UnitsType.Meters, Distance.DefaultUnits);
    }

}
