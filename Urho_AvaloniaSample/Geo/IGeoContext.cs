using Global;

namespace Geo
{
    public interface IGeoContext
    {
        public string StandardName { get; }

        public Point3D ToWGS84(Point3D pt);
        public Distance2D ToWGS84(Distance2D pt);
        /// <summary>
        /// Result is in meters, on the earth's surface at "altitude zero", within some "local" orthographic coordinate system.
        /// </summary>
        /// <param name="geoPt"></param>
        /// <returns></returns>
        public Point3D FromWGS84(Point3D geoPt);
        public Distance2D FromWGS84(Distance2D geoPt);

        // Projection+Zone.
    }
}
