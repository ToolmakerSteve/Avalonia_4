using System;
using System.Collections.Generic;
using System.Drawing;
using Urho;
using static OU.Utils;

namespace OU
{
	public struct Dist2D : IEquatable<Dist2D>
	{
		public static Dist2D[] OneElementArray(Dist2D point)
		{
			Dist2D[] points = new Dist2D[1];
			points[0] = point;
			return points;
		}

		static public DistD DistanceBetween(Dist2D cornerTL, Dist2D cornerTR)
		{
			return (cornerTR - cornerTL).Length;
		}

		public static Dist2D Average(Dist2D p0, Dist2D p1)
		{
			return new Dist2D(Utils.Average(p0.X, p1.X), Utils.Average(p0.Y, p1.Y));
		}

		public static Dist2D Average3(Dist2D p0, Dist2D p1, Dist2D p2)
		{
			return new Dist2D(Utils.Average3(p0.X, p1.X, p2.X), Utils.Average3(p0.Y, p1.Y, p2.Y));
		}


		#region --- data ----------------------------------------
		public DistD X { get; set; }
		public DistD Y { get; set; }
		#endregion


		#region --- new ----------------------------------------
		public Dist2D(DistD x, DistD y)
		{
			X = x;
			Y = y;
		}

		/// <summary>
		/// To make it clear that we are cloning.
		/// (A bit overkill for a "struct" - could just "=".)
		/// </summary>
		/// <param name="pt"></param>
		public Dist2D(Dist2D pt)
		{
			X = pt.X;
			Y = pt.Y;
		}

		public Dist2D(Dist3D pt)
		{
			X = pt.X;
			Y = pt.Y;
		}

		/// <summary>
		/// Set both X and Y to "value".
		/// </summary>
		/// <param name="value"></param>
		public Dist2D(DistD value)
		{
			X = value;
			Y = value;
		}

