using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public partial struct Distance
    {
        public struct Exact
        {
            #region "-- data, new --"
            public double Value;
            public EUnit Units;
            public UnitDesc UnitOb => UnitDesc.AsDistanceUnit(Units);


            public Exact(double value, EUnit units)
            {
                Value = value;
                Units = units == EUnit.Default ? DefaultUnits : units;
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
                if (Units == d.Units)
                {
                    Value = d.Value;
                }
                else
                {
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
}
