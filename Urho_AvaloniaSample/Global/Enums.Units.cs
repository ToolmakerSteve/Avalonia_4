using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public enum EUnitType
    {
        Length,   // Uses ELengthUnit.
    }

    /// <summary>
    /// 1:1 with "Unit.LengthMults".
    /// </summary>
    public enum ELengthUnit
    {
        Meter = 0,
        Yard,
        Foot,
        Inch,
        Centimeter
    }
}
