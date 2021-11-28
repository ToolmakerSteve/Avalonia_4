using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	/// <summary>Double-precision two-dimensional (XY) unitless vector value.   Same as a Vector2, but with double-precision.</summary>
	public struct Vec2D
	{
		#region --- data ----------------------------------------
		public double X;
		public double Y;
		#endregion


		#region --- new ----------------------------------------
		public Vec2D(double x, double y)
		{
			X = x;
			Y = y;
		}
		#endregion
	}
}
