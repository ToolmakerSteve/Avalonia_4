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
    }
}
