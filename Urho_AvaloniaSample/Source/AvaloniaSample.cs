using System;
using System.Collections.Generic;
using Urho;
using Urho.Avalonia;
using Urho.Gui;
using Urho.IO;
using Urho.Urho2D;

namespace AvaloniaSample
{
	public partial class AvaloniaSample : Sample
	{
        public static AvaloniaSample It;   // TMS

        const bool IncludeAvaloniaLayer = false;   // TMS
        const bool UseWaterScene = true;//true;   // TMS
        const bool DrawWallAsFly = false && UseWaterScene;   // TMS
        const bool ShowWireframe = false;//false;   // TMS

        Camera Camera = null;
		public Scene Scene;
        Viewport Viewport2;
        // Used by Water Scene.
        public Terrain Terrain;
        Node WaterNode;
        Node ReflectionCameraNode;
        // Used when showing wireframe.
        Material WireframeMaterial;

        // TheWall.
        public Node WallNode;

		// private SampleAvaloniaWindow _window;
        [Preserve]
        public AvaloniaSample() : base(
            new ApplicationOptions(assetsFolder: "Data;CoreData")
            {
                ResizableWindow = true
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

            Node parentOfCamera = GetMainCamerasParentAndMaybeSetCameraWorldBaseNode();
            // Create a scene node for the camera, which we will move around
            // Can override camera's default settings later. (1000 far clip distance, 45 degrees FOV, set aspect ratio automatically)
            CameraNode = parentOfCamera.CreateChild("camera");
            Camera = CameraNode.CreateComponent<Camera>();
            //if (ShowWireframe)
            //    Camera.FillMode = FillMode.Wireframe;
            SetCameraPositionNode();

            if (UseWaterScene)
                CreateWaterScene(Scene);
            else
                CreateMushroomScene(Scene);

            if (ShowTwoViewports)
                SetupSecondCamera();

            SimpleCreateInstructionsWithWasd();
            if (ShowTwoViewports)
                SetupTwoViewports();
            else
                SetupOneViewport();


            UI.Root.SetDefaultStyle(ResourceCache.GetXmlFile("UI/DefaultStyle.xml"));

			Input.SetMouseVisible(true);
			Input.SetMouseMode(MouseMode.Free);

            if (IncludeAvaloniaLayer)
                _SetupAvaloniaUI();
        }
        #endregion

        #region --- OnUpdate ----------------------------------------
        uint _startTime = 0;
        bool WallDrawStarted;   // TMS
        SceneSource.GroundLine TheWall;
        Vector2 LastWallPosition2D;
        const float MinWallSegmentLength = 1;   // TBD: Good value.

        private IntVector2 _lastScreenSize;

        protected override void OnUpdate(float timeStep)
        {
            if (Graphics.Size != _lastScreenSize)
            {
                _lastScreenSize = Graphics.Size;
                _DockWindows();
            }

            base.OnUpdate(timeStep);

            OnUpdate_Wireframe();

            var reflectionCamera = ReflectionCameraNode?.GetComponent<Camera>();
            if (reflectionCamera != null)
                reflectionCamera.AspectRatio = (float)Graphics.Width / Graphics.Height;


            // TMS HACK: Which pane are we over?
            bool overViewport2 = false;
            if (ShowTwoViewports && Viewport2 != null)
            {
                IntVector2 mousePosition = Input.MousePosition; //TBD - ScreenPosition;
                IntRect rect2 = Viewport2.Rect;
                if (mousePosition.X < rect2.Right)
                    overViewport2 = true;
                else
                    overViewport2 = false;   // (redundant - for debugging)
            }

            if (Camera != null)
            {
                if (SimpleMoveCamera3D(timeStep, 10.0f, overViewport2) && DrawWallAsFly)
                {
                    if (Input.GetMouseButtonDown(MouseButton.Left))
                    {
                        OnUpdate_DrawWall();
                    }
                }

            }
        }

        private void OnUpdate_Wireframe()
        {
            if (ShowWireframe)
            {
                int liveMillis = 0;
                uint now = Time.SystemTime;
                if (_startTime > 0)
                    liveMillis = (int)(now - _startTime);
                else
                    _startTime = now;

                int flashesPerSecond = 6;
                // "1.0f" to have wireframe stay visible; lesser values to flash on and off.
                float fractionOn = 1.0f; //0.7f; //0.3f;
                int millisPerFlash = (int)Math.Round(1000.0 / flashesPerSecond);
                int millisOn = (int)Math.Round(millisPerFlash * fractionOn);

                bool asWireframe = (liveMillis % millisPerFlash) < millisOn;
                SetWireframeVisibility(asWireframe);
            }
        }

        private void OnUpdate_DrawWall()
        {
            Vector2 penPosition2D = InGroundPlane(CameraPositionNode.Position);
            bool doAddPoint = false;
            if (!WallDrawStarted)
            {
                StartWall();
                doAddPoint = true;
            }
            else
            {
                var length = Vector2.Subtract(penPosition2D, LastWallPosition2D).Length;
                if (length > MinWallSegmentLength)
                    doAddPoint = true;
            }

            if (doAddPoint)
            {
                // HACK: Put a box at this point.
                //AddBoxAt(penPosition2D);
                // Create or Extend a path, and a corresponding extruded model.
                TheWall.AddPoint(new Global.Distance2D(penPosition2D, null));
                TheWall.OnUpdate();

                LastWallPosition2D = penPosition2D;
            }
        }

        private void StartWall()
        {
            WallDrawStarted = true;
            TheWall = new SceneSource.GroundLine(2, 8);
        }

        private void AddBoxAt(Vector2 penPosition2D)
        {
            //float boxScale = MinWallSegmentLength / 2;
            // "- small-value": Deliberate gap to see segments.
            float boxScale = MinWallSegmentLength - 0.1f;
            AddBoxToScene(Scene, FromGroundPlane(penPosition2D), boxScale, true);
        }
        #endregion


        #region --- scene specifics ----------------------------------------
        void CreateMushroomScene(Scene scene)
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
            planeNode.Scale = new Vector3(100, 1, 100);
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
            const int NMushrooms = 20; //200
            for (int i = 0; i < NMushrooms; i++)
            {
                var mushroom = scene.CreateChild("Mushroom");
                mushroom.Position = new Vector3(random.Next(90) - 45, 0, random.Next(90) - 45);
                mushroom.Rotation = new Quaternion(0, random.Next(360), 0);
                mushroom.SetScale(0.5f + random.Next(20000) / 10000.0f);
                var mushroomObject = mushroom.CreateComponent<StaticModel>();
                mushroomObject.Model = ResourceCache.GetModel("Models/Mushroom.mdl");
                mushroomObject.SetMaterial(ResourceCache.GetMaterial("Materials/Mushroom.xml"));
            }

            MushroomSceneMainCameraSettings(CameraPositionNode);
        }

