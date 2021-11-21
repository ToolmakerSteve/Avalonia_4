using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public class Distance
    {
        #region "-- static --"
        // TBD: Replace "const" with "static", if allow it to change at run-time.
        // HOWEVER, such a change would have to be done at a moment when NO data is in memory.
        public const EDistanceUnit DefaultEUnit = EDistanceUnit.Meter;
        public readonly static DistanceUnit DefaultUnit = DistanceUnit.Meter;

        public readonly static Meters Zero = new Meters(0);
        #endregion


        #region "-- data, new --"
        public double Value;
        public EDistanceUnit Unit;
        public DistanceUnit UnitOb => DistanceUnit.AsDistanceUnit(Unit);


        public Distance(double value, EDistanceUnit unit = EDistanceUnit.Default)
        {
            Value = value;
            Unit = unit == EDistanceUnit.Default ? DefaultEUnit : unit;
        }
        #endregion


        public double ToMeters => DistanceUnit.ToMeters(Value, Unit);

        public double ToDefaultUnits => DistanceUnit.ToDefaultUnits(Value, Unit);

        public double ToUnits(EDistanceUnit dstUnit)
        {
            return DistanceUnit.ConvertUnits(Value, Unit, dstUnit);
        }

        public void SetFrom(Distance d)
        {
            if (Unit == d.Unit)
                Value = d.Value;
            else
                Value = DistanceUnit.ConvertUnits(d.Value, d.Unit, Unit);
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
