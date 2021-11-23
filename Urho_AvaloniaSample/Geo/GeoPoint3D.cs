using System;
using System.Collections.Generic;
using Global;

namespace Geo
{
    public class GeoPoint3D
    {
        public Point3D Pt { get; private set; }
        public IGeoContext Context { get; set; }

        public Distance X { get => Pt.X; }
        public Distance Y { get => Pt.Y; }
        public Distance Z { get => Pt.Z; }


        public GeoPoint3D(Point3D pt, IGeoContext context)
        {
            Pt = pt;
            Context = context;
        }

        public GeoPoint3D(double x, double y, double z, IGeoContext context)
        {
            // TODO: How will GeoContext deal with units?
            Pt = new Point3D(x, y, z, null);
            Context = context;
        }


    }
}
