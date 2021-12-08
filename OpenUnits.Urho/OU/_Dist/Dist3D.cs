using System;
using System.Drawing;
using Urho;
//using System.Numerics;
using static OU.Utils;

namespace OU
{
	public struct Dist3D : IEquatable<Dist3D>
	{
		#region --- static ----------------------------------------
		// E.g. Maya ground plane in XZ, plus altitude above ground.
		public static Dist3D FromXZ(Dist2D xz, DistD altitude)
		{
			// NOTE: "xz.Y" is actually "Z".
			return new Dist3D(xz.X, altitude, xz.Y);
		}

		public static Dist3D[] OneElementArray(Dist3D point)
		{
			Dist3D[] points = new Dist3D[1];
			points[0] = point;
			return points;
		}

		/// <summary>
		/// Distance in ground plane.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static DistD CalcDistance2D(Dist3D p1, Dist3D p2, bool yIsAltitude = false)
		{
			if (yIsAltitude)
				return (p2.XZ() - p1.XZ()).Length;
			else
				return (p2.To2D() - p1.To2D()).Length;
		}
		#endregion


		#region --- data ----------------------------------------
		public DistD X { get; set; }
		public DistD Y { get; set; }
		public DistD Z { get; set; }
		#endregion


		#region --- new ----------------------------------------
		public Dist3D(DistD x, DistD y, DistD z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public Dist3D(double x, double y, double z, DistD.UnitsType units)
		{
			if (units == null)
			{
				X = DistD.FromDefaultUnits(x);
				Y = DistD.FromDefaultUnits(y);
				Z = DistD.FromDefaultUnits(z);
			}
			else
			{
				X = DistD.FromSpecifiedUnits(x, units);
				Y = DistD.FromSpecifiedUnits(y, units);
				Z = DistD.FromSpecifiedUnits(z, units);
			}
		}

		public Dist3D(DistD x, DistD y)
		{
			X = x;
			Y = y;
			Z = DistD.Zero;
		}

		public Dist3D(double x, double y, DistD.UnitsType units)
		{
			if (units == null)
			{
				X = DistD.FromDefaultUnits(x);
				Y = DistD.FromDefaultUnits(y);
				Z = DistD.Zero;
			}
			else
			{
				X = DistD.FromSpecifiedUnits(x, units);
				Y = DistD.FromSpecifiedUnits(y, units);
				Z = DistD.FromSpecifiedUnits(0, units);
			}
		}

		public Dist3D(Dist2D pt) : this(pt.X, pt.Y) { }

		public Dist3D(Dist2D pt, DistD z) : this(pt.X, pt.Y, z) { }

		public Dist3D(PointF pt, DistD.UnitsType units) : this(pt.X, pt.Y, units) { }

        public Dist3D(Vec3 pt, DistD.UnitsType units) : this(pt.X, pt.Y, pt.Z, units) { }
        public Dist3D(Vector3 pt, DistD.UnitsType units) : this(pt.X, pt.Y, pt.Z, units) { }

        public Dist3D(double value, DistD.UnitsType units) : this(value, value, value, units) { }
        #endregion


        #region --- implicit/explicit conversions ----------------------------------------
        // Explicit due to loss of precision.
        static public explicit operator Vec3(Dist3D it) => new Vec3((float)it.X, (float)it.Y, (float)it.Z);
        static public explicit operator Vector3(Dist3D it) => new Vector3((float)it.X, (float)it.Y, (float)it.Z);
        // Explicit due to automatically applying default units.
        static public explicit operator Dist3D(Vec3 value) => (new Dist3D(value, null));
        static public explicit operator Dist3D(Vector3 value) => (new Dist3D(value, null));
        #endregion-


        public bool IsValid => Dist2D.CoordIsValid(X) && Dist2D.CoordIsValid(Y) && Dist2D.CoordIsValid(Z);


		public bool IsNaN => (double.IsNaN(X.Value) || double.IsNaN(Y.Value) || double.IsNaN(Z.Value));

		public bool IsZero => (X.Value == 0) && (Y.Value == 0) && (Z.Value == 0);

		/// <summary>
		/// Quicker than "Length" - avoids Sqrt.
		/// HACK CAUTION: Result is in units "DistD.DefaultUnits SQUARED".
		/// It is no longer a DISTANCE.
		/// </summary>
		public double LengthSquared => (X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value);

		public DistD Length => DistD.FromDefaultUnits(Math.Sqrt(X.Value * X.Value + Y.Value * Y.Value + Z.Value * Z.Value));

		// Return point with units length (or zero, if Me is zero).
		public Dist3D Normalized()
		{
			double len = this.Length.Value;
			return len == 0 ? this : this / len;
		}


		public override bool Equals(object obj)
		{
			/* TODO ERROR: Skipped IfDirectiveTrivia *//* TODO ERROR: Skipped DisabledTextTrivia *//* TODO ERROR: Skipped ElseDirectiveTrivia */
			if (obj is Dist3D)
				// NOTE: This is NOT recursive; it is call to "Equals(other As Dist3D)".
				return Equals((Dist3D)obj);
			else
				return false;
		}

		public override int GetHashCode()
		{
			return MakeHash(X, Y, Z);
		}



		public bool Equals(Dist3D other)
		{
			return (X.Value == other.X.Value) && (Y.Value == other.Y.Value) && (Z.Value == other.Z.Value);
		}

		/// <summary>
		/// </summary>
		/// <returns>Implicitly has units DistD.DefaultUnits.</returns>
		public Vec3 ToVector3()
		{
			return new Vec3(Convert.ToSingle(X.Value), Convert.ToSingle(Y.Value), Convert.ToSingle(Z.Value));
		}

		/// <summary>
		/// </summary>
		/// <returns>Implicitly has units DistD.DefaultUnits.</returns>
		public PointF ToPointF()
		{
			return new PointF(Convert.ToSingle(X.Value), Convert.ToSingle(Y.Value));
		}

		/// <summary>
		/// CAUTION: If altitude is in Y, use "XZ" instead.
		/// </summary>
		/// <returns></returns>
		public Dist2D To2D()
		{
			return new Dist2D(X, Y);
		}

		// E.G. extract position on Maya ground plane.
		public Dist2D XZ()
		{
			return new Dist2D(X, Z);
		}

		/// <summary>
		/// </summary>
		/// <returns>Implicitly has units DistD.DefaultUnits.</returns>
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

		public void Add(Dist3D pt2)
		{
			this.X += pt2.X;
			this.Y += pt2.Y;
			this.Z += pt2.Z;
		}

		public void Add(Dist2D pt)
		{
			this.X += pt.X;
			this.Y += pt.Y;
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

		public static bool operator ==(Dist3D ptd1, Dist3D ptd2)
		{
			return (ptd1.X == ptd2.X && ptd1.Y == ptd2.Y && ptd1.Z == ptd2.Z);
		}

		public static bool operator !=(Dist3D ptd1, Dist3D ptd2)
		{
			return !(ptd1.X == ptd2.X && ptd1.Y == ptd2.Y && ptd1.Z == ptd2.Z);
		}


		public static Dist3D operator +(Dist3D ptd1, Dist3D ptd2)
		{
			return new Dist3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
		}

		//// TODO: Not valid without a third parameter to specify Units.
		//public static Dist3D operator +(Dist3D ptd1, Vector3 ptd2)
		//{
		//    return new Dist3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
		//}

		//// TODO: Not valid without a third parameter to specify Units.
		//public static Dist3D operator +(Vector3 ptd1, Dist3D ptd2)
		//{
		//    return new Dist3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z + ptd2.Z);
		//}

		public static Dist3D operator +(Dist3D ptd1, Dist2D ptd2)
		{
			return new Dist3D(ptd1.X + ptd2.X, ptd1.Y + ptd2.Y, ptd1.Z);
		}

		// Negate
		public static Dist3D operator -(Dist3D point)
		{
			return new Dist3D(-point.X, -point.Y, -point.Z);
		}

		// Subtract
		public static Dist3D operator -(Dist3D ptd1, Dist3D ptd2)
		{
			return new Dist3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z - ptd2.Z);
		}

		//// TBD: Not valid without a third parameter to specify Units.
		//public static Dist3D operator -(Dist3D ptd1, Vector3 ptd2)
		//{
		//    return new Dist3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z - ptd2.Z);
		//}

		public static Dist3D operator -(Dist3D ptd1, Dist2D ptd2)
		{
			return new Dist3D(ptd1.X - ptd2.X, ptd1.Y - ptd2.Y, ptd1.Z);
		}


		/// <summary>
		/// Scale the axes by independent "unitless" multipliers.
		/// CAUTION: This is NOT the "dot product" of two vectors; see "DotProduct".
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="pt2"></param>
		/// <returns></returns>
		public static Dist3D operator *(Dist3D pt1, Vec3D pt2)
		{
			return new Dist3D(pt1.X * pt2.X, pt1.Y * pt2.Y, pt1.Z * pt2.Z);
		}
		public static Dist3D operator *(Vec3D pt1, Dist3D pt2)
		{
			return new Dist3D(pt1.X * pt2.X, pt1.Y * pt2.Y, pt1.Z * pt2.Z);
		}

		// -- commented out; I think its okay to automatically promote to the "double" version. --
		//public static Dist3D operator *(Dist3D pt, int n)
		//{
		//    return new Dist3D(pt.X * n, pt.Y * n, pt.Z * n);
		//}

		//public static Dist3D operator *(Dist3D pt, float n)
		//{
		//    return new Dist3D(pt.X * n, pt.Y * n, pt.Z * n);
		//}

		public static Dist3D operator *(Dist3D pt, double n)
		{
			return new Dist3D(pt.X * n, pt.Y * n, pt.Z * n);
		}

		public static Dist3D operator *(double n1, Dist3D pt)
		{
			return new Dist3D(pt.X * n1, pt.Y * n1, pt.Z * n1);
		}


		public DistD DotProduct(Dist3D p2)
		{
			return DistD.FromDefaultUnits(X.Value * p2.X.Value + Y.Value * p2.Y.Value + Z.Value * p2.Z.Value);
		}



		public static Dist3D operator /(Dist3D ptd1, Vec3D ptd2)
		{
			return new Dist3D(ptd1.X / ptd2.X, ptd1.Y / ptd2.Y, ptd1.Z / ptd2.Z);
		}

		// -- commented out; I think its okay to automatically promote to the "double" version. --
		//public static Dist3D operator /(Dist3D ptd1, int int2)
		//{
		//    return new Dist3D(ptd1.X / int2, ptd1.Y / int2, ptd1.Z / int2);
		//}

		//public static Dist3D operator /(Dist3D ptd1, float sng2)
		//{
		//    return new Dist3D(ptd1.X / sng2, ptd1.Y / sng2, ptd1.Z / sng2);
		//}

		public static Dist3D operator /(Dist3D ptd1, double dbl2)
		{
			return new Dist3D(ptd1.X / dbl2, ptd1.Y / dbl2, ptd1.Z / dbl2);
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
		public static Vec3D operator /(DistD numerator, Dist3D p2)
		{
			return new Vec3D(numerator / p2.X, numerator / p2.Y, numerator / p2.Z);
		}


		// Compare two Dist3D's for "equal within a tolerance".
		public bool NearlyEquals(Dist3D p2)
		{
			return this.X.NearlyEquals(p2.X) && this.Y.NearlyEquals(p2.Y) && this.Z.NearlyEquals(p2.Z);
		}

		// Compare two Dist3D's for "equal within a tolerance".
		public bool NearlyEquals(Dist3D p2, double tolerance)
		{
			return this.X.NearlyEquals(p2.X, tolerance) && this.Y.NearlyEquals(p2.Y, tolerance) && this.Z.NearlyEquals(p2.Z, tolerance);
		}

		public Dist3D Round2()
		{
			return new Dist3D(Utils.Round2(this.X.Value), Utils.Round2(this.Y.Value), Utils.Round2(this.Z.Value), null);
		}
		public Dist3D Round3()
		{
			return new Dist3D(Utils.Round3(this.X.Value), Utils.Round3(this.Y.Value), Utils.Round3(this.Z.Value), null);
		}

		public Dist3D SwapYZ()
		{
			return new Dist3D(this.X, this.Z, this.Y);
		}

		public static Dist3D Zero()
		{
			return new Dist3D();
		}

		public static Dist3D NaN()
		{
			return new Dist3D(double.NaN, double.NaN, double.NaN, null);
		}

		public static readonly Dist3D MinValue = new Dist3D(double.MinValue, double.MinValue, double.MinValue, null);
		public static readonly Dist3D MaxValue = new Dist3D(double.MaxValue, double.MaxValue, double.MaxValue, null);
		/// <summary>
		/// CAUTION: ASSUMES DefaultUnit is desired. Technically, this should probably be a "Unitless3D".
		/// </summary>
		public static readonly Dist3D UnitY = new Dist3D(0, 1, 0, null);
		public static readonly Dist3D UnitZ = new Dist3D(0, 0, 1, null);

		public static Dist3D[] ListFromPoint2Ds(ref Dist2D[] Point2Ds)
		{
			int nPoints = Point2Ds.Length;
			Dist3D[] Point3Ds = new Dist3D[nPoints - 1 + 1];

			for (int index = 0; index <= nPoints - 1; index++)
				Point3Ds[index] = new Dist3D(Point2Ds[index]);

			return Point3Ds;
		}

		// Usage: IList(Of Dist3D).Sort(AddressOf Dist3D.IncreasingX))
		public static int IncreasingX(Dist3D p1, Dist3D p2)
		{
			return p1.X.Value.CompareTo(p2.X.Value);
		}
		// Usage: IList(Of Dist3D).Sort(AddressOf Dist3D.IncreasingY))
		public static int IncreasingY(Dist3D p1, Dist3D p2)
		{
			return p1.Y.Value.CompareTo(p2.Y.Value);
		}
	}
	/* TODO ERROR: Skipped EndIfDirectiveTrivia */

}