        void CreateWaterScene(Scene scene)
        {
            var cache = ResourceCache;

            // Create octree, use default volume (-1000, -1000, -1000) to (1000, 1000, 1000)
            scene.CreateComponent<Octree>();

            // Create a Zone component for ambient lighting & fog control
            var zoneNode = scene.CreateChild("Zone");
            var zone = zoneNode.CreateComponent<Zone>();
            zone.SetBoundingBox(new BoundingBox(-1000.0f, 1000.0f));
            zone.AmbientColor = new Color(0.15f, 0.15f, 0.15f);
            //float fogBrightness = ShowWireframe ? 0.2f : 1.0f;   // TMS
            float fogBrightness = ShowWireframe ? 0.0f : 1.0f;
            zone.FogColor = new Color(fogBrightness, fogBrightness, fogBrightness);
            zone.FogStart = 500.0f;
            zone.FogEnd = 750.0f;

            // Create a directional light to the world. Enable cascaded shadows on it
            var lightNode = scene.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;
            light.CastShadows = true;
            light.ShadowBias = new BiasParameters(0.00025f, 0.5f);
            light.ShadowCascade = new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);
            light.SpecularIntensity = 0.5f;
            // Apply slightly overbright lighting to match the skybox
            light.Color = new Color(1.2f, 1.2f, 1.2f);

