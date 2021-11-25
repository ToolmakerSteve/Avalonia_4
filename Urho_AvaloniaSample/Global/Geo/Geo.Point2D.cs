using System;
using System.Collections.Generic;

namespace Global
{
    public static partial class Geo
    {
        public struct Point2D
        {
            public Distance2D Pt;
            public IContext Context { get; set; }


            public Distance X { get => Pt.X; set => Pt.X = value; }
            public Distance Y { get => Pt.Y; set => Pt.Y = value; }


            public Point2D(Distance2D pt, IContext context)
            {
                Pt = pt;
                Context = context;
            }

            public Point2D(double x, double y, IContext context)
            {
                // TODO: How will GeoContext deal with units?
                Pt = new Distance2D(x, y, null);
                Context = context;
            }


        }
    }
}
