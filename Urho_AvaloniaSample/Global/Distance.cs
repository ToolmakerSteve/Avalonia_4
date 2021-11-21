using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// A physical distance that includes units.
    /// E.g. "Value=3, Units=Yards" means "3 Yards".
    /// </summary>
    public partial struct Distance
    {
        #region "-- static --"
        // TBD: Replace "const" with "static", if allow it to change at run-time.
        // HOWEVER, such a change would have to be done at a moment when NO data is in memory.
        static public EUnits DefaultUnits { get; private set; }
        static public UnitDesc DefaultUnitDesc { get; private set; }

        static public readonly Distance Zero = new Distance();


        static private bool s_initialized = false;

        static public void SetDefaultUnit(EUnits units)
        {
            if (s_initialized)
                throw new InvalidProgramException("SetDefaultUnit called twice");
            s_initialized = true;

            DefaultUnits = units;
            DefaultUnitDesc = UnitDesc.AsDistanceUnit(units);
        }



        public static double S_ToMeters(double value, EUnits units)
        {
            return UnitDesc.All[(int)units].ToMeters(value);
        }

        /// <summary>
        /// Equivalent to "ConvertUnits(value, units, DefaultEUnit)".
        /// Slightly better performance if DefaultEUnit is "const".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        /// <returns></returns>
        public static double S_ToDefaultUnits(double value, EUnits units)
        {
            if (units == DefaultUnits)
                return value;

            double meters = units == EUnits.Meters ? value : UnitDesc.AsDistanceUnit(units).ToMeters(value);
            return DefaultUnits == EUnits.Meters ? meters : DefaultUnitDesc.FromMeters(meters);
        }


        static Distance()
        {
            DefaultUnits = EUnits.Meters;
            DefaultUnitDesc = UnitDesc.AsDistanceUnit(EUnits.Meters);
        }
        #endregion


        #region "-- data, new --"
        public double Value;

        public EUnits Units => DefaultUnits;
        public UnitDesc UnitOb => DefaultUnitDesc;


        public Distance(double value, EUnits units)
        {
            Value = units == DefaultUnits|| units == EUnits.Default ? value : S_ToDefaultUnits(value, units);
        }
        #endregion


        public Meters ToMeters => new Meters(Meters);
        public double Meters => S_ToMeters(Value, Units);

        public double ToDefaultUnits => S_ToDefaultUnits(Value, Units);

        public double ToUnits(EUnits dstUnit)
        {
            return UnitDesc.ConvertUnits(Value, Units, dstUnit);
        }

        public void SetFrom(Distance d)
        {
            if (Units == d.Units) {
                Value = d.Value;
            } else {
                Value = UnitDesc.ConvertUnits(d.Value, d.Units, Units);
            }
        }

        public void SetFromMeters(double m)
        {
            if (Units == EUnits.Meters)
                Value = m;
            else
                Value = UnitOb.FromMeters(m);
        }

        public void SetFromDefaultUnits(double nDefault)
        {
            if (Units == DefaultUnits)
                Value = nDefault;
            else
                Value = UnitOb.FromDefaultUnits(nDefault);
        }
    }
}
