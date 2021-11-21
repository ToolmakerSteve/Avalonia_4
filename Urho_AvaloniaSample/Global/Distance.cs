using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public partial struct Distance
    {
        #region "-- static --"
        // TBD: Replace "const" with "static", if allow it to change at run-time.
        // HOWEVER, such a change would have to be done at a moment when NO data is in memory.
        static public EUnit DefaultUnits { get; private set; }
        static public UnitDesc DefaultUnitDesc { get; private set; }

        static public readonly Distance Zero = new Distance();


        static private bool s_initialized = false;

        static public void SetDefaultUnit(EUnit unit)
        {
            if (s_initialized)
                throw new InvalidProgramException("SetDefaultUnit called twice");
            s_initialized = true;

            DefaultUnits = unit;
            DefaultUnitDesc = UnitDesc.AsDistanceUnit(unit);
        }



        public static double S_ToMeters(double value, EUnit unit)
        {
            return UnitDesc.All[(int)unit].ToMeters(value);
        }

        /// <summary>
        /// Equivalent to "ConvertUnits(value, unit, DefaultEUnit)".
        /// Slightly better performance if DefaultEUnit is "const".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double S_ToDefaultUnits(double value, EUnit unit)
        {
            if (unit == DefaultUnits)
                return value;

            double meters = unit == EUnit.Meter ? value : UnitDesc.AsDistanceUnit(unit).ToMeters(value);
            return DefaultUnits == EUnit.Meter ? meters : DefaultUnitDesc.FromMeters(meters);
        }


        static Distance()
        {
            DefaultUnits = EUnit.Meter;
            DefaultUnitDesc = UnitDesc.AsDistanceUnit(EUnit.Meter);
        }
        #endregion


        #region "-- data, new --"
        public double Value;

        public EUnit Units => DefaultUnits;
        public UnitDesc UnitOb => DefaultUnitDesc;


        /// <summary>
        /// Exact must specify.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units"></param>
        public Distance(double value, EUnit units)
        {
            Value = units == DefaultUnits ? value : S_ToDefaultUnits(value, units);
        }
        #endregion


        public Meters ToMeters => new Meters(Meters);
        public double Meters => S_ToMeters(Value, Units);

        public double ToDefaultUnits => S_ToDefaultUnits(Value, Units);

        public double ToUnits(EUnit dstUnit)
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
            if (Units == EUnit.Meter)
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
