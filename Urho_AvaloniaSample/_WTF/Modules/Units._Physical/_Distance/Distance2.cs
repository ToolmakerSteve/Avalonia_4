using Urho;
using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Numerics;
using static WTF.Units.Utils;
using double2 = WTF.Numerics.double2;

namespace WTF.Units
{
    public struct Distance2 : IEquatable<Distance2>
    {
        public static Distance2[] OneElementArray(Distance2 point)
        {
            Distance2[] points = new Distance2[1];
            points[0] = point;
            return points;
        }

        internal static Distance DistanceBetween(Distance2 cornerTL, Distance2 cornerTR)
        {
            return (cornerTR - cornerTL).Length;
        }

        public static Distance2 Average(Distance2 p0, Distance2 p1)
        {
            return new Distance2(Utils.Average(p0.X, p1.X), Utils.Average(p0.Y, p1.Y));
        }

        public static Distance2 Average3(Distance2 p0, Distance2 p1, Distance2 p2)
        {
            return new Distance2(Utils.Average3(p0.X, p1.X, p2.X), Utils.Average3(p0.Y, p1.Y, p2.Y));
        }


        #region --- data ----------------------------------------
        public Distance X { get; set; }
        public Distance Y { get; set; }
        #endregion


        #region --- new ----------------------------------------
        public Distance2(Distance x, Distance y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// To make it clear that we are cloning.
        /// (A bit overkill for a "struct" - could just "=".)
        /// </summary>
        /// <param name="pt"></param>
        public Distance2(Distance2 pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        public Distance2(Distance3 pt)
        {
            X = pt.X;
            Y = pt.Y;
        }

        /// <summary>
        /// Set both X and Y to "value".
        /// </summary>
        /// <param name="value"></param>
        public Distance2(Distance value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="units">null means "use Distance.DefaultUnits".</param>
        public Distance2(double x, double y, Distance.UnitsType units)
        {
            if (units == null)
            {
                X = Distance.FromDefaultUnits(x);
                Y = Distance.FromDefaultUnits(y);
            }
            else
            {
                X = Distance.FromSpecifiedUnits(x, units);
                Y = Distance.FromSpecifiedUnits(y, units);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="units">null means "use Distance.DefaultUnits".</param>
        public Distance2(PointF pt, Distance.UnitsType units) : this(pt.X, pt.Y, units)
        {
        }

        public Distance2(Vector2 pt, Distance.UnitsType units) : this(pt.X, pt.Y, units)
        {
        }

        /// <summary>
        /// Set both X and Y to "value".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units">null means "use Distance.DefaultUnits".</param>
        public Distance2(double value, Distance.UnitsType units) : this(value, value, units)
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
        public bool Equals(Distance2 other)
        {
            return (X == other.X) && (Y == other.Y);
        }

        // Compare two Distance2's for "equal within a tolerance".
        public bool NearlyEquals(Distance2 p2, double tolerance = EpsilonForOne)
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
        public Distance2 Normalize
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
            return new PointF((float)(X.Value), (float)(Y.Value));
        }

        public Distance3 ToPoint3D()
        {
            return new Distance3(X, Y);
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
            return new Vector2((float)(X.Value), (float)(Y.Value));
        }

        /// <summary>Converts to raw unitless vector (no units).</summary>
        /// <remarks>For situations where the units of X and Y "cancel out" (e.g. a ratio, or scalar value).</remarks>
        public double2 ToDouble2()
        {
            return new double2(X.Value, Y.Value);
        }

        public Distance2 SwapXY()
        {
            return new Distance2(Y, X);
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

        public void Add(Distance2 ptdPoint)
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

        public Distance2 Round(int digits)
        {
            return new Distance2(Math.Round(X.Value, digits), Math.Round(Y.Value, digits), null);
        }

        public Distance2 Map(UnaryDeleg action)
        {
            return new Distance2(action(X.Value), action(Y.Value), null);
        }

        /// <summary>
        /// "Cross Product".
        /// TBD: Is this meaningful for Distance2, OR should this only be for Unitless2D?
        /// Or to put it another way, is "Distance.Squared" an appropriate unit type for the result?
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public Distance.Squared Cross(Distance2 p1, Distance2 p2)
        {
            return (p1.Y - Y) * (p2.X - X) - (p1.X - X) * (p2.Y - Y);
        }

        // Return Normal to segment between Me and p2.
        public Distance2 SegmentNormal(Distance2 p2)
        {
            Distance2 normal1 = new Distance2(-(p2.Y - Y), p2.X - X);
            normal1 = normal1.Normalize;
            return normal1;
        }

        public static bool operator ==(Distance2 ptd1, Distance2 ptd2)
        {
            return (ptd1.X == ptd2.X) && (ptd1.Y == ptd2.Y);
        }

        public static bool operator !=(Distance2 ptd1, Distance2 ptd2)
        {
            return !(ptd1 == ptd2);
        }


        public static Distance2 operator +(Distance2 p1, Distance2 p2)
        {
            return new Distance2(p1.X + p2.X, p1.Y + p2.Y);
        }


        public static Distance2 operator -(Distance2 p1, Distance2 p2)
        {
            return new Distance2(p1.X - p2.X, p1.Y - p2.Y);
        }


        // Negate (unary)
        public static Distance2 operator -(Distance2 ptd1)
        {
            return new Distance2(-ptd1.X, -ptd1.Y);
        }

        /// <summary>
        /// "Dot Product".
        /// TBD: Is this meaningful for Distance2, OR should this only be for Unitless2D?
        /// Or to put it another way, is "Distance.Squared" an appropriate unit type for the result?
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public Distance.Squared Dot(Distance2 b)
        {
            return (X * b.X) + (Y * b.Y);
        }

        /// <summary>
        /// See Also "Dot" and "Cross".
        /// </summary>
        /// <param name="ptd1"></param>
        /// <param name="ptd2"></param>
        /// <returns></returns>
        public static Distance2 operator *(Distance2 ptd1, double2 ptd2)
        {
            return new Distance2(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
        }
        public static Distance2 operator *(double2 ptd1, Distance2 ptd2)
        {
            return new Distance2(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
        }

        // -- commented out; I think its okay to automatically promote to the "double" version. --
        //public static Distance2 operator *(Distance2 ptd1, int int2)
        //{
        //    return new Distance2(ptd1.X * int2, ptd1.Y * int2);
        //}

        //public static Distance2 operator *(Distance2 ptd1, float sng2)
        //{
        //    return new Distance2(ptd1.X * sng2, ptd1.Y * sng2);
        //}

        public static Distance2 operator *(Distance2 ptd1, double dbl2)
        {
            return new Distance2(ptd1.X * dbl2, ptd1.Y * dbl2);
        }

        public static Distance2 operator *(double dbl1, Distance2 ptd2)
        {
            return new Distance2(dbl1 * ptd2.X, dbl1 * ptd2.Y);
        }


        public static Distance2 operator /(Distance2 p1, double2 p2)
        {
            return new Distance2(p1.X / p2.X, p1.Y / p2.Y);
        }

        // -- commented out; I think its okay to automatically promote to the "double" version. --
        //public static Distance2 operator /(Distance2 p1, int int2)
        //{
        //    return new Distance2(p1.X / int2, p1.Y / int2);
        //}

        //public static Distance2 operator /(Distance2 p1, float sng2)
        //{
        //    return new Distance2(p1.X / sng2, p1.Y / sng2);
        //}

        public static Distance2 operator /(Distance2 p1, double dbl2)
        {
            return new Distance2(p1.X / dbl2, p1.Y / dbl2);
        }

        /// <summary>
        /// Used for "inverse"; e.g. "1 / point". HOWEVER be aware that it is w.r.t. "1 DefaultUnit".
        /// </summary>
        /// <param name="dbl1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double2 operator /(Distance dbl1, Distance2 p2)
        {
            return new double2(dbl1 / p2.X, dbl1 / p2.Y);
        }


        public static Distance2 Zero()
        {
            return new Distance2();
        }

        public static Distance2 NaN()
        {
            return new Distance2(double.NaN, double.NaN, null);
        }

        public static readonly Distance2 MinValue = new Distance2(double.MinValue, double.MinValue, null);
        public static readonly Distance2 MaxValue = new Distance2(double.MaxValue, double.MaxValue, null);

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

        public Distance2 Abs()
        {
            return new Distance2(X.Abs(), Y.Abs());
        }

        // Return Min of (each coordinate of) Me and p2.
        public Distance2 Min(Distance2 p2)
        {
            return new Distance2(Distance.Min(X, p2.X), Distance.Min(Y, p2.Y));
        }
        // Return Max of (each coordinate of) Me and p2.
        public Distance2 Max(Distance2 p2)
        {
            return new Distance2(Distance.Max(X, p2.X), Distance.Max(Y, p2.Y));
        }


        public RectangleF Mult(RectangleF rect)
        {
            return new RectangleF((float)(X.Value * rect.Left), (float)(Y.Value * rect.Top), (float)(X.Value * rect.Width), (float)(Y.Value * rect.Height));
        }


        public static Distance2[] ArrayFromPointFs(PointF[] points, Distance.UnitsType units)
        {
            int nPoints = points.Length;
            Distance2[] Point2Ds = new Distance2[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point2Ds[index] = new Distance2(points[index], units);

            return Point2Ds;
        }

        public static Distance2[] ArrayFromDouble2s(double[,] points, Distance.UnitsType units)
        {
            int lastIndex = points.GetUpperBound(0);
            Distance2[] Point2Ds = new Distance2[lastIndex + 1];

            for (int index = 0; index <= lastIndex; index++)
                Point2Ds[index] = new Distance2(points[index, 0], points[index, 1], units);

            return Point2Ds;
        }

        // NOTE: "point3Ds" might be List or Array.
        public static Distance2[] ListFromPoint3Ds(IList<Distance3> point3Ds)
        {
            int nPoints = point3Ds.Count;
            Distance2[] Point2Ds = new Distance2[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point2Ds[index] = new Distance2(point3Ds[index]);

            return Point2Ds;
        }

        // NOTE: "point3Ds" might be List or Array.
        public static Distance2[] ListFromPointXZs(IList<Distance3> point3Ds)
        {
            int nPoints = point3Ds.Count;
            Distance2[] Point2Ds = new Distance2[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
                Point2Ds[index] = point3Ds[index].XZ();

            return Point2Ds;
        }

        // NOTE: "point2Ds" might be List or Array.
        public static PointF[] ListToPointFs(IList<Distance2> point2Ds)
        {
            int nPoints = point2Ds.Count;
            PointF[] PointFs = new PointF[nPoints - 1 + 1];

            for (int index = 0; index <= nPoints - 1; index++)
            {
                Distance2 p = point2Ds[index];
                PointFs[index] = new PointF((float)(p.X.Value), (float)(p.Y.Value));
            }

            return PointFs;
        }


        public static List<Distance2> CalcDeltas(IList<Distance2> points)
        {
            List<Distance2> deltas = new List<Distance2>();

            Distance2 priorPt = default(Distance2);
            bool hasPriorPt = false;
            foreach (Distance2 point in points)
            {
                if (hasPriorPt)
                {
                    Distance2 delta = point - priorPt;
                    deltas.Add(delta);
                }
                priorPt = point;
                hasPriorPt = true;
            }

            return deltas;
        }
    }
}
