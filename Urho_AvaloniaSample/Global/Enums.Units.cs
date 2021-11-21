using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// TBD: Someday there can be generic "PhysicalUnitType<T>" class as base to "Distance".
    /// </summary>
    public enum EPhysicalUnitType
    {
        Distance,   // Uses Distance.EUnit; DistanceUnit instances.
    }



    public partial struct Distance
    {
        /// <summary>
        /// 1:1 with "DistanceUnit.MeterMults".
        /// "Default" is a special case. Used for test cases where data is to use the current default units,
        /// without any conversion. Most code doesn't need to be written to handle it.
        /// OR handle by duplicating the UnitDesc for the default?
        /// </summary>
        public enum EUnits
        {
            Default = -1,   // Equivalent to current Distance.DefaultEUnit.
            Meters = 0,
            Yards,
            Feet,
            Inches,
            Centimeters
        }
    }
}
