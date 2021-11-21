//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using System.Runtime.CompilerServices;
//using System.Security;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.VisualBasic;
//using System.Runtime.InteropServices;

//using static Global.Utils;
//using static System.Math;
//using System.Drawing;

//public static class mDL2DLib
//{
//    // A zig-zag rectangle-as-points has LR corner in (3).
//    // (Zig-zag Order is UL-UR-LL-LR.) (Clockwise order is UL-UR-LR-LL).
//    // CAUTION: DO NOT call on WGS-84: the variable scaling sometimes breaks the "DistanceSquared2D" test.
//    public static bool IsZigZag(Point2D[] rectPoints)
//    {
//        // TMS HACK: Some exports have LR corner in (3). Can tell which it is, by finding which is farther from UL corner.
//        Point2D p0 = rectPoints[0];
//        Point2D p2 = rectPoints[2];
//        Point2D p3 = rectPoints[3];
//        return (DistanceSquared2D(p0, p2) < DistanceSquared2D(p0, p3));
//    }

//    // Converts ZigZag index order to Clockwise index order.
//    // TECHNICALLY: result might be "counterclockwise"; we don't testing winding order.
//    // ASSUMES input has 4 elements.
//    // CAUTION: DO NOT call on WGS-84: the variable scaling breaks the "IsZigZag" test.
//    public static Point2D[] EnsureClockwise(Point2D[] rectPoints)
//    {
//        if (IsZigZag(rectPoints))
//        {
//            Point2D[] rectPoints2 = new Point2D[LastIndex(rectPoints) + 1];
//            // Swap (2) with (3).
//            rectPoints2[0] = rectPoints[0];
//            rectPoints2[1] = rectPoints[1];
//            rectPoints2[2] = rectPoints[3];
//            rectPoints2[3] = rectPoints[2];
//            return rectPoints2;
//        }
//        else
//            return rectPoints;
//    }




//    public static SizeF ToSizeF(Vector2 vec)
//    {
//        return new SizeF(vec.X, vec.Y);
//    }

//    public static SizeF ToSizeF(Size siz)
//    {
//        return new SizeF(siz.Width, siz.Height);
//    }

//    public static Size ToSize(SizeF siz, bool doRound)
//    {
//        return new Size(ToInt(siz.Width, doRound), ToInt(siz.Height, doRound));
//    }

//    // When doRound, does Round; else does Floor.
//    public static int ToInt(float value, bool doRound)
//    {
//        if (doRound)
//            return System.Convert.ToInt32(Math.Round(value));
//        else
//            return System.Convert.ToInt32(Math.Floor(value));
//    }





//    public static bool IsBad(this double value)
//    {
//        return (double.IsNaN(value) || double.IsInfinity(value));
//    }

//    public static bool IsBadOrMarker(this double value)
//    {
//        return (double.IsNaN(value) || double.IsInfinity(value) || (value == double.MinValue) || (value == double.MaxValue));
//    }


//    public delegate Point2D Point2DUnaryDeleg(Point2D value);



//    public struct Point2D : IEquatable<Point2D>
//    {
//        public static Point2D[] OneElementArray(Point2D point)
//        {
//            Point2D[] points = new Point2D[1];
//            points[0] = point;
//            return points;
//        }

//        public static Point2D Average(Point2D p0, Point2D p1)
//        {
//            return new Point2D(mDLMiscellaneous.Average(p0.X, p1.X), mDLMiscellaneous.Average(p0.Y, p1.Y));
//        }

//        public static Point2D Average3(Point2D p0, Point2D p1, Point2D p2)
//        {
//            return new Point2D(mDLMiscellaneous.Average3(p0.X, p1.X, p2.X), mDLMiscellaneous.Average3(p0.Y, p1.Y, p2.Y));
//        }


//        public double X;
//        public double Y;


//        public Point2D(PointF ptfPoint)
//        {
//            this.X = ptfPoint.X;
//            this.Y = ptfPoint.Y;
//        }

//        public Point2D(Point3D ptdPoint)
//        {
//            this.X = ptdPoint.X;
//            this.Y = ptdPoint.Y;
//        }

//        public Point2D(double dblValue)
//        {
//            this.X = dblValue;
//            this.Y = dblValue;
//        }

//        public Point2D(double dblX, double dblY)
//        {
//            this.X = dblX;
//            this.Y = dblY;
//        }

//        public Point2D(Vector2 v2)
//        {
//            this.X = v2.X;
//            this.Y = v2.Y;
//        }

//        public Point2D(Size2D size)
//        {
//            this.X = size.Width;
//            this.Y = size.Height;
//        }

//        public Point2D(SizeF size)
//        {
//            this.X = size.Width;
//            this.Y = size.Height;
//        }



//        public bool IsValid
//        {
//            get
//            {
//                return CoordIsValid(X) && CoordIsValid(Y);
//            }
//        }
//        public bool IsBad
//        {
//            get
//            {
//                return !IsValid;
//            }
//        }
//        public bool IsNaN
//        {
//            get
//            {
//                return (double.IsNaN(X) || double.IsNaN(Y));
//            }
//        }
//        // If True, then "Me" is the default value, for optional parameter "= Nothing".
//        public bool IsZero
//        {
//            get
//            {
//                return (X == 0) && (Y == 0);
//            }
//        }
//        public bool EitherIsZero
//        {
//            get
//            {
//                return (X == 0) || (Y == 0);
//            }
//        }

//        // When geo coords are stored in a point, X is EW, Y is NS.
//        public double LongitudeEW
//        {
//            get
//            {
//                return X;
//            }
//        }
//        public double LatitudeNS
//        {
//            get
//            {
//                return Y;
//            }
//        }

//        // Quicker than "Length" - avoids Sqrt.
//        public double LengthSquared
//        {
//            get
//            {
//                return (X * X + Y * Y);
//            }
//        }

//        public double Length
//        {
//            get
//            {
//                return Math.Sqrt(X * X + Y * Y);
//            }
//        }

//        // Return point with unit length (or zero, if Me is zero).
//        public Point2D Normalize
//        {
//            get
//            {
//                double len = this.Length;
//                if (len == 0)
//                    return this;
//                return this / len;
//            }
//        }


//        public PointF ToPointF()
//        {
//            if (this.X == double.MaxValue)
//                return new PointF(float.MaxValue, float.MaxValue);
//            return new PointF(System.Convert.ToSingle(this.X), System.Convert.ToSingle(this.Y));
//        }

//        public Point3D ToPoint3D()
//        {
//            return new Point3D(this.X, this.Y);
//        }

//        public Drawing.Point ToPoint()
//        {
//            if (this.X == double.MaxValue)
//                return new Drawing.Point(int.MaxValue, int.MaxValue);
//            return new Drawing.Point(System.Convert.ToInt32(this.X), System.Convert.ToInt32(this.Y));
//        }

//        public Vector2 ToVector2()
//        {
//            if (this.X == double.MaxValue)
//                return new Vector2(float.MaxValue, float.MaxValue);
//            return new Vector2(System.Convert.ToSingle(this.X), System.Convert.ToSingle(this.Y));
//        }


//        public Point2D SwapXY()
//        {
//            return new Point2D(Y, X);
//        }

//        public new bool Equals(Point2D other)
//        {
//            return (this.X == other.X) && (this.Y == other.Y);
//        }

//        public override string ToString()
//        {
//            return "{" + Round4or6(this.X) + ", " + Round4or6(this.Y) + "}";
//        }

//        public string ToShortString
//        {
//            get
//            {
//                return "{" + ShortString(this.X) + ", " + ShortString(this.Y) + "}";
//            }
//        }

//        public string ToAbbrevString()
//        {
//            return "{" + AbbrevString(this.X) + ", " + AbbrevString(this.Y) + "}";
//        }

//        public string ToFractionString()
//        {
//            return "{" + FractionString(this.X) + ", " + FractionString(this.Y) + "}";
//        }

//        public string ToF1String()
//        {
//            return string.Format("({0:f1}, {1:f1})", X, Y);
//        }

//        // Public Shared Operator =(ByVal left As PointD, ByVal right As PointD) As PointD
//        // left.X = right.X
//        // left.Y = right.Y
//        // End Operator

//        // Public Shared Operator <>(ByVal left As PointD, ByVal right As PointD) As PointD
//        // End Operator

//        public void Add(Point2D ptdPoint)
//        {
//            this.X += ptdPoint.X;
//            this.Y += ptdPoint.Y;
//        }

//        public void Add(PointF ptfPoint)
//        {
//            this.X += ptfPoint.X;
//            this.Y += ptfPoint.Y;
//        }

//        public Point2D Round(int digits)
//        {
//            return new Point2D(Math.Round(this.X, digits), Math.Round(this.Y, digits));
//        }

//        public Point2D Map(UnaryDeleg action)
//        {
//            return new Point2D(action(this.X), action(this.Y));
//        }

//        public double Cross(Point2D p1, Point2D p2)
//        {
//            return (p1.Y - this.Y) * (p2.X - this.X) - (p1.X - this.X) * (p2.Y - this.Y);
//        }

//        // Return Normal to segment between Me and p2.
//        public Point2D SegmentNormal(Point2D p2)
//        {
//            Point2D normal1 = new Point2D(-(p2.Y - this.Y), p2.X - this.X);
//            normal1 = normal1.Normalize;
//            return normal1;
//        }

//        public new static bool operator ==(Point2D ptd1, Point2D ptd2)
//        {
//            return (ptd1.X == ptd2.X) && (ptd1.Y == ptd2.Y);
//        }

//        public new static bool operator !=(Point2D ptd1, Point2D ptd2)
//        {
//            return !(ptd1 == ptd2);
//        }


//        public new static Point2D operator +(Point2D p1, Point2D p2)
//        {
//            return new Point2D(p1.X + p2.X, p1.Y + p2.Y);
//        }

//        public new static Point2D operator +(Point2D p1, Size2D size2)
//        {
//            return new Point2D(p1.X + size2.Width, p1.Y + size2.Height);
//        }

//        public new static Point2D operator +(Point2D p1, int n2)
//        {
//            return new Point2D(p1.X + n2, p1.Y + n2);
//        }

//        public new static Point2D operator +(Point2D p1, float n2)
//        {
//            return new Point2D(p1.X + n2, p1.Y + n2);
//        }

//        public new static Point2D operator +(Point2D p1, double n2)
//        {
//            return new Point2D(p1.X + n2, p1.Y + n2);
//        }


//        public new static Point2D operator -(Point2D p1, Point2D p2)
//        {
//            return new Point2D(p1.X - p2.X, p1.Y - p2.Y);
//        }

//        public new static Point2D operator -(Point2D p1, int int2)
//        {
//            return new Point2D(p1.X - int2, p1.Y - int2);
//        }

//        public new static Point2D operator -(Point2D p1, float sng2)
//        {
//            return new Point2D(p1.X - sng2, p1.Y - sng2);
//        }

//        public new static Point2D operator -(Point2D p1, double dbl2)
//        {
//            return new Point2D(p1.X - dbl2, p1.Y - dbl2);
//        }

//        // Negate (unary)
//        public new static Point2D operator -(Point2D ptd1)
//        {
//            return new Point2D(-ptd1.X, -ptd1.Y);
//        }

//        // "Dot Product"
//        public double Dot(Point2D b)
//        {
//            return (this.X * b.X) + (this.Y * b.Y);
//        }

//        // Q: When is this meaningful?
//        // See Also "Dot" and "Cross" (where?)
//        public new static Point2D operator *(Point2D ptd1, Point2D ptd2)
//        {
//            return new Point2D(ptd1.X * ptd2.X, ptd1.Y * ptd2.Y);
//        }

//        public new static Point2D operator *(Point2D ptd1, int int2)
//        {
//            return new Point2D(ptd1.X * int2, ptd1.Y * int2);
//        }

//        public new static Point2D operator *(Point2D ptd1, float sng2)
//        {
//            return new Point2D(ptd1.X * sng2, ptd1.Y * sng2);
//        }

//        public new static Point2D operator *(Point2D ptd1, double dbl2)
//        {
//            return new Point2D(ptd1.X * dbl2, ptd1.Y * dbl2);
//        }

//        public new static Point2D operator *(double dbl1, Point2D ptd2)
//        {
//            return new Point2D(dbl1 * ptd2.X, dbl1 * ptd2.Y);
//        }


//        public new static Point2D operator /(Point2D p1, Point2D p2)
//        {
//            return new Point2D(p1.X / p2.X, p1.Y / p2.Y);
//        }

//        public new static Point2D operator /(Point2D p1, int int2)
//        {
//            return new Point2D(p1.X / int2, p1.Y / int2);
//        }

//        public new static Point2D operator /(Point2D p1, float sng2)
//        {
//            return new Point2D(p1.X / sng2, p1.Y / sng2);
//        }

//        public new static Point2D operator /(Point2D p1, double dbl2)
//        {
//            return new Point2D(p1.X / dbl2, p1.Y / dbl2);
//        }

//        // Used for "inverse"; e.g. "1 / point".
//        public new static Point2D operator /(double dbl1, Point2D p2)
//        {
//            return new Point2D(dbl1 / p2.X, dbl1 / p2.Y);
//        }


//        public static Point2D Zero()
//        {
//            return new Point2D();
//        }

//        public static Point2D NaN()
//        {
//            return new Point2D(double.NaN, double.NaN);
//        }

//        public static readonly Point2D MinValue = new Point2D(double.MinValue, double.MinValue);
//        public static readonly Point2D MaxValue = new Point2D(double.MaxValue, double.MaxValue);

//        public static bool CoordIsValid(double xOrY)
//        {
//            return (!double.IsNaN(xOrY)) && (!double.IsInfinity(xOrY)) && (double.MaxValue > xOrY) && (double.MinValue < xOrY);
//        }

//        // Just check one coordinate.
//        public bool NotNanQuick()
//        {
//            return !double.IsNaN(this.X);
//        }

//        public Point2D Abs()
//        {
//            return new Point2D(Math.Abs(X), Math.Abs(Y));
//        }

//        // Return Min of (each coordinate of) Me and p2.
//        public Point2D Min(Point2D p2)
//        {
//            return new Point2D(Math.Min(this.X, p2.X), Math.Min(this.Y, p2.Y));
//        }
//        // Return Max of (each coordinate of) Me and p2.
//        public Point2D Max(Point2D p2)
//        {
//            return new Point2D(Math.Max(this.X, p2.X), Math.Max(this.Y, p2.Y));
//        }

//        // Compare two Point2D's for "equal within a tolerance".
//        public bool NearlyEquals(Point2D p2)
//        {
//            return this.X.NearlyEquals(p2.X) && this.Y.NearlyEquals(p2.Y);
//        }

//        // Compare two Point2D's for "equal within a tolerance".
//        public bool NearlyEquals(Point2D p2, double tolerance)
//        {
//            return this.X.NearlyEquals(p2.X, tolerance) && this.Y.NearlyEquals(p2.Y, tolerance);
//        }


//        public RectangleF Mult(RectangleF rect)
//        {
//            return new RectangleF(System.Convert.ToSingle(this.X * rect.Left), System.Convert.ToSingle(this.Y * rect.Top), System.Convert.ToSingle(this.X * rect.Width), System.Convert.ToSingle(this.Y * rect.Height));
//        }


//        public static Point2D[] ArrayFromPointFs(PointF[] points)
//        {
//            int nPoints = points.Length;
//            Point2D[] Point2Ds = new Point2D[nPoints - 1 + 1];

//            for (int index = 0; index <= nPoints - 1; index++)
//                Point2Ds[index] = new Point2D(points[index]);

//            return Point2Ds;
//        }

//        public static Point2D[] ArrayFromDouble2s(double[,] points)
//        {
//            int lastIndex = points.GetUpperBound(0);
//            Point2D[] Point2Ds = new Point2D[lastIndex + 1];

//            for (int index = 0; index <= lastIndex; index++)
//                Point2Ds[index] = new Point2D(points[index, 0], points[index, 1]);

//            return Point2Ds;
//        }

//        // NOTE: "point3Ds" might be List or Array.
//        public static Point2D[] ListFromPoint3Ds(IList<Point3D> point3Ds)
//        {
//            int nPoints = point3Ds.Count;
//            Point2D[] Point2Ds = new Point2D[nPoints - 1 + 1];

//            for (int index = 0; index <= nPoints - 1; index++)
//                Point2Ds[index] = new Point2D(point3Ds[index]);

//            return Point2Ds;
//        }

//        // NOTE: "point3Ds" might be List or Array.
//        public static Point2D[] ListFromPointXZs(IList<Point3D> point3Ds)
//        {
//            int nPoints = point3Ds.Count;
//            Point2D[] Point2Ds = new Point2D[nPoints - 1 + 1];

//            for (int index = 0; index <= nPoints - 1; index++)
//                Point2Ds[index] = point3Ds[index].XZ();

//            return Point2Ds;
//        }

//        // NOTE: "point2Ds" might be List or Array.
//        public static PointF[] ListToPointFs(IList<Point2D> point2Ds)
//        {
//            int nPoints = point2Ds.Count;
//            PointF[] PointFs = new PointF[nPoints - 1 + 1];

//            for (int index = 0; index <= nPoints - 1; index++)
//            {
//                Point2D p = point2Ds[index];
//                PointFs[index] = new PointF(System.Convert.ToSingle(p.X), System.Convert.ToSingle(p.Y));
//            }

//            return PointFs;
//        }


//        public static List<Point2D> CalcDeltas(IList<Point2D> points)
//        {
//            List<Point2D> deltas = new List<Point2D>();

//            Point2D priorPt = default(Point2D);
//            bool hasPriorPt = false;
//            foreach (Point2D point in points)
//            {
//                if (hasPriorPt)
//                {
//                    Point2D delta = point - priorPt;
//                    deltas.Add(delta);
//                }
//                priorPt = point;
//                hasPriorPt = true;
//            }

//            return deltas;
//        }
//    }




//    public static bool IsValid(this double value)
//    {
//        return (!double.IsNaN(value)) && (!double.IsInfinity(value)) && (double.MaxValue > value) && (double.MinValue < value);
//    }

//    // True if points are in clockwise order;
//    // False if points are in anti-clockwise (counter-clockwise) order.
//    public static bool AreClockwise(IList<Point2D> points)
//    {
//        double sum = 0;
//        for (int index = 0; index <= LastIndex(points); index++)
//        {
//            Point2D p1 = points[index];

//            // TBD: Maybe should leave off the wrap angle - suppose it is an "open" sequence of points?
//            int nextIndex = (index + 1) % points.Count;
//            Point2D p2 = points[nextIndex];

//            sum += p1.X * p2.Y - p2.X * p1.Y;
//        }

//        // Sum is negative => clockwise (True).
//        return (sum < 0);
//    }


//    // Returns a RectangleF with zero size and location.
//    public static RectangleF Empty_RectangleF()
//    {
//        return new RectangleF(0, 0, 0, 0);
//    }

//    // True if rect has zero size.
//    public static bool IsEmpty(RectangleF rect)
//    {
//        return (rect.Width == 0.0F) || (rect.Height == 0.0F);
//    }

//    // True if rectAsPts has zero size, or is nothing.
//    public static bool Rect_Empty(Point2D[] rectAsPts)
//    {
//        if (rectAsPts == null)
//            return true;

//        return Rectangle2D.FromRectAsPoints(rectAsPts).IsEmpty;
//    }

//    public static RectangleF PointFRect_To_RectangleF(PointF[] ptfRec)
//    {
//        return new RectangleF(ptfRec[0].X, ptfRec[0].Y, ptfRec[1].X - ptfRec[0].X, ptfRec[3].Y - ptfRec[0].Y);
//    }

//    public static PointF[] RectangleF_To_PointFRect(RectangleF rcfRectangle)
//    {
//        float x0 = rcfRectangle.X;
//        float y0 = rcfRectangle.Y;
//        float x1 = rcfRectangle.X + rcfRectangle.Width;
//        float y1 = rcfRectangle.Y + rcfRectangle.Height;
//        return new PointF[] { new PointF(x0, y0), new PointF(x1, y0), new PointF(x1, y1), new PointF(x0, y1) };
//    }

//    public static RectangleF MinMaxPoints_To_RectangleF(PointF minPt, PointF maxPt)
//    {
//        return new RectangleF(minPt.X, minPt.Y, maxPt.X - minPt.X, maxPt.Y - minPt.Y);
//    }


//    public static Point2D[] BoundsAtAngleDegrees(IList<Point2D> points, double angleDegrees, bool zigZag, bool highXFirst, double extendBy = 0)
//    {
//        Point2D minRotatedPt = Point2D.MaxValue;
//        Point2D maxRotatedPt = Point2D.MinValue;

//        foreach (Point2D pt in points)
//        {
//            // UNROTATE (if angleDegrees relative to x-axis, then this places points along x-axis).
//            Point2D rotatedPt = RotateByDegrees2D_New(pt, -angleDegrees);
//            AccumMinMax(rotatedPt, ref minRotatedPt, ref maxRotatedPt);
//        }

//        double x0 = minRotatedPt.X - extendBy;
//        double x1 = maxRotatedPt.X + extendBy;
//        double y0 = minRotatedPt.Y - extendBy;
//        double y1 = maxRotatedPt.Y + extendBy;

//        Point2D[] bounds = MakeRectAsPoints(x0, y0, x1, y1, zigZag, highXFirst);

//        for (int i = 0; i <= 4 - 1; i++)
//            // ROTATE.
//            bounds[i] = RotateByDegrees2D_New(bounds[i], angleDegrees);

//        return bounds;
//    }

//    // Return list of points, representing the rectangle as its four corners.
//    // If zigZag=True, then points are in "zig-zag" order:    TopLeft-TopRight-BottomLeft-BottomRight.
//    // If zigZag=False, then points are in "clockwise" order: TopLeft-TopRight-BottomRight-BottomLeft.
//    public static Point2D[] MakeRectAsPoints(double x0, double y0, double x1, double y1, bool zigZag, bool highXFirst)
//    {
//        Point2D[] pts = new Point2D[4];

//        if (highXFirst)
//        {
//            // Rectangle "top" edge is at x1, from y1..y0.
//            // This is different than "default" case, whose top edge is a constant y.
//            pts[0] = new Point2D(x1, y0); // y1)
//            pts[1] = new Point2D(x1, y1); // y0)
//            pts[2] = new Point2D(x0, y1); // y0)
//            pts[3] = new Point2D(x0, y0); // y1)

//            if (zigZag)
//                Swap(pts[2], pts[3]);
//        }
//        else
//            FillRectAsPoints(ref pts, x0, y0, x1, y1, zigZag);

//        return pts;
//    }

//    // Return list of points, representing the rectangle as its four corners.
//    // If zigZag=True, then points are in "zig-zag" order:    TopLeft-TopRight-BottomLeft-BottomRight.
//    // If zigZag=False, then points are in "clockwise" order: TopLeft-TopRight-BottomRight-BottomLeft.
//    public static void FillRectAsPoints(ref Point2D[] pts, double x0, double y0, double x1, double y1, bool zigZag)
//    {
//        pts = new Point2D[4];
//        pts[0] = new Point2D(x0, y0);
//        pts[1] = new Point2D(x1, y0);
//        pts[2] = new Point2D(x1, y1);
//        pts[3] = new Point2D(x0, y1);

//        if (zigZag)
//            Swap(pts[2], pts[3]);
//    }

//    // Return rectAsPts (cloned and) expanded to include rectangle.
//    // REQUIRES rectAsPts to Exist. (So we know whether should be y-flipped, zig-zag)
//    // Like Rectangle2D.Add, but rectangle is represented as list of 4 points.
//    // Maintains xFlip, yFlip, zigZag as needed (implicit in the ordering of the four corners).
//    public static Point2D[] AddRect_To_RectAsPoints(Point2D[] rectAsPts, Rectangle2D addedRect)
//    {
//        // Dim r1 As Point2D() = CType(rectAsPts.Clone(), Point2D())   ' tmstest

//        // Maintain flip.
//        bool zigZag = IsZigZag(rectAsPts);
//        // Must have non-negative width & height, for logic that adds rectangle.
//        // NOTE: "False": don't preserve rotation. We will lose it anyway, when do Union below.
//        bool xFlip, yFlip;
//        Rectangle2D rect = Rectangle2D.FromRectAsPoints_Cardinal(rectAsPts, false, out xFlip, out yFlip);

//        // --- Add rectangle ---
//        // FOR NOW, expands Me to non-rotated rectangle, then does union. TODO: Is there a better algorithm?
//        rect = rect.Union(addedRect);

//        Point2D[] expandedRect = rect.ToRectAsPoints(xFlip, yFlip, zigZag);

//        // 'tmstest - verify rectangle conversions (Comment out the "Add rectangle" logic.)
//        // Dim r2 As Point2D() = expandedRect
//        // If (r1(0) <> r2(0)) OrElse (r1(1) <> r2(1)) OrElse (r1(2) <> r2(2)) OrElse (r1(3) <> r2(3)) Then
//        // Dim trouble = 0
//        // End If

//        return expandedRect;
//    }


//    public static PointF RectangleCenter(RectangleF rcfRectangle)
//    {
//        return new PointF(rcfRectangle.X + rcfRectangle.Width / (double)2, rcfRectangle.Y + rcfRectangle.Height / (double)2);
//    }

//    public static PointF RectangleCenter(PointF[] ptfRec)
//    {
//        return RectangleCenter(PointFRect_To_RectangleF(ptfRec));
//    }

//    public static Point2D RectangleCenter(Point2D[] ptdRec)
//    {
//        return Average(ptdRec[0], ptdRec[2]);
//    }

//    public static PointF RectangleSize(PointF[] ptfRec)
//    {
//        RectangleF rect = PointFRect_To_RectangleF(ptfRec);
//        return new PointF(rect.Width, rect.Height);
//    }


//    // NOTE: angles that are NearlyEqual to zero are considered zero (no rotation).
//    public static bool AngleIsRotated(double rotationRadians)
//    {
//        return !rotationRadians.NearlyEquals(0.0);
//    }
//    public static bool AngleDegreesIsRotated(double rotationDegrees)
//    {
//        return !rotationDegrees.NearlyEquals(0.0);
//    }

//    public static Point3D RotateAtByDegrees2D(Point3D ptdOrigo, Point3D ptdPoint, double dblAngleDegrees)
//    {
//        // Dim ptdRet As Point3D
//        // Dim dblCurrentAngle As Double = GetAngleDegrees2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y)
//        // Dim dblDI As Double = m2DLib.Distance2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y)

//        // ptdRet.X = ptdOrigo.X + dblDI * Math.Cos(((dblCurrentAngle - dblAngleDegrees) / 180) * Math.PI)
//        // ptdRet.Y = ptdOrigo.Y + dblDI * Math.Sin(((dblCurrentAngle - dblAngleDegrees) / 180) * Math.PI)
//        // ptdRet.Z = ptdPoint.Z

//        // Return ptdRet

//        Point3D ptdRet = ptdOrigo;
//        float dblAngle = System.Convert.ToSingle(dblAngleDegrees * Math.PI / 180);

//        ptdRet.X += (ptdPoint.X - ptdOrigo.X) * Math.Cos(dblAngle) - (ptdPoint.Y - ptdOrigo.Y) * Math.Sin(dblAngle);
//        ptdRet.Y += (ptdPoint.X - ptdOrigo.X) * Math.Sin(dblAngle) + (ptdPoint.Y - ptdOrigo.Y) * Math.Cos(dblAngle);
//        ptdRet.Z = ptdPoint.Z;


//        return ptdRet;
//    }

//    public static Point2D RotateAtByDegrees2D(Point2D ptdOrigo, Point2D ptdPoint, double dblAngleDegrees)
//    {
//        Point2D ptdRet;
//        double dblCurrentAngle = GetAngleDegrees2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y);
//        double dblDI = mDL2DLib.Distance2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y);

//        // If dblAngleDegrees > 360 Then dblAngleDegrees -= 360
//        // If dblAngleDegrees < 0 Then dblAngleDegrees = 360 + dblAngleDegrees


//        ptdRet.X = ptdOrigo.X + dblDI * Math.Cos(((dblCurrentAngle - dblAngleDegrees) / 180) * Math.PI);
//        ptdRet.Y = ptdOrigo.Y + dblDI * Math.Sin(((dblCurrentAngle - dblAngleDegrees) / 180) * Math.PI);

//        return ptdRet;
//    }

//    public static PointF RotateAtByDegrees2D(PointF ptfOrigo, PointF ptfPoint, double dblAngleDegrees)
//    {
//        PointF ptfRet;
//        double dblCurrentAngle = GetAngleDegrees2D(ptfOrigo.X, ptfOrigo.Y, ptfPoint.X, ptfPoint.Y);
//        double dblDI = mDL2DLib.Distance2D(ptfOrigo.X, ptfOrigo.Y, ptfPoint.X, ptfPoint.Y);

//        ptfRet.X = System.Convert.ToSingle(ptfOrigo.X + dblDI * Math.Cos(((dblCurrentAngle - dblAngleDegrees) / 180) * Math.PI));
//        ptfRet.Y = System.Convert.ToSingle(ptfOrigo.Y + dblDI * Math.Sin(((dblCurrentAngle - dblAngleDegrees) / 180) * Math.PI));

//        return ptfRet;
//    }

//    public static Point3D RotateAtByRadians2D(Point3D ptdOrigo, Point3D ptdPoint, double dblAngleRadians)
//    {
//        Point3D ptdRet;
//        double dblCurrentAngle = GetAngleDegrees2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y);
//        double dblDI = mDL2DLib.Distance2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y);

//        ptdRet.X = ptdOrigo.X + dblDI * Math.Cos(((dblCurrentAngle) / 180) * Math.PI - dblAngleRadians);
//        ptdRet.Y = ptdOrigo.Y + dblDI * Math.Sin(((dblCurrentAngle) / 180) * Math.PI - dblAngleRadians);
//        ptdRet.Z = ptdPoint.Z;

//        return ptdRet;
//    }

//    public static Point2D RotateAtByRadians2D(Point2D ptdOrigo, Point2D ptdPoint, double dblAngleRadians)
//    {
//        Point2D ptdRet;
//        double dblCurrentAngle = GetAngleDegrees2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y);
//        double dblDI = mDL2DLib.Distance2D(ptdOrigo.X, ptdOrigo.Y, ptdPoint.X, ptdPoint.Y);

//        ptdRet.X = ptdOrigo.X + dblDI * Math.Cos(((dblCurrentAngle) / 180) * Math.PI - dblAngleRadians);
//        ptdRet.Y = ptdOrigo.Y + dblDI * Math.Sin(((dblCurrentAngle) / 180) * Math.PI - dblAngleRadians);

//        return ptdRet;
//    }

//    public static PointF RotateAtByRadians2D(PointF ptfOrigo, PointF ptfPoint, double dblAngleRadians)
//    {
//        PointF ptfRet;
//        double dblCurrentAngle = GetAngleDegrees2D(ptfOrigo.X, ptfOrigo.Y, ptfPoint.X, ptfPoint.Y);
//        double dblDI = mDL2DLib.Distance2D(ptfOrigo.X, ptfOrigo.Y, ptfPoint.X, ptfPoint.Y);

//        ptfRet.X = System.Convert.ToSingle(ptfOrigo.X + dblDI * Math.Cos(((dblCurrentAngle) / 180) * Math.PI - dblAngleRadians));
//        ptfRet.Y = System.Convert.ToSingle(ptfOrigo.Y + dblDI * Math.Sin(((dblCurrentAngle) / 180) * Math.PI - dblAngleRadians));

//        return ptfRet;
//    }

//    public static PointF Delta2D(PointF ptfOrigon, PointF ptfPoint)
//    {
//        return new PointF(ptfPoint.X - ptfOrigon.X, ptfPoint.Y - ptfOrigon.Y);
//    }

//    public static Point2D Delta2D(Point2D ptdOrigon, Point2D ptdPoint)
//    {
//        return new Point2D(ptdPoint.X - ptdOrigon.X, ptdPoint.Y - ptdOrigon.Y);
//    }

//    public static Point3D Delta2D(Point3D ptdOrigon, Point3D ptdPoint)
//    {
//        return new Point3D(ptdPoint.X - ptdOrigon.X, ptdPoint.Y - ptdOrigon.Y, 0);
//    }

//    public static Point2D Delta2D(double dblOrigonX, double dblOrigonY, double dblPointX, double dblPointY)
//    {
//        return new Point2D(dblPointX - dblOrigonX, dblPointY - dblOrigonY);
//    }

//    public static float DeltaX2D(PointF ptfOrigon, PointF ptfPoint)
//    {
//        return ptfPoint.X - ptfOrigon.X;
//    }

//    public static double DeltaX2D(Point2D ptdOrigon, Point2D ptdPoint)
//    {
//        return ptdPoint.X - ptdOrigon.X;
//    }

//    public static double DeltaX2D(Point3D ptdOrigon, Point3D ptdPoint)
//    {
//        return ptdPoint.X - ptdOrigon.X;
//    }

//    public static double DeltaX2D(double dblOrigonX, double dblPointX)
//    {
//        return dblPointX - dblOrigonX;
//    }

//    public static float DeltaY2D(PointF ptfOrigon, PointF ptfPoint)
//    {
//        return ptfPoint.Y - ptfOrigon.Y;
//    }

//    public static double DeltaY2D(Point2D ptdOrigon, Point2D ptdPoint)
//    {
//        return ptdPoint.Y - ptdOrigon.Y;
//    }

//    public static double DeltaY2D(Point3D ptdOrigon, Point3D ptdPoint)
//    {
//        return ptdPoint.Y - ptdOrigon.Y;
//    }

//    public static double DeltaY2D(double dblOrigonY, double dblPointY)
//    {
//        return dblPointY - dblOrigonY;
//    }


//    public static double Distance1D(double x1, double x2)
//    {
//        return Math.Abs(x2 - x1);
//    }


//    public static double Square(double n)
//    {
//        return n * n;
//    }

//    public static int Square(int n)
//    {
//        return n * n;
//    }

//    // PERFORMANCE: Quicker than Distance, because does not need SQRT.
//    public static double DistanceSquared3D(Point3D p1, Point3D p2)
//    {
//        return DistanceSquared3D(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z);
//    }

//    // PERFORMANCE: Quicker than Distance, because does not need SQRT.
//    public static double DistanceSquared3D(Vector3 p1, Vector3 p2)
//    {
//        return DistanceSquared3D(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z);
//    }

//    // PERFORMANCE: Quicker than Distance, because does not need SQRT.
//    public static double DistanceSquared3D(double x1, double y1, double z1, double x2, double y2, double z2)
//    {
//        var dx = x2 - x1;
//        var dy = y2 - y1;
//        var dz = z2 - z1;
//        return (dx * dx) + (dy * dy) + (dz * dz);
//    }

//    // PERFORMANCE: Quicker than Distance, because does not need SQRT.
//    public static double DistanceSquared2D(double x1, double y1, double x2, double y2)
//    {
//        var dx = x2 - x1;
//        var dy = y2 - y1;
//        return (dx * dx) + (dy * dy);
//    }

//    // PERFORMANCE: Quicker than Distance, because does not need SQRT.
//    public static double DistanceSquared2D(PointF ptfP1, PointF ptfP2)
//    {
//        return DistanceSquared2D(ptfP1.X, ptfP1.Y, ptfP2.X, ptfP2.Y);
//    }

//    // PERFORMANCE: Quicker than Distance, because does not need SQRT.
//    public static double DistanceSquared2D(Point2D ptdP1, Point2D ptdP2)
//    {
//        return DistanceSquared2D(ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y);
//    }

//    // PERFORMANCE: Quicker than Distance, because does not need SQRT.
//    public static double DistanceSquared2D(Point3D p1, Point3D p2)
//    {
//        return DistanceSquared2D(p1.X, p1.Y, p2.X, p2.Y);
//    }


//    public static double Distance2D(double dblDeltaX, double dblDeltaY)
//    {
//        return Math.Sqrt(dblDeltaX * dblDeltaX + dblDeltaY * dblDeltaY);
//    }

//    public static double Distance2D(double dblX1, double dblY1, double dblX2, double dblY2)
//    {
//        return Math.Sqrt(DistanceSquared2D(dblX1, dblY1, dblX2, dblY2));
//    }

//    public static float Distance2D(float sngX1, float sngY1, float sngX2, float sngY2)
//    {
//        return System.Convert.ToSingle(Math.Sqrt(DistanceSquared2D(sngX1, sngY1, sngX2, sngY2)));
//    }

//    public static double Distance2D(Point2D ptdP1, Point2D ptdP2)
//    {
//        return Math.Sqrt(DistanceSquared2D(ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y));
//    }

//    public static double Distance2D(Point3D ptdP1, Point3D ptdP2)
//    {
//        return Distance2D(ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y);
//    }

//    public static double Distance2D(PointF ptfP1, PointF ptfP2)
//    {
//        return Distance2D(ptfP1.X, ptfP1.Y, ptfP2.X, ptfP2.Y);
//    }

//    public static double Distance2D(PointF3D p1, PointF3D p2)
//    {
//        return Distance2D(p1.X, p1.Y, p2.X, p2.Y);
//    }

//    public static double Distance2D(Point2D[] pts)
//    {
//        if (!Exists(pts))
//            return -1;

//        double tot = 0;

//        for (int i = 1; i <= pts.Length - 1; i++)
//            tot += Distance2D(pts[i - 1], pts[i]);

//        return tot;
//    }

//    // 2D distance calculation, in X and Z.
//    public static double DistanceXZ(Point3D ptdP1, Point3D ptdP2)
//    {
//        return Distance2D(ptdP1.X, ptdP1.Z, ptdP2.X, ptdP2.Z);
//    }


//    // While technically one should add a SizeF to a PointF (and there is a built-in PointF.Add to do so),
//    // sometimes it is more convenient for the delta-point to be in a PointF.
//    public static PointF Add(PointF p1, PointF p2)
//    {
//        return new PointF(p1.X + p2.X, p1.Y + p2.Y);
//    }

//    // Drawing.Point has no operator to subtract two points, yielding the distance between them.
//    // Perhaps because it is unclear whether the result should be signed or not.
//    // The return type would be better named "DeltaPoint", but there is no such type.
//    // Could return a "Size", but then have to remember that "Width" goes with "X",
//    // and "Height" goes with "Y".
//    // Seemed easier to simply return a Point, whose "X" really means "DX" (delta-X), and "Y" means "DY".
//    public static Drawing.Point Subtract(Drawing.Point p1, Drawing.Point p2)
//    {
//        return new Drawing.Point(p1.X - p2.X, p1.Y - p2.Y);
//    }

//    // Technically, the result should be a "SizeF".
//    // But the way this is currently used, "X" and "Y" of result seemed more sensible than "Width" and "Height".
//    public static PointF Subtract(PointF p1, PointF p2)
//    {
//        return new PointF(p1.X - p2.X, p1.Y - p2.Y);
//    }

//    public static SizeF DeltaBetween(PointF Left, PointF right)
//    {
//        return new SizeF(Left.X - right.X, Left.Y - right.Y);
//    }


//    public static PointF PointFRect_Center(PointF[] ptfRec)
//    {
//        return Average(ptfRec[0], ptfRec[2]);
//    }


//    // Fixes rectangles with negative widths and/or heights
//    public static void FixRectangle(ref RectangleF rcfRectangle)
//    {
//        if (rcfRectangle.Width < 0)
//        {
//            rcfRectangle.X += rcfRectangle.Width;
//            rcfRectangle.Width = Math.Abs(rcfRectangle.Width);
//        }

//        if (rcfRectangle.Height < 0)
//        {
//            rcfRectangle.Y += rcfRectangle.Height;
//            rcfRectangle.Height = Math.Abs(rcfRectangle.Height);
//        }
//    }

//    // Fixes rectangles with negative widths and/or heights
//    // CAUTION: If rectangle is rotated, expands to surrounding unrotated rectangle.
//    public static void FixRectangle(ref Rectangle2D rcdRectangle)
//    {
//        rcdRectangle = rcdRectangle.MaybeExpandToUnrotatedRectangle(false);
//    }

//    // Ensure the 4 corners of rectangle have consistent X and Y (Min/Max).
//    // (Assumes the rectangle is intended to be grid aligned, not rotated?)
//    // If rectangle is rotated, expands it to surrounding unrotated rectangle.
//    // RESULT is clockwise (not zig-zag).
//    public static void FixRectangle(ref Point2D[] rectAsPts)
//    {
//        if (rectAsPts.Length < 4)
//            throw new InvalidProgramException("FixRectangle - missing point(s)");

//        Point2D maxPt;
//        Point2D minPt = Calculate_MinMax(rectAsPts, ref maxPt);

//        rectAsPts[0] = minPt;
//        rectAsPts[1] = new Point2D(maxPt.X, minPt.Y);
//        // RESULT is clockwise (not zig-zag).
//        rectAsPts[2] = maxPt;
//        rectAsPts[3] = new Point2D(minPt.X, maxPt.Y);
//    }

//    // SIDE-EFFECT: If rectangle is rotated, expands it to surrounding unrotated rectangled.
//    public static bool PointInsideRectangle2D(Point2D ptdPoint, RectangleF rcfRectangle)
//    {
//        FixRectangle(rcfRectangle);

//        if (ptdPoint.X > rcfRectangle.X & ptdPoint.X < rcfRectangle.X + rcfRectangle.Width & ptdPoint.Y > rcfRectangle.Y & ptdPoint.Y < rcfRectangle.Y + rcfRectangle.Height)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangle2D(Point3D ptdPoint, RectangleF rcfRectangle)
//    {
//        FixRectangle(rcfRectangle);

//        if (ptdPoint.X > rcfRectangle.X & ptdPoint.X < rcfRectangle.X + rcfRectangle.Width & ptdPoint.Y > rcfRectangle.Y & ptdPoint.Y < rcfRectangle.Y + rcfRectangle.Height)
//            return true;

//        return false;
//    }

//    // NOTE: "Strict": Excludes boundary points of rectangle. ("<" not "<=")
//    // The purpose of "tolerance" is to shrink boundary a hair,
//    // accounting for round-off error. So only return True if point is definitely inside.
//    // "tolerance": must not be negative.
//    // If > 0, then points must be that amount inside the rect bounds,
//    // to be considered inside.
//    public static bool PointInsideRectangle2D_Strict(Point2D point, Rectangle2D rect, double tolerance = 0)
//    {
//        return _PointInsideRectangle2D(point, rect, false, tolerance);
//    }

//    // NOTE: "Inclusive": Includes boundary points of rectangle. ("<=")
//    // The purpose of "tolerance" is to expand boundary a hair,
//    // to account for round-off error. So includes any points that MIGHT be inside.
//    // "tolerance": must not be negative.
//    // If > 0, then points can be up to that amount outside of the rect bounds,
//    // and will still be considered inside.
//    public static bool PointInsideRectangle2D_Inclusive(Point2D point, Rectangle2D rect, double tolerance = 0)
//    {
//        return _PointInsideRectangle2D(point, rect, true, tolerance);
//    }

//    // NOTE: "Inclusive=True": Includes boundary points of rectangle. ("<=")
//    // Tolerance either expands or shrinks "rect", depending on "inclusive".
//    // See comments on PointInsideRectangle2D_Strict and PointInsideRectangle2D_Inclusive.
//    // TODO: "tolerance" is ignored when rect.IsRotated.
//    private static bool _PointInsideRectangle2D(Point2D point, Rectangle2D rect, bool inclusive, double tolerance = 0)
//    {
//        // tolerance < 0 would be meaningless; ignore.
//        tolerance = ClampMin(tolerance, 0);

//        if (rect.IsRotated)
//        {
//            if (tolerance > 0)
//                throw new NotImplementedException("_PointInsideRectangle2D with rotated rect - tolerance is not supported.");
//            // Using A as origin, project point onto vectors forming two sides of rectangle adjacent to origin.
//            Point2D A = rect.TopLeft;
//            // "Basis Vectors": two adjacent sides of rectangle, surrounding origin.
//            Point2D AB = rect.TopRight - A;
//            Point2D AD = rect.BottomLeft - A;
//            // Vector representing point.
//            Point2D AP = point - A;
//            // Project AP on to basis vectors. Compare its magnitude (k) along each vector to the magnitude of vector endpoint.
//            // NOTE: "0" is "AA.Dot(AA)" - the projection of A onto itself.
//            // The "k" values must fall between this and the other end of vector.
//            // NOTE: A vector Dot itself "V.Dot(V)" is always non-negative, so can use Less3 rather than Between.
//            if (inclusive)
//                return Leq3(0, AP.Dot(AB), AB.Dot(AB)) && Leq3(0, AP.Dot(AD), AD.Dot(AD));
//            else
//                return Less3(0, AP.Dot(AB), AB.Dot(AB)) && Less3(0, AP.Dot(AD), AD.Dot(AD));
//        }

//        // NOTE: Some uses of Rectangle2D permit negative Width and/or Height.
//        if (inclusive)
//            return BetweenInclusive_WithTolerance(point.X, rect.X, rect.X + rect.Width, tolerance) && BetweenInclusive_WithTolerance(point.Y, rect.Y, rect.Y + rect.Height, tolerance);
//        else
//            return BetweenExclusive_WithTolerance(point.X, rect.X, rect.X + rect.Width, tolerance) && BetweenExclusive_WithTolerance(point.Y, rect.Y, rect.Y + rect.Height, tolerance);
//    }

//    public static bool PointInsideRectangle2D_WithTolerance(Point2D point, Rectangle2D rect, double tolerance)
//    {
//        if (rect.IsRotated)
//            throw new NotImplementedException("PointInsideRectangle2D_Tolerance: rect.IsRotated");

//        // NOTE: Some uses of Rectangle2D permit negative Width and/or Height.
//        return BetweenExclusive_WithTolerance(point.X, rect.X, rect.X + rect.Width, tolerance) && BetweenExclusive_WithTolerance(point.Y, rect.Y, rect.Y + rect.Height, tolerance);
//    }

//    // NOTE: "Strict": Excludes boundary points of rectangle. ("<" not "<=")
//    public static bool PointInsideRectangle2D_Strict(Point3D point, Rectangle2D rectangle)
//    {
//        return PointInsideRectangle2D_Strict(point.ToPoint2D(), rectangle);
//    }

//    // TODO: Appears to be incomplete (doesn't have logic for ptdOrigoVector < 0).
//    public static bool PointInsideRectangle2D(double dblX, double dblY, Rectangle2D rcdRectangle, Point3D ptdOrigoVector)
//    {
//        FixRectangle(ref rcdRectangle);

//        bool bolIsectX = false;
//        bool bolIsectY = false;

//        if (ptdOrigoVector.X < 0)
//        {
//        }
//        else if (dblX > rcdRectangle.X && dblX < rcdRectangle.X + rcdRectangle.Width)
//            bolIsectX = true;

//        if (ptdOrigoVector.Y < 0)
//        {
//        }
//        else if (dblY > rcdRectangle.Y && dblY < rcdRectangle.Y + rcdRectangle.Height)
//            bolIsectY = true;

//        return bolIsectX && bolIsectY;
//    }

//    public static bool PointInsideRectangleX2D(Point2D ptdPoint, RectangleF rcfRectangle)
//    {
//        FixRectangle(rcfRectangle);

//        if (ptdPoint.X > rcfRectangle.X & ptdPoint.X < rcfRectangle.X + rcfRectangle.Width)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangleX2D(Point3D ptdPoint, RectangleF rcfRectangle)
//    {
//        FixRectangle(rcfRectangle);

//        if (ptdPoint.X > rcfRectangle.X & ptdPoint.X < rcfRectangle.X + rcfRectangle.Width)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangleY2D(Point2D ptdPoint, RectangleF rcfRectangle)
//    {
//        FixRectangle(rcfRectangle);

//        if (ptdPoint.Y > rcfRectangle.Y & ptdPoint.Y < rcfRectangle.Y + rcfRectangle.Height)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangle2D(PointF pt, RectangleF rect)
//    {
//        FixRectangle(rect);

//        return (pt.X > rect.Left) && (pt.X < rect.Right) && (pt.Y > rect.Top) && (pt.Y < rect.Bottom);
//    }

//    public static bool PointWithinToleranceOfRectangle(PointF pt, RectangleF rect, float tolerance)
//    {
//        return (pt.X > rect.Left - tolerance) && (pt.X < rect.Right + tolerance) && (pt.Y > rect.Top - tolerance) && (pt.Y < rect.Bottom + tolerance);
//    }

//    public static bool PointInsideRectangleX2D(PointF ptfPoint, RectangleF rcfRectangle)
//    {
//        FixRectangle(rcfRectangle);

//        if (ptfPoint.X > rcfRectangle.X & ptfPoint.X < rcfRectangle.X + rcfRectangle.Width)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangleY2D(PointF ptfPoint, RectangleF rcfRectangle)
//    {
//        FixRectangle(rcfRectangle);

//        if (ptfPoint.Y > rcfRectangle.Y & ptfPoint.Y < rcfRectangle.Y + rcfRectangle.Height)
//            return true;

//        return false;
//    }

//    // Bounding rectangle is not rotated, so can do quicker check.
//    // "Loose": Falling exactly on boundary is considered "inside".
//    public static bool PointInsideBoundingRectangle2D_Loose(Point2D pt, Point2D[] rectangleAsPoints)
//    {
//        Point2D rectMin = rectangleAsPoints[0];
//        Point2D rectMax = rectangleAsPoints[2];
//        // ">=" and "<=": Falling exactly on boundary is considered "inside".
//        return (pt.X >= rectMin.X) && (pt.X <= rectMax.X) && (pt.Y >= rectMin.Y) && (pt.Y <= rectMax.Y);
//    }

//    // Low performance
//    public static bool PointInsideRectangle2D(Point2D ptdPoint, Point2D[] ptdRectangle)
//    {
//        Point2D[] ptdBounds = (Point2D[])ptdRectangle.Clone();

//        if (ptdBounds == null)
//            return false;

//        if (!ptdBounds[0].Y == ptdBounds[1].Y | !ptdBounds[1].X == ptdBounds[2].X)
//        {
//            double dblAngle = GetAngleDegrees2D(ptdBounds[0], ptdBounds[1]);
//            Point2D ptdOrigon;

//            ptdOrigon = ptdBounds[0];
//            ptdPoint = Delta2D(ptdOrigon, ptdPoint);

//            RotateByDegrees2D(ref ptdPoint, -dblAngle);

//            for (int intIdx = 0; intIdx <= 3; intIdx++)
//            {
//                ptdBounds[intIdx] = Delta2D(ptdOrigon, ptdBounds[intIdx]);
//                RotateByDegrees2D(ref ptdBounds[intIdx], -dblAngle);
//            }
//        }

//        if (ptdBounds[0].X > ptdBounds[1].X)
//        {
//            Swap(ptdBounds[0].X, ptdBounds[1].X);
//            Swap(ptdBounds[3].X, ptdBounds[2].X);
//        }

//        if (ptdBounds[0].Y > ptdBounds[3].Y)
//        {
//            Swap(ptdBounds[0].Y, ptdBounds[3].Y);
//            Swap(ptdBounds[1].Y, ptdBounds[2].Y);
//        }

//        if (ptdPoint.X > ptdBounds[0].X & ptdPoint.X < ptdBounds[1].X & ptdPoint.Y > ptdBounds[0].Y & ptdPoint.Y < ptdBounds[2].Y)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangle2D(Point3D ptdPoint, Point3D[] ptdRectangle)
//    {
//        Point3D[] ptdBounds = (Point3D[])ptdRectangle.Clone();

//        if (ptdBounds == null)
//            return false;

//        if (!ptdBounds[0].Y == ptdBounds[1].Y | !ptdBounds[1].X == ptdBounds[2].X)
//        {
//            double dblAngle = GetAngleDegrees2D(ptdBounds[0], ptdBounds[1]);
//            Point3D ptdOrigon;

//            ptdOrigon = ptdBounds[0];
//            ptdPoint = Delta2D(ptdOrigon, ptdPoint);

//            RotateByDegrees2D(ptdPoint, -dblAngle);

//            for (int intIdx = 0; intIdx <= 3; intIdx++)
//            {
//                ptdBounds[intIdx] = Delta2D(ptdOrigon, ptdBounds[intIdx]);
//                RotateByDegrees2D(ptdBounds[intIdx], -dblAngle);
//            }
//        }

//        if (ptdBounds[0].X > ptdBounds[1].X)
//        {
//            Swap(ptdBounds[0].X, ptdBounds[1].X);
//            Swap(ptdBounds[3].X, ptdBounds[2].X);
//        }

//        if (ptdBounds[0].Y > ptdBounds[3].Y)
//        {
//            Swap(ptdBounds[0].Y, ptdBounds[3].Y);
//            Swap(ptdBounds[1].Y, ptdBounds[2].Y);
//        }

//        if (ptdPoint.X > ptdBounds[0].X & ptdPoint.X < ptdBounds[1].X & ptdPoint.Y > ptdBounds[0].Y & ptdPoint.Y < ptdBounds[2].Y)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangleX2D(Point2D ptdPoint, Point2D[] ptdRectangle)
//    {
//        Point2D[] ptdBounds = (Point2D[])ptdRectangle.Clone();

//        if (ptdBounds == null)
//            return false;

//        if (!ptdBounds[0].Y == ptdBounds[1].Y | !ptdBounds[1].X == ptdBounds[2].X)
//        {
//            double dblAngle = GetAngleDegrees2D(ptdBounds[0], ptdBounds[1]);
//            Point2D ptdOrigon;

//            ptdOrigon = ptdBounds[0];
//            ptdPoint = Delta2D(ptdOrigon, ptdPoint);

//            RotateByDegrees2D(ref ptdPoint, -dblAngle);

//            for (int intIdx = 0; intIdx <= 3; intIdx++)
//            {
//                ptdBounds[intIdx] = Delta2D(ptdOrigon, ptdBounds[intIdx]);
//                RotateByDegrees2D(ref ptdBounds[intIdx], -dblAngle);
//            }
//        }

//        if (ptdBounds[0].X > ptdBounds[1].X)
//        {
//            Swap(ptdBounds[0].X, ptdBounds[1].X);
//            Swap(ptdBounds[3].X, ptdBounds[2].X);
//        }

//        if (ptdPoint.X > ptdBounds[0].X & ptdPoint.X < ptdBounds[1].X)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangleY2D(Point2D ptdPoint, Point2D[] ptdRectangle)
//    {
//        Point2D[] ptdBounds = (Point2D[])ptdRectangle.Clone();

//        if (ptdBounds == null)
//            return false;

//        if (!ptdBounds[0].Y == ptdBounds[1].Y | !ptdBounds[1].X == ptdBounds[2].X)
//        {
//            double dblAngle = GetAngleDegrees2D(ptdBounds[0], ptdBounds[1]);
//            Point2D ptdOrigon;

//            ptdOrigon = ptdBounds[0];
//            ptdPoint = Delta2D(ptdOrigon, ptdPoint);

//            RotateByDegrees2D(ref ptdPoint, -dblAngle);

//            for (int intIdx = 0; intIdx <= 3; intIdx++)
//            {
//                ptdBounds[intIdx] = Delta2D(ptdOrigon, ptdBounds[intIdx]);
//                RotateByDegrees2D(ref ptdBounds[intIdx], -dblAngle);
//            }
//        }

//        if (ptdBounds[0].Y > ptdBounds[3].Y)
//        {
//            Swap(ptdBounds[0].Y, ptdBounds[3].Y);
//            Swap(ptdBounds[1].Y, ptdBounds[2].Y);
//        }

//        if (ptdPoint.Y > ptdBounds[0].Y & ptdPoint.Y < ptdBounds[2].Y)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangle2D(PointF ptfPoint, PointF[] ptfRectangle)
//    {
//        if (ptfRectangle == null)
//            return false;

//        PointF[] ptfBounds = (PointF[])ptfRectangle.Clone();

//        if (ptfBounds == null)
//            return false;

//        if (!ptfBounds[0].Y == ptfBounds[1].Y | !ptfBounds[1].X == ptfBounds[2].X)
//        {
//            double dblAngle = GetAngleDegrees2D(ptfBounds[0], ptfBounds[1]);
//            PointF ptfOrigon;

//            ptfOrigon = ptfBounds[0];
//            ptfPoint = Delta2D(ptfOrigon, ptfPoint);

//            RotateByDegrees2D(ptfPoint, -dblAngle);

//            for (int intIdx = 0; intIdx <= 3; intIdx++)
//            {
//                ptfBounds[intIdx] = Delta2D(ptfOrigon, ptfBounds[intIdx]);
//                RotateByDegrees2D(ptfBounds[intIdx], -dblAngle);
//            }
//        }

//        if (ptfBounds[0].X > ptfBounds[1].X)
//        {
//            Swap(ptfBounds[0].X, ptfBounds[1].X);
//            Swap(ptfBounds[3].X, ptfBounds[2].X);
//        }

//        if (ptfBounds[0].Y > ptfBounds[3].Y)
//        {
//            Swap(ptfBounds[0].Y, ptfBounds[3].Y);
//            Swap(ptfBounds[1].Y, ptfBounds[2].Y);
//        }

//        if (ptfPoint.X > ptfBounds[0].X & ptfPoint.X < ptfBounds[1].X & ptfPoint.Y > ptfBounds[0].Y & ptfPoint.Y < ptfBounds[2].Y)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangleX2D(PointF ptfPoint, PointF[] ptfRectangle)
//    {
//        PointF[] ptfBounds = (PointF[])ptfRectangle.Clone();

//        if (ptfBounds == null)
//            return false;

//        if (!ptfBounds[0].Y == ptfBounds[1].Y | !ptfBounds[1].X == ptfBounds[2].X)
//        {
//            double dblAngle = GetAngleDegrees2D(ptfBounds[0], ptfBounds[1]);
//            PointF ptfOrigon;

//            ptfOrigon = ptfBounds[0];
//            ptfPoint = Delta2D(ptfOrigon, ptfPoint);

//            RotateByDegrees2D(ptfPoint, -dblAngle);

//            for (int intIdx = 0; intIdx <= 3; intIdx++)
//            {
//                ptfBounds[intIdx] = Delta2D(ptfOrigon, ptfBounds[intIdx]);
//                RotateByDegrees2D(ptfBounds[intIdx], -dblAngle);
//            }
//        }

//        if (ptfBounds[0].X > ptfBounds[1].X)
//        {
//            Swap(ptfBounds[0].X, ptfBounds[1].X);
//            Swap(ptfBounds[3].X, ptfBounds[2].X);
//        }

//        if (ptfPoint.X > ptfBounds[0].X & ptfPoint.X < ptfBounds[1].X)
//            return true;

//        return false;
//    }

//    public static bool PointInsideRectangleY2D(PointF ptfPoint, PointF[] ptfRectangle)
//    {
//        PointF[] ptfBounds = (PointF[])ptfRectangle.Clone();

//        if (ptfBounds == null)
//            return false;

//        if (!ptfBounds[0].Y == ptfBounds[1].Y | !ptfBounds[1].X == ptfBounds[2].X)
//        {
//            double dblAngle = GetAngleDegrees2D(ptfBounds[0], ptfBounds[1]);
//            PointF ptfOrigon;

//            ptfOrigon = ptfBounds[0];
//            ptfPoint = Delta2D(ptfOrigon, ptfPoint);

//            RotateByDegrees2D(ptfPoint, -dblAngle);

//            for (int intIdx = 0; intIdx <= 3; intIdx++)
//            {
//                ptfBounds[intIdx] = Delta2D(ptfOrigon, ptfBounds[intIdx]);
//                RotateByDegrees2D(ptfBounds[intIdx], -dblAngle);
//            }
//        }

//        if (ptfBounds[0].Y > ptfBounds[3].Y)
//        {
//            Swap(ptfBounds[0].Y, ptfBounds[3].Y);
//            Swap(ptfBounds[1].Y, ptfBounds[2].Y);
//        }

//        if (ptfPoint.Y > ptfBounds[0].Y & ptfPoint.Y < ptfBounds[2].Y)
//            return true;

//        return false;
//    }


//    // Rectangle formed by intersection of two rectangles.
//    // REQUIRES: NON-ROTATED rectangles!
//    public static Rectangle2D RectangleIntersection(Rectangle2D rect1, Rectangle2D rect2)
//    {
//        // Calc intersection.
//        double minX = Math.Max(rect1.X, rect2.X);
//        double maxX = Math.Min(rect1.Right, rect2.Right);
//        if (minX > maxX)
//            return Rectangle2D.Empty;

//        double minY = Math.Max(rect1.Y, rect2.Y);
//        double maxY = Math.Min(rect1.Bottom, rect2.Bottom);
//        if (minY > maxY)
//            return Rectangle2D.Empty;

//        return Rectangle2D.FromMinMax(minX, minY, maxX, maxY);
//    }

//    // True if 2 rectangles intersect -- this only works on rectangles without any form of rotation.
//    // High performance.
//    public static bool RectanglesIntersects2D(RectangleF rect1, RectangleF rect2)
//    {
//        // Calc intersection.
//        float minX = Math.Max(rect1.X, rect2.X);
//        float maxX = Math.Min(rect1.Right, rect2.Right);
//        if (minX > maxX)
//            return false;

//        float minY = Math.Max(rect1.Y, rect2.Y);
//        float maxY = Math.Min(rect1.Bottom, rect2.Bottom);
//        return (minY <= maxY);
//    }

//    // True if 2 rectangles intersect (overlap).
//    // REQUIRES: NON-ROTATED rectangles!
//    // High performance.
//    // "Inclusive": If barely touch, returns True (but size of overlap region would be zero).
//    // "inclusive=False": Is also true if (rare) one has zero width, but is within, and not at min/max of other rectangle.
//    // That case is used by ShapesScanBin.BreakAtIntersections.
//    public static bool RectanglesIntersects2D(Rectangle2D rect1, Rectangle2D rect2, bool inclusive = true)
//    {
//        // Calc intersection.
//        // These are the min/max values of the intersecting portion of the rectangles.
//        double minX = Math.Max(rect1.MinX, rect2.MinX);
//        double maxX = Math.Min(rect1.MaxX, rect2.MaxX);
//        if (minX > maxX)
//            return false;
//        else if (minX == maxX)
//        {
//            if (!inclusive)
//            {
//                // Not inclusive + zero thickness intersection.
//                if (((rect1.MaxX == rect1.MinX) && (rect2.MinX < minX) && (minX < rect2.MaxX)) || ((rect2.MaxX == rect2.MinX) && (rect1.MinX < minX) && (minX < rect1.MaxX)))
//                    // Rare case where one has zero width,
//                    // but is within, and not at min/max of, other rectangle.
//                    // Used by ShapesScanBin.BreakAtIntersections.
//                    // True so far - FALL THROUGH to Y test.
//                    DoNothing();
//                else
//                    return false;
//            }
//        }
//        // If get here, is True so far (X's intersect/overlap).

//        double minY = Math.Max(rect1.MinY, rect2.MinY);
//        double maxY = Math.Min(rect1.MaxY, rect2.MaxY);
//        if (inclusive)
//            return (minY <= maxY);
//        else if (minY == maxY)
//        {
//            // Not inclusive + zero thickness intersection.
//            if (((rect1.MaxY == rect1.MinY) && (rect2.MinY < minY) && (minY < rect2.MaxY)) || ((rect2.MaxY == rect2.MinY) && (rect1.MinY < minY) && (minY < rect1.MaxY)))
//                // Rare case where one has zero width,
//                // but is within, and not at min/max of, other rectangle.
//                return true;
//            else
//                return false;
//        }
//        else
//            return (minY < maxY);
//    }

//    // Segment (x1A to x1B) and segment (x2A to x2B).
//    // xs are not ordered; B may be either less or greater than A.
//    // "Inclusive": If barely touching, return True.
//    public static bool XsIntersect(double x1A, double x1B, double x2A, double x2B, bool inclusive)
//    {
//        // order each segment's x's.
//        if (x1B < x1A)
//            Swap(x1A, x1B);
//        if (x2B < x2A)
//            Swap(x2A, x2B);
//        double minX1 = x1A;
//        double maxX1 = x1B;
//        double minX2 = x2A;
//        double maxX2 = x2B;
//        // Now can do min/max algorithm for intersection, similar to X-component of RectanglesIntersects2D.
//        // These are the endpoints of the intersecting portion of the segments.
//        double minX = Math.Max(minX1, minX2);
//        double maxX = Math.Min(maxX1, maxX2);
//        // 
//        if (minX > maxX)
//            // No overlap between the segments.
//            return false;
//        else if (minX == maxX)
//        {
//            if (inclusive)
//                // When inclusive, barely touching is considered intersecting, so return True.
//                return true;
//            else if (((maxX1 == minX1) && (minX2 < minX) && (minX < maxX2)) || ((maxX2 == minX2) && (minX1 < minX) && (minX < maxX1)))
//                // Rare case where one segment has zero length,
//                // but is within, and not at min/max of, other segment.
//                // Used by ShapesScanBin.BreakAtIntersections.
//                return true;
//            else
//                return false;
//        }
//        else
//            return true;
//    }

//    // IMPORTANT: Caller responsible for comparing min to max, to see if result is empty.
//    public static MinMaxDouble XsIntersectionRegion(double x1A, double x1B, double x2A, double x2B)
//    {
//        // order each segment's x's.
//        if (x1B < x1A)
//            Swap(x1A, x1B);
//        if (x2B < x2A)
//            Swap(x2A, x2B);
//        double minX1 = x1A;
//        double maxX1 = x1B;
//        double minX2 = x2A;
//        double maxX2 = x2B;
//        // Now can do min/max algorithm for intersection, similar to X-component of RectanglesIntersects2D.
//        // These are the endpoints of the intersecting portion of the segments.
//        double minX = Math.Max(minX1, minX2);
//        double maxX = Math.Min(maxX1, maxX2);
//        return new MinMaxDouble(minX, maxX);
//    }

//    // Two rectangles, each represented by min and max. Return True if intersect.
//    public static bool MinMaxIntersect(Point2D min1, Point2D max1, Point2D min2, Point2D max2)
//    {
//        // Calc intersection.
//        double minX = Math.Max(min1.X, min2.X);
//        double maxX = Math.Min(max1.X, max2.X);
//        if (minX > maxX)
//            return false;

//        double minY = Math.Max(min1.Y, min2.Y);
//        double maxY = Math.Min(max1.Y, max2.Y);
//        return (minY <= maxY);
//    }

//    // True if 2 rectangles intersect or touch -- this only works on rectangles without any form of rotation.
//    // "or touch" because bounds may be within "tolerance" of each other.
//    // If each rectangle has its own tolerance, caller should pass in maximum of the two tolerances.
//    // Used as quick "miss"check by algorithms that need to report "touch" of shapes.
//    // REQUIRES: NON-ROTATED rectangles!
//    // High performance.
//    public static bool RectanglesIntersectOrTouch(Rectangle2D rect1, Rectangle2D rect2, double tolerance)
//    {
//        // Calc intersection.
//        double minX = Math.Max(rect1.X, rect2.X);
//        double maxX = Math.Min(rect1.Right, rect2.Right);
//        if (minX > maxX + tolerance)
//            return false;

//        double minY = Math.Max(rect1.Y, rect2.Y);
//        double maxY = Math.Min(rect1.Bottom, rect2.Bottom);
//        return (minY <= maxY + tolerance);
//    }

//    // True if 2 rectangles intersect.
//    // REQUIRES: NON-ROTATED rectangles!
//    // Low Performance.
//    public static bool RectanglesIntersects2D(Rectangle2D rcdRect1, Rectangle2D rcdRect2, Point3D ptdOrigoVector)
//    {
//        Point2D[] ptdRect1 = new Point2D[4];
//        Point2D[] ptdRect2 = new Point2D[4];

//        if (ptdOrigoVector.X < 0)
//        {
//            rcdRect1.X -= rcdRect1.Width;
//            rcdRect2.X -= rcdRect2.Width;
//        }
//        else
//        {
//        }

//        if (ptdOrigoVector.Y < 0)
//        {
//        }
//        else
//        {
//            rcdRect1.Y -= rcdRect1.Height;
//            rcdRect2.Y -= rcdRect2.Height;
//        }

//        // Only Works if rcdRect1 all positiv numbers
//        ptdRect1[0].X = rcdRect1.X; ptdRect1[0].Y = rcdRect1.Y;
//        ptdRect1[1].X = rcdRect1.X + rcdRect1.Width; ptdRect1[1].Y = rcdRect1.Y;
//        ptdRect1[2].X = rcdRect1.X; ptdRect1[2].Y = rcdRect1.Y + rcdRect1.Height;
//        ptdRect1[3].X = rcdRect1.X + rcdRect1.Width; ptdRect1[3].Y = rcdRect1.Y + rcdRect1.Height;

//        // if rcdRect2  All Positiv numbers
//        if (rcdRect1.Width > -1 & rcdRect1.Height > -1 & rcdRect2.Width > -1 & rcdRect2.Height > -1)
//        {
//            ptdRect2[0].X = rcdRect2.X; ptdRect2[0].Y = rcdRect2.Y;
//            ptdRect2[1].X = rcdRect2.X + rcdRect2.Width; ptdRect2[1].Y = rcdRect2.Y;
//            ptdRect2[2].X = rcdRect2.X; ptdRect2[2].Y = rcdRect2.Y + rcdRect2.Height;
//            ptdRect2[3].X = rcdRect2.X + rcdRect2.Width; ptdRect2[3].Y = rcdRect2.Y + rcdRect2.Height;
//        }

//        // if rcdRect2 Width and Height Negativ
//        if (rcdRect2.Width < 0 & rcdRect2.Height < 0)
//        {
//            ptdRect2[0].X = rcdRect2.X + rcdRect2.Width; ptdRect2[0].Y = rcdRect2.Y + rcdRect2.Height;
//            ptdRect2[1].X = rcdRect2.X; ptdRect2[1].Y = rcdRect2.Y + rcdRect2.Height;
//            ptdRect2[2].X = rcdRect2.X + rcdRect2.Width; ptdRect2[2].Y = rcdRect2.Y;
//            ptdRect2[3].X = rcdRect2.X; ptdRect2[3].Y = rcdRect2.Y;
//        }

//        // if rcdRect2 width negativ
//        if (rcdRect2.Width < 0 & rcdRect2.Height > -1)
//        {
//            ptdRect2[0].X = rcdRect2.X + rcdRect2.Width; ptdRect2[0].Y = rcdRect2.Y;
//            ptdRect2[1].X = rcdRect2.X; ptdRect2[1].Y = rcdRect2.Y;
//            ptdRect2[2].X = rcdRect2.X + rcdRect2.Width; ptdRect2[2].Y = rcdRect2.Y + rcdRect2.Height;
//            ptdRect2[3].X = rcdRect2.X; ptdRect2[3].Y = rcdRect2.Y + rcdRect2.Height;
//        }

//        // if rcdRect2 Height Negativ
//        if (rcdRect2.Width > -1 & rcdRect2.Height < 0)
//        {
//            ptdRect2[0].X = rcdRect2.X; ptdRect2[0].Y = rcdRect2.Y + rcdRect2.Height;
//            ptdRect2[1].X = rcdRect2.X + rcdRect2.Width; ptdRect2[1].Y = rcdRect2.Y + rcdRect2.Height;
//            ptdRect2[2].X = rcdRect2.X; ptdRect2[2].Y = rcdRect2.Y;
//            ptdRect2[3].X = rcdRect2.X + rcdRect2.Width; ptdRect2[3].Y = rcdRect2.Y;
//        }

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            if (ptdRect1[intIdx].X > ptdRect2[0].X & ptdRect1[intIdx].X < ptdRect2[1].X & ptdRect1[intIdx].Y > ptdRect2[0].Y & ptdRect1[intIdx].Y < ptdRect2[2].Y)
//                return true;

//            if (ptdRect2[intIdx].X > ptdRect1[0].X & ptdRect2[intIdx].X < ptdRect1[1].X & ptdRect2[intIdx].Y > ptdRect1[0].Y & ptdRect2[intIdx].Y < ptdRect1[2].Y)
//                return true;
//        }

//        // Special cases if one rectangle is over another, but its points are outside
//        if (ptdRect1[0].X > ptdRect2[0].X & ptdRect1[1].X < ptdRect2[1].X & ptdRect1[0].Y < ptdRect2[0].Y & ptdRect1[2].Y > ptdRect2[2].Y)
//            return true;

//        if (ptdRect2[0].X > ptdRect1[0].X & ptdRect2[1].X < ptdRect1[1].X & ptdRect2[0].Y < ptdRect1[0].Y & ptdRect2[2].Y > ptdRect1[2].Y)
//            return true;

//        return false;
//    }

//    // Low Performance.
//    public static bool RectanglesIntersects2D(Point2D[] ptdRec1, Point2D[] ptdRec2)
//    {
//        if (ptdRec1 == null | ptdRec2 == null)
//            return false;
//        if (ptdRec1.Length < 4 | ptdRec2.Length < 4)
//            throw new InvalidProgramException("RectanglesIntersects2D - missing point(s)");

//        // First check if a rectangle point is inside the other rectangle
//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptdRec1[intIdx], ptdRec2))
//                return true;
//            if (PointInsideRectangle2D(ptdRec2[intIdx], ptdRec1))
//                return true;
//        }

//        // Secondly check if the lines intersect
//        for (int i = 0; i <= 3; i++)
//        {
//            int ii = i - 1;

//            if (ii < 0)
//                ii = 3;

//            for (int j = 0; j <= 3; j++)
//            {
//                int jj = j - 1;

//                if (jj < 0)
//                    jj = 3;

//                if (LinesIntersects2D(ptdRec1[i], ptdRec1[ii], ptdRec2[j], ptdRec2[jj]))
//                    return true;
//            }
//        }

//        return false;
//    }

//    // TODO FIXA DENNA ASAP
//    // Public Function RectanglesIntersects2D(ByVal ptdRec1() As Point3D, ByVal ptdRec2() As Point3D) As Boolean
//    // If ptdRec1 Is Nothing Or ptdRec2 Is Nothing Then Return False
//    // If ptdRec1.Length < 3 Or ptdRec2.Length < 3 Then Return False

//    // Dim bolPIR1X(3) As Boolean
//    // Dim bolPIR1Y(3) As Boolean
//    // Dim bolPIR2X(3) As Boolean
//    // Dim bolPIR2Y(3) As Boolean

//    // For intIdx As Integer = 0 To 3
//    // bolPIR1X(intIdx) = PointInsideRectangleX2D(ptdRec1(intIdx), ptdRec2)
//    // bolPIR1Y(intIdx) = PointInsideRectangleY2D(ptdRec1(intIdx), ptdRec2)
//    // bolPIR2X(intIdx) = PointInsideRectangleX2D(ptdRec2(intIdx), ptdRec1)
//    // bolPIR2Y(intIdx) = PointInsideRectangleY2D(ptdRec2(intIdx), ptdRec1)

//    // If PointInsideRectangle2D(ptdRec1(intIdx), ptdRec2) Then Return True
//    // If PointInsideRectangle2D(ptdRec2(intIdx), ptdRec1) Then Return True
//    // Next

//    // If bolPIR1X(0) And bolPIR1X(1) And bolPIR2Y(1) And bolPIR2Y(2) Then Return True
//    // If bolPIR1Y(0) And bolPIR1Y(3) And bolPIR2X(0) And bolPIR2X(1) Then Return True
//    // If bolPIR2X(0) And bolPIR2X(1) And bolPIR1Y(1) And bolPIR1Y(2) Then Return True
//    // If bolPIR2Y(0) And bolPIR2Y(3) And bolPIR1X(0) And bolPIR1X(1) Then Return True

//    // Return False
//    // End Function

//    // Low Performance.
//    public static bool RectanglesIntersects2D(PointF[] ptfRec1, PointF[] ptfRec2)
//    {
//        if (ptfRec1 == null | ptfRec2 == null)
//            return false;
//        if (ptfRec1.Length < 3 | ptfRec2.Length < 3)
//            return false;

//        bool[] bolPIR1X = new bool[4];
//        bool[] bolPIR1Y = new bool[4];
//        bool[] bolPIR2X = new bool[4];
//        bool[] bolPIR2Y = new bool[4];

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            bolPIR1X[intIdx] = PointInsideRectangleX2D(ptfRec1[intIdx], ptfRec2);
//            bolPIR1Y[intIdx] = PointInsideRectangleY2D(ptfRec1[intIdx], ptfRec2);
//            bolPIR2X[intIdx] = PointInsideRectangleX2D(ptfRec2[intIdx], ptfRec1);
//            bolPIR2Y[intIdx] = PointInsideRectangleY2D(ptfRec2[intIdx], ptfRec1);

//            if (PointInsideRectangle2D(ptfRec1[intIdx], ptfRec2))
//                return true;
//            if (PointInsideRectangle2D(ptfRec2[intIdx], ptfRec1))
//                return true;
//        }

//        if (bolPIR1X[0] & bolPIR1X[1] & bolPIR2Y[1] & bolPIR2Y[2])
//            return true;
//        if (bolPIR1Y[0] & bolPIR1Y[3] & bolPIR2X[0] & bolPIR2X[1])
//            return true;
//        if (bolPIR2X[0] & bolPIR2X[1] & bolPIR1Y[1] & bolPIR1Y[2])
//            return true;
//        if (bolPIR2Y[0] & bolPIR2Y[3] & bolPIR1X[0] & bolPIR1X[1])
//            return true;

//        return false;
//    }

//    // True if 2 rectangles intersect -- this only works on rectangles without any form of rotation.
//    // High performance.
//    public static bool BoundsRectanglesIntersects2D(Point2D[] rect1, Point2D[] rect2)
//    {
//        // rect(0) contains minX,Y; rect(2) contains maxX,Y.
//        // Calc intersection.
//        double minX = Math.Max(rect1[0].X, rect2[0].X);
//        double maxX = Math.Min(rect1[2].X, rect2[2].X);
//        if (minX > maxX)
//            return false;

//        double minY = Math.Max(rect1[0].Y, rect2[0].Y);
//        double maxY = Math.Min(rect1[2].Y, rect2[2].Y);
//        return (minY <= maxY);
//    }

//    // True if 2 rectangles intersect or touch -- this only works on rectangles without any form of rotation.
//    // "or touch" because bounds may be within "tolerance" of each other.
//    // If each rectangle has its own tolerance, caller should pass in maximum of the two tolerances.
//    // Used as quick "miss"check by algorithms that need to report "touch" of shapes.
//    // High performance.
//    public static bool BoundsRectanglesIntersectOrTouch(Point2D[] rect1, Point2D[] rect2, double tolerance)
//    {
//        // rect(0) contains minX,Y; rect(2) contains maxX,Y.
//        // Calc intersection.
//        double minX = Math.Max(rect1[0].X, rect2[0].X);
//        double maxX = Math.Min(rect1[2].X, rect2[2].X);
//        if (minX > maxX + tolerance)
//            return false;

//        double minY = Math.Max(rect1[0].Y, rect2[0].Y);
//        double maxY = Math.Min(rect1[2].Y, rect2[2].Y);
//        return (minY <= maxY + tolerance);
//    }


//    // Public Sub Test_LinesIntersects2D()
//    // End Sub

//    public static bool LinesIntersects2D(PointF p1, PointF p2, PointF p3, PointF p4)
//    {
//        Point2D p2Isect;
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, ref p2Isect);
//    }

//    // Legacy behavior: parallel lines return False, even if colinear and touching.
//    public static bool LinesIntersects2D(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
//    {
//        if (p1.IsBad || p2.IsBad || p3.IsBad || p4.IsBad)
//            throw new InvalidDataException("LinesIntersects2D - bad point");
//        Point2D pIsect;
//        LineOverlap overlap;
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, false, ref pIsect, ref overlap);
//    }

//    // Legacy behavior: parallel lines return Nothing, even if colinear and touching.
//    public static Point2D? LinesIntersection2D(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
//    {
//        if (p1.IsBad || p2.IsBad || p3.IsBad || p4.IsBad)
//            throw new InvalidDataException("LinesIntersects2D - bad point");
//        Point2D pIsect;
//        LineOverlap overlap;
//        bool result = LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, false, ref pIsect, ref overlap);
//        // BUG: This shorthand returns a Point2D with default value, instead of a Nullable with Nothing.
//        // BUG Return If(result, pIsect, Nothing)
//        if (result)
//            return pIsect;
//        else
//            return default(Point2D?);
//    }

//    public static bool LinesIntersects2D_AllowColinearTouch(Point2D p1, Point2D p2, Point2D p3, Point2D p4)
//    {
//        Point2D pIsect;
//        LineOverlap overlap;
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, true, ref pIsect, ref overlap);
//    }

//    public static bool LinesIntersects2D_AllowColinearTouch(Point2D p1, Point2D p2, Point2D p3, Point2D p4, ref Point2D pIsect)
//    {
//        LineOverlap overlap;
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, true, ref pIsect, ref overlap);
//    }

//    // Parallel segments which Overlap (by more than a single point) Return False,
//    // because returning one of the (multiple) points of contact could mislead the caller.
//    // If fails, pIsect=Point2D.MaxValue.
//    public static bool LinesIntersects2D_AllowColinearTouch(Point2D p1, Point2D p2, Point2D p3, Point2D p4, ref Point2D pIsect, out LineOverlap overlap, double tolerance = 0)
//    {
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, true, ref pIsect, ref overlap, tolerance);
//    }

//    // True if segment(p1, p2) intersects segment(p3, p4)
//    public static bool LinesIntersects2D(Point3D p1, Point3D p2, Point3D p3, Point3D p4)
//    {
//        Point2D p2Isect;
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, ref p2Isect);
//    }

//    public static bool LinesIntersects2D(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
//    {
//        Point2D pIsect;
//        return LinesIntersects2D(ax, ay, bx, by, cx, cy, dx, dy, ref pIsect);
//    }


//    // Z coords ignored; Returned z is zero.
//    public static bool LinesIntersects2D(Point3D p1, Point3D p2, Point3D p3, Point3D p4, out Point3D pIsect)
//    {
//        Point2D p2Isect;
//        bool result = LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, ref p2Isect);
//        pIsect = new Point3D(p2Isect.X, p2Isect.Y);
//        return result;
//    }

//    public static bool LinesIntersects2D(Point2D p1, Point2D p2, Point2D p3, Point2D p4, out Point2D intersectionPt)
//    {
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, ref intersectionPt);
//    }

//    public static bool LinesIntersects2D(Point2D p1, Point2D p2, Point2D p3, Point2D p4, out Point2D pIsect, out LineOverlap overlap)
//    {
//        return LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, false, ref pIsect, ref overlap);
//    }

//    public static bool LinesIntersects2D(PointF p1, PointF p2, PointF p3, PointF p4, ref PointF pIsect)
//    {
//        Point2D p2Isect;
//        bool result = LinesIntersects2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, ref p2Isect);
//        pIsect = new PointF(System.Convert.ToSingle(p2Isect.X), System.Convert.ToSingle(p2Isect.Y));
//        return result;
//    }

//    // Legacy behavior: parallel lines return False, even if colinear and touching.
//    public static bool LinesIntersects2D(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy, out Point2D pIsect)
//    {
//        LineOverlap overlap;
//        return LinesIntersects2D(ax, ay, bx, by, cx, cy, dx, dy, false, ref pIsect, ref overlap);
//    }

//    // allowColinearTouch=False => Legacy behavior: parallel lines return False, even if colinear and touching.
//    // Parallel segments which Overlap (by more than a single point) Return False,
//    // because returning one of the (multiple) points of contact could mislead the caller.
//    // If fails, pIsect=Point2D.MaxValue.
//    public static bool LinesIntersects2D(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy, bool allowColinearTouch, out Point2D pIsect, out LineOverlap overlap, double tolerance = 0)
//    {
//        // Default tolerance=NearZero.
//        if (tolerance == 0)
//            tolerance = NearZero;

//        LineOverlap overlap1, overlap2;
//        Point2D outP2;
//        pIsect = LinesIntersectsAt2D(ax, ay, bx, by, cx, cy, dx, dy, tolerance, ref overlap, ref overlap1, ref overlap2, ref outP2);

//        // ' verify
//        // Dim result As Boolean = (overlap = LineOverlap.Crossing) OrElse (overlap = LineOverlap.CrossingTouch)
//        // Dim oldIsect As Point2D
//        // Dim oldResult As Boolean = LinesIntersects2D_OLD(ax, ay, bx, by, cx, cy, dx, dy, oldIsect)
//        // If result <> oldResult OrElse (result AndAlso Not oldIsect.NearlyEquals(pIsect)) Then
//        // If result AndAlso (Not oldResult) AndAlso (overlap = LineOverlap.CrossingTouch) AndAlso oldIsect.NearlyEquals(pIsect) Then
//        // ' Check whether new is better answer.
//        // If pIsect.NearlyEquals(New Point2D(ax, ay)) OrElse _
//        // pIsect.NearlyEquals(New Point2D(bx, by)) OrElse _
//        // pIsect.NearlyEquals(New Point2D(cx, cy)) OrElse _
//        // pIsect.NearlyEquals(New Point2D(dx, dy)) Then
//        // Dim better = 0
//        // Else
//        // Dim trouble = 0
//        // End If
//        // Else
//        // If oldResult AndAlso (Not result) AndAlso ColinearIntersect(overlap) Then
//        // Dim better = 0
//        // Else
//        // Dim trouble2 = 0
//        // End If
//        // End If
//        // End If

//        return (overlap == LineOverlap.Crossing) || (overlap == LineOverlap.CrossingTouch) || (allowColinearTouch && (overlap == LineOverlap.TouchEnd));
//    }

//    public static bool LinesIntersects2D_OLD(double dblP1X, double dblP1Y, double dblP2X, double dblP2Y, double dblP3X, double dblP3Y, double dblP4X, double dblP4Y, ref Point2D ptdIsect)
//    {
//        ptdIsect = LinesIntersectsAt2D_OLD(dblP1X, dblP1Y, dblP2X, dblP2Y, dblP3X, dblP3Y, dblP4X, dblP4Y);
//        Point2D ptdDelta = Delta2D(dblP1X, dblP1Y, dblP2X, dblP2Y);


//        if (Math.Abs(ptdDelta.X) > Math.Abs(ptdDelta.Y))
//        {
//            if (ptdIsect.X >= Math.Min(dblP1X, dblP2X) & ptdIsect.X <= Math.Max(dblP1X, dblP2X))
//            {
//                Point2D ptdDelta2 = Delta2D(dblP3X, dblP3Y, dblP4X, dblP4Y);

//                if (Math.Abs(ptdDelta2.X) > Math.Abs(ptdDelta2.Y))
//                {
//                    if (ptdIsect.X >= Math.Min(dblP3X, dblP4X) & ptdIsect.X <= Math.Max(dblP3X, dblP4X))
//                        return true;
//                }
//                else if (ptdIsect.Y >= Math.Min(dblP3Y, dblP4Y) & ptdIsect.Y <= Math.Max(dblP3Y, dblP4Y))
//                    return true;
//            }
//        }
//        else if (ptdIsect.Y >= Math.Min(dblP1Y, dblP2Y) & ptdIsect.Y <= Math.Max(dblP1Y, dblP2Y))
//        {
//            Point2D ptdDelta2 = Delta2D(dblP3X, dblP3Y, dblP4X, dblP4Y);

//            if (Math.Abs(ptdDelta2.X) > Math.Abs(ptdDelta2.Y))
//            {
//                if (ptdIsect.X >= Math.Min(dblP3X, dblP4X) & ptdIsect.X <= Math.Max(dblP3X, dblP4X))
//                    return true;
//            }
//            else if (ptdIsect.Y >= Math.Min(dblP3Y, dblP4Y) & ptdIsect.Y <= Math.Max(dblP3Y, dblP4Y))
//                return true;
//        }

//        return false;
//    }


//    // "allowColinear=False": Legacy behavior: parallel lines return False, even if colinear and touching.
//    // Returns crossing point. If no valid crossing point, returns Point2D.MaxValue.
//    public static Point2D LinesIntersectsAt2D(Point2D a, Point2D b, Point2D c, Point2D d, bool allowColinear = false)
//    {
//        return LinesIntersectsAt2D(a.X, a.Y, b.X, b.Y, c.X, c.Y, d.X, d.Y, allowColinear);
//    }

//    // Returns crossing point. If no valid crossing point, returns Point2D.MaxValue.
//    // "allowColinear=False": Legacy behavior: parallel lines return False, even if colinear and touching.
//    // Like previous version, may return a point that extends the segments (each segment is extended into a near-infinite line).
//    public static Point2D LinesIntersectsAt2D(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy, bool allowColinear)
//    {
//        LineOverlap overlap, overlap1, overlap2;
//        Point2D outP2;
//        Point2D outP = LinesIntersectsAt2D(ax, ay, bx, by, cx, cy, dx, dy, NearZero, ref overlap, ref overlap1, ref overlap2, ref outP2);
//        bool result = (overlap == LineOverlap.Crossing) || (overlap == LineOverlap.CrossingTouch);
//        if (allowColinear)
//            // TODO: When InsideTouchEnd, the intersection point is ambiguous. What is "outP" set to in this case?
//            result = result || (overlap == LineOverlap.TouchEnd) || (overlap == LineOverlap.InsideTouchEnd);

//        // ' verify
//        // Dim oldIsect As Point2D
//        // Dim oldResult As Boolean = LinesIntersects2D_OLD(ax, ay, bx, by, cx, cy, dx, dy, oldIsect)
//        // If result <> oldResult OrElse (result AndAlso Not oldIsect.NearlyEquals(outP)) Then
//        // If result AndAlso (Not oldResult) AndAlso (overlap = LineOverlap.CrossingTouch) AndAlso oldIsect.NearlyEquals(outP) Then
//        // ' Check whether new is better answer.
//        // If outP.NearlyEquals(New Point2D(ax, ay)) OrElse _
//        // outP.NearlyEquals(New Point2D(bx, by)) OrElse _
//        // outP.NearlyEquals(New Point2D(cx, cy)) OrElse _
//        // outP.NearlyEquals(New Point2D(dx, dy)) Then
//        // Dim better = 0
//        // Else
//        // Dim trouble = 0
//        // End If
//        // Else
//        // If oldResult AndAlso (Not result) AndAlso ColinearIntersect(overlap) Then
//        // Dim better = 0
//        // Else
//        // Dim trouble2 = 0
//        // End If
//        // End If
//        // End If

//        // Like previous version, may return a point that extends the segments (each segment is extended into a near-infinite line).
//        if (result || (overlap == LineOverlap.CrossingOutside))
//            return outP;
//        return Point2D.MaxValue; // Consider invalid.
//    }

//    // segment "1" is (a,b); segment "2" is (c,d).
//    // The returned point MIGHT BE OUTSIDE of (a, b) and (c, d) -- it is the intersection of the infinite lines extended from those segments.
//    // Returns Point2D.MaxValue if fails (A result which can be compared, and will always fall outside of the segments.)
//    // Sets "overlap1" to status of segment 1, "overlap2" to status of segment 2, "overlap" to combined status.
//    // 
//    // Note: Consider overlap=Outside, overlap1=Touch, overlap2=Outside. This indicates that overlap1 touches THE EXTENSION OF segment 2.
//    // If want to know whether segment 1 truly touches segment 2, must first check overlap=Touch. (or equivalently, check that 2 is Crossing or CrossingTouch)
//    // REQUIRES: Caller has cleaned polygons, so a<>b and c<>d. (Line-segments are not zero-length.)
//    // 
//    // TODO PERFORMANCE: If caller doesn't care about cause of non-intersect (e.g. whether is parallel or crossing outside),
//    // could first check whether rect bounds w/i tolerance of each other. If not, immediately fail, with overlap=BoundsNoOverlap.
//    // Based on http://en.wikipedia.org/wiki/Line-line_intersection; "Given two points on each line".
//    public static Point2D LinesIntersectsAt2D(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double tolerance, out LineOverlap overlap, out LineOverlap overlap1, out LineOverlap overlap2, out Point2D outP2)
//    {
//        overlap1 = LineOverlap.Undefined; overlap2 = LineOverlap.Undefined;
//        outP2 = Point2D.MaxValue;    // i.e. undefined.

//        // NOTE: None of these are affected by the origX1/Y1 translation later, so safe to compute now.
//        // (Didn't want to modify x's and y's until sure we aren't calling LinesOverlapOrTouch.)
//        double x12 = (x1 - x2);
//        double x34 = (x3 - x4);
//        double y12 = (y1 - y2);
//        double y34 = (y3 - y4);

//        double scale = Math.Max(Math.Max(Math.Abs(x12), Math.Abs(x34)), Math.Max(Math.Abs(y12), Math.Abs(y34)));
//        // If values are within a small range, tolerances must be correspondingly smaller.
//        double scale_clamped = Math.Min(1.0, scale);
//        // But there is a limit to how low it is meaningful to go.
//        double tolerance_scaled = ClampMin(scale_clamped * tolerance, ScaledToleranceDouble);

//        var determinant = x12 * y34 - y12 * x34;

//        // Parallel?
//        double absDeterminant = Math.Abs(determinant);
//        double scale_clamped_squared = scale_clamped * scale_clamped;
//        // TBD: Why is this the test to decide whether is parallel?
//        // TBD: Why did it originally use "* NearZero" instead of "* tolerance"?  Was treating as parallel when it shouldn't.
//        bool asParallel = (absDeterminant <= scale_clamped_squared * tolerance);
//        if ((absDeterminant <= scale_clamped_squared * OneThousandth) && (!asParallel))
//        {
//            // Tolerance might make it necessary to treat as parallel.
//            // Work with the shorter segment.
//            double xa, ya, xb, yb, xd, yd;
//            if (DistanceSquared2D(x1, y1, x2, y2) <= DistanceSquared2D(x3, y3, x4, y4))
//            {
//                // (p1,p2) is shorter.
//                xd = x12; yd = y12; xa = 0; ya = 0; xb = x34; yb = y34;
//            }
//            else
//            {
//                // (p3,p4) is shorter.
//                xd = x34; yd = y34; xa = 0; ya = 0; xb = x12; yb = y12;
//            }
//            double distSq = PointDeltaToLineExtended2D(new Point2D(xd, yd), new Point2D(xa, ya), new Point2D(xb, yb)).LengthSquared;
//            // If within tolerance, treat as parallel.
//            if (distSq <= tolerance_scaled * tolerance_scaled)
//                asParallel = true;
//            else
//                Test();
//        }

//        // If asParallel Then Return LinesOverlapOrTouch(x1, y1, x2, y2, x3, y3, x4, y4, tolerance_scaled, overlap, overlap1, overlap2, outP2)
//        if (asParallel)
//        {
//            Point2D outP1 = LinesOverlapOrTouch(x1, y1, x2, y2, x3, y3, x4, y4, tolerance_scaled, ref overlap, ref overlap1, ref overlap2, ref outP2);
//            if (overlap != LineOverlap.NotParallel)
//                return outP1;
//            else
//                // Parallel logic rejected it. Fall through to crossing logic.
//                // CAUTION: Falling through may lead to divide by zero, if determinant = 0.
//                Test();
//        }

//        // Better precision, by moving values closer to zero.
//        double origX1 = x1; double origY1 = y1;
//        x1 -= origX1; x2 -= origX1; x3 -= origX1; x4 -= origX1;
//        y1 -= origY1; y2 -= origY1; y3 -= origY1; y4 -= origY1;

//        // Not parallel.
//        double xy12 = (x1 * y2 - y1 * x2);
//        double xy34 = (x3 * y4 - y3 * x4);
//        double rx;
//        double ry;
//        bool fail = false;
//        try
//        {
//            // CAUTION: Might divide by zero, or overflow.
//            rx = (xy12 * x34 - x12 * xy34) / determinant;
//            ry = (xy12 * y34 - y12 * xy34) / determinant;
//            fail = !rx.IsValid() || !ry.IsValid();
//        }
//        catch (Exception ex)
//        {
//            // If unable to calculate, treat as "no intersection".
//            fail = true;
//        }
//        if (fail)
//        {
//            overlap = LineOverlap.Undefined;
//            return Point2D.MaxValue;    // i.e. undefined.
//        }
//        Point2D pr = new Point2D(rx, ry);

//        // It is more reliable to check the axis with longer projection,
//        // especially when other projection is near zero. (And necessary, when is zero.)
//        bool checkY1 = FirstIsShorter(x2 - x1, y2 - y1);
//        bool checkY2 = FirstIsShorter(x4 - x3, y4 - y3);
//        overlap = WhichCrossingType(x1, y1, x2, y2, x3, y3, x4, y4, tolerance_scaled, pr, checkY1, checkY2, ref overlap1, ref overlap2);

//        if (overlap == LineOverlap.CrossingOutside)
//        {
//            // Even though the exact intersection point is outside of both segments,
//            // Check whether any endpoint is within tolerance_scaled of the other line segment.
//            double toleranceSq = tolerance_scaled * tolerance_scaled;
//            Point2D p1 = new Point2D(x1, y1); Point2D p2 = new Point2D(x2, y2);
//            Point2D p3 = new Point2D(x3, y3); Point2D p4 = new Point2D(x4, y4);

//            Point2D prForMinTouch;
//            double minTouchDistSq = PointDistanceSquaredToLine2D(p1, p3, p4, ref prForMinTouch);
//            int caseForMinTouch = 0;
//            Point2D pClosest;
//            if (AccumMin(PointDistanceSquaredToLine2D(p2, p3, p4, ref pClosest), ref minTouchDistSq))
//            {
//                caseForMinTouch = 1; prForMinTouch = pClosest;
//            }
//            if (AccumMin(PointDistanceSquaredToLine2D(p3, p1, p2, ref pClosest), ref minTouchDistSq))
//            {
//                caseForMinTouch = 2; prForMinTouch = pClosest;
//            }
//            if (AccumMin(PointDistanceSquaredToLine2D(p4, p1, p2, ref pClosest), ref minTouchDistSq))
//            {
//                caseForMinTouch = 3; prForMinTouch = pClosest;
//            }
//            if (minTouchDistSq < toleranceSq)
//            {
//                // An endpoint is considered to be touching the other line segment.
//                overlap = LineOverlap.CrossingTouch;

//                // Because we are close to an endpoint, use that endpoint as closest point.
//                switch (caseForMinTouch)
//                {
//                    case 0:
//                        {
//                            pr = p1;
//                            break;
//                        }

//                    case 1:
//                        {
//                            pr = p2;
//                            break;
//                        }

//                    case 2:
//                        {
//                            pr = p3;
//                            break;
//                        }

//                    case 3:
//                        {
//                            pr = p4;
//                            break;
//                        }

//                    default:
//                        {
//                            throw new InvalidProgramException("");
//                            break;
//                        }
//                }

//                // Set appropriate overlap codes.
//                // Is prForMinTouch within touching distance of an endpoint on other segment?
//                Point2D pr1;
//                Point2D pr2 = Point2D.MaxValue;
//                if (caseForMinTouch < 2)
//                {
//                    pr1 = caseForMinTouch == 0 ? p1 : p2;
//                    overlap1 = caseForMinTouch == 0 ? LineOverlap.CrossingTouchA : LineOverlap.CrossingTouchB;
//                    overlap2 = LineOverlap.Crossing;
//                    // "other segment" is (p3, p4)
//                    if (DistanceSquared2D(p3, prForMinTouch) < toleranceSq)
//                    {
//                        pr2 = p3;
//                        overlap2 = LineOverlap.CrossingTouchA;
//                    }
//                    else if (DistanceSquared2D(p4, prForMinTouch) < toleranceSq)
//                    {
//                        pr2 = p4;
//                        overlap2 = LineOverlap.CrossingTouchB;
//                    }
//                }
//                else
//                {
//                    pr1 = caseForMinTouch == 2 ? p3 : p4;
//                    overlap2 = caseForMinTouch == 2 ? LineOverlap.CrossingTouchA : LineOverlap.CrossingTouchB;
//                    overlap1 = LineOverlap.Crossing;
//                    // "other segment" is (p1, p2)
//                    if (DistanceSquared2D(p1, prForMinTouch) < toleranceSq)
//                    {
//                        pr2 = p1;
//                        overlap1 = LineOverlap.CrossingTouchA;
//                    }
//                    else if (DistanceSquared2D(p2, prForMinTouch) < toleranceSq)
//                    {
//                        pr2 = p2;
//                        overlap1 = LineOverlap.CrossingTouchB;
//                    }
//                }

//                if (pr2.IsValid)
//                    // Touches endpoints on both segments.
//                    pr = Average(pr1, pr2);
//            }
//        }

//        // Restore original values.
//        pr.X += origX1; pr.Y += origY1;
//        // NotNecessary-only-used-by-LinesOverlapOrTouch If outP2.X <> Double.MaxValue Then outP2.X += origX1 : outP2.Y += origY1
//        return pr;
//    }

//    // Returns True if value is smaller than min. In that case, sets min to value.
//    // Before first call, caller must initialize min to Double.MaxValue.
//    // Usage: Accumulates minimum, plus gives caller a chance to save corresponding variables.
//    public static bool AccumMin(double value, ref double min)
//    {
//        if (value < min)
//        {
//            min = value; return true;
//        }
//        return false;
//    }

//    // Returns True if value is larger than max. In that case, sets max to value.
//    // Before first call, caller must initialize max to Double.MinValue.
//    // Usage: Accumulates maximum, plus gives caller a chance to save corresponding variables.
//    public static bool AccumMax(double value, ref double max)
//    {
//        if (value > max)
//        {
//            max = value; return true;
//        }
//        return false;
//    }
//    public static bool AccumMax(float value, ref float max)
//    {
//        if (value > max)
//        {
//            max = value; return true;
//        }
//        return false;
//    }
//    public static bool AccumMax(int value, ref int max)
//    {
//        if (value > max)
//        {
//            max = value; return true;
//        }
//        return false;
//    }

//    public static bool FirstIsShorter(double dx, double dy)
//    {
//        return Math.Abs(dx) < Math.Abs(dy);
//    }

//    // Between segments (1)..(2) and (3)..(4).
//    // Return overlap, set overlap1 & 2.
//    // overlap1 is relationship between r and segment (1)..(2).
//    // overlap2 is relationship between r and segment (3)..(4). 
//    private static LineOverlap WhichCrossingType(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double tolerance, Point2D r, bool checkY1, bool checkY2, out LineOverlap overlap1, out LineOverlap overlap2)
//    {

//        // Check within tolerance of each endpoint.
//        double toleranceSq = tolerance * tolerance;
//        bool withinTol1A = (DistanceSquared2D(r.X, r.Y, x1, y1) <= toleranceSq);
//        bool withinTol1B = (DistanceSquared2D(r.X, r.Y, x2, y2) <= toleranceSq);
//        bool withinTol2A = (DistanceSquared2D(r.X, r.Y, x3, y3) <= toleranceSq);
//        bool withinTol2B = (DistanceSquared2D(r.X, r.Y, x4, y4) <= toleranceSq);

//        overlap1 = checkY1 ? WhichCrossingType(r.Y, y1, y2, withinTol1A, withinTol1B) : WhichCrossingType(r.X, x1, x2, withinTol1A, withinTol1B);
//        overlap2 = checkY2 ? WhichCrossingType(r.Y, y3, y4, withinTol2A, withinTol2B) : WhichCrossingType(r.X, x3, x4, withinTol2A, withinTol2B);

//        // If either one is Outside, the result is Outside.
//        // Otherwise, if either one is Touch, the result is Touch.
//        LineOverlap overlap;
//        if ((overlap1 == LineOverlap.CrossingOutside) || (overlap2 == LineOverlap.CrossingOutside))
//            overlap = LineOverlap.CrossingOutside;
//        else if ((overlap1 == LineOverlap.CrossingTouchA) || (overlap1 == LineOverlap.CrossingTouchB) || (overlap2 == LineOverlap.CrossingTouchA) || (overlap2 == LineOverlap.CrossingTouchB))
//            overlap = LineOverlap.CrossingTouch;
//        else
//            overlap = LineOverlap.Crossing;
//        return overlap;
//    }

//    // Describes a single segment; either overlap1 or overlap2.
//    private static LineOverlap WhichCrossingType(double rx, double ax, double bx, bool withinTolA, bool withinTolB)
//    {
//        if (withinTolA)
//            return LineOverlap.CrossingTouchA;
//        else if (withinTolB)
//            return LineOverlap.CrossingTouchB;
//        else if (BetweenInclusive(rx, ax, bx))
//            return LineOverlap.Crossing;
//        else
//            return LineOverlap.CrossingOutside;
//    }

//    // Reference: http://www.geog.ubcca/courses/klink/gis.notes/ncgia/u32.html
//    // y = kx + m
//    public static Point2D LinesIntersectsAt2D_OLD2(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, double tolerance, out LineOverlap overlap, out LineOverlap overlap1, out LineOverlap overlap2, out Point2D outP2)
//    {
//        overlap1 = LineOverlap.Undefined; overlap2 = LineOverlap.Undefined;
//        outP2 = Point2D.MaxValue;    // i.e. undefined.

//        // Both lines parallel to Y-axis. Test ensures at least one of (b1, b2) is good.
//        if (x1 == x2 & x3 == x4)
//            return LinesOverlapOrTouch(x1, y1, x2, y2, x3, y3, x4, y4, tolerance, ref overlap, ref overlap1, ref overlap2, ref outP2);

//        // One of these could still do divide by zero.
//        double k1 = (y2 - y1) / (x2 - x1);
//        double k2 = (y4 - y3) / (x4 - x3);
//        // Parallel lines.
//        if (k1 == k2)
//            return LinesOverlapOrTouch(x1, y1, x2, y2, x3, y3, x4, y4, tolerance, ref overlap, ref overlap1, ref overlap2, ref outP2);

//        double a1 = y1 - (k1 * x1);
//        double a2 = y3 - (k2 * x3);
//        double m = k1 - k2;
//        double n = a2 - a1;

//        Point2D r = new Point2D();
//        bool checkY1, checkY2;
//        if (x1 == x2)
//        {
//            // (p1, p2) is parallel to Y-axis. Avoid invalid k1.
//            r.X = x1;
//            r.Y = a2 + (k2 * r.X);
//            checkY1 = true; checkY2 = false;
//        }
//        else if (x3 == x4)
//        {
//            // (p3, p4) is parallel to Y-axis. Avoid invalid k2.
//            r.X = x3;
//            r.Y = a1 + (k1 * r.X);
//            checkY1 = false; checkY2 = true;
//        }
//        else
//        {
//            r.X = n / m;
//            r.Y = a1 + (k1 * r.X);
//            checkY1 = false; checkY2 = false;
//        }

//        overlap = WhichCrossingType(x1, y1, x2, y2, x3, y3, x4, y4, tolerance, r, checkY1, checkY2, ref overlap1, ref overlap2);
//        return r;
//    }

//    public static Point2D LinesIntersectsAt2D_OLD(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
//    {
//        // Referens: http://www.geog.ubcca/courses/klink/gis.notes/ncgia/u32.html
//        // y = kx + m

//        // Dim foo As Boolean
//        // foo = m2DLibLinesIntersects2D(aX, aY, bX, bY, cX, cY, dX, dY)
//        double a1, a2, b1, b2, m, n;
//        Point2D r = new Point2D();

//        if (ax == bx & cx == dx & ax == cx)
//            return default(Point2D);

//        // If cX = dX Then dX += 0.001

//        b1 = (by - ay) / (bx - ax);
//        b2 = (dy - cy) / (dx - cx);
//        if (b1 == b2)
//            return default(Point2D); // Paralella linjer
//        a1 = ay - (b1 * ax);
//        a2 = cy - (b2 * cx);
//        m = b1 - b2;
//        n = a2 - a1;
//        r.X = n / m;
//        r.Y = a1 + (b1 * r.X);
//        if (ax == bx & cx != dx)
//        {
//            r.X = ax;
//            r.Y = a2 + (b2 * r.X);
//        }
//        if (cx == dx & ax != bx)
//        {
//            r.X = cx;
//            r.Y = a1 + (b1 * r.X);
//        }

//        return r;
//    }


//    // PERFORMANCE: Caller should check polygon BOUNDS first.
//    public static bool LineIntersectsWithLines(Point2D ptdP1, Point2D ptdP2, Point2D[] ptdLines)
//    {
//        for (int i = 0; i <= ptdLines.Length - 2; i++)
//        {
//            int j = (i + 1);

//            if (mDL2DLib.LinesIntersects2D(ptdP1, ptdP2, ptdLines[i], ptdLines[j]))
//                return true;
//        }

//        return false;
//    }

//    /// <summary>
//    ///     ''' TBD: Identical to "LineIntersectsWithLines".
//    ///     ''' </summary>
//    ///     ''' <param name="ptdP1"></param>
//    ///     ''' <param name="ptdP2"></param>
//    ///     ''' <param name="ptdPol"></param>
//    ///     ''' <returns></returns>
//    public static bool LineIntersectsWithPolygon(Point2D ptdP1, Point2D ptdP2, Point2D[] ptdPol)
//    {
//        for (int i = 0; i <= ptdPol.Length - 1; i++)
//        {
//            int j = (i + 1) % ptdPol.Length;

//            if (mDL2DLib.LinesIntersects2D(ptdP1, ptdP2, ptdPol[i], ptdPol[j]))
//                return true;
//        }

//        return false;
//    }

//    // PERFORMANCE: Caller should check polygon BOUNDS first.
//    public static Point2D[] LineIntersectsWithLinesAt(Point2D p1, Point2D p2, Point2D[] linePts)
//    {
//        Point2D[] result = null;

//        for (int i = 0; i <= linePts.Length - 2; i++)
//        {
//            // Wrap.
//            int j = (i + 1);

//            if (mDL2DLib.LinesIntersects2D(p1, p2, linePts[i], linePts[j]))
//            {
//                Point2D isect = mDL2DLib.LinesIntersectsAt2D(p1, p2, linePts[i], linePts[j]);

//                // Check for final point near to first point.
//                if (HasElements(result) && isect.NearlyEquals(result[0], VerySmall))
//                    continue;

//                if (result == null)
//                    result = new Point2D[1];
//                else
//                {
//                    var oldResult = result;
//                    result = new Point2D[result.Length + 1];
//                    if (oldResult != null)
//                        Array.Copy(oldResult, result, Math.Min(result.Length + 1, oldResult.Length));
//                }

//                result[result.Length - 1] = isect;
//            }
//        }

//        return result;
//    }


//    public static Point2D[] LineIntersectsWithPolygonAt(Point2D p1, Point2D p2, IList<Point2D> polyPts)
//    {
//        return LineIntersectsWithPolygonAt(p1, p2, polyPts, ref null);
//    }

//    // Does NOT extend line beyond (p1, p2).
//    // If caller wants segmentIFs, they must pass in a list object. Otherwise, Nothing.
//    // PERFORMANCE: Caller should check polygon BOUNDS first.
//    // "segmentIFs": If not Nothing, contains "indexAndFraction" (aka "IndexFrac") identifying position along polyPts, per returned point.
//    // ASSERT: segmentIFs 1:1 with result points.
//    // REQUIRE: polyPts does NOT have final point that is duplicate of first point.
//    public static Point2D[] LineIntersectsWithPolygonAt(Point2D p1, Point2D p2, IList<Point2D> polyPts, ref IList<float> segmentIFs)
//    {
//        Point2D[] result = null;

//        if (Exists(segmentIFs))
//            segmentIFs.Clear();

//        for (int i = 0; i <= polyPts.Count - 1; i++)
//        {
//            // Wrap.
//            int j = (i + 1) % polyPts.Count;

//            if (mDL2DLib.LinesIntersects2D(p1, p2, polyPts[i], polyPts[j]))
//            {
//                Point2D isect = mDL2DLib.LinesIntersectsAt2D(p1, p2, polyPts[i], polyPts[j]);

//                // Check for final point near to first point.
//                if (HasElements(result) && isect.NearlyEquals(result[0], VerySmall))
//                    continue;

//                if (result == null)
//                    result = new Point2D[1];
//                else
//                {
//                    var oldResult = result;
//                    result = new Point2D[result.Length + 1];
//                    if (oldResult != null)
//                        Array.Copy(oldResult, result, Math.Min(result.Length + 1, oldResult.Length));
//                }

//                result[result.Length - 1] = isect;
//                if (Exists(segmentIFs))
//                {
//                    float segmentFrac = System.Convert.ToSingle(WgtFromResult(polyPts[i], polyPts[j], isect));
//                    segmentIFs.Add(i + segmentFrac);
//                }
//            }
//        }

//        // ASSERT: segmentIFs 1:1 with result points.
//        return result;
//    }


//    // True if both endpoints common. They might be swapped.
//    public static bool LinesIdentical2D(PointF ptfP1, PointF ptfP2, PointF ptfP3, PointF ptfP4)
//    {
//        return LinesIdentical2D(ptfP1.X, ptfP1.Y, ptfP2.X, ptfP2.Y, ptfP3.X, ptfP3.Y, ptfP4.X, ptfP4.Y);
//    }

//    // True if both endpoints common. They might be swapped.
//    public static bool LinesIdentical2D(Point3D ptdP1, Point3D ptdP2, Point3D ptdP3, Point3D ptdP4)
//    {
//        return LinesIdentical2D(ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y, ptdP3.X, ptdP3.Y, ptdP4.X, ptdP4.Y);
//    }

//    // True if both endpoints common. They might be swapped.
//    public static bool LinesIdentical2D(Point2D ptdP1, Point2D ptdP2, Point2D ptdP3, Point2D ptdP4)
//    {
//        return LinesIdentical2D(ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y, ptdP3.X, ptdP3.Y, ptdP4.X, ptdP4.Y);
//    }

//    // True if both endpoints common. They might be swapped.
//    public static bool LinesIdentical2D(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
//    {
//        return (ax.NearlyEquals(cx, NearZero) && ay.NearlyEquals(cy) && (bx.NearlyEquals(dx, NearZero) && by.NearlyEquals(dy, NearZero))) || ((ax.NearlyEquals(dx, NearZero) && ay.NearlyEquals(dy, NearZero)) && (bx.NearlyEquals(cx, NearZero) && by.NearlyEquals(cy, NearZero)));
//    }

//    // True if both endpoints common. They might be swapped.
//    public static bool LinesIdentical2D_Old(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
//    {
//        return ((ax == cx && ay == cy) && (bx == dx && by == dy)) || ((ax == dx && ay == dy) && (bx == cx && by == cy));
//    }


//    public static void Test_Line2InsideLine1()
//    {
//        List<bool> results = new List<bool>();
//        Point2D p0 = new Point2D(0, 0);
//        Point2D p1 = new Point2D(1, 1);
//        Point2D p2 = new Point2D(2, 2);
//        Point2D p3 = new Point2D(3, 3);
//        Point2D p4 = new Point2D(4, 4);
//        results.Add(Line2InsideLine1(p1, p4, p2, p3, false));   // True
//        results.Add(Line2InsideLine1(p2, p3, p1, p4, false));   // False
//        results.Add(Line2InsideLine1(p1, p4, p2, p4, false));   // True
//        results.Add(Line2InsideLine1(p1, p4, p1, p4, false));   // False
//        results.Add(Line2InsideLine1(p1, p4, p1, p4, true));   // True
//        results.Add(Line2InsideLine1(p1, p4, p0, p3, false));   // False
//        results.Add(Line2InsideLine1(p1, p4, p3, p2, false));   // True
//        results.Add(Line2InsideLine1(p1, p4, p4, p2, false));   // True
//        results.Add(Line2InsideLine1(p1, p4, p0, p4, false));   // False
//        results.Add(Line2InsideLine1(p1, p4, p4, p0, false));   // False
//    }

//    // True if line (p3,p4) is inside of line (p1,p2).
//    // If the two lines are identical, then the value of "allowIdentical" is returned.
//    // If one line is a point, False will be returned (LinesIntersectsAt2D rejects as BadData).
//    public static bool Line2InsideLine1(Point2D p1, Point2D p2, Point2D p3, Point2D p4, bool allowIdentical)
//    {
//        LineOverlap overlap, overlap1, overlap2;
//        Point2D outP2;
//        Point2D pIsect = LinesIntersectsAt2D(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, NearZero, ref overlap, ref overlap1, ref overlap2, ref outP2);

//        if (overlap == LineOverlap.Identical)
//            return allowIdentical;
//        return ThisSegmentContainsOther_ButNotIdentical(overlap1);
//    }
//    // True if (exactly) one endpoint common between the line segments, and one of the segments has an endpoint on the other segment.
//    // Tolerance of Single.Epsilon allows for math errors in the intersection algorithm.
//    // Tolerance of VerySmall allows for segments that are based on Singles rather than Doubles, or conversions that might cause small errors.
//    // NearZero, or TolerablyNearZero, are useful choices, if Single.Epsilon is too strict, but VerySmall is too lax.
//    // NOTE: If Lines are identical, returns FALSE. If this is undesirable, comment out the first two lines, and identical segments will return True (because endpoint is zero distance from segment).
//    public static bool LinesCommonEndAndOverlap2D(Point2D a, Point2D b, Point2D c, Point2D d, double tolerance)
//    {
//        if (LinesIdentical2D(a, b, c, d))
//            return false;

//        Point2D p1, p2;
//        if (a.NearlyEquals(c, tolerance))
//        {
//            p1 = b; p2 = d;
//        }
//        else if (a.NearlyEquals(d, tolerance))
//        {
//            p1 = b; p2 = c;
//        }
//        else if (b.NearlyEquals(c, tolerance))
//        {
//            p1 = a; p2 = d;
//        }
//        else if (b.NearlyEquals(d, tolerance))
//        {
//            p1 = a; p2 = c;
//        }
//        else
//            return false;

//        double tolSq = tolerance * tolerance;
//        if (PointDistanceSquaredToLine2D(p1, a, b) <= tolSq)
//            return true;
//        else if (PointDistanceSquaredToLine2D(p1, c, d) <= tolSq)
//            return true;

//        return false;
//    }

//    // Describe the ways line segments can intersect (when non-parallel), or overlap (when parallel).
//    public enum LineOverlap
//    {
//        Undefined   // Not assigned a value.
//,
//        BadData     // a=b or c=d
//,
//        NotParallel // Line segments are not parallel (not determined whether cross or not).
//,

//        // These are the ways in which segments might not overlap.
//        NoOverlap   // No overlap between the lines. Exact reason not stated.
//,
//        BoundsNoOverlap // Bounding rectangles do not overlap (including tolerance). (Only returned by Parallel-testing logic, because crossing logic wants to know about CrossingOutside.)
//,
//        CrossingOutside // Non-parallel; Crossing point exists IF EXTEND THE SEGMENTS.
//,
//        NotColinear // Parallel; No overlap, because not co-linear.
//,
//        ColinearNoOverlap   // Parallel; co-linear; there is a gap between the segments.
//,

//        // --- NOTE IMPORTANT: Above are all "Don't Intersect"; Below are all "Do Intersect". ---
//        DoIntersect // Intersect or overlap; details not stated. <-- NOTE: DoesIntersect() expects this first.
//,

//        // Following are non-parallel intersections.
//        Crossing    // Segments cross, in middle of both segments.
//,
//        CrossingTouch   // Segments cross, touching end of one or both segments (within tolerance).
//,
//        // They are "crossing" in the sense that they are "not parallel" - if both segments were extended into lines, they "cross".
//        // (Touch => they don't "cross" in the vernacular sense; they "meet", but neither makes it across to the other side of the other.)

//        // Non-parallel; describing ONE of the two segments.
//        CrossingTouchA  // Crossing; "a" is touched (by an endpoint of "b"?). [See "CrossingTouch" - they barely "meet".]
//,
//        CrossingTouchB  // Crossing; "b" is touched. [See "CrossingTouch" - they barely "meet".]
//,

//        // --- Following are "co-linear" intersections. ---
//        Identical   // The two line segments are identical. <-- NOTE: ColinearIntersect() expects this option listed first in this sub-list.
//,
//        TouchEnd    // One end is shared, that is the only overlap.
//,
//        InsideTouchEnd   // One segment is inside the other, with one end shared
//,
//        Inside      // One segment is inside the other, away from the ends.
//,
//        Overlap     // Each segment has one endpoint inside the other.
//,

//        // co-linear; describing ONE of the two segments.
//        TouchEndA   // "a" (first endpoint) is shared, that is the only overlap.
//,
//        TouchEndB   // "b" (second endpoint) is shared, that is the only overlap.
//,
//        InsideTouchA  // This segment is inside the other one, touching at "a" (first endpoint).   
//,
//        InsideTouchB  // This segment is inside the other one, touching at "b" (second endpoint).
//,
//        Contains    // This segment contains the other one; neither end touches.
//,
//        ContainsTouchA    // Contains and TouchEndA.
//,
//        ContainsTouchB    // Contains and TouchEndB.
//,
//        OverlapA    // Co-linear; The other segment overlaps "a".
//,
//        OverlapB    // Co-linear; The other segment overlaps "b".
//    }

//    public static string Nick(LineOverlap overlap, LineOverlap overlap1, LineOverlap overlap2)
//    {
//        // In these cases, overlap1 & 2 do not contain additional useful information.
//        if ((!DoesIntersect(overlap)) || (overlap == LineOverlap.DoIntersect) || (overlap == LineOverlap.Crossing) || (overlap == LineOverlap.Identical))
//            return Nick(overlap);

//        return Nick0(overlap) + "/" + Nick1(overlap1) + "/" + Nick1(overlap2);
//    }

//    public static string Nick(LineOverlap overlap)
//    {
//        switch (overlap)
//        {
//            case LineOverlap.CrossingTouch:
//                {
//                    return "CrossTch";
//                }

//            case LineOverlap.CrossingOutside:
//                {
//                    return "CrossOut";
//                }

//            case LineOverlap.ColinearNoOverlap:
//                {
//                    return "ColinNo";
//                }

//            case LineOverlap.InsideTouchEnd:
//                {
//                    return "InTouch";
//                }

//            case LineOverlap.Identical:
//                {
//                    return "Identicl";
//                }

//            default:
//                {
//                    return overlap.ToString();
//                }
//        }
//    }

//    public static string Nick0(LineOverlap overlap)
//    {
//        switch (overlap)
//        {
//            case LineOverlap.Crossing:
//                {
//                    return "X";
//                }

//            case LineOverlap.CrossingTouch:
//                {
//                    return "XT";
//                }

//            case LineOverlap.TouchEnd:
//                {
//                    return "Tch";
//                }

//            case LineOverlap.InsideTouchEnd:
//                {
//                    return "InT";
//                }

//            case LineOverlap.Inside:
//                {
//                    return "In";
//                }

//            case LineOverlap.Overlap:
//                {
//                    return "Ov";
//                }

//            default:
//                {
//                    return overlap.ToString();
//                }
//        }
//    }

//    // Many of the 1&2 cases assume primary type is already specified by 0 case.
//    // E.g. CrossingTouchA => TA rather than XTA, as part of "XT/TA/.."
//    public static string Nick1(LineOverlap overlap12)
//    {
//        switch (overlap12)
//        {
//            case LineOverlap.CrossingOutside:
//                {
//                    return "Out";
//                }

//            case LineOverlap.Crossing:
//                {
//                    return "X";
//                }

//            case LineOverlap.CrossingTouchA:
//                {
//                    return "TA";
//                }

//            case LineOverlap.CrossingTouchB:
//                {
//                    return "TB";
//                }

//            case LineOverlap.TouchEndA:
//                {
//                    return "TA";
//                }

//            case LineOverlap.TouchEndB:
//                {
//                    return "TB";
//                }

//            case LineOverlap.InsideTouchA:
//                {
//                    return "TA";
//                }

//            case LineOverlap.InsideTouchB:
//                {
//                    return "TB";
//                }

//            case LineOverlap.Inside:
//                {
//                    return "In";
//                }

//            case LineOverlap.Contains:
//                {
//                    return "Cnt";
//                }

//            case LineOverlap.ContainsTouchA:
//                {
//                    return "CTA";
//                }

//            case LineOverlap.ContainsTouchB:
//                {
//                    return "CTB";
//                }

//            case LineOverlap.OverlapA:
//                {
//                    return "OvA";
//                }

//            case LineOverlap.OverlapB:
//                {
//                    return "OvB";
//                }

//            default:
//                {
//                    return overlap12.ToString();
//                }
//        }
//    }

//    public static bool DoesIntersect(LineOverlap overlap)
//    {
//        return overlap >= LineOverlap.DoIntersect;
//    }

//    // Intersects and co-linear.
//    public static bool ColinearIntersect(LineOverlap overlap)
//    {
//        return overlap >= LineOverlap.Identical;
//    }

//    // Bad Data, or other problem.
//    public static bool NotGoodAnswer(LineOverlap overlap)
//    {
//        return overlap <= LineOverlap.NotParallel;
//    }

//    // Touches at point A.
//    public static bool TouchA(LineOverlap overlap)
//    {
//        return (overlap == LineOverlap.CrossingTouchA) || (overlap == LineOverlap.TouchEndA) || (overlap == LineOverlap.InsideTouchA) || (overlap == LineOverlap.ContainsTouchA);
//    }

//    // Touches at point B.
//    public static bool TouchB(LineOverlap overlap)
//    {
//        return (overlap == LineOverlap.CrossingTouchB) || (overlap == LineOverlap.TouchEndB) || (overlap == LineOverlap.InsideTouchB) || (overlap == LineOverlap.ContainsTouchB);
//    }

//    // Pass in "overlap1" to find out if line 1 contains line 2.
//    // Pass in "overlap2" to find out if line 2 contains line 1.
//    // Do NOT pass in the main "overlap" -- that would be either "Inside" or "InsideTouchEnd",
//    // and won't tell you WHICH is inside the other.
//    // If the two lines are identical, then False is returned -- caller must test "overlap" if needs that case.
//    public static bool ThisSegmentContainsOther_ButNotIdentical(LineOverlap overlap1)
//    {
//        return (overlap1 == LineOverlap.Contains) || (overlap1 == LineOverlap.ContainsTouchA) || (overlap1 == LineOverlap.ContainsTouchB);
//    }

//    // Returns op1 (first point of overlap).
//    public static Point2D LinesOverlapOrTouch(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy, double tolerance, out LineOverlap overlap, out LineOverlap overlap1, out LineOverlap overlap2, out Point2D op2)
//    {
//        Point2D op1;
//        bool doesOverlap = LinesOverlapOrTouch(new Point2D(ax, ay), new Point2D(bx, by), new Point2D(cx, cy), new Point2D(dx, dy), tolerance, ref overlap, ref overlap1, ref overlap2, ref op1, ref op2);
//        return op1;
//    }

//    // line-segment(a, b) compared to line-segment(c, d).
//    // NOTE: This is for PARALLEL line segments only.
//    // Return True for Identical, TouchEnd, InsideAndTouchEnd, Inside, Overlap.
//    // Return False for BadData, NotParallel, NotColinear, NoOverlap.
//    // NOTE IMPORTANT: If returns False, but overlap=NotParallel, then these might be crossing/touching non-parallel lines.
//    // Results in overlap, op1, and op2.
//    // If overlap=LineOverlap.Identical, then op1=a, op2=b. Note that c&d might be reversed compared to a&b.
//    // If ...TouchEnd, then op1=(the common endpoint), op2=Point2D.Nan.
//    // If ...InsideAndTouchEnd, Inside, or Overlap, then op1 & op2 describe the shared portion (intersection).
//    public static bool LinesOverlapOrTouch(Point2D a, Point2D b, Point2D c, Point2D d, double tolerance, out LineOverlap overlap, out LineOverlap overlap1, out LineOverlap overlap2, out Point2D op1, out Point2D op2)
//    {
//        overlap1 = LineOverlap.Undefined; overlap2 = LineOverlap.Undefined;
//        op1 = Point2D.NaN(); op2 = Point2D.NaN();

//        Point2D delta1 = b - a;
//        Point2D delta2 = d - c;

//        if (delta1 == Point2D.Zero() || delta2 == Point2D.Zero())
//        {
//            overlap = LineOverlap.BadData; return false;
//        }

//        // Parallel? Compare slopes.
//        double slope1, slope2;
//        double brx, crx, drx; // Projected to x-axis (or y-axis, when More Vertical).
//        const double arx = 0;       // a.x becomes origin, so a-relative-x is zero.
//        double cry;           // "c-a", Other axis.  For determining colinear.
//        const double ary = 0;     // a becomes origin.

//        if (Math.Abs(delta1.X) > Math.Abs(delta1.Y))
//        {
//            // More Horizontal than Vertical.
//            if (delta2.X == 0)
//            {
//                overlap = LineOverlap.NotParallel; return false;
//            } // Caller didn't call via LinesIntersectsAt2D, to intercept non-parallel cases.
//            slope1 = delta1.Y / delta1.X;
//            slope2 = delta2.Y / delta2.X;
//            // Project to x-axis, with a.x as origin.
//            brx = b.X - a.X; crx = c.X - a.X; drx = d.X - a.X;
//            cry = c.Y - a.Y; // Other axis
//        }
//        else
//        {
//            // More Vertical than Horizontal.
//            if (delta2.Y == 0)
//            {
//                overlap = LineOverlap.NotParallel; return false;
//            } // Caller didn't call via LinesIntersectsAt2D, to intercept non-parallel cases.
//            slope1 = delta1.X / delta1.Y;
//            slope2 = delta2.X / delta2.Y;
//            // Project to y-axis, with a.y as origin.
//            brx = b.Y - a.Y; crx = c.Y - a.Y; drx = d.Y - a.Y;
//            cry = c.X - a.X; // Other axis
//        }

//        double predictedCry = ary + slope1 * crx;
//        if (Math.Abs(cry - predictedCry) > tolerance)
//        {
//            if (Math.Abs(cry - predictedCry) < 0.001)
//            {
//                Rectangle2D bounds1 = Rectangle2D.FromOppositeCorners(a, b);
//                Rectangle2D bounds2 = Rectangle2D.FromOppositeCorners(c, d);
//                if (!RectanglesIntersectOrTouch(bounds1, bounds2, tolerance))
//                {
//                    overlap = LineOverlap.BoundsNoOverlap; return false; // Parallel, ambiguous co-linearity.
//                }
//            }

//            double absDeltaSlope = Math.Abs(slope2 - slope1);
//            if (absDeltaSlope > Math.Max(4 * tolerance, VerySmall))
//            {
//                overlap = LineOverlap.NotParallel; return false;
//            } // Caller didn't call via LinesIntersectsAt2D, to intercept non-parallel cases?

//            overlap = LineOverlap.NotColinear; return false; // Parallel and not co-linear.
//        }

//        double sign = 1;
//        if (brx < 0)
//        {
//            // Switch directions, to avoid having two sets of logic.
//            sign = -1;
//            brx = -brx; crx = -crx; drx = -drx;
//        }

//        // Now, brx is greater than 0 (than arx).
//        bool swapCD = false;
//        if (drx < crx)
//        {
//            // Swap c and d, to avoid having two sets of logic.
//            swapCD = true;
//            Swap(crx, drx);
//        }

//        // Now, drx is greater than crx.
//        if ((drx < (arx - tolerance)) || (crx > (brx + tolerance)))
//        {
//            overlap = LineOverlap.ColinearNoOverlap; return false;
//        }

//        // Note that b>a and d>c, so the possible cases are limited.
//        if (crx.NearlyEquals(arx, tolerance))
//        {
//            // Because we've transformed so b>a and d>c, we don't need to do "OrElse (d=a and c=b)".
//            if (drx.NearlyEquals(brx, tolerance))
//            {
//                op1 = a; op2 = b; overlap = LineOverlap.Identical; return true;
//            }
//            else if (drx < brx)
//            {
//                // cd inside ab, touching at a=c (if swapCD, touching a=d, to c).
//                op1 = c; op2 = d; overlap = LineOverlap.InsideTouchEnd;
//                overlap1 = LineOverlap.ContainsTouchA;
//                overlap2 = swapCD ? LineOverlap.InsideTouchB : LineOverlap.InsideTouchA; return true;
//            }
//            else
//            {
//                // ab inside cd + touching a=c (if swapCD, touching a=d)
//                op1 = a; op2 = b; overlap = LineOverlap.InsideTouchEnd;
//                overlap1 = LineOverlap.InsideTouchA;
//                overlap2 = swapCD ? LineOverlap.ContainsTouchB : LineOverlap.ContainsTouchA; return true;
//            }
//        }
//        else if (crx.NearlyEquals(brx, tolerance))
//        {
//            // only touch, at b=c (if swapCD, =d).
//            op1 = b; overlap = LineOverlap.TouchEnd;
//            overlap1 = LineOverlap.TouchEndB;
//            overlap2 = swapCD ? LineOverlap.TouchEndB : LineOverlap.TouchEndA; return true;
//        }
//        else if (crx < arx)
//        {
//            if (drx.NearlyEquals(arx, tolerance))
//            {
//                // only touch, at a=d (if swapCD, =c).
//                op1 = a; overlap = LineOverlap.TouchEnd;
//                overlap1 = LineOverlap.TouchEndA;
//                overlap2 = swapCD ? LineOverlap.TouchEndA : LineOverlap.TouchEndB; return true;
//            }
//            else if (drx.NearlyEquals(brx, tolerance))
//            {
//                // ab inside cd, touching at b=d (if swapCD, =c).
//                op1 = a; op2 = b; overlap = LineOverlap.InsideTouchEnd;
//                overlap1 = LineOverlap.InsideTouchB;
//                overlap2 = swapCD ? LineOverlap.ContainsTouchA : LineOverlap.ContainsTouchB; return true;
//            }
//            else if (drx < brx)
//            {
//                // Overlap from a to d (if swapCD, c).
//                op1 = a; op2 = swapCD ? c : d; overlap = LineOverlap.Overlap;
//                overlap1 = LineOverlap.OverlapA;
//                overlap2 = swapCD ? LineOverlap.OverlapA : LineOverlap.OverlapB; return true;
//            }
//            else
//            {
//                // ab inside cd.
//                op1 = a; op2 = b; overlap = LineOverlap.Inside;
//                overlap1 = LineOverlap.Inside;
//                overlap2 = LineOverlap.Contains;
//                LineIntersectionB();
//                return true;
//            }
//        }
//        else if (drx.NearlyEquals(brx, tolerance))
//        {
//            // cd inside ab, touching at b=d (if swapCD, from d to b=c).
//            op1 = c; op2 = d; overlap = LineOverlap.InsideTouchEnd;
//            overlap1 = LineOverlap.ContainsTouchB;
//            overlap2 = swapCD ? LineOverlap.InsideTouchA : LineOverlap.InsideTouchB; return true;
//        }
//        else if (drx < brx)
//        {
//            // cd inside ab.
//            op1 = c; op2 = d; overlap = LineOverlap.Inside;
//            overlap1 = LineOverlap.Contains;
//            overlap2 = LineOverlap.Inside; return true;
//        }
//        else
//        {
//            // Overlap from c (if swapCD, d) to b.
//            op1 = swapCD ? d : c; op2 = b; overlap = LineOverlap.Overlap;
//            overlap1 = LineOverlap.OverlapB;
//            overlap2 = swapCD ? LineOverlap.OverlapB : LineOverlap.OverlapA; return true;
//        }

//        // Never gets here.
//        throw new InvalidProgramException("LinesOverlapOrTouch - logic bug");
//    }

//    // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Given_the_equations_of_the_lines
//    // two non-vertical lines: y=ax+c and y=bx+d.
//    // x1 = (d - c) / (a - b); y1 = a*x1 + c
//    private static void LineIntersectionB()
//    {
//    }



//    public static void RotateByDegrees2D(ref PointF point, double degrees)
//    {
//        if (degrees == 0)
//            return;
//        RotateByRadians2D(point, DegreesToRadians(degrees));
//    }

//    // Set "point" to itself rotated around "origin" by "degrees.
//    public static void RotateByDegrees2D_Origin(ref Point2D point, Point2D origin, double degrees)
//    {
//        point = RotateByDegrees2D_Origin_New(point, origin, degrees);
//    }

//    // Return "point" rotated around "origin" by "degrees".
//    public static Point2D RotateByDegrees2D_Origin_New(Point2D point, Point2D origin, double degrees)
//    {
//        return RotateByDegrees2D_New(point - origin, degrees) + origin;
//    }

//    // Return "point" rotated around "(0, 0)" by "degrees".
//    public static Point2D RotateByDegrees2D_New(Point2D point, double degrees)
//    {
//        if (degrees == 0)
//            return point;

//        return RotateByRadians2D_New(point, DegreesToRadians(degrees));
//    }

//    // Set "point" to its rotated value.
//    public static void RotateByDegrees2D(ref Point2D point, double degrees)
//    {
//        if (degrees == 0)
//            return;
//        RotateByRadians2D(ref point, DegreesToRadians(degrees));
//    }

//    public static void RotateByDegrees2D(ref Point3D point, double degrees)
//    {
//        if (degrees == 0)
//            return;
//        RotateByRadians2D(point, DegreesToRadians(degrees));
//    }

//    public static void RotateByDegrees2D(ref float x, ref float y, double degrees)
//    {
//        if (degrees == 0)
//            return;
//        RotateByRadians2D(ref x, ref y, DegreesToRadians(degrees));
//    }

//    public static void RotateByDegrees2D(ref double x, ref double y, double degrees)
//    {
//        if (degrees == 0)
//            return;
//        RotateByRadians2D(ref x, ref y, DegreesToRadians(degrees));
//    }


//    public static void RotateByRadians2D(ref PointF point, double radians)
//    {
//        if (radians == 0)
//            return;
//        double tx;
//        double ty = RotateByRadians2D_A(point.X, point.Y, radians, ref tx);
//        point.X = System.Convert.ToSingle(tx); point.Y = System.Convert.ToSingle(ty);
//    }

//    // Return the rotated point.
//    public static Point2D RotateByRadians2D_New(Point2D point, double radians)
//    {
//        if (radians == 0)
//            return point;
//        double tx;
//        double ty = RotateByRadians2D_A(point.X, point.Y, radians, ref tx);
//        return new Point2D(tx, ty);
//    }

//    // Set "point" to its rotated value.
//    public static void RotateByRadians2D(ref Point2D point, double radians)
//    {
//        if (radians == 0)
//            return;
//        double tx;
//        double ty = RotateByRadians2D_A(point.X, point.Y, radians, ref tx);
//        point.X = tx; point.Y = ty;
//    }

//    public static void RotateByRadians2D(ref Point3D point, double radians)
//    {
//        if (radians == 0)
//            return;
//        double tx;
//        double ty = RotateByRadians2D_A(point.X, point.Y, radians, ref tx);
//        point.X = tx; point.Y = ty;
//    }

//    public static void RotateByRadians2D(ref float x, ref float y, double radians)
//    {
//        if (radians == 0)
//            return;
//        double tx;
//        double ty = RotateByRadians2D_A(x, y, radians, ref tx);
//        x = System.Convert.ToSingle(tx); y = System.Convert.ToSingle(ty);
//    }

//    public static void RotateByRadians2D(ref double x, ref double y, double radians)
//    {
//        if (radians == 0)
//            return;
//        double tx;
//        double ty = RotateByRadians2D_A(x, y, radians, ref tx);
//        x = tx; y = ty;
//    }

//    private static double lastRadians = double.NegativeInfinity;
//    private static double lastCos;
//    private static double lastSin;

//    // RETURN ty; set tx.
//    private static double RotateByRadians2D_A(double x, double y, double radians, ref double tx)
//    {
//        double cosR;
//        double sinR;
//        if (radians == lastRadians)
//        {
//            cosR = lastCos;
//            sinR = lastSin;
//        }
//        else
//        {
//            cosR = Math.Cos(radians);
//            sinR = Math.Sin(radians);
//            lastRadians = radians;
//            lastCos = cosR;
//            lastSin = sinR;
//        }

//        tx = x * cosR - y * sinR;
//        return x * sinR + y * cosR;
//    }

//    // Returns unit vector in direction "angleDegrees", where x-axis is considered angle 0.
//    public static Point2D AngleDegrees_To_DirectionVector(double angleDegrees)
//    {
//        return AngleRadians_To_DirectionVector(DegreesToRadians(angleDegrees));
//    }

//    // Returns unit vector in direction "angleRadians", where x-axis is considered angle 0.
//    // Inverse of GetAngleRadians2D.
//    // That is, GetAngleRadians2D(AngleRadians_To_DirectionVector(angleRadians)) ~= angleRadians.
//    public static Point2D AngleRadians_To_DirectionVector(double angleRadians)
//    {
//        return new Point2D(Math.Cos(angleRadians), Math.Sin(angleRadians));
//    }

//    // "deltaPt" should be a difference between two points; e.g. "ptB - ptA".
//    // x-axis is considered angle 0.
//    public static double DirectionVector_To_AngleDegrees(Point2D deltaPt)
//    {
//        return RadiansToDegrees(DirectionVector_To_AngleRadians(deltaPt));
//    }

//    // "deltaPt" should be a difference between two points; e.g. "ptB - ptA".
//    // x-axis is considered angle 0.
//    public static double DirectionVector_To_AngleRadians(Point2D deltaPt)
//    {
//        return GetAngleRadians2D(deltaPt.X, deltaPt.Y);
//    }

//    /// <summary>
//    ///     ''' Valid for Orthogonal projections (E.g. UTM), but not for WGS-84 (or other spherical projections).
//    ///     ''' </summary>
//    ///     ''' <param name="originAndAim"></param>
//    ///     ''' <returns></returns>
//    public static Point2D DirectionVectorFromOriginAndAim(Pair<Point2D> originAndAim)
//    {
//        Point2D origin = originAndAim.First;
//        Point2D aim = originAndAim.Second;
//        Point2D deltaPt = aim - origin;
//        return deltaPt;
//    }

//    public static string Get_AngleDegree_Heading_String(double dblAngle)
//    {
//        string strRet = "N/A";

//        if ((dblAngle >= 0 && dblAngle < 25) || dblAngle == 360)
//            strRet = "N";
//        else if (dblAngle >= 35 && dblAngle <= 55)
//            strRet = "NE";
//        else if (dblAngle > 55 && dblAngle < 125)
//            strRet = "E";
//        else if (dblAngle >= 125 && dblAngle <= 145)
//            strRet = "SE";
//        else if (dblAngle > 145 && dblAngle < 215)
//            strRet = "S";
//        else if (dblAngle >= 215 && dblAngle <= 235)
//            strRet = "SW";
//        else if (dblAngle > 235 && dblAngle < 305)
//            strRet = "W";
//        else if (dblAngle >= 305 && dblAngle <= 325)
//            strRet = "NW";
//        else if (dblAngle > 325 && dblAngle < 360)
//            strRet = "N";

//        return strRet;
//    }


//    // Angle change or "bend" at the middle point of a sequence of three points p0-p1-p2 (forming two line segments).
//    // Zero when the points are all on a straight line. +-90 when they form a right angle.
//    // This is "180 - central-angle". (The central angle p0-p1-p2 is 180 degrees when points are on a straight line.)
//    public static double CalcBendDegrees(Point2D p0, Point2D p1, Point2D p2)
//    {
//        double heading01 = GetAngleDegrees2D(p0, p1);
//        double heading12 = GetAngleDegrees2D(p1, p2);
//        // From -180 to +180.
//        return AngleDegreesAsShorterDirection(heading12 - heading01);
//    }

//    // This calculation works if angle change <= 90 degrees. Beyond that, is +-"180 - correct_answer".
//    // Due to ambiguity in arc-sine?
//    // Private Function CalcBendDegrees(p0 As Point2D, p1 As Point2D, p2 As Point2D) As Double
//    // Dim crossProduct As Double = p1.Cross(p0, p2)
//    // Dim magnitudeProduct As Double = Math.Sqrt(DistanceSquared2D(p0, p1) * DistanceSquared2D(p1, p2))

//    // Dim sinA As Double = crossProduct / magnitudeProduct
//    // Return RadiansToDegrees(Math.Asin(sinA))
//    // End Function

//    public static double GetAngleDegrees2D(Point3D ptdOrigo, Point3D ptdHeading)
//    {
//        return GetAngleDegrees2D(ptdOrigo.X, ptdOrigo.Y, ptdHeading.X, ptdHeading.Y);
//    }

//    public static double GetAngleDegrees2D(Point2D ptdOrigo, Point2D ptdHeading)
//    {
//        return GetAngleDegrees2D(ptdOrigo.X, ptdOrigo.Y, ptdHeading.X, ptdHeading.Y);
//    }

//    public static double GetAngleDegrees2D(PointF ptfOrigo, PointF ptfHeading)
//    {
//        return GetAngleDegrees2D(ptfOrigo.X, ptfOrigo.Y, ptfHeading.X, ptfHeading.Y);
//    }

//    // A better name might be "vector direction": it is based on a single vector (it is not an angle between two vectors).
//    // Calculate Rotation given AimPoint and Origin By Simple ArcTangent^2 . Right Mid Square Zero Deg
//    // I believe this is rotation relative to X-axis.
//    public static double GetAngleDegrees2D(double originX, double originY, double aimPointX, double aimPointY)
//    {
//        if ((originX == m_cacheOrigoX) && (originY == m_cacheOrigoY) && (aimPointX == m_cachePointX) && (aimPointY == m_cachePointY))
//        {
//            t_nAngleFast += 1;
//            return m_cacheAngle;
//        }

//        double angle = Math.Atan2(aimPointY - originY, aimPointX - originX) * (180 / Math.PI);
//        if (angle < 0)
//            angle += 360;

//        m_cacheAngle = angle;
//        m_cacheOrigoX = originX; m_cacheOrigoY = originY; m_cachePointX = aimPointX; m_cachePointY = aimPointY;

//        t_nAngleSlow += 1;
//        return angle;
//    }

//    private static double m_cacheOrigoX, m_cacheOrigoY, m_cachePointX, m_cachePointY, m_cacheAngle;
//    private static int t_nAngleFast, t_nAngleSlow;

//    // E.g. returns "45" If the angles differ by 45 degrees, regardless of which is greater,
//    // and taking into account wrapping at +-180 or +-360.
//    // Always takes SHORTER direction around the circle, so answer is in range 0..180.
//    public static double AbsDifferenceTwoAngleDegrees2D(double angleDegrees1, double angleDegrees2)
//    {
//        // This takes any possible angle values, and yields a (wrapped) delta of 0..360.
//        double absDeltaAngleDegrees = Math.Abs(angleDegrees2 - angleDegrees1) % 360;
//        if (absDeltaAngleDegrees > 180)
//            // Take the shorter direction. 0..180.
//            absDeltaAngleDegrees = 360 - absDeltaAngleDegrees;
//        return absDeltaAngleDegrees;
//    }

//    // Input might be from -360 to +360.
//    // Return angle, from -180 to +180 degrees.
//    // For Sweep angle, assuming sweep is less than 180 degrees,
//    // but might be in either direction.
//    public static double AngleDegreesAsShorterDirection(double deltaAngleDegrees)
//    {
//        return DeltaArcAsShorterDirection(deltaAngleDegrees, 360);
//    }

//    // "Arc" generalized to any wrapping coordinate.
//    public static double DeltaArcAsShorterDirection(double deltaArc, double wrapAt)
//    {
//        // Just in case, reduce large range.
//        deltaArc = deltaArc % wrapAt;
//        double halfWrapAt = wrapAt / 2.0;

//        if (deltaArc > halfWrapAt)
//            deltaArc -= wrapAt;
//        else if (deltaArc <= -halfWrapAt)
//            deltaArc += wrapAt;
//        else
//        {
//        }

//        // Verify
//        if ((deltaArc < -halfWrapAt) || (deltaArc > halfWrapAt))
//            Trouble();

//        return deltaArc;
//    }

//    // Convert any angle (in degrees) to value in 0..360.
//    // Negative values become positive values.
//    public static double Wrap360(double angleDegrees)
//    {
//        // Reduce large range.
//        angleDegrees = angleDegrees % 360;
//        if (angleDegrees < 0)
//            angleDegrees += 360;

//        return angleDegrees;
//    }

//    public static void Test_AngleDegreesAsShorterDirection()
//    {
//        if (!AngleDegreesAsShorterDirection(360).NearlyEquals(0) || !AngleDegreesAsShorterDirection(359).NearlyEquals(-1) || !AngleDegreesAsShorterDirection(200).NearlyEquals(-160) || !AngleDegreesAsShorterDirection(560).NearlyEquals(-160) || !AngleDegreesAsShorterDirection(180.01).NearlyEquals(-179.99) || !AngleDegreesAsShorterDirection(180).NearlyEquals(180) || !AngleDegreesAsShorterDirection(179.99).NearlyEquals(179.99) || !AngleDegreesAsShorterDirection(179).NearlyEquals(179) || !AngleDegreesAsShorterDirection(1).NearlyEquals(1) || !AngleDegreesAsShorterDirection(0.01).NearlyEquals(0.01) || !AngleDegreesAsShorterDirection(0).NearlyEquals(0) || !AngleDegreesAsShorterDirection(-360).NearlyEquals(0) || !AngleDegreesAsShorterDirection(-359).NearlyEquals(1) || !AngleDegreesAsShorterDirection(-200).NearlyEquals(160) || !AngleDegreesAsShorterDirection(-560).NearlyEquals(160) || !AngleDegreesAsShorterDirection(-180.01).NearlyEquals(179.99) || !(AngleDegreesAsShorterDirection(-180).NearlyEquals(180) || AngleDegreesAsShorterDirection(-180).NearlyEquals(-180)) || !AngleDegreesAsShorterDirection(-179.99).NearlyEquals(-179.99) || !AngleDegreesAsShorterDirection(-179).NearlyEquals(-179) || !AngleDegreesAsShorterDirection(-1).NearlyEquals(-1) || !AngleDegreesAsShorterDirection(-0.01).NearlyEquals(-0.01))
//            Trouble();
//    }

//    // By "sorted", we mean ordered such that the angle from #1 to #2 is <= 180 degrees --
//    // the shorter distance around the circle.
//    public static void SortTwoAnglesSoShorterDistance(ref double angleDegrees1, ref double angleDegrees2)
//    {
//        double deltaAngleDegrees = (angleDegrees2 - angleDegrees1);
//        if (deltaAngleDegrees < -180)
//            // E.g. (350, 10) -- keep same order.
//            return;
//        else if (deltaAngleDegrees < 0)
//        {
//        }
//        else if (deltaAngleDegrees <= 180)
//            // E.g. (170, 190) -- keep same order.
//            return;
//        else
//        {
//        }

//        // Swap them.
//        Swap(angleDegrees1, angleDegrees2);
//    }

//    // Given Two angles (in degrees), are they pointing "more away from each other" than "towards each other"?
//    public static bool AngleDegrees_Are_Opposite(double angleDegrees1, double angleDegrees2)
//    {
//        // AbsDifferenceTwoAngleDegrees2D is in range 0..180.
//        // Half that is "90".
//        return (AbsDifferenceTwoAngleDegrees2D(angleDegrees1, angleDegrees2) > 90);
//    }


//    public static double GetAngleRadians2D(Point2D ptdOrigo, Point2D ptdHeading)
//    {
//        return GetAngleRadians2D(ptdOrigo.X, ptdOrigo.Y, ptdHeading.X, ptdHeading.Y);
//    }

//    public static double GetAngleRadians2D(PointF ptfOrigo, PointF ptfHeading)
//    {
//        return GetAngleRadians2D(ptfOrigo.X, ptfOrigo.Y, ptfHeading.X, ptfHeading.Y);
//    }

//    // Calculate Rotation given Point and Origo By Simple ArcTangent^2 . Right Mid Square Zero Deg
//    public static double GetAngleRadians2D(double originX, double originY, double pointX, double pointY)
//    {
//        double angleRadians = Math.Atan2(pointY - originY, pointX - originX);
//        return angleRadians;
//    }

//    // "deltaPt" should be a difference between two points; e.g. "ptB - ptA".
//    // aka "DirectionVector To AngleRadians".
//    public static double GetAngleRadians2D(Point2D deltaPt)
//    {
//        return GetAngleRadians2D(deltaPt.X, deltaPt.Y);
//    }
//    public static double GetAngleRadians2D(Size2D twist)
//    {
//        return GetAngleRadians2D(twist.Width, twist.Height);
//    }
//    public static double GetAngleRadians2D(double dx, double dy)
//    {
//        double angleRadians = Math.Atan2(dy, dx);
//        return angleRadians;
//    }



//    // From origin, move in direction of secondPoint, by specified distance.
//    // The result may be either nearer or farther from origin than secondPoint is.
//    // secondPoint is merely used to give a direction.
//    public static Point2D MoveOnHeading(Point2D origin, Point2D secondPoint, double distance)
//    {
//        Point2D delta = secondPoint - origin;
//        return MoveOnHeading_GivenDelta(origin, delta, distance);
//    }

//    // Move away from origin, starting at currentPt, by specified distance.
//    // The result is on the line from origin through currentPt, but farther away by "distance".
//    // NOTE: Negative distance => move towards "origin".
//    public static Point2D MoveFartherOnHeading(Point2D origin, Point2D currentPt, double distance)
//    {
//        // NOTE: Alternative implementation would be similar to code in "Point3D" version.

//        Point2D delta = currentPt - origin;
//        return MoveOnHeading_GivenDelta(currentPt, delta, distance);
//    }

//    // Move away from origin, starting at currentPt, by specified distance.
//    // The result is on the line from origin through currentPt, but farther away by "distance".
//    // NOTE: Negative distance => move towards "origin".
//    public static Point3D MoveFartherOnHeading(Point3D origin, Point3D currentPt, double distance)
//    {
//        double totalDistance = Distance3D(origin, currentPt) + distance;
//        return MoveOnHeading(origin, currentPt, totalDistance);
//    }

//    // Move from startPoint, by specified distance, at RIGHT ANGLE TO line towards origin.
//    public static Point2D MoveRightAngleToHeading(Point2D startPoint, double distance, Point2D origin)
//    {
//        Point2D delta = startPoint - origin;
//        return MoveRightAngleToHeading_GivenDelta(startPoint, delta, distance);
//    }

//    // Use this if you already have the delta between two points.
//    // Moves in direction of delta, by specified distance.
//    public static Point2D MoveOnHeading_GivenDelta(Point2D startPoint, Point2D delta, double distance)
//    {
//        double deltaLength = delta.Length;
//        if (deltaLength == 0)
//            return startPoint; // Cannot determine direction to move, so don't.

//        Point2D result = startPoint + (distance / deltaLength) * delta;
//        // ' test - verify
//        // Dim verifyDistanceSq As Double = DistanceSquared2D(startPoint, result)
//        // If Not verifyDistanceSq.NearlyEquals(distance * distance) Then
//        // Test()
//        // End If

//        return result;
//    }

//    // Use this if you already have the delta between two points.
//    // Moves at RIGHT ANGLE TO delta, by specified distance.
//    public static Point2D MoveRightAngleToHeading_GivenDelta(Point2D startPoint, Point2D delta, double distance)
//    {
//        double deltaLength = delta.Length;
//        if (deltaLength == 0)
//            return startPoint; // Cannot determine direction to move, so don't.

//        Point2D result = startPoint + (distance / deltaLength) * RightAngleToDelta(delta);
//        return result;
//    }

//    // ARBITRARILY pick one of the right angles.
//    public static Point2D RightAngleToDelta(Point2D delta)
//    {
//        return (new Point2D(-delta.Y, delta.X));
//    }


//    public static Point2D MoveOnHeadingDegrees(Point2D origin, double angleDegrees, double distance)
//    {
//        return MoveOnHeadingRadians(origin, DegreesToRadians(angleDegrees), distance);
//    }

//    public static Point2D MoveOnHeadingRadians(Point2D origin, double angleRadians, double distance)
//    {
//        Point2D directionVector = AngleRadians_To_DirectionVector(angleRadians);
//        Point2D result = origin + distance * directionVector;
//        return result;
//    }

//    // Move in direction of secondPoint, specified distance from origin.
//    // (Put in 2D lib, next to 2D version, so callers don't have to specify which module to reference.)
//    public static Point3D MoveOnHeading(Point3D origin, Point3D secondPoint, double distance)
//    {
//        Point3D delta = secondPoint - origin;
//        double deltaLength = delta.Length;
//        if (deltaLength == 0)
//            return origin; // Cannot determine direction to move, so don't.

//        Point3D result = origin + delta * (distance / deltaLength);
//        // ' test - verify
//        // Dim verify As Double = DistanceSquared2D(result, origin)
//        // If Not verify.NearlyEquals(distance * distance) Then
//        // Test()
//        // End If
//        return result;
//    }


//    public static double LinearInterpolationX2D(double dblY, PointF ptfP1, PointF ptfP2)
//    {
//        return LinearInterpolationX2D(dblY, ptfP1.X, ptfP1.Y, ptfP2.X, ptfP2.Y);
//    }

//    public static double LinearInterpolationX2D(double dblY, Point2D ptdP1, Point2D ptdP2)
//    {
//        return LinearInterpolationX2D(dblY, ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y);
//    }

//    public static double LinearInterpolationX2D(double dblY, double dblP1X, double dblP1Y, double dblP2X, double dblP2Y)
//    {
//        return ((dblP1X - dblP2X) * dblY - (dblP1X * dblP2Y) + (dblP2X * dblP1Y)) / (dblP1Y - dblP2Y);
//    }

//    public static double LinearInterpolationY2D(double dblX, PointF ptfP1, PointF ptfP2)
//    {
//        return LinearInterpolationY2D(dblX, ptfP1.X, ptfP1.Y, ptfP2.X, ptfP2.Y);
//    }

//    public static double LinearInterpolationY2D(double dblX, Point2D ptdP1, Point2D ptdP2)
//    {
//        return LinearInterpolationY2D(dblX, ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y);
//    }

//    public static double LinearInterpolationY2D(double dblX, double dblP1X, double dblP1Y, double dblP2X, double dblP2Y)
//    {
//        return dblP1Y + (((dblX - dblP1X) * (dblP2Y - dblP1Y)) / (dblP2X - dblP1X));
//    }

//    public static double PointFurthestDistanceToRectangle2D(PointF ptfPoint, PointF[] ptfRec)
//    {
//        if (ptfRec == null)
//            return -1;

//        double dblFurthestDI = -1;

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            double dblDI = Distance2D(ptfPoint, ptfRec[intIdx]);

//            if (dblDI > dblFurthestDI)
//                dblFurthestDI = dblDI;
//        }

//        return dblFurthestDI;
//    }

//    public static double PointFurthestDistanceToRectangle2D(Point2D ptdPoint, Point2D[] ptdRec)
//    {
//        if (ptdRec == null)
//            return -1;

//        double dblFurthestDI = -1;

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            double dblDI = Distance2D(ptdPoint, ptdRec[intIdx]);

//            if (dblDI > dblFurthestDI)
//                dblFurthestDI = dblDI;
//        }

//        return dblFurthestDI;
//    }

//    public static double PointFurthestDistanceToRectangle2D(Point3D ptdPoint, Point3D[] ptdRec)
//    {
//        if (ptdRec == null)
//            return -1;

//        double dblFurthestDI = -1;

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            double dblDI = Distance2D(ptdPoint, ptdRec[intIdx]);

//            if (dblDI > dblFurthestDI)
//                dblFurthestDI = dblDI;
//        }

//        return dblFurthestDI;
//    }

//    public static void PointFurthestAndClosestToShape(PointF point, PointF[] shape, bool closed, out PointF closestPt, out double closestDI, out PointF furthestPt, out double furthestDI)
//    {
//        if (shape == null)
//            return;

//        closestDI = -1;
//        furthestDI = -1;

//        int length = shape.Length - 1;

//        if (!closed || (shape.Length > 1 && shape[0] == shape[shape.Length - 1]))
//            length = shape.Length - 2;

//        for (int i = 0; i <= length; i++)
//        {
//            int j = (i + 1) % length;

//            double dic = PointDistanceToLine2D(point, shape[i], shape[j]);
//            PointF ptc = LinePointClosestToPoint2D(point, shape[i], shape[j]);

//            double dif;
//            PointF ptf = PointFurthestToLine(point, shape[i], shape[j], out dif);

//            if (furthestDI == -1 || dif > furthestDI)
//            {
//                furthestDI = dif;
//                furthestPt = ptf;
//            }

//            if (closestDI == -1 || dic < closestDI)
//            {
//                closestDI = dic;
//                closestPt = ptc;
//            }
//        }
//    }

//    public static PointF PointFurthestToLine(PointF point, PointF p1, PointF p2, out double di)
//    {
//        double di1 = Distance2D(point, p1);
//        double di2 = Distance2D(point, p2);

//        if (di1 > di2)
//        {
//            di = di1;
//            return p1;
//        }
//        else
//        {
//            di = di2;
//            return p2;
//        }
//    }

//    public static double PointDistanceToRectangle2D(PointF ptfPoint, PointF[] ptfRec)
//    {
//        if (ptfRec == null)
//            return -1;

//        double dblClosestDI = -1;

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            PointF ptfP1;
//            PointF ptfP2 = ptfRec[intIdx];

//            if (intIdx == 0)
//                ptfP1 = ptfRec[3];
//            else
//                ptfP1 = ptfRec[intIdx - 1];

//            double dblDI = PointDistanceToLine2D(ptfPoint, ptfP1, ptfP2);

//            if (dblClosestDI < 0)
//                dblClosestDI = dblDI;
//            else if (dblDI < dblClosestDI)
//                dblClosestDI = dblDI;
//        }

//        return dblClosestDI;
//    }

//    public static double PointDistanceToRectangle2D(Point2D ptdPoint, Point2D[] ptdRec)
//    {
//        if (ptdRec == null)
//            return -1;

//        double dblClosestDI = -1;

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            Point2D ptdP1;
//            Point2D ptdP2 = ptdRec[intIdx];

//            if (intIdx == 0)
//                ptdP1 = ptdRec[3];
//            else
//                ptdP1 = ptdRec[intIdx - 1];

//            double dblDI = PointDistanceToLine2D(ptdPoint, ptdP1, ptdP2);

//            if (dblClosestDI < 0)
//                dblClosestDI = dblDI;
//            else if (dblDI < dblClosestDI)
//                dblClosestDI = dblDI;
//        }

//        return dblClosestDI;
//    }

//    public static double PointDistanceToRectangle2D(Point3D ptdPoint, Point3D[] ptdRec)
//    {
//        if (ptdRec == null)
//            return -1;

//        double dblClosestDI = -1;

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            Point3D ptdP1;
//            Point3D ptdP2 = ptdRec[intIdx];

//            if (intIdx == 0)
//                ptdP1 = ptdRec[3];
//            else
//                ptdP1 = ptdRec[intIdx - 1];

//            double dblDI = PointDistanceToLine2D(ptdPoint, ptdP1, ptdP2);

//            if (dblClosestDI < 0)
//                dblClosestDI = dblDI;
//            else if (dblDI < dblClosestDI)
//                dblClosestDI = dblDI;
//        }

//        return dblClosestDI;
//    }

//    public static double PointDistanceToLine2D(PointF ptfPoint, PointF ptfP1, PointF ptfP2)
//    {
//        PointF ptfDelta;
//        PointF ptfClosest;
//        float t;

//        ptfDelta = Delta2D(ptfP1, ptfP2);

//        if (ptfDelta.X == 0 & ptfDelta.Y == 0)
//            return mDL2DLib.Distance2D(ptfPoint, ptfP1);

//        // Calculate the t that minimizes the Distance.
//        t = ((ptfPoint.X - ptfP1.X) * ptfDelta.X + (ptfPoint.Y - ptfP1.Y) * ptfDelta.Y) / (double)(ptfDelta.X * ptfDelta.X + ptfDelta.Y * ptfDelta.Y);

//        // See if this represents one of the segment's
//        // end points or a point in the middle.
//        if (t < 0)
//        {
//            ptfDelta.X = ptfPoint.X - ptfP1.X;
//            ptfDelta.Y = ptfPoint.Y - ptfP1.Y;
//            ptfClosest.X = ptfP1.X;
//            ptfClosest.Y = ptfP1.Y;
//        }
//        else if (t > 1)
//        {
//            ptfDelta.X = ptfPoint.X - ptfP2.X;
//            ptfDelta.Y = ptfPoint.Y - ptfP2.Y;
//            ptfClosest.X = ptfP2.X;
//            ptfClosest.Y = ptfP2.Y;
//        }
//        else
//        {
//            ptfClosest.X = ptfP1.X + t * ptfDelta.X;
//            ptfClosest.Y = ptfP1.Y + t * ptfDelta.Y;
//            ptfDelta.X = ptfPoint.X - ptfClosest.X;
//            ptfDelta.Y = ptfPoint.Y - ptfClosest.Y;
//        }

//        return mDL2DLib.Distance2D(ptfPoint, ptfClosest);
//    }
//    // Performance: Avoids Sqrt.
//    public static double PointDistanceSquaredToLine2D(PointF point, PointF p1, PointF p2)
//    {
//        PointF delta = Delta2D(p1, p2);
//        if (delta.X == 0 & delta.Y == 0)
//            return mDL2DLib.DistanceSquared2D(point, p1);

//        // Calculate the t that minimizes the Distance.
//        float t = ((point.X - p1.X) * delta.X + (point.Y - p1.Y) * delta.Y) / (double)(delta.X * delta.X + delta.Y * delta.Y);

//        // See if this represents one of the segment's
//        // end points or a point inbetween.
//        PointF closest;
//        if (t <= 0)
//        {
//            closest.X = p1.X;
//            closest.Y = p1.Y;
//        }
//        else if (t >= 1)
//        {
//            closest.X = p2.X;
//            closest.Y = p2.Y;
//        }
//        else
//        {
//            closest.X = p1.X + t * delta.X;
//            closest.Y = p1.Y + t * delta.Y;
//        }

//        return mDL2DLib.DistanceSquared2D(point, closest);
//    }

//    // p1 & p2 define the line. Allow the line to extend beyond the two points; find shortest delta from a point on that extended line.
//    // If p1 = p2, the result is undefined; represent as (Double.NaN, Double.NaN).
//    public static Point2D PointDeltaToLineExtended2D(Point2D point, Point2D p1, Point2D p2)
//    {
//        // Because we are allowing the line to extend, we don't clamp "closest" to be between p1 and p2.
//        // (Contrast with PointDistanceToLine2D.)
//        Point2D closest = ClosestPointOnLineExtended2D(point, p1, p2);
//        if (double.IsNaN(closest.X))
//            return new Point2D(double.NaN, double.NaN); // Undefined.

//        double deltaX = point.X - closest.X;
//        double deltaY = point.Y - closest.Y;
//        return new Point2D(deltaX, deltaY);
//    }

//    // p1 & p2 define the line. Allow the line to extend beyond the two points; find closest point on that extended line.
//    // If p1 = p2, the result is undefined; represent as (Double.NaN, Double.NaN).
//    public static Point2D ClosestPointOnLineExtended2D(Point2D point, Point2D p1, Point2D p2)
//    {
//        Point2D delta = Delta2D(p1, p2);

//        if (delta.X == 0 && delta.Y == 0)
//            return new Point2D(double.NaN, double.NaN); // Undefined.

//        // Calculate the t that minimizes the Distance.
//        double t = CalcTOfClosestPoint(point, p1, delta);

//        // Because we are allowing the line to extend, we don't clamp "closest" to be between p1 and p2.
//        // That is, we don't clip t to be between 0 and 1.
//        return TToPoint(t, p1, delta);
//    }

//    // Allows extension beyond original endpoints;
//    // I.E. may return value outside of 0..1.
//    // CAUTION: The last parameter is "delta"; it is NOT p2!
//    public static double CalcTOfClosestPoint(Point2D point, Point2D p1, Point2D delta)
//    {
//        return ((point.X - p1.X) * delta.X + (point.Y - p1.Y) * delta.Y) / (delta.X * delta.X + delta.Y * delta.Y);
//    }

//    // Allows extension beyond original endpoints;
//    // I.E. may return value outside of 0..1.
//    public static float CalcTOfClosestPoint(PointF point, PointF p1, PointF delta)
//    {
//        double dx = System.Convert.ToDouble(delta.X);
//        // For accuracy, intermediate values are Double.
//        double dy = System.Convert.ToDouble(delta.Y);
//        return System.Convert.ToSingle(((System.Convert.ToDouble(point.X) - p1.X) * dx + (System.Convert.ToDouble(point.Y) - p1.Y) * dy) / (dx * dx + dy * dy));
//    }

//    // t=0 => p1, t=1 => p1+delta.
//    public static Point2D TToPoint(double t, Point2D p1, Point2D delta)
//    {
//        double px = p1.X + t * delta.X;
//        double py = p1.Y + t * delta.Y;
//        return new Point2D(px, py);
//    }


//    // Line (p1, p2) is a segment: does not extend beyond its endpoints.
//    public static double PointDistanceToLine2D(Point2D point, Point2D p1, Point2D p2)
//    {
//        double t;
//        return PointDistanceToLine2D_AndT(point, p1, p2, out t);
//    }

//    // Line (p1, p2) is a segment: does not extend beyond its endpoints.
//    // PERFORMANCE: Avoids Sqrt, by returning distance-squared rather than distance.
//    public static double PointDistanceSqToLine2D(Point2D point, Point2D p1, Point2D p2)
//    {
//        double t;
//        return PointDistanceSqToLine2D_AndT(point, p1, p2, out t);
//    }

//    public static double PointDistanceSqToLine2D_AndClosestPoint(Point2D point, Point2D p1, Point2D p2, out Point2D closest)
//    {
//        double t;
//        closest = ClosestPointOnLine2D_AndT(point, p1, p2, ref t);
//        return mDL2DLib.DistanceSquared2D(point, closest);
//    }

//    // Line (p1, p2) is a segment: does not extend beyond its endpoints.
//    // "t" is "weight" of point on line that is closest to "point";
//    // it is 0 at p1, 1 at p2.
//    public static double PointDistanceToLine2D_AndT(Point2D point, Point2D p1, Point2D p2, out double t)
//    {
//        Point2D closest = ClosestPointOnLine2D_AndT(point, p1, p2, ref t);
//        return mDL2DLib.Distance2D(point, closest);
//    }

//    // Line (p1, p2) is a segment: does not extend beyond its endpoints.
//    // "t" is "weight" of point on line that is closest to "point";
//    // it is 0 at p1, 1 at p2.
//    // PERFORMANCE: Avoids Sqrt, by returning distance-squared rather than distance.
//    public static double PointDistanceSqToLine2D_AndT(Point2D point, Point2D p1, Point2D p2, out double t)
//    {
//        Point2D closest = ClosestPointOnLine2D_AndT(point, p1, p2, ref t);
//        return mDL2DLib.DistanceSquared2D(point, closest);
//    }

//    // Line (p1, p2) is a segment: does not extend beyond its endpoints.
//    // "t" is "weight" of point on line that is closest to "point";
//    // it is 0 at p1, 1 at p2.
//    public static Point2D ClosestPointOnLine2D_AndT(Point2D point, Point2D p1, Point2D p2, out double t)
//    {
//        Point2D delta = Delta2D(p1, p2);

//        if (delta.X == 0 & delta.Y == 0)
//        {
//            // Line is a single point; take distance from "point" to that point.
//            // All "t"s would yield same answer. (Or maybe there would be divide-by-zero.) Treat it as "half-way".
//            t = 0.5;
//            // p1 and p2 are the same; there is only one possible answer to "closest point".
//            return p1;
//        }

//        // ----> Main Work <----
//        // Out "t".
//        // Calculate the t that minimizes the Distance.
//        t = CalcTOfClosestPoint(point, p1, delta);


//        // See if "t" represents one of the segment's end points, or a point in-between.
//        // NOTE: Alternative implementation would be t = Clamp(t, 0, 1), then always use t * delta.
//        // However, that would have a (small) numerical error at t=1.
//        Point2D closest;
//        if (t <= 0)
//        {
//            t = 0;
//            closest = p1;
//        }
//        else if (t >= 1)
//        {
//            t = 1;
//            closest = p2;
//        }
//        else
//        {
//            closest.X = p1.X + t * delta.X;
//            closest.Y = p1.Y + t * delta.Y;
//        }

//        return closest;
//    }

//    // "2D" because "point" has no Z; calculating in XY plane.
//    // Return value then interpolates Z based on p1 and p2.
//    // Line (p1, p2) is a segment: does not extend beyond its endpoints.
//    // "t" is "weight" of point on line that is closest to "point";
//    // it is 0 at p1, 1 at p2.
//    public static Point3D ClosestPointOnLine2D_AndT(Point2D point, Point3D p1, Point3D p2, out double t)
//    {
//        Point2D p1Flat = p1.ToPoint2D();
//        Point2D p2Flat = p2.ToPoint2D();
//        Point2D delta = Delta2D(p1Flat, p2Flat);

//        if (delta.X == 0 & delta.Y == 0)
//        {
//            // Line is a single point; take distance from "point" to that point.
//            // All "t"s would yield same answer. (Or maybe there would be divide-by-zero.) Treat it as "half-way".
//            t = 0.5;
//            // p1 and p2 are the same; there is only one possible answer to "closest point".
//            return p1;
//        }

//        // ----> Main Work <----
//        // Out "t".
//        // Calculate the t that minimizes the Distance.
//        t = CalcTOfClosestPoint(point, p1Flat, delta);


//        // See if "t" represents one of the segment's end points, or a point in-between.
//        // NOTE: Alternative implementation would be t = Clamp(t, 0, 1), then always use t * delta.
//        // However, that would have a (small) numerical error at t=1.
//        Point3D closest;
//        if (t <= 0)
//        {
//            t = 0;
//            closest = p1;
//        }
//        else if (t >= 1)
//        {
//            t = 1;
//            closest = p2;
//        }
//        else
//        {
//            closest.X = p1.X + t * delta.X;
//            closest.Y = p1.Y + t * delta.Y;
//            double deltaZ = p2.Z - p1.Z;
//            closest.Z = p1.Z + t * deltaZ;
//            Test();
//        }

//        return closest;
//    }



//    // Finds closest points on the two segments; able to interpolate along either segment (but not both simultaneously).
//    // TODO: Currently can't simultaneously interpolate both segments; if segments intersect, should find that intersection point.
//    // PERFORMANCE: Interpolates all 4 pairs of "one end and the other full segment"; a lot of that work is redundant.
//    // "p1a..p1b" and "p2a..p2b" are the two segments. Do not extend beyond ends of segments.
//    public static double ClosestPointsOnSegments_Distance(Point2D p1a, Point2D p1b, Point2D p2a, Point2D p2b, out Point2D closestP1, out Point2D closestP2)
//    {
//        double minDistanceSq = double.MaxValue;

//        // Check all 4 segment ends for distance; closest end will be closest overall.
//        AccumClosestPointPairAndDistanceSq(p1a, p2a, p2b, ref minDistanceSq, ref closestP1, ref closestP2);
//        AccumClosestPointPairAndDistanceSq(p1b, p2a, p2b, ref minDistanceSq, ref closestP1, ref closestP2);
//        // CAUTION: "closestP1" corresponds to "p1a", so caller must change parameter order when testing a point from segment 2.
//        AccumClosestPointPairAndDistanceSq(p2a, p1a, p1b, ref minDistanceSq, ref closestP2, ref closestP1);
//        AccumClosestPointPairAndDistanceSq(p2b, p1a, p1b, ref minDistanceSq, ref closestP2, ref closestP1);

//        return Math.Sqrt(minDistanceSq);
//    }

//    // CAUTION: "closestP1" corresponds to "p1a", so caller must change parameter order when testing a point from segment 2.
//    private static void AccumClosestPointPairAndDistanceSq(Point2D p1a, Point2D p2a, Point2D p2b, ref double minDistanceSq, ref Point2D closestP1, ref Point2D closestP2)
//    {
//        Point2D partnerPt;
//        double distanceSq = PointDistanceSqToLine2D_AndClosestPoint(p1a, p2a, p2b, out partnerPt);

//        if (distanceSq < minDistanceSq)
//        {
//            minDistanceSq = distanceSq;
//            closestP1 = p1a;
//            closestP2 = partnerPt;
//        }
//    }


//    // Don't wrap. If want closest polygon, and poly is not already closed, caller must append first point at end of array.
//    public static double PointDistanceToPolyline(Point2D point, Point2D[] poly)
//    {
//        Point2D closestPt = default(Point2D);
//        return PointDistanceToPolyline_AndClosestPoint(point, poly, out closestPt);
//    }

//    // Don't wrap. If want closest polygon, and poly is not already closed, caller must append first point at end of array.
//    public static double PointDistanceToPolyline_AndClosestPoint(Point2D point, Point2D[] poly, out Point2D closestPt)
//    {
//        double minDistanceSq = double.MaxValue;
//        // OUT: closestPt
//        closestPt = default(Point2D);


//        int lastIndex = poly.Length - 1;
//        // Don't wrap. If want closest polygon, and poly is not already closed, caller must append first point at end of array.
//        for (int i = 0; i <= lastIndex - 1; i++)
//        {
//            int j = i + 1;
//            // If j >= lastIndex Then j = 0

//            Point2D closest1 = default(Point2D);
//            double distanceSq = PointDistanceSqToLine2D_AndClosestPoint(point, poly[i], poly[j], out closest1);

//            if (distanceSq < minDistanceSq)
//            {
//                minDistanceSq = distanceSq;
//                // OUT: closestPt
//                closestPt = closest1;
//            }
//        }


//        double minDistance = double.MaxValue;
//        if (Exists(closestPt))
//            minDistance = Math.Sqrt(minDistanceSq);
//        return minDistance;
//    }

//    // Performance: Avoids Sqrt.
//    public static double PointDistanceSquaredToLine2D(Point2D point, Point2D p1, Point2D p2)
//    {
//        Point2D closest = LinePointClosestToPoint2D(point, p1, p2);
//        return DistanceSquared2D(point, closest);
//    }
//    // Performance: Avoids Sqrt.
//    public static double PointDistanceSquaredToLine2D(Point2D point, Point2D p1, Point2D p2, out Point2D closest)
//    {
//        closest = LinePointClosestToPoint2D(point, p1, p2);
//        return DistanceSquared2D(point, closest);
//    }
//    // Performance: Avoids Sqrt.
//    public static double PointDistanceSquaredToLine2D(Point2D point, Point2D p1, Point2D p2, out double t)
//    {
//        Point2D closest = LinePointClosestToPoint2D(point, p1, p2, ref t);
//        return DistanceSquared2D(point, closest);
//    }

//    public static Point2D LinePointClosestToPoint2D(Point2D point, Point2D p1, Point2D p2)
//    {
//        double t;
//        return LinePointClosestToPoint2D(point, p1, p2, ref t);
//    }

//    // Set t to 0 if p1 is closest, 1 if p2 is closest, value between 0..1 to represent closest point in middle.
//    public static Point2D LinePointClosestToPoint2D(Point2D point, Point2D p1, Point2D p2, out double t)
//    {
//        Point2D delta = Delta2D(p1, p2);

//        // Test for zero-length line segment. If so, return that segment's only point.
//        if (delta.X == 0 & delta.Y == 0)
//            return p1;

//        // Calculate the t that minimizes the Distance.
//        t = ((point.X - p1.X) * delta.X + (point.Y - p1.Y) * delta.Y) / (delta.X * delta.X + delta.Y * delta.Y);

//        // See if this represents one of the segment's
//        // end points or a point in the middle.
//        Point2D closest;
//        if (t < 0)
//        {
//            t = 0;
//            closest = p1;
//        }
//        else if (t > 1)
//        {
//            t = 1;
//            closest = p2;
//        }
//        else
//        {
//            closest.X = p1.X + t * delta.X;
//            closest.Y = p1.Y + t * delta.Y;
//        }

//        // ' Verify
//        // Dim deltaPt As Point2D = Delta2D(point, closest)
//        // Dim dist As Single = deltaPt.Length
//        // Dim verify As Point2D = Lerp(p1, p2, t)

//        return closest;
//    }


//    public static PointF LinePointClosestToPoint2D(PointF point, PointF p1, PointF p2)
//    {
//        float t;
//        return LinePointClosestToPoint2D(point, p1, p2, t);
//    }
//    // Set t to 0 if p1 is closest, 1 if p2 is closest, value between 0..1 to represent closest point in middle.
//    public static PointF LinePointClosestToPoint2D(PointF point, PointF p1, PointF p2, out float t)
//    {
//        PointF delta = Delta2D(p1, p2);

//        // Test for zero-length line segment. If so, return that segment's only point.
//        if (delta.X == 0 & delta.Y == 0)
//            return p1;

//        // Calculate the t that minimizes the Distance.
//        t = ((point.X - p1.X) * delta.X + (point.Y - p1.Y) * delta.Y) / (double)(delta.X * delta.X + delta.Y * delta.Y);

//        // See if this represents one of the segment's
//        // end points or a point in the middle.
//        PointF closest;
//        if (t < 0)
//        {
//            t = 0;
//            closest = p1;
//        }
//        else if (t > 1)
//        {
//            t = 1;
//            closest = p2;
//        }
//        else
//        {
//            closest.X = p1.X + t * delta.X;
//            closest.Y = p1.Y + t * delta.Y;
//        }

//        // ' Verify
//        // Dim deltaPt As Point2D = Delta2D(point, closest)
//        // Dim dist As Single = deltaPt.Length
//        // Dim verify As Point2D = Lerp(p1, p2, t)

//        return closest;
//    }

//    // Public Function PointDistanceToLine2D(ByVal ptdPoint As Point2D, ByVal ptdP1 As Point2D, ByVal ptdP2 As Point2D) As Double
//    // Dim ptdDelta As Point2D
//    // Dim ptdClosest As Point2D
//    // Dim t As Double

//    // ptdDelta = Delta2D(ptdP1, ptdP2)

//    // If ptdDelta.X = 0 And ptdDelta.Y = 0 Then _
//    // Return m2DLib.Distance2D(ptdPoint, ptdP1)

//    // ' Calculate the t that minimizes the Distance.
//    // t = ((ptdPoint.X - ptdP1.X) * ptdDelta.X + (ptdPoint.Y - ptdP1.Y) * ptdDelta.Y) / (ptdDelta.X * ptdDelta.X + ptdDelta.Y * _
//    // ptdDelta.Y)

//    // ' See if this represents one of the segment's
//    // ' end points or a point in the middle.
//    // If t < 0 Then
//    // ptdDelta.X = ptdPoint.X - ptdP1.X
//    // ptdDelta.Y = ptdPoint.Y - ptdP1.Y
//    // ptdClosest.X = ptdP1.X
//    // ptdClosest.Y = ptdP1.Y
//    // ElseIf t > 1 Then
//    // ptdDelta.X = ptdPoint.X - ptdP2.X
//    // ptdDelta.Y = ptdPoint.Y - ptdP2.Y
//    // ptdClosest.X = ptdP2.X
//    // ptdClosest.Y = ptdP2.Y
//    // Else
//    // ptdClosest.X = ptdP1.X + t * ptdDelta.X
//    // ptdClosest.Y = ptdP1.Y + t * ptdDelta.Y
//    // ptdDelta.X = ptdPoint.X - ptdClosest.X
//    // ptdDelta.Y = ptdPoint.Y - ptdClosest.Y
//    // End If

//    // Return m2DLib.Distance2D(ptdPoint, ptdClosest)
//    // End Function

//    public static double PointDistanceToLine2D(double dblPointX, double dblPointY, double dblP1X, double dblP1Y, double dblP2X, double dblP2Y)
//    {
//        Point2D ptdDelta;
//        Point2D ptdClosest;
//        double t;

//        ptdDelta = Delta2D(dblP1X, dblP1Y, dblP2X, dblP2Y);

//        if (ptdDelta.X == 0 & ptdDelta.Y == 0)
//            return mDL2DLib.Distance2D(dblPointX, dblPointY, dblP1X, dblP1Y);

//        // Calculate the t that minimizes the Distance.
//        t = ((dblPointX - dblP1X) * ptdDelta.X + (dblPointY - dblP1Y) * ptdDelta.Y) / (ptdDelta.X * ptdDelta.X + ptdDelta.Y * ptdDelta.Y);

//        // See if this represents one of the segment's
//        // end points or a point in the middle.
//        if (t < 0)
//        {
//            ptdClosest.X = dblP1X;
//            ptdClosest.Y = dblP1Y;
//        }
//        else if (t > 1)
//        {
//            ptdClosest.X = dblP2X;
//            ptdClosest.Y = dblP2Y;
//        }
//        else
//        {
//            ptdClosest.X = dblP1X + t * ptdDelta.X;
//            ptdClosest.Y = dblP1Y + t * ptdDelta.Y;
//        }

//        return mDL2DLib.Distance2D(dblPointX, dblPointY, ptdClosest.X, ptdClosest.Y);
//    }

//    public static double PointDistanceToLine2D(Point3D ptdPoint, Point3D ptdP1, Point3D ptdP2)
//    {
//        Point3D ptdDelta;
//        Point3D ptdClosest;
//        double t;

//        ptdDelta = Delta2D(ptdP1, ptdP2);

//        if (ptdDelta.X == 0 & ptdDelta.Y == 0)
//            return mDL2DLib.Distance2D(ptdPoint, ptdP1);

//        // Calculate the t that minimizes the Distance.
//        t = ((ptdPoint.X - ptdP1.X) * ptdDelta.X + (ptdPoint.Y - ptdP1.Y) * ptdDelta.Y) / (double)(ptdDelta.X * ptdDelta.X + ptdDelta.Y * ptdDelta.Y);

//        // See if this represents one of the segment's
//        // end points or a point in the middle.
//        if (t < 0)
//        {
//            ptdDelta.X = ptdPoint.X - ptdP1.X;
//            ptdDelta.Y = ptdPoint.Y - ptdP1.Y;
//            ptdClosest.X = ptdP1.X;
//            ptdClosest.Y = ptdP1.Y;
//        }
//        else if (t > 1)
//        {
//            ptdDelta.X = ptdPoint.X - ptdP2.X;
//            ptdDelta.Y = ptdPoint.Y - ptdP2.Y;
//            ptdClosest.X = ptdP2.X;
//            ptdClosest.Y = ptdP2.Y;
//        }
//        else
//        {
//            ptdClosest.X = ptdP1.X + t * ptdDelta.X;
//            ptdClosest.Y = ptdP1.Y + t * ptdDelta.Y;
//            ptdDelta.X = ptdPoint.X - ptdClosest.X;
//            ptdDelta.Y = ptdPoint.Y - ptdClosest.Y;
//        }

//        return mDL2DLib.Distance2D(ptdPoint, ptdClosest);
//    }


//    public static double ClosestIndexFracAlongPoints(Point2D goalLocation, IList<Point2D> pts)
//    {
//        Point2D closestPt;
//        return ClosestIndexFracAlongPoints(goalLocation, pts, ref closestPt);
//    }

//    /// <summary> aka Closest IndexAndFraction AlongPoints.
//    ///     ''' Integer part of result is index of point at start of a segment,
//    ///     ''' Fraction part of result is (0..1) along segment which ends at pts(index+1).
//    ///     ''' </summary>
//    ///     ''' <param name="goalLocation">Goal to get near to.</param>
//    ///     ''' <param name="pts">Boundary of a closed shape. NO duplicate at end of first point.</param>
//    ///     ''' <param name="closestPt">corresponds to indexFrac; point along segment closest to "goalLocation".</param>
//    ///     ''' <returns></returns>
//    public static double ClosestIndexFracAlongPoints(Point2D goalLocation, IList<Point2D> pts, out Point2D closestPt)
//    {
//        int closestILeg;
//        double closestT;
//        closestPt = ClosestPointOnPointSequence(goalLocation, pts, out closestILeg, out closestT);

//        double closestIndexFrac = closestILeg + closestT;
//        return closestIndexFrac;
//    }

//    // Return "index", set "frac".
//    public static int SeparateIndexFrac(double indexFrac, int wrapAt, out double frac)
//    {
//        int index = System.Convert.ToInt32(Math.Floor(indexFrac));
//        frac = indexFrac - index;

//        // Simplify when round off error moves away from an index.
//        if (frac.NearlyEquals(1, VerySmall))
//        {
//            index += 1;
//            frac = 0;
//        }

//        if (wrapAt == int.MaxValue)
//            return index;
//        else
//            return (index % wrapAt);
//    }

//    // Returns a point along line segments between sequence of points (not just one of the pts; may be anywhere between two adjacent ones).
//    // E.g. "pts" might be line-of-play points from tee(s) to (start/center) green.
//    // closestILeg is 0, for the leg from pts(0) to pts(1). Etc.
//    // closestT (aka "closestFrac") is fraction (lerp weight) along the leg: it is 0 at the start of the leg; 1 at the end of the leg.
//    // The returned value "closestPt" can be calculated from closestILeg and closestT:
//    // = pts(closestILeg) + lerp(pts(closestILeg), pts(closestILeg+1), closestT)
//    // Special case when "closestILeg = LastIndex(pts)", because there is no "pts(closestILeg+1)":
//    // = pts(closestILeg)
//    // = LastElement(pts)
//    public static Point2D ClosestPointOnPointSequence(Point2D location, IList<Point2D> pts, out int closestILeg, out double closestT)
//    {
//        Point2D closestPt = pts[0];
//        closestILeg = 0;
//        closestT = 0;
//        double minDistanceSq = double.MaxValue;

//        // "- 1": Uses next point.
//        for (int iLeg = 0; iLeg <= LastIndex(pts) - 1; iLeg++)
//        {
//            Point2D p1 = pts[iLeg];
//            Point2D p2 = pts[iLeg + 1];
//            double t;
//            Point2D closestPt1 = ClosestPointOnLine2D_AndT(location, p1, p2, ref t);
//            double distanceSq1 = DistanceSquared2D(location, closestPt1);
//            if (distanceSq1 < minDistanceSq)
//            {
//                minDistanceSq = distanceSq1;
//                closestPt = closestPt1;
//                closestILeg = iLeg;
//                closestT = t;
//            }
//        }

//        return closestPt;
//    }

//    // Given an initial position along one leg of a sequence of points,
//    // "startILeg" and "startT" (e.g. ClosestPointOnPointSequence > closestILeg & closestT),
//    // and a distance to move, calculates where to move to.
//    // Won't go beyond end of sequence.
//    // "endILeg" is the START of the final leg that is used. (But if reach very end, it will be the final point.)
//    public static Point2D MoveAlongPointSequence(int startILeg, double startT, double moveDistance, Point2D[] pts, out int endILeg, out double endT)
//    {
//        // At start; these will be moved along below.
//        // "endILeg" is the START of the final leg that is used.
//        endILeg = startILeg;
//        endT = startT;
//        Point2D endPt = pts[startILeg];
//        // additional distance we need to move, from current endPt.
//        double remainingMoveDistance = moveDistance;

//        // Special case if we are already at end.
//        if (startILeg == LastIndex(pts))
//            return endPt;


//        // "<" rather than "<=", because to move, there must be another pt after this.
//        while (endILeg < LastIndex(pts))
//        {
//            // Move along the segment.
//            double legDistance = Distance2D(pts[endILeg], pts[endILeg + 1]);
//            // The current point is this distance along the start leg.
//            // ("endT" will be 0, except on start leg.)
//            double tDistance = endT * legDistance;
//            double remainingDistanceOnLeg = legDistance - tDistance;

//            if (remainingDistanceOnLeg > remainingMoveDistance)
//            {
//                // The move can be completed on the current leg.
//                double deltaT = remainingMoveDistance / legDistance;
//                endT += deltaT;
//                endPt = Lerp(endPt, pts[endILeg + 1], endT);
//                return endPt;
//            }
//            else
//            {
//                // Move to end of leg; which is beginning of next leg.
//                remainingMoveDistance -= remainingDistanceOnLeg;
//                // At beginning of next leg.
//                endILeg += 1;
//                endPt = pts[endILeg];
//                endT = 0;
//            }
//        }


//        // We reached the end of sequence; even if remainingMoveDistance > 0,
//        // we cannot move any farther.
//        return endPt;
//    }

//    // From "startLocation", moves to closest point along sequence of "pts", then moves along that sequence by "moveDistance".
//    public static Point2D PointAheadOnPointSequence(Point2D startLocation, Point2D[] pts, double moveDistance)
//    {
//        int startILeg = 0;
//        // Sets startILeg and startT. startPtOnSequence can be calculated from these (= pts(startILeg) + lerp(pts(startILeg), pts(startILeg+1), startT)).
//        double startT = 0.0;
//        Point2D startPtOnSequence = ClosestPointOnPointSequence(startLocation, pts, out startILeg, out startT);
//        int endILeg = 0;

//        // If will need to move again from endPt, then endILeg and endT would be the input to another call to MoveAlongPointSequence.
//        // Or can simply call PointAheadOnPointSequence again, with endPt as startLocation.
//        // If endILeg = LastIndex(pts) Then
//        // endPt = LastElement(pts)
//        // Else endPt = pts(endILeg) + lerp(pts(endILeg), pts(endILeg+1), endT)
//        double endT = 0.0;
//        Point2D endPt = MoveAlongPointSequence(startILeg, startT, moveDistance, pts, out endILeg, out endT);
//        return endPt;
//    }

//    // Used to calculate landing point along a line-of-play (LOP).
//    // Player is standing at "startLocation".
//    // From "startLocation", moves to closest point along sequence of "lopPts",
//    // then moves along that sequence until we are "landingDistance" from startLocation.
//    // (The final point of lopPts is CG.)
//    // This is different than "PointAheadOnPointSequence", in that the distance is calculated from startLocation, rather than measuring along LOP.
//    public static Point2D HitLineOfPlayAtDistance(Point2D startLocation, Point2D[] lopPts, double landingDistance)
//    {
//        int startILeg;
//        double startT;
//        Point2D closestPtOnSequence = ClosestPointOnPointSequence(startLocation, lopPts, out startILeg, out startT);

//        Point2D guessPt = closestPtOnSequence;
//        double guessDistance = Distance2D(startLocation, guessPt);

//        // If we are > landingDistance from LOP, then the best answer is to hit straight towards LOP.
//        // NOTE: This won't happen in golf, because we would be way out of bounds,
//        // but included for completeness, in case a small landingDistance is passed in.
//        if (guessDistance >= landingDistance)
//            return guessPt;

//        // Examine each point on LOP, until find one that is farther than landingDistance.
//        // The answer will be on the leg leading to that point.
//        // TBD: Is there ever a situation where there is A SECOND solution?
//        Point2D longPt = guessPt;
//        double longDistance = guessDistance;
//        int endILeg = startILeg + 1;
//        while (endILeg <= LastIndex(lopPts))
//        {
//            longPt = lopPts[endILeg];
//            longDistance = Distance2D(startLocation, longPt);
//            // This leg has a point at desired distance.
//            if (longDistance >= landingDistance)
//                break;
//            // Prep Next
//            endILeg += 1;
//        }

//        if (longDistance < landingDistance)
//            // We reached end of LOP, without reaching landingDistance.
//            // Return end of LOP.
//            return LastElement(lopPts);

//        // There is some point along this leg that is at landingDistance.
//        Point2D shortPt = lopPts[endILeg - 1];
//        double shortDistance = Distance2D(startLocation, shortPt);

//        double t;
//        Point2D closestPtOnLeg = ClosestPointOnLine2D_AndT(startLocation, shortPt, longPt, ref t);
//        double closestPtOnLegDistance = Distance2D(startLocation, closestPtOnLeg);
//        if (closestPtOnLegDistance < landingDistance)
//        {
//            // The usual case: closest point on leg falls short.
//            shortPt = closestPtOnLeg;
//            shortDistance = closestPtOnLegDistance;
//        }
//        else
//        {
//            // Don't think this can happen, but just in case:
//            // closest point on leg is long.
//            longPt = closestPtOnLeg;
//            longDistance = closestPtOnLegDistance;
//        }

//        // Theoretically possible, though unlikely to ever happen in practice.
//        // Avoids divide by zero in WgtFromResult.
//        if (shortDistance == longDistance)
//            return longPt;

//        double wgt = WgtFromResult(shortDistance, longDistance, landingDistance);
//        // Point along the leg.
//        // CAUTION: Not exact, because distance calculation from startPoint isn't a linear function along the segment.
//        guessPt = Lerp(shortPt, longPt, wgt);
//        guessDistance = Distance2D(startLocation, guessPt);

//        // Iterate to get more accurate answer.
//        int nMore = 5;
//        while ((nMore > 0))
//        {
//            if (guessDistance > landingDistance + 0.5)
//            {
//                // overshot
//                longPt = guessPt;
//                longDistance = guessDistance;
//            }
//            else if (guessDistance < landingDistance - 0.5)
//            {
//                // undershot
//                shortPt = guessPt;
//                shortDistance = guessDistance;
//            }
//            else
//                // Close enough.
//                break;

//            // Prep Next
//            nMore -= 1;
//            wgt = WgtFromResult(shortDistance, longDistance, landingDistance);
//            guessPt = Lerp(shortPt, longPt, wgt);
//            guessDistance = Distance2D(startLocation, guessPt);
//        }

//        return guessPt;
//    }


//    public static bool IsPointCloseToLine2D(PointF ptfPt, PointF[] ptfPoints, float dblErr)
//    {
//        if (ptfPoints == null)
//            return false;

//        double errDbl = System.Convert.ToDouble(dblErr);
//        double errSquared = errDbl * errDbl;

//        for (int intIdx = 1; intIdx <= ptfPoints.Length - 1; intIdx++)
//        {
//            // Performance: Avoids Sqrt.
//            if (PointDistanceSquaredToLine2D(ptfPt, ptfPoints[intIdx - 1], ptfPoints[intIdx]) < errSquared)
//                return true;
//        }

//        return false;
//    }

//    public static bool IsPointCloseToLine2D(Point2D ptdPt, Point2D[] ptdPoints, double dblErr)
//    {
//        if (ptdPoints == null)
//            return false;

//        for (int intIdx = 1; intIdx <= ptdPoints.Length - 1; intIdx++)
//        {
//            if (PointDistanceToLine2D(ptdPt, ptdPoints[intIdx - 1], ptdPoints[intIdx]) < dblErr)
//                return true;
//        }

//        return false;
//    }

//    public static bool IsPointCloseToLine2D(Point3D ptdPt, Point3D[] ptdPoints, double dblErr)
//    {
//        if (ptdPoints == null)
//            return false;

//        for (int intIdx = 1; intIdx <= ptdPoints.Length - 1; intIdx++)
//        {
//            if (PointDistanceToLine2D(ptdPt, ptdPoints[intIdx - 1], ptdPoints[intIdx]) < dblErr)
//                return true;
//        }

//        return false;
//    }

//    // http://alienryderflex.com/polygon/
//    public static bool IsPointWithinPolygon2D(PointF[] polygon, PointF point)
//    {
//        if (polygon == null)
//            return false;

//        int length = GetLength_ExcludingDuplicateEndpoint(polygon);

//        // Process points
//        bool odd = false;
//        double x = point.X;
//        double y = point.Y;
//        for (int i = 0; i <= length - 1; i++)
//        {
//            int j = (i + 1) % length;
//            double iy = polygon[i].Y;
//            double jy = polygon[j].Y;
//            if ((iy < y && jy >= y) || (jy < y && iy >= y))
//            {
//                double ix = polygon[i].X;
//                double jx = polygon[j].X;
//                if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                    odd = !odd;
//            }
//        }

//        return odd;
//    }

//    // For performance, the tolerance calculation is approximate. Allows somewhat longer diagonal distance.
//    public static bool IsPointWithinToleranceOfPolygon(PointF[] polygon, PointF point, float tolerance)
//    {
//        if (polygon == null)
//            return false;

//        int length = GetLength_ExcludingDuplicateEndpoint(polygon);

//        // Process points
//        bool odd = false;
//        double x = point.X;
//        double y = point.Y;
//        for (int i = 0; i <= length - 1; i++)
//        {
//            int j = (i + 1) % length;

//            PointF pi = polygon[i];
//            PointF pj = polygon[j];

//            double iy = pi.Y;
//            double jy = pj.Y;

//            double minY, maxY;
//            if (jy < iy)
//            {
//                minY = jy; maxY = iy;
//            }
//            else
//            {
//                minY = iy; maxY = jy;
//            }

//            if ((y < minY - tolerance) || (y > maxY + tolerance))
//                continue;

//            double ix = pi.X;
//            double jx = pj.X;

//            // point may be near this span.
//            double minX, maxX;
//            if (ix <= jx)
//            {
//                minX = ix; maxX = jx;
//            }
//            else
//            {
//                minX = jx; maxX = ix;
//            }

//            if (x < minX - tolerance)
//                // Point is safe distance to left. Not within tolerance of this segment.
//                // Not counted as crossing.
//                var toleft = 0;
//            else if (x > maxX + tolerance)
//            {
//                // Point is safe distance to right. Not within tolerance of this segment.
//                // Counts as crossing, if within y-span (not merely within tolerance).
//                // CAUTION: To avoid double-counting at endpoint, one test is "<", other test is "<=".
//                if ((minY < y) && (y <= maxY))
//                    odd = !odd;
//            }
//            else
//            {
//                // This is only time we need thorough check for "within tolerance" of segment.
//                // CAUTION: To avoid double-counting at endpoint, one test is "<", other test is "<=".
//                // (Also avoids divide-by-zero.)

//                PointF delta = Delta2D(pi, pj);

//                // If we get here, we are within tolerance of bounding box surrounding segment.
//                // Avoid Divide By Zero.
//                if (delta.Y == 0)
//                    // SPECIAL CASE: Segment is a point, or a horizontal line.
//                    // We are within tolerance. Approximately; might be slightly longer diagonal distance.
//                    return true;

//                Point2D closest = LinePointClosestToPoint2D(new Point2D(point), new Point2D(pi), new Point2D(pj));
//                // We only need approximate distance to compare to tolerance. The longer edge is an approximation to distance.
//                float approxDistance = System.Convert.ToSingle(Math.Max(Math.Abs(closest.X - x), Math.Abs(closest.Y - y)));
//                if (approxDistance < tolerance)
//                    return true;

//                if ((minY < y) && (y <= maxY))
//                {
//                    if ((ix + ((y - iy) / (jy - iy)) * (jx - ix)) < x)
//                        // Counts as crossing.
//                        odd = !odd;
//                }
//            }
//        }

//        return odd;
//    }

//    // Check if the first point and the last point are equal
//    private static int GetLength_ExcludingDuplicateEndpoint(PointF[] polygon)
//    {
//        int length;

//        int lastIndex1 = LastIndex(polygon);
//        if (polygon[0].X == polygon[lastIndex1].X & polygon[0].Y == polygon[lastIndex1].Y)
//            length = polygon.Length - 1;
//        else
//            length = polygon.Length;

//        return length;
//    }

//    public static bool IsPointWithinPolygon2D(Point2D[] ptdPolygon, Point2D point)
//    {
//        // http://alienryderflex.com/ptdPolygon/

//        if (ptdPolygon == null)
//            return false;

//        bool odd = false;
//        int length;
//        int i, j;
//        double ix, iy, jx, jy, x, y;

//        // Check if the first point and the last point are equal
//        if (ptdPolygon[0].X == ptdPolygon[ptdPolygon.Length - 1].X & ptdPolygon[0].Y == ptdPolygon[ptdPolygon.Length - 1].Y)
//            length = ptdPolygon.Length - 1;
//        else
//            length = ptdPolygon.Length;

//        // Process points
//        for (i = 0; i <= length - 1; i++)
//        {
//            j = (i + 1) % length;
//            ix = ptdPolygon[i].X;
//            iy = ptdPolygon[i].Y;
//            jx = ptdPolygon[j].X;
//            jy = ptdPolygon[j].Y;
//            x = point.X;
//            y = point.Y;
//            if ((iy < y & jy >= y) | (jy < y & iy >= y))
//            {
//                if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                    odd = !odd;
//            }
//        }

//        return odd;
//    }

//    public static bool IsPointWithinPolygon2D(Point3D[] ptdPolygon, Point3D point)
//    {
//        // http://alienryderflex.com/ptdPolygon/

//        if (ptdPolygon == null)
//            return false;

//        bool odd = false;
//        int length;
//        int i, j;
//        double ix, iy, jx, jy, x, y;

//        // Check if the first point and the last point are equal
//        if (ptdPolygon[0].X == ptdPolygon[ptdPolygon.Length - 1].X & ptdPolygon[0].Y == ptdPolygon[ptdPolygon.Length - 1].Y)
//            length = ptdPolygon.Length - 1;
//        else
//            length = ptdPolygon.Length;

//        // Process points
//        for (i = 0; i <= length - 1; i++)
//        {
//            j = (i + 1) % length;
//            ix = ptdPolygon[i].X;
//            iy = ptdPolygon[i].Y;
//            jx = ptdPolygon[j].X;
//            jy = ptdPolygon[j].Y;
//            x = point.X;
//            y = point.Y;
//            if ((iy < y & jy >= y) | (jy < y & iy >= y))
//            {
//                if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                    odd = !odd;
//            }
//        }

//        return odd;
//    }

//    public static bool IsPointWithinPolygon2D_Test_If_Faster(ref Point3D[] ptdPolygon, Point3D point, int intStartIdx, int intEndIdx)
//    {
//        // http://alienryderflex.com/ptdPolygon/

//        if (ptdPolygon == null)
//            return false;

//        bool odd = false;
//        int length;
//        int i, j;
//        double ix, iy, jx, jy, x, y;

//        // Check if the first point and the last point are equal
//        if (ptdPolygon[0].X == ptdPolygon[ptdPolygon.Length - 1].X & ptdPolygon[0].Y == ptdPolygon[ptdPolygon.Length - 1].Y)
//            // length = ptdPolygon.Length - 1
//            length = intEndIdx - intStartIdx;
//        else
//            // length = ptdPolygon.Length
//            length = intEndIdx - intStartIdx + 1;

//        // Process points
//        for (i = intStartIdx; i <= intEndIdx; i++)
//        {
//            j = (i + 1) % length;
//            ix = ptdPolygon[i].X;
//            iy = ptdPolygon[i].Y;
//            jx = ptdPolygon[j].X;
//            jy = ptdPolygon[j].Y;
//            x = point.X;
//            y = point.Y;
//            if ((iy < y & jy >= y) | (jy < y & iy >= y))
//            {
//                if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                    odd = !odd;
//            }
//        }

//        return odd;
//    }

//    public static bool IsPointsWithinRectangle2D(PointF[] ptfRec, PointF[] ptfPts)
//    {
//        if (ptfRec == null)
//            return false;

//        for (int intIdx = 0; intIdx <= ptfPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptfPts[intIdx], ptfRec))
//                return true;
//        }

//        return false;
//    }

//    public static bool IsPointsWithinRectangle2D(Point2D[] ptdRec, Point2D[] ptdPts)
//    {
//        if (ptdRec == null)
//            return false;

//        for (int intIdx = 0; intIdx <= ptdPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptdPts[intIdx], ptdRec))
//                return true;
//        }

//        return false;
//    }

//    public static bool IsPointsWithinRectangle2D(Point3D[] ptdRec, Point3D[] ptdPts)
//    {
//        if (ptdRec == null)
//            return false;

//        for (int intIdx = 0; intIdx <= ptdPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptdPts[intIdx], ptdRec))
//                return true;
//        }

//        return false;
//    }

//    // PERFORMANCE: Caller should do polygon (rectangular) bounds check before calling this, to quickly handle case where ptfRec is entirely outside that bounds.
//    public static bool DoesRectangleIntersectPolygon2DInterior(PointF[] ptfPolygon, PointF[] ptfRec)
//    {
//        // Cheap check: center and 4 corners.
//        foreach (PointF p in ptfRec)
//        {
//            if (IsPointWithinPolygon2D(ptfPolygon, p))
//                return true;
//        }

//        if (IsPointWithinPolygon2D(ptfPolygon, PointFRect_Center(ptfRec)))
//            return true;

//        // TODO: Check intersection of any of 4 lines with polygon interior. How?
//        // NOTE: Only matters if line passes ACROSS the polygon (with none of the rect corners inside the polygon -- those were tested above).
//        // HACK: Check 4 middles of lines. If those all miss, consider that "good enough" test.
//        if (IsPointWithinPolygon2D(ptfPolygon, Average(ptfRec[0], ptfRec[1])) || IsPointWithinPolygon2D(ptfPolygon, Average(ptfRec[1], ptfRec[2])) || IsPointWithinPolygon2D(ptfPolygon, Average(ptfRec[2], ptfRec[3])) || IsPointWithinPolygon2D(ptfPolygon, Average(ptfRec[3], ptfRec[0])))
//            return true;

//        return false;
//    }

//    // VERY SLOW.
//    // CAUTION: Won't detect case where one polygon completely encloses the other one.
//    public static bool PolygonsIntersects2D(PointF[] ptfPoly1, PointF[] ptfPoly2)
//    {
//        if (ptfPoly1 == null | ptfPoly2 == null)
//            return false;

//        for (int intIdx = 1; intIdx <= ptfPoly1.Length - 1; intIdx++)
//        {
//            for (int intIdx2 = 1; intIdx2 <= ptfPoly2.Length - 1; intIdx2++)
//            {
//                if (LinesIntersects2D(ptfPoly1[intIdx - 1], ptfPoly1[intIdx], ptfPoly2[intIdx2 - 1], ptfPoly2[intIdx2]))
//                    return true;
//            }
//        }

//        return false;
//    }

//    // VERY SLOW.
//    // Closed = include a segment between last point and first point.
//    public static bool PolygonClosedIntersects2D(PointF[] poly1, PointF[] poly2)
//    {
//        if ((poly1 == null) || (poly2 == null))
//            return false;

//        for (int index1 = 1; index1 <= poly1.Length; index1++)
//        {
//            for (int index2 = 1; index2 <= poly2.Length; index2++)
//            {
//                int index1wrap = !index1 == poly1.Length ? index1 : 0;
//                int index2wrap = !index2 == poly2.Length ? index2 : 0;
//                if (LinesIntersects2D(poly1[index1 - 1], poly1[index1wrap], poly2[index2 - 1], poly2[index2wrap]))
//                    return true;
//            }
//        }

//        return false;
//    }

//    // VERY SLOW.
//    // CAUTION: Won't detect case where one polygon completely encloses the other one.
//    public static bool PolygonsIntersects2D(Point2D[] ptdPoly1, Point2D[] ptdPoly2)
//    {
//        if (ptdPoly1 == null | ptdPoly2 == null)
//            return false;

//        for (int intIdx = 1; intIdx <= ptdPoly1.Length - 1; intIdx++)
//        {
//            for (int intIdx2 = 1; intIdx2 <= ptdPoly2.Length - 1; intIdx2++)
//            {
//                if (LinesIntersects2D(ptdPoly1[intIdx - 1], ptdPoly1[intIdx], ptdPoly2[intIdx2 - 1], ptdPoly2[intIdx2]))
//                    return true;
//            }
//        }

//        return false;
//    }

//    // VERY SLOW.
//    // CAUTION: Won't detect case where one polygon completely encloses the other one.
//    public static bool PolygonsIntersects2D(Point3D[] ptdPoly1, Point3D[] ptdPoly2)
//    {
//        if (ptdPoly1 == null | ptdPoly2 == null)
//            return false;

//        for (int intIdx = 1; intIdx <= ptdPoly1.Length - 1; intIdx++)
//        {
//            for (int intIdx2 = 1; intIdx2 <= ptdPoly2.Length - 1; intIdx2++)
//            {
//                if (LinesIntersects2D(ptdPoly1[intIdx - 1], ptdPoly1[intIdx], ptdPoly2[intIdx2 - 1], ptdPoly2[intIdx2]))
//                    return true;
//            }
//        }

//        return false;
//    }

//    // True if line segment (p1, p2) intersects any edge segment of polygon represented by polyPts.
//    public static bool SegmentIntersectsPolygon2D(Point2D p1, Point2D p2, IList<Point2D> polyPts)
//    {
//        Point2D minP = p1.Min(p2);
//        Point2D maxP = p1.Max(p2);

//        Point2D priorPt = polyPts[0];
//        for (int i = 1; i <= LastIndex(polyPts); i++)
//        {
//            Point2D pt = polyPts[i];
//            if (mDL2DLib.MinMaxIntersect(minP, maxP, pt.Min(priorPt), pt.Max(priorPt)))
//            {
//                if (LinesIntersects2D(p1, p2, priorPt, pt))
//                    return true;
//            }
//            // Prep Next
//            priorPt = pt;
//        }

//        return false;
//    }


//    public static Point2D ExtendLine2D(Point2D ptdP1, Point2D ptdP2, double dblDistance)
//    {
//        double dblTotalDistance = mDL2DLib.Distance2D(ptdP1, ptdP2) + dblDistance;
//        double dblDAngle = GetAngleDegrees2D(ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y);

//        return new Point2D(ptdP1.X + dblTotalDistance * Math.Cos((dblDAngle / 180) * Math.PI), ptdP1.Y + dblTotalDistance * Math.Sin((dblDAngle / 180) * Math.PI));
//    }

//    public static PointF ExtendLine2D(PointF ptfP1, PointF ptfP2, float sngDistance)
//    {
//        double dblTotalDistance = mDL2DLib.Distance2D(ptfP1, ptfP2) + sngDistance;
//        double dblDAngle = GetAngleDegrees2D(ptfP1.X, ptfP1.Y, ptfP2.X, ptfP2.Y);

//        return new PointF(System.Convert.ToSingle(ptfP1.X + dblTotalDistance * Math.Cos((dblDAngle / 180) * Math.PI)), System.Convert.ToSingle(ptfP1.Y + dblTotalDistance * Math.Sin((dblDAngle / 180) * Math.PI)));
//    }

//    public static double ReverseAngle(double dblAngle)
//    {
//        return 360 - dblAngle;
//    }

//    public static float ReverseAngle(float sngAngle)
//    {
//        return 360 - sngAngle;
//    }

//    public static int ReverseAngle(int intAngle)
//    {
//        return 360 - intAngle;
//    }

//    // Given an arbitrary angle in degrees, move it into range (-180, +180].
//    public static double Degrees_Signed180(double degrees)
//    {
//        degrees = degrees % 360;

//        if (degrees <= -180)
//            degrees += 360;
//        else if (degrees > 180)
//            degrees -= 360;

//        return degrees;
//    }

//    // Convert radians to degrees; return within range (-180, +180].
//    public static double RadiansToDegrees_Signed180(double radians)
//    {
//        return Degrees_Signed180(RadiansToDegrees(radians));
//    }

//    public static double RadiansToDegrees(double dblRadians)
//    {
//        return dblRadians * 180 / Math.PI;
//    }

//    public static double DegreesToRadians(double dblDegrees)
//    {
//        return dblDegrees * Math.PI / 180;
//    }

//    public const double TwoPI = 2 * Math.PI;

//    public const double Degrees90AsRadians = Math.PI / 2.0;
//    public const double Degrees89_9AsRadians = Degrees90AsRadians * 89.9 / 90;
//    public const double Degrees180AsRadians = Math.PI;
//    public const double Degrees270AsRadians = 3 * Math.PI / 2.0;
//    public const double Degrees360AsRadians = TwoPI;



//    // Returnerar antalet punkter ur ptdPts som ligger inom ptfRec
//    public static int PointsCountInRectangle2D(PointF[] ptfRec, PointF[] ptfPts)
//    {
//        if (ptfRec == null)
//            return -1;
//        if (ptfPts == null)
//            return -1;

//        int intPtsCtr = 0;

//        for (int intIdx = 0; intIdx <= ptfPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptfPts[intIdx], ptfRec))
//                intPtsCtr += 1;
//        }

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(Point2D[] ptdRec, Point2D[] ptdPts)
//    {
//        if (ptdRec == null)
//            return -1;
//        if (ptdPts == null)
//            return -1;

//        int intPtsCtr = 0;

//        for (int intIdx = 0; intIdx <= ptdPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptdPts[intIdx], ptdRec))
//                intPtsCtr += 1;
//        }

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(Point3D[] ptdRec, Point3D[] ptdPts)
//    {
//        if (ptdRec == null)
//            return -1;
//        if (ptdPts == null)
//            return -1;

//        int intPtsCtr = 0;

//        for (int intIdx = 0; intIdx <= ptdPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptdPts[intIdx], ptdRec))
//                intPtsCtr += 1;
//        }

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(RectangleF rcfRec, PointF[] ptfPts)
//    {
//        if (ptfPts == null)
//            return -1;

//        int intPtsCtr = 0;

//        for (int intIdx = 0; intIdx <= ptfPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptfPts[intIdx], rcfRec))
//                intPtsCtr += 1;
//        }

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(RectangleF rcfRec, Point2D[] ptdPts)
//    {
//        if (ptdPts == null)
//            return -1;

//        int intPtsCtr = 0;

//        for (int intIdx = 0; intIdx <= ptdPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptdPts[intIdx], rcfRec))
//                intPtsCtr += 1;
//        }

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(RectangleF rcfRec, Point3D[] ptdPts)
//    {
//        if (ptdPts == null)
//            return -1;

//        int intPtsCtr = 0;

//        for (int intIdx = 0; intIdx <= ptdPts.Length - 1; intIdx++)
//        {
//            if (PointInsideRectangle2D(ptdPts[intIdx], rcfRec))
//                intPtsCtr += 1;
//        }

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(RectangleF rcfRec, RectangleF rcfPts)
//    {
//        int intPtsCtr = 0;

//        if (PointInsideRectangle2D(new PointF(rcfPts.X, rcfPts.Y), rcfRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new PointF(rcfPts.X + rcfPts.Width, rcfPts.Y), rcfRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new PointF(rcfPts.X + rcfPts.Width, rcfPts.Y + rcfPts.Height), rcfRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new PointF(rcfPts.X, rcfPts.Y + rcfPts.Height), rcfRec))
//            intPtsCtr += 1;

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(PointF[] ptfRec, RectangleF rcfPts)
//    {
//        if (ptfRec == null)
//            return -1;

//        int intPtsCtr = 0;

//        if (PointInsideRectangle2D(new PointF(rcfPts.X, rcfPts.Y), ptfRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new PointF(rcfPts.X + rcfPts.Width, rcfPts.Y), ptfRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new PointF(rcfPts.X + rcfPts.Width, rcfPts.Y + rcfPts.Height), ptfRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new PointF(rcfPts.X, rcfPts.Y + rcfPts.Height), ptfRec))
//            intPtsCtr += 1;

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(Point2D[] ptdRec, RectangleF rcfPts)
//    {
//        if (ptdRec == null)
//            return -1;

//        int intPtsCtr = 0;

//        if (PointInsideRectangle2D(new Point2D(rcfPts.X, rcfPts.Y), ptdRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new Point2D(rcfPts.X + rcfPts.Width, rcfPts.Y), ptdRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new Point2D(rcfPts.X + rcfPts.Width, rcfPts.Y + rcfPts.Height), ptdRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new Point2D(rcfPts.X, rcfPts.Y + rcfPts.Height), ptdRec))
//            intPtsCtr += 1;

//        return intPtsCtr;
//    }

//    public static int PointsCountInRectangle2D(Point3D[] ptdRec, RectangleF rcfPts)
//    {
//        if (ptdRec == null)
//            return -1;

//        int intPtsCtr = 0;

//        if (PointInsideRectangle2D(new Point3D(rcfPts.X, rcfPts.Y), ptdRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new Point3D(rcfPts.X + rcfPts.Width, rcfPts.Y), ptdRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new Point3D(rcfPts.X + rcfPts.Width, rcfPts.Y + rcfPts.Height), ptdRec))
//            intPtsCtr += 1;
//        if (PointInsideRectangle2D(new Point3D(rcfPts.X, rcfPts.Y + rcfPts.Height), ptdRec))
//            intPtsCtr += 1;

//        return intPtsCtr;
//    }

//    public static Point2D PolygonCentroid2D(Point2D[] ptdPolygon, double dblDivider)
//    {
//        if (ptdPolygon == null || ptdPolygon.Length < 1)
//            return new Point2D(double.NegativeInfinity, double.NegativeInfinity);
//        if (ptdPolygon.Length == 1)
//            return ptdPolygon[0];
//        if (ptdPolygon.Length == 2)
//            return Average(ptdPolygon[0], ptdPolygon[1]);
//        try
//        {
//            double dblA = 0;
//            double dblXC = 0;
//            double dblYC = 0;
//            var dblMinX = ptdPolygon[0].X;
//            double dblMinY = ptdPolygon[0].Y;

//            for (int intIdx = 1; intIdx <= ptdPolygon.Length - 1; intIdx++)
//            {
//                double dblX = ptdPolygon[intIdx].X;
//                double dblY = ptdPolygon[intIdx].Y;

//                if (dblX < dblMinX)
//                    dblMinX = dblX;
//                if (dblY < dblMinY)
//                    dblMinY = dblY;
//            }

//            Point2D ptdMin = new Point2D(dblMinX / dblDivider, dblMinY / dblDivider);

//            for (int intIdx = 0; intIdx <= ptdPolygon.Length - 1; intIdx++)
//            {
//                Point2D ptdP1 = ptdPolygon[intIdx];
//                Point2D ptdP2 = ptdPolygon[(intIdx + 1) % ptdPolygon.Length];
//                if (ptdP2 == ptdP1)
//                    continue; // E.g. if final point is same as first point.

//                ptdP1.X /= dblDivider; ptdP1.Y /= dblDivider;
//                ptdP2.X /= dblDivider; ptdP2.Y /= dblDivider;

//                ptdP1 -= ptdMin;
//                ptdP2 -= ptdMin;

//                double dblP = ptdP1.X * ptdP2.Y - ptdP2.X * ptdP1.Y;

//                dblA += dblP;
//                dblXC += (ptdP1.X + ptdP2.X) * dblP;
//                dblYC += (ptdP1.Y + ptdP2.Y) * dblP;
//            }

//            Point2D ptdRet;
//            if (dblA == 0.0)
//            {
//                // Failed - simply average the points. (Happens if only two points.)
//                Point2D pSum = new Point2D(0, 0);
//                int len = ptdPolygon.Length;
//                if (LastElement(ptdPolygon) == ptdPolygon[0])
//                    len -= 1;
//                for (int i = 0; i <= len - 1; i++)
//                    pSum += ptdPolygon[i];
//                ptdRet = pSum / (double)len;
//            }
//            else
//            {
//                dblA /= 2;
//                ptdRet = new Point2D(dblXC / (6 * dblA), dblYC / (6 * dblA)) + ptdMin;
//                ptdRet.X *= dblDivider; ptdRet.Y *= dblDivider;
//            }

//            return ptdRet;
//        }
//        catch (Exception ex)
//        {
//            return new Point2D(double.NegativeInfinity, double.NegativeInfinity);
//        }
//    }

//    public static Point2D PolygonFindBestCentroidOnObject(Point2D[] ptdPolygon, Point2D ptdOrigo)
//    {
//        if (ptdPolygon == null)
//            return Point2D.Zero();
//        var dblMinX = ptdPolygon[0].X;
//        double dblMinY = ptdPolygon[0].Y;
//        var dblMaxX = ptdPolygon[0].X;
//        double dblMaxY = ptdPolygon[0].Y;
//        double dblFurthest;

//        for (int intIdx = 0; intIdx <= ptdPolygon.Length - 1; intIdx++)
//        {
//            if (ptdPolygon[intIdx].X < dblMinX)
//                dblMinX = ptdPolygon[intIdx].X;
//            if (ptdPolygon[intIdx].Y < dblMinY)
//                dblMinY = ptdPolygon[intIdx].Y;

//            if (ptdPolygon[intIdx].X < dblMaxX)
//                dblMaxX = ptdPolygon[intIdx].X;
//            if (ptdPolygon[intIdx].Y < dblMaxY)
//                dblMaxY = ptdPolygon[intIdx].Y;

//            double dblDistance = Distance2D(ptdOrigo, ptdPolygon[intIdx]);

//            if (dblDistance > dblFurthest)
//                dblFurthest = dblDistance;
//        }

//        dblFurthest *= 2;

//        Point2D ptdMin = new Point2D(dblMinX - 1, dblMinY - 1);
//        Point2D ptdMax = new Point2D(dblMaxX - 1, dblMaxY - 1);
//        Point2D ptdOrigoD = ptdOrigo - ptdMin;
//        Point2D[] ptdHits = null;
//        int intAcc = 8;

//        for (int intIdxAngle = 1; intIdxAngle <= intAcc; intIdxAngle++)
//        {
//            Point2D ptdRay;

//            ptdRay = RotateAtByDegrees2D(ptdOrigo, ptdOrigo + dblFurthest, 360 / (double)intAcc * intIdxAngle);

//            for (int intIdx = 0; intIdx <= ptdPolygon.Length - 1; intIdx++)
//            {
//                Point2D ptdP1 = ptdPolygon[intIdx] - ptdMin;
//                Point2D ptdP2 = ptdPolygon[(intIdx + 1) % ptdPolygon.Length] - ptdMin;

//                if (LinesIntersects2D(ptdP1, ptdP2, ptdOrigoD, ptdRay - ptdMin))
//                {
//                    Point2D ptdIsect = LinesIntersectsAt2D(ptdP1, ptdP2, ptdOrigoD, ptdRay - ptdMin);

//                    if (!(ptdIsect.X == 0 && ptdIsect.Y == 0))
//                    {
//                        if (ptdHits == null)
//                            ptdHits = new Point2D[1];
//                        else
//                        {
//                            var oldPtdHits = ptdHits;
//                            ptdHits = new Point2D[ptdHits.Length + 1];
//                            if (oldPtdHits != null)
//                                Array.Copy(oldPtdHits, ptdHits, Math.Min(ptdHits.Length + 1, oldPtdHits.Length));
//                        }

//                        ptdHits[ptdHits.Length - 1] = ptdIsect + ptdMin;
//                    }
//                }
//            }
//        }

//        if (!ptdHits == null)
//        {
//            double dblClosestDI = double.NegativeInfinity;
//            int intClosestIdx = -1;

//            for (int intIdx = 0; intIdx <= ptdHits.Length - 1; intIdx++)
//            {
//                double dblDistance = Distance2D(ptdOrigo, ptdHits[intIdx]);

//                if (double.IsInfinity(dblClosestDI) || dblDistance < dblClosestDI)
//                {
//                    dblClosestDI = dblDistance;
//                    intClosestIdx = intIdx;
//                }
//            }

//            if (intClosestIdx >= 0)
//                return ptdHits[intClosestIdx];
//        }

//        return Point2D.Zero();
//    }

//    // Public Function DistanceBetweenRectangles2D(ByVal ptfRec1() As PointF, ByVal ptfRec2() As PointF) As Double
//    // If ptfRec1 Is Nothing Then Return -1
//    // If ptfRec2 Is Nothing Then Return -1

//    // Dim dblDi As Double = -1

//    // ' Returns 0 if the both rectangles intersects

//    // If RectanglesIntersects2D(ptfRec1, ptfRec2) Then Return 0

//    // For intIdx As Integer = 0 To 3
//    // For intIdx2 As Integer = 0 To 3
//    // Dim dblDI1 As Double = PointDistanceToLine2D(ptfRec1(intIdx2), ptfRec2(IIf(intIdx = 0, 3, intIdx - 1)), ptfRec2(intIdx))
//    // Dim dblDI2 As Double = PointDistanceToLine2D(ptfRec2(intIdx2), ptfRec1(IIf(intIdx = 0, 3, intIdx - 1)), ptfRec1(intIdx))

//    // If dblDi = -1 Or dblDI1 < dblDi Then dblDi = dblDI1
//    // If dblDi = -1 Or dblDI2 < dblDi Then dblDi = dblDI2
//    // Next
//    // Next

//    // Return dblDi
//    // End Function

//    // Public Function DistanceBetweenRectangles2D(ByVal ptdRec1() As Point2D, ByVal ptdRec2() As Point2D) As Double
//    // If ptdRec1 Is Nothing Then Return -1
//    // If ptdRec2 Is Nothing Then Return -1

//    // Dim dblDi As Double = -1

//    // ' Returns 0 if the both rectangles intersects

//    // If RectanglesIntersects2D(ptdRec1, ptdRec2) Then Return 0

//    // For intIdx As Integer = 0 To 3
//    // For intIdx2 As Integer = 0 To 3
//    // Dim dblDI1 As Double = PointDistanceToLine2D(ptdRec1(intIdx2), ptdRec2(IIf(intIdx = 0, 3, intIdx - 1)), ptdRec2(intIdx))
//    // Dim dblDI2 As Double = PointDistanceToLine2D(ptdRec2(intIdx2), ptdRec1(IIf(intIdx = 0, 3, intIdx - 1)), ptdRec1(intIdx))

//    // If dblDi = -1 Or dblDI1 < dblDi Then dblDi = dblDI1
//    // If dblDi = -1 Or dblDI2 < dblDi Then dblDi = dblDI2
//    // Next
//    // Next

//    // Return dblDi
//    // End Function

//    // Public Function DistanceBetweenRectangles2D(ByVal ptdRec1() As Point3D, ByVal ptdRec2() As Point3D) As Double
//    // If ptdRec1 Is Nothing Then Return -1
//    // If ptdRec2 Is Nothing Then Return -1

//    // Dim dblDi As Double = -1

//    // ' Returns 0 if the both rectangles intersects

//    // 'HACK FIXA DENNA ASAP
//    // 'If RectanglesIntersects2D(ptdRec1, ptdRec2) Then Return 0

//    // For intIdx As Integer = 0 To 3
//    // For intIdx2 As Integer = 0 To 3
//    // Dim dblDI1 As Double = PointDistanceToLine2D(ptdRec1(intIdx2), ptdRec2(IIf(intIdx = 0, 3, intIdx - 1)), ptdRec2(intIdx))
//    // Dim dblDI2 As Double = PointDistanceToLine2D(ptdRec2(intIdx2), ptdRec1(IIf(intIdx = 0, 3, intIdx - 1)), ptdRec1(intIdx))

//    // If dblDi = -1 Or dblDI1 < dblDi Then dblDi = dblDI1
//    // If dblDi = -1 Or dblDI2 < dblDi Then dblDi = dblDI2
//    // Next
//    // Next

//    // Return dblDi
//    // End Function

//    public static Point2D[] Create_LinesByControlPoints(Point2D[] ptdPoints)
//    {
//        if (ptdPoints == null)
//            return null;
//        // ptdPoints = ptdPoints.Clone

//        Drawing2D.GraphicsPath gfpSplines = new Drawing2D.GraphicsPath();
//        PointF[] ptfPoints = new PointF[ptdPoints.Length - 1 + 1];

//        Point2D ptdRP = ptdPoints[0];

//        for (int intIdx = 0; intIdx <= ptdPoints.GetUpperBound(0); intIdx++)
//            ptfPoints[intIdx] = new PointF(System.Convert.ToSingle((ptdPoints[intIdx].X - ptdRP.X) * 10), System.Convert.ToSingle((ptdPoints[intIdx].Y - ptdRP.Y) * 10));

//        gfpSplines.AddLines(ptfPoints);

//        Point2D[] ptdRetPoints = new Point2D[gfpSplines.PathPoints.GetUpperBound(0) + 1];
//        PointF[] ptfPathPoints = gfpSplines.PathPoints;
//        for (int intIdx = 0; intIdx <= ptdRetPoints.GetUpperBound(0); intIdx++)
//            ptdRetPoints[intIdx] = new Point2D((ptfPathPoints[intIdx].X / (double)10) + ptdRP.X, (ptfPathPoints[intIdx].Y / (double)10) + ptdRP.Y);

//        return ptdRetPoints;
//    }

//    public static PointF[] Create_LinesByControlPoints(PointF[] ptfPoints)
//    {
//        if (ptfPoints == null)
//            return null;

//        return (PointF[])ptfPoints.Clone();
//    }

//    public static Point2D[] Create_PolygonByControlPoints(Point2D[] ptdPoints)
//    {
//        if (ptdPoints == null)
//            return null;
//        // ptdPoints = ptdPoints.Clone

//        // Dim gfpSplines As New Drawing2D.GraphicsPath
//        // Dim ptfPoints(ptdPoints.Length - 1) As PointF

//        // Dim ptdRP As Point2D = ptdPoints(0)

//        // For intIdx As Integer = 0 To ptdPoints.GetUpperBound(0)
//        // ptdPoints(intIdx).X -= ptdRP.X ' This is a fix for when the Point2D are to big numbers
//        // ptdPoints(intIdx).Y -= ptdRP.Y
//        // ptfPoints(intIdx) = ptdPoints(intIdx).ToPointF
//        // ptfPoints(intIdx).X *= 10
//        // ptfPoints(intIdx).Y *= 10
//        // Next

//        // gfpSplines.AddPolygon(ptfPoints)

//        // Dim ptdRetPoints(gfpSplines.PathPoints.GetUpperBound(0)) As Point2D

//        // For intIdx As Integer = 0 To ptdRetPoints.GetUpperBound(0)
//        // ptdRetPoints(intIdx) = New Point2D(gfpSplines.PathPoints(intIdx).X, gfpSplines.PathPoints(intIdx).Y)
//        // ptdRetPoints(intIdx).X /= 10
//        // ptdRetPoints(intIdx).Y /= 10
//        // ptdRetPoints(intIdx).X += ptdRP.X
//        // ptdRetPoints(intIdx).Y += ptdRP.Y
//        // Next

//        if (!ptdPoints[0].X == ptdPoints[ptdPoints.GetUpperBound(0)].X | !ptdPoints[0].Y == ptdPoints[ptdPoints.GetUpperBound(0)].Y)
//        {
//            var oldPtdPoints = ptdPoints;
//            ptdPoints = new Point2D[ptdPoints.GetUpperBound(0) + 1 + 1];
//            if (oldPtdPoints != null)
//                Array.Copy(oldPtdPoints, ptdPoints, Math.Min(ptdPoints.GetUpperBound(0) + 1 + 1, oldPtdPoints.Length));
//            ptdPoints[ptdPoints.GetUpperBound(0)] = ptdPoints[0];
//        }

//        return ptdPoints;
//    }

//    public static PointF[] Create_PolygonByControlPoints(PointF[] ptfPoints)
//    {
//        if (ptfPoints == null)
//            return null;

//        Drawing2D.GraphicsPath gfpSplines = new Drawing2D.GraphicsPath();

//        gfpSplines.AddPolygon(ptfPoints);

//        return gfpSplines.PathPoints;
//    }

//    private static int intSmoothness = 80;
//    private static int intPrecision = 1;

//    public static Point2D[] Create_ClosedSimplyfiedCardinalByControlPoints(Point2D[] ptdPoints)
//    {
//        if (ptdPoints == null)
//            return null;
//        // ptdPoints = ptdPoints.Clone

//        Drawing2D.GraphicsPath gfpSplines = new Drawing2D.GraphicsPath();
//        PointF[] ptfPoints = new PointF[ptdPoints.Length - 1 + 1];

//        Point2D ptdRP = ptdPoints[0];

//        for (int intIdx = 0; intIdx <= ptdPoints.GetUpperBound(0); intIdx++)
//        {
//            // ptdPoints(intIdx).X -= ptdRP.X ' This is a fix for when the Point2D are to big numbers
//            // ptdPoints(intIdx).Y -= ptdRP.Y
//            // ptfPoints(intIdx) = ptdPoints(intIdx).ToPointF
//            ptfPoints[intIdx].X = System.Convert.ToSingle(ptdPoints[intIdx].X - ptdRP.X);
//            ptfPoints[intIdx].Y = System.Convert.ToSingle(ptdPoints[intIdx].Y - ptdRP.Y);
//        }

//        gfpSplines.AddClosedCurve(ptfPoints);
//        gfpSplines.Flatten();

//        PointF[] ptfRetPoints = BezierApproximationToPointF(gfpSplines.PathPoints, intSmoothness, intPrecision, false);
//        Point2D[] ptdRetPoints;

//        ptdRetPoints = new Point2D[ptfRetPoints.GetUpperBound(0) + 1];

//        for (int intIdx = 0; intIdx <= ptfRetPoints.GetUpperBound(0); intIdx++)
//        {
//            ptdRetPoints[intIdx] = new Point2D(ptfRetPoints[intIdx].X, ptfRetPoints[intIdx].Y);
//            ptdRetPoints[intIdx].X += ptdRP.X;
//            ptdRetPoints[intIdx].Y += ptdRP.Y;
//        }

//        return ptdRetPoints;
//    }

//    public static PointF[] Create_ClosedSimplyfiedCardinalByControlPoints(PointF[] ptfPoints)
//    {
//        if (ptfPoints == null)
//            return null;

//        Drawing2D.GraphicsPath gfpSplines = new Drawing2D.GraphicsPath();

//        gfpSplines.AddClosedCurve(ptfPoints);
//        gfpSplines.Flatten();

//        PointF[] ptfRetPoints = BezierApproximationToPointF(gfpSplines.PathPoints, intSmoothness, intPrecision, false);

//        return ptfRetPoints;
//    }

//    public static Point2D[] Create_SimplyfiedCardinalByControlPoints(Point2D[] ptdPoints)
//    {
//        if (ptdPoints == null)
//            return null;
//        // ptdPoints = ptdPoints.Clone

//        Drawing2D.GraphicsPath gfpSplines = new Drawing2D.GraphicsPath();
//        PointF[] ptfPoints = new PointF[ptdPoints.Length - 1 + 1];

//        Point2D ptdRP = ptdPoints[0];

//        for (int intIdx = 0; intIdx <= ptdPoints.GetUpperBound(0); intIdx++)
//            ptfPoints[intIdx] = new PointF(System.Convert.ToSingle(ptdPoints[intIdx].X - ptdRP.X), System.Convert.ToSingle(ptdPoints[intIdx].Y - ptdRP.Y));

//        gfpSplines.AddCurve(ptfPoints);
//        gfpSplines.Flatten();

//        PointF[] ptfRetPoints = BezierApproximationToPointF(gfpSplines.PathPoints, intSmoothness, intPrecision, false);
//        Point2D[] ptdRetPoints = new Point2D[ptfRetPoints.GetUpperBound(0) + 1];

//        for (int intIdx = 0; intIdx <= ptfRetPoints.GetUpperBound(0); intIdx++)
//            ptdRetPoints[intIdx] = new Point2D(ptfRetPoints[intIdx].X + ptdRP.X, ptfRetPoints[intIdx].Y + ptdRP.Y);

//        return ptdRetPoints;
//    }

//    public static PointF[] Create_SimplyfiedCardinalByControlPoints(PointF[] ptfPoints)
//    {
//        if (ptfPoints == null)
//            return null;

//        Drawing2D.GraphicsPath gfpSplines = new Drawing2D.GraphicsPath();

//        gfpSplines.AddCurve(ptfPoints);
//        gfpSplines.Flatten();

//        PointF[] ptfRetPoints = BezierApproximationToPointF(gfpSplines.PathPoints, intSmoothness, intPrecision, false);

//        return ptfRetPoints;
//    }

//    public static Point2D[] Create_ClosedCardinalByControlPoints(Point2D[] ptdPts)
//    {
//        if (ptdPts == null)
//            return null;
//        Point2D[] ptdRet;
//        int intRetCtr = 0;

//        if (ptdPts.Length < 2)
//            return null;
//        else if (ptdPts.Length == 2)
//        {
//            ptdRet = new Point2D[4];

//            ptdRet[0] = ptdPts[0];
//            ptdRet[1] = ptdPts[0];
//            ptdRet[2] = ptdPts[1];
//            ptdRet[3] = ptdPts[1];

//            return ptdRet;
//        }

//        float tension = 0.5;
//        float control_scale;
//        Point2D pt;
//        Point2D pt_before;
//        Point2D pt_after;
//        Point2D pt_after2;
//        Point2D Di;
//        Point2D DiPlus1;
//        Point2D p1, p2, p3, p4;
//        int intPts = ptdPts.Length;

//        control_scale = System.Convert.ToSingle(tension / 0.5 * 0.175);

//        ptdRet = new Point2D[(intPts - 1) * 3 + 3 + 1];

//        ptdRet[0] = ptdPts[0];

//        for (int intIdx = 0; intIdx <= Information.UBound(ptdPts); intIdx++)
//        {
//            pt_before = ptdPts[(intIdx - 1 + intPts) % intPts];
//            pt = ptdPts[intIdx];
//            pt_after = ptdPts[(intIdx + 1) % intPts];
//            pt_after2 = ptdPts[(intIdx + 2) % intPts];

//            p1 = pt;
//            p4 = pt_after;

//            Di.X = pt_after.X - pt_before.X;
//            Di.Y = pt_after.Y - pt_before.Y;
//            p2.X = pt.X + control_scale * Di.X;
//            p2.Y = pt.Y + control_scale * Di.Y;

//            DiPlus1.X = pt_after2.X - ptdPts[intIdx].X;
//            DiPlus1.Y = pt_after2.Y - ptdPts[intIdx].Y;
//            p3.X = pt_after.X - control_scale * DiPlus1.X;
//            p3.Y = pt_after.Y - control_scale * DiPlus1.Y;

//            // ptdRet(intRetCtr) = p1
//            ptdRet[intRetCtr + 1] = p2;
//            ptdRet[intRetCtr + 2] = p3;
//            ptdRet[intRetCtr + 3] = p4;

//            intRetCtr += 3;
//        }

//        return ptdRet;
//    }

//    public static Point2D[] Create_CardinalByControlPoints(Point2D[] ptdPts)
//    {
//        if (ptdPts == null)
//            return null;
//        Point2D[] ptdRet;
//        int intRetCtr = 0;

//        if (ptdPts.Length < 2)
//            return null;
//        else if (ptdPts.Length == 2)
//        {
//            ptdRet = new Point2D[4];

//            ptdRet[0] = ptdPts[0];
//            ptdRet[1] = ptdPts[0];
//            ptdRet[2] = ptdPts[1];
//            ptdRet[3] = ptdPts[1];

//            return ptdRet;
//        }

//        float tension = 0.5;
//        float control_scale;
//        Point2D pt;
//        Point2D pt_before;
//        Point2D pt_after;
//        Point2D pt_after2;
//        Point2D Di;
//        Point2D DiPlus1;
//        Point2D p1, p2, p3, p4;
//        int intPts = ptdPts.Length;

//        control_scale = System.Convert.ToSingle(tension / 0.5 * 0.175);

//        ptdRet = new Point2D[(intPts - 1) * 3 + 1];

//        ptdRet[0] = ptdPts[0];

//        for (int intIdx = 0; intIdx <= intPts - 2; intIdx++)
//        {
//            pt_before = ptdPts[Math.Max(intIdx - 1, 0)];
//            pt = ptdPts[intIdx];
//            pt_after = ptdPts[intIdx + 1];
//            pt_after2 = ptdPts[Math.Min(intIdx + 2, Information.UBound(ptdPts))];

//            p1 = ptdPts[intIdx];
//            p4 = ptdPts[intIdx + 1];

//            Di.X = pt_after.X - pt_before.X;
//            Di.Y = pt_after.Y - pt_before.Y;
//            p2.X = pt.X + control_scale * Di.X;
//            p2.Y = pt.Y + control_scale * Di.Y;

//            DiPlus1.X = pt_after2.X - ptdPts[intIdx].X;
//            DiPlus1.Y = pt_after2.Y - ptdPts[intIdx].Y;
//            p3.X = pt_after.X - control_scale * DiPlus1.X;
//            p3.Y = pt_after.Y - control_scale * DiPlus1.Y;

//            ptdRet[intRetCtr + 1] = p2;
//            ptdRet[intRetCtr + 2] = p3;
//            ptdRet[intRetCtr + 3] = p4;

//            intRetCtr += 3;
//        }

//        return ptdRet;
//    }

//    // Returns 2 Point2D
//    public static Point2D[] Get_CrossingLine2D(Point2D ptdP1, Point2D ptdP2, double dblRadius)
//    {
//        Point2D[] ptdCross = new Point2D[2]; // (0) = "Left" side     (1) = "Right" side
//        double dblAngle = mDL2DLib.GetAngleDegrees2D(ptdP1.X, ptdP1.Y, ptdP2.X, ptdP2.Y);

//        ptdCross[0].X = dblRadius;
//        ptdCross[1].X = dblRadius;

//        RotateByDegrees2D(ref ptdCross[0], dblAngle + 90);
//        RotateByDegrees2D(ref ptdCross[1], dblAngle - 90);
//        ptdCross[0].X += ptdP2.X;
//        ptdCross[0].Y += ptdP2.Y;
//        ptdCross[1].X += ptdP2.X;
//        ptdCross[1].Y += ptdP2.Y;

//        return ptdCross;
//    }

//    public static double CalculatePolygonArea2D(PointF[] ptfPts)
//    {
//        if (ptfPts == null)
//            return -1;
//        double dblArea = 0;

//        for (int intIdx = 1; intIdx <= ptfPts.Length - 1; intIdx++)
//        {
//            if (intIdx == 0)
//                dblArea += (ptfPts[intIdx].X - ptfPts[ptfPts.Length - 1].X) * (ptfPts[intIdx].Y + ptfPts[ptfPts.Length - 1].Y) / (double)2;
//            else
//                dblArea += (ptfPts[intIdx].X - ptfPts[intIdx - 1].X) * (ptfPts[intIdx].Y + ptfPts[intIdx - 1].Y) / (double)2;
//        }

//        return Math.Abs(dblArea);
//    }

//    // E.g. http://www.mathopenref.com/coordpolygonarea2.html
//    // Take area from line segment to X-axis, as traverse the polygon.
//    // On side where X's are increasing, is adding the area.
//    // On side where X's are decreasing, is subtracting the area.
//    // The area between polygon and axis is adding and subtracted, so cancels out,
//    // leaving the area of the polygon as the result.
//    // Abs is needed because order of points might yield negative of area.
//    public static double CalculatePolygonArea2D(IList<Point2D> pts)
//    {
//        if (pts == null)
//            return 0;

//        double area = 0;
//        Point2D prevPt = LastElement(pts);

//        foreach (Point2D pt in pts)
//        {
//            area += (pt.X - prevPt.X) * (pt.Y + prevPt.Y) / 2;
//            // Prep Next
//            prevPt = pt;
//        }

//        return Math.Abs(area);
//    }

//    public static double CalculatePolygonArea2D(Point3D[] ptdPts)
//    {
//        if (ptdPts == null)
//            return -1;
//        double dblArea = 0;

//        for (int intIdx = 1; intIdx <= ptdPts.Length - 1; intIdx++)
//        {
//            if (intIdx == 0)
//                dblArea += (ptdPts[intIdx].X - ptdPts[ptdPts.Length - 1].X) * (ptdPts[intIdx].Y + ptdPts[ptdPts.Length - 1].Y) / (double)2;
//            else
//                dblArea += (ptdPts[intIdx].X - ptdPts[intIdx - 1].X) * (ptdPts[intIdx].Y + ptdPts[intIdx - 1].Y) / (double)2;
//        }

//        return Math.Abs(dblArea);
//    }

//    // When "autoClose", adds distance between last and first point.
//    // (For our polygons, those are usually the same location, so autoClose is not needed.)
//    public static double Perimeter(IList<Point2D> pts, bool autoClose = false)
//    {
//        double result = 0;
//        // "- 2" (rather than "- 1") because calculation uses "i + 1".
//        for (int i = 0; i <= pts.Count - 2; i++)
//        {
//            Point2D p1 = pts[i];
//            Point2D p2 = pts[i + 1];
//            double segmentLength = Distance2D(p1, p2);
//            result += segmentLength;
//        }

//        if (autoClose)
//        {
//            double segmentLength = Distance2D(LastElement(pts), FirstElement(pts));
//            result += segmentLength;
//        }

//        return result;
//    }


//    public static Point2D[] ExtendPolygon2D(Point2D[] ptdPolyPts, double dblRange) // Created By Kimpa 070227
//    {
//        if (ptdPolyPts == null)
//            return null;
//        if (ptdPolyPts.Length < 2)
//            return null;

//        double tmpV1, tmpV2;
//        Point2D tmpP1, tmpP2;
//        Point2D[] ptd = null;
//        double tmpVadd1, tmpVadd2;

//        tmpVadd1 = 0; tmpVadd2 = 0;

//        if (!ptdPolyPts[0].X == ptdPolyPts[ptdPolyPts.Length - 1].X | !ptdPolyPts[0].Y == ptdPolyPts[ptdPolyPts.Length - 1].Y)
//        {
//            var oldPtdPolyPts = ptdPolyPts;
//            ptdPolyPts = new Point2D[ptdPolyPts.Length + 1];
//            if (oldPtdPolyPts != null)
//                Array.Copy(oldPtdPolyPts, ptdPolyPts, Math.Min(ptdPolyPts.Length + 1, oldPtdPolyPts.Length));
//            ptdPolyPts[ptdPolyPts.Length - 1] = ptdPolyPts[0];
//        }

//        for (int intIdx = 1; intIdx <= ptdPolyPts.Length - 1; intIdx++)
//        {
//            // Angel P1<->P2 and P2<->P1
//            double v1 = mDL2DLib.GetAngleDegrees2D(ptdPolyPts[intIdx - 1], ptdPolyPts[intIdx]);
//            double v2 = mDL2DLib.GetAngleDegrees2D(ptdPolyPts[intIdx], ptdPolyPts[intIdx - 1]);

//            // Calculate Outside/inside
//            Point2D CheckD1 = mDL2DLib.NewPosByDistanceAndAngle2D(dblRange >= 0 ? Degrees0_360(v1 + 270) : Degrees0_360(v1 + 90), ptdPolyPts[intIdx - 1], Math.Abs(dblRange));
//            Point2D CheckD2 = mDL2DLib.NewPosByDistanceAndAngle2D(dblRange >= 0 ? Degrees0_360(v2 - 270) : Degrees0_360(v2 - 90), ptdPolyPts[intIdx], Math.Abs(dblRange));
//            if (dblRange >= 0)
//            {
//                if (mDL2DLib.PointInPolygon2D(ptdPolyPts, CheckD1) == true | mDL2DLib.PointInPolygon2D(ptdPolyPts, CheckD2) == true)
//                {
//                    tmpVadd1 = v1 + 90;
//                    tmpVadd2 = v2 - 90;
//                }
//                else
//                {
//                    tmpVadd1 = v1 + 270;
//                    tmpVadd2 = v2 - 270;
//                }
//            }
//            else if (dblRange < 0)
//            {
//                if (mDL2DLib.PointInPolygon2D(ptdPolyPts, CheckD1) == false | mDL2DLib.PointInPolygon2D(ptdPolyPts, CheckD2) == false)
//                {
//                    tmpVadd1 = v1 + 270;
//                    tmpVadd2 = v2 - 270;
//                }
//                else
//                {
//                    tmpVadd1 = v1 + 90;
//                    tmpVadd2 = v2 - 90;
//                }
//            }

//            // Calculate Angle
//            v1 = Degrees0_360(tmpVadd1);    // v1 = IIf(dblRange >= 0, m2DLib.DegreeCorrection(v1 + 270), m2DLib.DegreeCorrection(v1 + 90))
//            v2 = Degrees0_360(tmpVadd2);    // v2 = IIf(dblRange >= 0, m2DLib.DegreeCorrection(v2 - 270), m2DLib.DegreeCorrection(v2 - 90))


//            // Calculate New Position
//            Point2D d1 = mDL2DLib.NewPosByDistanceAndAngle2D(v1, ptdPolyPts[intIdx - 1], Math.Abs(dblRange));
//            Point2D d2 = mDL2DLib.NewPosByDistanceAndAngle2D(v2, ptdPolyPts[intIdx], Math.Abs(dblRange));

//            // Add New Position to 2d Array
//            if (ptd == null)
//            {
//                ptd = new Point2D[1];
//                ptd[0] = d1;
//            }
//            else if (intIdx != ptdPolyPts.Length - 1)
//            {
//                if (mDL2DLib.LinesIntersects2D(d1, d2, tmpP1, tmpP2) == true)
//                {
//                    var oldPtd = ptd;
//                    ptd = new Point2D[ptd.Length + 1];
//                    if (oldPtd != null)
//                        Array.Copy(oldPtd, ptd, Math.Min(ptd.Length + 1, oldPtd.Length));
//                    ptd[ptd.Length - 1] = mDL2DLib.LinesIntersectsAt2D(d1, d2, tmpP1, tmpP2);
//                }
//                else
//                {
//                    var oldPtd = ptd;
//                    ptd = new Point2D[ptd.Length + 1];
//                    if (oldPtd != null)
//                        Array.Copy(oldPtd, ptd, Math.Min(ptd.Length + 1, oldPtd.Length));
//                    ptd[ptd.Length - 1] = tmpP2;
//                    var oldPtd = ptd;
//                    ptd = new Point2D[ptd.Length + 1];
//                    if (oldPtd != null)
//                        Array.Copy(oldPtd, ptd, Math.Min(ptd.Length + 1, oldPtd.Length));
//                    ptd[ptd.Length - 1] = d1;
//                }
//            }
//            else if (intIdx == ptdPolyPts.Length - 1)
//            {
//                if (ptd.Length >= 2)
//                {
//                    if (mDL2DLib.LinesIntersects2D(d1, d2, ptd[0], ptd[1]))
//                    {
//                        var oldPtd = ptd;
//                        ptd = new Point2D[ptd.Length + 1];
//                        if (oldPtd != null)
//                            Array.Copy(oldPtd, ptd, Math.Min(ptd.Length + 1, oldPtd.Length));
//                        ptd[ptd.Length - 1] = ptd[0];
//                    }
//                }
//            }
//            tmpP1 = d1;
//            tmpP2 = d2;
//            tmpV1 = v1;
//            tmpV2 = v2;
//        }

//        return ptd;
//    }

//    public static int AnglePerpendicularZone(double intAngleDegree)
//    {
//        // Zones: 1:0-90,2:91-180,3:181-270,4:271-359
//        if (intAngleDegree >= 0 && intAngleDegree <= 90)
//            return 1;
//        if (intAngleDegree > 90 && intAngleDegree <= 180)
//            return 2;
//        if (intAngleDegree > 180 && intAngleDegree <= 270)
//            return 3;
//        if (intAngleDegree > 270 && intAngleDegree < 360)
//            return 4;

//        return 0;
//    }

//    public static Point2D[] MeargePolygons2D(System.Drawing.Graphics gfx, Point2D[] ptdPolyPts1, Point2D[] ptdPolyPts2) // Created By Kimpa 070221
//    {
//        if (ptdPolyPts1 == null)
//            return null;
//        if (ptdPolyPts2 == null)
//            return null;

//        Point2D p1, p2, p3, p4, pMid;
//        double xR, xL, yU, yD, dblLength, dblStep;
//        double dblR = 0;
//        xR = 0; xL = 0; yU = 0; yD = 0;

//        // Close the Polypts1 and PolyPts2
//        if (ptdPolyPts1[0].X != ptdPolyPts1[ptdPolyPts1.Length - 1].X | ptdPolyPts1[0].Y == ptdPolyPts1[ptdPolyPts1.Length - 1].Y)
//        {
//            var oldPtdPolyPts1 = ptdPolyPts1;
//            ptdPolyPts1 = new Point2D[ptdPolyPts1.Length + 1];
//            if (oldPtdPolyPts1 != null)
//                Array.Copy(oldPtdPolyPts1, ptdPolyPts1, Math.Min(ptdPolyPts1.Length + 1, oldPtdPolyPts1.Length));
//            ptdPolyPts1[ptdPolyPts1.Length - 1] = ptdPolyPts1[0];
//        }
//        if (ptdPolyPts2[0].X != ptdPolyPts2[ptdPolyPts2.Length - 1].X | ptdPolyPts2[0].Y == ptdPolyPts2[ptdPolyPts2.Length - 1].Y)
//        {
//            var oldPtdPolyPts2 = ptdPolyPts2;
//            ptdPolyPts2 = new Point2D[ptdPolyPts2.Length + 1];
//            if (oldPtdPolyPts2 != null)
//                Array.Copy(oldPtdPolyPts2, ptdPolyPts2, Math.Min(ptdPolyPts2.Length + 1, oldPtdPolyPts2.Length));
//            ptdPolyPts2[ptdPolyPts2.Length - 1] = ptdPolyPts2[0];
//        }

//        // Calculate a big Rectangle around p1,p2
//        for (int intIdx = 0; intIdx <= ptdPolyPts1.Length - 1; intIdx++)
//        {
//            if (intIdx == 0)
//            {
//                xR = ptdPolyPts1[intIdx].X;
//                xL = ptdPolyPts1[intIdx].X;
//                yU = ptdPolyPts1[intIdx].Y;
//                yD = ptdPolyPts1[intIdx].Y;
//            }

//            if (ptdPolyPts1[intIdx].X > xR)
//                xR = ptdPolyPts1[intIdx].X;
//            if (ptdPolyPts1[intIdx].X < xL)
//                xL = ptdPolyPts1[intIdx].X;
//            if (ptdPolyPts1[intIdx].Y > yD)
//                yD = ptdPolyPts1[intIdx].Y;
//            if (ptdPolyPts1[intIdx].Y < yU)
//                yU = ptdPolyPts1[intIdx].Y;
//        }
//        for (int intIdx = 0; intIdx <= ptdPolyPts2.Length - 1; intIdx++)
//        {
//            if (ptdPolyPts2[intIdx].X > xR)
//                xR = ptdPolyPts2[intIdx].X;
//            if (ptdPolyPts2[intIdx].X < xL)
//                xL = ptdPolyPts2[intIdx].X;
//            if (ptdPolyPts2[intIdx].Y > yD)
//                yD = ptdPolyPts2[intIdx].Y;
//            if (ptdPolyPts2[intIdx].Y < yU)
//                yU = ptdPolyPts2[intIdx].Y;
//        }

//        p1.X = xL; p1.Y = yU;
//        p2.X = xR; p2.Y = yU;
//        p3.X = xR; p3.Y = yD;
//        p4.X = xL; p4.Y = yD;

//        // Diameter
//        if ((Math.Abs(xL - xR) / 2) >= (Math.Abs(yU - yD) / 2))
//            dblR = (Math.Abs(xL - xR) / 2) * 2;
//        if ((Math.Abs(xL - xR) / 2) < (Math.Abs(yU - yD) / 2))
//            dblR = (Math.Abs(yU - yD) / 2) * 2;

//        // Holds the Rectangles
//        Point2D[] ptdRec1 = new Point2D[4];
//        Point2D[] ptdRec2 = new Point2D[4];

//        // Calculate Rectangle Around ptdPolypts1
//        for (int intIdx = 0; intIdx <= ptdPolyPts1.Length - 1; intIdx++)
//        {
//            if (intIdx == 0)
//            {
//                xR = ptdPolyPts1[intIdx].X;
//                xL = ptdPolyPts1[intIdx].X;
//                yU = ptdPolyPts1[intIdx].Y;
//                yD = ptdPolyPts1[intIdx].Y;
//            }

//            if (ptdPolyPts1[intIdx].X > xR)
//                xR = ptdPolyPts1[intIdx].X;
//            if (ptdPolyPts1[intIdx].X < xL)
//                xL = ptdPolyPts1[intIdx].X;
//            if (ptdPolyPts1[intIdx].Y > yD)
//                yD = ptdPolyPts1[intIdx].Y;
//            if (ptdPolyPts1[intIdx].Y < yU)
//                yU = ptdPolyPts1[intIdx].Y;
//        }

//        ptdRec1[0] = new Point2D(xL, yU); ptdRec1[1] = new Point2D(xR, yU);
//        ptdRec1[2] = new Point2D(xR, yD); ptdRec1[3] = new Point2D(xL, yD);

//        // Calculate Rectangle around ptdPolypts2
//        for (int intIdx = 0; intIdx <= ptdPolyPts2.Length - 1; intIdx++)
//        {
//            if (intIdx == 0)
//            {
//                xR = ptdPolyPts2[intIdx].X;
//                xL = ptdPolyPts2[intIdx].X;
//                yU = ptdPolyPts2[intIdx].Y;
//                yD = ptdPolyPts2[intIdx].Y;
//            }

//            if (ptdPolyPts2[intIdx].X > xR)
//                xR = ptdPolyPts2[intIdx].X;
//            if (ptdPolyPts2[intIdx].X < xL)
//                xL = ptdPolyPts2[intIdx].X;
//            if (ptdPolyPts2[intIdx].Y > yD)
//                yD = ptdPolyPts2[intIdx].Y;
//            if (ptdPolyPts2[intIdx].Y < yU)
//                yU = ptdPolyPts2[intIdx].Y;
//        }

//        ptdRec2[0] = new Point2D(xL, yU); ptdRec2[1] = new Point2D(xR, yU);
//        ptdRec2[2] = new Point2D(xR, yD); ptdRec2[3] = new Point2D(xL, yD);
//        Point2D ptdCross1 = new Point2D(0, 0);
//        Point2D ptdCross2 = new Point2D(0, 0);

//        double tmpD = 0;
//        for (int intIdx = 1; intIdx <= ptdPolyPts1.Length - 1; intIdx++)
//        {
//            if (mDL2DLib.LinesIntersects2D(new Point2D(Math.Abs(ptdRec2[1].X - ptdRec2[0].X) / 2 + ptdRec2[0].X, Math.Abs(ptdRec2[3].Y - ptdRec2[0].Y) / 2 + ptdRec2[0].Y), new Point2D(Math.Abs(ptdRec1[1].X - ptdRec1[0].X) / 2 + ptdRec1[0].X, Math.Abs(ptdRec1[3].Y - ptdRec1[0].Y) / 2 + ptdRec1[0].Y), ptdPolyPts1[intIdx - 1], ptdPolyPts1[intIdx]))
//                ptdCross1 = mDL2DLib.LinesIntersectsAt2D(new Point2D(Math.Abs(ptdRec2[1].X - ptdRec2[0].X) / 2 + ptdRec2[0].X, Math.Abs(ptdRec2[3].Y - ptdRec2[0].Y) / 2 + ptdRec2[0].Y), new Point2D(Math.Abs(ptdRec1[1].X - ptdRec1[0].X) / 2 + ptdRec1[0].X, Math.Abs(ptdRec1[3].Y - ptdRec1[0].Y) / 2 + ptdRec1[0].Y), ptdPolyPts1[intIdx - 1], ptdPolyPts1[intIdx]);
//        }
//        tmpD = 0;
//        for (int intIdx = 1; intIdx <= ptdPolyPts2.Length - 1; intIdx++)
//        {
//            if (mDL2DLib.LinesIntersects2D(new Point2D(Math.Abs(ptdRec2[1].X - ptdRec2[0].X) / 2 + ptdRec2[0].X, Math.Abs(ptdRec2[3].Y - ptdRec2[0].Y) / 2 + ptdRec2[0].Y), new Point2D(Math.Abs(ptdRec1[1].X - ptdRec1[0].X) / 2 + ptdRec1[0].X, Math.Abs(ptdRec1[3].Y - ptdRec1[0].Y) / 2 + ptdRec1[0].Y), ptdPolyPts2[intIdx - 1], ptdPolyPts2[intIdx]))
//                ptdCross2 = mDL2DLib.LinesIntersectsAt2D(new Point2D(Math.Abs(ptdRec2[1].X - ptdRec2[0].X) / 2 + ptdRec2[0].X, Math.Abs(ptdRec2[3].Y - ptdRec2[0].Y) / 2 + ptdRec2[0].Y), new Point2D(Math.Abs(ptdRec1[1].X - ptdRec1[0].X) / 2 + ptdRec1[0].X, Math.Abs(ptdRec1[3].Y - ptdRec1[0].Y) / 2 + ptdRec1[0].Y), ptdPolyPts2[intIdx - 1], ptdPolyPts2[intIdx]);
//        }

//        if (ptdCross1.X == 0 & ptdCross1.Y == 0)
//            ptdCross1 = new Point2D(Math.Abs(ptdRec1[1].X - ptdRec1[0].X) / 2 + ptdRec1[0].X, Math.Abs(ptdRec1[3].Y - ptdRec1[0].Y) / 2 + ptdRec1[0].Y);
//        if (ptdCross2.X == 0 & ptdCross2.Y == 0)
//            ptdCross2 = new Point2D(Math.Abs(ptdRec2[1].X - ptdRec2[0].X) / 2 + ptdRec2[0].X, Math.Abs(ptdRec2[3].Y - ptdRec2[0].Y) / 2 + ptdRec2[0].Y);


//        // Midpoint in Rectangle
//        pMid.X = Math.Abs(ptdCross1.X - ptdCross2.X) / 2 + ptdCross1.X > ptdCross2.X ? ptdCross2.X : ptdCross1.X; // ((Math.Abs(xL - xR)) / 2) + xL 
//        pMid.Y = Math.Abs(ptdCross1.Y - ptdCross2.Y) / 2 + ptdCross1.Y > ptdCross2.Y ? ptdCross2.Y : ptdCross1.Y; // ((Math.Abs(yU - yD)) / 2) + yU

//        gfx.DrawEllipse(Pens.Blue, System.Convert.ToSingle(ptdCross1.X) - 2, System.Convert.ToSingle(ptdCross1.Y) - 2, 4, 4);
//        gfx.DrawEllipse(Pens.Red, System.Convert.ToSingle(ptdCross2.X) - 2, System.Convert.ToSingle(ptdCross2.Y) - 2, 4, 4);
//        gfx.DrawEllipse(Pens.Black, System.Convert.ToSingle(pMid.X) - 2, System.Convert.ToSingle(pMid.Y) - 2, 4, 4);
//        dblLength = (ptdPolyPts1.Length + ptdPolyPts2.Length);
//        dblStep = 1; // 360 / dblLength

//        // Calculate Merge Points
//        double tmpDistance = 0;
//        Point2D[] ptdMerge2d = null;
//        int intCount = 0;
//        double Angle = 0;
//        Point2D tmpP2;

//        while (!Angle > 360)
//        {
//            Angle = Angle + dblStep;
//            tmpP2 = new Point2D(0, 0);

//            // Backup Code with New lineIntersect..
//            // For i As Integer = 1 To ptdPolyPts1.Length - 1
//            // If m2DLib.LinesIntersects2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1(i - 1), ptdPolyPts1(i)) Then
//            // If m2DLib.Distance2D(pMid, m2DLib.LinesIntersectsAt2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1(i - 1), ptdPolyPts1(i))) > tmpDistance Then
//            // tmpDistance = m2DLib.Distance2D(pMid, m2DLib.LinesIntersectsAt2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1(i - 1), ptdPolyPts1(i)))
//            // tmpP2 = m2DLib.LinesIntersectsAt2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1(i - 1), ptdPolyPts1(i))
//            // End If
//            // End If
//            // Next
//            // For j As Integer = 1 To ptdPolyPts2.Length - 1
//            // If m2DLib.LinesIntersects2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2(j - 1), ptdPolyPts2(j)) Then
//            // If m2DLib.Distance2D(pMid, m2DLib.LinesIntersectsAt2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2(j - 1), ptdPolyPts2(j))) > tmpDistance Then
//            // tmpDistance = m2DLib.Distance2D(pMid, m2DLib.LinesIntersectsAt2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2(j - 1), ptdPolyPts2(j)))
//            // tmpP2 = m2DLib.LinesIntersectsAt2D(pMid, m2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2(j - 1), ptdPolyPts2(j)) 
//            // End If
//            // End If
//            // Next

//            // Caculate New Points
//            for (int i = 1; i <= ptdPolyPts1.Length - 1; i++)
//            {
//                if (mDL2DLib.IS_OldLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1[i - 1], ptdPolyPts1[i]))
//                {
//                    if (mDL2DLib.Distance2D(pMid, mDL2DLib.OLDLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1[i - 1], ptdPolyPts1[i])) > tmpDistance)
//                    {
//                        tmpDistance = mDL2DLib.Distance2D(pMid, mDL2DLib.OLDLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1[i - 1], ptdPolyPts1[i]));
//                        tmpP2 = mDL2DLib.OLDLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts1[i - 1], ptdPolyPts1[i]);
//                    }
//                }
//            }

//            // Caculate New Points
//            for (int j = 1; j <= ptdPolyPts2.Length - 1; j++)
//            {
//                if (mDL2DLib.IS_OldLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2[j - 1], ptdPolyPts2[j]) == true)
//                {
//                    if (mDL2DLib.Distance2D(pMid, mDL2DLib.OLDLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2[j - 1], ptdPolyPts2[j])) > tmpDistance)
//                    {
//                        tmpDistance = mDL2DLib.Distance2D(pMid, mDL2DLib.OLDLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2[j - 1], ptdPolyPts2[j]));
//                        tmpP2 = mDL2DLib.OLDLineIntersect2D(pMid, mDL2DLib.NewPosByDistanceAndAngle2D(Angle, pMid, dblR), ptdPolyPts2[j - 1], ptdPolyPts2[j]);
//                    }
//                }
//            }

//            tmpDistance = 0;
//            if (tmpP2.X > 0 | tmpP2.X < 0 | tmpP2.Y > 0 | tmpP2.Y < 0)
//            {
//                if (ptdMerge2d == null)
//                {
//                    ptdMerge2d = new Point2D[intCount + 1];
//                    ptdMerge2d[intCount] = tmpP2;
//                    intCount = intCount + 1;
//                }
//                else
//                {
//                    var oldPtdMerge2d = ptdMerge2d;
//                    ptdMerge2d = new Point2D[intCount + 1];
//                    if (oldPtdMerge2d != null)
//                        Array.Copy(oldPtdMerge2d, ptdMerge2d, Math.Min(intCount + 1, oldPtdMerge2d.Length));
//                    ptdMerge2d[intCount] = tmpP2;
//                    intCount = intCount + 1;
//                }
//            }
//        }

//        return ptdMerge2d;
//    }

//    // Wraps at 360 degrees, so result is in [0..360).
//    // TBD: Same as Wrap360.
//    public static double Degrees0_360(double dblDeg) // Created By Kimpa 070221
//    {
//        if (dblDeg > 360)
//            return dblDeg % 360;

//        if (dblDeg < 0)
//            // "360 +" moves from negative to positive, because Mod of negative returns a negative.
//            return 360 + (dblDeg % 360);

//        return dblDeg;
//    }
//    public static double OppsiteDegree(double dblDeg)  // Created By Kimpa 070221
//    {
//        return dblDeg >= 180 ? dblDeg - 180 : dblDeg + 180;
//    }
//    public static Point2D NewPosByDistanceAndAngle2D(double dblDeg, Point2D ptdPointOrigo2d, double Distance) // Created By Kimpa 070221
//    {
//        return new Point2D(ptdPointOrigo2d.X + (Math.Cos((Math.PI / 180) * dblDeg) * Distance), ptdPointOrigo2d.Y + (Math.Sin((Math.PI / 180) * dblDeg) * Distance));
//    }
//    private static double[] bubble_sort(double[] dblIndex) // Created By Kimpa 070221
//    {
//        if (dblIndex == null)
//            return null;

//        for (int i = 0; i <= dblIndex.Length - 1; i++)
//        {
//            for (int j = 0; j <= dblIndex.Length - 2; j++)
//            {
//                if (dblIndex[j] > dblIndex[j + 1])
//                    Swap(dblIndex[j], dblIndex[j + 1]);
//            }
//        }

//        return dblIndex;
//    }

//    public static Point2D OLDLineIntersect2D(Point2D p1n, Point2D p2n, Point2D p1m, Point2D p2m) // Created By Kim
//    {
//        return mDL2DLib.OLDLineIntersect2D(p1n.X, p1n.Y, p2n.X, p2n.Y, p1m.X, p1m.Y, p2m.X, p2m.Y);
//    }
//    public static bool IS_OldLineIntersect2D(Point2D p1n, Point2D p2n, Point2D p1m, Point2D p2m) // Created By Kim
//    {
//        return mDL2DLib.IS_OldLineIntersect2D(p1n.X, p1n.Y, p2n.X, p2n.Y, p1m.X, p1m.Y, p2m.X, p2m.Y);
//    }

//    public static Point2D OLDLineIntersect2D(double dblStartFirstPointX, double dblStartFirstPointY, double dblEndFirstPointX, double dblEndFirstPointY, double dblStartSecondPointX, double dblStartSecondPointY, double dblEndSecondPointX, double dblEndSecondPointY, ref double dblx = 0, ref double dbly = 0) // Created By Kim
//    {
//        // Can handle better LineIntersect
//        double b1, b2, a1, a2, xi, yi, x1hi, x1lo, y1hi, y1lo, ax, bx, ay, by, cx, cy, d, f, e;

//        ax = dblEndFirstPointX - dblStartFirstPointX;
//        bx = dblStartSecondPointX - dblEndSecondPointX;

//        if (ax < 0)
//        {
//            x1lo = dblEndFirstPointX; x1hi = dblStartFirstPointX;
//        }
//        else
//        {
//            x1hi = dblEndFirstPointX; x1lo = dblStartFirstPointX;
//        }
//        if (bx > 0)
//        {
//            if (x1hi < dblEndSecondPointX | dblStartSecondPointX == x1lo)
//                return default(Point2D);
//        }
//        else if (x1hi < dblStartSecondPointX | dblEndSecondPointX < x1lo)
//            return default(Point2D);

//        ay = dblEndFirstPointY - dblStartFirstPointY;
//        by = dblStartSecondPointY - dblEndSecondPointY;
//        if (ay < 0)
//        {
//            y1lo = dblEndFirstPointY; y1hi = dblStartFirstPointY;
//        }
//        else
//        {
//            y1hi = dblEndFirstPointY; y1lo = dblStartFirstPointY;
//        }

//        if (by > 0)
//        {
//            if (y1hi < dblEndSecondPointY | dblStartSecondPointY < y1lo)
//                return default(Point2D);
//        }
//        else if (y1hi < dblStartSecondPointY | dblEndSecondPointY < y1lo)
//            return default(Point2D);

//        cx = dblStartFirstPointX - dblStartSecondPointX;
//        cy = dblStartFirstPointY - dblStartSecondPointY;

//        d = by * cx - bx * cy;
//        f = ay * bx - ax * by;

//        if (f > 0)
//        {
//            if ((d < 0 | d > f))
//                return default(Point2D);
//        }
//        else if ((d > 0 | d < f))
//            return default(Point2D);

//        e = ax * cy - ay * cx;
//        if (f > 0)
//        {
//            if ((e < 0 | e > f))
//                return default(Point2D);
//        }
//        else if ((e > 0 | e < f))
//            return default(Point2D);
//        if (f == 0)
//            return default(Point2D);

//        // Calculate Where it intersect
//        if (!(dblStartFirstPointX - dblEndFirstPointX) == 0)
//            b1 = (dblStartFirstPointY - dblEndFirstPointY) / (dblStartFirstPointX - dblEndFirstPointX);
//        else
//            b1 = (dblStartFirstPointY - dblEndFirstPointY) / (dblStartFirstPointX - (dblEndFirstPointX + 0.000001));
//        if (!(dblStartSecondPointX - dblEndSecondPointX) == 0)
//            b2 = (dblStartSecondPointY - dblEndSecondPointY) / (dblStartSecondPointX - dblEndSecondPointX);
//        else
//            b2 = (dblStartSecondPointY - dblEndSecondPointY) / (dblStartSecondPointX - (dblEndSecondPointX + 0.000001));

//        a1 = dblStartFirstPointY - (b1 * dblStartFirstPointX);
//        a2 = dblStartSecondPointY - (b2 * dblStartSecondPointX);
//        xi = (-(a1 - a2)) / (b1 - b2);
//        yi = a1 + b1 * xi;

//        double q1, q2, q3, q4;

//        q1 = (dblStartFirstPointX - xi) * (xi - dblEndFirstPointX);
//        q2 = (dblStartSecondPointX - xi) * (xi - dblEndSecondPointX);
//        q3 = (dblStartFirstPointY - yi) * (yi - dblEndFirstPointY);
//        q4 = (dblStartSecondPointY - yi) * (yi - dblEndSecondPointY);

//        q1 = Math.Round(q1, 3);
//        q2 = Math.Round(q2, 3);
//        q3 = Math.Round(q3, 3);
//        q4 = Math.Round(q4, 3);

//        if (q1 >= 0 & q2 >= 0 & q3 >= 0 & q4 >= 0)
//        {
//            dblx = xi;
//            dbly = yi;
//            return new Point2D(dblx, dbly);
//        }
//        else
//            return default(Point2D);

//        return default(Point2D);
//    }

//    public static bool IS_OldLineIntersect2D(double dblStartFirstPointX, double dblStartFirstPointY, double dblEndFirstPointX, double dblEndFirstPointY, double dblStartSecondPointX, double dblStartSecondPointY, double dblEndSecondPointX, double dblEndSecondPointY, ref double dblx = 0, ref double dbly = 0) // Created By Kim
//    {
//        // Can handle better LineIntersect
//        double b1, b2, a1, a2, xi, yi, x1hi, x1lo, y1hi, y1lo, ax, bx, ay, by, cx, cy, d, f, e;

//        ax = dblEndFirstPointX - dblStartFirstPointX;
//        bx = dblStartSecondPointX - dblEndSecondPointX;

//        if (ax < 0)
//        {
//            x1lo = dblEndFirstPointX; x1hi = dblStartFirstPointX;
//        }
//        else
//        {
//            x1hi = dblEndFirstPointX; x1lo = dblStartFirstPointX;
//        }
//        if (bx > 0)
//        {
//            if (x1hi < dblEndSecondPointX | dblStartSecondPointX == x1lo)
//                return false;
//        }
//        else if (x1hi < dblStartSecondPointX | dblEndSecondPointX < x1lo)
//            return false;

//        ay = dblEndFirstPointY - dblStartFirstPointY;
//        by = dblStartSecondPointY - dblEndSecondPointY;
//        if (ay < 0)
//        {
//            y1lo = dblEndFirstPointY; y1hi = dblStartFirstPointY;
//        }
//        else
//        {
//            y1hi = dblEndFirstPointY; y1lo = dblStartFirstPointY;
//        }

//        if (by > 0)
//        {
//            if (y1hi < dblEndSecondPointY | dblStartSecondPointY < y1lo)
//                return false;
//        }
//        else if (y1hi < dblStartSecondPointY | dblEndSecondPointY < y1lo)
//            return false;

//        cx = dblStartFirstPointX - dblStartSecondPointX;
//        cy = dblStartFirstPointY - dblStartSecondPointY;

//        d = by * cx - bx * cy;
//        f = ay * bx - ax * by;

//        if (f > 0)
//        {
//            if ((d < 0 | d > f))
//                return false;
//        }
//        else if ((d > 0 | d < f))
//            return false;

//        e = ax * cy - ay * cx;
//        if (f > 0)
//        {
//            if ((e < 0 | e > f))
//                return false;
//        }
//        else if ((e > 0 | e < f))
//            return false;
//        if (f == 0)
//            return false;

//        // Calculate Where it intersect
//        if (!(dblStartFirstPointX - dblEndFirstPointX) == 0)
//            b1 = (dblStartFirstPointY - dblEndFirstPointY) / (dblStartFirstPointX - dblEndFirstPointX);
//        else
//            b1 = (dblStartFirstPointY - dblEndFirstPointY) / (dblStartFirstPointX - (dblEndFirstPointX + 0.000001));
//        if (!(dblStartSecondPointX - dblEndSecondPointX) == 0)
//            b2 = (dblStartSecondPointY - dblEndSecondPointY) / (dblStartSecondPointX - dblEndSecondPointX);
//        else
//            b2 = (dblStartSecondPointY - dblEndSecondPointY) / (dblStartSecondPointX - (dblEndSecondPointX + 0.000001));

//        a1 = dblStartFirstPointY - (b1 * dblStartFirstPointX);
//        a2 = dblStartSecondPointY - (b2 * dblStartSecondPointX);
//        xi = (-(a1 - a2)) / (b1 - b2);
//        yi = a1 + b1 * xi;

//        double q1, q2, q3, q4;

//        q1 = (dblStartFirstPointX - xi) * (xi - dblEndFirstPointX);
//        q2 = (dblStartSecondPointX - xi) * (xi - dblEndSecondPointX);
//        q3 = (dblStartFirstPointY - yi) * (yi - dblEndFirstPointY);
//        q4 = (dblStartSecondPointY - yi) * (yi - dblEndSecondPointY);

//        q1 = Math.Round(q1, 3);
//        q2 = Math.Round(q2, 3);
//        q3 = Math.Round(q3, 3);
//        q4 = Math.Round(q4, 3);

//        if (q1 >= 0 & q2 >= 0 & q3 >= 0 & q4 >= 0)
//        {
//            dblx = xi;
//            dbly = yi;
//            return true;
//        }
//        else
//            return false;
//        // --------End Slow but very very good

//        return false;
//    }


//    // Use this when need to know if point is near boundary versus being a specified distance inside.
//    public enum InOut
//    {
//        Inside = 0,
//        Outside = 1,
//        Border = 2
//    }

//    // Span has A and B sides, which may differ in whether they are Inside or Outside the other polygon.
//    // MUST match InOutContinue values, for In/Out/BorderSame/BorderOpposite.
//    public enum InOutPair
//    {
//        Inside = 0,
//        Outside = 1,
//        BorderSameSide = 2  // Interiors of both shapes are on same side of span.
//,
//        BorderOpposite = 3  // Interiors of shapes are on opposite sides of span.
//,
//        AInBOut = 4    // A Inside, B Outside
//,
//        AOutBIn = 5    // A Outside, B Inside
//,
//        Border_TooClose = 6     // Border; Can't resolve interiors. TBD whether finer tolerance would fix, or cause other problems.
//    }

//    // Can pass in either InOutPair or InOutContinue.
//    public static bool SpanTypeIsBorder(InOutPair eInOutPair)
//    {
//        return (eInOutPair == InOutPair.BorderSameSide) || (eInOutPair == InOutPair.BorderOpposite) || (eInOutPair == InOutPair.Border_TooClose);
//    }

//    // Like InOutPair, but add possibility of "Continue"
//    // MUST match InOutPair values, for In/Out/BorderSame/BorderOpposite.
//    public enum InOutContinue
//    {
//        Inside = 0,
//        Outside = 1,
//        BorderSameSide = 2  // Interiors of both shapes are on same side of span.
//,
//        BorderOpposite = 3  // Interiors of shapes are on opposite sides of span.
//,
//        AInBOut = 4    // A Inside, B Outside
//,
//        AOutBIn = 5    // A Outside, B Inside
//,
//        Border_TooClose = 6     // Border; Can't resolve interiors. TBD whether finer tolerance would fix, or cause other problems.
//,
//        Continue,
//        Conflict
//    }

//    public static string Nick(object ob, int nChars)
//    {
//        string str = ob.ToString();
//        if (str.Length <= nChars)
//            return str;
//        return str.Remove(nChars);
//    }

//    // 6-letter Nickname (aka abbreviation).
//    public static string Nick(InOut eInOut)
//    {
//        return Nick(eInOut, 6);
//    }

//    // 1-letter Nickname.
//    public static char Nick1(InOut eInOut)
//    {
//        return eInOut.ToString()[0];
//    }

//    // 4-letter Nickname (aka abbreviation).
//    public static string Nick(InOutPair eInOut)
//    {
//        switch (eInOut)
//        {
//            case InOutPair.AInBOut:
//                {
//                    return "AiBo";
//                }

//            case InOutPair.AOutBIn:
//                {
//                    return "AoBi";
//                }

//            default:
//                {
//                    return mDL2DLib.Nick(eInOut, 4);
//                }
//        }
//    }



//    public class EdgeSegmentIndexs
//    {
//        // Indexs into containing polygon's point list.
//        public readonly int Index1, Index2;
//        public EdgeSegmentIndexs(int index1, int index2)
//        {
//            this.Index1 = index1;
//            this.Index2 = index2;
//        }


//        public override string ToString()
//        {
//            return string.Format("EdgeSegment({0}, {1})", Index1, Index2);
//        }


//        public override bool Equals(object obj)
//        {
//            EdgeSegmentIndexs b = obj as EdgeSegmentIndexs;
//            if (b == null)
//                return false;

//            return ((Index1 == b.Index1) && (Index2 == b.Index2)) || ((Index1 == b.Index2) && (Index2 == b.Index1));
//        }

//        public override int GetHashCode()
//        {
//            // Use consistent ordering, because Equals is true if same, but in different order!
//            if (Index1 <= Index2)
//                return MakeHash(Index1, Index2);
//            else
//                return MakeHash(Index2, Index1);
//        }
//    }


//    public class EdgeSegmentPoints
//    {
//        // CAUTION: For scan-crossing, P2.Y is excluded from segment (does not quite reach P2.Y).
//        public readonly Point2D P1, P2;
//        public EdgeSegmentPoints(Point2D p1, Point2D p2)
//        {
//            this.P1 = p1;
//            this.P2 = p2;
//        }


//        public override string ToString()
//        {
//            return string.Format("{0} {1} {2}", this.GetType().Name, this.P1, this.P2);
//        }


//        // CAUTION: For scan-crossing, P2.Y is excluded from segment (does not quite reach P2.Y). That is not accounted for here.
//        public double MinY()
//        {
//            return Math.Min(this.P1.Y, this.P2.Y);
//        }

//        // CAUTION: For scan-crossing, P2.Y is excluded from segment (does not quite reach P2.Y). That is not accounted for here.
//        public double MaxY()
//        {
//            return Math.Max(this.P1.Y, this.P2.Y);
//        }


//        // NaN if does not intersect.
//        public double XAtY(double y)
//        {
//            double ix = P1.X;
//            double jx = P2.X;
//            double iy = P1.Y;
//            double jy = P2.Y;
//            // NOTE: Excludes P2.Y (jy).
//            if ((iy <= y && y < jy) || (jy < y && y <= iy))
//            {
//                // This segment spans the target y.

//                // x on segment, at this y.
//                double totalDy = jy - iy;
//                double dy = y - iy;
//                double totalDx = jx - ix;
//                double calcDx = dy * (totalDx / totalDy);
//                double calcX = ix + calcDx;
//                return calcX;
//            }

//            return double.NaN;
//        }
//    }


//    public class IntersectsAtY
//    {
//        public readonly double Y;
//        public readonly List<double> Xs = new List<double>();
//        public IntersectsAtY(double y)
//        {
//            this.Y = y;
//        }


//        public override string ToString()
//        {
//            string xsStr = string.Join(", ", (from x in this.Xs
//                                              select (Round4or6(x).ToString())).ToArray());
//            return string.Format("{0} @Y={1} Xs=({2})", this.GetType().Name, this.Y, xsStr);
//        }


//        // REQUIRE Xs.Count > 0.
//        public double FirstX()
//        {
//            return Xs[0];
//        }

//        // REQUIRE Xs.Count > 0.
//        public double LastX()
//        {
//            return LastElement(Xs);
//        }


//        public void AddX(double x)
//        {
//            Xs.Add(x);
//        }

//        public void SortXs()
//        {
//            this.Xs.Sort();
//        }
//    }

//    // A scan area bounded by left/right line segments.
//    public class ScanQuadrilateral
//    {
//        public readonly double MinY, MaxY;
//        public readonly double X1AtMinY, X2AtMinY, X1AtMaxY, X2AtMaxY;

//        public ScanQuadrilateral(double x1AtMinY, double x2AtMinY, double minY, double x1AtMaxY, double x2AtMaxY, double maxY)
//        {
//            this.X1AtMinY = x1AtMinY;
//            this.X2AtMinY = x2AtMinY;
//            this.MinY = minY;
//            this.X1AtMaxY = x1AtMaxY;
//            this.X2AtMaxY = x2AtMaxY;
//            this.MaxY = maxY;
//        }


//        public override string ToString()
//        {
//            return string.Format("{0} {7}x{8}={9}  ({1}..{2}@Y={3}) .. ({4}..{5}@Y={6})", this.GetType().Name, Round2(X1AtMinY), Round2(X2AtMinY), Round2(MinY), Round2(X1AtMaxY), Round2(X2AtMaxY), Round2(MaxY), Round2(AverageDx()), Round2(Dy()), Round2(Area()));
//        }


//        public double Area()
//        {
//            return AverageDx() * Dy();
//        }

//        public double AverageDx()
//        {
//            double dxAtMinY = X2AtMinY - X1AtMinY;
//            double dxAtMaxY = X2AtMaxY - X1AtMaxY;
//            return Average(dxAtMinY, dxAtMaxY);
//        }

//        public double Dy()
//        {
//            return MaxY - MinY;
//        }

//        public Point2D RandomLocation(Random rand = null)
//        {
//            if (rand == null)
//                rand = Random1;
//            double xWgt = rand.NextDouble();
//            double yWgt = rand.NextDouble();
//            double xAtY0 = Lerp(X1AtMinY, X2AtMinY, xWgt);
//            double xAtY1 = Lerp(X1AtMaxY, X2AtMaxY, xWgt);
//            Point2D pt = new Point2D(Lerp(xAtY0, xAtY1, yWgt), Lerp(MinY, MaxY, yWgt));
//            return pt;
//        }
//    }


//    public class ScanBin
//    {
//        // Return created bins; Out binStepY (y-extent of bin, or y-distance between bins).
//        public static ScanBin[] CreateBins(Point2D minV, Point2D maxV, int count, out double binStepY)
//        {
//            double deltaY = maxV.Y - minV.Y;
//            binStepY = deltaY / count;

//            ScanBin[] bins = new ScanBin[count - 1 + 1];
//            for (int i = 0; i <= count - 1; i++)
//                bins[i] = new ScanBin();

//            return bins;
//        }

//        public static ScanBin PickBin(ScanBin[] bins, double y, double minBinY, double binStepY)
//        {
//            if (double.IsNaN(y) || double.IsInfinity(y) || y == double.MaxValue)
//                return null; // No valid answer.

//            int count = bins.Length;
//            int iBin = RawBinIndex(y, minBinY, binStepY);
//            if ((iBin < 0) || (iBin >= count))
//                return null; // Outside of range of bins.

//            return bins[iBin];
//        }

//        // Return highBinIndex; set lowBinIndex. If highBinIndex < 0, there are no bins within range.
//        public static int BinIndexRange(ScanBin[] bins, double y1, double y2, double minBinY, double stepY, out int lowBinIndex)
//        {
//            int maxIndex = bins.Length - 1;
//            int iBinLow = RawBinIndex(y1, minBinY, stepY);
//            int iBinHigh = RawBinIndex(y2, minBinY, stepY);
//            if (iBinHigh < iBinLow)
//                Swap(iBinLow, iBinHigh);

//            if (iBinLow < 0)
//                iBinLow = 0;
//            if (iBinHigh > maxIndex)
//                iBinHigh = maxIndex;

//            if (iBinHigh < iBinLow)
//            {
//                lowBinIndex = -1;
//                return -1;
//            }

//            lowBinIndex = iBinLow;
//            return iBinHigh;
//        }

//        // May return value that is not a valid index.
//        public static int RawBinIndex(double y, double minBinY, double binStepY)
//        {
//            return FloorInt((y - minBinY) / binStepY);
//        }

//        public List<EdgeSegmentIndexs> Segments;

//        public ScanBin()
//        {
//            Segments = new List<EdgeSegmentIndexs>();
//        }

//        public void Dispose()
//        {
//            Segments.Clear();
//            Segments = null;
//        }

//        public void Add(EdgeSegmentIndexs seg)
//        {
//            Segments.Add(seg);
//        }
//    }


//    private const int Max1NScanBins = 64;
//    private const int Max2NScanBins = 512;
//    private const double MaxSmallTolerance = OneThousandth;


//    // Private ReadOnly t_history As New List(Of String)

//    // For efficient Point_InOut, when processing many points versus one polygon.
//    // Divides EdgeSegments of polygon into bins, by y.
//    // For each point check, only need to examine the segments in the bin containing point.y.
//    public class ScanBinnedPolygon
//    {
//        public readonly Point2D minV_tol, maxV_tol;   // Min/Max (including tolerance) in x & y.
//        public readonly double Tolerance;
//        // binStepY is y-distance between bins (or y-extent of a bin).
//        private readonly double toleranceSquared, binStepY;
//        // Sometimes we use a large tolerance, for detecting points w/i "tolerance" of poly edge.
//        // "border" means "within tolerance" of border, so when tolerance is large,
//        // this includes points outside of actual border (but within that tolerance),
//        // and also points INSIDE actual border (but within that tolerance of border).
//        // There are places where we should NOT use that tolerance, when large.
//        private readonly double smallToleranceSquared;
//        public double test_minDanger = double.MaxValue;
//        private readonly int ID;

//        public readonly int TrueLength;  // If duplicate point at end, exclude it.
//        // Don't rely on vertices.Count/LastIndex/LastElement (may be excluding final point).
//        // Don't assume this is a single sequence of points - when AddHole, those vertices are added at end.
//        // TBD: Would be easier to add hole if this were a List.
//        public IList<Point2D> Vertices;
//        // TBD: Can we rely on this being closed?
//        public IList<Point2D> OriginalVertices;

//        // Optional: Caller can set this, so can retrieve later.
//        // (Type is "Object", because here we don't have access to "cTGFWorldPolygon".)
//        public object OriginalPolygon;

//        public ScanBin[] scanBins;

//        public ScanBinnedPolygon(IList<Point2D> vertices, double tolerance, int id = 0)
//        {
//            this.OriginalVertices = vertices;
//            this.Vertices = new List<Point2D>(vertices);
//            bool didCopy = true;

//            this.Tolerance = tolerance;
//            this.toleranceSquared = tolerance * tolerance;
//            double smallTolerance = ClampMax(tolerance, MaxSmallTolerance);
//            this.smallToleranceSquared = smallTolerance * smallTolerance;
//            this.ID = id;
//            // t_history.Add(String.Format("ID={0} ({1})", Me.ID, Me.vertices.Count))
//            // If (t_history.Count Mod 5000) = 0 Then
//            // t_history.Sort()
//            // End If

//            // If duplicate point at end, exclude it.
//            // "While" in case there are multiple near-identical points.
//            int nRemoved = 0;
//            while (this.Vertices.Count > 1 && FirstElement(this.Vertices).X.NearlyEquals_ScaledTolerance(LastElement(this.Vertices).X, ScaledToleranceDouble) && FirstElement(this.Vertices).Y.NearlyEquals_ScaledTolerance(LastElement(this.Vertices).Y, ScaledToleranceDouble))
//            {
//                // Remove the last element (which is near-duplicate of first element).
//                if (nRemoved == 0)
//                {
//                    // First removal.
//                    if (!didCopy)
//                        this.Vertices = new List<Point2D>(vertices);
//                }
//                this.Vertices.RemoveAt(LastIndex(this.Vertices));
//                nRemoved += 1;
//            }
//            TrueLength = this.Vertices.Count;
//            if (nRemoved > 1)
//                Test();

//            minV_tol = Point2D.MaxValue;
//            maxV_tol = Point2D.MinValue;
//            for (int i = 0; i <= TrueLength - 1; i++)
//                AccumMinMax(vertices[i], ref minV_tol, ref maxV_tol);
//            // Include tolerance in x & y.
//            minV_tol.X -= tolerance; minV_tol.Y -= tolerance;
//            maxV_tol.X += tolerance; maxV_tol.Y += tolerance;

//            int nBins1 = Math.Min(System.Convert.ToInt32(Math.Ceiling(maxV_tol.Y - MinBinY)), Max1NScanBins);
//            int nBins2 = Math.Min(System.Convert.ToInt32(Math.Ceiling(2 * Math.Sqrt(vertices.Count))), Max2NScanBins);
//            int nScanBins = Math.Max(nBins1, nBins2);
//            if (nScanBins < 4)
//                nScanBins = 4;
//            scanBins = ScanBin.CreateBins(minV_tol, maxV_tol, nScanBins, out binStepY);

//            // Place each edgeSegment in all bins that it overlaps (or is within tolerance of).
//            for (int i = 0; i <= TrueLength - 1; i++)
//            {
//                // Segment starting at last point wraps back to [0].
//                int j = (i + 1) % TrueLength;
//                CreateEdgeSegmentAndAddToBins(i, j);
//            }
//        }

//        private void CreateEdgeSegmentAndAddToBins(int i, int j)
//        {
//            Point2D pointI = Vertices[i];
//            Point2D pointJ = Vertices[j];
//            // 
//            if (pointJ.Y < pointI.Y)
//                // Swap, so pointI has lower y.
//                Swap(pointI, pointJ);

//            double safeMinY = pointI.Y - Tolerance;
//            double safeMaxY = pointJ.Y + Tolerance;

//            int lowBinIndex;
//            int highBinIndex = ScanBin.BinIndexRange(scanBins, safeMinY, safeMaxY, MinBinY, binStepY, out lowBinIndex);
//            if (highBinIndex < 0)
//                return;

//            EdgeSegmentIndexs seg = new EdgeSegmentIndexs(i, j);
//            for (int iBin = lowBinIndex; iBin <= highBinIndex; iBin++)
//                scanBins[iBin].Add(seg);
//        }

//        // ASSUME hole is closed, and does not extend outside the originally provided vertices.
//        public void AddHole(Point2D[] holeVertices)
//        {
//            // If duplicate point at end, exclude it.
//            int holeVertTrueLen;
//            if (holeVertices[0].X == LastElement(holeVertices).X & holeVertices[0].Y == LastElement(holeVertices).Y)
//                holeVertTrueLen = holeVertices.Length - 1;
//            else
//                holeVertTrueLen = holeVertices.Length;

//            List<Point2D> allVerts = new List<Point2D>(this.Vertices);
//            int oldLen = this.Vertices.Count;
//            int newLen = oldLen + holeVertTrueLen;
//            allVerts.AddRange(holeVertices);
//            if (allVerts.Count > newLen)
//                allVerts.RemoveAt(LastIndex(allVerts));
//            // BEFORE "CreateEdgeSegmentAndAddToBins".
//            this.Vertices = allVerts.ToArray();

//            // Place each edgeSegment in all bins that it overlaps (or is within tolerance of).
//            for (int ri = 0; ri <= holeVertTrueLen - 1; ri++)
//            {
//                int rj = (ri + 1) % holeVertTrueLen;
//                // "oldLen + ": hole verts were appended after poly verts.
//                int i = oldLen + ri;
//                int j = oldLen + rj;
//                CreateEdgeSegmentAndAddToBins(i, j);
//            }
//            // Edge segment that closes the hole.
//            // "oldLen + ": hole verts were appended after poly verts.
//            CreateEdgeSegmentAndAddToBins(oldLen + holeVertTrueLen - 1, oldLen + 0);
//        }

//        public void Dispose()
//        {
//            for (int i = 0; i <= LastIndex(scanBins); i++)
//            {
//                scanBins[i].Dispose();
//                scanBins[i] = null;
//            }
//            scanBins = null;
//        }


//        public override string ToString()
//        {
//            return string.Format("ScanBinnedPolygon ID={0}, {1} points", this.ID, this.Vertices.Count);
//        }


//        // lower bound
//        public Point2D MinV2IncludingTolerance
//        {
//            get
//            {
//                return minV_tol;
//            }
//        }
//        // upper bound
//        public Point2D MaxV2IncludingTolerance
//        {
//            get
//            {
//                return maxV_tol;
//            }
//        }

//        public Rectangle2D MinMaxIncludingTolerance
//        {
//            get
//            {
//                return Rectangle2D.FromMinMax(minV_tol, maxV_tol);
//            }
//        }


//        private double MinBinY
//        {
//            get
//            {
//                return this.minV_tol.Y;
//            }
//        }
//        private int MaxValidBinIndex
//        {
//            get
//            {
//                return LastIndex(scanBins);
//            }
//        }

//        public bool Point_SafelyInside(Point2D point)
//        {
//            // Dim oInOut As InOut = Point_InOut(point)
//            // If oInOut = InOut.Border Then
//            // If (t_polId <> t_prev_polId) OrElse (t_polId2 <> t_prev_polId2) Then
//            // Dim dubious = 0
//            // t_prev_polId = t_polId : t_prev_polId2 = t_polId2
//            // End If
//            // End If
//            return (Point_InOut(point) == InOut.Inside);
//        }

//        // Use this when need to know if point is near boundary of polygon
//        // (segments between vertices) versus being a specified distance inside.
//        public InOut Point_InOut(Point2D point)
//        {
//            float segIndexAndFrac;
//            double minGap, minDistanceSq, proposeX;
//            return Point_InOut_Work(point, false, false, out segIndexAndFrac, out minDistanceSq, out minGap, out proposeX);
//        }

//        // tmstest
//        public InOut Point_InOut_Fake(Point2D point)
//        {
//            double x = point.X;
//            double y = point.Y;

//            if ((x < minV_tol.X) || (x > maxV_tol.X) || (y < MinBinY) || (y > maxV_tol.Y))
//                // test_nQuick += 1
//                return InOut.Outside;

//            if (Random1.NextDouble() < 0.1)
//                return InOut.Inside;
//            else
//                return InOut.Outside;
//        }

//        // ' NO - segIndexAndFrac only valid when InOut.Border.
//        // Public Function Point_InOut(point As Point2D, <Out()> ByRef closestPt As Point2D) As InOut
//        // Dim minGap, proposeX As Double, segIndexAndFrac As Single
//        // Dim result As InOut = Point_InOut(point, False, segIndexAndFrac, minGap, proposeX)

//        // closestPt = Me.SegIFAsPoint(segIndexAndFrac)
//        // Return result
//        // End Function

//        public InOut Point_InOut(Point2D point, out float segIndexAndFrac)
//        {
//            // NOTE: accurateNearBorder=True. Slower, but more accurate "nearest" point.
//            double minGap, minDistanceSq, proposeX;
//            return Point_InOut_Work(point, false, true, out segIndexAndFrac, out minDistanceSq, out minGap, out proposeX);
//        }

//        public InOut Point_InOut(Point2D point, bool extraCheck, out double minGap, out double proposeX)
//        {
//            float segIndexAndFrac;
//            double minDistanceSq;
//            return Point_InOut_Work(point, extraCheck, false, out segIndexAndFrac, out minDistanceSq, out minGap, out proposeX);
//        }

//        public List<EdgeSegmentIndexs> AllEdgeSegments()
//        {
//            return EdgeSegmentsInBinRange(0, LastIndex(scanBins));
//        }

//        public List<EdgeSegmentIndexs> EdgeSegmentsInRange(Point2D p1, Point2D p2)
//        {
//            double minX = Math.Min(p1.X, p2.X);
//            double minY = Math.Min(p1.Y, p2.Y);
//            double maxX = Math.Max(p1.X, p2.X);
//            double maxY = Math.Max(p1.Y, p2.Y);

//            if ((maxX < minV_tol.X) || (minX > maxV_tol.X) || (maxY < MinBinY) || (minY > maxV_tol.Y))
//                return new List<EdgeSegmentIndexs>(); // out of bounds => None.

//            if (minY < MinBinY)
//                minY = MinBinY;
//            int minBinI = ScanBin.RawBinIndex(minY, MinBinY, binStepY);

//            if (maxY > maxV_tol.Y)
//                maxY = maxV_tol.Y;
//            int maxBinI = ScanBin.RawBinIndex(maxY, MinBinY, binStepY);
//            if (maxBinI > LastIndex(scanBins))
//                maxBinI = LastIndex(scanBins);

//            return EdgeSegmentsInBinRange(minBinI, maxBinI);
//        }

//        private List<EdgeSegmentIndexs> EdgeSegmentsInBinRange(int minBinI, int maxBinI)
//        {
//            // HashSet: Segment may be in multiple bins; only want once.
//            HashSet<EdgeSegmentIndexs> segments = new HashSet<EdgeSegmentIndexs>();

//            for (int binIndex = minBinI; binIndex <= maxBinI; binIndex++)
//            {
//                ScanBin bin = scanBins[binIndex];
//                foreach (EdgeSegmentIndexs segment in bin.Segments)
//                    segments.Add(segment);
//            }

//            return segments.ToList();
//        }


//        public bool SegmentIntersects(Point2D p1, Point2D p2)
//        {
//            return SegmentIntersection(p1, p2).HasValue;
//        }

//        public Point2D? SegmentIntersection(Point2D p1, Point2D p2)
//        {
//            Point2D minP = p1.Min(p2);
//            Point2D maxP = p1.Max(p2);

//            List<EdgeSegmentIndexs> segs = EdgeSegmentsInRange(p1, p2);
//            foreach (EdgeSegmentIndexs seg in segs)
//            {
//                Point2D ps1 = this.Vertices[seg.Index1];
//                Point2D ps2 = this.Vertices[seg.Index2];
//                if (mDL2DLib.MinMaxIntersect(minP, maxP, ps1.Min(ps2), ps1.Max(ps2)))
//                {
//                    Point2D? isectPt = LinesIntersection2D(p1, p2, ps1, ps2);
//                    if (isectPt.HasValue)
//                        return isectPt;
//                }
//            }

//            return default(Point2D?);
//        }

//        public EdgeSegmentPoints AsPoints(EdgeSegmentIndexs segment)
//        {
//            return new EdgeSegmentPoints(Vertices[segment.Index1], Vertices[segment.Index2]);
//        }

//        public List<ScanQuadrilateral> AsQuads(out double minY, out double maxY, out double areaSum)
//        {
//            minY = double.MaxValue;
//            maxY = double.MinValue;

//            List<EdgeSegmentPoints> segmentPs = (from segment in AllEdgeSegments()
//                                                 select AsPoints(segment)).ToList();
//            // Sort by MinY.
//            segmentPs.Sort((a, b) =>
//            {
//                return a.MinY().CompareTo(b.MinY());
//            });

//            // TBD: Since we have sorted segmentPs, we could get minY from first segmentP.
//            foreach (EdgeSegmentPoints segment in segmentPs)
//            {
//                AccumMinMax(segment.P1.Y, ref minY, ref maxY);
//                AccumMinMax(segment.P2.Y, ref minY, ref maxY);
//            }

//            List<IntersectsAtY> isectss = new List<IntersectsAtY>();
//            double minY1 = minY;
//            List<EdgeSegmentPoints> activeSegments = new List<EdgeSegmentPoints>();
//            foreach (EdgeSegmentPoints segment in segmentPs)
//            {
//                double newMinY = segment.MinY();
//                if (newMinY > minY1)
//                {
//                    if (activeSegments.Count == 0)
//                        Dubious();
//                    minY1 = ProcessSegments(activeSegments, isectss, minY1, newMinY);
//                }
//                else
//                {
//                }
//                activeSegments.Add(segment);
//            }

//            // Build quads from intersections on scans.
//            List<ScanQuadrilateral> quads = new List<ScanQuadrilateral>();
//            areaSum = 0;
//            IntersectsAtY atY1 = null;
//            foreach (IntersectsAtY atY2 in isectss)
//            {
//                if (Exists(atY1))
//                {
//                    // ASSUME already sorted xs.
//                    // NOTE: LastX may equal FirstX. Result is a triangle.
//                    int ixLim = Math.Min(atY1.Xs.Count, atY2.Xs.Count);
//                    // TODO: If odd number, haven't kept info to know which goes with which.
//                    // Therefore, correct solution is to build a list of edges.
//                    // Alternatively, find intersections a hair away from the key ys,
//                    // so know where they were going/coming from.
//                    for (int ix = 0; ix <= ixLim - 1; ix += 2)
//                    {
//                        // "If..": if both ends have same X, skip the quad.
//                        if ((ix < LastIndex(atY1.Xs)) || (ix < LastIndex(atY2.Xs)))
//                        {
//                            double x1AtY1 = atY1.Xs[ix];
//                            double x2AtY1 = atY1.Xs[ClampMax(ix + 1, LastIndex(atY1.Xs))];
//                            double x1AtY2 = atY2.Xs[ix];
//                            double x2AtY2 = atY2.Xs[ClampMax(ix + 1, LastIndex(atY2.Xs))];
//                            ScanQuadrilateral quad = new ScanQuadrilateral(x1AtY1, x2AtY1, atY1.Y, x1AtY2, x2AtY2, atY2.Y);
//                            quads.Add(quad);
//                            areaSum += quad.Area();   // tmstest
//                        }
//                        else
//                            Test();
//                    }
//                }
//                // Prep Next
//                atY1 = atY2;
//            }

//            return quads;
//        }

//        // We've moved past minY, so see if any quads can be output.
//        // SIDE-EFFECT: Adds elements to "quads"
//        private double ProcessSegments(IList<EdgeSegmentPoints> activeSegments, List<IntersectsAtY> isectss, double minY, double newMinY)
//        {
//            List<EdgeSegmentPoints> toRemove = new List<EdgeSegmentPoints>();
//            while (minY < newMinY)
//            {
//                toRemove.Clear();
//                // Find segments that intersect minY; x per segment.
//                IntersectsAtY atY = new IntersectsAtY(minY);
//                double nextY = double.MaxValue;
//                foreach (EdgeSegmentPoints segment in activeSegments)
//                {
//                    // Find intersection at minY.
//                    double x = segment.XAtY(minY);
//                    if (!double.IsNaN(x))
//                        atY.AddX(x);
//                    else
//                        // Does not overlap. Ready to remove?
//                        if (segment.MaxY() < minY)
//                        toRemove.Add(segment);
//                    else
//                        // Happens if P2.Y = minY.  Maybe also happens if P1.Y = minY.
//                        // To be safe, leave segment active, until we are definitely past.
//                        Test();
//                    // Look for next smallest y.
//                    double minY0 = segment.MinY();
//                    if (minY0 > minY)
//                        AccumMin(minY0, ref nextY);
//                    double maxY0 = segment.MaxY();
//                    if (maxY0 > minY)
//                        AccumMin(maxY0, ref nextY);
//                }
//                // Found intersections?
//                if (atY.Xs.Count > 0)
//                {
//                    atY.SortXs();
//                    isectss.Add(atY);
//                }
//                else
//                    Dubious();
//                // Next minY.
//                minY = nextY;
//                // Remove any that no longer overlap.
//                if (toRemove.Count > 0)
//                {
//                    foreach (EdgeSegmentPoints segment in toRemove)
//                        activeSegments.Remove(segment);
//                }
//                if (minY < newMinY)
//                    // Does this ever happen?  Or is newMinY always the next y?
//                    Test();
//            }

//            return newMinY;
//        }

//        // Use this when need to know if point is near boundary of polygon
//        // (segments between vertices) versus being a specified distance inside.
//        // segIndexAndFrac only valid when InOut.Border.
//        // minGap and proposeX only valid when extraCheck=True.
//        private InOut Point_InOut_Work(Point2D point, bool extraCheck, bool accurateNearBorder, out float segIndexAndFrac, out double minDistanceSq, out double minGap, out double proposeX)
//        {
//            double x = point.X;
//            double y = point.Y;
//            segIndexAndFrac = -1;    // Undefined
//            minDistanceSq = double.MaxValue;
//            minGap = double.MaxValue; proposeX = double.NaN;   // Undefined

//            if ((x < minV_tol.X) || (x > maxV_tol.X) || (y < MinBinY) || (y > maxV_tol.Y))
//                // test_nQuick += 1
//                return InOut.Outside;

//            double borderToleranceSquared = accurateNearBorder ? smallToleranceSquared : toleranceSquared;
//            bool foundNearBorder = false;
//            // Only valid near border.
//            float closestSegIF = -1;
//            Point2D closestPt = Point2D.NaN();

//            // test_nSlow += 1
//            // The only relevant edges are in bin.
//            ScanBin bin = ScanBin.PickBin(scanBins, y, MinBinY, binStepY);
//            if (bin == null)
//                return InOut.Outside; // Should not happen, since we did bounds check above.

//            bool odd = false;
//            List<double> crossingXs = null;
//            if (extraCheck)
//                crossingXs = new List<double>();
//            foreach (EdgeSegmentIndexs seg in bin.Segments)
//            {
//                // test_nEdgeChecks += 1
//                Point2D p1 = this.Vertices[seg.Index1];
//                Point2D p2 = this.Vertices[seg.Index2];

//                // NearBoundary check.
//                double ix = p1.X;
//                double jx = p2.X;

//                // point may be near this span.
//                double safeMinX;
//                double safeMaxX;
//                if (ix <= jx)
//                {
//                    safeMinX = ix - Tolerance;
//                    safeMaxX = jx + Tolerance;
//                }
//                else
//                {
//                    safeMinX = jx - Tolerance;
//                    safeMaxX = ix + Tolerance;
//                }
//                if ((safeMinX <= x) && (x <= safeMaxX))
//                {
//                    // point may be near this span.
//                    double t;
//                    double distanceSq = PointDistanceSquaredToLine2D(point, p1, p2, ref t);
//                    if (distanceSq <= toleranceSquared)
//                    {
//                        // --- is "InOut.Border" ---
//                        // We could "Return" here, if we know caller doesn't care about segIndexAndFrac.

//                        // TBD: Is this still needed by any caller?
//                        // If within small-tolerance of an endpoint, return THAT. Sometimes avoids returning value slightly into next segment.
//                        // TBD: Now that "toleranceSquared <> smallToleranceSquared",
//                        // Should we instead test whether t Is near Then 0 Or 1?
//                        if (DistanceSquared2D(point, p1) < smallToleranceSquared)
//                            t = 0;
//                        else if (DistanceSquared2D(point, p2) < smallToleranceSquared)
//                            t = 1;

//                        // NOTE: This might not be the CLOSEST point - it is any point within tolerance.
//                        float segIndexAndFrac1;
//                        if (seg.Index2 == 0 || seg.Index2 == seg.Index1 + 1)
//                            segIndexAndFrac1 = System.Convert.ToSingle(seg.Index1 + t);
//                        else
//                            segIndexAndFrac1 = System.Convert.ToSingle(Lerp(seg.Index1, seg.Index2, t));

//                        if (!accurateNearBorder)
//                        {
//                            // We can return any "close enough" point.
//                            segIndexAndFrac = segIndexAndFrac1;
//                            return InOut.Border;
//                        }
//                        else
//                        {
//                            foundNearBorder = true;
//                            if (distanceSq < minDistanceSq)
//                            {
//                                // This is closest so far.
//                                minDistanceSq = distanceSq;
//                                segIndexAndFrac = segIndexAndFrac1;
//                            }
//                            // Continue to test remaining segments.
//                            continue;
//                        }
//                    }
//                    if (distanceSq < test_minDanger)
//                        test_minDanger = distanceSq;
//                }


//                // --- Current segment not near border ---

//                if (foundNearBorder)
//                    // Current segment is away from border, so we can ignore it (because we already have a better candidate).
//                    continue;


//                // odd/even counting to determine whether inside.
//                double iy = p1.Y;
//                double jy = p2.Y;
//                // TBD: Shouldn't this be consistent w.r.t. jy?  Always exclude "=" with jy, to exclude P2.Y?
//                if ((iy < y & jy >= y) | (jy < y & iy >= y))
//                {
//                    // This segment spans the target y.

//                    if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                        odd = !odd;

//                    if (extraCheck)
//                    {
//                        // x on segment, at this y.
//                        double totalDy = jy - iy;
//                        double dy = y - iy;
//                        double totalDx = jx - ix;
//                        double calcDx = dy * (totalDx / totalDy);
//                        double calcX = ix + calcDx;
//                        crossingXs.Add(calcX);
//                    }
//                }
//            }

//            if (extraCheck)
//            {
//                crossingXs.Sort();
//                // Find smallest gap.
//                // TODO: Is there ever zero gap, due to endpoint appearing twice? (Shouldn't be, if segment test done correctly)
//                minGap = double.MaxValue;
//                double priorX = double.MinValue;
//                proposeX = double.MinValue;
//                foreach (double crossingX in crossingXs)
//                {
//                    double gap = crossingX - priorX;
//                    if (gap < minGap)
//                    {
//                        minGap = gap;
//                        proposeX = Average(priorX, crossingX);
//                    }
//                    // Next
//                    priorX = crossingX;
//                }
//            }

//            if (foundNearBorder)
//                return InOut.Border;
//            else
//                return odd ? InOut.Inside : InOut.Outside;
//        }

//        public Point2D SegIFAsPoint(float segIndexAndFrac)
//        {
//            float segFrac;
//            int segIndex = SplitIntegerAndFraction(segIndexAndFrac, ref segFrac);

//            Point2D pt0 = this.Vertices[IndexWrap(segIndex)];
//            Point2D pt1 = this.Vertices[NextIndexWrap(segIndex)];
//            return Lerp(pt0, pt1, segFrac);
//        }

//        // REQUIRE: index >= 0.
//        public int IndexWrap(int index)
//        {
//            return (index % this.TrueLength);
//        }
//        public int NextIndexWrap(int index)
//        {
//            return ((index + 1) % this.TrueLength);
//        }
//        public int PriorIndexWrap(int index)
//        {
//            // "+ length" to avoid negative value.
//            return ((index - 1 + TrueLength) % this.TrueLength);
//        }

//        // Find closest other stuff to given pt on polygon (described as segIndexAndFrac).
//        // That is, exclude the segments(s) involved in this pt.
//        public double MinDistanceToSelf(float givenSegIndexAndFrac, ref float closestSegIndexAndFrac)
//        {
//            float givenSegFrac;
//            int givenSegIndex = SplitIntegerAndFraction(givenSegIndexAndFrac, ref givenSegFrac);
//            int skipIndexA = givenSegIndex;
//            int skipIndexB = NextIndexWrap(givenSegIndex);
//            int skipIndexZ;
//            skipIndexZ = PriorIndexWrap(skipIndexA); // Skip prior segment also

//            // If givenSegFrac <= 0.1 Then
//            // skipIndexZ = PriorIndexWrap(skipIndexA) ' Skip prior segment also
//            // ElseIf givenSegFrac >= 0.9 Then
//            // skipIndexZ = NextIndexWrap(skipIndexB)  ' Skip next segment also
//            // Else
//            // skipIndexZ = -1 ' (Will never match.)
//            // End If

//            Point2D givenPt = SegIFAsPoint(givenSegIndexAndFrac);

//            // Pick bin containing y, and the bins immediately above and below.
//            // NOTE: In rare circumstances, this may omit the closest segment -- but only when closest distance > MinBinY.
//            // Even if y is out of range, force at least one bin to be selected.
//            int iBin = ScanBin.RawBinIndex(givenPt.Y, MinBinY, binStepY);
//            int minBinI = Clamp(iBin - 1, 0, MaxValidBinIndex);
//            int maxBinI = Clamp(iBin + 1, 0, MaxValidBinIndex);
//            List<EdgeSegmentIndexs> segments = EdgeSegmentsInBinRange(minBinI, maxBinI);

//            // Find closest segment.
//            double minDistanceSq = double.MaxValue;
//            EdgeSegmentIndexs segmentAtMin = null;
//            double tAtMin;
//            foreach (EdgeSegmentIndexs segment in segments)
//            {
//                int segIndex = segment.Index1;
//                var doSkip = (segIndex == skipIndexA) || (segIndex == skipIndexB) || (segIndex == skipIndexZ);
//                if (!doSkip)
//                {
//                    double t;
//                    double distSq = PointDistanceSquaredToLine2D(givenPt, Vertices[segIndex], Vertices[segment.Index2], ref t);
//                    if (distSq < minDistanceSq)
//                    {
//                        minDistanceSq = distSq;
//                        segmentAtMin = segment;
//                        tAtMin = t;
//                    }
//                }
//            }

//            closestSegIndexAndFrac = System.Convert.ToSingle(segmentAtMin.Index1 + tAtMin);
//            return Math.Sqrt(minDistanceSq);
//        }

//        // Slow.
//        public double DistanceToBorder(Point2D pt, out InOut eInOut, out Point2D closestPt)
//        {
//            // Slow.
//            float segIndexAndFrac;
//            double minDistance = DistanceToBorder(pt, ref eInOut, ref segIndexAndFrac);

//            if (segIndexAndFrac < 0)
//                closestPt = new Point2D(0, 0);
//            else
//                closestPt = this.SegIFAsPoint(segIndexAndFrac);
//            // 
//            return minDistance;
//        }

//        // Slow (could speed up by checking only segments w/i 1 bin of pt). (When above/below all bins, would need to find closest non-empty bin.)
//        public double DistanceToBorder(Point2D pt, out InOut eInOut, out float segIndexAndFrac)
//        {
//            eInOut = Point_InOut(pt);

//            // Find shortest distance to a polygon segment.
//            // ASSUME polygon's last point = first point.
//            double minDistSq = double.MaxValue;
//            int iAtMinDist;
//            float tAtMinDist;
//            Point2D prevSegPt;
//            for (int i = 0; i <= LastIndex(Vertices); i++)
//            {
//                Point2D segPt = Vertices[i];
//                if (i > 0)
//                {
//                    // If i = 329 Then
//                    // Test()
//                    // End If
//                    double t;
//                    double distSq = PointDistanceSquaredToLine2D(pt, prevSegPt, segPt, ref t);
//                    if (distSq < minDistSq)
//                    {
//                        minDistSq = distSq;
//                        iAtMinDist = i;
//                        tAtMinDist = System.Convert.ToSingle(t);
//                    }
//                }

//                prevSegPt = segPt;
//            }

//            segIndexAndFrac = (iAtMinDist - 1) + tAtMinDist;

//            return Math.Sqrt(minDistSq);
//        }
//    }


//    // REQUIRE: Start point NOT duplicated at end of sequence.
//    public static Point2D IFAsPoint(float indexAndFrac, IList<Point2D> sequence)
//    {
//        float seqFrac;
//        int seqI = SplitIntegerAndFraction(indexAndFrac, ref seqFrac);

//        int nWrap = sequence.Count;
//        Point2D pt0 = sequence[WrappedIndex(seqI, nWrap)];
//        Point2D pt1 = sequence[WrappedIndex(seqI + 1, nWrap)];
//        return Lerp(pt0, pt1, seqFrac);
//    }


//    // Return Integer portion; Out resultFraction.
//    public static int SplitIntegerAndFraction(float integerAndFraction, out float resultFraction)
//    {
//        int resultInteger = FloorInt(integerAndFraction);

//        resultFraction = integerAndFraction - resultInteger;
//        return resultInteger;
//    }
//    // Return Integer portion; Out resultFraction.
//    public static int SplitIntegerAndFraction(double integerAndFraction, out double resultFraction)
//    {
//        int resultInteger = FloorInt(integerAndFraction);

//        resultFraction = integerAndFraction - resultInteger;
//        return resultInteger;
//    }

//    // Public t_polId, t_polId2 As Integer
//    // Private t_prev_polId, t_prev_polId2 As Integer

//    public static PointF PointF_Zero()
//    {
//        return new PointF(0, 0);
//    }

//    public static PointF PointF_MinValue()
//    {
//        return new PointF(float.MinValue, float.MinValue);
//    }

//    public static PointF PointF_MaxValue()
//    {
//        return new PointF(float.MaxValue, float.MaxValue);
//    }

//    // Before first call, caller must initialize minV to Integer.MaxValue, and maxV to Integer.MinValue.
//    public static void AccumMinMax(int v, ref int minV, ref int maxV)
//    {
//        if (v < minV)
//            minV = v;
//        if (v > maxV)
//            maxV = v;
//    }
//    // Before first call, caller must initialize minV to Single.MaxValue, and maxV to Single.MinValue.
//    public static void AccumMinMax(float v, ref float minV, ref float maxV)
//    {
//        if (v < minV)
//            minV = v;
//        if (v > maxV)
//            maxV = v;
//    }
//    // Before first call, caller must initialize minV to Double.MaxValue, and maxV to Double.MinValue.
//    public static void AccumMinMax(double v, ref double minV, ref double maxV)
//    {
//        if (v < minV)
//            minV = v;
//        if (v > maxV)
//            maxV = v;
//    }
//    // Before first call, caller must initialize minV to Double.MaxValue, and maxV to Double.MinValue.
//    public static void AccumMinMax(double v, int index, ref double minV, ref double maxV, ref int minI, ref int maxI)
//    {
//        if (v < minV)
//        {
//            minV = v; minI = index;
//        }
//        if (v > maxV)
//        {
//            maxV = v; maxI = index;
//        }
//    }

//    // Before first call,
//    // caller must initialize minV to New Vector2(Single.MaxValue),
//    // and maxV to New Vector2(Single.MinValue).
//    public static void AccumMinMax(Vector2 v, ref Vector2 minV, ref Vector2 maxV)
//    {
//        if (v.X < minV.X)
//            minV.X = v.X;
//        if (v.Y < minV.Y)
//            minV.Y = v.Y;

//        if (v.X > maxV.X)
//            maxV.X = v.X;
//        if (v.Y > maxV.Y)
//            maxV.Y = v.Y;
//    }

//    // Before first call,
//    // caller must initialize minV to Point2D.MaxValue,
//    // and maxV to Point2D.MinValue.
//    public static void AccumMinMax(Point2D v, ref Point2D minV, ref Point2D maxV)
//    {
//        if (v.X < minV.X)
//            minV.X = v.X;
//        if (v.Y < minV.Y)
//            minV.Y = v.Y;

//        if (v.X > maxV.X)
//            maxV.X = v.X;
//        if (v.Y > maxV.Y)
//            maxV.Y = v.Y;
//    }

//    // Before first call,
//    // caller must initialize minV to Point2D.MaxValue,
//    // and maxV to Point2D.MinValue.
//    public static void AccumMinMax(Rectangle2D rect, ref Point2D minPt, ref Point2D maxPt)
//    {
//        AccumMinMax(rect.TopLeft, ref minPt, ref maxPt);
//        AccumMinMax(rect.BottomRight, ref minPt, ref maxPt);
//    }

//    // Before first call, caller must initialize minPt and maxPt. E.g.:
//    // Dim minPt As PointF = PointF_MaxValue, maxPt As PointF = PointF_MinValue
//    public static void AccumMinMax(PointF v, ref PointF minPt, ref PointF maxPt)
//    {
//        if (v.X < minPt.X)
//            minPt.X = v.X;
//        if (v.Y < minPt.Y)
//            minPt.Y = v.Y;

//        if (v.X > maxPt.X)
//            maxPt.X = v.X;
//        if (v.Y > maxPt.Y)
//            maxPt.Y = v.Y;
//    }

//    // Before first call, caller must initialize minPt and maxPt. E.g.:
//    // Dim minPt As PointF = PointF_MaxValue, maxPt As PointF = PointF_MinValue
//    public static void AccumMinMax(RectangleF rect, ref PointF minPt, ref PointF maxV)
//    {
//        AccumMinMax(rect.Location, minPt, maxV);
//        AccumMinMax(new PointF(rect.Right, rect.Bottom), minPt, maxV);
//    }


//    // Before first call, caller must initialize:
//    // sum to zero, count to zero.
//    // minV to Double.MaxValue, and maxV to Double.MinValue.
//    public static void AccumSumMinMax(double v, ref double sum, ref double count, ref double minV, ref double maxV)
//    {
//        sum += v;
//        count += 1;
//        if (v < minV)
//            minV = v;
//        if (v > maxV)
//            maxV = v;
//    }


//    // Private test_nQuick, test_nSlow, test_nEdgeChecks As Integer

//    // Use this when need to know if point is near boundary versus being a specified distance inside.
//    // Slow - use only if don't know polygon bounding rectangle.
//    public static InOut Point_InOut_Polygon2D(Point2D[] polygon, Point2D point, double tolerance)
//    {
//        // NOTE: "Min/MaxValue / 2" in case it is unsafe to do tolerance math near to Min/MaxValues.
//        return Point_InOut_Polygon2D(polygon, point, tolerance, new Point2D(double.MinValue / 2, double.MinValue / 2), new Point2D(double.MaxValue / 2, double.MaxValue / 2));
//    }

//    // Use this when need to know if point is near boundary versus being a specified distance inside.
//    // See Class ScanBinnedPolygon for faster approach if testing many points against a polygon.
//    public static InOut Point_InOut_Polygon2D(Point2D[] polygon, Point2D point, double tolerance, Point2D minV, Point2D maxV)
//    {
//        if (polygon == null)
//            return InOut.Outside;

//        double x = point.X;
//        double y = point.Y;
//        if ((x < minV.X - tolerance) || (x > maxV.X + tolerance) || (y < minV.Y - tolerance) || (y > maxV.Y + tolerance))
//            // test_nQuick += 1
//            return InOut.Outside;

//        // test_nSlow += 1
//        double toleranceSquared = tolerance * tolerance;

//        int length;
//        if (polygon[0].X == LastElement(polygon).X & polygon[0].Y == LastElement(polygon).Y)
//            length = polygon.Length - 1;
//        else
//            length = polygon.Length;

//        bool odd = false;
//        for (int i = 0; i <= length - 1; i++)
//        {
//            int j = (i + 1) % length;
//            Point2D pointI = polygon[i];
//            Point2D pointJ = polygon[j];
//            double iy = pointI.Y;
//            double jy = pointJ.Y;

//            double safeMinY;
//            double safeMaxY;
//            if (iy <= jy)
//            {
//                safeMinY = iy - tolerance;
//                safeMaxY = jy + tolerance;
//            }
//            else
//            {
//                safeMinY = jy - tolerance;
//                safeMaxY = iy + tolerance;
//            }

//            if ((safeMinY <= y) && (y <= safeMaxY))
//            {
//                double ix = pointI.X;
//                double jx = pointJ.X;

//                // point may be near this span.
//                double safeMinX;
//                double safeMaxX;
//                if (ix <= jx)
//                {
//                    safeMinX = ix - tolerance;
//                    safeMaxX = jx + tolerance;
//                }
//                else
//                {
//                    safeMinX = jx - tolerance;
//                    safeMaxX = ix + tolerance;
//                }
//                if ((safeMinX <= x) && (x <= safeMaxX))
//                {
//                    // point may be near this span.
//                    if (PointDistanceSquaredToLine2D(point, pointI, pointJ) <= toleranceSquared)
//                        return InOut.Border;
//                }

//                // odd/even counting to determine whether inside.
//                if ((iy < y & jy >= y) | (jy < y & iy >= y))
//                {
//                    if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                        odd = !odd;
//                }
//            }
//        }

//        return odd ? InOut.Inside : InOut.Outside;
//    }


//    // p versus line (a, b).
//    // Returns -1, 0, or 1.
//    // "0" if on line, or if line has length zero.
//    // Formula is Z-component of cross-product.
//    // http://stackoverflow.com/questions/1560492/how-to-tell-whether-a-point-is-to-the-right-or-left-of-a-line/1560510#1560510
//    public static int PointWhichSideOfLine_A(Point2D p, Point2D a, Point2D b)
//    {
//        Point2D rb = b - a;
//        Point2D rp = p - a;
//        return Math.Sign((rb.X * rp.Y) - (rb.Y * rp.X));
//    }

//    // Returns -2, -1, 0, or 1. "0" if falls on line, "-1" if point's Y is less than line's Y for that X.
//    // "-2" if line has length zero, so cannot define the result.
//    // If line is near vertical, roles of X and Y are reversed.
//    public static int PointWhichSideOfLine(Point2D point, Point2D lineP1, Point2D lineP2)
//    {
//        Point2D deltaP2 = lineP2 - lineP1;
//        Point2D deltaP = point - lineP1;
//        if (Math.Abs(deltaP2.Y) > Math.Abs(deltaP2.X))
//        {
//            // Near vertical; swap roles of X and Y.
//            deltaP2 = new Point2D(deltaP2.Y, deltaP2.X);
//            deltaP = new Point2D(deltaP.Y, deltaP.X);
//        }

//        if (deltaP2.X == 0)
//            return -2; // Undefined

//        double lineDeltaYAtX = deltaP.X * (deltaP2.Y / deltaP2.X);
//        if (deltaP.Y > lineDeltaYAtX)
//            return 1;
//        if (deltaP.Y == lineDeltaYAtX)
//            return 0;
//        // <
//        return -1;
//    }

//    public static void Prep_WhichSideOfLine(Point2D lineP1, Point2D lineP2, out bool swapXY, out double slope)
//    {
//        Point2D deltaP2 = lineP2 - lineP1;

//        swapXY = Math.Abs(deltaP2.Y) > Math.Abs(deltaP2.X);
//        if (swapXY)
//            // Near vertical; swap roles of X and Y.
//            deltaP2 = new Point2D(deltaP2.Y, deltaP2.X);

//        if (deltaP2.X == 0)
//            slope = double.NaN;  // Undefined
//        else
//            slope = deltaP2.Y / deltaP2.X;
//    }

//    // The line is described by "lineP1, swapXY, slope".
//    public static int PointWhichSideOfLine_UsePrep(Point2D point, Point2D lineP1, bool swapXY, double slope)
//    {
//        if (slope == double.NaN)
//            return -2; // Undefined

//        Point2D deltaP = point - lineP1;
//        if (swapXY)
//            // Near vertical; swap roles of X and Y.
//            deltaP = new Point2D(deltaP.Y, deltaP.X);

//        double lineDeltaYAtX = deltaP.X * slope;
//        if (deltaP.Y > lineDeltaYAtX)
//            return 1;
//        if (deltaP.Y == lineDeltaYAtX)
//            return 0;
//        // <
//        return -1;
//    }

//    public static double XAtY(double goalY, Point2D lineP1, Point2D lineP2, bool allowExtend)
//    {
//        // Swap roles of X and Y.
//        return YAtX(goalY, lineP1.SwapXY(), lineP2.SwapXY(), allowExtend);
//    }

//    // Find y at goalX on line (lineP1, lineP2).
//    // "allowExtend": If allowed to go beyond ends of line.
//    // If no solution, returns Double.NaN.
//    public static double YAtX(double goalX, Point2D lineP1, Point2D lineP2, bool allowExtend)
//    {
//        double x1 = lineP1.X;
//        double x2 = lineP2.X;
//        double y1 = lineP1.Y;
//        double y2 = lineP2.Y;

//        double lineDx = x2 - x1;
//        double lineDy = y2 - y1;
//        bool xWithinLine = BetweenInclusive_WithTolerance(goalX, x1, x2, Math.Abs(lineDx * VerySmall));
//        if (!xWithinLine)
//        {
//            // If near an endpoint, return endpoint.
//            // When line is very short, return closest endpoint.
//            double error1 = Math.Abs(x1 - goalX);
//            double error2 = Math.Abs(x2 - goalX);
//            if (error1 <= error2)
//            {
//                // p1 is closer.
//                if (error1.NearlyEquals(0))
//                    return y1;
//            }
//            else
//                // p2 is closer.
//                if (error2.NearlyEquals(0))
//                return y2;
//            if (!allowExtend)
//                return double.NaN;
//            // Check for vertical line, which would make it impossible.
//            // TBD: If working with tiny numbers, need tolerance "lineDx * VerySmall".
//            // BUT if do that, may need additional logic to ensure don't get numeric overflow trying to reach the goal.
//            if (lineDx.NearlyEquals(0))
//                return double.NaN;
//        }

//        double goalDx = goalX - x1;

//        // Traditional formula based on slope (dy/dx).
//        return y1 + (lineDy / lineDx) * goalDx;
//    }


//    // True if ANY of "pts" is inside "polygon".
//    // PERFORMANCE: Recommend caller first check points against polygon's bounding rectangle.
//    public static bool PointsInPolygon2D(Point2D[] polygon, Point2D[] pts)
//    {
//        foreach (Point2D ptdPt in pts)
//        {
//            if (PointInPolygon2D(polygon, ptdPt))
//                return true;
//        }

//        return false;
//    }

//    // True if ANY of "pts" is inside "polygon".
//    // PERFORMANCE: Recommend caller first check against polygon's bounding rectangle.
//    public static bool PointsInPolygon2D(PointF[] polygon, PointF[] pts)
//    {
//        foreach (PointF ptfPt in pts)
//        {
//            if (PointInPolygon2D(polygon, ptfPt))
//                return true;
//        }

//        return false;
//    }


//    // PERFORMANCE: Recommend caller first check against polygon's bounding rectangle.
//    // CAUTION: IF pass in "polygon as points", the 4 corners must be CLOCKWISE not ZIGZAG order.
//    public static bool PointInPolygon2D(Point2D[] polygon, Point2D point)
//    {
//        if (polygon == null)
//            return false;

//        bool odd = false;
//        int length;
//        int i, j;
//        double ix, iy, jx, jy, x, y;


//        if (polygon[0].X == polygon[polygon.Length - 1].X & polygon[0].Y == polygon[polygon.Length - 1].Y)
//            length = polygon.Length - 1;
//        else
//            length = polygon.Length;

//        x = point.X;
//        y = point.Y;
//        for (i = 0; i <= length - 1; i++)
//        {
//            j = (i + 1) % length;
//            ix = polygon[i].X;
//            iy = polygon[i].Y;
//            jx = polygon[j].X;
//            jy = polygon[j].Y;
//            // if y-within-span
//            if ((iy < y && jy >= y) || (jy < y && iy >= y))
//            {
//                // if x-past-segment's-x-at-y
//                if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                    // point is past segment.
//                    odd = !odd;
//            }
//        }

//        return odd;
//    }

//    // PERFORMANCE: Recommend caller first check against polygon's bounding rectangle.
//    public static bool PointInPolygon2D(PointF[] polygon, PointF point)
//    {
//        if (polygon == null)
//            return false;

//        bool odd = false;
//        int length;
//        int i, j;
//        double ix, iy, jx, jy, x, y;

//        if (polygon[0].X == polygon[polygon.Length - 1].X & polygon[0].Y == polygon[polygon.Length - 1].Y)
//            length = polygon.Length - 1;
//        else
//            length = polygon.Length;

//        x = point.X;
//        y = point.Y;

//        for (i = 0; i <= length - 1; i++)
//        {
//            j = (i + 1) % length;
//            ix = polygon[i].X;
//            iy = polygon[i].Y;
//            jx = polygon[j].X;
//            jy = polygon[j].Y;
//            if ((iy < y & jy >= y) | (jy < y & iy >= y))
//            {
//                if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
//                    odd = !odd;
//            }
//        }
//        return odd;
//    }

//    public static bool CirclesIntersects2D(Point2D ptdP1, Point2D ptdP2, double dblR1, double dblR2)
//    {
//        double dblDI = Distance2D(ptdP1, ptdP2);

//        if (dblDI > dblR1 + dblR2)
//            return false;

//        return true;
//    }

//    public static Point2D[] CirclesIntersectsAt2D(Point2D ptdP1, Point2D ptdP2, double dblR1, double dblR2)
//    {
//        double dblDI = Distance2D(ptdP1, ptdP2);

//        if (dblDI > dblR1 + dblR2)
//            return null;
//        if (dblDI < Math.Abs(dblR1 - dblR2))
//            return null;

//        Point2D ptdDelta = new Point2D(ptdP2.X - ptdP1.X, ptdP2.Y - ptdP1.Y);
//        // dblDI = Math.Sqrt((ptdDelta.Y * ptdDelta.Y) + (ptdDelta.X * ptdDelta.X))
//        double a = ((dblR1 * dblR1) - (dblR2 * dblR2) + (dblDI * dblDI)) / (2 * dblDI);
//        Point2D ptdP3 = new Point2D(ptdP1.X + (ptdDelta.X * a / dblDI), ptdP1.Y + (ptdDelta.Y * a / dblDI));
//        double h = Math.Sqrt((dblR1 * dblR1) - (a * a));
//        Point2D ptdR = new Point2D(-ptdDelta.Y * (h / dblDI), ptdDelta.X * (h / dblDI));
//        Point2D[] ptdRet = new Point2D[2];

//        ptdRet[0].X = ptdP3.X + ptdR.X;
//        ptdRet[0].Y = ptdP3.Y + ptdR.Y;
//        ptdRet[1].X = ptdP3.X - ptdR.X;
//        ptdRet[1].Y = ptdP3.Y - ptdR.Y;

//        return ptdRet;
//    }

//    public static Point2D[] Merge_Linear_Shapes(Point2D[] ptdShape1, Point2D[] ptdShape2)
//    {
//        // Assumes shaper are 3rd degree beziers
//        Point2D[] ptdRet = null;

//        Point2D[] ptdNewShape1 = null;
//        int intIdxNewS1 = 0;
//        Point2D[] ptdNewShape2 = null;

//        for (int intIdxS1 = 0; intIdxS1 <= ptdShape1.Length - 1; intIdxS1++)
//        {
//            Point2D ptdS1P1;
//            Point2D ptdS1P2 = ptdShape1[intIdxS1];
//            Point2D[] ptdCrossPoints = null;

//            if (intIdxS1 == 0)
//                ptdS1P1 = ptdShape1[ptdShape1.Length - 1];
//            else
//                ptdS1P1 = ptdShape1[intIdxS1 - 1];

//            for (int intIdxS2 = 0; intIdxS2 <= ptdShape2.Length - 1; intIdxS2++)
//            {
//                Point2D ptdS2P1;
//                Point2D ptdS2P2 = ptdShape2[intIdxS2];

//                if (intIdxS2 == 0)
//                    ptdS2P1 = ptdShape1[ptdShape2.Length - 1];

//                if (LinesIntersects2D(ptdS1P1, ptdS1P2, ptdS2P1, ptdS2P2))
//                {
//                    // Korsar
//                    if (ptdCrossPoints == null)
//                        ptdCrossPoints = new Point2D[1];
//                    else
//                    {
//                        var oldPtdCrossPoints = ptdCrossPoints;
//                        ptdCrossPoints = new Point2D[ptdCrossPoints.Length + 1];
//                        if (oldPtdCrossPoints != null)
//                            Array.Copy(oldPtdCrossPoints, ptdCrossPoints, Math.Min(ptdCrossPoints.Length + 1, oldPtdCrossPoints.Length));
//                    }

//                    ptdCrossPoints[ptdCrossPoints.Length - 1] = LinesIntersectsAt2D(ptdS1P1, ptdS1P2, ptdS2P1, ptdS2P2);
//                }
//            }

//            int intBound = 0;

//            if (ptdNewShape1 == null)
//                intBound = 1;
//            else
//                intBound = ptdNewShape1.Length + 1;

//            if (!ptdCrossPoints == null)
//                intBound += ptdCrossPoints.Length;
//            var oldPtdNewShape1 = ptdNewShape1;
//            ptdNewShape1 = new Point2D[intBound + 1];
//            if (oldPtdNewShape1 != null)
//                Array.Copy(oldPtdNewShape1, ptdNewShape1, Math.Min(intBound + 1, oldPtdNewShape1.Length));

//            ptdNewShape1[intIdxNewS1] = ptdS1P1; intIdxNewS1 += 1;

//            if (!ptdCrossPoints == null)
//            {
//                for (int intIdx = 0; intIdx <= ptdCrossPoints.Length - 1; intIdx++)
//                {
//                }
//            }
//        }

//        return ptdRet;
//    }



//    public static Point3D[] Calculate_Convex_Hull(ref Point3D[] ptdVertices)
//    {
//        // The Convex Hull will be the current points (unless there are three co-linear points and OnBorder = False... we will ignore this case) 
//        if (ptdVertices.Length <= 3)
//            return (Point3D[])ptdVertices.Clone();

//        Point3D[] ptdRet = null;
//        int intIXNAStart = 0;
//        bool bolOnBorder = false;
//        // Find the starting index of the hull by finding the leftmost point in the polygon 

//        for (int intIXNA = 1; intIXNA <= ptdVertices.Length - 1; intIXNA++)
//        {
//            if (ptdVertices[intIXNA].X < ptdVertices[intIXNAStart].X)
//                // Point is Left Most 
//                intIXNAStart = intIXNA;
//            else if (ptdVertices[intIXNA].X == ptdVertices[intIXNAStart].X)
//            {
//                // Point is tied for left most, see if it's higher 
//                if (ptdVertices[intIXNA].Y < ptdVertices[intIXNAStart].Y)
//                    intIXNAStart = intIXNA;
//            }
//        }

//        bool[] bolInHull = new bool[ptdVertices.Length - 1 + 1];

//        // Sort until we get back to StartIndex 
//        int intIXNALast = intIXNAStart;

//        do
//        {
//            int intSelected = -1;
//            for (int intIXNA = 0; intIXNA <= ptdVertices.Length - 1; intIXNA++)
//            {
//                if (!bolInHull[intIXNA] & intIXNA != intIXNALast)
//                {
//                    if (intSelected == -1)
//                        // No point has been selected yet, select this one 
//                        intSelected = intIXNA;
//                    else
//                    {
//                        double dblCross = CrossProduct2D(ptdVertices[intIXNA], ptdVertices[intIXNALast], ptdVertices[intSelected]);
//                        if (dblCross == 0.0)
//                        {
//                            // On the line 
//                            if (bolOnBorder)
//                            {
//                                // Since we want the points on the border, take the one closer to LastIndex 
//                                if (DotProduct2D(ptdVertices[intIXNALast], ptdVertices[intIXNA], ptdVertices[intIXNA]) < DotProduct2D(ptdVertices[intIXNALast], ptdVertices[intSelected], ptdVertices[intSelected]))
//                                    intSelected = intIXNA;
//                            }
//                            else if (DotProduct2D(ptdVertices[intIXNALast], ptdVertices[intIXNA], ptdVertices[intIXNA]) > DotProduct2D(ptdVertices[intIXNALast], ptdVertices[intSelected], ptdVertices[intSelected]))
//                                intSelected = intIXNA;
//                        }
//                        else if (dblCross < 0.0)
//                            // ptdVertices(intIXNA) is more counter-clockwise 
//                            intSelected = intIXNA;
//                    }
//                }
//            }
//            // Set LastIndex to the final Selected point 
//            intIXNALast = intSelected;

//            // Update the InHull array to know this point has been added to the hull 
//            bolInHull[intIXNALast] = true;

//            // Add the point 
//            if (ptdRet == null)
//                ptdRet = new Point3D[1];
//            else
//            {
//                var oldPtdRet = ptdRet;
//                ptdRet = new Point3D[ptdRet.Length + 1];
//                if (oldPtdRet != null)
//                    Array.Copy(oldPtdRet, ptdRet, Math.Min(ptdRet.Length + 1, oldPtdRet.Length));
//            }

//            ptdRet[ptdRet.Length - 1] = ptdVertices[intIXNALast];
//        }
//        while (intIXNALast != intIXNAStart); // Check if we're back to the starting point 

//        return ptdRet;
//    }

//    public static double CrossProduct2D(Point2D Origin, Point2D A, Point2D B)
//    {
//        return (A.X - Origin.X) * (B.Y - Origin.Y) - (B.X - Origin.X) * (A.Y - Origin.Y);
//    }

//    // NOTE: See mDL3DLib for "CrossProduct3D".
//    public static double CrossProduct2D(Point3D Origin, Point3D A, Point3D B)
//    {
//        return (A.X - Origin.X) * (B.Y - Origin.Y) - (B.X - Origin.X) * (A.Y - Origin.Y);
//    }

//    public static double DotProduct2D(Point3D Origin, Point3D A, Point3D B)
//    {
//        return (A.X - Origin.X) * (B.X - Origin.X) + (A.Y - Origin.Y) * (B.Y - Origin.Y);
//    }



//    public static bool Is_Vertex_Inside_Triangle2D(Point3D ptdPt, Point3D ptdA, Point3D ptdB, Point3D ptdC)
//    {
//        Point3D e0, e1, e2;
//        double a1, a2, a3, b1, b2;
//        double det;
//        double c1, c2;

//        e0.X = ptdPt.X - ptdA.X;
//        e0.Y = ptdPt.Y - ptdA.Y;

//        e1.X = ptdB.X - ptdA.X;
//        e1.Y = ptdB.Y - ptdA.Y;

//        e2.X = ptdC.X - ptdA.X;
//        e2.Y = ptdC.Y - ptdA.Y;

//        a1 = e1.X * e1.X + e1.Y * e1.Y;
//        a2 = e1.X * e2.X + e1.Y * e2.Y;
//        a3 = e2.X * e2.X + e2.Y * e2.Y;

//        b1 = e0.X * e1.X + e0.Y * e1.Y;
//        b2 = e0.X * e2.X + e0.Y * e2.Y;

//        det = a1 * a3 - a2 * a2;

//        c1 = (b1 * a3 - b2 * a2) / det;
//        c2 = (b2 * a1 - b1 * a2) / det;

//        return (c1 >= 0.0) && (c1 <= 1.0) && (c2 >= 0) && (c2 <= 1);
//    }

//    public static double Calculate_ZValue_Inside_Triangle(Point3D ptdPt, Point3D ptdA, Point3D ptdB, Point3D ptdC)
//    {
//        Point3D v1, v2, n;
//        double d, e;

//        v1 = new Point3D(ptdB.X - ptdA.X, ptdB.Y - ptdA.Y, ptdB.Z - ptdA.Z); // v1 = ptdB - ptdA
//        v2 = new Point3D(ptdC.X - ptdA.X, ptdC.Y - ptdA.Y, ptdC.Z - ptdA.Z); // v2 = ptdC - ptdA

//        n.X = v1.Y * v2.Z - v1.Z * v2.Y;
//        n.Y = v1.Z * v2.X - v1.X * v2.Z;
//        n.Z = v1.X * v2.Y - v1.Y * v2.X;

//        d = n.X * ptdA.X + n.Y * ptdA.Y + n.Z * ptdA.Z;

//        ptdPt.Z = 0.0;
//        e = n.X * ptdPt.X + n.Y * ptdPt.Y + n.Z * 0;

//        ptdPt.Z = (d - e) / n.Z;

//        return ptdPt.Z;
//    }

//    public static bool PointInsideTriangle2D(double dblPTX, double dblPTY, double dblPTZ, double dblAX, double dblAY, double dblAZ, double dblBX, double dblBY, double dblBZ, double dblCX, double dblCY, double dblCZ)
//    {
//        double dble0X, dble0Y, dble0Z;
//        double dble1X, dble1Y, dble1Z;
//        double dble2X, dble2Y, dble2Z;
//        double a1, a2, a3, b1, b2;
//        double det;
//        double c1, c2;

//        dble0X = dblPTX - dblAX;
//        dble0Y = dblPTY - dblAY;
//        dble0Z = dblPTZ - dblAZ;

//        dble1X = dblBX - dblAX;
//        dble1Y = dblBY - dblAY;
//        dble1Z = dblBZ - dblAZ;

//        dble2X = dblCX - dblAX;
//        dble2Y = dblCY - dblAY;
//        dble2Z = dblCZ - dblAZ;

//        a1 = dble1X * dble1X + dble1Y * dble1Y + dble1Z * dble1Z;
//        a2 = dble1X * dble2X + dble1Y * dble2Y + dble1Z * dble2Z;
//        a3 = dble2X * dble2X + dble2Y * dble2Y + dble2Z * dble2Z;

//        b1 = dble0X * dble1X + dble0Y * dble1Y + dble0Z * dble1Z;
//        b2 = dble0X * dble2X + dble0Y * dble2Y + dble0Z * dble2Z;

//        det = a1 * a3 - a2 * a2;

//        c1 = (b1 * a3 - b2 * a2) / det;
//        c2 = (b2 * a1 - b1 * a2) / det;

//        return (c1 >= 0.0) && (c1 <= 1.0) && (c2 >= 0) && (c2 <= 1);
//    }


//    public static bool NewRectanglesIntersects2D(Rectangle2D rcdRect1, Rectangle2D rcdRect2, Point3D ptdOrigoVector)
//    {
//        Point2D[] ptdRect1 = new Point2D[4];
//        Point2D[] ptdRect2 = new Point2D[4];

//        // If ptdOrigoVector.X < 0 Then
//        // rcdRect1.X -= rcdRect1.Width
//        // rcdRect2.X -= rcdRect2.Width
//        // Else

//        // End If

//        if (ptdOrigoVector.Y < 0)
//        {
//        }
//        else
//        {
//            rcdRect1.Y -= rcdRect1.Height;
//            rcdRect2.Y -= rcdRect2.Height;
//        }

//        // Debug.WriteLine("Rectangle X0 Y0 W" & rcdRect1.Width & " H" & rcdRect1.Height)
//        // Debug.WriteLine("Point     X" & rcdRect2.X - rcdRect1.X & " Y" & rcdRect2.Y - rcdRect1.Y)

//        // Only Works if rcdRect1 all positiv numbers
//        ptdRect1[0].X = rcdRect1.X; ptdRect1[0].Y = rcdRect1.Y;
//        ptdRect1[1].X = rcdRect1.X + rcdRect1.Width; ptdRect1[1].Y = rcdRect1.Y;
//        ptdRect1[2].X = rcdRect1.X; ptdRect1[2].Y = rcdRect1.Y + rcdRect1.Height;
//        ptdRect1[3].X = rcdRect1.X + rcdRect1.Width; ptdRect1[3].Y = rcdRect1.Y + rcdRect1.Height;

//        ptdRect2[0].X = rcdRect2.X; ptdRect2[0].Y = rcdRect2.Y;
//        ptdRect2[1].X = rcdRect2.X + rcdRect2.Width; ptdRect2[1].Y = rcdRect2.Y;
//        ptdRect2[2].X = rcdRect2.X; ptdRect2[2].Y = rcdRect2.Y + rcdRect2.Height;
//        ptdRect2[3].X = rcdRect2.X + rcdRect2.Width; ptdRect2[3].Y = rcdRect2.Y + rcdRect2.Height;

//        for (int intIdx = 0; intIdx <= 3; intIdx++)
//        {
//            if (PointInsideRectangle2D_Strict(ptdRect1[intIdx], rcdRect2) || PointInsideRectangle2D_Strict(ptdRect2[intIdx], rcdRect1))
//                return true;
//        }

//        return false;
//    }

//    public static Rectangle2D Calculate_Rectangle(Point2D[] polygonAsPoints)
//    {
//        if (polygonAsPoints == null)
//            return default(Rectangle2D);

//        Point2D maxPt;
//        Point2D minPt = Calculate_MinMax(polygonAsPoints, ref maxPt);

//        return Rectangle2D.FromMinMax(minPt, maxPt);
//    }

//    public static Point2D Calculate_CenterOfRotatableRectangle(Point2D[] ptdPolygon)
//    {
//        Point2D pointSum = new Point2D(0, 0);
//        int pointCount = 0;
//        foreach (Point2D point in ptdPolygon)
//        {
//            pointSum += point;
//            pointCount += 1;
//        }
//        return (pointSum / (double)pointCount);
//    }


//    public static Point2D[] Calculate_MinMax(Point2D[] points)
//    {
//        Point2D maxPt;
//        Point2D minPt = Calculate_MinMax(points, ref maxPt);

//        if (maxPt.X < minPt.X)
//            return null;

//        Point2D[] retPoints = new Point2D[2];
//        retPoints[0] = minPt;
//        retPoints[1] = maxPt;
//        return retPoints;
//    }

//    public static MinMaxSingle Calculate_MinMax(IList<Vector3> vecs)
//    {
//    }

//    // Return minPt; (out) maxPt.
//    // If zero points are input, then outgoing max's are less then min's.
//    public static Point2D Calculate_MinMax(Point2D pt1, Point2D pt2, out Point2D maxPt)
//    {
//        Point2D minPt = pt1;
//        maxPt = pt1;

//        if (pt2.X < minPt.X)
//            minPt.X = pt2.X;
//        if (pt2.Y < minPt.Y)
//            minPt.Y = pt2.Y;

//        if (pt2.X > maxPt.X)
//            maxPt.X = pt2.X;
//        if (pt2.Y > maxPt.Y)
//            maxPt.Y = pt2.Y;

//        return minPt;
//    }

//    // Return minPt; (out) maxPt.
//    // If zero points are input, then outgoing max's are less then min's.
//    public static Point2D Calculate_MinMax(Point2D[] points, out Point2D maxPt)
//    {
//        Point2D minPt = Point2D.MaxValue;
//        maxPt = Point2D.MinValue;

//        // "no data" indicated by (minPt > maxPt).
//        if ((points == null) || (points.Length == 0))
//            return minPt;

//        for (int i = 0; i <= LastIndex(points); i++)
//            mDL2DLib.AccumMinMax(points[i], ref minPt, ref maxPt);

//        return minPt;
//    }


//    // Because WGS-84 unit is tiny, multiply tolerance by this.
//    private static double Unit_Scaled(Point2D[] pts)
//    {
//        Point2D minPt = Point2D.MaxValue;
//        Point2D maxPt = Point2D.MinValue;
//        for (int i = 0; i <= LastIndex(pts); i++)
//            mDL2DLib.AccumMinMax(pts[i], ref minPt, ref maxPt);

//        double shapeSize = Distance2D(minPt, maxPt);
//        // Don't have scale above 1 - only affect tolerance when coordinates are tiny.
//        double unitScale = Math.Min(1.0, shapeSize);

//        return unitScale;
//    }

//    // If fineDetail=True, then long sections retain more points.
//    public static Point2D[] Simplify_Polygon(Point2D[] pts, bool fineDetail)
//    {
//        return Simplify_Polygon2(pts, fineDetail ? 2.0 : 5.0, fineDetail ? 0.015 : 0.02);
//    }

//    // "EPS" is a measure of angle (change in direction).
//    // When "doSmooth", looks ahead for any future point within distance tolerance and EPS.
//    // (To be precise, find smallest EPS within distance.)
//    // That is, allowed to remove "bumps".
//    public static Point2D[] Simplify_Polygon2(Point2D[] pts, double tolerance, double EPS = 0.02, bool doSmooth = false)
//    {
//        string strReport = "";

//        List<int> outIs = new List<int>();

//        double unitScale = Unit_Scaled(pts);

//        // Dim threshold As Double = DegreesToRadians(0.05)
//        double distSqThreshold = Math.Pow((unitScale * tolerance), 2);
//        // This limits how large a bump we can eliminate.  Too small, and might miss a continuation, too large and performance suffers.
//        double bumpSizeTolerance = 4 * tolerance;
//        double bumpSizeSqThreshold = Math.Pow((unitScale * bumpSizeTolerance), 2);
//        // TODO: Variable formula combining distance and EPS, such that larger distances would have smaller angle threshold (instead of abruptly deciding to keep the point at a given distance).


//        // Method 3 Cross product
//        // Dim minDiff As Double = Double.MaxValue, maxDiff As Double = Double.MinValue
//        // Dim test_history As New List(Of Single)
//        // Dim t_nChecks As Integer = 0, t_nBetterFar As Integer = 0

//        // Always keep first two points, which establish initial angle.
//        outIs.Add(0);
//        outIs.Add(1);
//        int iSkipUntil = 0;
//        // Almost always will be stopped first by distance.
//        int nLookAhead = LastIndex(pts) / 2;
//        // 
//        for (int inI = 2; inI <= LastIndex(pts); inI++)
//        {
//            if (inI < iSkipUntil)
//                continue;
//            // Dim crossProduct As Double = pts(inI-1).Cross(pts(inI), pts(inI + 1))
//            int lastOutII = LastIndex(outIs);
//            Point2D p0 = pts[outIs[lastOutII - 1]];
//            // aka previous outI
//            int lastOutI = outIs[lastOutII];
//            Point2D p1 = pts[lastOutI];
//            Point2D p2 = pts[inI];
//            double distSq = DistanceSquared2D(p1, p2);

//            double dist01Sq = DistanceSquared2D(p0, p1);
//            double absCross = CalcAbsCross(p0, p1, p2, dist01Sq);
//            // If (absCross < minDiff) AndAlso (absCross > 0) Then _
//            // minDiff = absCross
//            // AccumMax(absCross, maxDiff)
//            // test_history.Add(absCross)

//            int prevInI = inI - 1;


//            if (doSmooth)
//            {
//                // (absCross >= EPS) OrElse
//                if (((distSq >= distSqThreshold)))
//                {
//                    // Exceeded thresholds (since last kept point).
//                    // Find best point to keep before the threshold is exceeded. (Though sometimes have no choice but to take inI.)
//                    // Take into account the angle AFTER each point, by finding the best NEXT point to go with each possible choice.
//                    int bestInJ = CalcBestInJ(pts, lastOutI, inI, p0, p1, dist01Sq, absCross, nLookAhead, distSqThreshold, bumpSizeSqThreshold);
//                    outIs.Add(bestInJ);
//                }
//            }
//            else
//                // ' --- TODO: Better (needs testing) ---
//                // If (absCross >= EPS) OrElse (distSq >= distSqThreshold) Then
//                // If lastOutI < prevInI Then
//                // ' Keep PREVIOUS point (so within spec).
//                // outIs.Add(prevInI)
//                // Else
//                // ' Previous is already kept; keep CURRENT point (best we can do).
//                // outIs.Add(inI)
//                // End If
//                // Else
//                // ' Don't need current point (unless next point is beyond spec; at that time will be "previous")
//                // Test()
//                // End If

//                if ((absCross >= EPS))
//            {
//                // After a bend.
//                // ALSO keep the point IN THE CORNER OF the bend (if not already kept).
//                if ((lastOutI < prevInI) && (!doSmooth))
//                    outIs.Add(prevInI);
//                // After bend - keep inI
//                // NOTE: If kept prevInI, Only need if "DistanceSquared2D(pts(prevInI), p2) >= distSqThreshold",
//                // but not worth further testing to optimize that case.
//                outIs.Add(inI);
//            }
//            else if (distSq >= distSqThreshold)
//                // Exceeded distance.
//                // TBD: Keep the BEST point up to inI.
//                // TBD: Do we ever get here, given the logic above?
//                outIs.Add(inI);
//            else
//                // Near last out point, and almost a straight line - discard inI.
//                Test();
//        }

//        // Always keep final point.
//        if (LastElement(outIs) != LastIndex(pts))
//            outIs.Add(LastIndex(pts));

//        // strReport &= "Min diff: " & minDiff.ToString(ValStr(minDiff)) & vbCrLf
//        // Debug.WriteLine(strReport)



//        strReport = "";

//        List<Point2D> outPts = new List<Point2D>(outIs.Count);
//        foreach (int outI in outIs)
//            outPts.Add(pts[outI]);

//        if (doSmooth)
//        {
//            // Spread bends into straight sections.
//            // (Without this, when form Bezier's, there are "kinks" where angles are zero.)

//            int nWrap = GetNWrap(outPts);
//            // Angle per point, via wrapping. If DoesWrap, no angle for last point, as that is identical to first point.
//            List<float> outAngles = new List<float>(nWrap);
//            float sumAngleDegrees = 0;
//            for (int iPt = 0; iPt <= nWrap - 1; iPt++)
//            {
//                Point2D p0 = outPts[WrappedIndex(iPt - 1, nWrap)];
//                Point2D p1 = outPts[iPt];
//                Point2D p2 = outPts[WrappedIndex(iPt + 1, nWrap)];
//                float angle = System.Convert.ToSingle(CalcBendDegrees(p0, p1, p2));
//                outAngles.Add(angle);
//                sumAngleDegrees += angle;
//            }

//            // "-1": Each distance uses two points.
//            // outDistances(i) corresponds to outPts(i) and (i+1).
//            List<float> outDistances = new List<float>(nWrap - 1);
//            // "1": Each distance uses point before.
//            for (int iPt = 1; iPt <= LastIndex(outPts); iPt++)
//            {
//                Point2D pt0 = outPts[iPt - 1];
//                Point2D pt1 = outPts[iPt];
//                outDistances.Add(System.Convert.ToSingle(Distance2D(pt0, pt1)));
//            }

//            for (int iAngle = 0; iAngle <= nWrap - 1; iAngle++)
//            {
//                int iPt = iAngle;
//                float a0 = outAngles[WrappedIndex(iAngle - 1, nWrap)];
//                float a1 = outAngles[iAngle];
//                float a2 = outAngles[WrappedIndex(iAngle + 1, nWrap)];
//                float absA0 = Math.Abs(a0);

//                float dist01 = outDistances[WrappedIndex(iAngle - 1, nWrap)];
//                float dist12 = outDistances[iAngle];

//                // Delta is positive when a1 is smaller.
//                float da01 = Math.Abs(a0) - Math.Abs(a1);
//                float da21 = Math.Abs(a2) - Math.Abs(a1);
//                if (iAngle >= 112)
//                    Test();
//                if (da01 >= 1)
//                    // a0 is significantly greater than a1.
//                    // Spread bend from a0 to a1.
//                    // But "limited" by bend at a2?
//                    Test();
//            }
//        }

//        strReport += "START Vertice count: " + pts.Length + Constants.vbCrLf;
//        strReport += "END Vertice count: " + outPts.Count + Constants.vbCrLf;
//        // Debug.WriteLine(strReport)

//        return outPts.ToArray();
//    }

//    private static int CalcBestInJ(Point2D[] pts, int lastOutI, int inI, Point2D p0, Point2D p1, double dist01Sq, double absCross, int nLookAhead, double distSqThreshold, double bumpSizeSqThreshold)
//    {
//        // NOTE: This "absCross" is from inI, which might exceed distance.
//        // HOWEVER, if there is no better one, should we keep it?
//        // NO, we haven't tested what far point would go with this, so may not be valid (even ignoring distance threshold).
//        double minAbsCross = double.MaxValue;   // absCross
//        int bestInJ = inI;

//        // TBD: Wrap around?
//        // inI + 1 To Math.Min(inI + nLookAhead, LastIndex(pts))
//        // There might not be ANY points in this range. In which case, use inI (from above).
//        for (int inJ = lastOutI + 1; inJ <= inI - 1; inJ++)
//        {
//            Point2D p2b = pts[inJ];
//            double distBSq = DistanceSquared2D(p1, p2b);
//            // TBD: I don't think either of these conditions ever happen, because inI is the first point out-of-range.
//            if (distBSq >= bumpSizeSqThreshold)
//                // Exceeded look-ahead.
//                // NOTE: If bump is taller than this check, will stop here - even if a later point might be within threshold.
//                break;
//            else if (distBSq >= distSqThreshold)
//                // Not an acceptable continuation point.
//                continue;

//            double absCrossB = CalcAbsCross(p0, p1, p2b, dist01Sq);
//            if (absCrossB < minAbsCross)
//            {
//                // Check whether OTHER end's cross is also better.
//                double absCrossBEnd = CalcAbsCrossBEnd(pts, inJ, p1, p2b, absCrossB, nLookAhead, distBSq, distSqThreshold, bumpSizeSqThreshold);
//                // The WORSE of the two ends is what we minimize.
//                absCrossB = Math.Max(absCrossB, absCrossBEnd);
//                if (absCrossB < minAbsCross)
//                {
//                    minAbsCross = absCrossB;
//                    bestInJ = inJ;
//                }
//            }
//        }

//        return bestInJ;
//    }

//    // NOTE: "Angle p1-p0-p2". Would it make more sense to work with "180 degrees minus Angle p0-p1-p2"?
//    private static double CalcAbsCross(Point2D p0, Point2D p1, Point2D p2, double dist01Sq)
//    {
//        double crossProduct = p0.Cross(p1, p2);
//        double magnitudeProduct = Math.Sqrt(dist01Sq * DistanceSquared2D(p0, p2));

//        double sinA = crossProduct / magnitudeProduct;
//        return Math.Abs(sinA);
//    }

//    private static double CalcAbsCrossBEnd(Point2D[] pts, int inJ, Point2D p1, Point2D p2b, double absCrossB, int nLookAhead, double distBSq, double distSqThreshold, double bumpSizeSqThreshold)
//    {
//        double absCrossBEnd = double.MaxValue;

//        int nWrap = GetNWrap(pts);

//        // Find best angle near far end.
//        // CAUTION: inK allowed to exceed last index; it wraps.
//        for (int inK = inJ + 1; inK <= inJ + nLookAhead; inK++)
//        {
//            Point2D p3 = pts[inK % nWrap];
//            // "If": Always calculate the first inK, even if it is too far. (Far if points are separated by excessive distance.)
//            if (absCrossBEnd < double.MaxValue)
//            {
//                double distBEndSq = DistanceSquared2D(p2b, p3);
//                if (distBEndSq >= bumpSizeSqThreshold)
//                    // Exceeded look-ahead.
//                    // NOTE: If bump is taller than this check, will stop here - even if a later point might be within threshold.
//                    break;
//                else if (distBEndSq >= distSqThreshold)
//                    continue;
//            }
//            AccumMin(CalcAbsCross(p1, p2b, p3, distBSq), ref absCrossBEnd);
//            if (absCrossBEnd <= absCrossB)
//                // Quality limited by BStart.
//                break;
//        }

//        return absCrossBEnd;
//    }



//    // Pair of values representing a range.
//    public abstract class MinMax<T>
//    {
//        protected T _min, _max;
//        // Number of values accumulated.
//        protected int _count;

//        public MinMax(T min, T max)
//        {
//            this._min = min;
//            this._max = max;
//        }

//        public override string ToString()
//        {
//            return $"{ToStringPrefix()} (min={_min}, max={_max})";
//        }



//        public T Min
//        {
//            get
//            {
//                return _min;
//            }
//        }
//        public T Max
//        {
//            get
//            {
//                return _max;
//            }
//        }

//        public int Count
//        {
//            get
//            {
//                return _count;
//            }
//        }


//        // ALL subclasses must override to accumulate,
//        // then do "_count += 1".
//        public abstract void Accumulate(T value);


//        protected string ToStringPrefix()
//        {
//            return $"{this.GetType().Name} N={_count}";
//        }
//    }


//    public class MinMaxInteger : MinMax<int>
//    {

//        // Start an empty accumulation.
//        public static MinMaxInteger CreateForAccumulate()
//        {
//            // Start with reversed values, which indicates empty accumulation.
//            return new MinMaxInteger(int.MaxValue, int.MinValue);
//        }



//        public MinMaxInteger(int min, int max) : base(min, max)
//        {
//        }

//        public override string ToString()
//        {
//            return $"{MinMax(Of System.Int32).ToStringPrefix()} (min={MinMax(Of System.Int32)._min}, max={MinMax(Of System.Int32)._max})";
//        }



//        public override void Accumulate(int value)
//        {
//            mDL2DLib.AccumMinMax(value, ref MinMax(Of System.Int32)._min, ref MinMax(Of System.Int32)._max);
//            MinMax(OfSystem.Int32)._count += 1;
//        }


//        public int Delta
//        {
//            get
//            {
//                return MinMax(OfSystem.Int32)._max - MinMax(Of System.Int32)._min;
//            }
//        }
//    }


//    public class MinMaxSingle : MinMax<float>
//    {
//        public MinMaxSingle(float min, float max) : base(min, max)
//        {
//        }

//        public override string ToString()
//        {
//            return $"{MinMax(Of System.Single).ToStringPrefix()} (min={MinMax(Of System.Single)._min}, max={MinMax(Of System.Single)._max})";
//        }



//        public override void Accumulate(float value)
//        {
//            mDL2DLib.AccumMinMax(value, ref MinMax(Of System.Single)._min, ref MinMax(Of System.Single)._max);
//            MinMax(OfSystem.Single)._count += 1;
//        }


//        public float Delta
//        {
//            get
//            {
//                return MinMax(OfSystem.Single)._max - MinMax(Of System.Single)._min;
//            }
//        }
//    }


//    public class MinMaxDouble : MinMax<double>
//    {

//        // Incoming values may be in either order.
//        public static MinMaxDouble FromPair(double a, double b)
//        {
//            if (b < a)
//                return new MinMaxDouble(b, a);
//            else
//                return new MinMaxDouble(a, b);
//        }

//        // Start an empty accumulation.
//        public static MinMaxDouble CreateForAccumulate()
//        {
//            // Start with reversed values, which indicates empty accumulation.
//            return new MinMaxDouble(double.MaxValue, double.MinValue);
//        }



//        public MinMaxDouble(double min, double max) : base(min, max)
//        {
//        }


//        public override string ToString()
//        {
//            return $"{MinMax(Of System.Double).ToStringPrefix()} (min={MinMax(Of System.Double)._min}, max={MinMax(Of System.Double)._max})";
//        }


//        public override void Accumulate(double value)
//        {
//            mDL2DLib.AccumMinMax(value, ref MinMax(Of System.Double)._min, ref MinMax(Of System.Double)._max);
//            MinMax(OfSystem.Double)._count += 1;
//        }



//        // CAUTION: Overflow if call before any elements have been Accumulated.
//        // CAUTION: May be negative for an empty accumulation.
//        // To be safe, call HasExtent first, only call Delta if that is True.
//        // TBD: Perhaps this should incorporate HasExtent logic,
//        // convert uninitialized value (no Accumulate calls) to "-1".
//        public double Delta
//        {
//            get
//            {
//                return MinMax(OfSystem.Double)._max - MinMax(Of System.Double)._min;
//            }
//        }

//        public bool HasExtent(bool inclusive)
//        {
//            if (MinMax(OfSystem.Double)._max < MinMax(OfSystem.Double)._min )
//                return false;
//            else if (MinMax(OfSystem.Double)._max == MinMax(OfSystem.Double)._min )
//                return inclusive;
//            else
//                return true;
//        }

//        public bool Intersects(MinMaxDouble other)
//        {
//            double minY = Math.Max(this.Min, other.Min);
//            double maxY = Math.Min(this.Max, other.Max);
//            return (minY <= maxY);
//        }

//        // "tracking": Tell caller whether this is new minimum or maximum.
//        public void AccumulateWithTracking(double v, out bool isNewMin, out bool isNewMax)
//        {
//            if (v < MinMax(OfSystem.Double)._min )
//            {
//                MinMax(OfSystem.Double)._min = v;
//                isNewMin = true;
//            }
//            else
//                isNewMin = false;

//            if (v > MinMax(OfSystem.Double)._max )
//            {
//                MinMax(OfSystem.Double)._max = v;
//                isNewMax = true;
//            }
//            else
//                isNewMax = false;
//        }
//    }


//    public class MinMaxPoint : MinMax<Point>
//    {

//        // Standard instance that has IsValid=False.
//        public static MinMaxPoint NotValidOne = new MinMaxPoint(0, 0, -1, -1);

//        public MinMaxPoint(Point min, Point max) : base(min, max)
//        {
//        }

//        public MinMaxPoint(int minX, int minY, int maxX, int maxY) : this(new Point(minX, minY), new Point(maxX, maxY))
//        {
//        }



//        public override void Accumulate(Point value)
//        {
//            if (value.X < MinMax(Of Point)._min.X)
//                MinMax(Of Point)._min.X = value.X;
//            if (value.Y < MinMax(Of Point)._min.Y)
//                MinMax(Of Point)._min.Y = value.Y;

//            if (value.X > MinMax(Of Point)._max.X)
//                MinMax(Of Point)._max.X = value.X;
//            if (value.Y > MinMax(Of Point)._max.Y)
//                MinMax(Of Point)._max.Y = value.Y;

//            MinMax(OfPoint)._count += 1;
//        }


//        // Public ReadOnly Property Delta() As Point
//        // Get
//        // Return m_Max - m_Min
//        // End Get
//        // End Property

//        /// <summary>
//        ///         ''' aka "Is Not Empty".
//        ///         ''' </summary>
//        ///         ''' <returns></returns>
//        public bool IsValid
//        {
//            get
//            {
//                return ((this.Max.X >= this.Min.X) && (this.Max.Y >= this.Min.Y));
//            }
//        }

//        public int MinX
//        {
//            get
//            {
//                return MinMax(Of Point)._min.X;
//            }
//        }
//        public int MaxX
//        {
//            get
//            {
//                return MinMax(Of Point)._max.X;
//            }
//        }
//        public int MinY
//        {
//            get
//            {
//                return MinMax(Of Point)._min.Y;
//            }
//        }
//        public int MaxY
//        {
//            get
//            {
//                return MinMax(Of Point)._max.Y;
//            }
//        }
//    }


//    public class MinMaxPoint2D : MinMax<Point2D>
//    {

//        // Start an empty accumulation.
//        public static MinMaxPoint2D CreateForAccumulate()
//        {
//            // Start with reversed values, which indicates empty accumulation.
//            return new MinMaxPoint2D(Point2D.MaxValue, Point2D.MinValue);
//        }

//        // Used when corners might not be in min/max order. E.g. due to flip of x or y.
//        public static MinMaxPoint2D CreateFromCorners(Point2D corner1, Point2D corner2)
//        {
//            Point2D minPt = corner1;
//            Point2D maxPt = corner1;
//            mDL2DLib.AccumMinMax(corner2, ref minPt, ref maxPt);

//            return new MinMaxPoint2D(minPt, maxPt);
//        }

//        // "corners" might be two UL/BR or might be four (possibly rotated rectangle).
//        public static MinMaxPoint2D CreateFromCorners(Point2D[] corners)
//        {
//            MinMaxPoint2D mm = MinMaxPoint2D.CreateForAccumulate();

//            foreach (Point2D corner in corners)
//                mm.Accumulate(corner);

//            return mm;
//        }

//        public static bool Valid(Point2D min, Point2D max)
//        {
//            return (min.IsValid && max.IsValid && (min.X <= max.X) && (min.Y <= max.Y));
//        }



//        public MinMaxPoint2D(Point2D min, Point2D max) : base(min, max)
//        {
//        }

//        public MinMaxPoint2D(RectangleF rect) : this(new Point2D(rect.X, rect.Y), new Point2D(rect.Right, rect.Bottom))
//        {
//        }


//        public override string ToString()
//        {
//            return string.Format("{0}(min={1}, max={2})", this.GetType().Name, MinMax(Of TestProject.mDL2DLib.Point2D)._min, MinMax(Of TestProject.mDL2DLib.Point2D)._max);
//        }

//        public string ShortString()
//        {
//            return string.Format("(min={0}, max={1})", MinMax(Of TestProject.mDL2DLib.Point2D)._min, MinMax(Of TestProject.mDL2DLib.Point2D)._max);
//        }



//        public override void Accumulate(Point2D pt)
//        {
//            mDL2DLib.AccumMinMax(pt, ref MinMax(Of TestProject.mDL2DLib.Point2D)._min, ref MinMax(Of TestProject.mDL2DLib.Point2D)._max);
//            MinMax(OfTestProject.mDL2DLib.Point2D)._count += 1;
//        }

//        public void AccumulateRect(Rectangle2D rect)
//        {
//            Accumulate(rect.TopLeft);
//            Accumulate(rect.BottomRight);
//        }

//        /// <summary>
//        ///         ''' Positive values enlarge rectangle.
//        ///         ''' </summary>
//        ///         ''' <param name="lowX"></param>
//        ///         ''' <param name="lowY"></param>
//        ///         ''' <param name="highX"></param>
//        ///         ''' <param name="highY"></param>
//        public void ExpandBy(double lowX, double lowY, double highX, double highY)
//        {
//            if (IsValid())
//            {
//                // Positive lowX means make rectangle larger, by lowering minX.
//                MinMax(Of TestProject.mDL2DLib.Point2D)._min.X -= lowX;
//                MinMax(Of TestProject.mDL2DLib.Point2D)._min.Y -= lowY;
//                MinMax(Of TestProject.mDL2DLib.Point2D)._max.X += highX;
//                MinMax(Of TestProject.mDL2DLib.Point2D)._max.Y += highY;
//            }
//        }


//        // NOTE: If created by "CreateForAccumulate", before any calls made to Accumulate, this will return False.
//        public bool IsValid
//        {
//            get
//            {
//                return Valid(this.Min, this.Max);
//            }
//        }

//        public Point2D Center
//        {
//            get
//            {
//                return Average(MinMax(Of TestProject.mDL2DLib.Point2D)._max, MinMax(Of TestProject.mDL2DLib.Point2D)._min);
//            }
//        }

//        public Point2D Delta
//        {
//            get
//            {
//                return MinMax(OfTestProject.mDL2DLib.Point2D)._max - MinMax(Of TestProject.mDL2DLib.Point2D)._min;
//            }
//        }

//        public Rectangle2D AsRect
//        {
//            get
//            {
//                return Rectangle2D.FromMinMax(MinMax(Of TestProject.mDL2DLib.Point2D)._min, MinMax(Of TestProject.mDL2DLib.Point2D)._max);
//            }
//        }


//        public bool Contains(Point2D pt)
//        {
//            return BetweenInclusive(pt.X, MinMax(Of TestProject.mDL2DLib.Point2D).Min.X, MinMax(Of TestProject.mDL2DLib.Point2D).Max.X) && BetweenInclusive(pt.Y, MinMax(Of TestProject.mDL2DLib.Point2D).Min.Y, MinMax(Of TestProject.mDL2DLib.Point2D).Max.Y);
//        }
//    }




//    public static Point2D[] ConvertArrayPointFtoPoint2D(System.Drawing.PointF[] ptfPoint)
//    {
//        if (ptfPoint == null)
//            return null;
//        Point2D[] ptdPoint = new Point2D[ptfPoint.Length - 1 + 1];
//        for (int intIdx = 0; intIdx <= ptfPoint.Length - 1; intIdx++)
//            ptdPoint[intIdx] = new Point2D((ptfPoint[intIdx]));
//        return ptdPoint;
//    }

//    public static System.Drawing.PointF[] ConvertArrayPoint2DToPointF(Point2D[] ptdPoint)
//    {
//        if (ptdPoint == null)
//            return null;
//        System.Drawing.PointF[] ptfPoint = new System.Drawing.PointF[ptdPoint.Length - 1 + 1];
//        for (int intIdx = 0; intIdx <= ptdPoint.Length - 1; intIdx++)
//            ptfPoint[intIdx] = ptdPoint[intIdx].ToPointF();
//        return ptfPoint;
//    }
//    public static PointF ConvertPoint2DToPointF(Point2D ptdPoint)
//    {
//        return new System.Drawing.PointF(System.Convert.ToSingle(ptdPoint.X), System.Convert.ToSingle(ptdPoint.Y));
//    }
//    public static Point2D ConvertPointFToPoint2d(System.Drawing.PointF ptfPoint)
//    {
//        return new Point2D(ptfPoint);
//    }

//    public static Point2D[] ConvertRectangleToPoint2D(System.Drawing.RectangleF rcfRect)
//    {
//        Point2D[] ptdpoint = new Point2D[4];
//        ptdpoint[0] = new Point2D(rcfRect.X, rcfRect.Y);
//        ptdpoint[1] = new Point2D(System.Convert.ToDouble(rcfRect.X + rcfRect.Width), System.Convert.ToDouble(rcfRect.Y));
//        ptdpoint[2] = new Point2D(System.Convert.ToDouble(rcfRect.X + rcfRect.Width), System.Convert.ToDouble(rcfRect.Y + rcfRect.Height));
//        ptdpoint[3] = new Point2D(System.Convert.ToDouble(rcfRect.X), System.Convert.ToDouble(rcfRect.Y + rcfRect.Height));
//        return ptdpoint;
//    }
//}
