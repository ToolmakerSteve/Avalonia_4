//using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using Urho;
using Urho.Gui;
using PointF = System.Drawing.PointF;

namespace AvaloniaSample.LineLayer3D
{
	public partial class LineLayer3D
	{
		public class Manager
		{
			//public Dictionary<MapLayerType, LineLayer3D> _lineLayers = new Dictionary<MapLayerType, LineLayer3D>();

			public readonly View3D View;

            private Camera Camera => View.CameraNode.GetComponent<Camera>();
            public Vector3 CameraPosition { get { return View.CameraNode.Position; } }
			//public Node BaseNode { get { return View.BaseNode; } }
			//public Node TransparentNode { get { return View.TransparentNode; } }
			//private Text _textElement;


			public Manager(View3D view)
			{
				View = view;
				_lineWidth = 0.08f;
			}

			public float _colorFalloff = 1.15f; //0.4f; //1.2f;
			public float _alphaFalloff = 3.5f; // 1.2f; //12.0f;
			public int _lineColorIndex = 0;

			private float _lastCamY;
			public int TotalLineSegments;
			public float _lastValueChanged;
			public string _lastValNameChanged;

			public string LastLayerLoaded = "";

			//public GeoRectangle GeoArea;
			//public Projection3D Projection;

			private float _lastLineWidth;
			private float _lineWidth;

			//public void ToggleLineLayer(MapLayerType layerType, bool? enableIt)
			//{
			//	LineLayer3D layer;
			//	if (_lineLayers.TryGetValue(layerType, out layer))
			//	{

			//		if (enableIt == null)
			//		{
			//			enableIt = !layer.IsVisible;
			//			if (enableIt.Value && layer.Filled != null && layer.Filled.IsVisible && layer._config.IsOutlined)
			//			{	// Last time disabled the main node, so this time disable the filled shape
			//				enableIt = false;
			//				layer.Filled.IsVisible = false;
			//			}
			//		}
			//		layer.IsVisible = enableIt.Value;

			//		if (layer.Filled != null && enableIt.Value)
			//			layer.Filled.IsVisible = true; // turn them both back on
			//	}
			//}

			public void OnUpdate(float timeStep)
			{
				float widthFactor = (CameraPosition.Y + 1);
				float minWidth = _GetLineWidthFromActual(0.0007f * widthFactor);
				float maxWidth = _GetLineWidthFromActual(0.012f * widthFactor);

				//bool swap = _AdjustValue("Width", ref _lineWidth, timeStep, 1.2f * _lineWidth, minWidth, maxWidth, iKey.C, iKey.Z);
				//bool changeParams = _AdjustValue("Alpha", ref _alphaFalloff, timeStep, 1 * _alphaFalloff, 0.1f, 100f, iKey.E, iKey.Q);
				//changeParams |= _AdjustValue("Border", ref _colorFalloff, timeStep, _colorFalloff, 0.1f, 40f, iKey.R, iKey.F);

				if (_lineWidth != _lastLineWidth)
				{
					AdjustLineWidth();
				}
				//else if (changeParams)
				//{
				//	SetShaderParams();
				//}
				else if (CameraPosition.Y != _lastCamY)
				{
					AdjustLineWidth();
				}
				else
				{
					_SetCamPos();
				}

			}

			//public LineLayer3D AddLinesLayer(ref int lineIndex, ref RenderOrder fillOrder, Color? lineColor, MapLayerType dataType, int lodLevel, int maxVertexData, int maxIndices,
			//	Func<MapLayerType, int, Data.ISpatialDatabase<GeoShape>> dbGetter = null,
			//	float[] zoomThresholds = null, float widthFactor = 1f, Color? fillColor = null, float alphaFactor = 1f, float borderFactor = 1f, Color? fillColorNear = null)
			//{
			//	RenderMode rm = RenderMode._Invisible;
			//	if (lineColor != null)
			//		rm |= RenderMode.Outlines;
			//	if (fillColor != null)
			//		rm |= RenderMode.Filled;

			//	LineLayerConfig config = new LineLayerConfig(null, dataType)
			//	{
			//		RenderMode = rm,
			//		CrispFactor = alphaFactor,
			//		BorderFactor = borderFactor,
			//		LineWidth = widthFactor,
			//		ZoomLodThresholds = zoomThresholds ?? new float[4] { 2f, 5f, 15f, float.MaxValue }
			//	};

			//	if (lineColor != null)
			//	{
			//		config.LineColor = lineColor;

			//		if (fillColorNear != null)
			//			config.FillColorNear = fillColorNear;
			//	}

