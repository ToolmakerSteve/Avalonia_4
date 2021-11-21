using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using static Global.Utils;

namespace Global
{
    public struct Point2D : IPoint, IEquatable<Point2D>
    {
        public static Point2D[] OneElementArray(Point2D point)
        {
            Point2D[] points = new Point2D[1];
            points[0] = point;
            return points;
        }

        internal static double Distance(Point2D cornerTL, Point2D cornerTR)
        {
            return (cornerTR - cornerTL).Length;
        }

        public static Point2D Average(Point2D p0, Point2D p1)
        {
            return new Point2D(Utils.Average(p0.X, p1.X), Utils.Average(p0.Y, p1.Y));
        }

        public static Point2D Average3(Point2D p0, Point2D p1, Point2D p2)
        {
            return new Point2D(Utils.Average3(p0.X, p1.X, p2.X), Utils.Average3(p0.Y, p1.Y, p2.Y));
        }


        #region "-- data --"
        public double X { get; set; }
        public double Y { get; set; }

        public Type ValueType => typeof(Point2D);
        #endregion


        /// <summary>
        /// To make it clear that we are cloning.
        /// (Doesn't matter for a "struct" - could just "=".)
        /// </summary>
        /// <param name="ptdPoint"></param>
        public Point2D(Point2D ptdPoint)
        {
            X = ptdPoint.X;
            Y = ptdPoint.Y;
        }

        public Point2D(PointF ptfPoint)
        {
            X = ptfPoint.X;
            Y = ptfPoint.Y;
        }

        public Point2D(Point3D ptdPoint)
        {
            X = ptdPoint.X;
            Y = ptdPoint.Y;
        }

        public Point2D(double dblValue)
        {
            X = dblValue;
            Y = dblValue;
        }

        public Point2D(double dblX, double dblY)
        {
            X = dblX;
            Y = dblY;
        }

        public Point2D(Vector2 v2)
        {
            X = v2.X;
            Y = v2.Y;
        }



        public bool IsValid
        {
            get
            {
                return CoordIsValid(X) && CoordIsValid(Y);
            }
        }
        public bool IsBad
        {
            get
            {
                return !IsValid;
            }
        }
        public bool IsNaN
        {
            get
            {
                return (double.IsNaN(X) || double.IsNaN(Y));
            }
        }
        // If True, then "Me" is the default value, for optional parameter "= Nothing".
        public bool IsZero
        {
            get
            {
                return (X == 0) && (Y == 0);
            }
        }
        public bool EitherIsZero
        {
            get
            {
                return (X == 0) || (Y == 0);
            }
        }



        /// <summary>
        /// TBD: Is this necessary for a "struct"? Isn't it built in?
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Point2D other)
        {
            return (X == other.X) && (Y == other.Y);
        }

        // Compare two Point2D's for "equal within a tolerance".
        public bool NearlyEquals(Point2D p2, double tolerance = EpsilonForOne)
        {
            return X.NearlyEquals(p2.X, tolerance) && Y.NearlyEquals(p2.Y, tolerance);
        }


        // When geo coords are stored in a point, X is EW, Y is NS.
        public double LongitudeEW
        {
            get
            {
                return X;
            }
        }
        public double LatitudeNS
        {
            get
            {
                return Y;
            }
        }

