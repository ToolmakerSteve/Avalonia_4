using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	public partial struct DistD
	{
		public struct Exact
		{
			#region --- data, new ----------------------------------------
			public double Value;
			private int _unitTypeIndex;
			public UnitsType Units { get { return UnitsType.__GetByIndex(_unitTypeIndex); } }

			public Exact(double value, UnitsType units)
			{
				Value = value;
				_unitTypeIndex = units.TypeIndex;
			}
			#endregion

			public MetersD ToMeters => new MetersD(Meters);
			public double Meters => ConvertUnits(Value, Units, UnitsType.Meters);
			public double ToDefaultUnits => ConvertUnits(Value, Units, DefaultUnits);

			public double ToUnits(UnitsType dstUnit)
			{
				return ConvertUnits(Value, Units, dstUnit);
			}

			public void SetFrom(DistD d)
			{
				Value = ConvertUnits(d.Value, d.Units, Units);
			}

			public void SetFromMeters(double meters)
			{
				Value = ConvertUnits(meters, UnitsType.Meters, Units);
			}

			public void SetFromDefaultUnits(double defaultUnits)
			{
				Value = ConvertUnits(defaultUnits, DefaultUnits, Units);
			}
		}
	}
}
