using System;

namespace WTF.Units
{
    public static partial class Geo
    {
        public abstract class Context : IContext
        {
            public string StandardName { get; protected set; }

            public abstract Distance2 FromWGS84(Distance2 geoPt);

            public abstract Distance3 FromWGS84(Distance3 geoPt);

            public abstract Distance2 ToWGS84(Distance2 pt);

            public abstract Distance3 ToWGS84(Distance3 pt);
        }



        public class NoContext : Context
        {
            // Singleton.
            public readonly static NoContext It = new NoContext();


            public NoContext()
            {
                StandardName = "None";
            }


            public override Distance2 FromWGS84(Distance2 geoPt)
            {
                throw new NotImplementedException();
            }

            public override Distance3 FromWGS84(Distance3 geoPt)
            {
                throw new NotImplementedException();
            }

            public override Distance2 ToWGS84(Distance2 pt)
            {
                throw new NotImplementedException();
            }

            public override Distance3 ToWGS84(Distance3 pt)
            {
                throw new NotImplementedException();
            }
        }
    }
}
