using System;

namespace Global
{
    public static partial class Geo
    {
        public abstract class Context : IContext
        {
            public string StandardName { get; protected set; }

            public abstract Dist2D FromWGS84(Dist2D geoPt);

            public abstract Dist3D FromWGS84(Dist3D geoPt);

            public abstract Dist2D ToWGS84(Dist2D pt);

            public abstract Dist3D ToWGS84(Dist3D pt);
        }



        public class NoContext : Context
        {
            // Singleton.
            public readonly static NoContext It = new NoContext();


            public NoContext()
            {
                StandardName = "None";
            }


            public override Dist2D FromWGS84(Dist2D geoPt)
            {
                throw new NotImplementedException();
            }

            public override Dist3D FromWGS84(Dist3D geoPt)
            {
                throw new NotImplementedException();
            }

            public override Dist2D ToWGS84(Dist2D pt)
            {
                throw new NotImplementedException();
            }

            public override Dist3D ToWGS84(Dist3D pt)
            {
                throw new NotImplementedException();
            }
        }
    }
}
