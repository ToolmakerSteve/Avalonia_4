using System;
using System.Collections.Generic;
using Urho;

namespace Global
{
    /// <summary>
    /// Scene helper functions.
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// REQUIRE terrain and srcPt are in same orthographic projection.
        /// </summary>
        /// <param name="terrain"></param>
        /// <param name="srcPt"></param>
        /// <returns></returns>
        public static Vector3 PlaceOnTerrain(Terrain terrain, Vector2 srcPt, float altitude = 0)
        {
            Vector3 destPt = srcPt.ToXZ();
            // Y is altitude.
            destPt.Y = (terrain != null) ? (terrain.GetHeight(destPt) + altitude) : 0;
            return destPt;
        }


        #region --- terrain in XZ plane ----------------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <returns>pt.Z becomes Vector2.Y</returns>
        public static Vector2 XZ(this Vector3 pt)
        {
            return new Vector2(pt.X, pt.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="altitude"></param>
        /// <returns>altitude becomes Vector3.Y, pt.Y becomes Vector3.Z</returns>
        public static Vector3 FromXZ(this Vector2 pt, float altitude = 0)
        {
            return new Vector3(pt.X, altitude, pt.Y);
        }
        #endregion
    }
}
