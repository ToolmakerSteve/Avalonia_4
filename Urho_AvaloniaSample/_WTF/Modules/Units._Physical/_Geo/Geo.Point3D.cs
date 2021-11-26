using System;
using System.Collections.Generic;

namespace WTF.Units
{
    public static partial class Geo
    {
        public struct Point3D
        {
            public Distance3 Pt;
            public IContext Context { get; set; }


            public Distance X { get => Pt.X; }
            public Distance Y { get => Pt.Y; }
            public Distance Z { get => Pt.Z; }


            public Point3D(Distance3 pt, IContext context)
            {
                Pt = pt;
                Context = context;
            }

            public Point3D(double x, double y, double z, IContext context)
            {
                // TODO: How will GeoContext deal with units?
                Pt = new Distance3(x, y, z, null);
                Context = context;
            }


        }
    }
}
