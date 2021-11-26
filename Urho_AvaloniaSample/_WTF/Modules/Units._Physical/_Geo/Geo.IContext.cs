namespace WTF.Units
{
    public static partial class Geo
    {
        public interface IContext
        {
            public string StandardName { get; }

            public Distance3 ToWGS84(Distance3 pt);
            public Distance2 ToWGS84(Distance2 pt);
            /// <summary>
            /// Result is in meters, on the earth's surface at "altitude zero", within some "local" orthographic coordinate system.
            /// </summary>
            /// <param name="geoPt"></param>
            /// <returns></returns>
            public Distance3 FromWGS84(Distance3 geoPt);
            public Distance2 FromWGS84(Distance2 geoPt);

            // Projection+Zone.
        }
    }
}