        // Quicker than "Length" - avoids Sqrt.
        public double LengthSquared
        {
            get
            {
                return (X * X + Y * Y);
            }
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(X * X + Y * Y);
            }
        }

        // Return point with units length (or zero, if Me is zero).
        public Point2D Normalize
        {
            get
            {
                double len = Length;
                if (len == 0)
                    return this;
                return this / len;
            }
        }


        public PointF ToPointF()
        {
            if (X == double.MaxValue)
                return new PointF(float.MaxValue, float.MaxValue);
            return new PointF(System.Convert.ToSingle(X), System.Convert.ToSingle(Y));
        }

        public Point3D ToPoint3D()
        {
            return new Point3D(X, Y);
        }

        public Point ToPoint()
        {
            if (X == double.MaxValue)
                return new Point(int.MaxValue, int.MaxValue);
            return new Point(System.Convert.ToInt32(X), System.Convert.ToInt32(Y));
        }

        public Vector2 ToVector2()
        {
            if (X == double.MaxValue)
                return new Vector2(float.MaxValue, float.MaxValue);
            return new Vector2(System.Convert.ToSingle(X), System.Convert.ToSingle(Y));
        }


        public Point2D SwapXY()
        {
            return new Point2D(Y, X);
        }

        public override string ToString()
        {
            return "{" + Round4or6(X) + ", " + Round4or6(Y) + "}";
        }

        public string ToShortString
        {
            get
            {
                return "{" + ShortString(X) + ", " + ShortString(Y) + "}";
            }
        }


        public string ToAbbrevString()
        {
            return "{" + AbbrevString(X) + ", " + AbbrevString(Y) + "}";
        }

        public string ToFractionString()
        {
            return "{" + FractionString(X) + ", " + FractionString(Y) + "}";
        }

        public string ToF1String()
        {
            return string.Format("({0:f1}, {1:f1})", X, Y);
        }

        // Public Shared Operator =(ByVal left As PointD, ByVal right As PointD) As PointD
        // left.X = right.X
        // left.Y = right.Y
        // End Operator

        // Public Shared Operator <>(ByVal left As PointD, ByVal right As PointD) As PointD
        // End Operator

        public void Add(Point2D ptdPoint)
        {
            X += ptdPoint.X;
            Y += ptdPoint.Y;
        }

        public void Add(PointF ptfPoint)
        {
            X += ptfPoint.X;
            Y += ptfPoint.Y;
        }

        public Point2D Round(int digits)
        {
            return new Point2D(Math.Round(X, digits), Math.Round(Y, digits));
        }

        public Point2D Map(UnaryDeleg action)
        {
            return new Point2D(action(X), action(Y));
        }

        public double Cross(Point2D p1, Point2D p2)
        {
            return (p1.Y - Y) * (p2.X - X) - (p1.X - X) * (p2.Y - Y);
        }

        // Return Normal to segment between Me and p2.
        public Point2D SegmentNormal(Point2D p2)
        {
            Point2D normal1 = new Point2D(-(p2.Y - Y), p2.X - X);
            normal1 = normal1.Normalize;
            return normal1;
        }

        public static bool operator ==(Point2D ptd1, Point2D ptd2)
        {
            return (ptd1.X == ptd2.X) && (ptd1.Y == ptd2.Y);
        }

        public static bool operator !=(Point2D ptd1, Point2D ptd2)
        {
            return !(ptd1 == ptd2);
        }


        public static Point2D operator +(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point2D operator +(Point2D p1, int n2)
        {
            return new Point2D(p1.X + n2, p1.Y + n2);
        }

        public static Point2D operator +(Point2D p1, float n2)
        {
            return new Point2D(p1.X + n2, p1.Y + n2);
        }

        public static Point2D operator +(Point2D p1, double n2)
        {
            return new Point2D(p1.X + n2, p1.Y + n2);
        }


        public static Point2D operator -(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point2D operator -(Point2D p1, int int2)
        {
            return new Point2D(p1.X - int2, p1.Y - int2);
        }

        public static Point2D operator -(Point2D p1, float sng2)
        {
            return new Point2D(p1.X - sng2, p1.Y - sng2);
        }

        public static Point2D operator -(Point2D p1, double dbl2)
        {
            return new Point2D(p1.X - dbl2, p1.Y - dbl2);
        }

        // Negate (unary)
        public static Point2D operator -(Point2D ptd1)
        {
            return new Point2D(-ptd1.X, -ptd1.Y);
        }

        // "Dot Product"
        public double Dot(Point2D b)
        {
            return (X * b.X) + (Y * b.Y);
        }

        // Q: When is this meaningful?
        // See Also "Dot" and "Cross" (where?)
        public static Point2D operator *(Point2D ptd1, Point2D ptd2)
        {
            return new Point2D(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
        }

        public static Point2D operator *(Point2D ptd1, int int2)
        {
            return new Point2D(ptd1.X * int2, ptd1.Y * int2);
        }

        public static Point2D operator *(Point2D ptd1, float sng2)
        {
            return new Point2D(ptd1.X * sng2, ptd1.Y * sng2);
        }

        public static Point2D operator *(Point2D ptd1, double dbl2)
        {
            return new Point2D(ptd1.X * dbl2, ptd1.Y * dbl2);
        }

        public static Point2D operator *(double dbl1, Point2D ptd2)
        {
            return new Point2D(dbl1 * ptd2.X, dbl1 * ptd2.Y);
        }


        public static Point2D operator /(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.X / p2.X, p1.Y / p2.Y);
        }

        public static Point2D operator /(Point2D p1, int int2)
        {
            return new Point2D(p1.X / int2, p1.Y / int2);
        }

        public static Point2D operator /(Point2D p1, float sng2)
        {
            return new Point2D(p1.X / sng2, p1.Y / sng2);
        }

        public static Point2D operator /(Point2D p1, double dbl2)
        {
            return new Point2D(p1.X / dbl2, p1.Y / dbl2);
        }

        // Used for "inverse"; e.g. "1 / point".
        public static Point2D operator /(double dbl1, Point2D p2)
        {
            return new Point2D(dbl1 / p2.X, dbl1 / p2.Y);
        }


        public static Point2D Zero()
        {
            return new Point2D();
        }

        public static Point2D NaN()
        {
            return new Point2D(double.NaN, double.NaN);
        }

        public static readonly Point2D MinValue = new Point2D(double.MinValue, double.MinValue);
        public static readonly Point2D MaxValue = new Point2D(double.MaxValue, double.MaxValue);

        public static bool CoordIsValid(double xOrY)
        {
            return (!double.IsNaN(xOrY)) && (!double.IsInfinity(xOrY)) && (double.MaxValue > xOrY) && (double.MinValue < xOrY);
        }

        // Just check one coordinate.
        public bool NotNanQuick()
        {
            return !double.IsNaN(X);
        }

        public Point2D Abs()
        {
            return new Point2D(Math.Abs(X), Math.Abs(Y));
        }

        // Return Min of (each coordinate of) Me and p2.
        public Point2D Min(Point2D p2)
        {
            return new Point2D(Math.Min(X, p2.X), Math.Min(Y, p2.Y));
        }
        // Return Max of (each coordinate of) Me and p2.
        public Point2D Max(Point2D p2)
        {
            return new Point2D(Math.Max(X, p2.X), Math.Max(Y, p2.Y));
        }


        public RectangleF Mult(RectangleF rect)
        {
            return new RectangleF(System.Convert.ToSingle(X * rect.Left), System.Convert.ToSingle(Y * rect.Top), System.Convert.ToSingle(X * rect.Width), System.Convert.ToSingle(Y * rect.Height));
        }


        public static Point2D[] ArrayFromPointFs(PointF[] points)
        {
            int nPoints = points.Length;
            Point2D[] Point2Ds = new Point2D[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point2Ds[index] = new Point2D(points[index]);

            return Point2Ds;
        }

        public static Point2D[] ArrayFromDouble2s(double[,] points)
        {
            int lastIndex = points.GetUpperBound(0);
            Point2D[] Point2Ds = new Point2D[lastIndex + 1];

            for (int index = 0; index <= lastIndex; index++)
                Point2Ds[index] = new Point2D(points[index, 0], points[index, 1]);

            return Point2Ds;
        }

        // NOTE: "point3Ds" might be List or Array.
        public static Point2D[] ListFromPoint3Ds(IList<Point3D> point3Ds)
        {
            int nPoints = point3Ds.Count;
            Point2D[] Point2Ds = new Point2D[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point2Ds[index] = new Point2D(point3Ds[index]);

            return Point2Ds;
        }

        // NOTE: "point3Ds" might be List or Array.
        public static Point2D[] ListFromPointXZs(IList<Point3D> point3Ds)
        {
            int nPoints = point3Ds.Count;
            Point2D[] Point2Ds = new Point2D[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point2Ds[index] = point3Ds[index].XZ();

            return Point2Ds;
        }

        // NOTE: "point2Ds" might be List or Array.
        public static PointF[] ListToPointFs(IList<Point2D> point2Ds)
        {
            int nPoints = point2Ds.Count;
            PointF[] PointFs = new PointF[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
            {
                Point2D p = point2Ds[index];
                PointFs[index] = new PointF(System.Convert.ToSingle(p.X), System.Convert.ToSingle(p.Y));
            }

            return PointFs;
        }


        public static List<Point2D> CalcDeltas(IList<Point2D> points)
        {
            List<Point2D> deltas = new List<Point2D>();

            Point2D priorPt = default(Point2D);
            bool hasPriorPt = false;
            foreach (Point2D point in points)
            {
                if (hasPriorPt)
                {
                    Point2D delta = point - priorPt;
                    deltas.Add(delta);
                }
                priorPt = point;
                hasPriorPt = true;
            }

            return deltas;
        }
    }
}
