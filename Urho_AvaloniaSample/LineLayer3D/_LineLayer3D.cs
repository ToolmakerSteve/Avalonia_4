using System;
using System.Collections.Generic;
using System.Linq;
using Urho;
using Urho.Gui;
using Urho.Resources;
using PointF = System.Drawing.PointF;

namespace AvaloniaSample.LineLayer3D
{
	public class LineLayerConfig //: LayerConfig
	{
		public LineLayer3D.RenderMode RenderMode;
		public bool IsFilled { get { return (RenderMode & LineLayer3D.RenderMode.Filled) == LineLayer3D.RenderMode.Filled; } }
		public bool IsOutlined { get { return (RenderMode & LineLayer3D.RenderMode.Outlines) == LineLayer3D.RenderMode.Outlines; } }

		public bool IsOpaqueFill = false;
		public Color FillColor;
		public Color? FillColorNear;
		//public RenderOrder FillOrder;
		public int MaxVertexCount;
		public int MaxIndexCount;

		public Color? LineColor;
		public Color OutlineColor;
		public float CrispFactor; // Lower Value == less Crisp, wider band for fadeout.   High Value == less fadeout, more crisp edges (and can have more jaggedness on screen)
		public float BorderFactor; // Lower value == thicker Border

		public float LineWidth; // TODO: this needs to be transformed into 3 values
										//public float TargetThickness;
										//public float MinThickness;
										//public float MaxThickness;

		public LineLayerConfig(string name)//, MapLayerType layerType) : base(name, layerType)
        { }
	}

	public partial class LineLayer3D //: MapLayer3D
	{
		public readonly LineLayer3D.Manager Mgr;
        //public ISceneView View { get { return Mgr.View; } }
        public View3D View { get { return Mgr.View; } }
        private LineLayerConfig _config;

		public View3D SceneView { get { return Mgr.View; } }


		public Color? LineColor;
		public Color? NearLineColor;
		public float YPos;
        //public iFly.Data.ISpatialDatabase<GeoShape> Database;

        public ResourceCache Resources => AvaloniaSample.It.ResourceCache; //TMS_TODO //View.Resources;

        public float WidthFactor;
		public float AlphaFactor;
		public float BorderFactor;

		public StaticModel Model;
		public Geometry Geometry;
		public VertexBuffer Vertices;
		public IndexBuffer Indices;


		public uint VertexDataSize;
		static public float[] VertexData = new float[2000000];
		public uint VertexCount;
		public uint IndexDataSize;
		static public uint[] IndexData = new uint[400000];

		public Material Material;
		//public List<GeoShape> Shapes;
		public int LineCount;
		public BoundingBox Bounds;

		//public FilledShape Filled;

		public LineLayer3D WrappedLayer;


		//public LineLayer3D(LineLayer3D.Manager mgr, LineLayerConfig config, ref int lineIndex, int maxVertexData, int maxIndices,
		//	Func<MapLayerType, int, Data.ISpatialDatabase<GeoShape>> dbGetter = null, LineLayer3D parentLayer = null) :
		//	base(config, mgr.View, parentLayer) // context, rc, baseNode, camNode, projection)
		//{
		//	Mgr = mgr;
		//	_config = config;
		//	DatabaseGetter = dbGetter ?? DatabaseGetter;
		//	CurrentLOD = SelectLOD(99999999f, Coordinate1.FromDegrees(999));

		//	UsesNearArea = false;

		//	if (!IsWrapped)
		//		LayerIndex = lineIndex++;

		//	LineColor = config.LineColor;
		//	if (LineColor.HasValue)
		//		NearLineColor = config.FillColorNear;

		//	YPos = 0; // 0.001f * (1 + LayerIndex);
		//	WidthFactor = (config.LineWidth > 0) ? config.LineWidth : 0.001f; // widthFactor;
		//	AlphaFactor = config.CrispFactor; // alphaFactor;
		//	BorderFactor = config.BorderFactor; // borderFactor;

		//	Node = _baseNode.CreateChild();
		//	Node.Name = config.Name + "-Lines";

		//	YPos = 0;
		//	Node.Position = new Vector3(0, YPos, 0); // layer.YPos, 0);
		//	Shapes = new List<GeoShape>();
		//	//VertexData = new float[maxVertexData];
		//	//IndexData = new uint[maxIndices];

		//	if (config.IsFilled)
		//	{
		//		Filled = new FilledShape(this, config);
		//	}

		//	bool hasGradient = (_config.FillColorNear != null);