			//	if (fillColor != null)
			//	{  // Deal with FilledShape
			//		config.FillColor = fillColor.Value;
			//		config.FillColorNear = fillColorNear;
			//		config.FillOrder = fillOrder;
			//		fillOrder.Adjust(1);
			//	}

			//	return AddLinesLayer(ref lineIndex, ref fillOrder, config, maxVertexData, maxIndices, dbGetter);
			//}

			//public LineLayer3D AddLinesLayer(ref int lineIndex, ref RenderOrder fillOrder, LineLayerConfig config, int maxVertexData, int maxIndices,
			//	Func<MapLayerType, int, Data.ISpatialDatabase<GeoShape>> dbGetter)
			//{
			//	LineLayer3D layer = new LineLayer3D(this, config, ref lineIndex, maxVertexData, maxIndices, dbGetter);
			//	_lineLayers[config.LayerType] = layer;

			//	//Map.Layers.AddLayer(layer);

			//	return layer;
			//}

			//public void UpdateAllLayers(GeoRectangle geoArea, Projection3D proj, float camY)
			//{
			//	//TotalLineSegments = 0;

			//	//float areaWidth = (float)geoArea.GetWidth().Degrees;
			//	//float areaHeight = (float)geoArea.GetHeight().Degrees;
			//	//float areaMaxDim = Math.Max(areaWidth, areaHeight);

			//	//foreach (var layer in _lineLayers.Values)
			//	//{
			//	//	layer.SelectLOD(camY, areaMaxDim);
			//	//	//layer.UpdateContentIfNeeded(geoArea, camY, Map.ScreenPixelCoverage);
			//	//	//TotalLineSegments += layer.LineCount;
			//	//}

			//	SetShaderParams();
			//}

			//public void UpdateLinesLayer(MapLayerType dataType, GeoRectangle geoArea, Coordinate1 areaMaxDim, Projection3D proj, uint lodLevel, float widthFactor = 1f, float alphaFactor = 1f, float borderFactor = 1f)
			//{
			//	LineLayer3D layer;
			//	if (!_lineLayers.TryGetValue(dataType, out layer))
			//	{
			//		return; // layer not found!
			//	}

			//	layer.UpdateContent(geoArea, areaMaxDim, lodLevel);
			//	TotalLineSegments += layer.LineCount;
			//}

			public void SetShaderParams()
			{
				AdjustLineWidth();
			}

			public float PerspectiveRatio = 0.85f; // RRRRRRRRRRRRRRE: this should be set to the proper value!

