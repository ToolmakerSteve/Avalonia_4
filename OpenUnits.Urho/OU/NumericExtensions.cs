using System;
using System.Collections.Generic;
using System.Text;
using Urho;

namespace OU
{
	public static class OU_NumericExtensions
	{
		// Number to Distance - extensions
		static public DistD asDist(this double val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDist(this float val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDist(this int val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDistD(this int val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDist(this long val) { return DistD.FromDefaultUnits(val); }
		static public DistD asDistD(this long val) { return DistD.FromDefaultUnits(val); }

		static public DistD asMeters(this int val) { return DistD.FromMeters(val); }


        static public Dist2D asDist(this Vector2 v2) { return new Dist2D(DistD.FromDefaultUnits(v2.X), DistD.FromDefaultUnits(v2.Y)); }
        static public Dist2D asMeters(this Vector2 v2) { return Dist2D.FromMeters(v2.X, v2.Y); }



        /// <summary>
        /// </summary>
        /// <param name="it"></param>
        /// <returns>it becomes X and Z fields.</returns>
        static public Vec3 AsXZ(this Vec2 it)
        {
            return new Vec3(it.X, 0, it.Y);
        }
    }
}
