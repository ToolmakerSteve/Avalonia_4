using System;
using System.Collections.Generic;
using Global;

namespace Geo
{
    public struct GeoPoint2D
    {
        public Distance2D Pt;
        public IGeoContext Context { get; set; }


        public Distance X { get => Pt.X; set => Pt.X = value; }
        public Distance Y { get => Pt.Y; set => Pt.Y = value; }


        public GeoPoint2D(Distance2D pt, IGeoContext context)
        {
            Pt = pt;
            Context = context;
        }

        public GeoPoint2D(double x, double y, IGeoContext context)
        {
            // TODO: How will GeoContext deal with units?
            Pt = new Distance2D(x, y, null);
            Context = context;
        }


    }
}
