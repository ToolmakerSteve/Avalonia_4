using System;
using System.Collections.Generic;
using Urho;
using static System.Math;

namespace Global
{
    /// <summary>
    /// Math functions.  TODO: Move more function TO here FROM Utils.Misc.cs.
    /// TODO: Change function names to PascalCase (capitalize first letter).
    /// </summary>
    public static partial class Utils
    {

        public const long NANO_L = 1000000000L;
        public const double NANO_D = 1000000000.0;
        public const float NANO_F = 1000000000.0f;
        public const int MILLI = 1000;
        public const long MILLI_L = 1000L;
        public const double MILLI_D = 1000.0;
        public const float MILLI_F = 1000.0f;

        // NOTE: These are factors of 1024.
        // This is not consistent with international standards, but is traditional in computer software.
        // (Standardized terms are "Kibi" / "Ki", and "Mebi" / "Mi".)
        public const int KILO = 1024;
        public const float KILO_F = 1024.0f;
        public const int MEGA = KILO * KILO;
        public const float MEGA_F = KILO_F * KILO_F;
        // NOTE: These are factors of 1000.
        public const int KILO1 = 1000;
        public const float KILO1_F = 1000.0f;
        public const int MEGA1 = KILO1 * KILO1;
        public const float MEGA1_F = KILO1_F * KILO1_F;

        public const double NearZero = 0.00000012;
        public const double OneThousandth = 0.001;
        public const double OneMillionth = 0.000001;
        public const double VerySmall = 0.00001;
        public const float VerySmallF = 0.00001f;


        //        #region -- random --
        //        static Random rand = new Random();

        //        /// <summary>
        //        /// Returns "true" with likeliness "probability".
        //        /// "probability = 0" => always false.
        //        /// "probability = 1" => always true.
        //        /// "probability = 0.5" => true half the time.
        //        /// </summary>
        //        /// <param name="probability"></param>
        //        /// <returns></returns>
        //        public static bool ProbabilityTrue(double probability)
        //        {
        //            lock (rand)
        //                return rand.NextDouble() <= probability;
        //        }

        //        public static int RandomNextInt(int limit)
        //        {
        //            lock (rand)
        //                return rand.Next(limit);
        //        }
        //        #endregion


        //        // ========== isBad, clamp, nearlyEquals ==========

        //        public static bool isNonNegative(this float val)
        //        {
        //            return (val.isGood() && (val >= 0.0));
        //        }

        //        public static bool isNonNegative(this double val)
        //        {
        //            return (val.isGood() && (val >= 0.0));
        //        }

        //        public static bool isGood(this double val)
        //        {
        //            return !val.isBadOrMarker();
        //        }

        //        public static bool isGood(this float val)
        //        {
        //            return !val.isBadOrMarker();
        //        }

        //        public static bool isBad_IgnoreMarker(this double val)
        //        {
        //            return (double.IsNaN(val) || double.IsInfinity(val));
        //        }

        //        public static bool isBadOrMarker(this double val)
        //        {
        //            return (double.IsNaN(val) || double.IsInfinity(val) || (val == double.MaxValue) || (val == double.MinValue));
        //        }

        //        public static bool isBadOrMarker(this float val)
        //        {
        //            return (float.IsNaN(val) || float.IsInfinity(val) || (val == float.MaxValue) || (val == float.MinValue));
        //        }

        //        public static bool isMarker(this int val)
        //        {
        //            return ((val == int.MaxValue) || (val == int.MinValue));
        //        }


        //        /// <summary> Result is never less than "min" nor greater than "max" (is "clamped to range min..max").
        //        /// </summary>
        //		public static double clamp(double val, double min, double max)
        //        {
        //            if (val < min)
        //                return min;
        //            if (val > max)
        //                return max;
        //            return val;
        //        }

        //        /// <summary> Result is never less than "min" nor greater than "max" (is "clamped to range min..max").
        //        /// </summary>
        //		public static float clamp(float val, float min, float max)
        //        {
        //            if (val < min)
        //                return min;
        //            if (val > max)
        //                return max;
        //            return val;
        //        }

        //        /// <summary> Result is never less than "min" nor greater than "max" (is "clamped to range min..max").
        //        /// </summary>
        //		public static long clamp(long val, long min, long max)
        //        {
        //            if (val < min)
        //                return min;
        //            if (val > max)
        //                return max;
        //            return val;
        //        }

        //        /// <summary> Result is never less than "min" nor greater than "max" (is "clamped to range min..max").
        //        /// </summary>
        //		public static int clamp(int val, int min, int max)
        //        {
        //            if (val < min)
        //                return min;
        //            if (val > max)
        //                return max;
        //            return val;
        //        }


        //        /// <summary> Result is never less than "min" (is "clamped to a minimum of min").
        //        /// </summary>
        //		public static double clampMin(double val, double min)
        //        {
        //            if (val < min)
        //                return min;
        //            return val;
        //        }

        //        /// <summary> Result is never less than "min" (is "clamped to a minimum of min").
        //        /// </summary>
        //		public static float clampMin(float val, float min)
        //        {
        //            if (val < min)
        //                return min;
        //            return val;
        //        }

        //        /// <summary> Result is never less than "min" (is "clamped to a minimum of min").
        //        /// </summary>
        //		public static int clampMin(int val, int min)
        //        {
        //            if (val < min)
        //                return min;
        //            return val;
        //        }


        //        /// <summary> Result is never greater than "max" (is "clamped to a maximum of max").
        //        /// </summary>
        //		public static double clampMax(double val, double max)
        //        {
        //            if (val > max)
        //                return max;
        //            return val;
        //        }

        //        /// <summary> Result is never greater than "max" (is "clamped to a maximum of max").
        //        /// </summary>
        //		public static float clampMax(float val, float max)
        //        {
        //            if (val > max)
        //                return max;
        //            return val;
        //        }

        //        /// <summary> Result is never greater than "max" (is "clamped to a maximum of max").
        //        /// </summary>
        //		public static int clampMax(int val, int max)
        //        {
        //            if (val > max)
        //                return max;
        //            return val;
        //        }


        //        public static double max(double a, double b, double c)
        //        {
        //            return Math.Max(Math.Max(a, b), c);
        //        }

        //        public static float max(float a, float b, float c)
        //        {
        //            return Math.Max(Math.Max(a, b), c);
        //        }

        //        public static int max(int a, int b, int c)
        //        {
        //            return Math.Max(Math.Max(a, b), c);
        //        }


        /// <summary> CAUTION: Uses an absolute tolerance, not a scaled error term,
        /// so be careful, when values might be large, that tolerance is large enough
        /// to be distinguishable when added to one of the values. (More of an issue for Single than for Double.)
        /// Single math can easily exceed NearZero error, and after several steps might exceed NearZeroX10,
        /// so using VerySmallF as a (lenient) toleranceFraction.
        /// </summary>
        public static bool NearlyEquals(this float number, float target, float tolerance = VerySmallF)
        {
            return (Math.Abs(number - target) <= tolerance);
        }

        /// <summary> CAUTION: Uses an absolute tolerance, not a scaled error term,
        /// so be careful, when values might be large, that tolerance is large enough
        /// to be distinguishable when added to one of the values. (More of an issue for Single than for Double.)
        /// </summary>
        public static bool NearlyEquals(this double number, double target, double tolerance = EpsilonForOne)
        {
            return (Math.Abs(number - target) <= tolerance);
        }

        public static bool NearlyEquals(this Distance number, Distance target, double tolerance = EpsilonForOne)
        {
            return (Math.Abs(number.Value - target.Value) <= tolerance);
        }

        public static bool NearlyEquals(this Distance2D p1, Distance2D p2, double tolerance = EpsilonForOne)
        {
            return (NearlyEquals(p1.X, p2.X, tolerance) && NearlyEquals(p1.Y, p2.Y, tolerance));
        }

        public static bool NearlyEquals(this System.Drawing.PointF p1, System.Drawing.PointF p2, float tolerance)
        {
            return (p1.X.NearlyEquals(p2.X, tolerance) && p1.Y.NearlyEquals(p2.Y, tolerance));
        }
        // TBD: If make this a compiler extension AND name it "NearlyEquals",
        // then Poly2Tri.TriangulationPoint gets compiler error, because it can't resolve Vector3,
        // to determine which "NearlyEquals" is most applicable.
        public static bool NearlyEquals3(this Vector3 p1, Vector3 p2, float tolerance = (float)NearZeroX10)
        {
            return (p1.X.NearlyEquals(p2.X, tolerance) && p1.Y.NearlyEquals(p2.Y, tolerance) && p1.Z.NearlyEquals(p2.Z, tolerance));
        }


        //        // ========== Miscellaneous calculations ==========

        //        // TODO: May overflow.
        //        public static int nextPowerOf2(int v)
        //        {
        //            // Ignoring overflow.
        //            return (int)nextPowerOf2((uint)v);
        //        }

        //        public static uint nextPowerOf2(uint v)
        //        {
        //            // Works for 32-bit unsigned integer.
        //            v--;
        //            v |= v >> 1;
        //            v |= v >> 2;
        //            v |= v >> 4;
        //            v |= v >> 8;
        //            v |= v >> 16;
        //            v++;

        //            return v;
        //        }


        //        public static int FractionToPercentInt(this float frac)
        //        {
        //            return FractionToPercent(frac).RoundToInt();
        //        }

        //        public static float FractionToPercent(this float frac)
        //        {
        //            return frac * 100.0f;
        //        }

        //        public static float PercentToFraction(this float percent)
        //        {
        //            return percent / 100.0f;
        //        }

        //        public static float PercentToFraction(this int percent)
        //        {
        //            return percent / 100.0f;
        //        }


        //        // Given a value in range (-(TotalNumber-1)..0..whatever),
        //        // Return a value in range (1..TotalNumber),
        //        // wrapping at ends of range.
        //        // If "value" might be <= -TotalNumber, need a formula with two "%"s, to get in range.
        //        //
        //        public static int Wrap1ToN(int value, int TotalNumber)
        //        {
        //            // "+ TotalNumber" shifts inputs in (-(m-1)..0) to (1..m),
        //            // so after "- 1", is still >= 0:  C# "%" doesn't do what we want with negative inputs.
        //            // "1 + (v-1)%m" shifts output range from (0..m-1) to (1..m).
        //            return (1 + ((value + TotalNumber - 1) % TotalNumber));

        //            //			// I think this is the formula needed, if value can be any negative number.
        //            //			//   "((value - 1) % TotalNumber" is >= -TotalNumber, so
        //            //			//   that +TotalNumber is >= 0,
        //            //			//   so final answer is >= 1.
        //            //			return (1 + (((value - 1) % TotalNumber + TotalNumber) % TotalNumber));
        //        }


        //        // ========== AccumMinMax ==========
        //        // Before first call, caller must initialize minV to Integer.MaxValue, and maxV to Integer.MinValue.
        //        public static void AccumMinMax(int v, ref int minV, ref int maxV)
        //        {
        //            if (v < minV)
        //                minV = v;
        //            if (v > maxV)
        //                maxV = v;
        //        }
        //        // Before first call, caller must initialize minV to Single.MaxValue, and maxV to Single.MinValue.
        //        public static void AccumMinMax(float v, ref float minV, ref float maxV)
        //        {
        //            if (v < minV)
        //                minV = v;
        //            if (v > maxV)
        //                maxV = v;
        //        }

        //        // Before first call, caller must initialize minV to Double.MaxValue, and maxV to Double.MinValue.
        //        public static void AccumMinMax(double v, ref double minV, ref double maxV)
        //        {
        //            if (v < minV)
        //                minV = v;
        //            if (v > maxV)
        //                maxV = v;
        //        }


        // ========== Degrees and Radians - "double" ==========

        public static readonly double OneDegreeAsRadiansD = (Math.PI / 180.0);
        public static readonly double OneRadianAsDegreesD = (180.0 / Math.PI);
        public static readonly float OneDegreeAsRadiansF = ((float)Math.PI / 180.0f);
        public static readonly float OneRadianAsDegreesF = (180.0f / (float)Math.PI);

        public static double radiansToDegrees(double radians)
        {
            return OneRadianAsDegreesD * radians;
        }

        public static double degreesToRadians(double degrees)
        {
            return OneDegreeAsRadiansD * degrees;
        }

        public static float degreesToRadians(float degrees)
        {
            return OneDegreeAsRadiansF * degrees;
        }

        //        public static double getAngleDegrees(Distance2D origin, Distance2D aimPt)
        //        {
        //            return getAngleDegrees(origin.X.Value, origin.Y.Value, aimPt.X.Value, aimPt.Y.Value);
        //        }

        //        public static double getAngleDegrees(double originX, double originY, double aimPtX, double aimPtY)
        //        {
        //            double angle = Math.Atan2(aimPtY - originY, aimPtX - originX) * (180 / Math.PI);

        //            if (angle < 0)
        //                angle += 360.0;

        //            return angle;
        //        }


        //        // ========== Degrees and Radians - "float" for OpenGL ==========

        //        // OpenGL works in floats. We could make these double for internal accuracy,
        //        // and then convert to float at a later step.
        //        public static float asDegrees(float radians)
        //        {
        //            return (float)(OneRadianAsDegreesF * radians);
        //        }

        //        public static float asRadians(float degrees)
        //        {
        //            return (float)(OneDegreeAsRadiansF * degrees);
        //        }


        //        // ========== Headings (rotation & directions) ==========

        //        // Given start and end directions in XY, return rotation in radians that
        //        // will move from start to end.
        //        public static float headingChangeAsRotationRadians2D(Unitless2D startHeading, Unitless2D endHeading)
        //        {
        //            float rotationRadians = headingAsAngleRadians2D(endHeading) - headingAsAngleRadians2D(startHeading);
        //            return rotationRadians;
        //        }

        //        // Atan2 implicitly makes some direction "angle 0".
        //        // Is that "0" direction (1, 0)?
        //        public static float headingAsAngleRadians2D(Distance2D heading)
        //        {
        //            return headingAsAngleRadians2D(new Unitless2D(heading));
        //        }

        //        // Atan2 implicitly makes some direction "angle 0".
        //        // Is that "0" direction (1, 0)?
        //        public static float headingAsAngleRadians2D(Unitless2D heading)
        //        {
        //            float angleRadians = (float)Math.Atan2(heading.Y, heading.X);
        //            return angleRadians;
        //        }

        //        //public static void TEST_rotation()
        //        //{
        //        //    TEST_rotation1(new Distance2D(2, 1));
        //        //    TEST_rotation1(new Distance2D(2, -1));
        //        //    TEST_rotation1(new Distance2D(-2, 1));
        //        //    TEST_rotation1(new Distance2D(-2, -1));
        //        //    TEST_rotation1(new Distance2D(1, 2));

        //        //       void TEST_rotation1(Distance2D heading)
        //        //    {
        //        //        var landing = new Distance2D(0.5, 0.5) + heading;
        //        //        var landingDelta = landing - heading;
        //        //        var radians = headingAsAngleRadians2D(heading);
        //        //        var rotatedA = rotateByRadians(heading, -radians);
        //        //        var rotatedLandingDelta = rotateByRadians(landingDelta, -radians);
        //        //    }
        //        //}

        //        //internal static void TEST_LatLongPrecision()
        //        //{
        //        //	double v1 = 180.0;
        //        //	double v2 = v1;
        //        //	float f1 = (float)v1;
        //        //	float f2 = f1;
        //        //	int state = 1;
        //        //	for (double v = 180.0; v > 100; v -= 0.0000001) {
        //        //		float f = (float)v;
        //        //		if (f != f1) {
        //        //			// Found a "transition" value.
        //        //			switch (state) {
        //        //			case 1:
        //        //				// Found first "transition" value.
        //        //				state = 2;
        //        //				f1 = f;
        //        //				v1 = v;
        //        //				break;
        //        //			case 2:
        //        //				state = 3;
        //        //				f2 = f;
        //        //				v2 = v;
        //        //				break;
        //        //			}
        //        //		}

        //        //		if (state > 2)
        //        //			break;
        //        //	}
        //        //	// ANALYSIS: Not quite distinguished 5 decimal digits to right of decimal point.
        //        //	//   (To distinguish 5 digits, abs delta would be < 1.0 E-05)
        //        //	double deltaV = v2 - v1;            // -1.51999990976037E-05
        //        //	double deltaF = f2 - (double)f1;    // -1.52587890625E-05

        //        //	// To see all the digits in debugger or print out.
        //        //	double f1_exact = f1;               // 179.999984741211
        //        //	double f2_exact = f2;               // 179.999969482422

        //        //	// ANALYSIS: If attempt to distinguish 6 digits, can't.
        //        //	// Distinguishes ~179.999985 from ~179.999969
        //        //	// And the 5th digit isn't distinguishable in all cases.
        //        //}



        //        #region -- lat/long in signed 4-byte int with ~1 cm accuracy (+-179.1234567; 7 digits to right of decimal) --

        //        // This gives 7 digits to right of decimal. Can't do more, without overflowing signed 4-byte integer when above +-128 longitude.
        //        const double Fixed7Mult = 10000000;

        //        public static int DecimalDegreesToFixed7(double degrees)
        //        {
        //            return RoundToInt(degrees * Fixed7Mult);
        //        }

        //        public static double Fixed7ToDecimalDegrees(int fixed7)
        //        {
        //            return fixed7 / (double)Fixed7Mult;
        //        }


        //        // ----- TESTS -----
        //        ///// <summary>
        //        ///// This test (barely) fails in 7th digit to right of decimal point (0.0000001 as delta).
        //        ///// Passes with 0.0000002 as delta.
        //        ///// </summary>
        //        //internal static void TEST2A_LatLongPrecision()
        //        //{
        //        //	//VERY_SLOW_TEST Test2A_ForRange(-180, 360, 0.0000001);
        //        //	//FAILS Test2A_ForRange(-180, 0.1, 0.0000001);

