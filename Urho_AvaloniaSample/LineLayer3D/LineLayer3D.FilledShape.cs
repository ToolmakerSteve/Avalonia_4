//using System;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;

//using iFly.Data;
//using V3D = iFly.Vision3D;
//using PointF = System.Drawing.PointF;
//using Urho;

//using Resources = Urho.Resources;
//using FilledShape3D = iFly.Vision3D.FilledShape3D;

//namespace LineLayer3D
//{
//	public partial class LineLayer3D
//	{
//		public class FilledShape
//		{
//			public bool IsVisible { get { return Node.Enabled; } set { Node.Enabled = value; } }

//			public LineLayerConfig Config;

//			public iFly.Data.ISpatialDatabase<GeoShape> Database;

//			public StaticModel Model;
//			public Geometry Geometry;
//			public VertexBuffer Vertices;
//			public IndexBuffer Indices;
//			public int VertexDataSize;
//			//static public float[] VertexData;
//			public uint VertexCount;
//			public uint IndexDataSize;
//			//static public uint[] IndexData;

//			public Material Material;
//			public float[][] LinePoints;
//			public BoundingBox Bounds;

//			private LineLayer3D _layer;
//			public Node Node;
//			//static private BiasParameters s_BiasParameters = new BiasParameters() { ConstantBias = -0.001f };

//			public FilledShape(LineLayer3D layer, LineLayerConfig config) //, Urho.Context context, Urho.Resources.ResourceCache rc, Node baseNode, Node camNode, Projection3D projection)
//				//: base(config, context, rc, baseNode, camNode, projection)
//			{
//				_layer = layer;
//				Config = config;

//				if (s_BatchRRRRRR == null)
//				{  // KLudge
//					V3D.Terrain3D.Initialize();
//					new V3D.Terrain3D(null);
//					s_BatchRRRRRR = new V3D.FilledShape3D.Batch("EFB-Hydro", 65000, true);
//				}

//				if (config.IsOpaqueFill)
//					Node = layer.Node;
//				else
//					Node = layer.Mgr.TransparentNode.CreateChild();

//				//Shapes = new List<GeoShape>();
//				//int vdSize = 250000 * 3; // Config.MaxVertexCount * 3;
//				//if (VertexData == null || VertexData.Length < vdSize)
//				//	VertexData = new float[vdSize]; // 200,000 * 3

//				//if (IndexData == null || IndexData.Length < Config.MaxIndexCount)
//				//	IndexData = new uint[Config.MaxIndexCount]; // 500,000

//				Bounds = Projection3D.FullBounds;// new BoundingBox(-1000, 1000); // min, max); // RRRRRRR- KLUDGE to ensure consistent Z-Order for rendering.


//				bool hasGradient = (Config.FillColorNear.HasValue);
//				bool isFar = layer.Name.Contains("Far");
//				MaterialID matID;
//				if (isFar)
//					matID = (hasGradient) ? MaterialID.FilledShape3D_Far_Gradient : MaterialID.FilledShape3D_Far;
//				else
//					matID = (hasGradient) ? MaterialID.FilledShape3D_Gradient : MaterialID.FilledShape3D;

//				Material = layer.AppContext.Materials.Get3D(matID, Config.FillOrder);

//				Material.DepthBias = s_BiasParameters;
//				Material.SetShaderParameter("Color", Config.FillColor);

//				if (hasGradient)
//				{
//					Material.SetShaderParameter("NearColor", Config.FillColorNear.Value);
//					Material.SetShaderParameter("GradientThresholds", new Vector2(0.012f, 0.77f));
//				}

//				Model = Node.CreateComponent<StaticModel>();
//				Geometry = new Geometry();
//				Vertices = new VertexBuffer(layer.AppContext.UrhoContext, false);
//				Indices = new IndexBuffer(layer.AppContext.UrhoContext, false);

//				Geometry.SetVertexBuffer(0, Vertices);
//				Geometry.IndexBuffer = Indices;

//				var model = new Model();
//				model.NumGeometries = 1;
//				model.SetGeometry(0, 0, Geometry);
//				model.BoundingBox = Bounds;
//				Model.Model = model;

//				Model.SetMaterial(Material);
//			}

