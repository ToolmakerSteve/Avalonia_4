using System;
using System.Collections.Generic;
using Urho;
using Urho.Gui;
using PointF = System.Drawing.PointF;

namespace LineLayer3D
{
	public partial class LineLayer3D
	{
		public class Line3D //: GeoShape.IRecyclableContent
		{
			public uint VertexCount;
			public int LineCount;
			public float[] VertexData;
			public int[] IndexData;

			public bool WasRecycled;

			public void Recycle()
			{
				WasRecycled = true;
			}
		}
	}
}