        //        //	Test2A_ForRange(-180, 0.1, 0.0000002);
        //        //	Test2A_ForRange(0, 0.1, 0.0000002);
        //        //	Test2A_ForRange(179.9, 0.1, 0.0000002);
        //        //}

        //        ///// <summary>
        //        ///// Test for the smallest difference.  A: 9.9999994E-08.
        //        ///// </summary>
        //        //internal static void TEST2B_LatLongPrecision()
        //        //{
        //        //	double minDelta = double.MaxValue;
        //        //	double vAtMinDelta = 0;
        //        //	//VERY_SLOW_TEST Test2B_ForRange(-180, 360, ref minDelta, ref vAtMinDelta);
        //        //	Test2B_ForRange(-180, 0.1, ref minDelta, ref vAtMinDelta);
        //        //	Test2B_ForRange(0, 0.1, ref minDelta, ref vAtMinDelta);
        //        //	Test2B_ForRange(179.9, 0.1, ref minDelta, ref vAtMinDelta);

        //        //	// Fails. Smallest delta is 9.9999994E-08; due to slight rounding error in 7th decimal digit.
        //        //	//if (minDelta < 0.0000001)
        //        //	//	throw new InvalidProgramException($"Fixed7 has less than 7 decimal digits near {vAtMinDelta}");

        //        //	// Passes.
        //        //	if (minDelta < 0.000000099)
        //        //		throw new InvalidProgramException($"Fixed7 has less than 7 decimal digits near {vAtMinDelta}");
        //        //}

        //        //private static void Test2A_ForRange(double minV, double range, double deltaV)
        //        //{
        //        //	double prevV = 0;
        //        //	int prevFixed7 = 0;
        //        //	bool firstTime = true;
        //        //	double maxV = minV + range;
        //        //	for (double v = minV; v <= maxV; v += deltaV) {
        //        //		int fixed7 = DecimalDegreesToFixed7(v);
        //        //		if (firstTime)
        //        //			firstTime = false;
        //        //		else {
        //        //			// Check for failure to distinguish two values that differ only in 7th decimal digit.
        //        //			// Fails.
        //        //			if (fixed7 == prevFixed7)
        //        //				throw new InvalidProgramException($"Fixed7 doesn't distinguish between {prevV} and {v}");
        //        //		}
        //        //		prevV = v;
        //        //		prevFixed7 = fixed7;
        //        //	}
        //        //}

        //        //private static void Test2B_ForRange(double minV, double range, ref double minDelta, ref double vAtMinDelta)
        //        //{
        //        //	int minFixed7 = DecimalDegreesToFixed7(minV);
        //        //	int maxFixed7 = DecimalDegreesToFixed7(minV + range);

        //        //	bool firstTime = true;
        //        //	double prevV = 0;   // Initial value is ignored.
        //        //	for (int fixed7 = minFixed7; fixed7 < maxFixed7; fixed7++) {
        //        //		double v = Fixed7ToDecimalDegrees(fixed7);
        //        //		if (firstTime)
        //        //			firstTime = false;
        //        //		else {
        //        //			double delta = Math.Abs(v - prevV);
        //        //			if (delta < minDelta) {
        //        //				minDelta = delta;
        //        //				vAtMinDelta = v;
        //        //			}
        //        //		}
        //        //		prevV = v;
        //        //	}
        //        //}
        //        #endregion



        //        public static Unitless2D angleRadiansAsHeading(float angleRadians)
        //        {
        //            Unitless2D heading = new Unitless2D(Math.Cos(angleRadians), Math.Sin(angleRadians));

        //            // Verify ~ "angleRadians".
        //            float verifyAngle = headingAsAngleRadians2D(heading);

        //            return heading;
        //        }

        //        // vector "heading" must have length 1.
        //        public static Distance2D moveOnHeading(Distance2D origin, Distance2D heading, double distance)
        //        {
        //            return new Distance2D(origin.X + distance * heading.X, origin.Y + distance * heading.Y);
        //        }

        //        public static Distance2D moveOnAngleRadians(Distance2D origin, float angleRadians, double distance)
        //        {
        //            Distance2D heading = angleRadiansAsHeading(angleRadians);
        //            return moveOnHeading(origin, heading, distance);
        //        }

        //        // Ignores origin.Z.
        //        public static Distance2D moveOnAngleRadians(Distance3D origin, float angleRadians, double distance)
        //        {
        //            Distance2D heading = angleRadiansAsHeading(angleRadians);
        //            return moveOnHeading(new Distance2D(origin.X, origin.Y), heading, distance);
        //        }


        //        // ========== absDelta, distance ==========

        //        public static Distance2D absDelta(Distance2D p1, Distance2D p2)
        //        {
        //            double dX = p2.X - p1.X;
        //            double dY = p2.Y - p1.Y;

        //            return new Distance2D(Math.Abs(dX), Math.Abs(dY));
        //        }


        //        public static double distance(double p1x, double p1y, double p2x, double p2y)
        //        {
        //            double dX = p2x - p1x;
        //            double dY = p2y - p1y;

        //            return (double)Math.Sqrt((double)(dX * dX) + (double)(dY * dY));
        //        }

        //        public static double distance(Distance2D p1, Distance2D p2)
        //        {
        //            double dX = p2.X - p1.X;
        //            double dY = p2.Y - p1.Y;

        //            return (double)Math.Sqrt((double)(dX * dX) + (double)(dY * dY));
        //        }

        //        // https://en.wikibooks.org/wiki/Algorithms/Distance_approximations
        //        // Useful for "threshold" algorithms - DO NOT use this to COMPARE distances.
        //        public static double distance_approx(Distance2D p1, Distance2D p2)
        //        {
        //            double absDX = Math.Abs(p2.X - p1.X);
        //            double absDY = Math.Abs(p2.Y - p1.Y);
        //            if (absDY >= absDX)
        //                return 0.941246 * absDY + 0.41 * absDX;
        //            else
        //                return 0.941246 * absDX + 0.41 * absDY;
        //        }

        //        public static double distance2D(Distance3D p1, Distance3D p2)
        //        {
        //            double dX = p2.X - p1.X;
        //            double dY = p2.Y - p1.Y;

        //            return (double)Math.Sqrt((double)(dX * dX) + (double)(dY * dY));
        //        }

        //        public static double distanceSquared(double p1x, double p1y, double p2x, double p2y)
        //        {
        //            double dX = p2x - p1x;
        //            double dY = p2y - p1y;

        //            return (double)(dX * dX) + (double)(dY * dY);
        //        }

        //        public static double distanceSquared(Distance2D p1, Distance2D p2)
        //        {
        //            double dX = p2.X - p1.X;
        //            double dY = p2.Y - p1.Y;

        //            return (double)(dX * dX) + (double)(dY * dY);
        //        }

        //        public static double distanceSquared(Distance3D p1, Distance3D p2)
        //        {
        //            double dX = p2.X - p1.X;
        //            double dY = p2.Y - p1.Y;

        //            return (double)(dX * dX) + (double)(dY * dY);
        //        }

        //        // "scale" gives separate strength to x and y.
        //        public static double distanceSquared_approx(Distance2D p1, Distance2D p2, Distance2D scale)
        //        {
        //            double dX = scale.X * (p2.X - p1.X);
        //            double dY = scale.Y * (p2.Y - p1.Y);

        //            return (double)(dX * dX) + (double)(dY * dY);
        //        }



        //        // x=longitude, y= latitude.
        //        // aka GeoDistance;
        //        public static double calculateDistanceDD_AED(Distance2D p1, Distance2D p2)
        //        {
        //            return calculateDistanceDD_AED(p1.X, p1.Y, p2.X, p2.Y);
        //        }

        //        // x=longitude, y= latitude.
        //        public static double calculateDistanceDD_AED(Distance3D p1, Distance3D p2, bool approximate = false)
        //        {
        //            if (p1 == null || p2 == null)
        //                return double.MaxValue;
        //            return calculateDistanceDD_AED(p1.X, p1.Y, p2.X, p2.Y, approximate);
        //        }

        //        // x=longitude, y= latitude. oblate spheroid formula. TODO: This code or formula is from where?
        //        public static double calculateDistanceDD_AED(double lon1Xdeg, double lat1Ydeg, double lon2Xdeg, double lat2Ydeg,
        //                                                      bool approximate = false)
        //        {
        //            if (approximate)
        //            {
        //                // TBD: If comparing DISTANCES, is Haversine less susceptible to changes in direction?
        //                //   That is, are there uses where we should use Haversine instead, even for small distances?
        //                double approxDistance = ApproxDistanceGeo_PythagoreanCosLatitude(lon1Xdeg, lat1Ydeg, lon2Xdeg, lat2Ydeg);
        //                // For distance > 1000 m, use Haversine.
        //                if (approxDistance > 1000)
        //                    return HaversineApproxDistanceGeo(lon1Xdeg, lat1Ydeg, lon2Xdeg, lat2Ydeg);
        //                else
        //                    return approxDistance;
        //            }

        //            double c_dblEarthRadius = 6378.135; // km
        //            double c_dblFlattening = 1.0 / 298.257223563; // WGS84 inverse
        //                                                          // flattening
        //                                                          // Q: Why "-" for longitudes??
        //            double p1x = -degreesToRadians(lon1Xdeg);
        //            double p1y = degreesToRadians(lat1Ydeg);
        //            double p2x = -degreesToRadians(lon2Xdeg);
        //            double p2y = degreesToRadians(lat2Ydeg);

        //            double F = (p1y + p2y) / 2;
        //            double G = (p1y - p2y) / 2;
        //            double L = (p1x - p2x) / 2;

        //            double sing = Math.Sin(G);
        //            double cosl = Math.Cos(L);
        //            double cosf = Math.Cos(F);
        //            double sinl = Math.Sin(L);
        //            double sinf = Math.Sin(F);
        //            double cosg = Math.Cos(G);

        //            double S = sing * sing * cosl * cosl + cosf * cosf * sinl * sinl;
        //            double C = cosg * cosg * cosl * cosl + sinf * sinf * sinl * sinl;
        //            double W = Math.Atan2(Math.Sqrt(S), Math.Sqrt(C));
        //            if (W == 0.0)
        //                return 0.0;

        //            //if (W.isBadOrMarker())
        //            //	Dubious();

        //            double R = Math.Sqrt((S * C)) / W;
        //            double H1 = (3 * R - 1.0) / (2.0 * C);
        //            double H2 = (3 * R + 1.0) / (2.0 * S);
        //            double D = 2 * W * c_dblEarthRadius;

        //            // Apply flattening factor
        //            D = D * (1.0 + c_dblFlattening * H1 * sinf * sinf * cosg * cosg - c_dblFlattening * H2 * cosf * cosf * sing * sing);

        //            // Transform to meters
        //            D = D * 1000.0;
        //            if (D.isBadOrMarker())
        //                Dubious();

        //            // tmstest
        //            if (false)
        //            {
        //                // Compare Haversine.
        //                double haversine = HaversineApproxDistanceGeo(lon1Xdeg, lat1Ydeg, lon2Xdeg, lat2Ydeg);
        //                double error = haversine - D;
        //                double absError = Math.Abs(error);
        //                double errorRatio = absError / D;
        //                if (errorRatio > t_maxHaversineErrorRatio)
        //                {
        //                    if (errorRatio > t_maxHaversineErrorRatio * 1.1)
        //                        Test();
        //                    t_maxHaversineErrorRatio = errorRatio;
        //                }

        //                // Compare Polar Coordinate Flat Earth. 
        //                double polarDistanceGeo = ApproxDistanceGeo_Polar(lon1Xdeg, lat1Ydeg, lon2Xdeg, lat2Ydeg, D);
        //                double error2 = polarDistanceGeo - D;
        //                double absError2 = Math.Abs(error2);
        //                double errorRatio2 = absError2 / D;
        //                if (errorRatio2 > t_maxPolarErrorRatio)
        //                {
        //                    if (polarDistanceGeo > 0)
        //                    {
        //                        if (errorRatio2 > t_maxPolarErrorRatio * 1.1)
        //                            Test();
        //                        t_maxPolarErrorRatio = errorRatio2;
        //                    }
        //                    else
        //                        Dubious();
        //                }

        //                // Compare Pythagorean Theorem with Latitude Adjustment. 
        //                double pythagoreanDistanceGeo = ApproxDistanceGeo_PythagoreanCosLatitude(lon1Xdeg, lat1Ydeg, lon2Xdeg, lat2Ydeg, D);
        //                double error3 = pythagoreanDistanceGeo - D;
        //                double absError3 = Math.Abs(error3);
        //                double errorRatio3 = absError3 / D;
        //                if (errorRatio3 > t_maxPythagoreanErrorRatio)
        //                {
        //                    if (D < 2000)
        //                    {
        //                        if (errorRatio3 > t_maxPythagoreanErrorRatio * 1.05)
        //                            Test();
        //                        t_maxPythagoreanErrorRatio = errorRatio3;
        //                    }
        //                }
        //            }


        //            return D;
        //        }

        //        // As a fraction of the distance.
        //        private static double t_maxHaversineErrorRatio, t_maxPolarErrorRatio, t_maxPythagoreanErrorRatio;


        //        public static float calculateDistanceDD_AED(float p1x, float p1y, float p2x, float p2y)
        //        {
        //            float c_dblEarthRadius = 6378.135f; // km
        //            float c_dblFlattening = 1.0f / 298.257223563f; // WGS84 inverse
        //                                                           // flattening

        //            p1x = -degreesToRadians(p1x);
        //            p1y = degreesToRadians(p1y);
        //            p2x = -degreesToRadians(p2x);
        //            p2y = degreesToRadians(p2y);

        //            double F = (p1y + p2y) / 2;
        //            double G = (p1y - p2y) / 2;
        //            double L = (p1x - p2x) / 2;

        //            double sing = Math.Sin(G);
        //            double cosl = Math.Cos(L);
        //            double cosf = Math.Cos(F);
        //            double sinl = Math.Sin(L);
        //            double sinf = Math.Sin(F);
        //            double cosg = Math.Cos(G);

        //            double S = sing * sing * cosl * cosl + cosf * cosf * sinl * sinl;
        //            double C = cosg * cosg * cosl * cosl + sinf * sinf * sinl * sinl;
        //            double W = Math.Atan2(Math.Sqrt(S), Math.Sqrt(C));
        //            if (W == 0.0)
        //                return 0.0f;

        //            double R = Math.Sqrt((S * C)) / W;
        //            double H1 = (3 * R - 1.0) / (2.0 * C);
        //            double H2 = (3 * R + 1.0) / (2.0 * S);
        //            double D = 2 * W * c_dblEarthRadius;

        //            // Apply flattening factor
        //            D = D * (1.0 + c_dblFlattening * H1 * sinf * sinf * cosg * cosg - c_dblFlattening * H2 * cosf * cosf * sing * sing);

        //            // Transform to meters
        //            D = D * 1000.0;
        //            return (float)D;
        //        }



        //        // Average of equatorial and polar radii (meters).
        //        public const double EarthAvgRadius = 6371000;
        //        public const double EarthAvgCircumference = EarthAvgRadius * 2 * Math.PI;
        //        // CAUTION: This is an average of great circles; won't be the actual distance of any longitude or latitude degree.
        //        public const double EarthAvgMeterPerGreatCircleDegree = EarthAvgCircumference / 360;

        //        // Haversine formula (assumes Earth is sphere).
        //        // "deg" = degrees.
        //        // Perhaps based on Haversine Formula in https://cs.nyu.edu/visual/home/proj/tiger/gisfaq.html
        //        public static double HaversineApproxDistanceGeo(double lon1Xdeg, double lat1Ydeg, double lon2Xdeg, double lat2Ydeg)
        //        {
        //            double lon1 = degreesToRadians(lon1Xdeg);
        //            double lat1 = degreesToRadians(lat1Ydeg);
        //            double lon2 = degreesToRadians(lon2Xdeg);
        //            double lat2 = degreesToRadians(lat2Ydeg);

        //            double dlon = lon2 - lon1;
        //            double dlat = lat2 - lat1;
        //            double sinDLat2 = Sin(dlat / 2);
        //            double sinDLon2 = Sin(dlon / 2);
        //            double a = sinDLat2 * sinDLat2 + Cos(lat1) * Cos(lat2) * sinDLon2 * sinDLon2;
        //            double c = 2 * Atan2(Sqrt(a), Sqrt(1 - a));
        //            double d = EarthAvgRadius * c;
        //            return d;
        //        }

        //        // From https://stackoverflow.com/a/19772119/199364
        //        // Based on Polar Coordinate Flat Earth in https://cs.nyu.edu/visual/home/proj/tiger/gisfaq.html
        //        // RATIONALE: profiling shows substantial time spent in calculateDistanceDD_AED, when deciding if near any green bunker.
        //        // (OR Can we take some "bounding box" approach, but with geo coords? How enlarge such a box by "n" meters?)
        //        // (OR should we convert all geo coords to a local Cartesian coord system, around center of site?)
        //        public static double ApproxDistanceGeo_Polar(double lon1deg, double lat1deg, double lon2deg, double lat2deg, double D = 0)
        //        {
        //            double approxUnitDistSq = ApproxUnitDistSq_Polar(lon1deg, lat1deg, lon2deg, lat2deg, D);
        //            double c = Sqrt(approxUnitDistSq);
        //            return EarthAvgRadius * c;
        //        }

        //        // Might be useful to avoid taking Sqrt, when comparing to some threshold.
        //        // Threshold would have to be adjusted to match:  Power(threshold / EarthAvgRadius, 2)
        //        private static double ApproxUnitDistSq_Polar(double lon1deg, double lat1deg, double lon2deg, double lat2deg, double D = 0)
        //        {
        //            const double HalfPi = PI / 2; //1.5707963267949;

        //            double lon1 = degreesToRadians(lon1deg);
        //            double lat1 = degreesToRadians(lat1deg);
        //            double lon2 = degreesToRadians(lon2deg);
        //            double lat2 = degreesToRadians(lat2deg);