            if (!ShowWireframe)
            {
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
            if (ShowWireframe)
                WireframeMaterial = Terrain.Material;


            // Have some different color boxes, so can tell them apart (somewhat).
            var colors = new Color[] {
                    Color.Red, Color.Green, Color.Blue, Color.Yellow, Color.Cyan, Color.Magenta,
                    Color.White, Color.Black
            };
            List<Material> materials = new List<Material>();
            foreach (var color in colors)
            {
                var material = cache.GetMaterial("Materials/Stone.xml").Clone();
                material.SetShaderParameter("AmbientColor", color);
                materials.Add(material);
            }

            // Create 1000 boxes in the terrain. Always face outward along the terrain normal
            int numObjects = 1000;
            float boxScale = 5.0f;
            for (int i = 0; i < numObjects; ++i)
            {
                Vector3 position = new Vector3(NextRandom(2000.0f) - 1000.0f, 0.0f, NextRandom(2000.0f) - 1000.0f);

                // TMS: Make the boxes different.
                Material boxMaterial = materials[i % colors.Length];

                AddBoxToScene(scene, position, boxScale, true, boxMaterial, cache);
            }

            // Create a water plane object that is as large as the terrain
            WaterNode = scene.CreateChild("Water");
            WaterNode.Scale = new Vector3(2048.0f, 1.0f, 2048.0f);
            WaterNode.Position = new Vector3(0.0f, 5.0f, 0.0f);
            var water = WaterNode.CreateComponent<StaticModel>();
            water.Model = cache.GetModel("Models/Plane.mdl");
            water.SetMaterial(cache.GetMaterial("Materials/Water.xml"));
            // Set a different viewmask on the water plane to be able to hide it from the reflection camera
            water.ViewMask = 0x80000000;

            WaterSceneMainCameraSettings(CameraPositionNode, Camera);
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
            if (cache == null)
                cache = ResourceCache;
            // AFTER set cache.
            if (boxMaterial == null)
            {
                boxMaterial = cache.GetMaterial("Materials/Stone.xml");
            }

            var objectNode = parent.CreateChild("Box");

            if (terrainRelative)
            {
                // "boxScale/2": box's position is center; want its bottom to touch ground.
                // "- small-value": slightly underground so no gap due to uneven ground height. TBD: Proportional to box size?
                //   TBD: proportional to angle between normal and vertical axis?
                position.Y = Terrain.GetHeight(position) + boxScale / 2 - 0.1f; //2.25f;
            }
            objectNode.Position = position;

            if (terrainRelative) {
                // Create a rotation quaternion from up vector to terrain normal
                objectNode.Rotation = Quaternion.FromRotationTo(new Vector3(0.0f, 1.0f, 0.0f), Terrain.GetNormal(position));
            }

            objectNode.SetScale(boxScale);

            StaticModel obj = objectNode.CreateComponent<StaticModel>();
            obj.Model = cache.GetModel("Models/Box.mdl");
            obj.CastShadows = true;
            obj.SetMaterial(boxMaterial);
        }


        bool _wasVisible = false;

        void SetWireframeVisibility(bool visible)
        {
            if (visible && !_wasVisible)
            {
                WireframeMaterial.FillMode = FillMode.Wireframe;
                // Water exists over large area (not just where above the surface); remove it when terrain is wireframe.
                //Scene.RemoveChild(WaterNode);
                WaterNode.Enabled = false;
            }
            else if (!visible && _wasVisible)
            {
                WireframeMaterial.FillMode = FillMode.Solid;
                //MEMORY_CRASH Scene.AddChild(WaterNode);
                ///WaterNode.Enabled = true;
            }

            _wasVisible = visible;
        }


        private void MushroomSceneMainCameraSettings(Node cameraPositionNode)
        {
            // Set an initial position (for the camera node(s)) above the plane.
            cameraPositionNode.Position = new Vector3(0, 5, 0);
        }

        private void WaterSceneMainCameraSettings(Node cameraPositionNode, Camera camera)
        {
            camera.FarClip = 750.0f;
            // Set an initial position (for the camera scene node) above the plane.
            //cameraPositionNode.Position = new Vector3(0.0f, 7.0f, -20.0f);
            CameraNode.Position = new Vector3(0.0f, 7.0f, -20.0f);
        }
        #endregion


        #region --- First camera and viewport ----------------------------------------
        /// <summary>
        /// When two viewports, sets CameraWorldBaseNode.
        /// </summary>
        /// <returns>parentOfMainCameraNode: The node that will be parent of CameraNode.</returns>
        private Node GetMainCamerasParentAndMaybeSetCameraWorldBaseNode()
        {
            Node parentOfMainCameraNode;
            if (ShowTwoViewports)
            {
                CameraWorldBaseNode = Scene.CreateChild("cameraBase");

                if (MovementIgnoresPitch)
                {
                    CameraYawNode = CameraWorldBaseNode.CreateChild("cameraYaw");
                    parentOfMainCameraNode = CameraYawNode;
                }
                else
                    parentOfMainCameraNode = CameraWorldBaseNode;
            }
            else
            {   // One camera - create it directly in the scene.
                parentOfMainCameraNode = Scene;
            }

            return parentOfMainCameraNode;
        }

        /// <summary>
        /// Sets CameraPositionNode.
        /// </summary>
        private void SetCameraPositionNode()
        {
            if (ShowTwoViewports)
            {
                // This node gets the position.
                CameraPositionNode = CameraWorldBaseNode;
            }
            else
            {   // One camera - create it directly in the scene.
                CameraPositionNode = CameraNode;
            }
        }


