using System;
using Urho;

namespace Global
{
	static public class IntRectExts
	{
		static public float Width(this IntRect rect)
		{
			return rect.Right - rect.Left;
		}

		static public float Height(this IntRect rect)
		{
			// Abs in case Y-flipped.
			return Math.Abs(rect.Bottom - rect.Top);
		}
	}
}
