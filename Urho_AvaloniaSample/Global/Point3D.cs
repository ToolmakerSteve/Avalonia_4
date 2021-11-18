using System;
using System.Drawing;
using System.Numerics;
using static Global.Utils;

namespace Global
{
    public struct Point3D : IEquatable<Point3D>
    {
        #region "-- static --"
        // E.g. Maya ground plane in XZ, plus altitude above ground.
        public static Point3D FromXZ(Point2D xz, double altitude)
        {
            // NOTE: "xz.Y" is actually "Z".
            return new Point3D(xz.X, altitude, xz.Y);
        }

        public static Point3D[] OneElementArray(Point3D point)
        {
            Point3D[] points = new Point3D[1];
            points[0] = point;
            return points;
        }
        #endregion




        #region "-- new --"
        public Point3D(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Point3D(Point2D pt) : this(pt.X, pt.Y) { }

        public Point3D(Point2D pt, double z) : this(pt.X, pt.Y, z) { }

        public Point3D(PointF pt) : this(pt.X, pt.Y) { }

        public Point3D(Vector3 pt) : this(pt.X, pt.Y, pt.Z) { }

        public Point3D(double value) : this(value, value, value) { }
        #endregion


        public bool IsValid => Point2D.CoordIsValid(X) && Point2D.CoordIsValid(Y) && Point2D.CoordIsValid(Z);


        public double X { get; set; }

        public double Y { get; set; }

        public double Z { get; set; }


        public bool IsNaN => (double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z));

        public bool IsZero => (X == 0) && (Y == 0) && (Z == 0);

        // Quicker than "Length" - avoids Sqrt.
        public double LengthSquared => (X * X + Y * Y + Z * Z);

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        // Return point with unit length (or zero, if Me is zero).
        public Point3D Normalize
        {
            get
            {
                double len = this.Length;
                return len == 0 ? this : this / len;
            }
        }


        public override bool Equals(object obj)
        {
            /* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped ElseDirectiveTrivia */
            if (obj is Point3D)
                // NOTE: This is NOT recursive; it is call to "Equals(other As Point3D)".
                return Equals((Point3D)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return MakeHash(X, Y, Z);
        }



        public bool Equals(Point3D other)
        {
            return (X == other.X) && (Y == other.Y) && (Z == other.Z);
        }


        public Vector3 ToVector3()
        {
            return new Vector3(Convert.ToSingle(X), Convert.ToSingle(Y), Convert.ToSingle(Z));
        }

        public PointF ToPointF()
        {
            return new PointF(Convert.ToSingle(X), Convert.ToSingle(Y));
        }

        public Point2D To2D()
        {
            return new Point2D(X, Y);
        }

        // E.G. extract position on Maya ground plane.
        public Point2D XZ()
        {
            return new Point2D(X, Z);
        }

        public Point ToPoint()
        {
            return new Point(X.RoundInt(), Y.RoundInt());
        }


        public override string ToString()
        {
            return "{" + Utils.Round4or6(X) + ", " + Utils.Round4or6(Y) + ", " + Utils.Round3(Z) + "}";
        }

        public string ToShortString
        {
            get
            {
                return "{" + ShortString(this.X) + ", " + ShortString(this.Y) + ", " + ShortString(this.Z) + "}";
            }
        }


        // Public Shared Operator =(ByVal left As PointD, ByVal right As PointD) As PointD
        // left.X = right.X
        // left.Y = right.Y
        // End Operator

        // Public Shared Operator <>(ByVal left As PointD, ByVal right As PointD) As PointD
        // End Operator

        public void Add(Point3D ptdPoint)
        {
            this.X += ptdPoint.X;
            this.Y += ptdPoint.Y;
            this.Z += ptdPoint.Z;
        }

        public void Add(Point2D ptdPoint)
        {
            this.X += ptdPoint.X;
            this.Y += ptdPoint.Y;
        }

        public void Add(PointF ptfPoint)
        {
            this.X += ptfPoint.X;
            this.Y += ptfPoint.Y;
        }

        public static bool operator ==(Point3D ptd1, Point3D ptd2)
        {
            return (ptd1.X == ptd2.X && ptd1.Y == ptd2.Y && ptd1.Z == ptd2.Z);
        }

        public static bool operator !=(Point3D ptd1, Point3D ptd2)
        {
            return !(ptd1.X == ptd2.X && ptd1.Y == ptd2.Y && ptd1.Z == ptd2.Z);
        }


        public static Point3D operator +(Point3D ptd1, Point3D ptd2)
        {
            return new Point3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
        }

        public static Point3D operator +(Point3D ptd1, Vector3 ptd2)
        {
            return new Point3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
        }

        public static Point3D operator +(Vector3 ptd1, Point3D ptd2)
        {
            return new Point3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
        }

        public static Point3D operator +(Point3D ptd1, Point2D ptd2)
        {
            return new Point3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z);
        }

        public static Point3D operator +(Point3D ptd1, int int2)
        {
            return new Point3D(ptd1.X + int2, ptd1.Y + int2, ptd1.Z + int2);
        }

        public static Point3D operator +(Point3D ptd1, float sng2)
        {
            return new Point3D(ptd1.X + sng2, ptd1.Y + sng2, ptd1.Z + sng2);
        }

        public static Point3D operator +(Point3D ptd1, double dbl2)
        {
            return new Point3D(ptd1.X + dbl2, ptd1.Y + dbl2, ptd1.Z + dbl2);
        }

        // Negate
        public static Point3D operator -(Point3D point)
        {
            return new Point3D(-point.X, -point.Y, -point.Z);
        }

        // Subtract
        public static Point3D operator -(Point3D ptd1, Point3D ptd2)
        {
            return new Point3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z - ptd2.Z);
        }

