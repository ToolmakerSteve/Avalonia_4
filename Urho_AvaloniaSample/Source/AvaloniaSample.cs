using Urho;
using System;
using Urho.Gui;
using Urho.Avalonia;
using Urho.IO;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.Styling;
using AC = Avalonia.Controls;
using Urho.Urho2D;

namespace AvaloniaSample
{
	public class AvaloniaSample : Sample
	{
        const bool UseWaterScene = true;//true;   // TMS
        const bool IncludeAvaloniaLayer = false;//true;   // TMS

		Camera Camera = null;
		Scene Scene;
        Viewport Viewport2;
        // Used by Water Scene.
        Node waterNode;
        Node reflectionCameraNode;


        private AvaloniaUrhoContext avaloniaContext;
       

		// private SampleAvaloniaWindow _window;
        [Preserve]
		public AvaloniaSample() : base(new ApplicationOptions(assetsFolder: "Data;CoreData"){ResizableWindow = true}) { }

		public AvaloniaSample(ApplicationOptions options):base(options){}

        protected override void Setup()
        {
            base.Setup();
        }

		protected override void Start ()
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
            {
                //CreateWindow(InitializeAvaloniaControlCatalogDemo);
                CreateWindow(InitializeTopHUD);
                //CreateWindow(InitializeRenderDemo);
            }
        }

        protected override void OnUpdate(float timeStep)
		{
			base.OnUpdate(timeStep);

            var reflectionCamera = reflectionCameraNode?.GetComponent<Camera>();
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
                SimpleMoveCamera3D(timeStep, 10.0f, overViewport2);
		}

        private void CreateWindow(Func<Avalonia.Controls.Window> createMethod)
        {
            var mainWindow = createMethod();

            mainWindow.Width = this.Graphics.Width / 2;
            mainWindow.Height = (this.Graphics.Height / 8);
            mainWindow.Position = new Avalonia.PixelPoint(0, 0);   // TMS

            // najak - Needed to make Window Default Background be Transparent, rather than solid-white!
            var avRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(mainWindow) as global::Avalonia.Controls.TopLevel;
            avRoot.TransparencyBackgroundFallback = Avalonia.Media.Brushes.Transparent;

            AvaloniaUrhoContext.MainWindow = mainWindow;
            mainWindow.Show(UI.Root);
        }

        Avalonia.Controls.Window InitializeTopHUD()
        {
            avaloniaContext = Context.ConfigureAvalonia<TopHUD.App>();
            avaloniaContext.RenderScaling = 2.0;
            var window = new TopHUD.MainWindow();

            //var panel = new AC.Panel();
            //panel.Parent = window;
            //window.Content = panel;



            return window;
        }
        //Avalonia.Controls.Window InitializeRenderDemo()
        //{
        //    avaloniaContext = Context.ConfigureAvalonia<RenderDemo.App>();
        //    avaloniaContext.RenderScaling = 2.0;
        //    return new RenderDemo.MainWindow();
        //}

        Avalonia.Controls.Window InitializeAvaloniaControlCatalogDemo()
		{
            avaloniaContext = Context.ConfigureAvalonia<ControlCatalog.App>();
            avaloniaContext.RenderScaling = 2.0;

            return new ControlCatalog.MainWindow();

            //// najak - Needed to make Window Default Background be Transparent, rather than solid-white!
            //var avRoot = Avalonia.VisualTree.VisualExtensions.GetVisualRoot(mainWindow) as global::Avalonia.Controls.TopLevel;
            //avRoot.TransparencyBackgroundFallback = Avalonia.Media.Brushes.Transparent;

            //var content = mainWindow.Content;

            //AvaloniaUrhoContext.MainWindow = mainWindow;
            //mainWindow.Show(UI.Root);

            ////mainWindow.Position = new Avalonia.PixelPoint(100, 100);
            //mainWindow.Position = new Avalonia.PixelPoint(0,0);   // TMS
            //// TBD whether one HUD over both viewports, or each viewport has own HUD.
            //if (false)//ShowTwoViewports)
            //{   // Left half-screen.
            //    var width0DI = mainWindow.Width;
            //    var width1DI = ToDIUnits(Graphics.Width);
            //    mainWindow.Width = (int)(width1DI / 2);
            //    mainWindow.Height = (int)ToDIUnits(Graphics.Height);
            //}
            //else
            //{   // Full screen.
            //    mainWindow.Width = (int)ToDIUnits(Graphics.Width);
            //    mainWindow.Height = (int)ToDIUnits(Graphics.Height);
            //}
        }

        /// <summary>
        /// Convert pixels to Avalonia Device-Independent Units.
        /// </summary>
        /// <param name="pixels"></param>
        /// <returns></returns>
        public float ToDIUnits(int pixels)
        {
            return pixels / (float)avaloniaContext.RenderScaling;
        }


        #region "-- scene specifics --"
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
            zone.FogColor = new Color(1.0f, 1.0f, 1.0f);
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