//			static private FilledShape3D.Batch s_BatchRRRRRR; // placeholder - to make it work

//			public void ToggleEnable(bool? enableIt, GeoRectangle geoArea)
//			{
//				if (enableIt.HasValue)
//					_layer.IsVisible = enableIt.Value;
//				else
//					_layer.IsVisible = !_layer.IsVisible;
//			}

//			public void UpdateContent() //GeoRectangle area, uint lod, float pixelCoverage)
//			{
//				//int lod = 0;
//				//float camY = _layer._cameraNode.Position.Y * zoomFactor;

//				//if (Config.ZoomLodThresholds != null)
//				//{
//				//	while (camY > Config.ZoomLodThresholds[lod])
//				//	{
//				//		lod++;
//				//	}
//				//}
//				////lod += lodAdjust;
//				//lod = MathUtil.Clamp(lod, 0, 3);
//				//Database = iFly.MapLayers.LineShapeSectorLayer.GetVectorDatabase(Config.LayerType, lod).TreeData as SectorTree<GeoShape>;

//				VertexDataSize = 0;
//				IndexDataSize = 0;
//				VertexCount = 0;

//				uint startIndex = 0;

//				//Shapes.Clear();
//				////area = new GeoRectangle(Coordinate.FromDegrees(60, -80), Coordinate.FromDegrees(76, -70));
//				//Database.Search(area, ref Shapes);

//				int shapeIndex = -1;
//				Projection3D proj = _layer._projection as Projection3D;	

//				foreach (var shape in _layer.Shapes)
//				{
//					shapeIndex++;
//					foreach (var part in shape.Parts)
//					{
//						FilledShape3D shape3D = part.Path3D as FilledShape3D;
//						if (false) ///RRRRRRRRRRRRRRE-TODO, not efficient!:   shape3D != null && !shape3D.IsDirty)
//						{  // Already loaded
//							VertexCount += (uint)shape3D._vertexCount;
//						}
//						else
//						{
//							FilledShape3D.CompressedTriangulationData ctd;

//							if (shape3D != null && shape3D.CTD != null)
//							{  // Shape exists but is dirty
//								ctd = shape3D.CTD;
//							}
//							else
//							{
//								ctd = part.Path3D as FilledShape3D.CompressedTriangulationData;
//								const byte ID = (byte)0;
//								shape3D = new FilledShape3D(ID);
//								part.Path3D = shape3D;
//							}

//							var points = _GetVertices(proj, part, ctd);
//							shape3D.Configure(part, points.Count, ctd);

//							var fs = shape3D.GetRenderableShape(part);
//							_ConfigureAndCopy(part, points, ctd, false, ref startIndex); //
//							points.Clear();
//							//FilledShape3D.CompressedTriangulationData.CopyIndices(vCount, indexOffset, CTD, idxPtr);
//						}
//					}
//				}

//				if (VertexCount > 0)
//				{
//					Vertices.SetSize(VertexCount, ElementMask.Position, false);
//					Vertices.SetData(VertexData); // s_VerticesAry.ToArray());

//					uint numIndices = IndexDataSize;
//					Indices.SetSize(IndexDataSize, true, false);
//					if (IndexDataSize > 0)
//						Indices.SetData(IndexData);
//				}
//				Geometry.SetDrawRange(PrimitiveType.TriangleList, 0, IndexDataSize, false);
//			}

//			private List<Vector3> s_Vertices = new List<Vector3>();
//			public const int MaxPointsPerPath = 4096;

//			private unsafe void _ConfigureAndCopy(GeoShape.Part path, List<Vector3> points, FilledShape3D.CompressedTriangulationData ctd, bool usesOutlines, ref uint startIndex)
//			{
//				uint vertCount = (uint)(points.Count);

//				EnsureVertexCapacity(_layer, (uint)VertexDataSize, vertCount);

//				foreach (var pos in points)
//				{
//					AddVertex(pos);
//				}
//				VertexCount += (uint)points.Count;


//				// Now copy the Indices
//				int vCount = (int)vertCount;
//				int indexCount = FilledShape3D.CompressedTriangulationData.GetIndexCount(vCount, ctd);
//				ushort[] localIndices = FilledShape3D.CompressedTriangulationData.GetIndices(vCount, ctd);

