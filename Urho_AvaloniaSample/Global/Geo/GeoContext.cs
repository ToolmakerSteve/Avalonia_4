using System;
using Global;

namespace Geo
{
    public abstract class GeoContext : IGeoContext
    {
        public string StandardName { get; protected set; }

        public abstract Distance2D FromWGS84(Distance2D geoPt);

        public abstract Distance3D FromWGS84(Distance3D geoPt);

        public abstract Distance2D ToWGS84(Distance2D pt);

        public abstract Distance3D ToWGS84(Distance3D pt);
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

        public override Distance3D FromWGS84(Distance3D geoPt)
        {
            throw new NotImplementedException();
        }

        public override Distance2D ToWGS84(Distance2D pt)
        {
            throw new NotImplementedException();
        }

        public override Distance3D ToWGS84(Distance3D pt)
        {
            throw new NotImplementedException();
        }
    }
}
