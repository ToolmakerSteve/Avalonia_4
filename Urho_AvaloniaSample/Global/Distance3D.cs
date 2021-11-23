using System;
using System.Drawing;
//using System.Numerics;
using Urho;
using static Global.Utils;

namespace Global
{
    public struct Distance3D : IEquatable<Distance3D>
    {
        #region --- static ----------------------------------------
        // E.g. Maya ground plane in XZ, plus altitude above ground.
        public static Distance3D FromXZ(Distance2D xz, Distance altitude)
        {
            // NOTE: "xz.Y" is actually "Z".
            return new Distance3D(xz.X, altitude, xz.Y);
        }

        public static Distance3D[] OneElementArray(Distance3D point)
        {
            Distance3D[] points = new Distance3D[1];
            points[0] = point;
            return points;
        }

        /// <summary>
        /// Distance in ground plane.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Distance CalcDistance2D(Distance3D p1, Distance3D p2, bool yIsAltitude = false)
        {
            if (yIsAltitude)
                return (p2.XZ() - p1.XZ()).Length;
            else
                return (p2.To2D() - p1.To2D()).Length;
        }
        #endregion


        #region --- data ----------------------------------------
        public Distance X { get; set; }
        public Distance Y { get; set; }
        public Distance Z { get; set; }
        #endregion


        #region --- new ----------------------------------------
        public Distance3D(Distance x, Distance y, Distance z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Distance3D(double x, double y, double z, Distance.UnitsType units)
        {
            if (units == null)
            {
                X = Global.Distance.FromDefaultUnits(x);
                Y = Global.Distance.FromDefaultUnits(y);
                Z = Global.Distance.FromDefaultUnits(z);
            }
            else
            {
                X = Global.Distance.FromSpecifiedUnits(x, units);
                Y = Global.Distance.FromSpecifiedUnits(y, units);
                Z = Global.Distance.FromSpecifiedUnits(z, units);
            }
        }

        public Distance3D(Distance x, Distance y)
        {
            X = x;
            Y = y;
            Z = Distance.Zero;
        }

        public Distance3D(double x, double y, Distance.UnitsType units)
        {
            if (units == null)
            {
                X = Global.Distance.FromDefaultUnits(x);
                Y = Global.Distance.FromDefaultUnits(y);
                Z = Distance.Zero;
            }
            else
            {
                X = Global.Distance.FromSpecifiedUnits(x, units);
                Y = Global.Distance.FromSpecifiedUnits(y, units);
                Z = Global.Distance.FromSpecifiedUnits(0, units);
            }
        }

        public Distance3D(Distance2D pt) : this(pt.X, pt.Y) { }

        public Distance3D(Distance2D pt, Distance z) : this(pt.X, pt.Y, z) { }

        public Distance3D(PointF pt, Distance.UnitsType units) : this(pt.X, pt.Y, units) { }

        public Distance3D(Vector3 pt, Distance.UnitsType units) : this(pt.X, pt.Y, pt.Z, units) { }

        public Distance3D(double value, Distance.UnitsType units) : this(value, value, value, units) { }
        #endregion


        public bool IsValid => Distance2D.CoordIsValid(X) && Distance2D.CoordIsValid(Y) && Distance2D.CoordIsValid(Z);


        public bool IsNaN => (double.IsNaN(X.Value) || double.IsNaN(Y.Value) || double.IsNaN(Z.Value));

        public bool IsZero => (X.Value == 0) && (Y.Value == 0) && (Z.Value == 0);

        /// <summary>
        /// Quicker than "Length" - avoids Sqrt.
        /// HACK CAUTION: Result is in units "Distance.DefaultUnits SQUARED".
        /// It is no longer a DISTANCE.
        /// </summary>
        public double LengthSquared => (X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value);

        public Distance Length => Distance.FromDefaultUnits(Math.Sqrt(X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value));

        // Return point with units length (or zero, if Me is zero).
        public Distance3D Normalize
        {
            get
            {
                double len = this.Length.Value;
                return len == 0 ? this : this / len;
            }
        }


        public override bool Equals(object obj)
        {
            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped ElseDirectiveTrivia */
            if (obj is Distance3D)
                // NOTE: This is NOT recursive; it is call to "Equals(other As Distance3D)".
                return Equals((Distance3D)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return MakeHash(X, Y, Z);
        }



        public bool Equals(Distance3D other)
        {
            return (X.Value == other.X.Value) && (Y.Value == other.Y.Value) && (Z.Value == other.Z.Value);
        }

        /// <summary>
        /// </summary>
        /// <returns>Implicitly has units Distance.DefaultUnits.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(Convert.ToSingle(X.Value), Convert.ToSingle(Y.Value), Convert.ToSingle(Z.Value));
        }

        /// <summary>
        /// </summary>
        /// <returns>Implicitly has units Distance.DefaultUnits.</returns>
        public PointF ToPointF()
        {
            return new PointF(Convert.ToSingle(X.Value), Convert.ToSingle(Y.Value));
        }

        /// <summary>
        /// CAUTION: If altitude is in Y, use "XZ" instead.
        /// </summary>
        /// <returns></returns>
        public Distance2D To2D()
        {
            return new Distance2D(X, Y);
        }

        // E.G. extract position on Maya ground plane.
        public Distance2D XZ()
        {
            return new Distance2D(X, Z);
        }

        /// <summary>
        /// </summary>
        /// <returns>Implicitly has units Distance.DefaultUnits.</returns>
        public Point ToPoint()
        {
            return new Point(X.Value.RoundInt(), Y.Value.RoundInt());
        }


        public override string ToString()
        {
            return "{" + Utils.Round4or6(X.Value) + ", " + Utils.Round4or6(Y.Value) + ", " + Utils.Round3(Z.Value) + "}";
        }

        public string ToShortString
        {
            get
            {
                return "{" + ShortString(X.Value) + ", " + ShortString(Y.Value) + ", " + ShortString(Z.Value) + "}";
            }
        }


        // Public Shared Operator =(ByVal left As PointD, ByVal right As PointD) As PointD
        // left.X = right.X
        // left.Y = right.Y
        // End Operator

        // Public Shared Operator <>(ByVal left As PointD, ByVal right As PointD) As PointD
        // End Operator

        public void Add(Distance3D pt2)
        {
            this.X += pt2.X;
            this.Y += pt2.Y;
            this.Z += pt2.Z;
        }

        public void Add(Distance2D pt)
        {
            this.X += pt.X;
            this.Y += pt.Y;
        }

        public void Add(PointF pt, Distance.UnitsType units)
        {
            if (units == null)
            {
                X = Distance.FromDefaultUnits(X.Value + pt.X);
                Y = Distance.FromDefaultUnits(Y.Value + pt.Y);
            }
            else
            {
                X = X + Distance.FromSpecifiedUnits(pt.X, units);
                Y = Y + Distance.FromSpecifiedUnits(pt.Y, units);
            }
        }

        public static bool operator ==(Distance3D ptd1, Distance3D ptd2)
        {
            return (ptd1.X == ptd2.X && ptd1.Y == ptd2.Y && ptd1.Z == ptd2.Z);
        }

        public static bool operator !=(Distance3D ptd1, Distance3D ptd2)
        {
            return !(ptd1.X == ptd2.X && ptd1.Y == ptd2.Y && ptd1.Z == ptd2.Z);
        }


        public static Distance3D operator +(Distance3D ptd1, Distance3D ptd2)
        {
            return new Distance3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
        }

        //// TODO: Not valid without a third parameter to specify Units.
        //public static Distance3D operator +(Distance3D ptd1, Vector3 ptd2)
        //{
        //    return new Distance3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
        //}

        //// TODO: Not valid without a third parameter to specify Units.
        //public static Distance3D operator +(Vector3 ptd1, Distance3D ptd2)
        //{
        //    return new Distance3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
        //}

        public static Distance3D operator +(Distance3D ptd1, Distance2D ptd2)
        {
            return new Distance3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z);
        }