		//	MaterialID matID = (hasGradient) ? MaterialID.SmoothLine3D_Gradient : MaterialID.SmoothLine3D;
		//	Material = View.AppContext.Materials.Get3D(matID, config.RenderOrder);

		//	if (_config.LineColor.HasValue)
		//	{
		//		Material.SetShaderParameter("Color", _config.LineColor.Value);

		//		if (hasGradient)
		//		{
		//			Material.SetShaderParameter("NearColor", _config.FillColorNear.Value);
		//			Material.SetShaderParameter("GradientThresholds", new Vector2(0.012f, 0.77f));
		//		}
		//	}

		//	Material.SetShaderParameter("PerspectiveRatio", 0.85f);// .9985f);
		//	Material.SetShaderParameter("LineWidth", 0.001f);
			
		//	Material.DepthBias = s_BiasParameters;

		//	Model = Node.CreateComponent<StaticModel>();
		//	Vertices = new VertexBuffer(View.AppContext.UrhoContext, false);
		//	Indices = new IndexBuffer(View.AppContext.UrhoContext, false);

		//	Geometry = new Geometry();
		//	Geometry.SetVertexBuffer(0, Vertices);
		//	Geometry.IndexBuffer = Indices;

		//	if (!IsWrapped)
		//		WrappedLayer = new LineLayer3D(mgr, config, ref lineIndex, maxVertexData, maxIndices, dbGetter, this);
		//}

		//public Func<MapLayerType, int, Data.ISpatialDatabase<GeoShape>> DatabaseGetter = iFly.MapLayers.LineShapeSectorLayer.GetVectorDataTree;

		//protected override int SelectLOD(float camY, Coordinate1 areaMaxDim)
		//{
		//	camY *= Projection3D.InvUnitsPerDegree;
		//	if (_config.UseAreaDimForLOD)
		//		camY = (float)areaMaxDim.Degrees;

		//	int lod = 0;
		//	while (camY > _config.ZoomLodThresholds[lod])
		//	{
		//		lod++;
		//	}
		//	Database = DatabaseGetter(LayerType, lod); // iFly.MapLayers.LineShapeSectorLayer.GetVectorDataTree(LayerType, lod);
		//	return lod;
		//}

		//protected override void _OnMapLongitudeShifted(float lgShift)
		//{
		//	LgShift += lgShift;
		//}

		public float LgShift
		{
			get { return _lgShift; }
			set
			{
				_lgShift = value;

				Material.SetShaderParameter("LgShift", _lgShift);

				//if (Filled != null)
				//	Filled.Material.SetShaderParameter("LgShift", _lgShift);

				if (WrappedLayer != null)
				{
					//float wrappedShift = _lgShift + ((float)LgWrapResult * Projection3D.UnitsPerDegree_360); // ((LgWrapResult == GeoShape.LongitudeWrapResult.East) ? 360 : -360);
					//WrappedLayer.LgShift = wrappedShift;
				}

			}
		}
		private float _lgShift = 0;


		//public override void UpdateContent(GeoRectangle area, Coordinate1 areaMaxDim, uint lod)
		//{
		//	_CreateShapePoints(area, View.Projection as Projection3D, ref Shapes);
		//	Model.SetMaterial(Material);

		//	if (WrappedLayer != null)
		//		WrappedLayer.Model.SetMaterial(WrappedLayer.Material);


		//	if (Filled != null)
		//	{
		//		Filled.UpdateContent();
		//	}

		//	if (WrappedLayer != null) // && WrappedLayer.Shapes.Count > 0)
		//	{
		//		if (WrappedLayer.Filled != null)
		//			WrappedLayer.Filled.UpdateContent();
		//	}
		//	LgShift = 0; // reset to zero


		//	if (Constants.IsWPFTest && this.LayerType == iFly.MapLayerType.Hydro)
		//	{	// Hydro Metrics for diagnostics
		//		int shapeCount = Shapes.Count + WrappedLayer.Shapes.Count;
		//		int pointCount = 0;
		//		foreach (var s in Shapes)
		//		{
		//			pointCount += s.Part1.Points.Length;
		//		}
		//		foreach (var s in WrappedLayer.Shapes)
		//		{
		//			pointCount += s.Part1.Points.Length;
		//		}
		//		iFly.Data.GlobalMapData.Instance.HydroData3D = shapeCount + ":" + (pointCount >> 10) + "K";
		//	}
		//}

