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
        /// "Default" is a special case. Used for test cases where we want data to use the current default unit,
        /// without any conversion. Most code doesn't need to be written to handle it.
        /// OR handle by duplicating the UnitDesc for the default?
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
