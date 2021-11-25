namespace Global
{
    public static partial class Geo
    {
        public interface IContext
        {
            public string StandardName { get; }

            public Distance3D ToWGS84(Distance3D pt);
            public Distance2D ToWGS84(Distance2D pt);
            /// <summary>
            /// Result is in meters, on the earth's surface at "altitude zero", within some "local" orthographic coordinate system.
            /// </summary>
            /// <param name="geoPt"></param>
            /// <returns></returns>
            public Distance3D FromWGS84(Distance3D geoPt);
            public Distance2D FromWGS84(Distance2D geoPt);

            // Projection+Zone.
        }
    }
}
