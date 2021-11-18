using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// A physical value that includes a length unit.
    /// E.g. "Value=3, Unit=Yard" means "3 Yards".
    /// </summary>
    public struct Length
    {
        public readonly double Value;
        public readonly ELengthUnit Unit;


        public Length(double value, ELengthUnit unit)
        {
            Value = value;
            Unit = unit;
        }


        public double AsMeters => LengthUnit.ToMeters(Value, Unit);
    }
}