        //            double a = HalfPi - lat1;
        //            double b = HalfPi - lat2;
        //            double u = a * a + b * b;
        //            double dlon21 = lon2 - lon1;
        //            double cosDeltaLon = Cos(dlon21);
        //            double v = -2 * a * b * cosDeltaLon;
        //            // TBD: Is "Abs" necessary?  That is, is "u + v" ever negative?
        //            //   (I think not; "v" looks like a secondary term. Though might be round-off issue near zero when a~=b.)
        //            double approxUnitDistSq = Abs(u + v);

        //            //if (approxUnitDistSq.nearlyEquals(0, 1E-16))
        //            //	Dubious();
        //            //else if (D > 0)
        //            //{
        //            //	double dba = b - a;
        //            //	double unitD = D / EarthAvgRadius;
        //            //	double unitDSq = unitD * unitD;
        //            //	if (approxUnitDistSq > 2 * unitDSq)
        //            //		Dubious();
        //            //	else if (approxUnitDistSq * 2 < unitDSq)
        //            //		Dubious();
        //            //}

        //            return approxUnitDistSq;
        //        }

        //        // Pythagorean Theorem with Latitude Adjustment - from Ewan Todd - https://stackoverflow.com/a/1664836/199364
        //        // Refined by ToolmakerSteve - https://stackoverflow.com/a/53468745/199364
        //        public static double ApproxDistanceGeo_PythagoreanCosLatitude(double lon1deg, double lat1deg, double lon2deg, double lat2deg, double D = 0)
        //        {
        //            double approxDegreesSq = ApproxDegreesSq_PythagoreanCosLatitude(lon1deg, lat1deg, lon2deg, lat2deg);
        //            // approximate degrees on the great circle between the points.
        //            double d_degrees = Sqrt(approxDegreesSq);
        //            return d_degrees * EarthAvgMeterPerGreatCircleDegree;
        //        }

        //        public static double ApproxDegreesSq_PythagoreanCosLatitude(double lon1deg, double lat1deg, double lon2deg, double lat2deg)
        //        {
        //            double avgLatDeg = average(lat1deg, lat2deg);
        //            double avgLat = degreesToRadians(avgLatDeg);

        //            double d_ew = (lon2deg - lon1deg) * Cos(avgLat);
        //            double d_ns = (lat2deg - lat1deg);
        //            double approxDegreesSq = d_ew * d_ew + d_ns * d_ns;
        //            return approxDegreesSq;
        //        }



        //        // ========== lerp, wgtFromResult ==========

        //        public static double lerp(double a, double b, double wgtB)
        //        {
        //            return a + (wgtB * (b - a));
        //        }

        //        public static float lerp(float a, float b, float wgtB)
        //        {
        //            return a + (wgtB * (b - a));
        //        }

        //        // Equal to lerp(a, b, 0.5)
        //        public static double average(double a, double b)
        //        {
        //            return 0.5 * (a + b);
        //        }

        //        // Equal to lerp(a, b, 0.5)
        //        public static float average(float a, float b)
        //        {
        //            return 0.5f * (a + b);
        //        }

        //        // converse of lerp:
        //        // returns "wgt", such that
        //        //   result == lerp(a, b, wgt)
        //        public static double wgtFromResult(double a, double b, double result)
        //        {
        //            double denominator = b - a;

        //            if (Math.Abs(denominator) < 0.00000001)
        //            {
        //                if (Math.Abs(result - a) < 0.00000001)
        //                    return 0.5;

        //                return double.NaN;
        //            }

        //            double wgt = (result - a) / denominator;

        //            return wgt;
        //        }

        //        // Sets fields of outXY. (Avoids creating a Distance2D.Double instance.)
        //        public static void lerpXY(double ax, double ay, double bx, double by, double wgtB, Distance2D outXY)
        //        {
        //            outXY.X = lerp(ax, bx, wgtB);
        //            outXY.Y = lerp(ay, by, wgtB);
        //        }

        //        public static Distance2D lerpXY(double ax, double ay, double bx, double by, double wgtB)
        //        {
        //            return new Distance2D(lerp(ax, bx, wgtB), lerp(ay, by, wgtB));
        //        }

        //        public static Distance2D lerpXY(Distance2D a, Distance2D b, double wgtB)
        //        {
        //            return new Distance2D(lerp(a.X, b.X, wgtB), lerp(a.Y, b.Y, wgtB));
        //        }

        //        // xyWgt in (0..1, 0..1).
        //        public static Distance2D lerp2D(Distance2D xyWgt, Distance2D x0y0, Distance2D x1y0, Distance2D x0y1, Distance2D x1y1)
        //        {
        //            Distance2D xY0 = lerpXY(x0y0, x1y0, xyWgt.X);
        //            Distance2D xY1 = lerpXY(x0y1, x1y1, xyWgt.X);
        //            return lerpXY(xY0, xY1, xyWgt.Y);
        //        }

        //        public static Distance3D lerp3D(Distance3D a, Distance3D b, double wgtB)
        //        {
        //            return new Distance3D(lerp(a.X, b.X, wgtB), lerp(a.Y, b.Y, wgtB), lerp(a.Z, b.Z, wgtB));
        //        }


        //        // Returns (x, y), where each coordinate is in (0, 1) if it is within image.
        //        public static Distance2D inverseLerpWithZ(double ptx, double pty, double x0y0x, double x0y0y, double x1y0x, double x1y0y, double x0y1x, double x0y1y, double x1y1x, double x1y1y, double originZ, double ZtoY, double altitude)
        //        {

        //            Distance2D xyWgt = inverseLerp2D(ptx, pty, x0y0x, x0y0y, x1y0x, x1y0y, x0y1x, x0y1y, x1y1x, x1y1y);

        //            //		// Verify
        //            //		Distance2D xyWgt = lerp2D(new Distance2D(outx[0], outy[0]),
        //            //				   			   new Distance2D(x0y0x, x0y0y),
        //            //				   			   new Distance2D(x1y0x, x1y0y),
        //            //				   			   new Distance2D(x0y1x, x0y1y),
        //            //				   			   new Distance2D(x1y1x, x1y1y));
        //            //		if (!nearlyEquals(xyWgt, new Distance2D(ptx, pty), 0.000002))
        //            //			Dubious();

        //            /*
        //			 * Log.d("BEFORE", "lat" + ptx + " lon" + pty + " alt" + altitude +
        //			 * " ll lat" + x0y0y + " ll lon" + x0y0x + " lr lat" + x1y0y + " lr lon"
        //			 * + x1y0x + " ul lat" + x0y1y + " ul lon" + x0y1x + " ur lat" + x1y1y +
        //			 * " ur lon" + x1y1x + " oz" + originZ + " ztoy" + ZtoY + " out x" +
        //			 * outx[0] + " out y" + outy[0]);
        //			 */

        //            xyWgt.Y = xyWgt.Y + ZtoY * (altitude - originZ);

        //            /*
        //			 * Log.d("AFTER", "lat" + ptx + " lon" + pty + " alt" + altitude +
        //			 * " ll lat" + x0y0y + " ll lon" + x0y0x + " lr lat" + x1y0y + " lr lon"
        //			 * + x1y0x + " ul lat" + x0y1y + " ul lon" + x0y1x + " ur lat" + x1y1y +
        //			 * " ur lon" + x1y1x + " oz" + originZ + " ztoy" + ZtoY + " out x" +
        //			 * outx[0] + " out y" + outy[0]);
        //			 */
        //            // Log.d("formula", "" + imageHeight * ZtoY * (altitude - originZ));
        //            // Log.d("altitude", "" + altitude);

        //            return xyWgt;
        //        }

        //        // Returns (x, y), where each coordinate is in (0, 1) if it is within image.
        //        public static void inverseLerpWithZ(double ptx, double pty, double x0y0x, double x0y0y, double x1y0x, double x1y0y, double x0y1x, double x0y1y, double x1y1x, double x1y1y, double[] outx, double[] outy, double originZ, double ZtoY, double altitude)
        //        {
        //            inverseLerp2D(ptx, pty, x0y0x, x0y0y, x1y0x, x1y0y, x0y1x, x0y1y, x1y1x, x1y1y, outx, outy);

        //            //		// Verify
        //            //		Distance2D xyWgt = lerp2D(new Distance2D(outx[0], outy[0]),
        //            //				   			   new Distance2D(x0y0x, x0y0y),
        //            //				   			   new Distance2D(x1y0x, x1y0y),
        //            //				   			   new Distance2D(x0y1x, x0y1y),
        //            //				   			   new Distance2D(x1y1x, x1y1y));
        //            //		if (!nearlyEquals(xyWgt, new Distance2D(ptx, pty), 0.000002))
        //            //			Dubious();

        //            /*
        //			 * Log.d("BEFORE", "lat" + ptx + " lon" + pty + " alt" + altitude +
        //			 * " ll lat" + x0y0y + " ll lon" + x0y0x + " lr lat" + x1y0y + " lr lon"
        //			 * + x1y0x + " ul lat" + x0y1y + " ul lon" + x0y1x + " ur lat" + x1y1y +
        //			 * " ur lon" + x1y1x + " oz" + originZ + " ztoy" + ZtoY + " out x" +
        //			 * outx[0] + " out y" + outy[0]);
        //			 */

        //            outy[0] = outy[0] + ZtoY * (altitude - originZ);

        //            /*
        //			 * Log.d("AFTER", "lat" + ptx + " lon" + pty + " alt" + altitude +
        //			 * " ll lat" + x0y0y + " ll lon" + x0y0x + " lr lat" + x1y0y + " lr lon"
        //			 * + x1y0x + " ul lat" + x0y1y + " ul lon" + x0y1x + " ur lat" + x1y1y +
        //			 * " ur lon" + x1y1x + " oz" + originZ + " ztoy" + ZtoY + " out x" +
        //			 * outx[0] + " out y" + outy[0]);
        //			 */
        //            // Log.d("formula", "" + imageHeight * ZtoY * (altitude - originZ));
        //            // Log.d("altitude", "" + altitude);

        //        }


        //        // This is inverse of the final step in "inverseLerpWithZ", "outy[0] + ZtoY * (altitude - originZ)".
        //        // BUT with SCREEN y instead of FRACTIONAL y.
        //        public static double removeAltitudeFromScreenY(double screenY, double altitude, double originZ, double ZtoY, double paneHeight, double zoomY)
        //        {
        //            // "ZtoY" is multiplier for fy with range of 1;
        //            // "zoomY * paneHeight *" gives us multiplier for screenY with range of paneHeight, given zoomY.
        //            return screenY - zoomY * paneHeight * ZtoY * (altitude - originZ);
        //        }

        //        // This is inverse of the final step in "inverseLerpWithZ", "outy[0] + ZtoY * (altitude - originZ)".
        //        public static double removeAltitudeFromFracY(double fy, double altitude, double originZ, double ZtoY)
        //        {
        //            return fy - ZtoY * (altitude - originZ);
        //        }


        //        // Returns xyWgt corresponding to (ptx, pty).
        //        public static Distance2D inverseLerp2D(double ptx, double pty, double x0y0x, double x0y0y, double x1y0x, double x1y0y, double x0y1x, double x0y1y, double x1y1x, double x1y1y)
        //        {

        //            Distance2D xyWgt = inverseLerp2D_pass1(ptx, pty, x0y0x, x0y0y, x1y0x, x1y0y, x0y1x, x0y1y, x1y1x, x1y1y);

        //            // Too much trouble passing around all the individual x's and y's.
        //            // BUT COST: new/GC.
        //            Distance2D xy = new Distance2D(ptx, pty);
        //            Distance2D x0y0 = new Distance2D(x0y0x, x0y0y);
        //            Distance2D x1y0 = new Distance2D(x1y0x, x1y0y);
        //            Distance2D x0y1 = new Distance2D(x0y1x, x0y1y);
        //            Distance2D x1y1 = new Distance2D(x1y1x, x1y1y);

        //            Distance2D xyVerify = lerp2D(xyWgt, x0y0, x1y0, x0y1, x1y1);
        //            double newError = distance(xy, xyVerify);
        //            // Made it more accurate, for the case where it is oscillating between
        //            // an x-error and a y-error.
        //            // This forces more passes. TODO: Find a way to converge more quickly.
        //            double tolerance = distance(x0y0, x1y1) * 0.00005; // 0.0002;

        //            int nMorePasses = 12; // 8;
        //            do
        //            {
        //                Distance2D oldXyWgt = xyWgt;
        //                double oldError = newError;
        //                // Another pass. Project to lines near the point, from the prior
        //                // pass.
        //                xyWgt = inverseLerp2D_passN(xyWgt, xy, x0y0, x1y0, x0y1, x1y1);
        //                xyVerify = lerp2D(xyWgt, x0y0, x1y0, x0y1, x1y1);
        //                newError = distance(xy, xyVerify);
        //                if (newError <= tolerance)
        //                    return xyWgt;
        //                else if (newError > oldError)
        //                    // Bad, but getting worse, so don't use the new result.
        //                    // TODO_DEBUG_OUTPUT:
        //                    // Dubious("InverseLerp2D - error increasing {0} => {1}",
        //                    // oldError, newError)
        //                    return oldXyWgt;
        //                nMorePasses -= 1;
        //            } while (nMorePasses > 0);

        //            // TODO_DEBUG_OUTPUT:
        //            // Dubious("InverseLerp2D - error {0} not within tolerance, after all passes",
        //            // newError)
        //            return xyWgt;
        //        }

        //        // Sets outx[0], outy[0].
        //        public static Distance2D inverseLerp2D(double ptx, double pty, double x0y0x, double x0y0y, double x1y0x, double x1y0y, double x0y1x, double x0y1y, double x1y1x, double x1y1y, double[] outx, double[] outy)
        //        {

        //            Distance2D xyWgt = inverseLerp2D_pass1(ptx, pty, x0y0x, x0y0y, x1y0x, x1y0y, x0y1x, x0y1y, x1y1x, x1y1y);

        //            // Too much trouble passing around all the individual x's and y's.
        //            // BUT COST: new/GC.
        //            Distance2D xy = new Distance2D(ptx, pty);
        //            Distance2D x0y0 = new Distance2D(x0y0x, x0y0y);
        //            Distance2D x1y0 = new Distance2D(x1y0x, x1y0y);
        //            Distance2D x0y1 = new Distance2D(x0y1x, x0y1y);
        //            Distance2D x1y1 = new Distance2D(x1y1x, x1y1y);

        //            Distance2D xyVerify = lerp2D(xyWgt, x0y0, x1y0, x0y1, x1y1);
        //            double newError = distance(xy, xyVerify);
        //            // Made it more accurate, for the case where it is oscillating between
        //            // an x-error and a y-error.
        //            // This forces more passes. TODO: Find a way to converge more quickly.
        //            double tolerance = distance(x0y0, x1y1) * 0.00005; // 0.0002;

        //            int nMorePasses = 12; // 8;
        //            do
        //            {
        //                Distance2D oldXyWgt = xyWgt;
        //                double oldError = newError;
        //                // Another pass. Project to lines near the point, from the prior
        //                // pass.
        //                xyWgt = inverseLerp2D_passN(xyWgt, xy, x0y0, x1y0, x0y1, x1y1);
        //                xyVerify = lerp2D(xyWgt, x0y0, x1y0, x0y1, x1y1);
        //                newError = distance(xy, xyVerify);
        //                if (newError <= tolerance)
        //                {
        //                    // return xyWgt;
        //                    outx[0] = xyWgt.X;
        //                    outy[0] = xyWgt.Y;
        //                    return xyWgt;

        //                }
        //                else if (newError > oldError)
        //                {
        //                    // Bad, but getting worse, so don't use the new result.
        //                    // TODO_DEBUG_OUTPUT:
        //                    // Dubious("InverseLerp2D - error increasing {0} => {1}",
        //                    // oldError, newError)
        //                    outx[0] = oldXyWgt.X;
        //                    outy[0] = oldXyWgt.Y;
        //                    return oldXyWgt;
        //                }
        //                nMorePasses -= 1;
        //            } while (nMorePasses > 0);

        //            // TODO_DEBUG_OUTPUT:
        //            // Dubious("InverseLerp2D - error {0} not within tolerance, after all passes",
        //            // newError)
        //            // return xyWgt;
        //            outx[0] = xyWgt.X;
        //            outy[0] = xyWgt.Y;
        //            return xyWgt;
        //        }

        //        // Calls pointDistanceToLine_AndT with allowExtendedT=true, so can return a point outside of the image.
        //        // This is needed, because the point BEFORE taking z into account might fall outside.
        //        private static Distance2D inverseLerp2D_pass1(double ptx, double pty, double x0y0x, double x0y0y, double x1y0x, double x1y0y, double x0y1x, double x0y1y, double x1y1x, double x1y1y)
        //        {
        //            double distx0 = pointDistanceToLine_AndT(ptx, pty, x0y0x, x0y0y, x1y0x, x1y0y, out double wx0, true);
        //            double distx1 = pointDistanceToLine_AndT(ptx, pty, x0y1x, x0y1y, x1y1x, x1y1y, out double wx1, true);
        //            double disty0 = pointDistanceToLine_AndT(ptx, pty, x0y0x, x0y0y, x0y1x, x0y1y, out double wy0, true);
        //            double disty1 = pointDistanceToLine_AndT(ptx, pty, x1y0x, x1y0y, x1y1x, x1y1y, out double wy1, true);

        //            double wwx = distx0 / (distx0 + distx1);
        //            double wwy = disty0 / (disty0 + disty1);
        //            double wx = lerp(wx0, wx1, wwx);
        //            double wy = lerp(wy0, wy1, wwy);

        //            if ((wx < .0f) || (wx > 1.0))
        //            {
        //                if ((wx < -0.1) || (wx > 1.1))
        //                {
        //                }
        //                wx = clamp(wx, .0f, 1.0f);
        //            }

        //            if ((wy < -0f) || (wy > 1.0))
        //            {
        //                if ((wy < -0.1) || (wy > 1.1))
        //                {
        //                }
        //                wy = clamp(wy, .0f, 1.0f);
        //            }