        public static Point3D operator -(Point3D ptd1, Vector3 ptd2)
        {
            return new Point3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z - ptd2.Z);
        }

        public static Point3D operator -(Vector3 ptd1, Point3D ptd2)
        {
            return new Point3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z - ptd2.Z);
        }

        public static Point3D operator -(Point3D ptd1, Point2D ptd2)
        {
            return new Point3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z);
        }

        public static Point3D operator -(Point3D ptd1, int int2)
        {
            return new Point3D(ptd1.X - int2, ptd1.Y - int2, ptd1.Z - int2);
        }

        public static Point3D operator -(Point3D ptd1, float sng2)
        {
            return new Point3D(ptd1.X - sng2, ptd1.Y - sng2, ptd1.Z - sng2);
        }

        public static Point3D operator -(Point3D ptd1, double dbl2)
        {
            return new Point3D(ptd1.X - dbl2, ptd1.Y - dbl2, ptd1.Z - dbl2);
        }


        // CAUTION: This is NOT the "dot product" of the two vectors; see "DotProduct".
        public static Point3D operator *(Point3D pt1, Point3D pt2)
        {
            return new Point3D(pt1.X * pt2.X, pt1.Y * pt2.Y, pt1.Z * pt2.Z);
        }

        public static Point3D operator *(Point3D pt1, Point2D pt2)
        {
            return new Point3D(pt1.X * pt2.X, pt1.Y * pt2.Y, pt1.Z);
        }

        public static Point3D operator *(Point3D pt, int n)
        {
            return new Point3D(pt.X * n, pt.Y * n, pt.Z * n);
        }

        public static Point3D operator *(Point3D pt, float n)
        {
            return new Point3D(pt.X * n, pt.Y * n, pt.Z * n);
        }

        public static Point3D operator *(Point3D pt, double n)
        {
            return new Point3D(pt.X * n, pt.Y * n, pt.Z * n);
        }

        public static Point3D operator *(double n1, Point3D pt)
        {
            return new Point3D(pt.X * n1, pt.Y * n1, pt.Z * n1);
        }


        public double DotProduct(Point3D p2)
        {
            return this.X * p2.X + this.Y * p2.Y + this.Z * p2.Z;
        }



        public static Point3D operator /(Point3D ptd1, Point3D ptd2)
        {
            return new Point3D(ptd1.X / ptd2.X, ptd1.Y / ptd2.Y, ptd1.Z / ptd2.Z);
        }

        public static Point3D operator /(Point3D ptd1, Point2D ptd2)
        {
            return new Point3D(ptd1.X / ptd2.X, ptd1.Y / ptd2.Y, ptd1.Z);
        }

        public static Point3D operator /(Point3D ptd1, int int2)
        {
            return new Point3D(ptd1.X / int2, ptd1.Y / int2, ptd1.Z / int2);
        }

        public static Point3D operator /(Point3D ptd1, float sng2)
        {
            return new Point3D(ptd1.X / sng2, ptd1.Y / sng2, ptd1.Z / sng2);
        }

        public static Point3D operator /(Point3D ptd1, double dbl2)
        {
            return new Point3D(ptd1.X / dbl2, ptd1.Y / dbl2, ptd1.Z / dbl2);
        }


        // Compare two Point3D's for "equal within a tolerance".
        public bool NearlyEquals(Point3D p2)
        {
            return this.X.NearlyEquals(p2.X) && this.Y.NearlyEquals(p2.Y) && this.Z.NearlyEquals(p2.Z);
        }

        // Compare two Point3D's for "equal within a tolerance".
        public bool NearlyEquals(Point3D p2, double tolerance)
        {
            return this.X.NearlyEquals(p2.X, tolerance) && this.Y.NearlyEquals(p2.Y, tolerance) && this.Z.NearlyEquals(p2.Z, tolerance);
        }

        public Point3D Round2()
        {
            return new Point3D(Utils.Round2(this.X), Utils.Round2(this.Y), Utils.Round2(this.Z));
        }
        public Point3D Round3()
        {
            return new Point3D(Utils.Round3(this.X), Utils.Round3(this.Y), Utils.Round3(this.Z));
        }

        public Point3D SwapYZ()
        {
            return new Point3D(this.X, this.Z, this.Y);
        }

        public static Point3D Zero()
        {
            return new Point3D();
        }

        public static Point3D NaN()
        {
            return new Point3D(double.NaN, double.NaN, double.NaN);
        }

        public static readonly Point3D MinValue = new Point3D(double.MinValue, double.MinValue, double.MinValue);
        public static readonly Point3D MaxValue = new Point3D(double.MaxValue, double.MaxValue, double.MaxValue);
        public static readonly Point3D UnitY = new Point3D(0, 1, 0);
        public static readonly Point3D UnitZ = new Point3D(0, 0, 1);

        public static Point3D[] ListFromPoint2Ds(ref Point2D[] Point2Ds)
        {
            int nPoints = Point2Ds.Length;
            Point3D[] Point3Ds = new Point3D[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point3Ds[index] = new Point3D(Point2Ds[index]);

            return Point3Ds;
        }

        // Usage: IList(Of Point3D).Sort(AddressOf Point3D.IncreasingX))
        public static int IncreasingX(Point3D p1, Point3D p2)
        {
            return p1.X.CompareTo(p2.X);
        }
        // Usage: IList(Of Point3D).Sort(AddressOf Point3D.IncreasingY))
        public static int IncreasingY(Point3D p1, Point3D p2)
        {
            return p1.Y.CompareTo(p2.Y);
        }
    }
    /* TODO ERROR: Skipped EndIfDirectiveTrivia */

}
