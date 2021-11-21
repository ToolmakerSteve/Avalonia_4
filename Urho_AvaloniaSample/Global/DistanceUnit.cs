using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public class DistanceUnit
    {
        /// <summary>
        /// 1:1 with "EDistanceUnit".
        /// </summary>
        public static List<DistanceUnit> All;

        public static DistanceUnit Meter => All[(int)EDistanceUnit.Meter];

        public static DistanceUnit AsDistanceUnit(EDistanceUnit unit)
        {
            return All[(int)unit];
        }

        public static double ToMeters(double value, EDistanceUnit unit)
        {
            return All[(int)unit].ToMeters(value);
        }

        /// <summary>
        /// Equivalent to "ConvertUnits(value, unit, Distance.DefaultEUnit)".
        /// Slightly better performance if Distance.DefaultEUnit is "const".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double ToDefaultUnits(double value, EDistanceUnit unit)
        {
            if (unit == Distance.DefaultEUnit)
                return value;

            double meters = unit == EDistanceUnit.Meter ? value : AsDistanceUnit(unit).ToMeters(value);
            return Distance.DefaultEUnit == EDistanceUnit.Meter ? meters : Distance.DefaultUnit.FromMeters(meters);
        }

        public static double ConvertUnits(double value, EDistanceUnit srcUnit, EDistanceUnit dstUnit)
        {
            if (srcUnit == dstUnit)
                return value;

            double meters = srcUnit == EDistanceUnit.Meter ? value : AsDistanceUnit(srcUnit).ToMeters(value);
            return dstUnit == EDistanceUnit.Meter ? meters : AsDistanceUnit(dstUnit).FromMeters(meters);
        }

        /// <summary>
        /// 1:1 with "EDistanceUnit".
        /// Each one is the number of meters for the corresponding unit.
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
        static DistanceUnit()
        {
            All = new List<DistanceUnit>(MeterMults.Length);
            foreach (double mult in MeterMults)
            {
                All.Add(new DistanceUnit(mult));
            }
        }


        public readonly double Mult;

        /// <summary>
        /// "private" because REQUIRE 1:1 with ELengthUnit.
        /// </summary>
        /// <param name="meterMultiplier">aka "in meters": one of this DistanceUnit is this many meters.</param>
        private DistanceUnit(double meterMultiplier)
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
            if (Distance.DefaultEUnit == EDistanceUnit.Meter)
                return Mult * n;

            return Mult * n / Distance.DefaultUnit.Mult;
        }

        public double FromDefaultUnits(double nDefault)
        {
            if (Distance.DefaultEUnit == EDistanceUnit.Meter)
                return nDefault / Mult;

            return Distance.DefaultUnit.Mult * nDefault / Mult;
        }
    }

}