        //            return new Distance2D(wx, wy);
        //        }

        //        // Calls closestPointOnLine_AndT with allowExtendedT=true, so can return a point outside of the image.
        //        // This is needed, because the point BEFORE taking z into account might fall outside.
        //        private static Distance2D inverseLerp2D_passN(Distance2D xyWgt1, Distance2D xy, Distance2D x0y0, Distance2D x1y0, Distance2D x0y1, Distance2D x1y1)
        //        {
        //            // NOTE: the "x-guess" line yields an improved approximation to yWgt
        //            // (yWgt2);
        //            // the "y-guess" line yields improved xWgt.
        //            double xWgt = xyWgt1.X;
        //            Distance2D XguessY0 = lerpXY(x0y0, x1y0, xWgt);
        //            Distance2D XguessY1 = lerpXY(x0y1, x1y1, xWgt);
        //            // "t" along the X-guess line is adjusted yWgt.
        //            // EXPLAIN: each "constant xWgt" line is drawn from yWgt=0 at one end,
        //            // to yWgt=1 at other end.
        //            // Projecting to closest point on such a line, yields a "t" that
        //            // likewise goes from 0 to 1.
        //            // That "t" is an estimate of yWgt.
        //            // From the first pass, we have a line that is near to xy,
        //            // so the projection is more accurate than our first pass, which was
        //            // projecting
        //            // to the sides of the quadrilateral.
        //            // If the incoming xyWgt1 were exactly correct,
        //            // The line (XguessY0, XguessY1) and the line (X0Yguess, X1Yguess) would
        //            // both pass THRU "xy".
        //            // By definition of Lerp2D, the output "xyWgt2" would yield the same
        //            // weights.
        //            // (xy would be a point on each line; its "weight" along each line would
        //            // be the x-wgt and y-wgt.)
        //            // To understand this, first study "WgtFromResult", and understand how
        //            // that works in 1 dimension.
        //            // InverseLerp2D is essentially a 2-D version of WgtFromResult. (The
        //            // implementation is different, because in 2-D the situation is
        //            // non-linear.)
        //            //
        //            // Sets t[0].
        //            // x params => yWgt2 ("y" is not a typo).
        //            closestPointOnLine_AndT(xy, XguessY0, XguessY1, out double yWgt2, true);

        //            double yWgt = xyWgt1.Y;
        //            Distance2D X0Yguess = lerpXY(x0y0, x0y1, yWgt);
        //            Distance2D X1Yguess = lerpXY(x1y0, x1y1, yWgt);
        //            // y params => xWgt2 ("x" is not a typo).
        //            closestPointOnLine_AndT(xy, X0Yguess, X1Yguess, out double xWgt2, true);

        //            // double Xguess_error = CalcDistance2D(xy, Xguess_closest)
        //            // double Yguess_error = CalcDistance2D(xy, Yguess_closest)

        //            // Improved guess.
        //            return new Distance2D(xWgt2, yWgt2);
        //        }


        //        // ========== closest point, pointDistanceToLine_AndT ==========

        //        public static double calcTOfClosestPoint(double ptx, double pty, double p1x, double p1y, double deltax, double deltay)
        //        {
        //            return ((ptx - p1x) * deltax + (pty - p1y) * deltay) / (deltax * deltax + deltay * deltay);
        //        }

        //        private const float COVER_LINE_MIN_T = 0.0f;
        //        private const float COVER_LINE_MAX_T = 1.0f;
        //        // Somewhat before the (0..1) range of t that covers the line.
        //        private const float EXTENDED_MIN_T = -2.0f;
        //        // Somewhat after  the (0..1) range of t that covers the line.
        //        private const float EXTENDED_MAX_T = 3.0f;

        //        // pass T as array like new float[] = { val }
        //        // Sets t[0].
        //        // "allowExtendedT=true": ALLOW t to extend outside of (0..1) range, specifically (-1..2).
        //        // REASON: may need to calculate points somewhat outside in 2D;
        //        //         altitude may bring them back within visible region.
        //        //         (and if altitude DOESN'T bring them back,
        //        //          we want to correctly determine that they fall outside visible area.)
        //        public static Distance2D closestPointOnLine_AndT(double ptx, double pty, double p1x, double p1y,
        //                                                       double p2x, double p2y, out double t, bool allowExtendedT)
        //        {
        //            double deltax = p2x - p1x;
        //            double deltay = p2y - p1y;

        //            if (deltax == 0 && deltay == 0)
        //            {
        //                t = .5f;
        //                return new Distance2D(p1x, p1y);
        //            }

        //            t = calcTOfClosestPoint(ptx, pty, p1x, p1y, deltax, deltay);

        //            double closestx;
        //            double closesty;

        //            float min_t = (allowExtendedT ? EXTENDED_MIN_T : COVER_LINE_MIN_T);
        //            float max_t = (allowExtendedT ? EXTENDED_MAX_T : COVER_LINE_MAX_T);

        //            if (t < min_t)
        //                t = min_t;
        //            else if (t > max_t)
        //                t = max_t;
        //            else
        //            {
        //            }

        //            closestx = p1x + t * deltax;
        //            closesty = p1y + t * deltay;

        //            return new Distance2D(closestx, closesty);
        //        }

        //        public static Distance2D closestPointOnLine_AndT(Distance2D pt, Distance2D p1, Distance2D p2, out double t, bool allowExtendedT)
        //        {
        //            var result = closestPointOnLine_AndT(pt.X, pt.Y, p1.X, p1.Y, p2.X, p2.Y, out t, allowExtendedT);
        //            return result;
        //        }

        //        // closest is measured using 2D distances.
        //        // pts in Cartesian coords (not geo coords).
        //        public static Distance3D closestPointOnLine_AndT(Distance3D pt, Distance3D p1, Distance3D p2, out double t)
        //        {
        //            bool allowExtendedT = false;
        //            Distance2D pt2 = closestPointOnLine_AndT(pt.X, pt.Y, p1.X, p1.Y, p2.X, p2.Y, out t, allowExtendedT);


        //            return new Distance3D(pt2.X, pt2.Y, lerp(p1.Z, p2.Z, t));
        //        }

        //        // closest is measured using 2D distances.
        //        // pts in geo coords.
        //        public static Distance3D closestPointOnLine_AndT_geo(Distance3D geoPt, Distance3D geoP1, Distance3D geoP2, double[] t)
        //        {
        //            bool allowExtendedT = false;
        //            // TODO: How pick an appropriate orthographic coordinate system?
        //            throw new NotImplementedException("closestPointOnLine_AndT_geo");
        //            Geo.IContext context;
        //            Distance2D mayaPt = context.FromWGS84(geoPt.To2D());
        //            Distance2D mayaP1 = context.FromWGS84(geoP1.To2D());
        //            Distance2D mayaP2 = context.FromWGS84(geoP2.To2D());

        //            double t0;
        //            Distance2D mayaClosest = closestPointOnLine_AndT(mayaPt.X, mayaPt.Y, mayaP1.X, mayaP1.Y, mayaP2.X, mayaP2.Y, out t0, allowExtendedT);
        //            t[0] = t0;

        //            Distance2D geoClosest = context.ToWGS84(mayaClosest);
        //            return new Distance3D(geoClosest.X, geoClosest.Y, lerp(geoP1.Z, geoP2.Z, t0));
        //        }

        //        public static Distance2D closestPointOnLine_AndT_geo(Distance2D geoPt, Distance2D geoP1, Distance2D geoP2, double[] t)
        //        {
        //            bool allowExtendedT = false;
        //            // TODO: How pick an appropriate orthographic coordinate system?
        //            throw new NotImplementedException("closestPointOnLine_AndT_geo");
        //            Geo.IContext context;
        //            Distance2D mayaPt = context.FromWGS84(geoPt);
        //            Distance2D mayaP1 = context.FromWGS84(geoP1);
        //            Distance2D mayaP2 = context.FromWGS84(geoP2);

        //            double t0;
        //            Distance2D mayaClosest = closestPointOnLine_AndT(mayaPt.X, mayaPt.Y, mayaP1.X, mayaP1.Y, mayaP2.X, mayaP2.Y, out t0, allowExtendedT);
        //            t[0] = t0;

        //            Distance2D geoClosest = context.ToWGS84(mayaClosest);
        //            return geoClosest;
        //        }

        //        // pts in Cartesian coords (not geo coords).
        //        public static double pointDistanceToLine_AndT(double ptx, double pty, double p1x, double p1y, double p2x, double p2y,
        //                                                       out double t, bool allowExtendedT)
        //        {
        //            Distance2D closest = closestPointOnLine_AndT(ptx, pty, p1x, p1y, p2x, p2y, out t, allowExtendedT);
        //            return distance(ptx, pty, closest.X, closest.Y);
        //        }


        //        // ========== hitLineOfPlayAtDistance ==========

        //        // From "startLocation", moves to closest point along sequence of "pts", then moves along that sequence by "moveDistance".
        //        // "pts" are geo coords, so uses AED distance calc.
        //        public static Distance3D pointAheadOnPointSequence(Distance3D startGeo, Distance3D[] ptsGeo, double moveDistanceGeo)
        //        {
        //            // When no points, we can't move anywhere.
        //            if (ptsGeo == null)
        //                return startGeo;

        //            int[] startILeg = new int[1];
        //            double[] startT = new double[1];

        //            startILeg[0] = 0;
        //            startT[0] = 0.0;

        //            closestPointOnPointSequence(startGeo, ptsGeo, startILeg, startT);

        //            int[] endILeg = new int[1];
        //            double[] endT = new double[1];

        //            endILeg[0] = 0;
        //            endT[0] = 0.0;

        //            Distance3D endPt = moveAlongPointSequence(startILeg[0], startT[0], moveDistanceGeo, ptsGeo, endILeg, endT);

        //            return endPt;
        //        }

        //        // Given an initial position along one leg of a sequence of points,
        //        // "startILeg" and "startT" (e.g. ClosestPointOnPointSequence > closestILeg & closestT),
        //        // and a distance to move, calculates where to move to.
        //        // Won't go beyond end of sequence.
        //        // "endILeg" is the START of the final leg that is used. (But if reach very end, it will be the final point.)
        //        // "pts" are geo coords, so uses AED distance calc.
        //        public static Distance3D moveAlongPointSequence(int startILeg, double startT, double moveDistanceMaya, Distance3D[] pts, int[] endILeg, double[] endT)
        //        {
        //            endILeg[0] = startILeg;
        //            endT[0] = startT;
        //            double remainingMoveDI = moveDistanceMaya;
        //            Distance3D endPt = pts[startILeg];

        //            if (startILeg == pts.Length - 1)
        //                return endPt;

        //            while (endILeg[0] < pts.Length - 1)
        //            {
        //                throw new NotImplementedException("moveAlongPointSequence");
        //                double legDI;//TODO = Distance3D.calculateDistanceDD_AED_2D(pts[endILeg[0]], pts[endILeg[0] + 1]);
        //                double tDI = endT[0] * legDI;
        //                double remainingDistanceOnLeg = legDI - tDI;

        //                if (remainingDistanceOnLeg > remainingMoveDI)
        //                {
        //                    double deltaT = remainingMoveDI / remainingDistanceOnLeg;
        //                    endT[0] += deltaT;
        //                    endPt = lerp3D(endPt, pts[endILeg[0] + 1], endT[0]);

        //                    return endPt;
        //                }
        //                else
        //                {
        //                    // Move to end of leg; which is beginning of next leg.
        //                    remainingMoveDI -= remainingDistanceOnLeg;
        //                    // At beginning of next leg.
        //                    endILeg[0] += 1;
        //                    endPt = pts[endILeg[0]];
        //                    endT[0] = 0.0;
        //                }
        //            }

        //            return endPt;
        //        }


        //        // Interpolates between points.
        //        public static Distance3D closestPointOnPointSequence(Distance3D loc, Distance3D[] linePts)
        //        {
        //            int[] startILeg = new int[1];
        //            double[] legT = new double[1];
        //            return closestPointOnPointSequence(loc, linePts, startILeg, legT);
        //        }

        //        // 2D calculation: z's are ignored.
        //        // Returns a point along line segments between sequence of points (not just one of the pts; may be anywhere between two adjacent ones).
        //        //   E.g. "pts" might be line-of-play points from tee(s) to (start/center) green.
        //        // closestILeg is 0, for the leg from pts(0) to pts(1). Etc.
        //        // closestT if fraction (lerp weight) along the leg: it is 0 at the start of the leg; 1 at the end of the leg.
        //        // The returned value "closestPt" can be calculated from closestILeg and closestT:
        //        //   = pts(closestILeg) + lerp(pts(closestILeg), pts(closestILeg+1), closestT)
        //        // Special case when "closestILeg = LastIndex(pts)", because there is no "pts(closestILeg+1)":
        //        //   = pts(closestILeg)
        //        //   = LastElement(pts)
        //        // "pts" are geo coords, so uses AED distance calc.
        //        public static Distance3D closestPointOnPointSequence(Distance3D loc, IList<Distance3D> pts, int[] closestILeg, double[] closestT)
        //        {
        //            Distance3D ret = new Distance3D();
        //            closestILeg[0] = 0;
        //            closestT[0] = 0;
        //            double minDistanceSq = double.MaxValue;

        //            for (int iLeg = 0; iLeg < pts.Count - 1; iLeg++)
        //            {
        //                Distance3D p1 = pts[iLeg];
        //                Distance3D p2 = pts[iLeg + 1];
        //                double[] t = new double[1];

        //                Distance3D closestPt1 = closestPointOnLine_AndT_geo(loc, p1, p2, t);
        //                double distanceSq1 = distanceSquared(loc, closestPt1);

        //                if (distanceSq1 < minDistanceSq)
        //                {
        //                    minDistanceSq = distanceSq1;
        //                    ret = closestPt1;
        //                    closestILeg[0] = iLeg;
        //                    closestT[0] = t[0];
        //                }
        //            }

        //            return ret;
        //        }

        //        public static Distance2D closestPointOnPointSequence(Distance2D loc, Distance2D[] pts)
        //        {
        //            int closestILeg;
        //            double closestT;
        //            return closestPointOnPointSequence(loc, pts, out closestILeg, out closestT);
        //        }

        //        // "loc" in geo coords.
        //        public static Distance2D closestPointOnPointSequence(Distance2D loc, Distance2D[] pts, out int closestILeg, out double closestT)
        //        {
        //            Distance2D ret = new Distance2D();

        //            closestILeg = 0;
        //            closestT = 0.0;

        //            double minDistanceSq = double.MaxValue;

        //            for (int iLeg = 0; iLeg < pts.Length - 1; iLeg++)
        //            {
        //                Distance2D p1 = pts[iLeg];
        //                Distance2D p2 = pts[iLeg + 1];
        //                double[] t = new double[1];

        //                Distance2D closestPt1 = closestPointOnLine_AndT_geo(loc, p1, p2, t);
        //                double distanceSq1 = distanceSquared(loc, closestPt1);

        //                if (distanceSq1 < minDistanceSq)
        //                {
        //                    minDistanceSq = distanceSq1;
        //                    ret = closestPt1;
        //                    closestILeg = iLeg;
        //                    closestT = t[0];
        //                }
        //            }

        //            return ret;
        //        }


        //        // Caller responsible for not calling when A.Y = B.Y.
        //        // If caller passes in a Y that is beyond either end of span, will get X as if span extended.
        //        public static double XAtY(double y, Distance2D a, Distance2D b)
        //        {
        //            double dy = b.Y - a.Y;
        //            double dx = b.X - a.X;
        //            // Possible Divide By Zero: Caller responsible for not calling when A.Y = B.Y.
        //            double dxdy = dx / dy;

        //            // "r": Relative to A.
        //            double ry = y - a.Y;
        //            double rx = dxdy * ry;

        //            // At y=A.Y, ry=0, rx=0  =>  x=A.X
        //            // At y=B.Y, ry=dy, rx = (dx/dy) * dy = dx  =>  x = A.X + dx = B.X
        //            double x = a.X + rx;
        //            return x;
        //        }


        //        public static Distance3D pointAlongLine_AED(Distance3D geoStartLocation, Distance3D geoEndLocation, double maxDistance)
        //        {
        //            // TODO: How pick an appropriate orthographic coordinate system?
        //            throw new NotImplementedException("pointAlongLine_AED");
        //            Geo.IContext context;
        //            Distance3D mayaStartLocation = context.FromWGS84(geoStartLocation);
        //            Distance3D mayaEndLocation = context.FromWGS84(geoEndLocation);
        //            double endDistance = Distance3D.CalcDistance2D(mayaStartLocation, mayaEndLocation);

        //            // Can reach endLocation, without exceeding maxDistance (ignoring altitude change).
        //            if (endDistance <= maxDistance)
        //                return geoEndLocation;

        //            Distance3D mayaReachedPt = Lerp(mayaStartLocation, mayaEndLocation, maxDistance / endDistance);
        //            return context.ToWGS84(mayaReachedPt);
        //        }

        //        // Used to calculate landing point along a line-of-play (LOP).
        //        // Player is standing at "startLocation".
        //        // From "startLocation", moves to closest point along sequence of "lopPts",
        //        // then moves along that sequence until we are "landingDistance" from startLocation.
        //        // This is different than "PointAheadOnPointSequence", in that the distance is calculated from startLocation, rather than measuring along LOP.
        //        public static Distance3D hitLineOfPlayAtDistance(Distance3D startLocation, Distance3D[] lopPts, double landingDistance)
        //        {
        //            int[] startILeg = new int[1];
        //            double[] startT = new double[1];
        //            Distance3D startPtOnSequence = closestPointOnPointSequence(startLocation, lopPts, startILeg, startT);

        //            Distance3D guessPt = startPtOnSequence;
        //            double guessDistance = calculateDistanceDD_AED(startLocation, guessPt);

