using System;
using System.Collections.Generic;
using Global;
using SceneSource;
using Urho;
using Urho.Avalonia;
using Urho.Gui;
using Urho.IO;
using Urho.Urho2D;
using OU;
using U = OU.Utils;
using U2 = Global.Utils;
using static OU.DistD;
using System.Diagnostics;

namespace AvaloniaSample
{
	public partial class AvaloniaSample : Sample
	{
        public static AvaloniaSample It;   // TMS

		const bool HardcodedWall = false;//false; tmstest
        const bool IncludeAvaloniaLayer = false;
		// False uses MushroomScene, which has a flat plane
		const bool UseTerrainScene = false;//true;   // TMS
		static float GroundSize = 500;//2000;//500;
		const bool IncludeWater = false;
		const bool IncludeFog = false;
		const bool ShadowCascade = true;//TBD

		const bool IncludeScatteredModels = true;//true;
		const int NScatteredModels = 100;//1000;
		const bool ScatteredModelsAreBoxes = true;
		const bool BoxesHaveShadows = true;//true;  // TMS
		const bool RandomColorBoxes = false;//true;
		const bool RandomBoxRotation = true;
		const float BoxScale = 5.0f;//5.0f;

		const float _ZoneAmbient = 0.35f;//1.0f;//0.35f   TMS ttttt

		public const bool StartCameraOnLand = IncludeWater;
        public const bool WallKeys = true;   // Keys to control Wall Drawing. (StartNewWall)
        public const bool DrawWallPressDrag = true;   // In Top View.
        public const bool DrawWallAsFly = false && !DrawWallPressDrag;   // In Perspective View. TMS
		const float InitialAltitude2 = 250;//tmstest 100;
		const bool ShowWireframe = false;//false;   // TMS
		const bool ShowTerrainWireframe = false && ShowWireframe;
		const bool ShowWallWireframe = true && ShowWireframe;
		const float MinWallSegmentLength = GroundLine.SingleGeometryTEST ? 5 : 0.5f;//1f;//0.5f;   // TBD: Good value.


		public Scene Scene;
		public Octree Octree;

		Viewport Viewport1, Viewport2;
		Viewport CurrentViewport => OverViewport2 ? Viewport2 : Viewport1;
        Node WaterNode;
        Node ReflectionCameraNode;
        // Used when showing wireframe.
        Material WireframeMaterial;
        public readonly List<GroundLine> Walls = new List<GroundLine>();

        // CurrentWall.
        public Node WallsNode, CurrentWallNode;

		// private SampleAvaloniaWindow _window;
        [Preserve]
        public AvaloniaSample() : base(
			new ApplicationOptions(assetsFolder: "Parallax;Data;CoreData")
            {
                ResizableWindow = true,
				AutoloadCoreData = false
            })
        {
            It = this;
        }

        public AvaloniaSample(ApplicationOptions options) : base(options)
        {
            It = this;
        }


        #region --- Setup, Start ----------------------------------------
        protected override void Setup()
        {
            base.Setup();
        }

		protected override void Start()
		{
			base.Start ();

			VGRendering.LoadResources();

            Log.LogLevel = LogLevel.Info;


            Scene = new Scene();
			Octree = Scene.CreateComponent<Octree>();

			// Create a scene node for the camera, which we will move around
			Node parentOfCameraNode = GetParentOfCamera1Final();
            // Can override camera's default settings later. (1000 far clip distance, 45 degrees FOV, set aspect ratio automatically)
            Camera1FinalNode = parentOfCameraNode.CreateChild("camera");
            // When Camera1 does NOT have a two-node setup, make these the same.
            if (Camera1MainNode == null)
                Camera1MainNode = Camera1FinalNode;

            Camera1 = Camera1FinalNode.CreateComponent<Camera>();
            //if (ShowWireframe)
            //    Camera.FillMode = FillMode.Wireframe;

            if (UseTerrainScene)
                CreateTerrainScene(Scene);
            else
                CreateGroundPlaneScene(Scene);

            if (ShowTwoViewports)
                SetupSecondCamera();

            SimpleCreateInstructionsWithWasd();
            if (ShowTwoViewports)
                SetupTwoViewports();
            else
                SetupOneViewport();

			MaybeHardcodedWall();

            UI.Root.SetDefaultStyle(ResourceCache.GetXmlFile("UI/DefaultStyle.xml"));

			Input.SetMouseVisible(true);
			Input.SetMouseMode(MouseMode.Free);

            if (IncludeAvaloniaLayer)
                _SetupAvaloniaUI();
        }

