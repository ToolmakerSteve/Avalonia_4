using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
        private static ElementMask ElemMask = ElementMask.Position;
        //private static ElementMask ElemMask = ElementMask.Position | ElementMask.Normal;

        const bool Test_BoxPerWallSegment = false;//false;
        const bool AddOnlyNewQuads = true;



        #region --- data, new ----------------------------------------
        public Meters Width { get; set; }
        public Meters Height { get; set; }
        public Meters BaseAltitude { get; set; }

        public Geo.IContext Context { get; set; }
        /// <summary>
        /// Within coord system of a scene, "float" precision is sufficient;
        /// TBD: Make "float" version of Distance2D.
        /// </summary>
        public List<Distance2D> Points { get; private set; }

        private float WidthMetersF => (float)Width.Value;
        private float HeightMetersF => (float)Height.Value;

        private Poly3D Poly;

        public GroundLine() : this(Meters.Zero, Meters.Zero, Geo.NoContext.It)
        {
        }

        /// <summary>
        /// ASSUME METERS.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="context"></param>
        public GroundLine(double width, double height, Geo.IContext context = null)
                : this(new Meters(width), new Meters(height), context)
        {
        }

        public GroundLine(Meters width, Meters height, Geo.IContext context = null)
        {
            Width = new Meters(width.Value * 10);
            Height = height;
            // Initialized to altitude zero.
            BaseAltitude = Meters.Zero;

            if (context == null)
                context = Geo.NoContext.It;
            Context = context;

            Points = new List<Distance2D>();
        }
        #endregion


        #region --- OnUpdate ----------------------------------------
        internal void OnUpdate()
        {
            // TBD: Ideally, add some representation when there is only one point.
            // Given that we don't know what direction wall will go in,
            // this will have to represent "a point".
            // It could have the height and width of the wall. "Cylinder" would be ideal. "Rectangular Prism" would be okay.
            if (Points.Count < 2)
                return;

            Node wallNode = EnsureWallNode();
            StaticModel wall = EnsureModel(wallNode);

            var it = AvaloniaSample.AvaloniaSample.It;
            CreateGeometryFromPoints(wallNode, wall, it.Terrain);
        }
        #endregion

        #region --- public methods ----------------------------------------
        /// <summary>
        /// ASSUMES in same Context.
        /// </summary>
        /// <param name="pt"></param>
        public void AddPoint(Distance2D pt)
        {
            if (Points.Count > 0)
                pt.Y += Distance.FromDefaultUnits(100);   // TODO
            Points.Add(pt);
        }

        public void AddPoint(Geo.Point2D geoPt)
        {
            if (geoPt.Context != Context)
                throw new NotImplementedException("AddPoint from a different Context");

            AddPoint(geoPt.Pt);
        }

        private Node EnsureWallNode()
        {
            var it = AvaloniaSample.AvaloniaSample.It;
            if (it.WallNode == null)
                it.WallNode = it.Scene.CreateChild("Wall");

            return it.WallNode;
        }

        public StaticModel EnsureModel(Node node)
        {
            StaticModel model = node.GetComponent<StaticModel>();
            if (model == null)
            {
                model = node.CreateComponent<StaticModel>();
            }
            model.CastShadows = true;
            //Material mat = Material.FromColor(Color.Magenta);   // TODO
            Material mat = Material.FromColor(Color.Magenta);   // TODO
            model.SetMaterial(mat);
            return model;
        }

        public Poly3D EnsureGeometry(StaticModel model0)
        {
            var model = model0.Model;
            if (model == null)
            {
                model = new Model();
                model.NumGeometries = 1;
                model0.Model = model;
            }
            if (Poly == null)
            {
                Poly = new Poly3D();
                Poly.Init(model0, ElemMask);
                model.SetGeometry(0, 0, Poly.Geom);
                model.BoundingBox = new BoundingBox(-10000, 10000);
            }

            return Poly;
        }

        public void CreateGeometryFromPoints(Node node, StaticModel model, Terrain terrain)
        {
            if (Points.Count < 2)
                return;

            // TEST: Stop after adding one quad.
            if (_currentQuadCount > 0)
                return;

            Debug.WriteLine("\n\n------- CreateGeometryFromPoints -------");
            CreateOrClearGeometry(node, model);

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
            foreach (Distance2D srcPt in Points)
            {
                // On wall's center line.
                Vector3 cl2 = U.PlaceOnTerrain(terrain, srcPt.ToVector2());
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
                        firstPerpendicular = false;
                        // EXPLAIN: We're only on the second point, so cl0=cl1.
                        // There is only one perpendicular possible, from that point to cl2.
                        // Put it in perp0; this will get transferred to perp0 at end of loop.
                        // So it is perp0 for NEXT iteration.
                        perp1 = CalcPerpendicularXZ(cl0, cl2);
                    }
                    else
                    {
                        // GUESS that a good perpendicular at cl1 is tangent to the neighboring points.
                        // TBD: Adjust for relative distances to those points?
                        perp1 = CalcPerpendicularXZ(cl0, cl2);

                        AddQuad(Poly, cl0, cl1, perp0, perp1, terrain);
                    }
                }

                cl0 = cl1;
                cl1 = cl2;
                perp0 = perp1;
            }

            // Add the final quad.
            AddQuad(Poly, cl0, cl1, perp0, perp1, terrain);
            FinishGeometry();
        }

        private void CreateOrClearGeometry(Node node, StaticModel model)
        {
            if (Test_BoxPerWallSegment)
            {
                // Added box sub-nodes. Delete the old ones.
                // TBD: Or just keep adding on new ones, re-use old ones?
                _currentQuadCount = 0;
            }
            else
            {
                //throw new NotImplementedException("ClearGeometry");
                _currentQuadCount = 0;
                _ = EnsureGeometry(model);
                if (!AddOnlyNewQuads)
                    // Recreating all quads each time.
                    Poly.Clear();
            }
        }

        private void FinishGeometry()
        {
            _prevQuadCount = _currentQuadCount;

            if (Test_BoxPerWallSegment)
            {
            }
            else
            {
            }
        }

        private void AddQuad(Poly3D poly, Vector3 cl0, Vector3 cl1, Vector2 perp0, Vector2 perp1, Terrain terrain)
        {
            U.Pair<Vector3> wallPair0 = WallPerpendicularOnTerrain(cl0, WidthMetersF, perp0, HeightMetersF, terrain);
            U.Pair<Vector3> wallPair1 = WallPerpendicularOnTerrain(cl1, WidthMetersF, perp1, HeightMetersF, terrain);
            AddQuad(poly, wallPair0, wallPair1);
        }

        private int _prevQuadCount = 0;
        private int _currentQuadCount = 0;

        private void AddQuad(Poly3D poly, U.Pair<Vector3> wallPair0, U.Pair<Vector3> wallPair1)
        {
            Debug.WriteLine($"--- ({wallPair0}, {wallPair1} ---");
            //throw new NotImplementedException();

            if (Test_BoxPerWallSegment)
            {
                var midpoint0 = U.Average(wallPair0.First, wallPair0.Second);
                var midpoint1 = U.Average(wallPair1.First, wallPair1.Second);
                var midPoint = U.Average(midpoint0, midpoint1);
                _currentQuadCount++;
                // VERSION: Only add if it is a new one.
                if (_currentQuadCount > _prevQuadCount)
                {
                    var it = AvaloniaSample.AvaloniaSample.It;
                    // test: A box at midpoint.
                    it.AddBoxToScene(it.WallNode, midPoint, 0.4f, false);
                    // test: boxes midway to each corner.
                    it.AddBoxToScene(it.WallNode, U.Average(wallPair0.First, midPoint), 0.4f, false);
                    it.AddBoxToScene(it.WallNode, U.Average(wallPair0.Second, midPoint), 0.4f, false);
                    it.AddBoxToScene(it.WallNode, U.Average(wallPair1.First, midPoint), 0.4f, false);
                    it.AddBoxToScene(it.WallNode, U.Average(wallPair1.Second, midPoint), 0.4f, false);
                }
            }
            else
            {
                _currentQuadCount++;
                // ">": Only add if it is a new one. (unless !AddOnlyNewQuads)
                if (_currentQuadCount > _prevQuadCount || !AddOnlyNewQuads)
                {
                    var it = AvaloniaSample.AvaloniaSample.It;
                    poly.AddQuad(wallPair0, wallPair1);
                }
            }
            // OPTIONAL: Could set _prevQuadCount = _currentQuadCount here.
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
