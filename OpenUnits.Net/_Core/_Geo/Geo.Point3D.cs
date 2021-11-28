using System;
using System.Collections.Generic;

namespace OU
{
	public static partial class Geo
	{
		public struct Point3D
		{
			public Dist3D Pt;
			public IContext Context { get; set; }


			public DistD X { get => Pt.X; }
			public DistD Y { get => Pt.Y; }
			public DistD Z { get => Pt.Z; }


			public Point3D(Dist3D pt, IContext context)
			{
				Pt = pt;
				Context = context;
			}

			public Point3D(double x, double y, double z, IContext context)
			{
				// TODO: How will GeoContext deal with units?
				Pt = new Dist3D(x, y, z, null);
				Context = context;
			}


		}
	}
}