//				EnsureIndexCapacity(_layer, IndexDataSize, (uint)indexCount);

//				for (int i = 0; i < indexCount; i++)
//				{
//					var li = localIndices[i];
//					uint index = li + startIndex;
//					IndexData[IndexDataSize++] = index;
//				}

//				startIndex += (uint)points.Count;

//				//if (usesOutlines)
//				//{  // Add in Outline Indices here
//				//	indexCount = vCount * 2;
//				//	idxPtr = Owner.OutlineBuffMgr.GetCopyToBufferPtr((uint)indexCount, ref OutlineBuffer);
//				//	ushort* op = (ushort*)idxPtr;
//				//	int indexVal = indexOffset;
//				//	int startIndexVal = indexOffset;

//				//	for (int i = 0; i < vCount; i++)
//				//	{  // Create patter 0,1,  1,2,  2,3,  3,4...   but with indexOffset as the starting point, instead of 0
//				//		*op = (ushort)indexVal;
//				//		indexVal++;
//				//		op++;
//				//		*op = (ushort)indexVal;
//				//		op++;
//				//	}
//				//	op--;
//				//	*op = (ushort)startIndexVal;
//				//}
//			}

//			[StructLayout(LayoutKind.Sequential, Pack = 4)]
//			public struct Vertex
//			{
//				public const int Stride = 3 * 4;
//				public Vector3 VertexPos;

//				public Vertex(Vector3 pos)
//				{
//					VertexPos = pos;
//				}

//				public override string ToString()
//				{
//					return "Vert[" + VertexPos + "]";
//				}
//			}

//			public int YOffsetFt = 0;

//			private List<Vector3> _GetVertices(Projection3D proj, GeoShape.Part path, FilledShape3D.CompressedTriangulationData ctd)
//			{
//				var terrain = V3D.Terrain3D.Instance;
//				int pointCount = path.Points.Length;
//				s_Vertices.Clear();

//				float AltitudeConversionFactor = proj.AltitudeUnitsPerFt; // 0.4f / 65535f;
//				int zeroAltOffset = proj.ZeroAltitudeOffset_Feet + YOffsetFt + 0; // 100;

//				//var RRR3D = new GeoRectangle(Coordinate.FromDegrees(33.475, -118.225), Distance.FromFeet(1800)); // RRR3D
//				lock (s_ElevMap.DataLock)
//				{
//					int i = 0;
//					foreach (var elev in FilledShape3D.CompressedTriangulationData.GetElevations(pointCount, ctd, path))
//					{
//						var coord = path.Points[i++];
//						PointF posXZ = _layer._projection.CoordinateToXZPos(coord);

//						Altitude alt = s_ElevMap.GetElevation_NoLock(coord);
//						float yPos = (alt.Feet + zeroAltOffset) * AltitudeConversionFactor;

//						Vector3 vert = new Vector3(posXZ.X, yPos, posXZ.Y); // GetPos3D_GroundPrecise(terrain, coord, elev);
//						s_Vertices.Add(vert);
//					}
//				}
//				return s_Vertices;
//			}

//			private Vector3 GetPos3D_GroundPrecise(V3D.Terrain3D terrain, Coordinate loc, short elev)
//			{
//				int ltOffset = loc.Latitude.BaseUnits - terrain.CenterLoc.Latitude.BaseUnits;
//				int lgOffset = loc.Longitude.BaseUnits - terrain.CenterLoc.Longitude.BaseUnits;

//				float ltZPos = -ltOffset * V3D.Terrain3D.InvLtBaseUnitsPerFoot;
//				float lgXPos = lgOffset * terrain.InvLgBaseUnitsPerFoot;
//				float yPos = elev * V3D.Terrain3D.YScale;

//				Vector3 pos3D = new Vector3(lgXPos, yPos, ltZPos);
//				return pos3D;
//			}

//			public void AddVertex(Vector3 pos)
//			{
//				VertexData[VertexDataSize++] = pos.X;
//				VertexData[VertexDataSize++] = pos.Y;
//				VertexData[VertexDataSize++] = pos.Z;
//			}

//		}
//	}
//}