            // Create skybox. The Skybox component is used like StaticModel, but it will be always located at the camera, giving the
            // illusion of the box planes being far away. Use just the ordinary Box model and a suitable material, whose shader will
            // generate the necessary 3D texture coordinates for cube mapping
            var skyNode = scene.CreateChild("Sky");
            skyNode.SetScale(500.0f); // The scale actually does not matter
            var skybox = skyNode.CreateComponent<Skybox>();
            skybox.Model = cache.GetModel("Models/Box.mdl");
            skybox.SetMaterial(cache.GetMaterial("Materials/Skybox.xml"));

            // Create heightmap terrain
            var terrainNode = scene.CreateChild("Terrain");
            terrainNode.Position = new Vector3(0.0f, 0.0f, 0.0f);
            var terrain = terrainNode.CreateComponent<Terrain>();
            terrain.PatchSize = 64;
            terrain.Spacing = new Vector3(2.0f, 0.5f, 2.0f); // Spacing between vertices and vertical resolution of the height map
            terrain.Smoothing = true;
            terrain.SetHeightMap(cache.GetImage("Textures/HeightMap.png"));
            terrain.Material = cache.GetMaterial("Materials/Terrain.xml");
            // The terrain consists of large triangles, which fits well for occlusion rendering, as a hill can occlude all
            // terrain patches and other objects behind it
            terrain.Occluder = true;

            // Create 1000 boxes in the terrain. Always face outward along the terrain normal
            uint numObjects = 1000;
            for (uint i = 0; i < numObjects; ++i)
            {
                var objectNode = scene.CreateChild("Box");
                Vector3 position = new Vector3(NextRandom(2000.0f) - 1000.0f, 0.0f, NextRandom(2000.0f) - 1000.0f);
                position.Y = terrain.GetHeight(position) + 2.25f;
                objectNode.Position = position;
                // Create a rotation quaternion from up vector to terrain normal
                objectNode.Rotation = Quaternion.FromRotationTo(new Vector3(0.0f, 1.0f, 0.0f), terrain.GetNormal(position));
                objectNode.SetScale(5.0f);
                var obj = objectNode.CreateComponent<StaticModel>();
                obj.Model = cache.GetModel("Models/Box.mdl");
                obj.SetMaterial(cache.GetMaterial("Materials/Stone.xml"));
                obj.CastShadows = true;
            }

            // Create a water plane object that is as large as the terrain
            waterNode = scene.CreateChild("Water");
            waterNode.Scale = new Vector3(2048.0f, 1.0f, 2048.0f);
            waterNode.Position = new Vector3(0.0f, 5.0f, 0.0f);
            var water = waterNode.CreateComponent<StaticModel>();
            water.Model = cache.GetModel("Models/Plane.mdl");
            water.SetMaterial(cache.GetMaterial("Materials/Water.xml"));
            // Set a different viewmask on the water plane to be able to hide it from the reflection camera
            water.ViewMask = 0x80000000;

            WaterSceneMainCameraSettings(CameraPositionNode, Camera);
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


        #region "-- First camera and viewport --"
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

            Plane waterPlane = new Plane(waterNode.WorldRotation * new Vector3(0.0f, 1.0f, 0.0f), waterNode.WorldPosition);
            // Create a downward biased plane for reflection view clipping. Biasing is necessary to avoid too aggressive clipping
            Plane waterClipPlane = new Plane(waterNode.WorldRotation * new Vector3(0.0f, 1.0f, 0.0f), waterNode.WorldPosition - new Vector3(0.0f, 0.1f, 0.0f));

            // Create camera for water reflection
            // It will have the same farclip and position as the main viewport camera, but uses a reflection plane to modify
            // its position when rendering
            reflectionCameraNode = CameraNode.CreateChild();
            var reflectionCamera = reflectionCameraNode.CreateComponent<Camera>();
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
            // texture unit of the water material
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


        #region "-- Two viewports (and two cameras) --"
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

            // Clone the default render path so that we do not interfere with the other viewport, then add
            // bloom and FXAA post process effects to the front viewport. Render path commands can be tagged
            // for example with the effect name to allow easy toggling on and off. We start with the effects
            // disabled.
            var cache = ResourceCache;
            RenderPath effectRenderPath = viewport.RenderPath.Clone();
            effectRenderPath.Append(cache.GetXmlFile("PostProcess/Bloom.xml"));
            effectRenderPath.Append(cache.GetXmlFile("PostProcess/FXAA2.xml"));
            // Make the bloom mixing parameter more pronounced
            effectRenderPath.SetShaderParameter("BloomMix", new Vector2(0.9f, 0.6f));

            effectRenderPath.SetEnabled("Bloom", false);
            effectRenderPath.SetEnabled("FXAA2", false);
            viewport.RenderPath = effectRenderPath;

            // Set up the second camera viewport as left-hand pane (half of screen width).
            rect = RectBySize(0, 0, halfWidth, graphics.Height);
            Viewport2 = new Viewport(Context, Scene, CameraNode2.GetComponent<Camera>(), rect, null);

            renderer.SetViewport(1, Viewport2);
        }
        #endregion


        #region "-- Graphic Helpers (move to a static class) --"
        public static IntRect RectBySize(int xLeft, int yTop, int width, int height)
        {
            return new IntRect(xLeft, yTop, xLeft + width, yTop + height);
        }
        #endregion
    }
}
