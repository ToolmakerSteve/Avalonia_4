using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public enum EUnitType
    {
        Length,   // Uses EDistanceUnit; DistanceUnit instances.
    }

    /// <summary>
    /// 1:1 with "DistanceUnit.MeterMults".
    /// </summary>
    public enum EDistanceUnit
    {
        Default = -1,   // Equivalent to current Distance.DefaultEUnit.
        Meter = 0,
        Yard,
        Foot,
        Inch,
        Centimeter
    }
}