        void SetupOneViewport()
        {
            // Set up a viewport to the Renderer subsystem so that the 3D scene can be seen. We need to define the scene and the camera
            // at minimum. Additionally we could configure the viewport screen size and the rendering path (eg. forward / deferred) to
            // use, but now we just use full screen and default render path configured in the engine command line options
            Renderer.SetViewport(0, new Viewport(Context, Scene, Camera, null));

            if (UseWaterScene)
                SetupWaterReflectionAndItsViewport(Graphics, ResourceCache);
        }

        private void SetupWaterReflectionAndItsViewport(Graphics graphics, Urho.Resources.ResourceCache cache)
        {
            // Create a mathematical plane to represent the water in calculations

            Plane waterPlane = new Plane(WaterNode.WorldRotation * new Vector3(0.0f, 1.0f, 0.0f), WaterNode.WorldPosition);
            // Create a downward biased plane for reflection view clipping. Biasing is necessary to avoid too aggressive clipping
            Plane waterClipPlane = new Plane(WaterNode.WorldRotation * new Vector3(0.0f, 1.0f, 0.0f), WaterNode.WorldPosition - new Vector3(0.0f, 0.1f, 0.0f));

            // Create camera for water reflection
            // It will have the same farclip and position as the main viewport camera, but uses a reflection plane to modify
            // its position when rendering
            ReflectionCameraNode = CameraNode.CreateChild();
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
            if (false)
            {   // "Rear-view mirror".
                // Parent the rear camera node to the front camera node and turn it 180 degrees to face backward
                // Here, we use the angle-axis constructor for Quaternion instead of the usual Euler angles
                CameraNode2 = CameraNode.CreateChild("RearCamera");
                CameraNode2.Rotate(Quaternion.FromAxisAngle(Vector3.UnitY, 180.0f), TransformSpace.Local);

                Camera rearCamera = CameraNode2.CreateComponent<Camera>();
                rearCamera.FarClip = 300.0f;
                // Because the rear viewport is rather small, disable occlusion culling from it. Use the camera's
                // "view override flags" for this. We could also disable eg. shadows or force low material quality
                // if we wanted

                rearCamera.ViewOverrideFlags = ViewOverrideFlags.DisableOcclusion;
            }
            else
            {
                CameraNode2 = CameraWorldBaseNode.CreateChild("TopViewCamera");
                // Straight down. BUT then it shouldn't use rotation of first camera; only position.
                //CameraNode2.Rotate(Quaternion.FromAxisAngle(Vector3.UnitX, 90.0f), TransformSpace.Local);
                // TODO: motions are relative to camera orientation; how make them absolute orientation?
                CameraNode2.Rotation = new Quaternion(90, 0, 0);
                // Move camera 2 up in the air, to show larger terrain area.
                CameraNode2.Position = new Vector3(0, 100, 0);

                Camera topCamera = CameraNode2.CreateComponent<Camera>();
            }
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
            Viewport viewport = new Viewport(Context, Scene, CameraNode.GetComponent<Camera>(), null);
            var rect = RectBySize(halfWidth, 0, halfWidth, Graphics.Height);
            Renderer.SetViewport(0, new Viewport(Context, Scene, CameraNode.GetComponent<Camera>(), rect, null));

            var cache = ResourceCache;
            if (false)   // TMS "false": Disable Render PostProcessing not important for this demo.
            {
                // Clone the default render path so that we do not interfere with the other viewport, then add
                // bloom and FXAA post process effects to the front viewport. Render path commands can be tagged
                // for example with the effect name to allow easy toggling on and off. We start with the effects
                // disabled.
                RenderPath effectRenderPath = viewport.RenderPath.Clone();
                effectRenderPath.Append(cache.GetXmlFile("PostProcess/Bloom.xml"));
                effectRenderPath.Append(cache.GetXmlFile("PostProcess/FXAA2.xml"));
                // Make the bloom mixing parameter more pronounced
                effectRenderPath.SetShaderParameter("BloomMix", new Vector2(0.9f, 0.6f));

                effectRenderPath.SetEnabled("Bloom", false);
                effectRenderPath.SetEnabled("FXAA2", false);
                viewport.RenderPath = effectRenderPath;
            }

            // Set up the second camera viewport as left-hand pane (half of screen width).
            rect = RectBySize(0, 0, halfWidth, graphics.Height);
            Viewport2 = new Viewport(Context, Scene, CameraNode2.GetComponent<Camera>(), rect, null);

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
