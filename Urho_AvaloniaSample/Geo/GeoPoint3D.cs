using System;
using System.Collections.Generic;
using Global;

namespace Geo
{
    public class GeoPoint3D : IGeoPoint
    {
        public Point3D Pt { get; private set; }
        public IGeoContext Context { get; private set; }

        public Type ValueType => typeof(Point3D);

        public double X { get => Pt.X; }
        public double Y { get => Pt.Y; }
        public double Z { get => Pt.Z; }


        public GeoPoint3D(Point3D pt, IGeoContext context)
        {
            Pt = pt;
            Context = context;
        }

        public GeoPoint3D(double x, double y, double z, IGeoContext context)
        {
            Pt = new Point3D(x, y, z);
            Context = context;
        }


    }
}
