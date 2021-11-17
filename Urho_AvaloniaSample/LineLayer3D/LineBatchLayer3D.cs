//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Urho;
//using Urho.Gui;
//using Urho.Resources;
//using PointF = System.Drawing.PointF;

//namespace AvaloniaSample.LineLayer3D
//{
//	public class LineBatchLayerConfig : LayerConfig
//	{
//		//public int MaxVertexCount;
//		//public int MaxIndexCount;

//		//public Color LineColor;
//		//public Color OutlineColor;
//		//public float CrispFactor; // Lower Value == less Crisp, wider band for fadeout.   High Value == less fadeout, more crisp edges (and can have more jaggedness on screen)
//		//public float BorderFactor; // Lower value == thicker Border

//		//public float LineWidth; // TODO: this needs to be transformed into 3 values

//		public LineBatchLayerConfig(string name, MapLayerType layerType) : base(name, layerType) { }
//	}

//	public partial class LineBatchLayer3D : MapLayer3D
//	{
//		static public LineBatchLayer3D Instance { get; private set; }

//		public View3D View { get; private set; }
//		private LineBatchLayerConfig _config;

//		//public Color LineColor;
//		public List<Batch> Batches = new List<Batch>();

//		public ResourceCache Resources { get { return View.Resources; } }

//		public float WidthFactor;
//		public float AlphaFactor;
//		public float BorderFactor;

//		public LineBatchLayer3D(View3D view, LineBatchLayerConfig config) :
//			base(config, view, null) // context, rc, baseNode, camNode, projection)
//		{
//			View = view;
//			_config = config;

//			Node = _baseNode.CreateChild();
//			Node.Name = config.Name;
//			Instance = this;
//		}

//		public Batch CreateBatch(string name, int capacity, RenderOrder ro, Color lineColor, float lineWidth)
//		{
//			Batch batch = new Batch(this, capacity, ro, lineColor, lineWidth);
//			Batches.Add(batch);
//			return batch;
//		}

//		public void OnUpdate(float timeStep)
//		{ 
//			foreach(var batch in Batches)
//			{
//				batch.OnUpdate(timeStep);
//			}
//		}

//		protected override void _OnMapLongitudeShifted(float lgShift)
//		{
//			LgShift += lgShift;
//		}

//		public float LgShift
//		{
//			get { return _lgShift; }
//			set
//			{
//				_lgShift = value;

//				//Material.SetShaderParameter("LgShift", _lgShift);

//				//if (Filled != null)
//				//	Filled.Material.SetShaderParameter("LgShift", _lgShift);

//				//if (WrappedLayer != null)
//				//{
//				//	float wrappedShift = _lgShift + ((float)LgWrapResult * Projection3D.UnitsPerDegree_360); // ((LgWrapResult == GeoShape.LongitudeWrapResult.East) ? 360 : -360);
//				//	WrappedLayer.LgShift = wrappedShift;
//				//}

//			}
//		}
//		private float _lgShift = 0;


//		public override void UpdateContent(GeoRectangle area, Coordinate1 areaMaxDim, uint lod)
//		{

//		}
//	}
//}
