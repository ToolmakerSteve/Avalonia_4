using System;
using System.Collections.Generic;
using System.Text;
using Urho;

namespace ModelFrom2DShape
{
	/// <summary>
	/// Translated from https://github.com/urho3d/Urho3D/blob/master/Source/Urho3D/Graphics/Tangent.cpp
	/// The MIT License (MIT)
	/// Copyright(c) 2008-2021 the Urho3D project.
	/// </summary>
	internal class Tangent1
	{
		/// <summary>
		/// Calculate and Set tangent (used with Normal Map) on each vertex.
		/// REQUIRE: Offsets in "# elements" not "# bytes".
		/// OPTIMIZE: Can set TI to given type, then remove "Convert" calls. .NET 7 generic math will improve this.
		/// MAKE FLEXIBLE: .NET 7 generic math should allow vertexData to be any numeric type ("TVertex").
		///   "SetVector4" is currently forcing us to specify a type.
		/// </summary>
		/// <typeparam name="TIndex"></typeparam>
		/// <param name="vertexData"></param>
		/// <param name="vertexSize"># floats (or #"TVertex") per vertex</param>
		/// <param name="indexData"></param>
		/// <param name="indexStart">Often 0.</param>
		/// <param name="indexCount"># indices to be processed.</param>
		/// <param name="normalOffset">w/i vertex, in # floats (or #"TVertex")</param>
		/// <param name="texCoordOffset">aka "UV". w/i vertex, in # floats (or #"TVertex")</param>
		/// <param name="tangentOffset">w/i vertex, in # floats (or #"TVertex")</param>
		public static void GenerateTangents<TIndex>(float[] vertexData, uint vertexSize, TIndex[] indexData, uint indexStart, uint indexCount, uint normalOffset, uint texCoordOffset, uint tangentOffset)
		{
			// Tangent generation from
			// http://www.terathon.com/code/tangent.html
			int minVertexIndex = int.MaxValue;
			int maxVertexIndex = 0;
			// CAUTION: I've changed from "byte" to "TVertex[]" or "TFloat[]", so offsets must be in "# elements" not "# bytes".
			//var vertices = vertexData;

			// Each vertex may be used multiple times.
			// For efficiency, find RANGE of vertex indices to convert.
			for (uint ii = indexStart; ii < indexStart + indexCount; ++ii) {
				var index = Convert.ToInt32(indexData[ii]);

				// Accum min-max.
				if (index < minVertexIndex) {
					minVertexIndex = index;
				}
				if (index > maxVertexIndex) {
					maxVertexIndex = index;
				}
			}

			int vertexCount = maxVertexIndex + 1;
			Vector3[] tan1;
			Arrays.InitializeWithDefaultInstances(out tan1, vertexCount * 2);
			// in unsafe code, this would be a pointer into middle of tan1.
			//Vector3[] tan2 = tan1 + vertexCount;
			int iTan2 = vertexCount;

			for (uint ii = indexStart; ii < indexStart + indexCount; ii += 3) {
				uint i1 = Convert.ToUInt32(indexData[ii]);
				uint i2 = Convert.ToUInt32(indexData[ii + 1]);
				uint i3 = Convert.ToUInt32(indexData[ii + 2]);

				Vector3 v1 = AsVector3(vertexData, i1 * vertexSize);
				Vector3 v2 = AsVector3(vertexData, i2 * vertexSize);
				Vector3 v3 = AsVector3(vertexData, i3 * vertexSize);

				Vector2 w1 = AsVector2(vertexData, i1 * vertexSize + texCoordOffset);
				Vector2 w2 = AsVector2(vertexData, i2 * vertexSize + texCoordOffset);
				Vector2 w3 = AsVector2(vertexData, i3 * vertexSize + texCoordOffset);

				float x1 = v2.X - v1.X;
				float x2 = v3.X - v1.X;
				float y1 = v2.Y - v1.Y;
				float y2 = v3.Y - v1.Y;
				float z1 = v2.Z - v1.Z;
				float z2 = v3.Z - v1.Z;

				float s1 = w2.X - w1.X;
				float s2 = w3.X - w1.X;
				float t1 = w2.Y - w1.Y;
				float t2 = w3.Y - w1.Y;

				float r = 1.0f / (s1 * t2 - s2 * t1);
				Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
				Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

				tan1[i1] += sdir;
				tan1[i2] += sdir;
				tan1[i3] += sdir;

				tan1[iTan2 + i1] += tdir;
				tan1[iTan2 + i2] += tdir;
				tan1[iTan2 + i3] += tdir;
			}
			for (int i = minVertexIndex; i <= maxVertexIndex; i++) {
				Vector3 n = AsVector3(vertexData, (uint)(i * vertexSize + normalOffset));
				Vector3 t = tan1[i];
				Vector3 xyz = new Vector3();
				float w;

				// Gram-Schmidt orthogonalize
				Vector3 temp1 = t - n * Vector3.Dot(n, t);
				temp1.Normalize();
				xyz = temp1;

				// Calculate handedness
				w = Vector3.Dot(Vector3.Cross(n, t), tan1[iTan2 + i]) < 0.0f ? -1.0f : 1.0f;

				Vector4 tangent = new Vector4(xyz, w);
				SetVector4(vertexData, (uint)(i * vertexSize + tangentOffset), tangent);
			}

			tan1 = null;   // OPTIONAL; goes away when exit scope.
		}