			public void AdjustLineWidth()
			{
				//_lastCamY = CameraPosition.Y;
				//_lastLineWidth = _lineWidth;

				////double yFactor = (float)iFly.MathUtil.Clamp(_lastCamY, 1f, 9999f);
				////yFactor = Math.Pow(yFactor, 0.7);

				//float actualWidth = _GetActualLineWidth();

				//float factor = 1f / ((float)Math.Pow(_lastCamY * Projection3D.InvUnitsPerDegree, 0.25));
				//float colorFalloff = _colorFalloff; // * factor;
				//float alphaFalloff = _alphaFalloff * factor;

				//Vector3 camPos = this.View.Camera.CurrentPosition;

				//actualWidth = (float)(0.000007 * Math.Pow(camPos.Y, 0.6)); //   0.007f; // RRRRRRRRRRRRRRRRE- Kludge
				//float widthBoost = 1f;

				//Vector3 camForward = this.View.Camera.Node.Direction; // new Vector3(0, 0, 1);
				//Vector2 fadeDist = 0.7f * ((0.07f * Projection3D.UnitsPerDegree) + camPos.Y) * new Vector2(8, 10);

				//alphaFalloff = 2;
				//colorFalloff = 3;

				//float nearClip = Projection.NearDistance;

				//float clipRatio = 0.9f - (0.05f * (camPos.Y / 8000f));
				//if (camPos.Y > 70000)
				//	clipRatio = 0;
				//else { }

				//if (Terrain3D.Instance == null)
				//	return; // too soon

				//Vector2 waterFadeDist = new Vector2(Terrain3D.Instance.FadeDist.X, Terrain3D.Instance.FadeDist.Y);
				//float nearWaterCull = 0.25f * Terrain3D.Instance.NearTerrainSpan;
				//float minRatio = 0.4f / (float)Math.Pow(View.ScreenMetrics.AspectRatio, 0.7);
				//minRatio /= (float)Math.Pow(camPos.Y * 0.0000077, 0.12);
				//float minCullDist = minRatio * waterFadeDist.X;
				//if (nearWaterCull < minCullDist)
				//	nearWaterCull = minCullDist;
				//float maxCullDist = 0.65f * Terrain3D.Instance.NearTerrainSpan;
				//if (nearWaterCull > maxCullDist)
				//	nearWaterCull = maxCullDist;

				//float waterRatio = nearWaterCull / waterFadeDist.X; //  0.55f * Terrain3D.Instance.FadeDist.X;

				//float c = (float)(30000.0 / Math.Pow(120.0 * nearClip, PerspectiveRatio));

				//foreach (var layer in _lineLayers.Values)
				//{
				//	var m = layer.Material;
				//	m.SetShaderParameter("Falloff", new Vector3(colorFalloff * layer.BorderFactor, alphaFalloff * layer.AlphaFactor, 1));
				//	m.SetShaderParameter("LineWidth", actualWidth);
				//	//m.SetShaderParameter("WidthBoost", widthBoost);
				//	//m.SetShaderParameter("CamPos", camPos);
				//	//m.SetShaderParameter("CamForward", camForward);
				//	m.SetShaderParameter("Fadeout", fadeDist);
				//	m.SetShaderParameter("NearDist", nearClip);
				//	m.SetShaderParameter("ClipRatio", clipRatio);
				//	m.SetShaderParameter("C", c);
				//	//m.SetShaderParameter("PerspectiveRatio", PerspectiveRatio);

				//	if (layer.WrappedLayer != null)
				//	{
				//		var mw = layer.WrappedLayer.Material;
				//		mw.SetShaderParameter("Falloff", new Vector3(colorFalloff * layer.BorderFactor, alphaFalloff * layer.AlphaFactor, 1));
				//		mw.SetShaderParameter("LineWidth", actualWidth);
				//		//mw.SetShaderParameter("WidthBoost", widthBoost);
				//		//mw.SetShaderParameter("CamPos", camPos);
				//		//mw.SetShaderParameter("CamForward", camForward);
				//		mw.SetShaderParameter("Fadeout", fadeDist);
				//		mw.SetShaderParameter("NearDist", nearClip);
				//		mw.SetShaderParameter("ClipRatio", clipRatio);
				//		m.SetShaderParameter("C", c);
				//		//mw.SetShaderParameter("PerspectiveRatio", PerspectiveRatio);
				//	}

				//	if (layer.Filled != null)
				//	{
				//		layer.Filled.Material.SetShaderParameter("Fadeout", waterFadeDist);
				//		if (layer.Name == "FarHydro") 
				//			layer.Filled.Material.SetShaderParameter("NearCull", nearWaterCull);
				//	}
				//}

				//_lastLineWidth = _lineWidth;
			}

			private void _SetCamPos()
			{
				//Vector3 camPos = this.View.Camera.CurrentPosition;

				//foreach (var layer in _lineLayers.Values)
				//{
				//	layer.Material.SetShaderParameter("CamPos", camPos);

				//	if (layer.WrappedLayer != null)
				//	{
				//		layer.WrappedLayer.Material.SetShaderParameter("CamPos", camPos);
				//	}
				//}
			}


			private float _GetActualLineWidth()
			{
				double yFactor = _GetLineWidhtYFactor();
				float actualWidth = _lineWidth * (float)(0.1f * yFactor);
				return actualWidth;
			}

			private float _GetLineWidthFromActual(float actualWidth)
			{
				double yFactor = _GetLineWidhtYFactor();
				float lineWidth = actualWidth / (float)(0.1f * yFactor);
				return lineWidth;
			}

			private double _GetLineWidhtYFactor()
			{
				double yFactor = (float)Math.Clamp(_lastCamY, 0.02f, 9999f);
				if (yFactor > 1f)
					yFactor = Math.Pow(yFactor, 0.7);
				else
					yFactor = Math.Pow(yFactor, 0.8);
				return yFactor;
			}

			//private bool _AdjustValue(string valName, ref float val, float timeStep, float posIncr, float min, float max, iKey posKey, iKey negKey)
			//{
			//	float incr = 0;
			//	if (View.Screen.Input.GetKeyDown(posKey))
			//	{
			//		incr = posIncr;
			//	}
			//	else if (View.Screen.Input.GetKeyDown(negKey))
			//	{
			//		incr = -posIncr;
			//	}
			//	else
			//	{
			//		return false;
			//	}

			//	val += incr * timeStep;
			//	val = (float)MathUtil.Clamp(val, min, max);

			//	_lastValNameChanged = valName;
			//	_lastValueChanged = val;
			//	return true;
			//}

		}
	}
}