		private void _ProcessLine(Vector2 curEP, Vector2 nextEP, float w2, out Vector2 nextDelta, out Vector2 nextDir, out Vector2 nextPerp)
		{
			nextDelta = nextEP - curEP;
			nextDir = nextDelta;
			nextDir.NormalizeFast(); // Normalize();

			Vector2 nextPerpDir = new Vector2(nextDir.Y, -nextDir.X);
			nextPerp = w2 * nextPerpDir;
		}

		private void _OnCreatePolyLine()
		{
			if (IndexDataSize > 0)
			{
				Vertices.SetSize(VertexCount, ElementMask.Position | ElementMask.Normal, false); // | ElementMask.TexCoord2, false);
				Vertices.SetData(VertexData);

				Indices.SetSize(IndexDataSize, true, false);
				Indices.SetData(IndexData);
			}
			Geometry.SetDrawRange(PrimitiveType.TriangleList, 0, IndexDataSize, false);

			var model = Model.Model; // new Model();
			if (model == null)
			{
				model = new Model();
				model.NumGeometries = 1;
			}
			model.SetGeometry(0, 0, Geometry);
			model.BoundingBox = Bounds;
			Model.Model = model;
		}

		private int[] _indicesSegment = new int[] { 0, 2, 3, 3, 1, 0 };

		private void _AddVertices(Vector3 prevPos, Vector3 nextPos, ref uint vertexCount, List<float> vtxData, List<int> idxData)
		{
			int baseIndexOffset = (int)vertexCount;
			vertexCount += 4;

			Vector3 pos = prevPos;
			Vector3 pos2 = nextPos;

			for (int i = 0; i < 2; i++)
			{
				float uVal = (i * 2) - 1; // values will be -1 and +1

				vtxData.Add(pos.X);
				vtxData.Add(uVal * pos.Y);
				vtxData.Add(pos.Z);

				vtxData.Add(pos2.X);
				vtxData.Add(pos2.Y);
				vtxData.Add(pos2.Z);
			}

			Vector3 offset = nextPos - prevPos;
			pos = nextPos;
			pos2 = prevPos; // nextPos + offset;

			for (int i = 0; i < 2; i++)
			{
				float uVal = (i * 2) - 1; // values will be -1 and +1

				vtxData.Add(pos.X);
				vtxData.Add(uVal * pos.Y);
				vtxData.Add(pos.Z);

				vtxData.Add(pos2.X);
				vtxData.Add(-pos2.Y);
				vtxData.Add(pos2.Z);
			}


			// Next Add the Indices
			foreach (int indexOffset in _indicesSegment)
			{
				int index = baseIndexOffset + indexOffset;
				idxData.Add(index);
			}
		}

		private void _AppendPolyLine(Line3D line) //  ref uint nextLowIndex, out uint vertexCount, float[] vertexData, uint[] indexData)
		{
			if (line.VertexData == null)
				return; // nothing to add

			uint idxOffset = (uint)VertexCount;

			uint vertexDataCount = 6 * line.VertexCount;
			//EnsureVertexCapacity(this, VertexDataSize, vertexDataCount);
			Array.Copy(line.VertexData, 0, VertexData, VertexDataSize, vertexDataCount);
			VertexDataSize += vertexDataCount;
			//VertexData.AddRange(line.VertexData);

			uint indexCount = (uint)line.IndexData.Length;
			//EnsureIndexCapacity(this, IndexDataSize, indexCount);

			foreach (int idx in line.IndexData)
			{
				uint adjIdx = (uint)(idx + idxOffset);
				IndexData[IndexDataSize++] = adjIdx; // .Add(adjIdx);
			}
		}

		static public uint MaxVertexSize;
		static public uint MaxIndexSize;

		//static public void EnsureVertexCapacity(MapLayer layer, uint existingDataSize, uint addedDataSize)
		//{
		//	uint capacity = existingDataSize + addedDataSize;

		//	if (capacity > MaxVertexSize)
		//	{
		//		MaxVertexSize = capacity;

		//		if (VertexData.Length < capacity)
		//		{
		//			float[] newData = new float[(int)(1.2 * capacity)];
		//			Array.Copy(VertexData, newData, existingDataSize);
		//			VertexData = newData; // no need to copy, since we're doing this up front

		//			RRRRRRRRRRRoverflow[layer.Name] = newData.Length;

		//		}
		//	}
		//}

		//static public void EnsureIndexCapacity(MapLayer layer, uint existingIndexCount, uint addedIndexCount)
		//{
		//	uint newIndexCount = existingIndexCount + addedIndexCount;

		//	if (newIndexCount > MaxIndexSize)
		//	{
		//		MaxIndexSize = newIndexCount;

