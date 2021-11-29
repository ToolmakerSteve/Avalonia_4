using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	/// <summary>
	/// A physical distance that includes units.
	/// E.g. "Value=3, Units=Yards" means "3 Yards".
	/// IMPORTANT: All "Distance" instances are now in "DefaultUnits".
	/// Incoming values in OTHER units are CONVERTED to "DefaultUnits".
	/// REASON: MEMORY: Allows them to be a struct with a single field (instead of having a Units field on every instance).
	/// If you need to retain an EXACT value (or remember what units they were in), use type "DistD.Exact" -
	/// this has a settable "Units" property.
	/// </summary>
	public partial struct DistD
	{
		#region --- static -------------------------------------

		static public UnitsType DefaultUnits { get; private set; }

		// These two values are set for efficiency, assuming that Meters is our preferred/optimized unit (most common).
		//    Note, that this same efficiency can be added for other types as need arises.
		static private double _metersPerDefaultUnit;
		static private double _defaultUnitsPerMeter;

		static public readonly DistD Zero = new DistD();
		static public DistD OneDefaultUnit { get; private set; }


		// static constructor.
		static DistD()
		{
			//DefaultUnits = UnitsType.Meters;
			//_metersPerDefaultUnit = DefaultUnits.MetersPerUnit;
			//_defaultUnitsPerMeter = DefaultUnits.UnitsPerMeter;
			//OneDefaultUnit = DistD.FromDefaultUnits(1);

			_SetDefaultUnit(UnitsType.Meters);
		}

		static public void SetDefaultUnit(UnitsType units)
		{
			if (s_initialized)
				throw new InvalidProgramException("SetDefaultUnit called twice");
			s_initialized = true;

			if (DefaultUnits != units)
			{
				_SetDefaultUnit(units);

				if (s_InstancesHaveBeenConstructed)
					throw new InvalidProgramException("SetDefaultUnit called after Distance instances have already been created!  Call this first.");
			}
		}

		static private void _SetDefaultUnit(UnitsType units)
		{
			DefaultUnits = units;
			_metersPerDefaultUnit = units.MetersPerUnit;
			_defaultUnitsPerMeter = units.UnitsPerMeter;
			OneDefaultUnit = FromDefaultUnits(1);
		}

		static private bool s_initialized = false;
		static private bool s_InstancesHaveBeenConstructed = false;


		static public DistD Min(DistD a, DistD b)
		{
			return DistD.FromDefaultUnits(Math.Min(a.Value, b.Value));
		}

		static public DistD Max(DistD a, DistD b)
		{
			return DistD.FromDefaultUnits(Math.Max(a.Value, b.Value));
		}

		#endregion --- static -------------------------------------


		#region --- Instance Members -------------------------------------

		/// <summary>The value of this distance, in terms of 'Units'.</summary>
		public double Value;

		/// <summary>The 'Units' of this distance instance.</summary>
		/// <remarks>Note, these Units always match DistD.DefaultUnits, set by "SetDefaultUnit()" at App Startup.</remarks>
		public UnitsType Units => DefaultUnits;


		public DistD(double value)
		{
			Value = value;
			//s_NumInstancesConstructed++;
			s_InstancesHaveBeenConstructed = true;
		}

		public override string ToString()
		{
			return "Dist = " + Value + " (" + Units.Abbreviation + ")";
		}


		public MetersD ToMeters => new MetersD(Meters);
		public double Meters => (_metersPerDefaultUnit * Value);

		public double ToUnits(UnitsType dstUnits)
		{
			return ConvertUnits(Value, Units, dstUnits);
		}
		/// <summary>This is same as 'ToUnits' but will eliminate the chance of getting odd-looking results, such as "yards = 39.0000000001", or 38.99999999.
		/// It rounds to the nearest 7 decimal places, which should handle nearly all double-conversion rounding errors.</summary>
		public double ToUnitsRounded(UnitsType dstUnits)
		{
			double convertedValue = ConvertUnits(Value, Units, dstUnits);
			return Math.Round(convertedValue, 7);
		}

		public void SetFrom(DistD d)
		{
			Value = ConvertUnits(d.Value, d.Units, Units);
		}

		public void SetFromMeters(double meters)
		{
			Value = _defaultUnitsPerMeter * meters;
		}

		public DistD Abs()
		{
			return FromDefaultUnits(Math.Abs(Value));
		}

        #endregion --- Instance Members -------------------------------------


        #region === Static Conversion/Create Methods ===============================================
        static public implicit operator double(DistD it) => it.Value;
        static public explicit operator DistD(double value) => (DistD)(value);
        static public explicit operator float(DistD it) => (float)it.Value;
        static public explicit operator DistD(float value) => (DistD)(value);

        static public DistD FromMeters(double meters)
		{
			return new DistD(_defaultUnitsPerMeter * meters);
		}
		static public DistD FromDefaultUnits(double value)
		{
			return new DistD(value);
		}
		static public DistD FromSpecifiedUnits(double value, UnitsType units)
		{
			double valueInDefaultUnits = ConvertUnits(value, units, DefaultUnits);
			return new DistD(valueInDefaultUnits);
		}

		static public double ConvertUnits(double srcValue, UnitsType srcUnit, UnitsType dstUnit)
		{
			if (srcUnit == dstUnit)
				return srcValue;

			double destValue = srcValue * srcUnit.MetersPerUnit * dstUnit.UnitsPerMeter;
			return destValue;
		}
		#endregion === Static Conversion/Create Methods ===============================================


		#region --- static operators -------------------------------------

		static public bool operator ==(DistD a, DistD b)
		{
			return a.Value == b.Value;
		}

		static public bool operator !=(DistD a, DistD b)
		{
			return a.Value != b.Value;
		}

		static public DistD operator +(DistD a, DistD b)
		{
			return FromDefaultUnits(a.Value + b.Value);
		}

		static public DistD operator -(DistD a, DistD b)
		{
			return FromDefaultUnits(a.Value - b.Value);
		}

		static public DistD operator -(DistD a)
		{
			return FromDefaultUnits(-a.Value);
		}

		static public DistD operator *(DistD a, double b)
		{
			return FromDefaultUnits(a.Value * b);
		}

		static public DistD operator *(double a, DistD b)
		{
			return FromDefaultUnits(a * b.Value);
		}

		static public Squared operator *(DistD a, DistD b)
		{
			return Squared.FromDefaultUnitsSquared(a.Value * b.Value);
		}


		/// <summary>
		/// NOTE: the raw numeric value must be the divisor b (not the numerator a);
		/// it would not be meaningful to return a "Distance" for (a / (b meters)),
		/// because the result would be "inverse-meters" NOT "meters".
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public DistD operator /(DistD a, double b)
		{
			return FromDefaultUnits(a.Value / b);
		}

		/// <summary>
		/// aka "ratio" between two distances.
		/// IMPORTANT: The result is NOT A DISTANCE; it is a raw numeric value.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns>the ratio between a and b</returns>
		static public double operator /(DistD a, DistD b)
		{
			return (a.Value / b.Value);
		}

		#endregion --- static operators -------------------------------------
	}
}
