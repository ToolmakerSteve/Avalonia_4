using System;
using System.Collections.Generic;
using System.Text;

namespace Global
{
    /// <summary>
    /// A physical distance that includes units.
    /// E.g. "Value=3, Units=Yards" means "3 Yards".
    /// </summary>
    public partial struct Distance
    {
        #region "-- static --"
        static public UnitsType DefaultUnits { get; private set; }

        // These two values are set for efficiency, assuming that Meters is our preferred/optimized unit (most common).
        //    Note, that this same efficiency can be added for other types as need arises.
        static private double _metersPerDefaultUnit;
        static private double _defaultUnitsPerMeter;

        static public readonly Distance Zero = new Distance();

        static Distance()
        {
            DefaultUnits = UnitsType.Meters;
            _metersPerDefaultUnit = DefaultUnits.MetersPerUnit;
            _defaultUnitsPerMeter = DefaultUnits.UnitsPerMeter;
        }
        static public void SetDefaultUnit(UnitsType units)
        {
            if (s_initialized)
                throw new InvalidProgramException("SetDefaultUnit called twice");
            s_initialized = true;

            if (DefaultUnits != units)
			{
                DefaultUnits = units;

                if (s_NumInstancesConstructed != 0)
                    throw new InvalidProgramException("SetDefaultUnit called after Distance instances have already been created!  Call this first: " + s_NumInstancesConstructed);
            }
        }
        static private bool s_initialized = false;
        static private long s_NumInstancesConstructed = 0;

        #endregion


        #region "--- Instance Members ---"

        /// <summary>The value of this distance, in terms of 'Units'.</summary>
        public double Value;

        /// <summary>The 'Units' of this distance instance.</summary>
        /// <remarks>Note, these Units always match Distance.DefaultUnits, set by "SetDefaultUnit()" at App Startup.</remarks>
        public UnitsType Units => DefaultUnits;

        private Distance(double value)
        {
            Value = value;
            s_NumInstancesConstructed++;
        }
        public override string ToString()
        {
            return "Dist = " + Value + " (" + Units.Abbreviation + ")";
        }

        public Meters ToMeters => new Meters(Meters);
        public double Meters => (_metersPerDefaultUnit * Value);

        public double ToUnits(UnitsType dstUnits)
        {
            return ConvertUnits(Value, Units, dstUnits);
        }
        public double ToUnitsRounded(UnitsType dstUnits)
        {
            double convertedValue = ConvertUnits(Value, Units, dstUnits);
            return Math.Round(convertedValue, 9);
        }

        public void SetFrom(Distance d)
        {
            Value = ConvertUnits(d.Value, d.Units, Units);
        }

        public void SetFromMeters(double meters)
        {
            Value = _defaultUnitsPerMeter * meters;
        }

        #endregion "--- Instance Members ---"

        #region === Static Conversion/Create Methods ===============================================

        static public Distance FromMeters(double meters)
        {
            return new Distance(_defaultUnitsPerMeter * meters);
        }
        static public Distance FromDefaultUnits(double value)
        {
            return new Distance(value);
        }
        static public Distance FromSpecifiedUnits(double value, UnitsType units)
        {
            double valueInDefaultUnits = ConvertUnits(value, units, DefaultUnits);
            return new Distance(valueInDefaultUnits);
        }

        static public double ConvertUnits(double srcValue, UnitsType srcUnit, UnitsType dstUnit)
        {
            if (srcUnit == dstUnit)
                return srcValue;

            double destValue = srcValue * srcUnit.MetersPerUnit * dstUnit.UnitsPerMeter; //  EUnits.Meters ? meters : AsDistanceUnit(dstUnit).FromMeters(meters);
            return destValue;
        }

        #endregion === Static Conversion/Create Methods ===============================================

    }
}