		/// <summary>
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="units">null means "use DistD.DefaultUnits".</param>
		public Dist2D(double x, double y, DistD.UnitsType units)
		{
			if (units == null)
			{
				X = DistD.FromDefaultUnits(x);
				Y = DistD.FromDefaultUnits(y);
			}
			else
			{
				X = DistD.FromSpecifiedUnits(x, units);
				Y = DistD.FromSpecifiedUnits(y, units);
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="units">null means "use DistD.DefaultUnits".</param>
		public Dist2D(PointF pt, DistD.UnitsType units) : this(pt.X, pt.Y, units)
		{
		}

        public Dist2D(Vec2 pt, DistD.UnitsType units) : this(pt.X, pt.Y, units)
        {
        }
        public Dist2D(Vector2 pt, DistD.UnitsType units) : this(pt.X, pt.Y, units)
        {
        }

        /// <summary>
        /// Set both X and Y to "value".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="units">null means "use DistD.DefaultUnits".</param>
        public Dist2D(double value, DistD.UnitsType units) : this(value, value, units)
		{
		}
        #endregion


        #region --- implicit/explicit conversions ----------------------------------------
        // Explicit due to loss of precision.
        static public explicit operator Vec2(Dist2D it) => new Vec2((float)it.X, (float)it.Y);
        static public explicit operator Vector2(Dist2D it) => new Vector2((float)it.X, (float)it.Y);
        // Explicit due to automatically applying default units.
        static public explicit operator Dist2D(Vec2 value) => (new Dist2D(value, null));
        static public explicit operator Dist2D(Vector2 value) => (new Dist2D(value, null));
        #endregion-



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
		public bool Equals(Dist2D other)
		{
			return (X == other.X) && (Y == other.Y);
		}

		// Compare two Dist2D's for "equal within a tolerance".
		public bool NearlyEquals(Dist2D p2, double tolerance = EpsilonForOne)
		{
			return X.NearlyEquals(p2.X, tolerance) && Y.NearlyEquals(p2.Y, tolerance);
		}


		// When geo coords are stored in a point, X is EW, Y is NS.
		public DistD LongitudeEW
		{
			get
			{
				return X;
			}
		}
		public DistD LatitudeNS
		{
			get
			{
				return Y;
			}
		}

		// Quicker than "Length" - avoids Sqrt.
		public DistD.Squared LengthSquared
		{
			get
			{
				return (X * X + Y * Y);
			}
		}

		public DistD Length
		{
			get
			{
				return (X * X + Y * Y).Sqrt();
			}
		}

		// Return point with units length (or zero, if Me is zero).
		public Dist2D Normalize
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

		public Dist3D ToPoint3D()
		{
			return new Dist3D(X, Y);
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
		public Vec2 ToVector2()
		{
			if (X.Value == double.MaxValue)
				return new Vec2(float.MaxValue, float.MaxValue);
			return new Vec2((float)(X.Value), (float)(Y.Value));
		}

		/// <summary>Converts to raw unitless vector (no units).</summary>
		/// <remarks>For situations where the units of X and Y "cancel out" (e.g. a ratio, or scalar value).</remarks>
		public Vec2D ToDouble2()
		{
			return new Vec2D(X.Value, Y.Value);
		}

		public Dist2D SwapXY()
		{
			return new Dist2D(Y, X);
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

		public void Add(Dist2D ptdPoint)
		{
			X += ptdPoint.X;
			Y += ptdPoint.Y;
		}


		public void Add(PointF pt, DistD.UnitsType units)
		{
			if (units == null)
			{
				X = DistD.FromDefaultUnits(X.Value + pt.X);
				Y = DistD.FromDefaultUnits(Y.Value + pt.Y);
			}
			else
			{
				X = X + DistD.FromSpecifiedUnits(pt.X, units);
				Y = Y + DistD.FromSpecifiedUnits(pt.Y, units);
			}
		}

		public Dist2D Round(int digits)
		{
			return new Dist2D(Math.Round(X.Value, digits), Math.Round(Y.Value, digits), null);
		}

		public Dist2D Map(UnaryDeleg action)
		{
			return new Dist2D(action(X.Value), action(Y.Value), null);
		}

		/// <summary>
		/// "Cross Product".
		/// TBD: Is this meaningful for Dist2D, OR should this only be for Unitless2D?
		/// Or to put it another way, is "DistD.Squared" an appropriate unit type for the result?
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public DistD.Squared Cross(Dist2D p1, Dist2D p2)
		{
			return (p1.Y - Y) * (p2.X - X) - (p1.X - X) * (p2.Y - Y);
		}

		// Return Normal to segment between Me and p2.
		public Dist2D SegmentNormal(Dist2D p2)
		{
			Dist2D normal1 = new Dist2D(-(p2.Y - Y), p2.X - X);
			normal1 = normal1.Normalize;
			return normal1;
		}

		public static bool operator ==(Dist2D ptd1, Dist2D ptd2)
		{
			return (ptd1.X == ptd2.X) && (ptd1.Y == ptd2.Y);
		}

		public static bool operator !=(Dist2D ptd1, Dist2D ptd2)
		{
			return !(ptd1 == ptd2);
		}


		public static Dist2D operator +(Dist2D p1, Dist2D p2)
		{
			return new Dist2D(p1.X + p2.X, p1.Y + p2.Y);
		}


		public static Dist2D operator -(Dist2D p1, Dist2D p2)
		{
			return new Dist2D(p1.X - p2.X, p1.Y - p2.Y);
		}


		// Negate (unary)
		public static Dist2D operator -(Dist2D ptd1)
		{
			return new Dist2D(-ptd1.X, -ptd1.Y);
		}

		/// <summary>
		/// "Dot Product".
		/// TBD: Is this meaningful for Dist2D, OR should this only be for Unitless2D?
		/// Or to put it another way, is "DistD.Squared" an appropriate unit type for the result?
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public DistD.Squared Dot(Dist2D b)
		{
			return (X * b.X) + (Y * b.Y);
		}

		/// <summary>
		/// See Also "Dot" and "Cross".
		/// </summary>
		/// <param name="ptd1"></param>
		/// <param name="ptd2"></param>
		/// <returns></returns>
		public static Dist2D operator *(Dist2D ptd1, Vec2D ptd2)
		{
			return new Dist2D(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
		}
		public static Dist2D operator *(Vec2D ptd1, Dist2D ptd2)
		{
			return new Dist2D(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
		}

		// -- commented out; I think its okay to automatically promote to the "double" version. --
		//public static Dist2D operator *(Dist2D ptd1, int int2)
		//{
		//    return new Dist2D(ptd1.X * int2, ptd1.Y * int2);
		//}

		//public static Dist2D operator *(Dist2D ptd1, float sng2)
		//{
		//    return new Dist2D(ptd1.X * sng2, ptd1.Y * sng2);
		//}

		public static Dist2D operator *(Dist2D ptd1, double dbl2)
		{
			return new Dist2D(ptd1.X * dbl2, ptd1.Y * dbl2);
		}

		public static Dist2D operator *(double dbl1, Dist2D ptd2)
		{
			return new Dist2D(dbl1 * ptd2.X, dbl1 * ptd2.Y);
		}


		public static Dist2D operator /(Dist2D p1, Vec2D p2)
		{
			return new Dist2D(p1.X / p2.X, p1.Y / p2.Y);
		}

		// -- commented out; I think its okay to automatically promote to the "double" version. --
		//public static Dist2D operator /(Dist2D p1, int int2)
		//{
		//    return new Dist2D(p1.X / int2, p1.Y / int2);
		//}

		//public static Dist2D operator /(Dist2D p1, float sng2)
		//{
		//    return new Dist2D(p1.X / sng2, p1.Y / sng2);
		//}

		public static Dist2D operator /(Dist2D p1, double dbl2)
		{
			return new Dist2D(p1.X / dbl2, p1.Y / dbl2);
		}

		/// <summary>
		/// Used for "inverse"; e.g. "1 / point". HOWEVER be aware that it is w.r.t. "1 DefaultUnit".
		/// </summary>
		/// <param name="dbl1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static Vec2D operator /(DistD dbl1, Dist2D p2)
		{
			return new Vec2D(dbl1 / p2.X, dbl1 / p2.Y);
		}


		public static Dist2D Zero()
		{
			return new Dist2D();
		}

		public static Dist2D NaN()
		{
			return new Dist2D(double.NaN, double.NaN, null);
		}

		public static readonly Dist2D MinValue = new Dist2D(double.MinValue, double.MinValue, null);
		public static readonly Dist2D MaxValue = new Dist2D(double.MaxValue, double.MaxValue, null);

		public static bool CoordIsValid(double xOrY)
		{
			return (!double.IsNaN(xOrY)) && (!double.IsInfinity(xOrY)) && (double.MaxValue > xOrY) && (double.MinValue < xOrY);
		}

		public static bool CoordIsValid(DistD xOrY)
		{
			return CoordIsValid(xOrY.Value);
		}

		// Just check one coordinate.
		public bool NotNanQuick()
		{
			return !double.IsNaN(X.Value);
		}

		public Dist2D Abs()
		{
			return new Dist2D(X.Abs(), Y.Abs());
		}

		// Return Min of (each coordinate of) Me and p2.
		public Dist2D Min(Dist2D p2)
		{
			return new Dist2D(DistD.Min(X, p2.X), DistD.Min(Y, p2.Y));
		}
		// Return Max of (each coordinate of) Me and p2.
		public Dist2D Max(Dist2D p2)
		{
			return new Dist2D(DistD.Max(X, p2.X), DistD.Max(Y, p2.Y));
		}


		public RectangleF Mult(RectangleF rect)
		{
			return new RectangleF((float)(X.Value * rect.Left), (float)(Y.Value * rect.Top), (float)(X.Value * rect.Width), (float)(Y.Value * rect.Height));
		}


		public static Dist2D[] ArrayFromPointFs(PointF[] points, DistD.UnitsType units)
		{
			int nPoints = points.Length;
			Dist2D[] Point2Ds = new Dist2D[nPoints - 1 + 1];

			for (int index = 0; index <= nPoints - 1; index++)
				Point2Ds[index] = new Dist2D(points[index], units);

			return Point2Ds;
		}

		public static Dist2D[] ArrayFromDouble2s(double[,] points, DistD.UnitsType units)
		{
			int lastIndex = points.GetUpperBound(0);
			Dist2D[] Point2Ds = new Dist2D[lastIndex + 1];

			for (int index = 0; index <= lastIndex; index++)
				Point2Ds[index] = new Dist2D(points[index, 0], points[index, 1], units);

			return Point2Ds;
		}

		// NOTE: "point3Ds" might be List or Array.
		public static Dist2D[] ListFromPoint3Ds(IList<Dist3D> point3Ds)
		{
			int nPoints = point3Ds.Count;
			Dist2D[] Point2Ds = new Dist2D[nPoints - 1 + 1];

			for (int index = 0; index <= nPoints - 1; index++)
				Point2Ds[index] = new Dist2D(point3Ds[index]);

			return Point2Ds;
		}

		// NOTE: "point3Ds" might be List or Array.
		public static Dist2D[] ListFromPointXZs(IList<Dist3D> point3Ds)
		{
			int nPoints = point3Ds.Count;
			Dist2D[] Point2Ds = new Dist2D[nPoints - 1 + 1];

			for (int index = 0; index <= nPoints - 1; index++)
				Point2Ds[index] = point3Ds[index].XZ();

			return Point2Ds;
		}

		// NOTE: "point2Ds" might be List or Array.
		public static PointF[] ListToPointFs(IList<Dist2D> point2Ds)
		{
			int nPoints = point2Ds.Count;
			PointF[] PointFs = new PointF[nPoints - 1 + 1];

			for (int index = 0; index <= nPoints - 1; index++)
			{
				Dist2D p = point2Ds[index];
				PointFs[index] = new PointF((float)(p.X.Value), (float)(p.Y.Value));
			}

			return PointFs;
		}


		public static List<Dist2D> CalcDeltas(IList<Dist2D> points)
		{
			List<Dist2D> deltas = new List<Dist2D>();

			Dist2D priorPt = default(Dist2D);
			bool hasPriorPt = false;
			foreach (Dist2D point in points)
			{
				if (hasPriorPt)
				{
					Dist2D delta = point - priorPt;
					deltas.Add(delta);
				}
				priorPt = point;
				hasPriorPt = true;
			}

			return deltas;
		}


        static public Dist2D FromMeters(double x, double y)
        {
            return new Dist2D(DistD.FromMeters(x), DistD.FromMeters(y));
        }
	}
}
