using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	/// <summary>
	/// A physical distance whose Units is Meters.
	/// E.g. "Value=3" means "3 Meters".
	/// NOTE: If DistD.DefaultUnit = Meters, can use class Distance instead of class Meters.
	/// Therefore, most libraries will probably create "Distance"s, for generality.
	/// </summary>
	public struct MetersD
	{
		public readonly static MetersD Zero = new MetersD(0);
		static public readonly DistD.UnitsType UnitsType = DistD.UnitsType.Meters;


		#region --- data, new ----------------------------------------
		public double Meters;
		public DistD.UnitsType Units => UnitsType;


		public MetersD(double value)
		{
			Meters = value;
		}


		public override string ToString()
		{
			return "Meters = " + Meters;
		}
		#endregion

		static public MetersD FromDistance(DistD dist) { return new MetersD(dist.Meters); }
		public DistD ToDistance => DistD.FromMeters(Meters);
		public double ToDefaultUnits => DistD.ConvertUnits(Meters, DistD.UnitsType.Meters, DistD.DefaultUnits);

		#region === Implicit Conversion ===============================================
		static public implicit operator DistD(MetersD meters)
		{
			return meters.ToDistance;
		}
		static public implicit operator MetersD(DistD dist)
		{
			return FromDistance(dist);
		}
		#endregion === Implicit Conversion ===============================================

	}

}
