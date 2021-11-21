using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public partial struct Distance
    {
        public class UnitDesc
        {
            /// <summary>
            /// 1:1 with "EUnit".
            /// </summary>
            public static List<UnitDesc> All;

            public static UnitDesc Meters => All[(int)EUnits.Meters];

            public static UnitDesc AsDistanceUnit(EUnits units)
            {
                return All[(int)units];
            }

            public static double ConvertUnits(double value, EUnits srcUnit, EUnits dstUnit)
            {
                if (srcUnit == dstUnit)
                    return value;

                double meters = srcUnit == EUnits.Meters ? value : AsDistanceUnit(srcUnit).ToMeters(value);
                return dstUnit == EUnits.Meters ? meters : AsDistanceUnit(dstUnit).FromMeters(meters);
            }

            /// <summary>
            /// 1:1 with "EUnit".
            /// Each one is the number of meters for the corresponding units.
            /// </summary>
            public static double[] MeterMults = new double[] {
            1, //Meter = 0,
            0.9144, //Yard,
            0.3048, //Foot,
            0.0254, //Inch,
            0.01, //Centimeter,
        };

            /// <summary>
            /// static - one time work.
            /// </summary>
            static UnitDesc()
            {
                All = new List<UnitDesc>(MeterMults.Length);
                foreach (double mult in MeterMults)
                {
                    All.Add(new UnitDesc(mult));
                }
            }


            public readonly double Mult;

            /// <summary>
            /// "private" because REQUIRE 1:1 with ELengthUnit.
            /// </summary>
            /// <param name="meterMultiplier">aka "in meters": one of this DistanceUnit is this many meters.</param>
            private UnitDesc(double meterMultiplier)
            {
                Mult = meterMultiplier;
            }


            /// <summary>
            /// </summary>
            /// <param name="n"></param>
            /// <returns>The number of meters corresponding to "n" of these units.</returns>
            public double ToMeters(double n)
            {
                return Mult * n;
            }

            /// <summary>
            /// </summary>
            /// <param name="meters"></param>
            /// <returns>The number of these units corresponding to "meters" meters.</returns>
            public double FromMeters(double meters)
            {
                // TBD whether this optimization is overall beneficial or counterproductive.
                //if (Mult == 1)
                //    return meters;

                return meters / Mult;
            }

            public double ToDefaultUnits(double n)
            {
                if (DefaultUnits == EUnits.Meters)
                    return Mult * n;

                return Mult * n / DefaultUnitDesc.Mult;
            }

            public double FromDefaultUnits(double nDefault)
            {
                if (DefaultUnits == EUnits.Meters)
                    return nDefault / Mult;

                return DefaultUnitDesc.Mult * nDefault / Mult;
            }
        }
    }

}
