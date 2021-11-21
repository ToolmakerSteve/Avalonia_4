namespace Global
{
    public partial struct Distance
    {
        /// <summary>
        /// 1:1 with "DistanceUnit.MeterMults".
        /// "Default" is a special case. Used for test cases where data is to use the current default units,
        /// without any conversion. Most code doesn't need to be written to handle it.
        /// OR handle by duplicating the UnitDesc for the default?
        /// </summary>
        public partial class EUnits
        {
            static public EUnits _DEFAULT; // will equate to Meters
            static public EUnits Centimeters;
            static public EUnits Meters;
            static public EUnits Kilometers;
            static public EUnits Inches;
            static public EUnits Feet;
            static public EUnits Yards;
            static public EUnits Miles;
            static public EUnits NauticalMiles;

            static private void _InitializeDefaults()
            {
                _DEFAULT = __RegisterType("Meters", "m", 1.0); // Same as Meters
                Centimeters = __RegisterType("Centimeters", "cm", 0.01);
                Meters = __RegisterType("Meters", "m", 1.0);
                Kilometers = __RegisterType("Kilometers", "km", 1000);
                Inches = __RegisterType("Inches", "in", 0.0254);
                Feet = __RegisterType("Feet", "ft", 0.3048);
                Yards = __RegisterType("Yards", "yd", 0.9144);
                Miles = __RegisterType("Miles", "mi", 1609.344);
                NauticalMiles = __RegisterType("NauticalMiles", "nm", 1852.001376);
            }
        }
    }
}