        //            double verifyDistance = 0.0;
        //            // If we are > landingDistance from LOP, then the best answer is to hit straight towards LOP.
        //            // NOTE: This won't happen in golf, because we would be way out of bounds,
        //            // but included for completeness, in case a small landingDistance is passed in,
        //            // or viewing one hole while standing at a different hole.
        //            if (guessDistance > landingDistance)
        //            {
        //                verifyDistance = calculateDistanceDD_AED(startLocation, guessPt);
        //                // Under extreme circumstances, the landingPt might be farther away than the specified distance.
        //                // (This would mean player is farther away from LOP than the specified distance.)
        //                // In that case, The point they can reach is in the direction of the guessPt, at landingDistance.
        //                return pointAlongLine_AED(startLocation, guessPt, landingDistance);
        //                //return guessPt;			
        //            }

        //            // Examine each point on LOP, until find one that is farther than landingDistance.
        //            // The answer will be on the leg leading to that point.
        //            Distance3D longPt = guessPt;
        //            double longDistance = guessDistance;
        //            int[] endILeg = new int[1];
        //            endILeg[0] = startILeg[0] + 1;
        //            while (endILeg[0] <= lopPts.Length - 1)
        //            {
        //                longPt = lopPts[endILeg[0]];
        //                longDistance = calculateDistanceDD_AED(startLocation, longPt);
        //                // This leg has a point at desired distance.
        //                if (longDistance >= landingDistance)
        //                    break;
        //                // Prep Next
        //                endILeg[0] += 1;
        //            }

        //            if (longDistance < landingDistance)
        //            {
        //                // We reached end of LOP, without reaching landingDistance.
        //                // Return end of LOP.
        //                verifyDistance = calculateDistanceDD_AED(startLocation, guessPt);

        //                return LastElement(lopPts);
        //            }

        //            // There is some point along this leg that is at landingDistance.
        //            Distance3D shortPt = lopPts[endILeg[0] - 1];
        //            double shortDistance = calculateDistanceDD_AED(startLocation, shortPt);

        //            double[] t = new double[1];
        //            Distance3D closestPtOnLeg = closestPointOnLine_AndT_geo(startLocation, shortPt, longPt, t);
        //            double closestPtOnLegDistance = calculateDistanceDD_AED(startLocation, closestPtOnLeg);
        //            if (closestPtOnLegDistance < landingDistance)
        //                // The usual case: closest point on leg falls short.
        //                shortPt = closestPtOnLeg;
        //            else
        //            {
        //                // Don't think this can happen, but just in case:
        //                //    closest point on leg is long.
        //                longPt = closestPtOnLeg;
        //                longDistance = closestPtOnLegDistance;
        //            }

        //            // Theoretically possible, though unlikely to ever happen in practice.
        //            //   Avoids divide by zero in WgtFromResult.
        //            if (shortDistance == longDistance)
        //            {
        //                verifyDistance = calculateDistanceDD_AED(startLocation, longPt);

        //                return longPt;
        //            }

        //            double wgt = wgtFromResult(shortDistance, longDistance, landingDistance);
        //            // Point along the leg.
        //            // CAUTION: Not exact, because distance calculation from startPoint isn't a linear function along the segment.
        //            guessPt = lerp3D(shortPt, longPt, wgt);
        //            guessDistance = calculateDistanceDD_AED(startLocation, guessPt);

        //            // Iterate to get more accurate answer.
        //            int nMore = 5;
        //            while (nMore > 0)
        //            {
        //                if (guessDistance > landingDistance + .2)
        //                {
        //                    // Overshot.
        //                    longPt = guessPt;
        //                    longDistance = guessDistance;
        //                }
        //                else if (guessDistance < landingDistance - .2)
        //                {
        //                    // Undershot.
        //                    shortPt = guessPt;
        //                    shortDistance = guessDistance;
        //                }
        //                else
        //                {
        //                    // Close enough.
        //                    break;
        //                }

        //                // Prep Next.
        //                nMore -= 1;
        //                wgt = wgtFromResult(shortDistance, longDistance, landingDistance);
        //                guessPt = lerp3D(shortPt, longPt, wgt);
        //                guessDistance = calculateDistanceDD_AED(startLocation, guessPt);
        //            }

        //            verifyDistance = calculateDistanceDD_AED(startLocation, guessPt);

        //            return guessPt;
        //        }



        //        // returns (minDistance, closestPoint).
        //        // Don't wrap. If want closest polygon, and poly is not already closed, caller must append first point at end of array.
        //        public static Tuple<double, Distance2D> pointDistanceToPolyline_AndClosestPoint(Distance2D point, Distance2D[] poly)
        //        {
        //            double minDistanceSq = double.MaxValue;
        //            Distance2D closestPt = new Distance2D();

        //            int lastIndex = poly.Length - 1;
        //            for (int i = 0; i <= lastIndex - 1; i++)
        //            {
        //                // Don't wrap. If want closest polygon, and poly is not already closed, caller must append first point at end of array.
        //                int j = i + 1;
        //                //If j >= lastIndex Then j = 0

        //                Distance2D closest1 = new Distance2D();
        //                double distanceSq = pointDistanceSquaredToLine2D(point, poly[i], poly[j], ref closest1);
        //                if (distanceSq < minDistanceSq)
        //                {
        //                    minDistanceSq = distanceSq;
        //                    closestPt = closest1;
        //                }

        //            }
        //            double minDistance = double.MaxValue;
        //            if (closestPt != null)
        //                minDistance = Math.Sqrt(minDistanceSq);

        //            return new Tuple<double, Distance2D>(minDistance, closestPt);
        //        }

        //        public static double pointDistanceSquaredToLine2D(Distance2D pt, Distance2D p1, Distance2D p2, ref Distance2D closestPt)
        //        {
        //            // (distanceSquared, closestPoint).
        //            Tuple<double, Distance2D> ret = pointDistanceSqToLine2D_AndClosestPt(pt, p1, p2);

        //            // OUT: closestPoint.
        //            closestPt = ret.Item2;
        //            // RETURN: distanceSquared.
        //            return ret.Item1;
        //        }

        //        // returns (distanceSquared, closestPoint).
        //        public static Tuple<double, Distance2D> pointDistanceSqToLine2D_AndClosestPt(Distance2D pt, Distance2D p1, Distance2D p2)
        //        {
        //            double t;
        //            Distance2D closestPt = linePointClosestToPoint2D(pt, p1, p2, out t);
        //            double distanceSq = distanceSquared(pt, closestPt);
        //            return new Tuple<double, Distance2D>(distanceSq, closestPt);
        //        }

        //        public static Distance2D linePointClosestToPoint2D(Distance2D pt, Distance2D p1, Distance2D p2, out double t)
        //        {
        //            Distance2D delta = delta2D(p1, p2);

        //            if (delta.X == 0 && delta.Y == 0)
        //            {
        //                t = 0.0; // arbitrary - the points are identical.
        //                return p1;
        //            }

        //            t = ((pt.X - p1.X) * delta.X + (pt.Y - p1.Y) * delta.Y) / (delta.X * delta.X + delta.Y * delta.Y);

        //            Distance2D closest;
        //            if (t < 0)
        //            {
        //                t = 0.0;
        //                closest = p1;
        //            }
        //            else if (t > 1)
        //            {
        //                t = 1.0;
        //                closest = p2;
        //            }
        //            else
        //            {
        //                closest = new Distance2D(p1.X + t * delta.X, p1.Y + t * delta.Y);
        //            }

        //            return closest;
        //        }


        //        // ========== Vector for OpenGL ==========

        //        // Returns homogeneous coordinates.
        //        public static Vector4 newGLVector(float x, float y, float z)
        //        {
        //            return newGLVector(x, y, z, 1.0f);
        //        }

        //        // Input homogeneous coordinates. If unsure, pass "1.0" for w.
        //        public static Vector4 newGLVector(float x, float y, float z, float w)
        //        {
        //            return new Vector4(x, y, z, w);
        //        }

        //        public static Vector4 newGLVector(Distance3D v)
        //        {
        //            return newGLVector((float)v.X, (float)v.Y, (float)v.Z);
        //        }


        //        // NOTE: glVec is only single-precision, so we don't have double precision accuracy.
        //        public static Distance3D asDouble3(Vector4 glVec)
        //        {
        //            // REQUIRE: glVec.W==1.
        //            return new Distance3D(glVec.X, glVec.Y, glVec.Z);
        //        }


        //        // Result is a GL vector.
        //        // TODO: Do we need to invert the matrix, since we have params in opposite order from OpenTK??
        //        public static Vector4 glMultiplyMV(Matrix4 lhsMat, Vector4 rhsVec)
        //        {
        //            //WAS Matrix.multiplyMV(resultVec, 0, lhsMat, 0, rhsVec, 0);
        //            // TODO: Do we need to invert the matrix, since we have params in opposite order from OpenTK??
        //            Vector4 resultVec = Vector4.Transform(rhsVec, lhsMat);
        //            return resultVec;
        //        }

        //        // NOTE: OpenGL math is only single-precision, so we no longer have double precision accuracy.
        //        public static Distance3D glMultiplyMV(Matrix4 lhsMat, Distance3D rhsVec)
        //        {
        //            return asDouble3(glMultiplyMV(lhsMat, newGLVector(rhsVec)));
        //        }


        //        // ========== Matrix for OpenGL ==========

        //        // Returns inverse matrix.
        //        // Throws exception if can't invert.
        //        public static Matrix4 invertGLMatrix(Matrix4 m)
        //        {
        //            return Matrix4.Invert(m);
        //        }

        //        // INTRINSIC rotation (around m's x-axis, not world x-axis).
        //        // SIDE-EFFECT: changes m's elements.
        //        public static void rotateGLMatrixAroundX(ref Matrix4 m, float rotationRadians)
        //        {
        //            Matrix4 m2 = Matrix4.CreateRotationX(rotationRadians);
        //            // TODO: Is parameter order correct?
        //            m = Matrix4.Mult(m, m2);
        //        }

        //        // INTRINSIC rotation (around m's y-axis, not world y-axis).
        //        // SIDE-EFFECT: changes m's elements.
        //        public static void rotateGLMatrixAroundY(ref Matrix4 m, float rotationRadians)
        //        {
        //            Matrix4 m2 = Matrix4.CreateRotationY(rotationRadians);
        //            // TODO: Is parameter order correct?
        //            m = Matrix4.Mult(m, m2);
        //        }

        //        // INTRINSIC rotation (around m's z-axis, not world z-axis).
        //        // SIDE-EFFECT: changes m's elements.
        //        public static void rotateGLMatrixAroundZ(ref Matrix4 m, float rotationRadians)
        //        {
        //            Matrix4 m2 = Matrix4.CreateRotationZ(rotationRadians);
        //            // TODO: Is parameter order correct?
        //            m = Matrix4.Mult(m, m2);
        //        }

        //        // CAUTION: This applies the translation AFTER the existing rotations.
        //        // SIDE-EFFECT: changes m's elements.
        //        public static void translateGLMatrix(ref Matrix4 m, Distance3D translation)
        //        {
        //            Matrix4 m2 = Matrix4.CreateTranslation(translation.ToVector3());
        //            // TODO: Is parameter order correct?
        //            m = Matrix4.Mult(m, m2);
        //        }

        //        //		// HACK: stuffs values into translation elements, effectively applying them before the rotations.
        //        //		// SIDE-EFFECT: changes m's elements.
        //        //		public static void preTranslateGLMatrix(float[] m, Distance3D translation) {
        //        //			m[12] += (float)translation.X;
        //        //			m[13] += (float)translation.Y;
        //        //			m[14] += (float)translation.Z;
        //        //		}



        //        // The desired view space will be camera at origin, pointing down z-axis,
        //        // with up-vector along positive y axis.
        //        public static Matrix4 glViewTransformForTwoPointUprightCamera(Distance3D cameraPt, Distance3D aimPt)
        //        {
        //            // Start with world coords, and identity matrix.
        //            Matrix4 m = Matrix4.Identity;

        //            // 1. Rotate around z-axis, until camera and aim have same x.
        //            Distance3D d3Aim = aimPt - cameraPt;
        //            Distance2D aimHeadingXY = d3Aim.To2D();
        //            float aimAngleRadiansXY = headingAsAngleRadians2D(aimHeadingXY);
        //            float zRotation = ((float)degreesToRadians(90) - aimAngleRadiansXY);
        //            rotateGLMatrixAroundZ(ref m, zRotation);
        //            // Verify: x should be 0 w/i (+-1e-4) float accuracy.
        //            Distance3D d3Aim_step1 = glMultiplyMV(m, d3Aim);

        //            // 2. Rotate around (new) x-axis, until camera and aim (also) have same y,
        //            //    and aim has a more negative z than camera.
        //            Distance2D aimHeadingYZ_step2 = new Distance2D(d3Aim_step1.Y, d3Aim_step1.Z);
        //            float aimAngleRadiansYZ_step2 = headingAsAngleRadians2D(aimHeadingYZ_step2);
        //            // NO, neither of these are even close to having 0 in x&y.
        //            //rotateGLMatrixAroundX(m, ((float) degreesToRadians(90) - aimAngleRadiansYZ_step2));
        //            //rotateGLMatrixAroundX(m, ((float) - aimAngleRadiansYZ_step2));
        //            //
        //            // Do matrix-multiplies in reverse order.
        //            m = Matrix4.Identity;
        //            float xRotation = ((float)degreesToRadians(90) - aimAngleRadiansYZ_step2);
        //            rotateGLMatrixAroundX(ref m, xRotation);
        //            rotateGLMatrixAroundZ(ref m, zRotation);

        //            // Verify: x & y should be 0 (+-1e-4) float accuracy.
        //            // TODO: If z is positive instead of negative, then what?
        //            Distance3D d3Aim_step2 = glMultiplyMV(m, d3Aim);
        //            // If z is positive, rotate another 180 degrees.
        //            if (d3Aim_step2.Z > 0)
        //            {
        //                // Again, but 180 degrees more.
        //                xRotation = ((float)degreesToRadians(90 + 180) - aimAngleRadiansYZ_step2);
        //                //
        //                m = Matrix4.Identity;
        //                rotateGLMatrixAroundX(ref m, xRotation);
        //                rotateGLMatrixAroundZ(ref m, zRotation);
        //                d3Aim_step2 = glMultiplyMV(m, d3Aim);
        //            }

        //            // 2b. translate camera to origin.
        //            //		//     This needs to be done before the matrix-multiplies.
        //            //		m = newGLIdentityMatrix();
        //            //		preTranslateGLMatrix(m, Distance3D.negate(cameraPt));
        //            //		Distance3D verifyOrigin = glMultiplyMV(m, cameraPt);
        //            //		rotateGLMatrixAroundX(m, xRotation);
        //            //		verifyOrigin = glMultiplyMV(m, cameraPt);
        //            //		rotateGLMatrixAroundZ(m, zRotation);

        //            translateGLMatrix(ref m, -cameraPt);

        //            // 3. TODO: Scale X&Y based on angle-of-view (based on width, which is x).
        //            //    1/2 angle-of-view should result in "0.5" fraction of x-size (width).
        //            //    This interacts with perspective scale.
        //            //    Could say that it is abs-delta-z at which 

        //            // 4. TODO: Additional scale for Y, to correct for aspect ratio of image,
        //            //    because we will be returning 0..1 relative to length of edges.
        //            //    "* 1 / (y/x)" => "* (x/y)".  Remember to multiply this by scale from (3).

        //            // camera should transform to origin.
        //            Distance3D verifyOrigin = glMultiplyMV(m, cameraPt);

        //            // Now we can test the aimPt (not just a delta).
        //            // verify: should be (0, 0, -something)
        //            Distance3D verifyDownZN = glMultiplyMV(m, aimPt);

        //            // Verify that camera is upright: two points that differ only in z should have same x.
        //            Distance3D verifyAboveAimZeroX = glMultiplyMV(m, aimPt + new Distance3D(0, 0, 100));

        //            // Some different point.
        //            Distance3D otherPt = new Distance3D(aimPt.Y, aimPt.X, aimPt.Z);
        //            Distance3D viewOtherPt = glMultiplyMV(m, otherPt);
        //            // Should have same x as viewOtherPt.
        //            Distance3D viewOtherPtSameX = glMultiplyMV(m, otherPt + new Distance3D(0, 0, 100));

        //            return m;
        //        }


        //        // ========== DataBase/XML/Conversion Helpers ==========
        //        // --- NOTE: to/from string moved to Helper #regions "Format number to string" and "Parse string to number". ---



        //        // "roundUpExtra" is used to overcome slight math inaccuracy, so value near "N.5" returns "N+1".
        //        public static int RoundToInt(this double val, double roundUpExtra = 0)
        //        {
        //            return (int)Math.Round(val + roundUpExtra);
        //        }

        //        // "roundUpExtra" is used to overcome slight math inaccuracy, so value near "N.5" returns "N+1".
        //        public static int RoundToInt(this float val, float roundUpExtra = 0)
        //        {
        //            return (int)Math.Round(val + roundUpExtra);
        //        }

        //        /// <summary>
        //        /// "nParts=1" is same as Round. "nParts=2" rounds to nearest 1/2.
        //        /// "nParts=10" rounds to nearest 1/10, aka one decimal place.
        //        /// </summary>
        //        /// <param name="val"></param>
        //        /// <param name="nParts"></param>
        //        /// <returns></returns>
        //        public static float RoundToParts(this double val, int nParts)
        //        {
        //            return RoundToInt(nParts * val) / (float)nParts;
        //        }

        //        // 0-100 => 0-1.
        //        public static float percentToFraction(float percent)
        //        {
        //            return (percent / 100.0f);
        //        }


        //        public static float METERS_IN_A_YARD = 0.9144f;

        //        public static double metersToYards(double meters)
        //        {
        //            return meters / METERS_IN_A_YARD;
        //        }

        //        public static float metersToYards(float meters)
        //        {
        //            return meters / METERS_IN_A_YARD;
        //        }

        //        public static int metersToYards(int meters)
        //        {
        //            return RoundToInt(meters / METERS_IN_A_YARD);
        //        }


