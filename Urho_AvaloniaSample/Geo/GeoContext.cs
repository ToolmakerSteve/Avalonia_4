using System;
using Global;

namespace Geo
{
    public abstract class GeoContext : IGeoContext
    {
        public string StandardName { get; protected set; }

        public abstract Distance2D FromWGS84(Distance2D geoPt);

        public abstract Point3D FromWGS84(Point3D geoPt);

        public abstract Distance2D ToWGS84(Distance2D pt);

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


        public override Distance2D FromWGS84(Distance2D geoPt)
        {
            throw new NotImplementedException();
        }

        public override Point3D FromWGS84(Point3D geoPt)
        {
            throw new NotImplementedException();
        }

        public override Distance2D ToWGS84(Distance2D pt)
        {
            throw new NotImplementedException();
        }

        public override Point3D ToWGS84(Point3D pt)
        {
            throw new NotImplementedException();
        }
    }
}
