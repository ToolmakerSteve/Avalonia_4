﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using static Global.Utils;

namespace Global
{
    public struct Point2D : IEquatable<Point2D>
    {
        public static Point2D[] OneElementArray(Point2D point)
        {
            Point2D[] points = new Point2D[1];
            points[0] = point;
            return points;
        }

        internal static Distance DistanceBetween(Point2D cornerTL, Point2D cornerTR)
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


        #region --- data ----------------------------------------
        public Distance X { get; set; }
        public Distance Y { get; set; }
        #endregion


        #region --- new ----------------------------------------
        public Point2D(Distance x, Distance y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// To make it clear that we are cloning.
        /// (A bit overkill for a "struct" - could just "=".)
        /// </summary>
        /// <param name="pt"></param>
        public Point2D(Point2D pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        public Point2D(Point3D pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        /// <summary>
        /// Set both X and Y to "value".
        /// </summary>
        /// <param name="value"></param>
        public Point2D(Distance value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="units">null means "use Distance.DefaultUnits".</param>
        public Point2D(double x, double y, Distance.UnitsType units)
        {
            if (units == null)
            {
                X = Global.Distance.FromDefaultUnits(x);
                Y = Global.Distance.FromDefaultUnits(y);
            }
            else
            {
                X = Global.Distance.FromSpecifiedUnits(x, units);
                Y = Global.Distance.FromSpecifiedUnits(y, units);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="units">null means "use Distance.DefaultUnits".</param>
        public Point2D(PointF pt, Distance.UnitsType units) : this(pt.X, pt.Y, units)
        {
        }

        public Point2D(Vector2 pt, Distance.UnitsType units) : this(pt.X, pt.Y, units)
        {
        }

        /// <summary>
        /// Set both X and Y to "value".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units">null means "use Distance.DefaultUnits".</param>
        public Point2D(double value, Distance.UnitsType units) : this(value, value, units)
        {
        }
        #endregion



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
                return (double.IsNaN(X.Value) || double.IsNaN(Y.Value));
            }
        }
        // If True, then "Me" is the default value, for optional parameter "= Nothing".
        public bool IsZero
        {
            get
            {
                return (X.Value == 0) && (Y.Value == 0);
            }
        }
        public bool EitherIsZero
        {
            get
            {
                return (X.Value == 0) || (Y.Value == 0);
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
        public Distance LongitudeEW
        {
            get
            {
                return X;
            }
        }
        public Distance LatitudeNS
        {
            get
            {
                return Y;
            }
        }

        // Quicker than "Length" - avoids Sqrt.
        public Distance.Squared LengthSquared
        {
            get
            {
                return (X * X + Y * Y);
            }
        }

        public Distance Length
        {
            get
            {
                return (X * X + Y * Y).Sqrt();
            }
        }

        // Return point with units length (or zero, if Me is zero).
        public Point2D Normalize
        {
            get
            {
                double len = Length.Value;
                if (len == 0)
                    return this;
                return this / len;
            }
        }


        public PointF ToPointF()
        {
            if (X.Value == double.MaxValue)
                return new PointF(float.MaxValue, float.MaxValue);
            return new PointF(System.Convert.ToSingle(X), System.Convert.ToSingle(Y));
        }

        public Point3D ToPoint3D()
        {
            return new Point3D(X, Y);
        }

        /// <summary>
        /// HACK: If X.Value is MaxValue, assumes the point is (MaxValue, MaxValue).
        /// </summary>
        /// <returns></returns>
        public Point ToPoint()
        {
            if (X.Value == double.MaxValue)
                return new Point(int.MaxValue, int.MaxValue);
            return new Point(System.Convert.ToInt32(X), System.Convert.ToInt32(Y));
        }

        /// <summary>
        /// HACK: If X.Value is MaxValue, assumes the point is (MaxValue, MaxValue).
        /// </summary>
        /// <returns></returns>
        public Vector2 ToVector2()
        {
            if (X.Value == double.MaxValue)
                return new Vector2(float.MaxValue, float.MaxValue);
            return new Vector2(System.Convert.ToSingle(X), System.Convert.ToSingle(Y));
        }


        public Point2D SwapXY()
        {
            return new Point2D(Y, X);
        }

        public override string ToString()
        {
            return "{" + Round4or6(X.Value) + ", " + Round4or6(Y.Value) + "}";
        }

        public string ToShortString
        {
            get
            {
                return "{" + ShortString(X.Value) + ", " + ShortString(Y.Value) + "}";
            }
        }


        public string ToAbbrevString()
        {
            return "{" + AbbrevString(X.Value) + ", " + AbbrevString(Y.Value) + "}";
        }

        public string ToFractionString()
        {
            return "{" + FractionString(X.Value) + ", " + FractionString(Y.Value) + "}";
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

        public Point2D Round(int digits)
        {
            return new Point2D(Math.Round(X.Value, digits), Math.Round(Y.Value, digits), null);
        }

        public Point2D Map(UnaryDeleg action)
        {
            return new Point2D(action(X.Value), action(Y.Value), null);
        }

        /// <summary>
        /// "Cross Product".
        /// TBD: Is this meaningful for Point2D, OR should this only be for Unitless2D?
        /// Or to put it another way, is "Distance.Squared" an appropriate unit type for the result?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public Distance.Squared Cross(Point2D p1, Point2D p2)
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


        public static Point2D operator -(Point2D p1, Point2D p2)
        {
            return new Point2D(p1.X - p2.X, p1.Y - p2.Y);
        }


        // Negate (unary)
        public static Point2D operator -(Point2D ptd1)
        {
            return new Point2D(-ptd1.X, -ptd1.Y);
        }

        /// <summary>
        /// "Dot Product".
        /// TBD: Is this meaningful for Point2D, OR should this only be for Unitless2D?
        /// Or to put it another way, is "Distance.Squared" an appropriate unit type for the result?
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Distance.Squared Dot(Point2D b)
        {
            return (X * b.X) + (Y * b.Y);
        }

        /// <summary>
        /// See Also "Dot" and "Cross".
        /// </summary>
        /// <param name="ptd1"></param>
        /// <param name="ptd2"></param>
        /// <returns></returns>
        public static Point2D operator *(Point2D ptd1, double2 ptd2)
        {
            return new Point2D(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
        }
        public static Point2D operator *(double2 ptd1, Point2D ptd2)
        {
            return new Point2D(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
        }

        // -- commented out; I think its okay to automatically promote to the "double" version. --
        //public static Point2D operator *(Point2D ptd1, int int2)
        //{
        //    return new Point2D(ptd1.X * int2, ptd1.Y * int2);
        //}

        //public static Point2D operator *(Point2D ptd1, float sng2)
        //{
        //    return new Point2D(ptd1.X * sng2, ptd1.Y * sng2);
        //}

        public static Point2D operator *(Point2D ptd1, double dbl2)
        {
            return new Point2D(ptd1.X * dbl2, ptd1.Y * dbl2);
        }

        public static Point2D operator *(double dbl1, Point2D ptd2)
        {
            return new Point2D(dbl1 * ptd2.X, dbl1 * ptd2.Y);
        }


        public static Point2D operator /(Point2D p1, double2 p2)
        {
            return new Point2D(p1.X / p2.X, p1.Y / p2.Y);
        }

        // -- commented out; I think its okay to automatically promote to the "double" version. --
        //public static Point2D operator /(Point2D p1, int int2)
        //{
        //    return new Point2D(p1.X / int2, p1.Y / int2);
        //}

        //public static Point2D operator /(Point2D p1, float sng2)
        //{
        //    return new Point2D(p1.X / sng2, p1.Y / sng2);
        //}

        public static Point2D operator /(Point2D p1, double dbl2)
        {
            return new Point2D(p1.X / dbl2, p1.Y / dbl2);
        }

        /// <summary>
        /// Used for "inverse"; e.g. "1 / point". HOWEVER be aware that it is w.r.t. "1 DefaultUnit".
        /// </summary>
        /// <param name="dbl1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double2 operator /(Distance dbl1, Point2D p2)
        {
            return new double2(dbl1 / p2.X, dbl1 / p2.Y);
        }


        public static Point2D Zero()
        {
            return new Point2D();
        }

        public static Point2D NaN()
        {
            return new Point2D(double.NaN, double.NaN, null);
        }

        public static readonly Point2D MinValue = new Point2D(double.MinValue, double.MinValue, null);
        public static readonly Point2D MaxValue = new Point2D(double.MaxValue, double.MaxValue, null);

        public static bool CoordIsValid(double xOrY)
        {
            return (!double.IsNaN(xOrY)) && (!double.IsInfinity(xOrY)) && (double.MaxValue > xOrY) && (double.MinValue < xOrY);
        }

        public static bool CoordIsValid(Distance xOrY)
        {
            return CoordIsValid(xOrY.Value);
        }

        // Just check one coordinate.
        public bool NotNanQuick()
        {
            return !double.IsNaN(X.Value);
        }

        public Point2D Abs()
        {
            return new Point2D(X.Abs(), Y.Abs());
        }

        // Return Min of (each coordinate of) Me and p2.
        public Point2D Min(Point2D p2)
        {
            return new Point2D(Distance.Min(X, p2.X), Distance.Min(Y, p2.Y));
        }
        // Return Max of (each coordinate of) Me and p2.
        public Point2D Max(Point2D p2)
        {
            return new Point2D(Distance.Max(X, p2.X), Distance.Max(Y, p2.Y));
        }


        public RectangleF Mult(RectangleF rect)
        {
            return new RectangleF(System.Convert.ToSingle(X * rect.Left), System.Convert.ToSingle(Y * rect.Top), System.Convert.ToSingle(X * rect.Width), System.Convert.ToSingle(Y * rect.Height));
        }


        public static Point2D[] ArrayFromPointFs(PointF[] points, Distance.UnitsType units)
        {
            int nPoints = points.Length;
            Point2D[] Point2Ds = new Point2D[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point2Ds[index] = new Point2D(points[index], units);

            return Point2Ds;
        }

        public static Point2D[] ArrayFromDouble2s(double[,] points, Distance.UnitsType units)
        {
            int lastIndex = points.GetUpperBound(0);
            Point2D[] Point2Ds = new Point2D[lastIndex + 1];

            for (int index = 0; index <= lastIndex; index++)
                Point2Ds[index] = new Point2D(points[index, 0], points[index, 1], units);

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
