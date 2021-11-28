using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	public static class OU_NumericExtensions
	{
		// Number to Distance - extensions
		static public DistD asDist(this double val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDist(this float val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDist(this int val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDistD(this int val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDist(this long val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDistD(this long val) { return DistD.FromDefaultUnits(val); }

		static public DistD asMeters(this int val) { return DistD.FromMeters(val); }
	}
}