		private void MaybeHardcodedWall()
		{
			if (!HardcodedWall)
				return;

			Vector2[] points;
			if (GroundLine.SingleGeometryTEST) {
				// Hardcoded segment.
				float x0 = -30;
				float z0 = -40-20;
				float dz = 10;//-20;//-5;
				points = new[] {
					new Vector2(x0, z0),
					new Vector2(x0, z0 + dz),
				};
				//U.Swap(ref points[0], ref points[1]);   // tmstest: Reverse drawing direction; see what that changes.

			} else {
				points = new[] {
					new Vector2(-38.4f, -40.0f),
					new Vector2(-38.0f, -41.3f),
					new Vector2(-37.4f, -41.6f),
					new Vector2(-36.8f, -42.0f),

					//new Vector2(-36.3f, -42.4f),
					//new Vector2(-35.8f, -42.8f),
					//new Vector2(-35.4f, -43.1f),
				};
			}

			StartNewWall();
			foreach (var point in points) {
				CurrentWall.AddPoint((Dist2D)point);
			}
			FlushAndNullWall();
		}
		#endregion

		#region --- OnUpdate ----------------------------------------
		uint _startTime = 0;
        bool WallDrawStarted;   // TMS
        GroundLine CurrentWall;
		// LastPenPosition2D used to draw short segment when lift pen.
		// LastWallPosition2D used to measure how far mouse has moved (on the ground).
		Vector2 LastPenPosition2D, LastWallPosition2D;

        private IntVector2 _lastScreenSize;

        protected override void OnUpdate(float timeStep)
        {
            if (Graphics.Size != _lastScreenSize)
            {
                _lastScreenSize = Graphics.Size;
                _DockWindows();
            }

            base.OnUpdate(timeStep);
            if (Camera1 == null)
                return;

            OnUpdate_Wireframe();

            var reflectionCamera = ReflectionCameraNode?.GetComponent<Camera>();
            if (reflectionCamera != null)
                reflectionCamera.AspectRatio = (float)Graphics.Width / Graphics.Height;


            // TMS: Which pane are we over?
            if (ShowTwoViewports && Viewport2 != null)
            {
				bool overViewport2;
				IntVector2 mousePosition = Input.MousePosition; //TBD - ScreenPosition;
                IntRect rect2 = Viewport2.Rect;
                if (mousePosition.X < rect2.Right)
                    overViewport2 = true;
                else
                    overViewport2 = false;   // (redundant - for debugging)

                // So WASD keys know which camera to affect.
                OverViewport2 = overViewport2;
            }

            if (WallKeys && Input.GetKeyPress(Key.N))
                StartNewWall();

            if (DrawWallPressDrag)   //OverViewport2 && 
				OnUpdate_MaybeDrawingWall();

            if (MoveCamera3DFirstOrThirdPerson(timeStep, 10.0f, OverViewport2) && DrawWallAsFly)
            {
                if (!OverViewport2 && Input.GetMouseButtonDown(MouseButton.Left))
                    ExtendWallAtCameraPosition();
            }
        }

        private void OnUpdate_Wireframe()
        {
			SetWireframeVisibility(ShowWireframe);

			//if (ShowWireframe)
   //         {
   //             int liveMillis = 0;
   //             uint now = Time.SystemTime;
   //             if (_startTime > 0)
   //                 liveMillis = (int)(now - _startTime);
   //             else
   //                 _startTime = now;

   //             int flashesPerSecond = 6;
   //             // "1.0f" to have wireframe stay visible; lesser values to flash on and off.
   //             float fractionOn = 1.0f; //0.7f; //0.3f;
   //             int millisPerFlash = (int)Math.Round(1000.0 / flashesPerSecond);
   //             int millisOn = (int)Math.Round(millisPerFlash * fractionOn);

   //             bool asWireframe = (liveMillis % millisPerFlash) < millisOn;
   //             SetWireframeVisibility(asWireframe);
   //         }
        }

		private bool _wallDrawStartedOverViewport2;
		private bool _suppressWallDraw;

        private void StartNewWall()
        {
			_suppressWallDraw = false;
			_wallDrawStartedOverViewport2 = OverViewport2;

			// Keep existing wall, if it has contents.
			if (CurrentWall != null)
            {
                if (CurrentWall.Points.Count <= 0)
                {
                    // Its empty, so use it. Happens if StartNewWall twice, without adding contents.
                    // Starting over.
                    WallDrawStarted = false;
					CurrentWall.Clear();
                    return;
                }
                else
                {
                    // "Keep" is automatic, because CurrentWall was added to WallsNode when it was created.
                }
            }

            // Start a new one.
			FlushAndNullWall();
			// Don't use previous wall's node.
			CurrentWallNode = null;
            // Starting over.
            WallDrawStarted = false;
			// Start now, so we can set properties (DoDeferPoint) on it.
			StartWall();
        }


		private bool _wasDrawing;
		private bool _prevModeWasFreehand;
		// When click-click, this is used to start a new wall.
		private bool _sawShiftToggleUp;
		private bool _wasClickDrawing;

		private void OnUpdate_MaybeDrawingWall()
        {
			if (DoDeleteWalls()) {
				Walls.Clear();
				CurrentWall = null;
				WallsNode?.RemoveAllChildren();
				CurrentWallNode = null;
				WallDrawStarted = false;
				return;
			}

			bool drawing = Input.GetMouseButtonDown(MouseButton.Left);
			if (Input.GetKeyDown(Key.Shift)) {
				SetDoDeferPoint(false);
				PointToPointWallDrawing(drawing);
			} else {
				// Down-move-up freehand drawing.
				FreehandWallDrawing(drawing);
			}
		}

