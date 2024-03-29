﻿using System;
using System.Collections.Generic;
using System.Drawing;
using OU;

namespace Global
{
    public struct Size2D
    {
        public static Size2D Empty = new Size2D(0, 0);


        public double Width;
        public double Height;


        public Size2D(Size size)
        {
            this.Width = size.Width;
            this.Height = size.Height;
        }

        public Size2D(SizeF szfSize)
        {
            this.Width = szfSize.Width;
            this.Height = szfSize.Height;
        }

        /// <summary>
        /// Implicitly has units Distance.DefaultUnits.
        /// </summary>
        /// <param name="point"></param>
        public Size2D(Dist2D point)
        {
            this.Width = point.X.Value;
            this.Height = point.Y.Value;
        }

        public Size2D(double dblWidth, double dblHeight)
        {
            this.Width = dblWidth;
            this.Height = dblHeight;
        }

        public Size2D(DistD x, DistD y)
        {
            this.Width = x.Value;
            this.Height = y.Value;
        }


        // Size of rotated rectangle. Top-Left, Top-Right, Bottom-Left corners.
        public Size2D(Dist2D cornerTL, Dist2D cornerTR, Dist2D cornerBL)
        {
            this.Width = Dist2D.DistanceBetween(cornerTL, cornerTR).Value;
            this.Height = Dist2D.DistanceBetween(cornerTL, cornerBL).Value;
        }

        //// Size of rotated rectangle. "corners" hold 4 corners of rectangle.
        //// "isZigZag" "=True" corner order: TL, TR, BL, BR; "=False" corner order (clockwise): TL, TR, BR, BL.
        //public Size2D(Distance2D[] corners, bool isZigZag = false) : this(corners[CornerIndex.TopLeft], corners[CornerIndex.TopRight], isZigZag ? corners[ZigZagIndex.BottomLeft] : corners[CornerIndex.BottomLeft])
        //{

        //    // Verify isZigZag.  Top-Left to Bottom-Right diagonal should be greater than height.
        //    Distance2D cornerBR = isZigZag ? corners[ZigZagIndex.BottomRight] : corners[CornerIndex.BottomRight];
        //    double diagonalLength = CalcDistance2D(corners[CornerIndex.TopLeft], cornerBR);
        //    if (diagonalLength < this.Height)
        //        throw new InvalidProgramException("Size2D.New from corners -- Incorrect ordering of corners");
        //}


        public override string ToString()
        {
            return string.Format("{0}(W={1}, H={2})", this.GetType().Name, this.Width, this.Height);
        }



        // True if size is zero.
        public bool IsEmpty
        {
            get
            {
                return (Width == 0) || (Height == 0);
            }
        }


        public SizeF ToSizeF()
        {
            SizeF szfSize = new SizeF(System.Convert.ToSingle(this.Width), System.Convert.ToSingle(this.Height));

            return szfSize;
        }

        public Size ToSize()
        {
            Size szeSize = new Size(System.Convert.ToInt32(this.Width), System.Convert.ToInt32(this.Height));

            return szeSize;
        }

        public void ResizeProportionalByWidth(double dblWidth)
        {
            Vec2D ptdFactor = new Vec2D(this.Width / this.Height, this.Height / this.Width);

            this.Width = dblWidth;
            this.Height = dblWidth * ptdFactor.Y;
        }

        public void ResizeProportionalByHeight(double dblHeight)
        {
            Vec2D ptdFactor = new Vec2D(this.Width / this.Height, this.Height / this.Width);

            this.Width = dblHeight * ptdFactor.X;
            this.Height = dblHeight;
        }

        public static bool operator ==(Size2D szd1, Size2D szd2)
        {
            if (szd1.Width == szd2.Width && szd1.Height == szd2.Height)
                return true;
            else
                return false;
        }

        public static bool operator !=(Size2D szd1, Size2D szd2)
        {
            if (szd1.Width == szd2.Width && szd1.Height == szd2.Height)
                return true;
            else
                return false;
        }

        public static Size2D operator *(double scale, Size2D size)
        {
            return new Size2D(scale * size.Width, scale * size.Height);
        }

        public static Size2D operator *(Size2D size, double scale)
        {
            return new Size2D(scale * size.Width, scale * size.Height);
        }

        public static Size2D operator *(Dist2D scale, Size2D size)
        {
            return new Size2D(scale.X * size.Width, scale.Y * size.Height);
        }

        public static Size2D operator /(Size2D sz1, Size2D sz2)
        {
            return new Size2D(sz1.Width / sz2.Width, sz1.Height / sz2.Height);
        }
    }
}