        //        public static double yardsToMeters(double yards)
        //        {
        //            return yards * METERS_IN_A_YARD;
        //        }

        //        public static float yardsToMeters(float yards)
        //        {
        //            return yards * METERS_IN_A_YARD;
        //        }

        //        public static int yardsToMeters(int yards)
        //        {
        //            return RoundToInt(yards * METERS_IN_A_YARD);
        //        }


        //        public static float METERS_IN_A_MILE = 1609.34f;

        //        public static double metersToMiles(double meters)
        //        {
        //            return meters / METERS_IN_A_MILE;
        //        }

        //        public static double metersToKilometers(double meters)
        //        {
        //            return meters / KILO;
        //        }



        //        public static Distance2D delta2D(double dblOrigonX, double dblOrigonY, double dblPointX, double dblPointY)
        //        {
        //            return new Distance2D(dblPointX - dblOrigonX, dblPointY - dblOrigonY);
        //        }

        //        public static Distance2D delta2D(Distance2D origon, Distance2D point)
        //        {
        //            return new Distance2D(point.X - origon.X, point.Y - origon.Y);
        //        }


        //        public static List<Distance2D> intersectionsLineAndPolygon(Distance2D p1, Distance2D p2, IList<Distance2D> polygon)
        //        {
        //            List<Distance2D> intersections = new List<Distance2D>();

        //            // ASSUME polygon is closed (last point same as first point).
        //            //   (If it isn't, then should add a segment from last point back to first point.)

        //            Distance2D p3 = new Distance2D();
        //            foreach (Distance2D p4 in polygon)
        //            {
        //                if (p3 != null)
        //                {
        //                    Distance2D isect = new Distance2D();
        //                    if (linesIntersectAt(p1, p2, p3, p4, ref isect))
        //                        intersections.Add(isect);
        //                }
        //                // Prep Next.
        //                p3 = p4;
        //            }


        //            return intersections;
        //        }


        //        public static double pointDistanceToLineSegment2D(double x, double y, double x1, double y1, double x2, double y2)
        //        {
        //            Distance2D closestPt = closestPointOnLineSegment(x, y, x1, y1, x2, y2);
        //            return distance(x, y, closestPt.X, closestPt.Y);
        //        }

        //        public static Distance2D closestPointOnLineSegment(Distance2D p, Distance2D p1, Distance2D p2)
        //        {
        //            return closestPointOnLineSegment(p.X, p.Y, p1.X, p1.Y, p2.X, p2.Y);
        //        }

        //        public static Distance2D closestPointOnLineSegment(double x, double y, double x1, double y1, double x2, double y2)
        //        {
        //            Distance2D delta = delta2D(x1, y1, x2, y2);

        //            // TBD: Use NearlyEquals?
        //            if ((delta.X == 0.0) & (delta.Y == 0.0))
        //                return new Distance2D(x1, y1);

        //            double t = (((x - x1) * delta.X) + ((y - y1) * delta.Y)) / ((delta.X * delta.X) + (delta.Y * delta.Y));

        //            Distance2D closestPt;
        //            if (t <= 0.0)
        //            {
        //                closestPt = new Distance2D(x1, y1);
        //            }
        //            else if (t >= 1.0)
        //            {
        //                closestPt = new Distance2D(x2, y2);
        //            }
        //            else
        //            {
        //                closestPt = new Distance2D(x1 + (t * delta.X),
        //                                         y1 + (t * delta.Y));
        //            }

        //            return closestPt;
        //        }


        //        public static double pointDistanceToLineSegment2D_AED(double px, double py, double x1, double y1, double x2, double y2,
        //                                                               bool approximate, double cosLat)
        //        {
        //            // Convert to Cartesian, by "* Cos(SiteLatitude)".
        //            // IF we don't do this, then the chosen "closest point" is not accurate, so the distance measurement is not accurate.
        //            px *= cosLat;
        //            x1 *= cosLat;
        //            x2 *= cosLat;

        //            Distance2D delta = delta2D(x1, y1, x2, y2);

        //            // Zero-length segment.
        //            if ((delta.X == 0.0) && (delta.Y == 0.0))
        //                return calculateDistanceDD_AED(px, py, x1, y1, approximate);

        //            // interpolator along segment.
        //            double t = (((px - x1) * delta.X) + ((py - y1) * delta.Y)) / ((delta.X * delta.X) + (delta.Y * delta.Y));

        //            Distance2D ptOnSegment = new Distance2D();
        //            if (t < 0.0)
        //            {
        //                ptOnSegment.X = x1;
        //                ptOnSegment.Y = y1;
        //            }
        //            else if (t > 1.0)
        //            {
        //                ptOnSegment.X = x2;
        //                ptOnSegment.Y = y2;
        //            }
        //            else
        //            {
        //                ptOnSegment.X = x1 + (t * delta.X);
        //                ptOnSegment.Y = y1 + (t * delta.Y);
        //            }

        //            // Convert back to lat/long.
        //            double invCosLat = 1 / cosLat;
        //            ptOnSegment.X *= invCosLat;
        //            px *= invCosLat;
        //            // Not used again.
        //            //x1 *= invCosLat;
        //            //x2 *= invCosLat;

        //            return calculateDistanceDD_AED(px, py, ptOnSegment.X, ptOnSegment.Y, approximate);
        //        }

        //        public static double pointDistanceToLines(Distance2D goalPt, Distance2D[] polygon)
        //        {
        //            Distance2D closestPt;
        //            return pointDistanceToLines(goalPt, polygon, out closestPt);
        //        }

        //        public static double pointDistanceToLines(Distance2D goalPt, Distance2D[] polygon, out Distance2D closestPt)
        //        {
        //            closestPt = new Distance2D();

        //            double minDistanceSq = double.MaxValue;
        //            int maxIndex = polygon.LastIndex();
        //            for (int i = 0; i <= maxIndex; i++)
        //            {
        //                int i2 = i + 1;
        //                if (i2 > maxIndex)
        //                    break;

        //                Distance2D closestPtOnSegment = closestPointOnLineSegment(goalPt, polygon[i], polygon[i2]);
        //                double distSq = distanceSquared(goalPt, closestPtOnSegment);

        //                if (distSq < minDistanceSq)
        //                {
        //                    minDistanceSq = distSq;
        //                    closestPt = closestPtOnSegment;
        //                }
        //            }

        //            return Math.Sqrt(minDistanceSq);
        //        }

        //        // Perhaps default should be "approximate=true": This is a LOT slower if ask for accurate calculation,
        //        // for little gain (for distances less than 1 km).
        //        public static double pointDistanceToLines_AED(Distance2D goalPt, Distance2D[] polygon, bool approximate, double cosLat)
        //        {
        //            double minDistance = double.MaxValue;
        //            int lastIndex = polygon.Length - 1;

        //            // "<" rather than "<=": Needs "i+1".
        //            for (int i = 0; i < lastIndex; i++)
        //            {
        //                int i2 = i + 1;
        //                if (i2 > (polygon.Length - 1))
        //                    break;

        //                double di = pointDistanceToLineSegment2D_AED(goalPt.X, goalPt.Y, polygon[i].X, polygon[i].Y,
        //                                                              polygon[i2].X, polygon[i2].Y, approximate, cosLat);

        //                if (di < minDistance)
        //                    minDistance = di;
        //            }

        //            return minDistance;
        //        }

        //        public static double pointDistanceToLines(Distance3D goalPt, Distance3D[] polygon)
        //        {
        //            double minDistance = double.MaxValue;
        //            int len = polygon.Length - 1;

        //            for (int i = 0; i <= len; i++)
        //            {
        //                int index = i + 1;
        //                if (index > (polygon.Length - 1))
        //                    break;

        //                double di = pointDistanceToLineSegment2D(goalPt.X, goalPt.Y, polygon[i].X, polygon[i].Y, polygon[index].X, polygon[index].Y);

        //                if (di < minDistance)
        //                {
        //                    minDistance = di;
        //                }
        //            }

        //            return minDistance;
        //        }

        //        // Perhaps default should be "approximate=true": This is a LOT slower if ask for accurate calculation,
        //        // for little gain (for distances less than 1 km).
        //        public static double pointDistanceToLines_AED(Distance3D goalGeo, Distance3D[] polyGeo, bool approximate, double cosLat)
        //        {
        //            double maxValue = double.MaxValue;
        //            int len = polyGeo.Length - 1;

        //            for (int i = 0; i <= len; i++)
        //            {
        //                int i2 = i + 1;
        //                if (i2 > (polyGeo.Length - 1))
        //                    break;

        //                double di = pointDistanceToLineSegment2D_AED(goalGeo.X, goalGeo.Y, polyGeo[i].X, polyGeo[i].Y,
        //                                                              polyGeo[i2].X, polyGeo[i2].Y, approximate, cosLat);

        //                if (di < maxValue)
        //                    maxValue = di;
        //            }

        //            return maxValue;
        //        }

        //        public static double polygonDistanceToPolygon(Distance3D[] shape1, Distance3D[] shape2)
        //        {
        //            double diClosest = double.MaxValue;

        //            for (int i = 0; i < shape1.Length; i++)
        //            {
        //                Distance3D p = shape1[i - 1];

        //                double di = pointDistanceToLines(p, shape2);

        //                if (di < diClosest)
        //                    diClosest = di;
        //            }

        //            return diClosest;
        //        }

        //        public static double polygonDistanceToPolygon(Distance2D[] shape1, Distance2D[] shape2)
        //        {
        //            double diClosest = double.MaxValue;

        //            for (int i = 0; i < shape1.Length; i++)
        //            {
        //                Distance2D p = shape1[i - 1];

        //                double di = pointDistanceToLines(p, shape2);

        //                if (di < diClosest)
        //                    diClosest = di;
        //            }

        //            return diClosest;
        //        }

        //        //// This should probably ALWAYS be called with "approximate=true": Otherwise it is a LOT slower calculation,
        //        ////   for little benefit.
        //        //public static double polygonDistanceToPolygon_AED(Distance3D[] shape1, Distance3D[] shape2, bool approximate)
        //        //{
        //        //    double diClosest = double.MaxValue;

        //        //    // TBD: ASSUMES measuring on ActiveCourse.
        //        //    double cosLat = AppState.ActiveCourse.CosSiteCenterLatitude();
        //        //    foreach (Distance3D geoPt in shape1)
        //        //    {
        //        //        double di = pointDistanceToLines_AED(geoPt, shape2, approximate, cosLat);
        //        //        if (di < diClosest)
        //        //            diClosest = di;
        //        //    }

        //        //    return diClosest;
        //        //}

        //        // Shortest distance between edges of two polygons.
        //        // NOTE: Tests points of one against lines of the other.
        //        // "approximate=true": Calculate approximate geo distance, using a faster formula.
        //        // This should probably ALWAYS be called with "approximate=true": Otherwise it is a LOT slower calculation,
        //        //   for little benefit.
        //        public static double polygonDistanceToPolygon_AED(Distance2D[] shape1, Distance2D[] shape2, bool approximate, double cosLat)
        //        {
        //            double diClosest = double.MaxValue;

        //            foreach (Distance2D geoPt in shape1)
        //            {
        //                // TBD: Could work in "distance-squared", which saves some time when using "approximate=true".
        //                double di = pointDistanceToLines_AED(geoPt, shape2, approximate, cosLat);
        //                if (di < diClosest)
        //                    diClosest = di;
        //            }

        //            return diClosest;
        //        }

        //        //public static double pointDistanceToLine_AED(Distance3D ptdPoint, Distance3D[] ptdPol)
        //        //{
        //        //    double maxValue = double.MaxValue;
        //        //    int len = ptdPol.Length - 1;

        //        //    // TBD: ASSUMES measuring on ActiveCourse.
        //        //    double cosLat = AppState.ActiveCourse.CosSiteCenterLatitude();

        //        //    // TODO OPTIMIZE: Probably sufficient to find closest vertex,
        //        //    //   then check distance to the one or two segments that involve that vertex.
        //        //    for (int i = 0; i <= len; i++)
        //        //    {
        //        //        int index = i + 1;
        //        //        if (index > (ptdPol.Length - 1))
        //        //            break;

        //        //        double di = pointDistanceToLineSegment2D_AED(
        //        //                        ptdPoint.X, ptdPoint.Y, ptdPol[i].X, ptdPol[i].Y,
        //        //                        ptdPol[index].X, ptdPol[index].Y, true, cosLat);

        //        //        if (di < maxValue)
        //        //            maxValue = di;
        //        //    }

        //        //    return maxValue;
        //        //}

        //        public static double getAngleDegrees2D(double originX, double originY, double aimPointX, double aimPointY)
        //        {
        //            double ret = Math.Atan2(aimPointY - originY, aimPointX - originX) * (180 / Math.PI);

        //            return ret;
        //        }

        //        public static Distance2D extendLine2D(Distance2D p1, Distance2D p2, double di)
        //        {
        //            return extendLine2D(p1.X, p1.Y, p2.X, p2.Y, di);
        //        }

        //        public static Distance2D extendLine2D(double p1x, double p1y, double p2x, double p2y, double di)
        //        {
        //            double totDistance = distance(p1x, p1y, p2x, p2y) + di;
        //            double angle = getAngleDegrees(p1x, p1y, p2x, p2y);

        //            return new Distance2D(p1x + totDistance * Math.Cos((angle / 180) * Math.PI), p1y + totDistance * Math.Sin((angle / 180) * Math.PI));
        //        }

        //        public static bool linesIntersectAt(Distance2D p1, Distance2D p2, Distance2D p3, Distance2D p4, ref Distance2D outPt, double tolerance = NearZero)
        //        {
        //            return linesIntersectAt(p1.X, p1.Y, p2.X, p2.Y, p3.X, p3.Y, p4.X, p4.Y, ref outPt, tolerance);
        //        }

        //        public static bool linesIntersectAt(double x1, double y1, double x2, double y2,
        //                                             double x3, double y3, double x4, double y4,
        //                                             ref Distance2D outPt, double tolerance = NearZero)
        //        {
        //            LineOverlap overlap = LineOverlap.Undefined;
        //            LineOverlap overlap1 = LineOverlap.Undefined;
        //            LineOverlap overlap2 = LineOverlap.Undefined;

        //            Distance2D ret = linesIntersectAt(x1, y1, x2, y2, x3, y3, x4, y4, tolerance, ref overlap, ref overlap1, ref overlap2);

        //            if (overlap == LineOverlap.Crossing || overlap == LineOverlap.CrossingTouch)
        //            {
        //                outPt = ret;
        //                return true;
        //            }
        //            else
        //            {
        //                outPt = new Distance2D();
        //                return false;
        //            }
        //        }

        //        public static Distance2D linesIntersectAt(double x1, double y1, double x2, double y2,
        //                                                double x3, double y3, double x4, double y4,
        //                                                double tolerance,
        //                                                ref LineOverlap overlap, ref LineOverlap overlap1, ref LineOverlap overlap2)
        //        {
        //            overlap1 = LineOverlap.Undefined;
        //            overlap2 = LineOverlap.Undefined;


        //            double x12 = x1 - x2;
        //            double x34 = x3 - x4;
        //            double y12 = y1 - y2;
        //            double y34 = y3 - y4;

        //            if (x12 == 0 && y12 == 0)
        //                Test();
        //            if (x34 == 0 && y34 == 0)
        //                Test();

        //            double scale = Math.Max(Math.Max(Math.Abs(x12), Math.Abs(x34)), Math.Max(Math.Abs(y12), Math.Abs(y34)));
        //            double scale_clamped = Math.Min(1.0, scale);
        //            double tolerance_scaled = scale_clamped * tolerance;
        //            double determinant = x12 * y34 - y12 * x34;
        //            double absDeterminant = Math.Abs(determinant);
        //            double scale_clamped_squared = scale_clamped * scale_clamped;
        //            bool asParallel = (absDeterminant <= scale_clamped_squared * NearZero);

        //            if ((absDeterminant <= scale_clamped_squared * OneThousandth) && !asParallel)
        //            {
        //                double xa, ya, xb, yb, xd, yd;

        //                if (distanceSquared(x1, y1, x2, y2) <= distanceSquared(x3, y3, x4, y4))
        //                {
        //                    // (p1,p2) is shorter
        //                    xd = x12;
        //                    yd = y12;
        //                    xa = 0;
        //                    ya = 0;
        //                    xb = x34;
        //                    yb = y34;
        //                }
        //                else
        //                {
        //                    // (p3,p4) is shorter
        //                    xd = x34;
        //                    yd = y34;
        //                    xa = 0;
        //                    ya = 0;
        //                    xb = x12;
        //                    yb = y12;
        //                }

        //                double distSq = pointDeltaToLineExtended(
        //                                    new Distance2D(xd, yd), new Distance2D(xa, ya), new Distance2D(xb, yb)).LengthSquared;

        //                if (distSq <= tolerance_scaled * tolerance_scaled)
        //                    asParallel = true;
        //            }

        //            if (asParallel)
        //            {
        //                Distance2D outP1 = new Distance2D();

        //                linesOverlapOrTouch_Parallel(new Distance2D(x1, y1), new Distance2D(x2, y2), new Distance2D(x3, y3), new Distance2D(x4, y4), tolerance_scaled, ref overlap, ref overlap1, ref overlap2, ref outP1);

        //                if (overlap != LineOverlap.NotParallel)
        //                    return outP1;
        //            }

        //            // Not parallel
        //            double origx1 = x1;
        //            double origy1 = y1;

        //            x1 -= origx1;
        //            y1 -= origy1;
        //            x2 -= origx1;
        //            y2 -= origy1;
        //            x3 -= origx1;
        //            y3 -= origy1;
        //            x4 -= origx1;
        //            y4 -= origy1;

        //            // Not parallel
        //            double xy12 = (x1 * y2 - y1 * x2);
        //            double xy34 = (x3 * y4 - y3 * x4);
        //            double rx = (xy12 * x34 - x12 * xy34) / determinant;
        //            double ry = (xy12 * y34 - y12 * xy34) / determinant;

