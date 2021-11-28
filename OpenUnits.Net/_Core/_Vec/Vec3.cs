using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	// SAME AS Numerics - Vector2
	/// <summary>Double-precision two-dimensional (XY) unitless vector value.   Same as a Vector2, but with double-precision.</summary>
	public struct Vec3
	{
		public class Array : List<Vec3> // NOTE: this <Vector2> would be auto-gen'ed to the specified preferred library -- like Urho... or defaults to Numerics.Vector2
		{
		}

		#region --- data ----------------------------------------
		public float X;
		public float Y;
		public float Z;
		#endregion


		#region --- new ----------------------------------------
		public Vec3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}
		#endregion



	}
}
