using System;
using System.Collections.Generic;
using Global;

namespace Geo
{
    public class GeoPoint2D : IGeoPoint
    {
        public Point2D Pt { get; private set; }
        public IGeoContext Context { get; set; }
        public IPoint IValue
        {
            get => Pt;
            set
            {
                Pt = (Point2D)value;
            }
        }

        public Type ValueType => typeof(Point2D);

        public Distance X { get => Pt.X; }
        public Distance Y { get => Pt.Y; }

        public GeoPoint2D(Point2D pt, IGeoContext context)
        {
            Pt = pt;
            Context = context;
        }

        public GeoPoint2D(double x, double y, IGeoContext context)
        {
            // TODO: How will GeoContext deal with units?
            Pt = new Point2D(x, y, null);
            Context = context;
        }


    }
}
