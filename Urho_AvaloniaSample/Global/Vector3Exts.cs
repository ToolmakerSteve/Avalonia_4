using System;
using System.Collections.Generic;
using Urho;

namespace Global
{
    public static class Vector3Exts
    {
        public static float Altitude(this Vector3 vec)
        {
            // XZ is ground plane, Y is Altitude.
            return vec.Y;
        }

        /// <summary>
        /// Extension method; won't work with properties.
        /// Usage: dst = SetAltitude(dst, altitude);
        /// </summary>
        /// <param name="altitude"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static void SetAltitude(this Vector3 dst, float altitude)
        {
            dst.Y = altitude;
        }

        public static Vector3 CopyXZTo(this Vector3 src, Vector3 dst)
        {
            return new Vector3(src.X, dst.Y, src.Z);
        }
    }
}
