using OU;
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
		/// Access 2 elements as a Vec2.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vec2 AsVector2(float[] vertexData, uint offset)
		{
			return new Vec2(vertexData[offset],
							   vertexData[offset + 1]);
		}

		/// <summary>
		/// Access 3 elements as a Vec3.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vec3 AsVector3(float[] vertexData, uint offset)
		{
			return new Vec3(vertexData[offset],
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
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>pt.Z becomes Vec2.Y</returns>
		static public Vec2 XZ(this Vec3 pt)
		{
			return new Vec2(pt.X, pt.Z);
		}

		/// <summary>
		/// </summary>
		/// <param name="pt"></param>
		/// <returns>pt.Z becomes Vec2.Y</returns>
		static public Vec2 XZ(this Vec3 pt)
		{
			return new Vec2(pt.X, pt.Z);
		}

		/// <summary>
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="altitude"></param>
		/// <returns>altitude becomes Vec3.Y, pt.Y becomes Vec3.Z</returns>
		static public Vec3 FromXZ(this Vec2 pt, float altitude = 0)
        {
            return new Vec3(pt.X, altitude, pt.Y);
        }


		/// <summary>
		/// REQUIRE terrain and srcPt are in same orthographic projection.
		/// </summary>
		/// <param name="terrain"></param>
		/// <param name="srcPt"></param>
		/// <returns></returns>
		static public Vec3 PlaceOnTerrain(Terrain terrain, Vec2 srcPt, float altitude = 0)
		{
			Vec3 destPt = srcPt.AsXZ();
			// Y is altitude.
			destPt.Y = GetTerrainHeight(terrain, destPt) + altitude;
			return destPt;
		}

		static public float GetTerrainHeight(Terrain terrain, Vec3 pt)
		{
			return terrain == null ? 0 : terrain.GetHeight(pt);
		}
		#endregion
	}
}
