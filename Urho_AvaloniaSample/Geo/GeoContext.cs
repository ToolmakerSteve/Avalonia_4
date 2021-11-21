﻿using System;
using Global;

namespace Geo
{
    public abstract class GeoContext : IGeoContext
    {
        public string StandardName { get; protected set; }

        public abstract Point2D FromWGS84(Point2D geoPt);

        public abstract Point3D FromWGS84(Point3D geoPt);

        public abstract Point2D ToWGS84(Point2D pt);

        public abstract Point3D ToWGS84(Point3D pt);
    }



    public class NoGeoContext : GeoContext
    {
        // Singleton.
        public readonly static NoGeoContext It = new NoGeoContext();


        public NoGeoContext()
        {
            StandardName = "None";
        }


        public override Point2D FromWGS84(Point2D geoPt)
        {
            throw new NotImplementedException();
        }

        public override Point3D FromWGS84(Point3D geoPt)
        {
            throw new NotImplementedException();
        }

        public override Point2D ToWGS84(Point2D pt)
        {
            throw new NotImplementedException();
        }

        public override Point3D ToWGS84(Point3D pt)
        {
            throw new NotImplementedException();
        }
    }
}
