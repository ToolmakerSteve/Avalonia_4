using System;
using System.Collections.Generic;
using System.Drawing;
using static Global.Utils;

namespace Global
{
    // TBD: Now that rotation is allowed, store CENTER, do everything relative to that?
    // However, that is inefficient for non-rotated.
    // And requires frequently dividing with/height by 2. Store half-dimensions?
    public struct Rectangle2D
    {
        public static Rectangle2D Empty = new Rectangle2D(0, 0, 0, 0);

        public static Size2D NoTwist = new Size2D(0, 0);

        public static Rectangle2D NaN
        {
            get
            {
                return new Rectangle2D(double.NaN, double.NaN, double.NaN, double.NaN);
            }
        }

        public static Rectangle2D NegativeInfinity
        {
            get
            {
                return new Rectangle2D(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
            }
        }


        public static Rectangle2D FromCenter(Point2D centerP, double width, double height, double rotation)
        {
            // Create a rotated rect with corner at zero, so can use Rectangle2D's internal calculations to relate corner to center.
            Rectangle2D rect = new Rectangle2D(Point2D.Zero(), new Size2D(width, height), rotation, false);
            // Adjust so center is at correct position.
            Point2D invCenter = rect.Center;
            rect.m_position += centerP - invCenter;
            // Verify
            if (!rect.Center.NearlyEquals(centerP))
                Trouble("Rectangle2D.FromCenter");

            return rect;
        }


        // Preserves negative extents:  if minOrTopY > maxOrBottomY, will have negative height.
        // aka FromLTRB (left-top, right-bottom).
        public static Rectangle2D FromMinMax(double leftX, double minOrTopY, double rightX, double maxOrBottomY)
        {
            return new Rectangle2D(leftX, minOrTopY, rightX - leftX, maxOrBottomY - minOrTopY);
        }

        // Preserves negative extents:  if minY > maxY, will have negative height.
        // TODO: Units.
        public static Rectangle2D FromMinMax(Point2D minP, Point2D maxP)
        {
            return FromMinMax(minP.X.Value, minP.Y.Value, maxP.X.Value, maxP.Y.Value);
        }

        // If "preserveNegativeExtents = True", then corner1 => TopLeft, corner2 => BottomRight; Width and Height allowed to be negative.
        // TODO: Units.
        public static Rectangle2D FromOppositeCorners(Point2D corner1, Point2D corner2, bool preserveNegativeExtents = false)
        {
            if (preserveNegativeExtents)
                // corner1 => TopLeft, corner2 => BottomRight; Width and Height allowed to be negative.
                // Equivalent to calling FromMinMax(corner1.X, corner1.Y, corner2.X, corner2.Y).
                return new Rectangle2D(corner1,
                            new Size2D(corner2.X.Value - corner1.X.Value, corner2.Y.Value - corner1.Y.Value));
            else
                return FromMinMax(Math.Min(corner1.X.Value, corner2.X.Value),
                                  Math.Min(corner1.Y.Value, corner2.Y.Value),
                                  Math.Max(corner1.X.Value, corner2.X.Value),
                                  Math.Max(corner1.Y.Value, corner2.Y.Value));
        }

        // rectPoints is 4-point-polygon (4 corners, clockwise or zig-zag).
        // Preserves flip in x or y (width/height may be negative).
        // TODO: Units.
        public static Rectangle2D FromRectAsPoints_AssumeNoRotation_PreserveFlip(Point2D[] rectPoints)
        {
            // TopLeft, TopRight.
            Point2D TL = rectPoints[0];
            Point2D TR = rectPoints[1];

            // ASSUME No Rotation.
            double left = TL.X.Value;
            double right = TR.X.Value;
            double top = TL.Y.Value;
            // when not rotated, same Y on both (2) and (3), so can use either -
            // regardless of whether clockwise or zigzag indices.
            double bottom = rectPoints[2].Y.Value;
            // Preserves flip in x or y (width/height may be negative).
            return new Rectangle2D(left, top, right - left, bottom - top);
        }

        //// rectPoints is 4-point-polygon (4 corners, clockwise or zig-zag).
        //// NOTE: If you don't want a ROTATED rectangle, use FromRectAsPoints_NoRotation_NoFlip or FromRectAsPoints_Cardinal(True, ..).
        //// CAUTION: This may return a ROTATED rectangle; if Angle<>0, then XY is not UL (even if Width & Height are positive).
        //// CAUTION: If rectPoints doesn't form a rectangle, the resulting rectangle obviously won't replicate the coordinate system implied by the 4 rectPoints.
        //// This happens (to a perceptible amount) when convert WGS-84 to UTM.
        //// Also happens (even worse) when import changed corners from Maya.
        //// VERSION: If it is rotated, set twist based on rectPoints(1).
        //public static Rectangle2D FromRectAsPoints(Point2D[] rectPoints, bool preserveNegativeExtents = false)
        //{
        //    Point2D BL;
        //    // BottomLeft, BottomRight.
        //    bool isZigZag;
        //    Point2D BR = DetermineDiagonalPoint(rectPoints, out BL, out isZigZag);
        //    // TopLeft, TopRight.
        //    Point2D TL = rectPoints[0];
        //    Point2D TR = rectPoints[1];
        //    double rotation = GetAngleRadians2D(TR - TL);

        //    // So that round-off errors don't cause a rectangle to be rotated,
        //    // when for all practical purposes it is not rotated,
        //    // Treat a negligible rotation as a zero rotation.
        //    Rectangle2D rect;
        //    if (rotation.NearlyEquals(0.0))
        //    {
        //        rect = FromOppositeCorners(TL, BR, preserveNegativeExtents);
        //        rect.m_fromZigZag = isZigZag;
        //    }
        //    else
        //        // Rotated.  preserveNegativeExtents, so ExportSettings can re-create correct corner order from this rectangle.
        //        rect = From3Corners_Rotated(TL, TR, BL, rotation, isZigZag, preserveNegativeExtents);

        //    if (false && preserveNegativeExtents)
        //    {
        //        // Verify
        //        Point2D[] resultAsPts = rect.AsCorners;
        //        for (int i = 0; i <= 3; i++)
        //        {
        //            Point2D oldCorner = rectPoints[i];
        //            Point2D newCorner = resultAsPts[i];
        //            if (!newCorner.NearlyEquals(oldCorner))
        //                Trouble();
        //        }
        //    }

        //    if (!preserveNegativeExtents)
        //    {
        //        // Verify non-negative.
        //        if ((rect.Width < 0) || (rect.Height < 0))
        //            Trouble();
        //    }

        //    return rect;
        //}

        //// Width/Height always non-negative (does not preserve negative extents).
        //// If rect is rotated, returns the minimum non-rotated rectangle that includes all 4 corners.
        //// NOTE: "rectPoints" allowed to be an arbitrary polygon, with more or less than 4 points.
        //public static Rectangle2D FromRectAsPoints_NoRotation_NoFlip(Point2D[] rectPoints)
        //{
        //    // Use all 4 points, and expand (non-rotated) rect to include them.
        //    Point2D maxPt;
        //    Point2D minPt = Calculate_MinMax(rectPoints, ref maxPt);
        //    return Rectangle2D.FromMinMax(minPt, maxPt);
        //}


        //// Return BottomRight; set BottomLeft, isZigZag1.
        //// TMS HACK: Some exports have BR corner in (3). Can tell which corner it is, by finding which point is farther from point (0).
        //public static Point2D DetermineDiagonalPoint(Point2D[] rectPoints, out Point2D BL, out bool isZigZag1)
        //{
        //    Point2D BR;

        //    isZigZag1 = IsZigZag(rectPoints);
        //    if (isZigZag1)
        //    {
        //        BR = rectPoints[3];
        //        BL = rectPoints[2];
        //    }
        //    else
        //    {
        //        BR = rectPoints[2];
        //        BL = rectPoints[3];
        //    }

        //    return BR;
        //}

        //// NOTE: When "preserveNegativeExtents=False", the corners may be flipped.
        //// That is, returned rectangle may no longer have TL as its TL.
        //// Only handles y-flip. TODO: Also need x-flip logic? (If both are flipped, may be ambiguous when rotated.)
        //private static Rectangle2D From3Corners_Rotated(Point2D TL, Point2D TR, Point2D BL, double rotation, bool fromZigZag, bool preserveNegativeExtents = false)
        //{
        //    // Because the rectangle is rotated, to detect y-flip, compare the RELATIVE orientations of
        //    // two adjacent sides (rather than directly comparing y).
        //    double rotation2 = GetAngleRadians2D(BL - TL);
        //    double deltaDegrees = RadiansToDegrees_Signed180(rotation2 - rotation);
        //    if (!Math.Abs(deltaDegrees).NearlyEquals(90, 1))
        //        Dubious();
        //    bool yFlip2;
        //    if (deltaDegrees >= 0)
        //        yFlip2 = false;
        //    else
        //        // The corners passed in represent a rectangle that is y-flipped, then rotated.
        //        yFlip2 = true;

        //    Size2D size = new Size2D(Distance2D(TL, TR), Distance2D(TL, BL));

        //    if (yFlip2)
        //    {
        //        // The corners passed in represent a rectangle that is y-flipped, then rotated.
        //        if (preserveNegativeExtents)
        //            // Represent that y-flip with a negative height.
        //            size.Height = -size.Height;
        //        else
        //            // y-swap the corners, to yield the corresponding rectangle that has positive height.
        //            Swap(TL, BL);
        //    }

        //    Rectangle2D rect = new Rectangle2D(TL, size, rotation, fromZigZag);

        //    return rect;
        //}


        // Expand (a,b) range to include value. b might be less than a.
        private static void ExpandRange(ref double a, ref double b, double value)
        {
            if (a > b)
            {
                if (value > a)
                    a = value;
                else if (value < b)
                    b = value;
            }
            else if (value < a)
                a = value;
            else if (value > b)
                b = value;
        }

        //// Force result to have positive (non-negative) Width and Height.
        //// Remember whether flipped x and/or y to achieve that.
        //public static Rectangle2D FromRectAsPoints_Cardinal(Point2D[] rectPoints, bool allowRotated, out bool xFlip, out bool yFlip)
        //{
        //    Point2D pb;
        //    bool isZigZag;
        //    Point2D pDiag = DetermineDiagonalPoint(rectPoints, out pb, out isZigZag);
        //    Point2D p0 = rectPoints[0];
        //    Point2D delta = pDiag - p0;

        //    xFlip = (delta.X < 0);
        //    yFlip = (delta.Y < 0);
        //    MaybeFlip(rectPoints, xFlip, yFlip);

        //    if (allowRotated)
        //    {
        //        Rectangle2D cardinalRect = FromRectAsPoints(rectPoints, true);

        //        // verify
        //        if ((cardinalRect.Width < 0) || (cardinalRect.Height < 0))
        //            throw new InvalidProgramException("FromRectAsPoints_Cardinal");

        //        return cardinalRect;
        //    }
        //    else
        //        // NOTE: We've set xFlip & yFlip, so we know if the original rectPoints were flipped.
        //        return FromRectAsPoints_NoRotation_NoFlip(rectPoints);
        //}

        // Analyze rectangle that may have negative width or height. (flipped in x or y)
        // Convert to positive width/height, and remember any xFlip or yFlip.
        public static Rectangle2D RememberXOrYFlip(Rectangle2D origRect, out bool xFlip, out bool yFlip)
        {
            Rectangle2D cardinalRect = origRect;  // Clone. Since is structure, assign is same as clone.
            if (origRect.Width < 0)
            {
                xFlip = true;
                cardinalRect.X = cardinalRect.X + cardinalRect.Width;
                cardinalRect.Width = -cardinalRect.Width;
            }
            else
                xFlip = false;

            if (origRect.Height < 0)
            {
                yFlip = true;
                cardinalRect.Y = cardinalRect.Y + cardinalRect.Height;
                cardinalRect.Height = -cardinalRect.Height;
            }
            else
                yFlip = false;

            return cardinalRect;
        }

        //// TopLeft, TopRight, and BottomLeft.
        //public static Rectangle2D From3Corners(Point2D TL, Point2D TR, Point2D BL, bool fromZigZag)
        //{
        //    double rotation = GetAngleRadians2D(TR - TL);
        //    if (rotation.NearlyEquals(0.0))
        //    {
        //        // Verify
        //        if ((!(TR.Y - TL.Y).NearlyEquals(0.0)) || (!(BL.X - TL.X).NearlyEquals(0.0)))
        //            Trouble("Rectangle2D.From3Corners");
        //        return new Rectangle2D(TL.X, TL.Y, TR.X - TL.X, BL.Y - TL.Y);
        //    }
        //    else
        //    {
        //        Rectangle2D rect = From3Corners_Rotated(TL, TR, BL, rotation, fromZigZag);

        //        // Verify
        //        if ((!rect.TopLeft.NearlyEquals(TL)) || (!rect.TopRight.NearlyEquals(TR)) || (!rect.BottomLeft.NearlyEquals(BL)))
        //            Trouble("Rectangle2D.From3Corners");

        //        return rect;
        //    }
        //}



        private Point2D m_position;
        private Size2D m_size;
        private bool m_fromZigZag;
        private double m_rotationRadians;
        // Unit multipliers corresponding to m_rotationRadians
        private double m_cos, m_sin;



        // X aka Left, Y aka Top.
        public Rectangle2D(double x, double y, double width, double height)
                : this(new Point2D(x, y), new Size2D(width, height))
        {
        }

        public Rectangle2D(Point2D position, Size2D size)
        {
            m_position = position;
            m_size = size;

            m_fromZigZag = false;
            m_rotationRadians = 0;
            m_cos = 1;   // rotation=0.
            m_sin = 0;
        }

        public Rectangle2D(Point2D position, Size2D size, double rotationRadians1, bool fromZigZag)
        {
            m_position = position;
            m_size = size;

            m_fromZigZag = fromZigZag;
            m_rotationRadians = rotationRadians1;

            // Wasted, but needed before allowed to call RotationRadians setter.
            m_cos = 1;   // rotation=0.
            m_sin = 0;

            // Use setter, to round near-zero values, and to set m_cos, m_sin.
            this.RotationRadians = rotationRadians1;
        }

        public Rectangle2D(RectangleF rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        {
        }

        public Rectangle2D(Rectangle rect) : this(rect.X, rect.Y, rect.Width, rect.Height)
        {
        }



        public Point2D Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        public Size2D Size
        {
            get
            {
                return m_size;
            }
            set
            {
                m_size = value;
            }
        }

        // True if size is zero.
        public bool IsEmpty
        {
            get
            {
                return m_size.IsEmpty;
            }
        }

        // True if width,height aren't NaN or Infinity.
        public bool IsValid
        {
            get
            {
                bool isBad = double.IsNaN(m_size.Width) || double.IsInfinity(m_size.Width) || double.IsNaN(m_size.Height) || double.IsInfinity(m_size.Height);
                return !isBad;
            }
        }


        public double X
        {
            get
            {
                return m_position.X;
            }
            set
            {
                m_position.X = value;
            }
        }

        public double Y
        {
            get
            {
                return m_position.Y;
            }
            set
            {
                m_position.Y = value;
            }
        }

        // CAUTION: Some callers use negative height to represent y-flip.
        // CAUTION: Logic outside this class is only correct if width&height are non-negative!
        // CAUTION: These are UNROTATED Width/Height. See RotatedWidth/Height.
        public double Width
        {
            get
            {
                return m_size.Width;
            }
            set
            {
                m_size.Width = value;
            }
        }

        public double Height
        {
            get
            {
                return m_size.Height;
            }
            set
            {
                m_size.Height = value;
            }
        }

        public double RotationRadians
        {
            get
            {
                return m_rotationRadians;
            }
            set
            {
                // If rotation is essentially zero, make it zero.
                if (value.NearlyEquals(0.0))
                    value = 0.0;

                m_rotationRadians = value;
                // PERFORMANCE: Cache.
                m_cos = (Math.Cos(value));
                m_sin = (Math.Sin(value));
            }
        }

        public bool IsRotated
        {
            get
            {
                return (m_rotationRadians != 0.0);
            }
        }

        // X.
        // CAUTION: Left/Right/Top/Bottom only correct if width&height are non-negative, and Not IsRotated!
        // See Also: MinX.
        public double Left
        {
            get
            {
                if (this.IsRotated)
                    // Throw New NotImplementedException("Rectangle2D.Left getter")
                    return m_position.X;
                else
                    return m_position.X;
            }
            set
            {
                if (this.IsRotated)
                    throw new NotImplementedException("Rectangle2D.Left setter");
                else
                    m_position.X = value;
            }
        }

        // For clients who specify they are looking at X's min/max range.
        public double MinX
        {
            get
            {
                if (IsRotated)
                {
                    TODO();
                    return m_position.X;
                }
                else if (Width < 0)
                    // TBD: Will this surprise any callers?
                    return Right;
                // 
                return Left;
            }
        }

        // For clients who specify they are looking at X's min/max range.
        public double MaxX
        {
            get
            {
                if (IsRotated)
                    TODO();
                else if (Width < 0)
                    // TBD: Will this surprise any callers?
                    return Left;
                // 
                return Right;
            }
        }


        public Point2D WidthRotated
        {
            get
            {
                return m_size.Width * new Point2D(m_cos, m_sin);
            }
        }
        public Point2D HeightRotated
        {
            get
            {
                // "Top to Bottom" is vector at "angle + 90 degrees".
                // (cos, sin)(angle + 90 degrees) = (-sin, cos)(angle)
                return m_size.Height * new Point2D(-m_sin, m_cos);
            }
        }

        // TODO: Doesn't handle negative Width.
        public Point2D UnitWidthRotated
        {
            get
            {
                return new Point2D(m_cos, m_sin);
            }
        }
        // NOTE: If Height is negative, the unit's direction is inverted.
        public Point2D UnitHeightRotated
        {
            get
            {
                // "Top to Bottom" is vector at "angle + 90 degrees".
                // (cos, sin)(angle + 90 degrees) = (-sin, cos)(angle)
                return Math.Sign(m_size.Height) * new Point2D(-m_sin, m_cos);
            }
        }

        // X + Width.
        // See Also: MaxX.
        public double Right
        {
            get
            {
                if (this.IsRotated)
                    return BottomRight.X;
                // 
                return m_position.X + m_size.Width;
            }
            set
            {
                if (this.IsRotated)
                    throw new NotImplementedException("Rectangle2D.Right setter");
                else
                    m_size.Width = value - m_position.X;
            }
        }

        // Y
        // See Also: MinY.
        public double Top
        {
            get
            {
                // If Me.IsRotated Then
                // ' TBD whether this means "smallest y of any corner".
                // Throw New NotImplementedException("Rectangle2D.Top getter for rotated rectangle")
                // Else
                // ' TODO: If height is negative, should this add that negative height, to get "smallest y"?
                return m_position.Y;
            }
            set
            {
                // If Me.IsRotated Then
                // Throw New NotImplementedException("Rectangle2D.Top setter")
                // Else
                m_position.Y = value;
            }
        }

        // Y+Height.
        // See Also: MaxY.
        public double Bottom
        {
            get
            {
                if (this.IsRotated)
                    return BottomRight.Y;
                // Q: If height is negative, should this NOT add that negative height, so it returns "largest y"?
                // (Top would need corresponding change.)
                // A: NO, change Min/MaxY to handle that case.
                return m_position.Y + m_size.Height;
            }
            set
            {
                // If Me.IsRotated Then
                // Throw New NotImplementedException("Rectangle2D.Bottom setter")
                // Else
                m_size.Height = value - m_position.Y;
            }
        }

        // For clients who specify that they are looking at y's min/max range.
        // TODO: When rotated, should this be the true MinY of all 4 corners?
        public double MinY
        {
            get
            {
                if (IsRotated)
                    // TODO: When rotated, should this be the true MinY of all 4 corners?
                    TODO();
                else if (Height < 0)
                    // TBD: Will this surprise any callers?
                    return Bottom;
                // 
                return Top;
            }
        }

        public double MaxY
        {
            get
            {
                if (IsRotated)
                    // TODO: When rotated, should this be the true MaxY of all 4 corners?
                    TODO();
                else if (Height < 0)
                    // TBD: Will this surprise any callers?
                    return Top;
                // 
                return Bottom;
            }
        }

        public Point2D TopDelta
        {
            get
            {
                // Delta from Bottom to Top, taking rotation into account.
                return HeightRotated;
            }
        }

        public Point2D RightDelta
        {
            get
            {
                // Delta from Left to Right, taking rotation into account.
                return WidthRotated;
            }
        }

        public Point2D Center
        {
            get
            {
                if (this.IsRotated)
                    return Average(this.TopLeft, this.BottomRight);
                else
                    return new Point2D(m_position.X + m_size.Width / 2, m_position.Y + m_size.Height / 2);
            }
        }

        // CAUTION: Left/Right/Top/Bottom only correct if width&height are non-negative, and Not IsRotated!
        public Point2D TopLeft
        {
            get
            {
                return m_position;    // (Left, Top)
            }
            set
            {
                m_position = value;
            }
        }

        public Point2D TopRight
        {
            get
            {
                if (this.IsRotated)
                    return TopLeft + WidthRotated;
                else
                    return new Point2D(this.Right, this.Top);
            }
        }

        public Point2D BottomLeft
        {
            get
            {
                if (this.IsRotated)
                    return TopLeft + HeightRotated;
                else
                    return new Point2D(this.Left, this.Bottom);
            }
        }

        public Point2D BottomRight
        {
            get
            {
                if (this.IsRotated)
                    return TopLeft + WidthRotated + HeightRotated;
                else
                    return new Point2D(this.Right, this.Bottom);
            }
        }

        // Assume created with New variant that specifies fromZigZag;
        // otherwise defaults to ClockwiseCorners.
        public Point2D[] AsCorners
        {
            get
            {
                if (m_fromZigZag)
                    return this.ZigZagCorners;
                else
                    return this.ClockwiseCorners;
            }
        }

        // "clockwise" order: (Top-Left, Top-Right, Bottom-Right, Bottom-Left).
        // 4-point "polygon" representation of rectangle.
        // CAUTION: Some ExportRegion code must call ZigZagCorners, not this!
        // CAUTION: If negative extents, assumes TopLeft/etc do NOT attempt to compensate for negativity.
        // TopLeft = (X,Y), then BottomRight has extents added (possibly signed).
        public Point2D[] ClockwiseCorners
        {
            get
            {
                return new[] { this.TopLeft, this.TopRight, this.BottomRight, this.BottomLeft };
            }
        }

        // "zig-zag" order: (Top-Left, Top-Right, Bottom-Left, Bottom-Right).
        public Point2D[] ZigZagCorners
        {
            get
            {
                return new[] { this.TopLeft, this.TopRight, this.BottomLeft, this.BottomRight };
            }
        }

        // "zig-zag" order (but switch Top and Bottom): (Bottom-Left, Bottom-Right, Top-Left, Top-Right).
        public Point2D[] ZigZagCorners_YFlip
        {
            get
            {
                return new[] { this.BottomLeft, this.BottomRight, this.TopLeft, this.TopRight };
            }
        }

        public RectangleF ToRectangleF
        {
            get
            {
                // NO, this is not valid for rotated rectangle.
                return new RectangleF(System.Convert.ToSingle(this.X), System.Convert.ToSingle(this.Y), System.Convert.ToSingle(this.Width), System.Convert.ToSingle(this.Height));
            }
        }

        // Return rectangle based on Me, but flipped in Y
        public Rectangle2D YFlip()
        {
            return new Rectangle2D(this.X, this.Y + this.Height, this.Width, -this.Height);
        }

        public override string ToString()
        {
            return "X=" + Round3(m_position.X) + " Y=" + Round3(m_position.Y) +
                    ", W=" + Round3(m_size.Width) + " H=" + Round3(m_size.Height) +
                    ////", Angle=" + Round1(RadiansToDegrees(m_rotationRadians)) +
                    ", BR=(" + Round3(Right) + ", " + Round3(Bottom) + ")";
        }

        // Represent Rectangle2D as Min Point and Max Point.
        // Return maxP; Out minP
        // ASSUMES rectangle always has non-negative width & height (otherwise, min/max may be reversed).
        public Point2D ToMinMax(out Point2D minP)
        {
            if (this.IsRotated)
                throw new NotImplementedException("Rectangle2D.ToMinMax");
            else
            {
                minP = this.TopLeft;
                return this.BottomRight;
            }
        }

        //public MinMaxPoint2D ToMinMax()
        //{
        //    if (this.IsRotated)
        //        throw new NotImplementedException("Rectangle2D.ToMinMax");
        //    else
        //        return MinMaxPoint2D.CreateFromCorners(this.TopLeft, this.BottomRight);
        //}


        //// Return list of points representing Me. xFlip, yFlip swap corresponding coordinate.
        //// zigZag determines order of points.
        //public Point2D[] ToRectAsPoints(bool xFlip, bool yFlip, bool zigZag)
        //{
        //    if (this.IsRotated)
        //    {
        //        // (Top-Left, Top-Right, Bottom-Left, Bottom-Right)
        //        Point2D[] corners = this.ZigZagCorners;

        //        MaybeFlip(corners, xFlip, yFlip);
        //        if (!zigZag)
        //            Swap(ref corners[2], ref corners[3]);

        //        return corners;
        //    }
        //    else
        //    {
        //        double x0 = this.Left;
        //        double x1 = this.Right;
        //        double y0 = this.Top;
        //        double y1 = this.Bottom;

        //        if (xFlip)
        //            Swap(ref x0, ref x1);
        //        if (yFlip)
        //            Swap(ref y0, ref y1);

        //        return MakeRectAsPoints(x0, y0, x1, y1, zigZag, false);
        //    }
        //}

        //// SIDE-EFFECT: Swaps elements of "corners" as needed.
        //// No change if xFlip and yFlip both False.
        //private static void MaybeFlip(Point2D[] corners, bool xFlip, bool yFlip)
        //{
        //    if (xFlip)
        //    {
        //        Swap(corners[0], corners[1]);
        //        Swap(corners[2], corners[3]);
        //    }

        //    if (yFlip)
        //    {
        //        Swap(corners[0], corners[2]);
        //        Swap(corners[1], corners[3]);
        //    }
        //}

        //// Fill pts with representation of Me. xFlip, yFlip swap corresponding coordinate.
        //// zigZag determines order of points.
        //public void ToRectAsPoints(ref Point2D[] pts, bool xFlip, bool yFlip, bool zigZag)
        //{
        //    if (this.IsRotated)
        //        pts = ToRectAsPoints(xFlip, yFlip, zigZag);
        //    else
        //    {
        //        double x0 = this.Left;
        //        double x1 = this.Right;
        //        double y0 = this.Top;
        //        double y1 = this.Bottom;

        //        if (xFlip)
        //            Swap(x0, x1);
        //        if (yFlip)
        //            Swap(y0, y1);

        //        FillRectAsPoints(ref pts, x0, y0, x1, y1, zigZag);
        //    }
        //}


        //// Rectangle formed by intersection of two rectangles.
        //public Rectangle2D Intersection(Rectangle2D other)
        //{
        //    if (this.IsRotated)
        //        throw new NotImplementedException("Rectangle2D.ToRectAsPoints");
        //    else
        //        return mDL2DLib.RectangleIntersection(this, other);
        //}

        //// "Inclusive": If barely touch, returns True (but size of overlap region would be zero).
        //public bool Intersects(Rectangle2D rcdRectangle, bool inclusive = true)
        //{
        //    if (this.IsRotated)
        //        throw new NotImplementedException("Rectangle2D.ToRectAsPoints");
        //    else
        //        return mDL2DLib.RectanglesIntersects2D(this, rcdRectangle, inclusive);
        //}

        //// Expand to include r2. (Union, in the sense of drawing a rectangle that includes both rectangles.)
        //// Should there be optional logic that indicates whether added rectangle overlaps current rectangle?
        //// FOR NOW, expand Me to non-rotated rectangle, then add. TODO: Is there a better algorithm?
        //public Rectangle2D Union(Rectangle2D r2)
        //{
        //    // If the new points are all inside, there is nothing to do.
        //    if (this.Contains(r2))
        //        return this;

        //    if (r2.Contains(this))
        //        return r2;

        //    // FOR NOW, expand Me to non-rotated rectangle, then add. TODO: Is there a better algorithm?
        //    Rectangle2D r1 = MaybeExpandToUnrotatedRectangle(false);

        //    double newLeft = Math.Min(r1.Left, r2.Left);
        //    double newRight = Math.Max(r1.Right, r2.Right);

        //    double newTop = Math.Min(r1.Top, r2.Top);
        //    double newBottom = Math.Max(r1.Bottom, r2.Bottom);

        //    return new Rectangle2D(newLeft, newTop, newRight - newLeft, newBottom - newTop);
        //}


        //public bool Contains(double px, double py)
        //{
        //    return Contains(new Point2D(px, py));
        //}

        //// "_Inclusive": Allowed to touch border.
        //public bool Contains(Point2D point)
        //{
        //    return mDL2DLib.PointInsideRectangle2D_Inclusive(point, this);
        //}

        //public bool Contains_WithTolerance(Point2D point, double tolerance)
        //{
        //    return mDL2DLib.PointInsideRectangle2D_WithTolerance(point, this, tolerance);
        //}

        //// True if "rect2" is completely inside Me (allowed to touch border).
        //public bool Contains(Rectangle2D rect2)
        //{
        //    return Contains(rect2.TopLeft) && Contains(rect2.TopRight) && Contains(rect2.BottomLeft) && Contains(rect2.BottomRight);
        //}


        // If p is outside Me, adjust X and Y to closest edge of Me.
        public Point2D Clamp(Point2D p)
        {
            // --- TODO: Need logic for rotated. For now, we just use the regular logic, which won't correctly move it into rectangle, when rotated. ---
            // (Used indirectly somewhere in new 3D Mode. When setting up water?)

            // If Me.IsRotated Then
            // Throw New NotImplementedException("Rectangle2D.Clamp")

            // Else
            // NOTE: Point2D is Struct, so ok to modify p -- won't modify caller's p.
            if (p.X < this.MinX)
                p.X = this.MinX;
            else if (p.X > this.MaxX)
                p.X = this.MaxX;

            if (p.Y < this.MinY)
                p.Y = this.MinY;
            else if (p.Y > this.MaxY)
                p.Y = this.MaxY;

            return p;
        }

        //// If angle is rotated, expand to unrotated rectangle.
        //// "allowNegativeSize": Preserve negative Width, Height.
        //// To make a "bounding box", use allowNegativeSize=False.
        //public Rectangle2D MaybeExpandToUnrotatedRectangle(bool allowNegativeSize)
        //{
        //    // --- not rotated => no change ---
        //    if (!this.IsRotated)
        //        return this;

        //    // --- rotated => re-calc --
        //    bool xFlip = (this.Width < 0);
        //    bool yFlip = (this.Height < 0);
        //    Point2D maxPt;
        //    Point2D minPt = Calculate_MinMax(this.ClockwiseCorners, ref maxPt);
        //    Rectangle2D newRect = Rectangle2D.FromMinMax(minPt, maxPt);

        //    if (allowNegativeSize)
        //    {
        //        if (xFlip)
        //        {
        //            newRect.X += newRect.Width;   // High x becomes origin.
        //            newRect.Width = -newRect.Width;
        //        }
        //        if (yFlip)
        //        {
        //            newRect.Y += newRect.Height;   // High y becomes origin.
        //            newRect.Height = -newRect.Height;
        //        }
        //    }

        //    return newRect;
        //}

        public Rectangle2D Expand(double margin)
        {
            // TODO: Handle rotated rectangle.
            // TODO: Handle negative height.
            Point2D tl = new Point2D(this.Left - margin, this.Top - margin);
            Point2D br = new Point2D(this.Right + margin, this.Bottom + margin);
            return Rectangle2D.FromOppositeCorners(tl, br);
        }
    }
}
