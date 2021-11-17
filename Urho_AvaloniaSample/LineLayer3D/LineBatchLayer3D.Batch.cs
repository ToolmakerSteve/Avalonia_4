//using System;
//using System.Collections.Generic;
//using Urho;
//using Urho.Gui;
//using PointF = System.Drawing.PointF;

//namespace AvaloniaSample.LineLayer3D
//{
//	public partial class LineBatchLayer3D
//	{
//		public struct Line
//		{
//			public int LineIndex;
//			public override string ToString()
//			{
//				return "Line#" + LineIndex;
//			}
//		}

//		public const int VerticesPerLine = 4;
//		public const int FloatsPerVertex = 6;
//		public const int FloatsPerLine = VerticesPerLine * FloatsPerVertex;
//		public const int IndicesPerLine = 6;

//		public class Batch : GeoShape.IRecyclableContent
//		{
//			public LineBatchLayer3D Layer { get; private set; }

//			public int LineCount = 0;
//			public RenderOrder RenderOrder { get; private set; }
//			public int LineCapacity { get; private set; }
//			public Color LineColor { get; private set; }
//			public float LineWidth { get; private set; }

//			public uint VertexCount;
//			public uint IndexDataSize;
//			public float[] VertexData;
//			public uint[] IndexData;

//			public StaticModel Model;
//			public Geometry Geometry;
//			public VertexBuffer Vertices;
//			public IndexBuffer Indices;

//			//public uint VertexDataSize;

//			public Material Material;
//			public List<GeoShape> Shapes;
//			public BoundingBox Bounds;

//			public bool WasRecycled;

//			public void Recycle()
//			{
//				WasRecycled = true;
//			}

//			public Vector3 FadeOff = new Vector3(1, 2, 1);

//			public Batch(LineBatchLayer3D layer, int initialCapacity, RenderOrder ro, Color lineColor, float lineWidth)
//			{
//				Layer = layer;
//				LineCapacity = initialCapacity;
//				RenderOrder = ro;
//				LineColor = lineColor;
//				LineWidth = lineWidth;

//				_OnConstruct();
//				_Configure();
//			}
//			private void _OnConstruct()
//			{
//				Material = Layer.AppContext.Materials.Get3D(MaterialID.SmoothLine3DClose, RenderOrder);
//				Material.SetShaderParameter("Color", LineColor);
//				Material.SetShaderParameter("LineWidth", LineWidth);// LineWidth);
//				Material.SetShaderParameter("LgShift", 0f);
//				Material.SetShaderParameter("Falloff", FadeOff);// _config.BorderFactor, _config.AlphaFactor, 0));
//				//Material.DepthBias = s_BiasParameters;

//				Model = Layer.Node.CreateComponent<StaticModel>();
//				Vertices = new VertexBuffer(Layer.AppContext.UrhoContext, false);
//				Indices = new IndexBuffer(Layer.AppContext.UrhoContext, false);

//				Geometry = new Geometry();
//				Geometry.SetVertexBuffer(0, Vertices);
//				Geometry.IndexBuffer = Indices;

//				Model.SetMaterial(Material);

//				Bounds = Projection3D.FullBounds;
//			}

//			private void _Configure()
//			{
//				VertexCount = 0;
//				VertexData = new float[LineCapacity * FloatsPerLine];
//				IndexData = new uint[LineCapacity * IndicesPerLine];

//				// Set all the Indices upfront.
//				uint baseIndexOffset = 0;
//				int i = 0;

//				for (int lc = 0; lc < LineCapacity; lc++)
//				{
//					foreach (uint indexOffset in s_IndicesSegment)
//					{
//						uint index = baseIndexOffset + indexOffset;
//						IndexData[i++] = index;
//					}
//					baseIndexOffset += VerticesPerLine;
//				}
//			}
//			private void _IncreaseCapacity()
//			{
//				float[] vtxData = VertexData;
//				LineCapacity *= 2;
//				_Configure();
//				Array.Copy(vtxData, VertexData, vtxData.Length);
//			}
//			static private uint[] s_IndicesSegment = new uint[] { 0, 2, 3, 3, 1, 0 };

//			public void Clear()
//			{
//				// TODO -- !!!! RRRRRRRRRRRRRRRRRRE
//			}



//			public Line AddLine(Vector3 startPoint, Vector3 endPoint)
//			{
//				Line line = new Line() { LineIndex = LineCount };
//				LineCount++;
//				VertexCount += VerticesPerLine;
//				IndexDataSize += IndicesPerLine;

//				if (LineCount > LineCapacity)
//				{  // Oops, not enough capacity -- double our size and continue
//					LineCapacity *= 2;
//					_IncreaseCapacity();
//				}

//				SetLinePoints(line, startPoint, endPoint);
//				return line;
//			}

//			public void SetLinePoints(Line line, Vector3 startPos, Vector3 endPos)
//			{
//				int index = (int)(line.LineIndex * FloatsPerLine);

//				Vector3 pos = startPos;
//				Vector3 pos2 = endPos;

//				for (int i = 0; i < 2; i++)
//				{
//					float uVal = (i * 2) - 1; // values will be -1 and +1

//					VertexData[index++] = pos.X;
//					VertexData[index++] = uVal * pos.Y;
//					VertexData[index++] = pos.Z;

//					VertexData[index++] = pos2.X;
//					VertexData[index++] = pos2.Y;
//					VertexData[index++] = pos2.Z;
//				}

//				pos = endPos;
//				pos2 = startPos; // nextPos + offset;

//				for (int i = 0; i < 2; i++)
//				{
//					float uVal = (i * 2) - 1; // values will be -1 and +1

//					VertexData[index++] = pos.X;
//					VertexData[index++] = uVal * pos.Y;
//					VertexData[index++] = pos.Z;

//					VertexData[index++] = pos2.X;
//					VertexData[index++] = -pos2.Y;
//					VertexData[index++] = pos2.Z;
//				}
//				_isDirty = true;
//			}

//			private bool _isDirty = true;

//			public void OnUpdate(float timeDelta)
//			{
//				if (_isDirty)
//					_OnDataDirty();
//			}

//			private void _OnDataDirty()
//			{
//				_isDirty = false;

//				if (LineCount > 0)
//				{
//					Vertices.SetSize(VertexCount, ElementMask.Position | ElementMask.Normal, false); // | ElementMask.TexCoord2, false);
//					Vertices.SetData(VertexData);

//					Indices.SetSize(IndexDataSize, true, false);
//					Indices.SetData(IndexData);
//				}
//				Geometry.SetDrawRange(PrimitiveType.TriangleList, 0, IndexDataSize, false);

//				var model = Model.Model; // new Model();
//				if (model == null)
//				{
//					model = new Model();
//					model.NumGeometries = 1;
//				}
//				model.SetGeometry(0, 0, Geometry);
//				model.BoundingBox = Bounds;
//				Model.Model = model;

//				Model.SetMaterial(Material);
//			}
//		}
//	}
//}