        // Negate
        public static Distance3D operator -(Distance3D point)
        {
            return new Distance3D(-point.X, -point.Y, -point.Z);
        }

        // Subtract
        public static Distance3D operator -(Distance3D ptd1, Distance3D ptd2)
        {
            return new Distance3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z - ptd2.Z);
        }

        //// TBD: Not valid without a third parameter to specify Units.
        //public static Distance3D operator -(Distance3D ptd1, Vector3 ptd2)
        //{
        //    return new Distance3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z - ptd2.Z);
        //}

        public static Distance3D operator -(Distance3D ptd1, Distance2D ptd2)
        {
            return new Distance3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z);
        }


        /// <summary>
        /// Scale the axes by independent "unitless" multipliers.
        /// CAUTION: This is NOT the "dot product" of two vectors; see "DotProduct".
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <returns></returns>
        public static Distance3D operator *(Distance3D pt1, double3 pt2)
        {
            return new Distance3D(pt1.X * pt2.X, pt1.Y * pt2.Y, pt1.Z * pt2.Z);
        }
        public static Distance3D operator *(double3 pt1, Distance3D pt2)
        {
            return new Distance3D(pt1.X * pt2.X, pt1.Y * pt2.Y, pt1.Z * pt2.Z);
        }

        // -- commented out; I think its okay to automatically promote to the "double" version. --
        //public static Distance3D operator *(Distance3D pt, int n)
        //{
        //    return new Distance3D(pt.X * n, pt.Y * n, pt.Z * n);
        //}

        //public static Distance3D operator *(Distance3D pt, float n)
        //{
        //    return new Distance3D(pt.X * n, pt.Y * n, pt.Z * n);
        //}

        public static Distance3D operator *(Distance3D pt, double n)
        {
            return new Distance3D(pt.X * n, pt.Y * n, pt.Z * n);
        }

        public static Distance3D operator *(double n1, Distance3D pt)
        {
            return new Distance3D(pt.X * n1, pt.Y * n1, pt.Z * n1);
        }


        public Distance DotProduct(Distance3D p2)
        {
            return Distance.FromDefaultUnits(X.Value * p2.X.Value + Y.Value * p2.Y.Value + Z.Value * p2.Z.Value);
        }



        public static Distance3D operator /(Distance3D ptd1, double3 ptd2)
        {
            return new Distance3D(ptd1.X / ptd2.X, ptd1.Y / ptd2.Y, ptd1.Z / ptd2.Z);
        }

        // -- commented out; I think its okay to automatically promote to the "double" version. --
        //public static Distance3D operator /(Distance3D ptd1, int int2)
        //{
        //    return new Distance3D(ptd1.X / int2, ptd1.Y / int2, ptd1.Z / int2);
        //}

        //public static Distance3D operator /(Distance3D ptd1, float sng2)
        //{
        //    return new Distance3D(ptd1.X / sng2, ptd1.Y / sng2, ptd1.Z / sng2);
        //}

        public static Distance3D operator /(Distance3D ptd1, double dbl2)
        {
            return new Distance3D(ptd1.X / dbl2, ptd1.Y / dbl2, ptd1.Z / dbl2);
        }

        /// <summary>
        /// Used for "inverse"; usage: "OneDefaultUnit / point".
        /// HOWEVER note that that it is w.r.t. "1 DefaultUnit".
        /// The concept of "inverse" isn't strictly meaningful when discussing a Distance;
        /// Inverse is a "unitless" concept.
        /// </summary>
        /// <param name="numerator"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double3 operator /(Distance numerator, Distance3D p2)
        {
            return new double3(numerator / p2.X, numerator / p2.Y, numerator / p2.Z);
        }


        // Compare two Distance3D's for "equal within a tolerance".
        public bool NearlyEquals(Distance3D p2)
        {
            return this.X.NearlyEquals(p2.X) && this.Y.NearlyEquals(p2.Y) && this.Z.NearlyEquals(p2.Z);
        }

        // Compare two Distance3D's for "equal within a tolerance".
        public bool NearlyEquals(Distance3D p2, double tolerance)
        {
            return this.X.NearlyEquals(p2.X, tolerance) && this.Y.NearlyEquals(p2.Y, tolerance) && this.Z.NearlyEquals(p2.Z, tolerance);
        }

        public Distance3D Round2()
        {
            return new Distance3D(Utils.Round2(this.X.Value), Utils.Round2(this.Y.Value), Utils.Round2(this.Z.Value), null);
        }
        public Distance3D Round3()
        {
            return new Distance3D(Utils.Round3(this.X.Value), Utils.Round3(this.Y.Value), Utils.Round3(this.Z.Value), null);
        }

        public Distance3D SwapYZ()
        {
            return new Distance3D(this.X, this.Z, this.Y);
        }

        public static Distance3D Zero()
        {
            return new Distance3D();
        }

        public static Distance3D NaN()
        {
            return new Distance3D(double.NaN, double.NaN, double.NaN, null);
        }

        public static readonly Distance3D MinValue = new Distance3D(double.MinValue, double.MinValue, double.MinValue, null);
        public static readonly Distance3D MaxValue = new Distance3D(double.MaxValue, double.MaxValue, double.MaxValue, null);
        /// <summary>
        /// CAUTION: ASSUMES DefaultUnit is desired. Technically, this should probably be a "Unitless3D".
        /// </summary>
        public static readonly Distance3D UnitY = new Distance3D(0, 1, 0, null);
        public static readonly Distance3D UnitZ = new Distance3D(0, 0, 1, null);

        public static Distance3D[] ListFromPoint2Ds(ref Distance2D[] Point2Ds)
        {
            int nPoints = Point2Ds.Length;
            Distance3D[] Point3Ds = new Distance3D[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point3Ds[index] = new Distance3D(Point2Ds[index]);

            return Point3Ds;
        }

        // Usage: IList(Of Distance3D).Sort(AddressOf Distance3D.IncreasingX))
        public static int IncreasingX(Distance3D p1, Distance3D p2)
        {
            return p1.X.Value.CompareTo(p2.X.Value);
        }
        // Usage: IList(Of Distance3D).Sort(AddressOf Distance3D.IncreasingY))
        public static int IncreasingY(Distance3D p1, Distance3D p2)
        {
            return p1.Y.Value.CompareTo(p2.Y.Value);
        }
    }
    /* TODO ERROR: Skipped EndIfDirectiveTrivia */

}