		private void SetDoDeferPoint(bool value)
		{
			if (CurrentWall != null)
				CurrentWall.DoDeferPoint = value;
			else if (value)
				throw new InvalidProgramException("SetDoDeferPoint requires wall");
		}

		private bool DoDeleteWalls()
		{
			return Input.GetKeyDown(Key.Shift) && Input.GetKeyDown(Key.Delete);
		}

		/// <summary>
		/// Click-click-click: wall segment per mouse press.
		/// </summary>
		private void PointToPointWallDrawing(bool drawing)
		{
			if (drawing) {
				if (_sawShiftToggleUp || CurrentWall == null)// && !_prevModeWasFreehand)
					StartNewWall();
				if (MousePositionOnGroundPlane(out Vector2 groundPt))
					ExtendWall(groundPt, true);
				_sawShiftToggleUp = false;
				_prevModeWasFreehand = false;
				_wasClickDrawing = true;
			}
		}

		/// <summary>
		/// Down-move-up freehand drawing.
		/// </summary>
		/// <param name="drawing"></param>
		private void FreehandWallDrawing(bool drawing)
		{
			if (drawing) {
				// So can alternate freehand and click-click without starting new wall.
				_sawShiftToggleUp = false;

				if (!_wasDrawing)
					StartNewWall();
				else if (_wallDrawStartedOverViewport2 != OverViewport2)
					// To avoid wall making segment that leads to a far point.
					_suppressWallDraw = true;
				if (_suppressWallDraw)
					return;
				SetDoDeferPoint(true && !GroundLine.SingleGeometryTEST);
				if (MousePositionOnGroundPlane(out Vector2 groundPt))
					ExtendWall(groundPt);
				_prevModeWasFreehand = true;

			} else {
				// We get here if SHIFT is not down, nor is left-mouse.
				if (!_prevModeWasFreehand && _wasClickDrawing)
					_sawShiftToggleUp = true;
				if (_wasDrawing) {
					// Maybe.
					EndWall();
				}
			}

			_wasDrawing = drawing;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="groundPt">out ray collision position in XZ.</param>
		/// <returns>True if ray hit a mesh.</returns>
		private bool MousePositionOnGroundPlane(out Vector2 groundPt)
		{
			var normScreenPt = MouseToNormalizedScreenPt();
			Ray cameraRay = CurrentCamera.GetScreenRay(normScreenPt.X, normScreenPt.Y);
			var result = Octree.RaycastSingle(cameraRay, RayQueryLevel.Triangle, 10000, DrawableFlags.Geometry);
			if (result != null) {
				groundPt = result.Value.Position.XZ();
				//Debug.WriteLine($"--- mouse={Input.MousePosition} -> {groundPt} in scene ---");
				return true;
			}

			groundPt = new Vector2();   // Caller should ignore this.
			return false;
		}

		public Vector2 MouseToNormalizedScreenPt()
		{
			var screenPt = Input.MousePosition;
			IntRect rect = CurrentViewport.Rect;
			var normScreenPt = new Vector2((screenPt.X - rect.Left) / rect.Width(), (screenPt.Y - rect.Top) / rect.Height());
			//Debug.WriteLine($"--- screen={screenPt}, rect={rect} -> norm={normScreenPt} ---");
			return normScreenPt;
		}

		// This approach might work for an orthographic top view (but ours is currently perspective).
		//private Vector2 MousePositionOnGroundPlane()
		//{
		//	var screenPt = Input.MousePosition;
		//	IntRect rect2 = Viewport2.Rect;
		//	// TODO: What makes this multiplier necessary?
		//	float mult = 2;
		//	var normScreenPt = new Vector2(screenPt.X / rect2.Width(), screenPt.Y / rect2.Height());
		//	float depth = 100;   // TODO
		//	Vector2 pt = mult * Camera2.ScreenToWorldPoint(new Vector3(normScreenPt.X, normScreenPt.Y, depth)).XZ();
		//	Debug.WriteLine($"--- mouse={screenPt} -> {pt} in scene ---");
		//	return pt;
		//}

		private void EndWall()
		{
			// NOTE: Currently we don't allow you to draw a "single-point" wall.
			if (CurrentWall != null && CurrentWall.Points.Count > 0) {
				// When freehand drawing, catch up to mouse position.
				CurrentWall.Flush();
				// NOTE: Currently we don't allow you to draw a "single-point" wall.
				if (!LastPenPosition2D.NearlyEquals(LastWallPosition2D, 0.05f)) {
					CurrentWall.AddPoint(LastPenPosition2D.asDist());
					LastWallPosition2D = LastPenPosition2D;
					CurrentWall.Flush();
				}
				// Always do, for bake.
				CurrentWall.OnUpdate(true);
			}

			//MAYBE CurrentWall.CalcTangents();

			// Done with this wall.  TBD: Interferes with point-to-point drawing?
			//MAYBE WallDrawStarted = false;
		}

		private void FlushAndNullWall()
		{
			if (CurrentWall != null && CurrentWall.HasContents()) {
				CurrentWall.Flush();
				CurrentWall.OnUpdate(true);
				CurrentWall = null;
			}
		}

		private void ExtendWallAtCameraPosition()
		{
			Vector2 penPosition2D = InGroundPlane(CurrentCameraMainNode.Position);
			ExtendWall(penPosition2D);
		}

		private void ExtendWall(Vector2 penPosition2D, bool maybeBend = false)
		{
			bool doAddPoint = false;
			float length = 0;
			if (!WallDrawStarted) {
				// SETS CurrentWall, WallDrawStarted.
				StartWall();
				doAddPoint = true;
			} else {
				length = Vector2.Subtract(penPosition2D, LastWallPosition2D).Length;
				if (length > MinWallSegmentLength)
					doAddPoint = true;
			}

			if (doAddPoint) {
				if (maybeBend && CurrentWall != null) {
					if (CurrentWall.Points.Count > 1 && length > 2 * MinWallSegmentLength) {
						// Add a short join segment. This "absorbs" any angle change, so long segment has full wall width.
						// TBD: Calculate angle of direction change. Don't need for small angles.
						const float JoinLength = 0.1f;
						float joinWgt = JoinLength / length;
						// End of short join segment.
						var joinPt = U.Lerp(LastWallPosition2D, penPosition2D, joinWgt);
						CurrentWall.AddPoint((Dist2D)joinPt);
					}
				}
				// Create or Extend a path, and a corresponding extruded model.
				CurrentWall.AddPoint((Dist2D)penPosition2D);
				CurrentWall.OnUpdate();

				LastWallPosition2D = penPosition2D;
			}

			// LastPenPosition2D used to draw short segment when lift pen.
			LastPenPosition2D = penPosition2D;
		}

		/// <summary>
		/// SETS CurrentWall, WallDrawStarted.
		/// </summary>
		private void StartWall()
        {
			FlushAndNullWall();
			
			WallDrawStarted = true;
            CurrentWall = new GroundLine(2, 8);
			// Uncomment for "floating wall".
			//CurrentWall.BaseAltitude = 8 * DistD.OneDefaultUnit;   // tmstest
			//Debug.WriteLine($"--- StartWall N walls={Walls.Count} ---");
        }

        /// <summary>
        /// For test, where create a model at each point.
        /// </summary>
        /// <param name="penPosition2D"></param>
        private void AddBoxAt(Vector2 penPosition2D)
        {
            //float boxScale = MinWallSegmentLength / 2;
            // "- small-value": Deliberate gap to see segments.
            float boxScale = MinWallSegmentLength - 0.1f;
            AddBoxToScene(Scene, FromGroundPlane(penPosition2D), boxScale, true);
        }
        #endregion


        #region --- scene specifics ----------------------------------------
        void CreateGroundPlaneScene(Scene scene)
		{
			// Create the Octree component to the scene. This is required before adding any drawable components, or else nothing will
			// show up. The default octree volume will be from (-1000, -1000, -1000) to (1000, 1000, 1000) in world coordinates; it
			// is also legal to place objects outside the volume but their visibility can then not be checked in a hierarchically
			// optimizing manner
			scene.CreateComponent<Octree>();


			// Create a child scene node (at world origin) and a StaticModel component into it. Set the StaticModel to show a simple
			// plane mesh with a "stone" material. Note that naming the scene nodes is optional. Scale the scene node larger
			// (100 x 100 world units)
			var planeNode = scene.CreateChild("Plane");
			planeNode.Scale = new Vector3(GroundSize, 1, GroundSize);
			var planeObject = planeNode.CreateComponent<StaticModel>();
			planeObject.Model = ResourceCache.GetModel("Models/Plane.mdl");
			planeObject.SetMaterial(ResourceCache.GetMaterial("Materials/StoneTiled.xml"));


			// Create a directional light to the world so that we can see something. The light scene node's orientation controls the
			// light direction; we will use the SetDirection() function which calculates the orientation from a forward direction vector.
			// The light will use default settings (white light, no shadows)
			var lightNode = scene.CreateChild("DirectionalLight");
			lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f)); // The direction vector does not need to be normalized
			var light = lightNode.CreateComponent<Light>();
			light.LightType = LightType.Directional;


