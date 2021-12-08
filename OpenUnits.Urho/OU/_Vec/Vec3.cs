using System;
using System.Collections.Generic;
using System.Text;

namespace OU
{
    // SAME AS Numerics - Vector3
    public partial struct Vec3
	{
        #region --- class Array ----------------------------------------
        public class Array : List<Vec3> // NOTE: this <Vector2> would be auto-gen'ed to the specified preferred library -- like Urho... or defaults to Numerics.Vector2
		{
		}
        #endregion

        #region --- data (See Vec3.Intrinsics.cs) ----------------------------------------
        #endregion

        #region --- new (See Vec3.Intrinsics.cs) ----------------------------------------
        #endregion

        #region --- implicit operators ----------------------------------------
        static public implicit operator Urho.Vector3(Vec3 vec)
        {
            return new Urho.Vector3(vec.X, vec.Y, vec.Z);
        }

        static public implicit operator Vec3(Urho.Vector3 vec)
        {
            return new Vec3(vec.X, vec.Y, vec.Z);
        }
        #endregion
    }
}
