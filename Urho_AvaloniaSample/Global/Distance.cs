using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public struct Distance
    {
        #region "-- static --"
        // TBD: Replace "const" with "static", if allow it to change at run-time.
        // HOWEVER, such a change would have to be done at a moment when NO data is in memory.
        static public EDistanceUnit DefaultEUnit { get; private set; }
        static public DistanceUnitDesc DefaultUnit { get; private set; }

        static public readonly Meters Zero = new Meters(0);


        static private bool s_initialized = false;

        static public void SetDefaultUnit(EDistanceUnit unit)
        {
            if (s_initialized)
                throw new InvalidProgramException("Distance.SetDefaultUnit called twice");
            s_initialized = true;

            DefaultEUnit = unit;
            DefaultUnit = DistanceUnitDesc.AsDistanceUnit(unit);
        }

        static Distance()
        {
            DefaultEUnit = EDistanceUnit.Meter;
            DefaultUnit = DistanceUnitDesc.AsDistanceUnit(EDistanceUnit.Meter);
        }
        #endregion


        #region "-- data, new --"
        public double Value;
        public EDistanceUnit Unit;
        public DistanceUnitDesc UnitOb => DistanceUnitDesc.AsDistanceUnit(Unit);


        public Distance(double value, EDistanceUnit unit = EDistanceUnit.Default)
        {
            Value = value;
            Unit = unit == EDistanceUnit.Default ? DefaultEUnit : unit;
        }
        #endregion


        public Meters ToMeters => new Meters(Meters);
        public double Meters => DistanceUnitDesc.ToMeters(Value, Unit);

        public double ToDefaultUnits => DistanceUnitDesc.ToDefaultUnits(Value, Unit);

        public double ToUnits(EDistanceUnit dstUnit)
        {
            return DistanceUnitDesc.ConvertUnits(Value, Unit, dstUnit);
        }

        public void SetFrom(Distance d)
        {
            if (Unit == d.Unit) {
                Value = d.Value;
            } else {
                Value = DistanceUnitDesc.ConvertUnits(d.Value, d.Unit, Unit);
            }
        }

        public void SetFromMeters(double m)
        {
            if (Unit == EDistanceUnit.Meter)
                Value = m;
            else
                Value = UnitOb.FromMeters(m);
        }

        public void SetFromDefaultUnits(double nDefault)
        {
            if (Unit == Distance.DefaultEUnit)
                Value = nDefault;
            else
                Value = UnitOb.FromDefaultUnits(nDefault);
        }
    }
}
