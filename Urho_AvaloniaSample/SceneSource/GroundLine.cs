using System;
using System.Collections.Generic;
using System.Text;
using Geo;
using Global;
using ModelFrom2DShape;
using Urho;
using U = Global.Utils;

namespace SceneSource
{
    /// <summary>
    /// Aka "Wall". A line drawn along the ground, to be converted into a 3D model.
    /// TBD: Option for Point3Ds, to represent a variable altitude above/below ground?
    /// </summary>
    public class GroundLine : SourceItem
    {
        // TBD: Or use a "depth bias" technique?
        const float DrawOffsetAboveTerrain = 0.1f;


        #region "-- data, new --"
        public Meters Width { get; set; }
        public Meters Height { get; set; }
        public Meters BaseAltitude { get; set; }

        public IGeoContext Context { get; set; }
        /// <summary>
        /// Within coord system of a scene, "float" precision is sufficient, so use Vector2 instead of Point2D.
        /// </summary>
        public List<Vector2> Points { get; private set; }


        public GroundLine() : this(Meters.Zero, Meters.Zero, NoGeoContext.It)
        {
        }

        /// <summary>
        /// ASSUME METERS.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="context"></param>
        public GroundLine(double width, double height, IGeoContext context = null)
                : this(new Meters(width), new Meters(height), context)
        {
        }

        public GroundLine(Meters width, Meters height, IGeoContext context = null)
        {
            Width = width;
            Height = height;
            // Initialized to altitude zero.
            BaseAltitude = Meters.Zero;

            if (context == null)
                context = NoGeoContext.It;
            Context = context;

            Points = new List<Vector2>();
        }
        #endregion


        #region "-- public methods --"
        /// <summary>
        /// ASSUMES in same Context.
        /// </summary>
        /// <param name="pt"></param>
        public void AddPoint(Vector2 pt)
        {
            Points.Add(pt);
        }

        public void AddPoint(GeoPoint2D geoPt)
        {
            if (geoPt.Context != Context)
                throw new NotImplementedException("AddPoint from a different Context");

            var pt = geoPt.Pt;
            // Within coord system of a scene, "float" precision is sufficient, so use Vector2 instead of Point2D.
            AddPoint(new Vector2((float)pt.X, (float)pt.Y));
        }

        public StaticModel AsModelIn(Scene scene, Terrain terrain)
        {
            if (Points.Count == 0)
                return null;

            var node = scene.CreateChild("Wall");
            StaticModel model = node.CreateComponent<StaticModel>();

            model.CastShadows = true;
            Material mat = Material.FromColor(Color.Magenta);   // TODO
            model.SetMaterial(mat);

            CreateGeometryFromPoints(model, terrain);

            return model;
        }

        public void CreateGeometryFromPoints(StaticModel model, Terrain terrain)
        {
            var poly = new Poly3D();

            // TODO: TO calc good perpendicular (to give wall its width), need THREE points (except at ends).
            // TODO: Need to detect "closed shape", for good perpendicular when wraps.

            bool firstPoint = true;
            bool firstPerpendicular = true;
            // Initial values not used; avoid uninitialized warning.
            Vector3 pt0 = new Vector3();
            Vector3 pt1 = new Vector3();
            Vector3 perp1 = new Vector3();
            foreach (var srcPt in Points)
            {
                Vector3 pt2CenterLine = U.PlaceOnTerrain(terrain, srcPt);
                if (firstPoint)
                {
                    pt1 = pt2CenterLine;
                    // TODO: Can't calc normal yet.
                    firstPoint = false;
                }
                else
                {
                    // Make quad between pt0 and pt1.
                    if (firstPerpendicular)
                    {
                        // We had no way to compute perpendicular at first pt0; now we can.
                        // TODO: Perpendicular endpoints need to also land on terrain!
                        firstPerpendicular = false;
                    }
                }

                pt0 = pt1;
                pt1 = pt2CenterLine;
            }
        }
        #endregion


        #region "-- private methods --"
        /// <summary>
        /// Returns a unit vector, perpendicular to pa--pb, and lying in ground plane.
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        //private Vector2 CalcPerpendicular2D(Vector3 pa, Vector3 pb)
        //{
        //    // CAUTION: Altitude is in Y.
        //    Vector2 pa2 = pa.XZ();
        //    Vector2 pb2 = pb.XZ();

        //}


        /// <summary>
        /// </summary>
        /// <param name="wallCenter"></param>
        /// <param name="wallWidth"></param>
        /// <param name="perpendicularUnit">REQUIRE LENGTH=1</param>
        /// <param name="terrain"></param>
        /// <returns>Two points on terrain, perpendicular to wall center, "wallWidth" apart.</returns>
        private U.Pair<Vector3> WallPerpendicularOnTerrain(
                    Vector3 wallCenter, float wallWidth, Vector2 perpendicularUnit, Terrain terrain)
        {
            float halfWidth = wallWidth / 2.0f;
            Vector3 halfPerp = (perpendicularUnit * halfWidth).FromXZ();
            Vector3 first = wallCenter - halfPerp;
            Vector3 second = wallCenter + halfPerp;
            first.Y = terrain.GetHeight(first);
            second.Y = terrain.GetHeight(second);

            return new U.Pair<Vector3>(first, second);
        }
        #endregion

    }
}