		//		if (IndexData.Length < newIndexCount)
		//		{
		//			uint[] newData = new uint[(int)(1.2 * newIndexCount)];
		//			Array.Copy(IndexData, newData, existingIndexCount);
		//			IndexData = newData; // no need to copy, since we're doing this up front

		//			RRRRRRRRRRRoverflow2[layer.Name] = newData.Length;

		//		}
		//	}
		//	//_DumpMaximums();
		//}

		//static public string ConfigFilepath = Constants.UserDir + "lines.config";

		// kludge - TO HELP US optimize the Vertex/Index Buffer array sizes.
		private void _DumpMaximums()
		{

			//System.IO.StreamWriter writer = new System.IO.StreamWriter(ConfigFilepath);

			//foreach(var name in RRRRRRRRRRRoverflow.Keys)
			//{
			//	int vertCount = RRRRRRRRRRRoverflow[name];
			//	int idxCount = RRRRRRRRRRRoverflow2[name];
			//	writer.WriteLine(name + "|" + vertCount + "|" + idxCount);
			//}
			//writer.Close();
			//writer.Dispose();
		}

		static public Dictionary<string, int> RRRRRRRRRRRoverflow = new Dictionary<string, int>();
		static public Dictionary<string, int> RRRRRRRRRRRoverflow2 = new Dictionary<string, int>();


		private Line3D _CreatePolyLine(float y, float width, List<Vector3> posList, ref int nextLowIndex, out uint vertexCount, List<float> vertexData, List<int> indexData)
		{
			LineLayer3D.Line3D line = new LineLayer3D.Line3D();
			vertexCount = 0;

			if (posList.Count < 2)
				return line; // empty line

			int maxIndex = posList.Count - 1;
			Vector3 prevPos = posList[0];

			for (int i = 1; i <= maxIndex; i++)
			{
				Vector3 nextPos = posList[i];
				_AddVertices(prevPos, nextPos, ref vertexCount, vertexData, indexData);
				prevPos = nextPos;
			}

			line.VertexData = vertexData.ToArray();
			line.IndexData = indexData.ToArray();

			return line;
		}

		static private List<Vector3> s_Pos3DList = new List<Vector3>();
		static private List<float> s_vertexData = new List<float>();
		static private List<int> s_indexData = new List<int>();

		public int LinesLoaded;
		public int CacheHits;

		//private void _ConvertToRectangle(GeoShape shape)
		//{
		//	var bb = shape.Box;
		//	Coordinate p0 = bb.MinCoordinate;
		//	Coordinate p1 = new Coordinate(bb.MaxCoordinate.Latitude, bb.MinCoordinate.Longitude);
		//	Coordinate p2 = bb.MaxCoordinate;
		//	Coordinate p3 = new Coordinate(bb.MinCoordinate.Latitude, bb.MaxCoordinate.Longitude);
		//	shape.Part1.Points = new Coordinate[5] { p0, p1, p2, p3, p0 };
		//}

		//public Func<LineLayer3D, GeoRectangle> QueryAreaGetter = null;

		//private void _CreateShapePoints(GeoRectangle geoArea, Projection3D proj, ref List<GeoShape> data)
		//{
		//	iFly.Data.ISpatialDatabase<GeoShape> db = Database;

		//	if (db == null)
		//		return; // no data to search

		//	if (QueryAreaGetter != null)
		//		geoArea = QueryAreaGetter(this);

		//	List<GeoShape> wrappedData = (WrappedLayer != null) ? WrappedLayer.Shapes : null;
		//	db.SearchWrapped(geoArea, ref data, ref wrappedData, out LgWrapResult);

		//	if (data.Count > 15000 && CurrentLOD < 5)
		//	{	// KLUDGE to prevent overflow!
		//		CurrentLOD++;
		//		Database = DatabaseGetter(LayerType, CurrentLOD);
		//		_CreateShapePoints(geoArea, proj, ref data);
		//		return;
		//	}

		//	if (!_config.IsOutlined)
		//		return;  // Don't add Outlines... they are turned off

		//	CreateShapePoints(data, proj);

		//	if (WrappedLayer != null)
		//	{
		//		WrappedLayer.CreateShapePoints(wrappedData, proj);
		//	}
		//}

		//private GeoShape.LongitudeWrapResult LgWrapResult;

		//static private Elevations.ElevationMap s_ElevMap = Elevations.ElevationService.Instance.GetOrCreateMap(0, false);
		//static private BiasParameters s_BiasParameters = UrhoUtil.CreateDepthBias(-0.00012f);

