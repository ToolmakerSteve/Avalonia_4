﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        #region --- data, new ----------------------------------------
        public Meters Width { get; set; }
        public Meters Height { get; set; }
        public Meters BaseAltitude { get; set; }

        public IGeoContext Context { get; set; }
        /// <summary>
        /// Within coord system of a scene, "float" precision is sufficient, so use Vector2 instead of Distance2D.
        /// </summary>
        public List<Vector2> Points { get; private set; }

        private float WidthMetersF => (float)Width.Value;
        private float HeightMetersF => (float)Height.Value;


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


        #region --- public methods ----------------------------------------
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
            // Within coord system of a scene, "float" precision is sufficient, so use Vector2 instead of Distance2D.
            AddPoint(new Vector2((float)pt.X.Value, (float)pt.Y.Value));
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
            // These are on wall's center line.
            Vector3 cl0 = new Vector3();
            Vector3 cl1 = new Vector3();
            Vector2 perp0 = new Vector2();
            Vector2 perp1 = new Vector2();
            foreach (Vector2 srcPt in Points)
            {
                // On wall's center line.
                Vector3 cl2 = U.PlaceOnTerrain(terrain, srcPt);
                if (firstPoint)
                {
                    cl1 = cl2;
                    // TODO: Can't calc normal yet.
                    firstPoint = false;
                }
                else
                {
                    // Make quad between cl0 and cl1.
                    if (firstPerpendicular)
                    {
                        // We had no way to compute perpendicular at pt0; now we can.
                        // TODO: Perpendicular endpoints need to also land on terrain!
                        firstPerpendicular = false;
                        perp0 = CalcPerpendicularXZ(cl0, cl1);
                    }

                    // GUESS that a good perpendicular at cl1 is tangent to the neighboring points.
                    // TBD: Adjust for relative distances to those points?
                    perp1 = CalcPerpendicularXZ(cl0, cl2);

                    AddQuad(poly, cl0, cl1, perp0, perp1, terrain);
                }

                cl0 = cl1;
                cl1 = cl2;
                perp0 = perp1;
            }
        }

        private void AddQuad(Poly3D poly, Vector3 cl0, Vector3 cl1, Vector2 perp0, Vector2 perp1, Terrain terrain)
        {
            U.Pair<Vector3> wallPair0 = WallPerpendicularOnTerrain(cl0, WidthMetersF, perp0, HeightMetersF, terrain);
            U.Pair<Vector3> wallPair1 = WallPerpendicularOnTerrain(cl1, WidthMetersF, perp1, HeightMetersF, terrain);
            AddQuad(poly, wallPair0, wallPair1);
        }

        private void AddQuad(Poly3D poly, U.Pair<Vector3> wallPair0, U.Pair<Vector3> wallPair1)
        {
            Debug.WriteLine($"--- ({wallPair0}, {wallPair1} ---");
            //throw new NotImplementedException();
        }
        #endregion


        #region --- private methods ----------------------------------------
        /// <summary>
        /// Returns a unit vector, perpendicular to pa--pb, and lying in ground plane.
        /// </summary>
        /// <param name="pa"></param>
        /// <param name="pb"></param>
        /// <returns></returns>
        private Vector2 CalcPerpendicularXZ(Vector3 pa, Vector3 pb)
        {
            // CAUTION: Altitude is in Y.
            Vector2 pa2 = pa.XZ();
            Vector2 pb2 = pb.XZ();

            Vector2 delta = pb2 - pa2;
            Vector2 perpendicularUnit = U.RotateByDegrees(delta, 90);
            perpendicularUnit.Normalize();
            return perpendicularUnit;
        }


        /// <summary>
        /// </summary>
        /// <param name="wallCenter"></param>
        /// <param name="wallWidth"></param>
        /// <param name="perpendicularUnit">REQUIRE LENGTH=1</param>
        /// <param name="wallHeight">Distance above terrain</param>
        /// <param name="terrain"></param>
        /// <returns>Two points above terrain, perpendicular to wall center, "wallWidth" apart.</returns>
        private U.Pair<Vector3> WallPerpendicularOnTerrain(
                    Vector3 wallCenter, float wallWidth, Vector2 perpendicularUnit, float wallHeight, Terrain terrain)
        {
            float halfWidth = wallWidth / 2.0f;
            Vector3 halfPerp = (perpendicularUnit * halfWidth).FromXZ();
            Vector3 first = wallCenter - halfPerp;
            Vector3 second = wallCenter + halfPerp;
            first.Y = terrain.GetHeight(first) + wallHeight;
            second.Y = terrain.GetHeight(second) + wallHeight;

            return new U.Pair<Vector3>(first, second);
        }
        #endregion

    }
}
