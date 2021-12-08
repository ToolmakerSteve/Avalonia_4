using System;
using System.Collections.Generic;
using Urho;

namespace Global
{
    public static class Vector2Exts
    {
		/// <summary>
		/// </summary>
		/// <param name="it"></param>
		/// <returns>it becomes X and Z fields.</returns>
        public static Vector3 AsXZ(this Vector2 it)
        {
            return new Vector3(it.X, 0, it.Y);
        }
    }
}
