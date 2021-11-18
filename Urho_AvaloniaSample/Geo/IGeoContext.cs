using System;
using System.Collections.Generic;
using System.Text;
using Global;

namespace Geo
{
    public interface IGeoContext
    {
        public Point2D ToWGS84(Point2D pt);
        public Point2D FromWGS84(Point2D geoPt);

        // Projection+Zone.
    }
}