			// Create skybox. The Skybox component is used like StaticModel, but it will be always located at the camera, giving the
			// illusion of the box planes being far away. Use just the ordinary Box model and a suitable material, whose shader will
			// generate the necessary 3D texture coordinates for cube mapping
			var skyNode = scene.CreateChild("Sky");
			skyNode.SetScale(500.0f); // The scale actually does not matter
			var skybox = skyNode.CreateComponent<Skybox>();
			skybox.Model = ResourceCache.GetModel("Models/Box.mdl");
			skybox.SetMaterial(ResourceCache.GetMaterial("Materials/Skybox.xml"));


			// Create more StaticModel objects to the scene, randomly positioned, rotated and scaled. For rotation, we construct a
			// quaternion from Euler angles where the Y angle (rotation about the Y axis) is randomized. The mushroom model contains
			// LOD levels, so the StaticModel component will automatically select the LOD level according to the view distance (you'll
			// see the model get simpler as it moves further away). Finally, rendering a large number of the same object with the
			// same material allows instancing to be used, if the GPU supports it. This reduces the amount of CPU work in rendering the
			// scene.
			ScatterObjects(scene, NScatteredModels);

			GroundPlaneSceneMainCameraSettings(Camera1FinalNode);
		}

		void CreateTerrainScene(Scene scene)
		{
			var cache = ResourceCache;

			// Create octree, use default volume (-1000, -1000, -1000) to (1000, 1000, 1000)
			scene.CreateComponent<Octree>();

			// Create a Zone component for ambient lighting & fog control
			var zoneNode = scene.CreateChild("Zone");
			var zone = zoneNode.CreateComponent<Zone>();
			zone.SetBoundingBox(new BoundingBox(-1000.0f, 1000.0f));
			zone.AmbientColor = new Color(_ZoneAmbient, _ZoneAmbient, _ZoneAmbient);
			if (IncludeFog) {
				float fogBrightness = ShowWireframe ? 0.0f : 1.0f;
				zone.FogColor = new Color(fogBrightness, fogBrightness, fogBrightness);
				zone.FogStart = 500.0f;
				zone.FogEnd = 750.0f;
			}

			AddDirectionalLight(scene);

			if (!ShowWireframe) {
				// Create skybox. The Skybox component is used like StaticModel, but it will be always located at the camera, giving the
				// illusion of the box planes being far away. Use just the ordinary Box model and a suitable material, whose shader will
				// generate the necessary 3D texture coordinates for cube mapping
				var skyNode = scene.CreateChild("Sky");
				skyNode.SetScale(500.0f); // The scale actually does not matter
				var skybox = skyNode.CreateComponent<Skybox>();
				skybox.Model = cache.GetModel("Models/Box.mdl");
				skybox.SetMaterial(cache.GetMaterial("Materials/Skybox.xml"));
			}

			// Create heightmap terrain
			var terrainNode = scene.CreateChild("Terrain");
			terrainNode.Position = new Vector3(0.0f, 0.0f, 0.0f);
			Terrain = terrainNode.CreateComponent<Terrain>();
			Terrain.PatchSize = 64;
			Terrain.Spacing = new Vector3(2.0f, 0.5f, 2.0f); // Spacing between vertices and vertical resolution of the height map
			Terrain.Smoothing = true;
			Terrain.SetHeightMap(cache.GetImage("Textures/HeightMap.png"));
			Terrain.Material = cache.GetMaterial("Materials/Terrain.xml");
			// The terrain consists of large triangles, which fits well for occlusion rendering, as a hill can occlude all
			// terrain patches and other objects behind it
			Terrain.Occluder = true;
			MaybeSetWireframeMaterial();
			ScatterObjects(scene, NScatteredModels);

			if (IncludeWater) {
				// Create a water plane object that is as large as the terrain
				WaterNode = scene.CreateChild("Water");
				WaterNode.Scale = new Vector3(2048.0f, 1.0f, 2048.0f);
				WaterNode.Position = new Vector3(0.0f, 5.0f, 0.0f);
				var water = WaterNode.CreateComponent<StaticModel>();
				water.Model = cache.GetModel("Models/Plane.mdl");
				water.SetMaterial(cache.GetMaterial("Materials/Water.xml"));
				// Set a different viewmask on the water plane to be able to hide it from the reflection camera
				water.ViewMask = 0x80000000;
			}
			
			TerrainSceneMainCameraSettings(Camera1FinalNode, Camera1);
		}