		//public void CreateShapePoints(List<GeoShape> data, Projection3D proj)
		//{
		//	VertexDataSize = 0;
		//	IndexDataSize = 0;
		//	VertexCount = 0;
		//	LineCount = 0;

		//	Vector3 min = Bounds.Min;
		//	Vector3 max = Bounds.Max;

		//	int listIndex = 0;
		//	s_Pos3DList.Clear();

		//	float AltitudeConversionFactor = proj.AltitudeUnitsPerFt; // 0.4f / 65535f;
		//	int zeroAltOffset = proj.ZeroAltitudeOffset_Feet;

		//	lock (s_ElevMap.DataLock)
		//	{
		//		foreach (var shape in data)
		//		{
		//			foreach (var part in shape.Parts)
		//			{
		//				Line3D line = part.Path3D2 as Line3D;

		//				if (line == null)
		//				{  // Doesn't exist - so need to create it
		//					s_Pos3DList.Clear();
		//					PointF prevP = new PointF(float.NaN, float.NaN);

		//					if (false)
		//					{
		//						Coordinate coord = part.Points[0];
		//						var p3d = proj.CoordinateToXZPos(coord);
		//						float y = 0.007f * Projection3D.UnitsPerDegree;
		//						s_Pos3DList.Add(new Vector3(p3d.X, y, p3d.Y));
		//						coord.Latitude.BaseUnits += Coordinate1.BaseUnitsPerDegree >> 0;
		//						p3d = proj.CoordinateToXZPos(coord);
		//						s_Pos3DList.Add(new Vector3(p3d.X, y, p3d.Y));
		//					}
		//					else
		//					{
		//						foreach (var p in part.Points)
		//						{
		//							var p3d = proj.CoordinateToXZPos(p);

		//							if (p3d != prevP)
		//							{
		//								if (p3d.X < min.X)
		//									min.X = p3d.X;
		//								if (p3d.Y < min.Z)
		//									min.Z = p3d.Y;

		//								if (p3d.X > max.X)
		//									max.X = p3d.X;
		//								if (p3d.Y > max.Z)
		//									max.Z = p3d.Y;

		//								Altitude alt = s_ElevMap.GetElevation_NoLock(p);
		//								float yPos = (alt.Feet + zeroAltOffset + 20) * AltitudeConversionFactor;

		//								Vector3 pos3D = new Vector3(p3d.X, yPos, p3d.Y);
		//								s_Pos3DList.Add(pos3D);
		//								prevP = p3d;
		//							}
		//							else { } // found a duplicate!  Skip it ... RRRRRRRRRRRRRRRRRRE: NOTE, this elimination of dupes should be done server-side, not here in client.
		//						}
		//					}

		//					//float[] xyPosAry = xyPosList.ToArray();
		//					//xyPosLists[listIndex] = xyPosAry;

		//					int nextLowIndex = 0; // (int)layer.VertexCount;
		//					uint vertexCount;
		//					s_vertexData.Clear();
		//					s_indexData.Clear();
		//					line = _CreatePolyLine(YPos, WidthFactor, s_Pos3DList, ref nextLowIndex, out vertexCount, s_vertexData, s_indexData);
		//					line.VertexCount = vertexCount;
		//					line.LineCount = (s_Pos3DList.Count - 1);
		//					part.Path3D2 = line;
		//					LinesLoaded++;
		//				}
		//				else
		//				{  // Line3D already exists... re-use it.
		//					CacheHits++;
		//				}

		//				LineCount += line.LineCount;

		//				if (line.VertexCount > 3)
		//				{
		//					_AppendPolyLine(line);
		//					VertexCount += line.VertexCount;
		//				}
		//				else
		//				{
		//					if (line.VertexCount > 0)
		//					{ } // RRRRRRRRRRRRRRRRRE - Error?
		//				}
		//				//uint nextLowIndex = layer.VertexCount;
		//				//line = _CreatePolyLine(layer, layer.YPos, layer.WidthFactor, _lineUVPattern, _xyPosList, ref nextLowIndex);

		//				listIndex++;
		//			}
		//		}
		//	}

		//	//layer.LinePoints = xyPosLists;
		//	Bounds = Projection3D.FullBounds;

		//	if (true) //LineCount > 0)
		//		_OnCreatePolyLine();
		//	else
		//	{
		//		//foreach(var c in Node.Components)
		//		//{
		//		//	c.Enabled = false;
		//		//}
		//	}

		//	//return xyPosLists;
		//}


		[Flags]
		public enum RenderMode
		{
			_Invisible = 0,
			Outlines = 1,
			Filled = 2,
			Both = Outlines | Filled
		}


	}
}
