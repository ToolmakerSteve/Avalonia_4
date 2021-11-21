using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// A physical value that includes a length unit.
    /// E.g. "Value=3, Unit=Yard" means "3 Yards".
    /// </summary>
    public struct Meters
    {
        #region "-- static --"
        public const EDistanceUnit DefaultEUnit = EDistanceUnit.Meter;
        public readonly static DistanceUnitDesc DefaultUnit = DistanceUnitDesc.Meter;

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

        public Distance ToDistance => new Distance(Value, EDistanceUnit.Meter);



        public double AsDefaultUnits => Distance.DefaultEUnit == EDistanceUnit.Meter ? Value : Distance.DefaultUnit.FromMeters(Value);
        // Alternative version, if we usually have a DefaultUnit that is not meters.
        //public double AsDefaultUnits => Distance.DefaultUnit.FromMeters(Value);
    }

}
