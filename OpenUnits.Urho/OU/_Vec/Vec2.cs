using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
	// SAME AS Numerics - Vector2
	/// <summary>Double-precision two-dimensional (XY) unitless vector value.   Same as a Vector2, but with double-precision.</summary>
	public struct Vec2
	{
		public class Array : List<Vec2> // NOTE: this <Vector2> would be auto-gen'ed to the specified preferred library -- like Urho... or defaults to Numerics.Vector2
		{
		}

		#region --- data ----------------------------------------
		public float X;
		public float Y;
		#endregion


		#region --- new ----------------------------------------
		public Vec2(float x, float y)
		{
			X = x;
			Y = y;
		}
        #endregion


        static public implicit operator Urho.Vector2(Vec2 v2)
        {
            return new Urho.Vector2(v2.X, v2.Y);
        }

        static public implicit operator Vec2(Urho.Vector2 v2)
        {
            return new Vec2(v2.X, v2.Y);
        }

    }
}
