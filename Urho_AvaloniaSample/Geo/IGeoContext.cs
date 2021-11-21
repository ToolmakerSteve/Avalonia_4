using Global;

namespace Geo
{
    public interface IGeoContext
    {
        public string StandardName { get; }

        public Point3D ToWGS84(Point3D pt);
        public Point2D ToWGS84(Point2D pt);
        /// <summary>
        /// Result is in meters, on the earth's surface at "altitude zero", within some "local" orthographic coordinate system.
        /// </summary>
        /// <param name="geoPt"></param>
        /// <returns></returns>
        public Point3D FromWGS84(Point3D geoPt);
        public Point2D FromWGS84(Point2D geoPt);

        // Projection+Zone.
    }
}
