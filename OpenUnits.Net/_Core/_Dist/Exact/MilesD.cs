using System;
using System.Collections.Generic;
using System.Text;

namespace OU.Exact.Distances
{
	/// <summary>
	/// A physical distance whose Units is Miles.
	/// E.g. "Value=3" means "3 Miles".
	/// NOTE: If DistD.DefaultUnit = Miles, can use class Distance instead of class Miles.
	/// Therefore, most libraries will probably create "Distance"s, for generality.
	/// </summary>
	public struct MilesD
	{
		public readonly static MilesD Zero = new MilesD(0);
		static private DistD.UnitsType s_Units = DistD.UnitsType.Miles;


		#region --- data, new ----------------------------------------
		public double Value;
		public DistD.UnitsType Units => s_Units;


		public MilesD(double value)
		{
			Value = value;
		}


		public override string ToString()
		{
			return "Miles = " + Value;
		}
		#endregion

		static public MilesD FromDistance(DistD dist) { return new MilesD(dist.Meters); }
		public double InMiles => Value;
		public DistD ToDistance => DistD.FromMeters(Value);
		public double AsDefaultUnits => DistD.ConvertUnits(Value, DistD.UnitsType.Miles, DistD.DefaultUnits);

		#region === Implicit Conversion ===============================================
		static public implicit operator DistD(MilesD meters)
		{
			return meters.ToDistance;
		}
		static public implicit operator MilesD(DistD dist)
		{
			return FromDistance(dist);
		}
		#endregion === Implicit Conversion ===============================================

	}

}
