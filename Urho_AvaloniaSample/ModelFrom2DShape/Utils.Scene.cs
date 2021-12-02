using System;
using System.Collections.Generic;
using Urho;

namespace Global
{
    /// <summary>
    /// Scene helper functions.
    /// </summary>
    static public partial class Utils
    {
		#region --- Vertex Data: Standard element sizes ----------------------------------------
		public const uint FloatsForVertexPosition = 3;
		public const uint FloatsForVertexNormal = 3;
		public const uint FloatsForVertexUV = 2;
		public const uint FloatsForVertexTangent = 4;

		/// <summary>
		/// Access 2 elements as a Vector2.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vector2 AsVector2(float[] vertexData, uint offset)
		{
			return new Vector2(vertexData[offset],
							   vertexData[offset + 1]);
		}

		/// <summary>
		/// Access 3 elements as a Vector3.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vector3 AsVector3(float[] vertexData, uint offset)
		{
			return new Vector3(vertexData[offset],
							   vertexData[offset + 1],
							   vertexData[offset + 2]);
		}

		/// <summary>
		/// Access 4 elements as a Vector4.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vector4 AsVector4(float[] vertexData, uint offset)
		{
			return new Vector4(vertexData[offset],
							   vertexData[offset + 1],
							   vertexData[offset + 2],
							   vertexData[offset + 3]);
		}

		#endregion

		#region --- terrain in XZ plane ----------------------------------------
		/// <summary>
		/// 
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>pt.Z becomes Vector2.Y</returns>
		static public Vector2 XZ(this Vector3 pt)
        {
            return new Vector2(pt.X, pt.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="altitude"></param>
        /// <returns>altitude becomes Vector3.Y, pt.Y becomes Vector3.Z</returns>
        static public Vector3 FromXZ(this Vector2 pt, float altitude = 0)
        {
            return new Vector3(pt.X, altitude, pt.Y);
        }


		/// <summary>
		/// REQUIRE terrain and srcPt are in same orthographic projection.
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="srcPt"></param>
		/// <returns></returns>
		static public Vector3 PlaceOnTerrain(Terrain terrain, Vector2 srcPt, float altitude = 0)
		{
			Vector3 destPt = srcPt.ToXZ();
			// Y is altitude.
			destPt.Y = GetTerrainHeight(terrain, destPt) + altitude;
			return destPt;
		}

		static public float GetTerrainHeight(Terrain terrain, Vector3 pt)
		{
			return terrain == null ? 0 : terrain.GetHeight(pt);
		}
		#endregion
	}
}
