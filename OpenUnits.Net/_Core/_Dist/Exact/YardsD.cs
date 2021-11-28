using System;
using System.Collections.Generic;
using System.Text;

namespace OU.Exact.Distances
{
	/// <summary>
	/// A physical distance whose Units is Yards.
	/// E.g. "Value=3" means "3 Meters".
	/// NOTE: If DistD.DefaultUnit = Meters, can use class Distance instead of class Meters.
	/// Therefore, most libraries will probably create "Distance"s, for generality.
	/// </summary>
	public struct YardsD
	{
		public readonly static YardsD Zero = new YardsD(0);
		static private DistD.UnitsType s_Units = DistD.UnitsType.Meters;


		#region --- data, new ----------------------------------------
		public double Value;
		public DistD.UnitsType Units => s_Units;


		public YardsD(double value)
		{
			Value = value;
		}


		public override string ToString()
		{
			return "Meters = " + Value;
		}
		#endregion

		static public YardsD FromDistance(DistD dist) { return new YardsD(dist.ToUnits(DistD.UnitsType.Meters)); }
		public double InMeters => Value;
		public DistD ToDistance => DistD.FromMeters(Value);
		public double AsDefaultUnits => DistD.ConvertUnits(Value, DistD.UnitsType.Meters, DistD.DefaultUnits);

		#region === Implicit Conversion ===============================================
		static public implicit operator DistD(YardsD meters)
		{
			return meters.ToDistance;
		}
		static public implicit operator YardsD(DistD dist)
		{
			return FromDistance(dist);
		}
		#endregion === Implicit Conversion ===============================================

	}

}
