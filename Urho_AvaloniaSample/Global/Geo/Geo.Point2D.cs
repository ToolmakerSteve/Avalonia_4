using System;
using System.Collections.Generic;

namespace Global
{
    public static partial class Geo
    {
        public struct Point2D
        {
            public Dist2D Pt;
            public IContext Context { get; set; }


            public DistD X { get => Pt.X; set => Pt.X = value; }
            public DistD Y { get => Pt.Y; set => Pt.Y = value; }


            public Point2D(Dist2D pt, IContext context)
            {
                Pt = pt;
                Context = context;
            }

            public Point2D(double x, double y, IContext context)
            {
                // TODO: How will GeoContext deal with units?
                Pt = new Dist2D(x, y, null);
                Context = context;
            }


        }
    }
}