        //            Distance2D pr = new Distance2D(rx, ry);
        //            bool checkY1 = firstIsShorter(x2 - x1, y2 - y1);
        //            bool checkY2 = firstIsShorter(x4 - x3, y4 - y3);
        //            Distance2D a = new Distance2D(x1, y1);
        //            Distance2D b = new Distance2D(x2, y2);
        //            Distance2D c = new Distance2D(x3, y3);
        //            Distance2D d = new Distance2D(x4, y4);
        //            overlap = whichCrossingType(a, b, c, d, tolerance_scaled, pr, checkY1, checkY2, ref overlap1, ref overlap2);

        //            if (overlap == LineOverlap.CrossingOutside)
        //            {
        //                double toleranceSq = tolerance_scaled * tolerance_scaled;
        //                Distance2D prForMinTouch = new Distance2D();

        //                int caseForMinTouch = 0;
        //                double minTouchDistSq = pointDistanceSquaredToLine2D(a, c, d, ref prForMinTouch);

        //                Distance2D tempClosest = new Distance2D();
        //                // All 3 cases must be accumulated; they are not mutually-exclusive.
        //                if (accumMin(pointDistanceSquaredToLine2D(b, c, d, ref tempClosest), ref minTouchDistSq))
        //                {
        //                    caseForMinTouch = 1;
        //                    prForMinTouch = tempClosest;
        //                }
        //                if (accumMin(pointDistanceSquaredToLine2D(c, a, b, ref tempClosest), ref minTouchDistSq))
        //                {
        //                    caseForMinTouch = 2;
        //                    prForMinTouch = tempClosest;
        //                }
        //                if (accumMin(pointDistanceSquaredToLine2D(d, a, b, ref tempClosest), ref minTouchDistSq))
        //                {
        //                    caseForMinTouch = 3;
        //                    prForMinTouch = tempClosest;
        //                }

        //                if (minTouchDistSq < toleranceSq)
        //                {
        //                    overlap = LineOverlap.CrossingTouch;

        //                    if (caseForMinTouch == 0)
        //                        pr = a;
        //                    else if (caseForMinTouch == 1)
        //                        pr = b;
        //                    else if (caseForMinTouch == 2)
        //                        pr = c;
        //                    else if (caseForMinTouch == 3)
        //                        pr = d;
        //                    else
        //                    {
        //                        // Tthis should not happen
        //                    }

        //                    Distance2D pr1;
        //                    Distance2D pr2 = new Distance2D(double.MaxValue, double.MaxValue);

        //                    if (caseForMinTouch < 2)
        //                    {
        //                        if (caseForMinTouch == 0)
        //                            pr1 = a;
        //                        else
        //                            pr1 = b;
        //                        if (caseForMinTouch == 0)
        //                            overlap1 = LineOverlap.CrossingTouchA;
        //                        else
        //                            overlap1 = LineOverlap.CrossingTouchB;

        //                        overlap2 = LineOverlap.Crossing;

        //                        if (distanceSquared(c, prForMinTouch) < toleranceSq)
        //                        {
        //                            pr2 = c;
        //                            overlap2 = LineOverlap.CrossingTouchA;
        //                        }
        //                        else if (distanceSquared(d, prForMinTouch) < toleranceSq)
        //                        {
        //                            pr2 = d;
        //                            overlap2 = LineOverlap.CrossingTouchB;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (caseForMinTouch == 2)
        //                            pr1 = c;
        //                        else
        //                            pr1 = d;

        //                        if (caseForMinTouch == 2)
        //                            overlap2 = LineOverlap.CrossingTouchA;
        //                        else
        //                            overlap2 = LineOverlap.CrossingTouchB;

        //                        overlap2 = LineOverlap.Crossing;

        //                        if (distanceSquared(a, prForMinTouch) < toleranceSq)
        //                        {
        //                            pr2 = a;
        //                            overlap2 = LineOverlap.CrossingTouchA;
        //                        }
        //                        else if (distanceSquared(b, prForMinTouch) < toleranceSq)
        //                        {
        //                            pr2 = b;
        //                            overlap2 = LineOverlap.CrossingTouchB;
        //                        }
        //                    }
        //                }
        //            }

        //            pr.X += origx1;
        //            pr.Y += origy1;

        //            return pr;
        //        }

        //        public static bool accumMin(double val, ref double min)
        //        {
        //            if (val < min)
        //            {
        //                min = val;
        //                return true;
        //            }

        //            return false;
        //        }

        //        public static Distance2D pointDeltaToLineExtended(Distance2D pt, Distance2D p1, Distance2D p2)
        //        {
        //            Distance2D closest = closestPointOnLineExtended(pt, p1, p2);

        //            if (double.IsNaN(closest.X))
        //                return new Distance2D(double.NaN, double.NaN);

        //            double deltaX = pt.X - closest.X;
        //            double deltaY = pt.Y - closest.Y;

        //            return new Distance2D(deltaX, deltaY);
        //        }

        //        public static Distance2D closestPointOnPolygonExtended(Distance2D pt, Distance2D[] pts)
        //        {
        //            Distance2D closestPt = new Distance2D();
        //            double closestDi = -1.0;

        //            for (int i = 0; i < pts.Length; i++)
        //            {
        //                Distance2D p1 = pts[i == 0 ? pts.Length - 1 : i - 1];
        //                Distance2D p2 = pts[i];

        //                Distance2D close = closestPointOnLineExtended(pt, p1, p2);

        //                double di = Distance2D.DistanceBetween(pt, close);

        //                if (closestDi == -1.0 || di < closestDi)
        //                {
        //                    closestDi = di;
        //                    closestPt = close;
        //                }
        //            }

        //            return closestPt;
        //        }

        //        public static Distance2D closestPointOnLineExtended(Distance2D pt, Distance2D p1, Distance2D p2)
        //        {
        //            Distance2D delta = delta2D(p1, p2);

        //            if (delta.X == 0 && delta.Y == 0.0)
        //                return new Distance2D(double.NaN, double.NaN);

        //            double t = calcTOfClosestPoint(pt.X, pt.Y, p1.X, p1.Y, delta.X, delta.Y);

        //            return tToPoint(t, p1, delta);
        //        }

        //        public static Distance2D tToPoint(double t, Distance2D p1, Distance2D delta)
        //        {
        //            double px = p1.X + t * delta.X;
        //            double py = p1.Y + t * delta.Y;

        //            return new Distance2D(px, py);
        //        }

        //        public static bool linesOverlapOrTouch_Parallel(Distance2D a, Distance2D b, Distance2D c, Distance2D d, double tolerance, ref LineOverlap overlap, ref LineOverlap overlap1, ref LineOverlap overlap2, ref Distance2D op2)
        //        {
        //            Distance2D op1 = new Distance2D();

        //            return linesOverlapOrTouch_Parallel(a, b, c, d, tolerance, ref overlap, ref overlap1, ref overlap2, ref op1, ref op2);
        //        }

        //        static bool handleZeroLengthLine(Distance2D a, Distance2D c, Distance2D d, double tolerance, out double t, out LineOverlap overlap)
        //        {
        //            Distance2D closestPt = closestPointOnLine_AndT(a, c, d, out t, false);
        //            if (closestPt.NearlyEquals(a, tolerance))
        //            {
        //                // TODO: Also test for TouchEnd?  If so, callers may need to test that case also.
        //                // TODO: Isn't setting overlap1 or overlap2, when one line is zero-length.
        //                overlap = LineOverlap.CrossingTouch;
        //                return true;
        //            }

        //            overlap = LineOverlap.BadData;
        //            return false;
        //        }

        //        // TODO: Isn't setting overlap1 or overlap2, when one line is zero-length.
        //        public static bool linesOverlapOrTouch_Parallel(Distance2D a, Distance2D b, Distance2D c, Distance2D d, double tolerance, ref LineOverlap overlap, ref LineOverlap overlap1, ref LineOverlap overlap2, ref Distance2D op1, ref Distance2D op2)
        //        {
        //            overlap1 = LineOverlap.Undefined;
        //            overlap2 = LineOverlap.Undefined;
        //            op1 = Distance2D.NaN();
        //            op2 = Distance2D.NaN();


        //            Distance2D delta1 = b - a;
        //            Distance2D delta2 = d - c;

        //            double t;
        //            // If the zero-length line is very close to the other line,
        //            // then consider them to be touching.
        //            // Otherwise, is bad data (return false).
        //            if (Distance2D.Equals(delta1, Distance2D.Zero()))
        //            {
        //                return handleZeroLengthLine(a, c, d, tolerance, out t, out overlap);
        //            }
        //            else if (Distance2D.Equals(delta2, Distance2D.Zero()))
        //            {
        //                return handleZeroLengthLine(c, a, b, tolerance, out t, out overlap);
        //            }
        //            //			// -- OLD OLD --
        //            //			if (Distance2D.Equals( delta1, Distance2D.zero() ) || Distance2D.Equals( delta2, Distance2D.zero() )) {
        //            //				overlap = LineOverlap.BadData;
        //            //				return false;
        //            //			}

        //            double slope1, slope2;
        //            double brx, crx, drx;
        //            const double arx = 0.0;
        //            double cry;
        //            const double ary = 0.0;

        //            if (Math.Abs(delta1.X) > Math.Abs(delta1.Y))
        //            {
        //                // More horizontal than vertical
        //                if (delta2.X == 0.0)
        //                {
        //                    overlap = LineOverlap.NotParallel;

        //                    return false;
        //                }

        //                slope1 = delta1.Y / delta1.X;
        //                slope2 = delta2.Y / delta2.X;

        //                brx = b.X - a.X;
        //                crx = c.X - a.X;
        //                drx = d.X - a.X;
        //                cry = c.Y - a.Y;
        //            }
        //            else
        //            {
        //                // More horizontal than vertical
        //                if (delta2.Y == 0.0)
        //                {
        //                    overlap = LineOverlap.NotParallel;

        //                    return false;
        //                }

        //                slope1 = delta1.X / delta1.Y;
        //                slope2 = delta2.X / delta2.Y;

        //                brx = b.Y - a.Y;
        //                crx = c.Y - a.Y;
        //                drx = d.Y - a.Y;
        //                cry = c.X - a.X;
        //            }

        //            double predictedCry = ary + slope1 * crx;

        //            if (Math.Abs(cry - predictedCry) > tolerance)
        //            {
        //                if (Math.Abs(cry - predictedCry) < 0.001)
        //                {
        //                    Rectangle2D bounds1 = Rectangle2D.FromOppositeCorners(a, b);
        //                    Rectangle2D bounds2 = Rectangle2D.FromOppositeCorners(c, d);

        //                    if (!rectanglesIntersectOrTouch(bounds1, bounds2, tolerance))
        //                    {
        //                        overlap = LineOverlap.BoundsNoOverlap;

        //                        return false;
        //                    }
        //                }

        //                double absDeltaSlope = Math.Abs(slope2 - slope1);
        //                if (absDeltaSlope > Math.Max(4.0 * tolerance, VerySmall))
        //                {
        //                    overlap = LineOverlap.NotParallel;

        //                    return false;
        //                }

        //                overlap = LineOverlap.NotColinear;

        //                return false;
        //            }


        //            if (brx < 0)
        //            {
        //                brx = -brx;
        //                crx = -crx;
        //                drx = -drx;
        //            }

        //            bool swapCD = false;

        //            if (drx < crx)
        //            {
        //                swapCD = true;
        //                double tmp = crx;

        //                crx = drx;
        //                drx = tmp;
        //            }

        //            if (drx < (arx - tolerance) || crx > (brx + tolerance))
        //            {
        //                overlap = LineOverlap.ColinearNoOverlap;

        //                return false;
        //            }

        //            if (NearlyEquals(crx, arx, tolerance))
        //            {
        //                if (NearlyEquals(drx, brx, tolerance))
        //                {
        //                    op1 = a;
        //                    op2 = b;
        //                    overlap = LineOverlap.Identical;

        //                    return true;
        //                }
        //                else if (drx < brx)
        //                {
        //                    op1 = c;
        //                    op2 = d;
        //                    overlap = LineOverlap.InsideTouchEnd;
        //                    overlap1 = LineOverlap.ContainsTouchA;
        //                    if (swapCD)
        //                        overlap2 = LineOverlap.InsideTouchB;
        //                    else
        //                        overlap2 = LineOverlap.InsideTouchA;

        //                    return true;
        //                }
        //                else
        //                {
        //                    op1 = a;
        //                    op2 = b;
        //                    overlap = LineOverlap.InsideTouchEnd;
        //                    overlap1 = LineOverlap.InsideTouchA;
        //                    if (swapCD)
        //                        overlap2 = LineOverlap.ContainsTouchB;
        //                    else
        //                        overlap2 = LineOverlap.ContainsTouchA;

        //                    return true;
        //                }
        //            }
        //            else if (NearlyEquals(crx, brx, tolerance))
        //            {
        //                op1 = b;
        //                overlap = LineOverlap.TouchEnd;
        //                overlap1 = LineOverlap.TouchEndB;
        //                if (swapCD)
        //                    overlap2 = LineOverlap.TouchEndB;
        //                else
        //                    overlap2 = LineOverlap.TouchEndA;

        //                return true;
        //            }
        //            else if (crx < arx)
        //            {
        //                if (NearlyEquals(drx, arx, tolerance))
        //                {
        //                    op1 = a;
        //                    overlap = LineOverlap.TouchEnd;
        //                    overlap1 = LineOverlap.TouchEndA;
        //                    if (swapCD)
        //                        overlap2 = LineOverlap.TouchEndA;
        //                    else
        //                        overlap2 = LineOverlap.TouchEndB;

        //                    return true;
        //                }
        //                else if (NearlyEquals(drx, brx, tolerance))
        //                {
        //                    op1 = a;
        //                    op2 = b;
        //                    overlap = LineOverlap.InsideTouchEnd;
        //                    overlap1 = LineOverlap.InsideTouchB;
        //                    if (swapCD)
        //                        overlap2 = LineOverlap.ContainsTouchA;
        //                    else
        //                        overlap2 = LineOverlap.ContainsTouchB;

        //                    return true;
        //                }
        //                else if (drx < brx)
        //                {
        //                    op1 = a;
        //                    if (swapCD)
        //                        op2 = c;
        //                    else
        //                        op2 = d;
        //                    overlap = LineOverlap.Overlap;
        //                    overlap1 = LineOverlap.OverlapA;
        //                    if (swapCD)
        //                        overlap2 = LineOverlap.OverlapA;
        //                    else
        //                        overlap2 = LineOverlap.OverlapB;

        //                    return true;
        //                }
        //                else
        //                {
        //                    op1 = a;
        //                    op2 = b;
        //                    overlap = LineOverlap.Inside;
        //                    overlap1 = LineOverlap.Inside;
        //                    overlap2 = LineOverlap.Contains;

        //                    return true;
        //                }
        //            }
        //            else
        //            {
        //                if (NearlyEquals(drx, brx, tolerance))
        //                {
        //                    op1 = c;
        //                    op2 = d;
        //                    overlap = LineOverlap.InsideTouchEnd;
        //                    overlap1 = LineOverlap.ContainsTouchB;
        //                    if (swapCD)
        //                        overlap2 = LineOverlap.InsideTouchA;
        //                    else
        //                        overlap2 = LineOverlap.InsideTouchB;

        //                    return true;
        //                }
        //                else if (drx < brx)
        //                {
        //                    op1 = c;
        //                    op2 = d;
        //                    overlap = LineOverlap.Inside;
        //                    overlap1 = LineOverlap.Contains;
        //                    overlap2 = LineOverlap.Inside;

        //                    return true;
        //                }
        //                else
        //                {
        //                    if (swapCD)
        //                        op1 = d;
        //                    else
        //                        op1 = c;

        //                    op2 = b;
        //                    overlap = LineOverlap.Overlap;
        //                    overlap1 = LineOverlap.OverlapB;
        //                    if (swapCD)
        //                        overlap2 = LineOverlap.OverlapB;
        //                    else
        //                        overlap2 = LineOverlap.OverlapA;

        //                    return true;
        //                }
        //            }
        //        }

        //        // TODO!
        //        public static bool rectanglesIntersectOrTouch(Rectangle2D r1, Rectangle2D r2, double tolerance)
        //        {
        //            return false;
        //        }


        //        public static bool firstIsShorter(double dx, double dy)
        //        {
        //            return Math.Abs(dx) < Math.Abs(dy);
        //        }

        //        public static LineOverlap whichCrossingType(Distance2D a, Distance2D b, Distance2D c, Distance2D d, double tolerance, Distance2D r, bool checkY1, bool checkY2, ref LineOverlap overlap1, ref LineOverlap overlap2)
        //        {
        //            double toleranceSq = tolerance * tolerance;
        //            bool withinTol1A = distanceSquared(r, a) <= toleranceSq;
        //            bool withinTol1B = distanceSquared(r, b) <= toleranceSq;
        //            bool withinTol2A = distanceSquared(r, c) <= toleranceSq;
        //            bool withinTol2B = distanceSquared(r, d) <= toleranceSq;

        //            if (checkY1)
        //                overlap1 = whichCrossingType(r.Y, a.Y, b.Y, withinTol1A, withinTol1B);
        //            else
        //                overlap1 = whichCrossingType(r.X, a.X, b.X, withinTol1A, withinTol1B);

        //            if (checkY2)
        //                overlap2 = whichCrossingType(r.Y, c.Y, d.Y, withinTol2A, withinTol2B);
        //            else
        //                overlap2 = whichCrossingType(r.X, c.X, d.X, withinTol2A, withinTol2B);

        //            LineOverlap overlap;

        //            if (overlap1 == LineOverlap.CrossingOutside || overlap2 == LineOverlap.CrossingOutside)
        //                overlap = LineOverlap.CrossingOutside;
        //            else if ((overlap1 == LineOverlap.CrossingTouchA || overlap2 == LineOverlap.CrossingTouchB) || (overlap2 == LineOverlap.CrossingTouchA || overlap1 == LineOverlap.CrossingTouchB))
        //                overlap = LineOverlap.CrossingTouch;
        //            else
        //                overlap = LineOverlap.Crossing;