		private static void AddDirectionalLight(Scene scene)
		{
			// Create a directional light to the world. Enable cascaded shadows on it
			var lightNode = scene.CreateChild("DirectionalLight");
			//lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
			lightNode.SetDirection(new Vector3(0.3f, -1.0f, 0.4f));
			//lightNode.SetDirection(new Vector3(0.1f, -1.0f, 0.1f));
			var light = lightNode.CreateComponent<Light>();
			light.LightType = LightType.Directional;
			light.CastShadows = true;
			light.ShadowBias = new BiasParameters(0.00025f, 0.5f);
			if (ShadowCascade) {
				light.ShadowCascade = new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);
			}
			//ttttt light.SpecularIntensity = 0.5f;
			// true=Apply slightly overbright lighting to match the skybox
			float bright = false ? 1.2f : 1.0f;
			light.Color = new Color(bright, bright, bright);
		}

		private void ScatterObjects(Scene scene, int nScatteredModels)
		{
			if (IncludeScatteredModels) {
				if (ScatteredModelsAreBoxes)
					ScatterBoxes(scene, nScatteredModels);
				else
					ScatterMushrooms(scene, nScatteredModels);
			}
		}

		private void ScatterMushrooms(Scene scene, int nScatteredModels)
		{
			for (int i = 0; i < nScatteredModels; i++) {
				var mushroom = scene.CreateChild("Mushroom");
				mushroom.Position = new Vector3(random.Next(90) - 45, 0, random.Next(90) - 45);
				mushroom.Rotation = new Quaternion(0, random.Next(360), 0);
				mushroom.SetScale(0.5f + random.Next(20000) / 10000.0f);
				var mushroomObject = mushroom.CreateComponent<StaticModel>();
				mushroomObject.Model = ResourceCache.GetModel("Models/Mushroom.mdl");
				mushroomObject.SetMaterial(ResourceCache.GetMaterial("Materials/Mushroom.xml"));
			}
		}

		private void ScatterBoxes(Scene scene, int nScatteredModels)
		{
			var colors = new Color[] {
					Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Cyan, Color.Magenta,
					Color.White, Color.Black
			};
			List<Material> materials = new List<Material>();
			if (RandomColorBoxes) {
				// Have some different color boxes, so can tell them apart (somewhat).
				foreach (var color in colors) {
					var material = ResourceCache.GetMaterial(BoxMaterialName()).Clone();
					// NOTE: This self-lighting lessens effectiveness of Normal map.
					material.SetShaderParameter("AmbientColor", color);
					materials.Add(material);
				}
			}

			// Create boxes in the terrain. Always face outward along the terrain normal
			float halfGroundSize = GroundSize / 2;
			for (int i = 0; i < nScatteredModels; ++i) {
				Vector3 position = new Vector3(NextRandom(GroundSize) - halfGroundSize, 0.0f, NextRandom(GroundSize) - halfGroundSize);

				Material boxMaterial = null;
				if (RandomColorBoxes)
					// TMS: Make the boxes different.
					boxMaterial = materials[i % colors.Length];

				AddBoxToScene(scene, position, BoxScale, true, boxMaterial, ResourceCache);
			}
		}

		public void MaybeSetWireframeMaterial(Material mat = null)
		{
			if (ShowWireframe) {
				if (ShowTerrainWireframe)
					WireframeMaterial = Terrain.Material;   // To show terrain's wireframe.
				else if (ShowWallWireframe && mat != null) {
					WireframeMaterial = mat;
				}
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="position">if terrainRelative, .Y is height above terrain.</param>
		/// <param name="boxScale"></param>
		/// <param name="terrainRelative">when true, position.Y is height above terrain. And box rotates to follow the terrain's surface.</param>
		/// <param name="boxMaterial"></param>
		/// <param name="cache"></param>
		public void AddBoxToScene(Node parent, Vector3 position, float boxScale, bool terrainRelative,
                                   Material boxMaterial = null, Urho.Resources.ResourceCache cache = null)
        {
			if (Terrain == null)
				terrainRelative = false;

			if (cache == null)
                cache = ResourceCache;
            // AFTER set cache.
            if (boxMaterial == null) {
				boxMaterial = cache.GetMaterial(BoxMaterialName());
			}

			var objectNode = parent.CreateChild("Box");

			// "boxScale/2": box's position is center; want its bottom to touch ground.
			// "- small-value": slightly underground so no gap due to uneven ground height. TBD: Proportional to box size?
			//   TBD: proportional to angle between normal and vertical axis?
			position.Y = U2.GetTerrainHeight(Terrain, position) + boxScale / 2 - 0.1f; //2.25f;
			objectNode.Position = position;

            if (terrainRelative && !RandomBoxRotation) {
                // Create a rotation quaternion from up vector to terrain normal
                objectNode.Rotation = Quaternion.FromRotationTo(new Vector3(0.0f, 1.0f, 0.0f), Terrain.GetNormal(position));
				// TBD: How ALSO rotate around vertical axis?
            } else if (RandomBoxRotation) {
				objectNode.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, U.RandomNextInt(360));
			}

			objectNode.SetScale(boxScale);

            StaticModel obj = objectNode.CreateComponent<StaticModel>();
            obj.Model = cache.GetModel("Models/Box.mdl");
            obj.CastShadows = BoxesHaveShadows;
            obj.SetMaterial(boxMaterial);
        }

		private static float RandomM1ToP1()
		{
			return (float)U.RandomPlusMinus(1);
		}

		public static string BoxMaterialName()
		{
			//return "Materials/Stone.xml";
			//return "Materials/StoneWall4.xml";
			//return "Materials/StoneWall4Normal.xml";
			//return "Parallax/Materials/ParallaxStonesDemoVer.xml";
			//return "Materials/StonesParallaxOffset.xml";
			//return "Materials/StonesParallaxOcclusion.xml";
			//return "Materials/StoneWall4ParallaxOffset.xml";
			//return "Materials/StoneWall4ParallaxOcclusion.xml";

			//return "Materials/BricksNormal.xml";
			//return "Materials/BricksParallaxOffset.xml";
			//return "Materials/BricksParallaxOcclusion.xml";
			//return "Materials/BrickWall19.xml";
			return "Materials/BrickWall19ParallaxOcclusion.xml";
		}

		bool _wasVisible = false;

        void SetWireframeVisibility(bool visible)
        {
			if (WireframeMaterial == null)
				return;

            if (visible && !_wasVisible)
            {
                WireframeMaterial.FillMode = FillMode.Wireframe;
				if (IncludeWater) {
					// Water exists over large area (not just where above the surface); remove it when terrain is wireframe.
					//Scene.RemoveChild(WaterNode);
					WaterNode.Enabled = false;
				}
			}
            else if (!visible && _wasVisible)
            {
                WireframeMaterial.FillMode = FillMode.Solid;
                //MEMORY_CRASH Scene.AddChild(WaterNode);
                ///WaterNode.Enabled = true;
            }

            _wasVisible = visible;
        }


        private void GroundPlaneSceneMainCameraSettings(Node cameraPositionNode)
        {
            // Set an initial position (for the camera node(s)) above the plane.
            cameraPositionNode.Position = new Vector3(0, 5, 0);
			// See shadowed side of box too.
			Yaw = -20;
			ApplyPitchYawToCamera();
			MaybeInitThirdPersonPerspective();
		}

		private void TerrainSceneMainCameraSettings(Node cameraPositionNode, Camera camera)
        {
            camera.FarClip = 750.0f;
			// Set an initial position (for the camera scene node) above the plane.
			float startAltitude = 7.0f;
			if (StartCameraOnLand)
            {
				Yaw = 30;
				Camera1MainNode.Position = new Vector3(20.0f, startAltitude, 100.0f);
			} else {
				if (BoxesHaveShadows)
					Camera1MainNode.Position = new Vector3(-20.0f, startAltitude, -60.0f);
				else
					Camera1MainNode.Position = new Vector3(0.0f, startAltitude, -20.0f);
			}
			//if (BoxesHaveShadows)
			//	Yaw = 210;  // See direction where shadows fall.

			EnforceMinimumAltitudeAboveTerrain(Camera1MainNode, startAltitude);
			ApplyPitchYawToCamera();
			MaybeInitThirdPersonPerspective();
        }

		private void MaybeInitThirdPersonPerspective()
		{
			if (ThirdPersonPerspective) {
				float altitude = Camera1MainNode.Position.Altitude();
				Camera1MainNode.Position = U.WithAltitude(Camera1MainNode.Position, 0);
				Pitch = 45;
				ApplyPitchYawToCamera();
				MaybeApplyThirdPersonPerspective();
			}
		}
        #endregion


        #region --- First camera and viewport ----------------------------------------
        /// <summary>
        /// When MovementIgnoresPitch, there is a separate "Camera1MainNode".
        /// </summary>
        /// <returns>parentOfCamera: The node that will be parent of Camera1FinalNode.</returns>
        private Node GetParentOfCamera1Final()
        {
            Camera1MainNode = null;

            Node parentOfCamera;
            if (MovementIgnoresPitch)
            {
                Camera1MainNode = Scene.CreateChild("cameraYaw");
                parentOfCamera = Camera1MainNode;
            }
            else
                parentOfCamera = Scene;

            return parentOfCamera;
        }


        void SetupOneViewport()
        {
            // Set up a viewport to the Renderer subsystem so that the 3D scene can be seen. We need to define the scene and the camera
            // at minimum. Additionally we could configure the viewport screen size and the rendering path (eg. forward / deferred) to
            // use, but now we just use full screen and default render path configured in the engine command line options
            Renderer.SetViewport(0, new Viewport(Context, Scene, Camera1, null));

            if (UseTerrainScene && IncludeWater)
                SetupWaterReflectionAndItsViewport(Graphics, ResourceCache);
        }

        private void SetupWaterReflectionAndItsViewport(Graphics graphics, Urho.Resources.ResourceCache cache)
        {
			if (!IncludeWater)
				return;

            // Create a mathematical plane to represent the water in calculations

            Plane waterPlane = new Plane(WaterNode.WorldRotation * new Vector3(0.0f, 1.0f, 0.0f), WaterNode.WorldPosition);
            // Create a downward biased plane for reflection view clipping. Biasing is necessary to avoid too aggressive clipping
            Plane waterClipPlane = new Plane(WaterNode.WorldRotation * new Vector3(0.0f, 1.0f, 0.0f), WaterNode.WorldPosition - new Vector3(0.0f, 0.1f, 0.0f));

            // Create camera for water reflection
            // It will have the same farclip and position as the main viewport camera, but uses a reflection plane to modify
            // its position when rendering
            ReflectionCameraNode = Camera1FinalNode.CreateChild();
            var reflectionCamera = ReflectionCameraNode.CreateComponent<Camera>();
            reflectionCamera.FarClip = 750.0f;
            reflectionCamera.ViewMask = 0x7fffffff; // Hide objects with only bit 31 in the viewmask (the water plane)
            reflectionCamera.AutoAspectRatio = false;
            reflectionCamera.UseReflection = true;
            reflectionCamera.ReflectionPlane = waterPlane;
            reflectionCamera.UseClipping = true; // Enable clipping of geometry behind water plane
            reflectionCamera.ClipPlane = waterClipPlane;
            // The water reflection texture is rectangular. Set reflection camera aspect ratio to match
            reflectionCamera.AspectRatio = (float)graphics.Width / graphics.Height;
            // View override flags could be used to optimize reflection rendering. For example disable shadows
            //reflectionCamera.ViewOverrideFlags = ViewOverrideFlags.DisableShadows;

            // Create a texture and setup viewport for water reflection. Assign the reflection texture to the diffuse
            // texture units of the water material
            int texSize = 1024;
            Texture2D renderTexture = new Texture2D();
            renderTexture.SetSize(texSize, texSize, Graphics.RGBFormat, TextureUsage.Rendertarget);
            renderTexture.FilterMode = TextureFilterMode.Bilinear;
            RenderSurface surface = renderTexture.RenderSurface;
            var rttViewport = new Viewport(Context, Scene, reflectionCamera, null);
            surface.SetViewport(0, rttViewport);
            var waterMat = cache.GetMaterial("Materials/Water.xml");
            waterMat.SetTexture(TextureUnit.Diffuse, renderTexture);
        }
        #endregion


        #region --- Two viewports (and two cameras) ----------------------------------------
        void SetupSecondCamera()
        {
            Camera2MainNode = Scene.CreateChild("TopViewMain");
            // Move camera 2 up in the air, to show larger terrain area.
            Camera2MainNode.Position = U.WithAltitude(Camera1MainNode.Position, InitialAltitude2);

            Camera2FinalNode = Camera2MainNode.CreateChild("TopViewFinal");
            // Straight down.
            Camera2FinalNode.Rotation = new Quaternion(90, 0, 0);

            Camera topCamera = Camera2FinalNode.CreateComponent<Camera>();
        }

        /// <summary>
        /// Creates TWO Viewports.
        /// </summary>
        void SetupTwoViewports()
        {
            var renderer = Renderer;
            var graphics = Graphics;

            renderer.NumViewports = 2;
            int halfWidth = (int)(Graphics.Width / 2);

            // Set up the first camera viewport as right-hand pane (half of screen width).
            var rect = RectBySize(halfWidth, 0, halfWidth, Graphics.Height);
			Viewport1 = new Viewport(Context, Scene, Camera1, rect, null);
			Renderer.SetViewport(0, Viewport1);

            var cache = ResourceCache;
            if (false)   // TMS "false": Disable Render PostProcessing not important for this demo.
            {
                // Clone the default render path so that we do not interfere with the other viewport, then add
                // bloom and FXAA post process effects to the front viewport. Render path commands can be tagged
                // for example with the effect name to allow easy toggling on and off. We start with the effects
                // disabled.
                RenderPath effectRenderPath = Viewport1.RenderPath.Clone();
                effectRenderPath.Append(cache.GetXmlFile("PostProcess/Bloom.xml"));
                effectRenderPath.Append(cache.GetXmlFile("PostProcess/FXAA2.xml"));
                // Make the bloom mixing parameter more pronounced
                effectRenderPath.SetShaderParameter("BloomMix", new Vector2(0.9f, 0.6f));

                effectRenderPath.SetEnabled("Bloom", false);
                effectRenderPath.SetEnabled("FXAA2", false);
                Viewport1.RenderPath = effectRenderPath;
            }

            // Set up the second camera viewport as left-hand pane (half of screen width).
            rect = RectBySize(0, 0, halfWidth, graphics.Height);
			Camera2 = Camera2FinalNode.GetComponent<Camera>();
			Viewport2 = new Viewport(Context, Scene, Camera2, rect, null);

            renderer.SetViewport(1, Viewport2);
        }
        #endregion


        #region --- local Helpers ----------------------------------------
        /// <summary>
        /// Convert pixels to Avalonia Device-Independent Units.
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public float ToDIUnits(int pixels)
        {
            return pixels / (float)avaloniaContext.RenderScaling;
        }
        #endregion


        #region --- Graphic Helpers (move to a static class) ----------------------------------------
        public static IntRect RectBySize(int xLeft, int yTop, int width, int height)
        {
            return new IntRect(xLeft, yTop, xLeft + width, yTop + height);
        }

        /// <summary>
        /// Our 3D altitude is "Y", so XZ is position in ground plane.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 InGroundPlane(Vector3 position)
        {
            // Our 3D altitude is "Y", so XZ is position in ground plane.
            return new Vector2(position.X, position.Z);
        }

        /// <summary>
        /// Our 3D altitude is "Y", so the position goes to X and Z.
        /// </summary>
        /// <param name="position2D"></param>
        /// <returns></returns>
        public static Vector3 FromGroundPlane(Vector2 position2D)
        {
            // Our 3D altitude is "Y", so the position goes to X and Z.
            return new Vector3(position2D.X, 0, position2D.Y);
        }
        #endregion
    }
}
