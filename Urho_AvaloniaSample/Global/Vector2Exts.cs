using System;
using System.Collections.Generic;
using Urho;

namespace Global
{
    public static class Vector2Exts
    {
        public static Vector3 ToXZ(this Vector2 it)
        {
            return new Vector3(it.X, 0, it.Y);
        }
    }
}
