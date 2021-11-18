using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    public enum EUnitType
    {
        Length,   // Uses ELengthUnit; LengthUnit instances.
    }

    /// <summary>
    /// 1:1 with "LengthUnit.LengthMults".
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