        //            return overlap;
        //        }

        //        public static bool betweenInclusive(double val, double a, double b)
        //        {
        //            return ((a <= val)) && (val <= b) || ((b <= val) && (val <= a));
        //        }

        //        public static LineOverlap whichCrossingType(double rx, double ax, double bx, bool withinTolA, bool withinTolB)
        //        {
        //            if (withinTolA)
        //                return LineOverlap.CrossingTouchA;
        //            else if (withinTolB)
        //                return LineOverlap.CrossingTouchB;
        //            else if (betweenInclusive(rx, ax, bx))
        //                return LineOverlap.Crossing;
        //            else
        //                return LineOverlap.CrossingOutside;
        //        }


        //        // Avoids some boundary issues near 0.0..05 for larger "decimals"; does Math.Round(val, decimals) have similar logic?
        //        public static double round(double val, int decimals)
        //        {
        //            double f = Math.Pow(10, decimals);
        //            double interestedInZeroDPs = val * f;

        //            return Math.Round(interestedInZeroDPs) / f;
        //        }

        //        public static float round(float val, int decimals)
        //        {
        //            float f = (float)Math.Pow(10, decimals);
        //            float interestedInZeroDPs = val * f;

        //            return (float)Math.Round(interestedInZeroDPs) / f;
        //        }

        //        public static double round1(double val)
        //        {
        //            return round(val, 1);
        //        }

        //        public static double round3(double val)
        //        {
        //            return round(val, 3);
        //        }

        //        // "val" is rounded to nearest "1/parts".
        //        public static double roundParts(double val, int parts)
        //        {
        //            // TODO (minor): Use BigDecimal for more accuracy near half-a-part?
        //            return (double)Math.Round(val * parts) / (double)parts;
        //        }


        //        /// <summary>
        //        /// "val" is truncated to nearest "1/parts".
        //        /// </summary>
        //        /// <param name="val"></param>
        //        /// <param name="parts"></param>
        //        /// <returns></returns>
        //        public static double truncateParts(double val, int parts)
        //        {
        //            // TODO (minor): Use BigDecimal for more accuracy near half-a-part?
        //            return (double)Math.Floor(val * parts) / (double)parts;
        //        }



        //        public static double parametricBezierX(double t, double p1x, double c1x, double c2x, double p2x)
        //        {
        //            return p1x * Math.Pow((1 - t), 3) + c1x * 3 * t * Math.Pow((1 - t), 2) + c2x * 3 * Math.Pow(t, 2) * (1 - t) + p2x * Math.Pow(t, 3);
        //        }

        //        public static double parametricBezierY(double t, double p1y, double c1y, double c2y, double p2y)
        //        {
        //            return p1y * Math.Pow((1 - t), 3) + c1y * 3 * t * Math.Pow((1 - t), 2) + c2y * 3 * Math.Pow(t, 2) * (1 - t) + p2y * Math.Pow(t, 3);
        //        }

        //        // Find position along "lop", that is "fraction" of the total distance.
        //        public static Distance3D calculateFractionOnLines(Distance3D[] lop, float fraction)
        //        {
        //            if (lop == null)
        //                return new Distance3D();

        //            double diTotal = TotalLengthLines(lop);
        //            return pointAheadOnPointSequence(lop[0], lop, diTotal * fraction);
        //        }

        //        // PERFORMANCE: Can pass in pre-calculated diTotal.
        //        public static double CalculateFractionFromGeo(Distance3D[] lop, Distance3D geo, double diTotal = 0)
        //        {
        //            if (lop == null)
        //                return 0;

        //            if (diTotal <= 0)
        //                diTotal = TotalLengthLines(lop);

        //            double fracClosest = 0;
        //            double minDist = double.MaxValue;
        //            Distance3D prevPt = new Distance3D();
        //            double diPartial = 0.0f;
        //            foreach (Distance3D pt in lop)
        //            {
        //                if (prevPt == null)
        //                {   // First point.
        //                    prevPt = pt;
        //                    continue;
        //                }

        //                var legLength = calculateDistanceDD_AED(prevPt, pt);

        //                double t;
        //                var closestPt = closestPointOnLine_AndT(geo, prevPt, pt, out t);
        //                // Have we reached or passed the goal?
        //                // Because geo might not be on LOP,
        //                // Calculate possible response at each step,
        //                //   keep the one with smallest distance to closestPt.
        //                double partialLegLength = t * legLength;
        //                var frac1 = (diPartial + partialLegLength) / diTotal;
        //                double dist1 = calculateDistanceDD_AED(geo, closestPt);
        //                if (dist1 < minDist)
        //                {
        //                    minDist = dist1;
        //                    fracClosest = frac1;
        //                }

        //                diPartial += legLength;
        //                prevPt = pt;
        //            }

        //            return fracClosest;
        //        }

        //        private static double TotalLengthLines(Distance3D[] lop)
        //        {
        //            Distance3D prevPt = new Distance3D();
        //            double diTotal = 0.0f;

        //            foreach (Distance3D pt in lop)
        //            {
        //                if (prevPt == null)
        //                {
        //                    prevPt = pt;
        //                    continue;
        //                }

        //                diTotal += calculateDistanceDD_AED(prevPt, pt);

        //                prevPt = pt;
        //            }

        //            return diTotal;
        //        }

        public static Vector2 RotateByDegrees(Vector2 pos, float degrees)
        {
            return RotateByRadians(pos, degreesToRadians(degrees));
        }

        public static Vector2 RotateByRadians(Vector2 pos, float radians)
        {
            float cosR = (float)Math.Cos(radians);
            float sinR = (float)Math.Sin(radians);

            Vector2 ret = new Vector2(pos.X * cosR - pos.Y * sinR, pos.X * sinR + pos.Y * cosR);

            return ret;
        }

        public static Distance2D RotateByDegrees(Distance2D pos, double degrees)
        {
            return rotateByRadians(pos, degreesToRadians(degrees));
        }

        public static Distance2D rotateByRadians(Distance2D pos, double radians)
        {
            double cosR = Math.Cos(radians);
            double sinR = Math.Sin(radians);

            Distance2D ret = new Distance2D(pos.X * cosR - pos.Y * sinR, pos.X * sinR + pos.Y * cosR);

            return ret;
        }

        //public static Distance2D rotateAtByDegrees(Distance2D origo, Distance2D point, double degrees)
        //{
        //    return rotateAtByRadians(origo, point, degreesToRadians(degrees));
        //}

        //        public static Distance2D rotateAtByRadians(Distance2D origo, Distance2D point, double radians)
        //        {
        //            Distance2D ret = new Distance2D(origo);
        //            double cosR = Math.Cos(radians);
        //            double sinR = Math.Sin(radians);

        //            ret.X += (point.X - origo.X) * cosR - (point.Y - origo.Y) * sinR;
        //            ret.Y += (point.X - origo.X) * sinR + (point.Y - origo.Y) * cosR;

        //            return ret;
        //        }

        //        public static Vector2 rotateAtByDegrees(Vector2 origo, Vector2 point, float degrees)
        //        {
        //            return rotateAtByRadians(origo, point, degreesToRadians(degrees));
        //        }

        //        public static Vector2 rotateAtByRadians(Vector2 origo, Vector2 point, float radians)
        //        {
        //            Vector2 ret = origo;   // Clone. (For a struct, can simply assign.)
        //            double cosR = Math.Cos(radians);
        //            double sinR = Math.Sin(radians);

        //            ret.X += (float)((point.X - origo.X) * cosR - (point.Y - origo.Y) * sinR);
        //            ret.Y += (float)((point.X - origo.X) * sinR + (point.Y - origo.Y) * cosR);

        //            return ret;
        //        }

        //        public static bool pointInsidePolygon(Distance2D[] pts, Distance2D pt, bool testing = false)
        //        {
        //            if (pts == null)
        //                return false;

        //            bool odd = false;
        //            int length, i, j;
        //            double ix, iy, jx, jy, x, y;

        //            if (pts[0].X == pts[pts.Length - 1].X && pts[0].Y == pts[pts.Length - 1].Y)
        //                length = pts.Length - 1;
        //            else
        //                length = pts.Length;

        //            // CLOSED shape: INCLUDE "i == length - 1".
        //            // WRAPS: when "i == length - 1", j = 0.
        //            for (i = 0; i < length; i++)
        //            {
        //                j = (i + 1) % length;
        //                ix = pts[i].X;
        //                iy = pts[i].Y;
        //                jx = pts[j].X;
        //                jy = pts[j].Y;
        //                x = pt.X;
        //                y = pt.Y;

        //                if ((iy < y && jy >= y) || (jy < y && iy >= y))
        //                {
        //                    if (ix + (y - iy) / (jy - iy) * (jx - ix) < x)
        //                        odd = !odd;
        //                }
        //            }

        //            if (testing)
        //                Test();
        //            return odd;
        //        }

        //        public static Distance2D extendLine(Distance2D p1, Distance2D p2, double di)
        //        {
        //            double totalDi = distance(p1, p2) + di;
        //            double angle = getAngleDegrees(p1.X, p1.Y, p2.X, p2.Y);

        //            return new Distance2D(p1.X + totalDi * Math.Cos((angle / 180) * Math.PI), p1.Y + totalDi * Math.Sin((angle / 180) * Math.PI));
        //        }

        //        public static double ToRadians(double angle)
        //        {
        //            return (Math.PI / 180.0) * angle;
        //        }

        //        public static float ToRadians(float angle)
        //        {
        //            return ((float)Math.PI / 180.0f) * angle;
        //        }


        //        // ========== interpolateThreePoints ==========

        //        public static double interpolateThreePoints(Distance3D[] closestGeos, double[] minDists)
        //        {
        //            if (closestGeos[0] == null)
        //                // Arbitrary - no data.
        //                return 0;
        //            if (closestGeos[1] == null)
        //                return closestGeos[0].Z;
        //            if (closestGeos[2] == null)
        //            {
        //                Tuple<Distance3D, double> geoWithDistOnly2 = interpolate2(closestGeos[0], minDists[0], closestGeos[1], minDists[1], true);
        //                if (geoWithDistOnly2?.Item1 == null)
        //                    // Weights must be bad; average the 2 data that exist.
        //                    return average(closestGeos[0].Z, closestGeos[1].Z);
        //                return geoWithDistOnly2.Item1.Z;
        //            }

        //            // --- interpolate between the three points. ---
        //            // If no points exist, return NaN.
        //            // Interpolate the CLOSEST 2 of the 3, then interpolate result with farthest.
        //            Tuple<Distance3D, double> geoWithDist2a = interpolate2(closestGeos[0], minDists[0], closestGeos[1], minDists[1], true);
        //            Tuple<Distance3D, double> geoWithDist2b = interpolate2(geoWithDist2a.Item1, geoWithDist2a.Item2, closestGeos[2], minDists[2], true);
        //            // --- alternative --
        //            // Interpolate the FARTHEST 2 of the 3, then interpolate result with closest.
        //            Tuple<Distance3D, double> geoWithDist1a = interpolate2(closestGeos[2], minDists[2], closestGeos[1], minDists[1], true);
        //            Tuple<Distance3D, double> geoWithDist1b = interpolate2(geoWithDist1a.Item1, geoWithDist1a.Item2, closestGeos[0], minDists[0], true);
        //            // Average the two approaches. TODO: Is either approach "better"; just do that?
        //            if (geoWithDist1b?.Item1 == null || geoWithDist2b?.Item1 == null)
        //                // Weights must be bad; average the data directly.
        //                return (closestGeos[0].Z + closestGeos[1].Z + closestGeos[2].Z) / 3;
        //            return Average(geoWithDist2b.Item1.Z, geoWithDist1b.Item1.Z);
        //        }

        //        private static double interpolate2_getZ(Distance3D closest1Geo, double minDistanceOrSq1, Distance3D closest2Geo, double minDistanceOrSq2, bool areDistanceSquareds)
        //        {
        //            Tuple<Distance3D, double> geoWithDist = interpolate2(closest1Geo, minDistanceOrSq1, closest2Geo, minDistanceOrSq2, areDistanceSquareds);
        //            return geoWithDist.Item1.Z;
        //        }

        //        // When areDistanceSquareds, "minDistanceOrSq1/2" are "minDistanceSq"s,
        //        // when false, .. are "minDistance"s.
        //        // Return Pair<Distance3D, Double>(resultGeo, resultDist).
        //        // If no points exist, return NaN.
        //        // NOTE: Despite "geo" in param names, is doing interpolation as if in
        //        //   an orthogonal coordinate system.
        //        private static Tuple<Distance3D, double> interpolate2(Distance3D closest1Geo, double minDistanceOrSq1, Distance3D closest2Geo, double minDistanceOrSq2, bool areDistanceSquareds)
        //        {
        //            if ((closest1Geo == null) && (closest2Geo == null))
        //                return new Tuple<Distance3D, double>(new Distance3D(), double.NaN);
        //            else if ((closest2Geo == null) || (minDistanceOrSq1 == 0.0D))
        //                return new Tuple<Distance3D, double>(closest1Geo, minDistanceOrSq1);
        //            else if ((closest1Geo == null) || (minDistanceOrSq2 == 0.0))
        //                return new Tuple<Distance3D, double>(closest2Geo, minDistanceOrSq2);
        //            else
        //            {
        //                // Interpolate between the two geos, based on distance.
        //                double distance1, distance2;
        //                if (areDistanceSquareds)
        //                {
        //                    distance1 = Math.Sqrt(minDistanceOrSq1);
        //                    distance2 = Math.Sqrt(minDistanceOrSq2);
        //                }
        //                else
        //                {
        //                    distance1 = minDistanceOrSq1;
        //                    distance2 = minDistanceOrSq2;
        //                }
        //                //
        //                // Divide-by-zero won't happen: Zero distances caught above.
        //                //			//
        //                //			// The closer we are, the greater the weight.
        //                //			double wgt1x = 1 / distance1;
        //                //			double wgt2x = 1 / distance2;
        //                //			double wgt2xNorm = wgt2x / (wgt1x + wgt2x);
        //                //
        //                // The closer we are, the greater the weight. (This version avoids divide by very small value sometimes.)
        //                // Equivalent to wgt1 = (1/distance1), wgt2 = (1/distance2);
        //                double wgt1raw = distance2;
        //                double wgt2raw = distance1;
        //                double wgt2norm = wgt2raw / (wgt1raw + wgt2raw);
        //                //
        //                //			if (!MathHelper.nearlyEquals(wgt2xNorm, wgt2norm, MathHelper.NearZero))
        //                //				Dubious();
        //                Distance3D resultGeo = Lerp(closest1Geo, closest2Geo, wgt2norm);
        //                // TODO: Is this valid?  If not, when 3 points, need to work with all 3 at once.
        //                double resultDist = Lerp(distance1, distance2, wgt2norm);
        //                if (areDistanceSquareds)
        //                    resultDist = resultDist * resultDist;
        //                return new Tuple<Distance3D, double>(resultGeo, resultDist);
        //            }
        //        }



        //        // ========== polygon triangulation; vertex in triangle ==========

        //        //public static List<Triangle> TriangulatePolygon()
        //        //{

        //        //}

        //        public static bool IsInsideTriangle(Distance3D pt, Distance3D a, Distance3D b, Distance3D c)
        //        {
        //            return IsInsideTriangle(pt.To2D(), a.To2D(), b.To2D(), c.To2D());
        //        }

        //        // "parametric equations" technique. Each point in plane is a linear combination of vectors (a->b) and (a->c).
        //        // The final line checks whether the coefficients (u, v) of the representation of "pt"
        //        // falls within the unit triangle "A=>(u:0, v:0),  B=>(u:1, v:0),  C=>(u:0, v:1)".
        //        public static bool IsInsideTriangle(Distance2D pt, Distance2D a, Distance2D b, Distance2D c)
        //        {
        //            // "relative p", "relative b", "relative c": values relative to "a". That is, treat "a" as origin (0, 0).
        //            Distance2D rP = pt - a;
        //            Distance2D rB = b - a;
        //            Distance2D rC = c - a;

        //            double dotBB = rB.X * rB.X + rB.Y * rB.Y;
        //            double dotBC = rB.X * rC.X + rB.Y * rC.Y;
        //            double dotCC = rC.X * rC.X + rC.Y * rC.Y;

        //            // dot product of rP and rB.
        //            double dotPB = rP.X * rB.X + rP.Y * rB.Y;
        //            // dot product of rP and rC.
        //            double dotPC = rP.X * rC.X + rP.Y * rC.Y;

        //            // determinant.
        //            double det = dotBB * dotCC - dotBC * dotBC;

        //            // A=>(u:0, v:0),  B=>(u:1, v:0),  C=>(u:0, v:1).
        //            // When v=0 => a point along A-B.
        //            // When v=0 => a point along A-C.
        //            // When u+v=1 => a point along B-C.
        //            double u = (dotPB * dotCC - dotPC * dotBC) / det;
        //            double v = (dotPC * dotBB - dotPB * dotBC) / det;

        //            // True if the coefficients (u, v) of the representation of "pt"
        //            // falls within the unit triangle "A=>(u:0, v:0),  B=>(u:1, v:0),  C=>(u:0, v:1)".
        //            return (u >= 0.0) && (u <= 1.0) && (v >= 0) && (v <= 1) && (u + v <= 1);
        //        }


        /// <summary>
        /// Not an extension method, so will work with properties.
        /// Usage: dst = CopyXZ(src, dst);
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static Vector3 CopyXZ(Vector3 src, Vector3 dst)
        {
            return new Vector3(src.X, dst.Y, src.Z);
        }

        /// <summary>
        /// Not an extension method, so will work with properties.
        /// Usage: dst = SetAltitude(dst, altitude);
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static Vector3 SetAltitude(Vector3 dst, float altitude)
        {
            return new Vector3(dst.X, altitude, dst.Z);
        }
    }
}