		#region --- static Helpers -----------------------------------------
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


		/// <summary>
		/// Access 2 elements as a Vector2.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <typeparam name="TV"></typeparam>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vector2 AsVector2<TV>(TV[] vertexData, uint offset)
		{
			return new Vector2(Convert.ToSingle(vertexData[offset]),
							   Convert.ToSingle(vertexData[offset + 1]));
		}

		/// <summary>
		/// Access 3 elements as a Vector3.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <typeparam name="TV"></typeparam>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vector3 AsVector3<TV>(TV[] vertexData, uint offset)
		{
			return new Vector3(Convert.ToSingle(vertexData[offset]),
							   Convert.ToSingle(vertexData[offset + 1]),
							   Convert.ToSingle(vertexData[offset + 2]));
		}

		/// <summary>
		/// Access 4 elements as a Vector4.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// </summary>
		/// <typeparam name="TV"></typeparam>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to get. Units are "# elements" not "# bytes"</param>
		/// <returns></returns>
		public static Vector4 AsVector4<TV>(TV[] vertexData, uint offset)
		{
			return new Vector4(Convert.ToSingle(vertexData[offset]),
							   Convert.ToSingle(vertexData[offset + 1]),
							   Convert.ToSingle(vertexData[offset + 2]),
							   Convert.ToSingle(vertexData[offset + 3]));
		}


		/// <summary>
		/// Set 4 elements to the fields of a Vector4.
		/// REQUIRE: Offset in "# elements" not "# bytes".
		/// MAKE FLEXIBLE: .NET 7 generic math should allow vertexData to be any numeric type ("TVertex").
		/// </summary>
		/// <param name="vertexData"></param>
		/// <param name="offset">to first element to set. Units are "# elements" not "# bytes"</param>
		/// <param name="value"></param>
		public static void SetVector4(float[] vertexData, uint offset, Vector4 value)
		{
			vertexData[offset] = value.X;
			vertexData[offset + 1] = value.Y;
			vertexData[offset + 2] = value.Z;
			vertexData[offset + 3] = value.W;
		}
		#endregion
	}



	#region --- static class Arrays -----------------------------------------
	//Helper class added by C++ to C# Converter:

	//----------------------------------------------------------------------------------------
	//	Copyright © 2006 - 2021 Tangible Software Solutions, Inc.
	//	This class can be used by anyone provided that the copyright notice remains intact.
	//
	//	This class provides the ability to initialize and delete array elements.
	//----------------------------------------------------------------------------------------
	internal static class Arrays
	{
		public static void InitializeWithDefaultInstances<T>(out T[] array, int length) where T : new()
		{
			array = new T[length];
			for (int i = 0; i < length; i++) {
				array[i] = new T();
			}
		}

		public static string[] InitializeStringArrayWithDefaultInstances(int length)
		{
			string[] array = new string[length];
			for (int i = 0; i < length; i++) {
				array[i] = "";
			}
			return array;
		}

		public static T[] PadWithNull<T>(int length, T[] existingItems) where T : class
		{
			if (length > existingItems.Length) {
				T[] array = new T[length];

				for (int i = 0; i < existingItems.Length; i++) {
					array[i] = existingItems[i];
				}

				return array;
			} else
				return existingItems;
		}

		public static T[] PadValueTypeArrayWithDefaultInstances<T>(int length, T[] existingItems) where T : struct
		{
			if (length > existingItems.Length) {
				T[] array = new T[length];

				for (int i = 0; i < existingItems.Length; i++) {
					array[i] = existingItems[i];
				}

				return array;
			} else
				return existingItems;
		}

		public static T[] PadReferenceTypeArrayWithDefaultInstances<T>(int length, T[] existingItems) where T : class, new()
		{
			if (length > existingItems.Length) {
				T[] array = new T[length];

				for (int i = 0; i < existingItems.Length; i++) {
					array[i] = existingItems[i];
				}

				for (int i = existingItems.Length; i < length; i++) {
					array[i] = new T();
				}

				return array;
			} else
				return existingItems;
		}

		public static string[] PadStringArrayWithDefaultInstances(int length, string[] existingItems)
		{
			if (length > existingItems.Length) {
				string[] array = new string[length];

				for (int i = 0; i < existingItems.Length; i++) {
					array[i] = existingItems[i];
				}

				for (int i = existingItems.Length; i < length; i++) {
					array[i] = "";
				}

				return array;
			} else
				return existingItems;
		}

		public static void DeleteArray<T>(T[] array) where T : System.IDisposable
		{
			foreach (T element in array) {
				if (element != null)
					element.Dispose();
			}
		}
	}
	#endregion
}
