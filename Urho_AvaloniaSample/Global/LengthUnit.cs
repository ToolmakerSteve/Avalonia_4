using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public class LengthUnit
    {
        /// <summary>
        /// 1:1 with "ELengthUnit".
        /// </summary>
        public static List<LengthUnit> All;

        public static LengthUnit AsLengthUnit(ELengthUnit unit)
        {
            return All[(int)unit];
        }

        public static double ToMeters(double value, ELengthUnit unit)
        {
            return All[(int)unit].ToMeters(value);
        }

        /// <summary>
        /// 1:1 with "ELengthUnit".t
        /// Each one is the number of meters for the corresponding unit.
        /// </summary>
        public static double[] LengthMults = new double[] {
            1, //Meter = 0,
            0.9144, //Yard,
            0.3048, //Foot,
            0.0254, //Inch,
            0.01, //Centimeter,
        };

        /// <summary>
        /// static - one time work.
        /// </summary>
        static LengthUnit()
        {
            All = new List<LengthUnit>(LengthMults.Length);
            foreach (double mult in LengthMults)
            {
                All.Add(new LengthUnit(mult));
            }
        }


        public readonly double Mult;

        /// <summary>
        /// "private" because REQUIRE 1:1 with ELengthUnit.
        /// </summary>
        /// <param name="meterMultiplier">aka "in meters": one of this LengthUnit is this many meters.</param>
        private LengthUnit(double meterMultiplier)
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
            return meters / Mult;
        }
    }

}
