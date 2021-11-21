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
        #region "-- static --"
        public const Distance.EUnits DefaultEUnit = Distance.EUnits.Meters;
        public readonly static Distance.UnitDesc DefaultUnit = Distance.UnitDesc.Meters;

        public readonly static Meters Zero = new Meters(0);
        #endregion


        #region "-- data, new --"
        public double Value;


        public Meters(double value)
        {
            Value = value;
        }
        #endregion


        public double AsMeters => Value;

        public Distance ToDistance => new Distance(Value, Distance.EUnits.Meters);



        public double AsDefaultUnits => Distance.DefaultUnits == Distance.EUnits.Meters ? Value : Distance.DefaultUnitDesc.FromMeters(Value);
        // Alternative version, if we usually have a DefaultUnit that is not meters.
        //public double AsDefaultUnits => Distance.DefaultUnit.FromMeters(Value);
    }

}
