using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// TBD: So someday there can be generic "<T>" classes as base to "Distance".
    /// </summary>
    public enum EUnitType
    {
        Distance,   // Uses Distance.EUnit; DistanceUnit instances.
    }



    public partial struct Distance
    {
        /// <summary>
        /// 1:1 with "DistanceUnit.MeterMults".
        /// </summary>
        public enum EUnit
        {
            Default = -1,   // Equivalent to current Distance.DefaultEUnit.
            Meter = 0,
            Yard,
            Foot,
            Inch,
            Centimeter
        }
    }
}
