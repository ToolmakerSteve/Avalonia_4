using Urho;
using System;
using Urho.Gui;
using Urho.Avalonia;
using Urho.IO;
using Avalonia.Styling;
using Avalonia.Markup.Xaml.Styling;

namespace AvaloniaSample
{
	public class AvaloniaSample : Sample
	{


		Camera Camera = null;
		Scene Scene;
        Viewport Viewport2;

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
            CreateScene();
            SimpleCreateInstructionsWithWasd();
            if (ShowTwoViewports)
                SetupTwoViewports();
            else
                SetupOneViewport();


            UI.Root.SetDefaultStyle(ResourceCache.GetXmlFile("UI/DefaultStyle.xml"));
			
			Input.SetMouseVisible(true);
			Input.SetMouseMode(MouseMode.Free);

            InitializeAvaloniaSample();
			
		
		}

        void InitializeAvaloniaSample()
        {
            // InitializeAvaloniaTodoDemo();
            // InitializeAvaloniaDockDemo();
            // InitializeAvaloniaDockDemo2();
            //  InitializeAvaloniaNotePadDemo();

            InitializeAvaloniaControlCatalogDemo();
        }


		protected override void OnUpdate(float timeStep)
		{
			base.OnUpdate(timeStep);

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

		void InitializeAvaloniaControlCatalogDemo()
		{
            avaloniaContext = Context.ConfigureAvalonia<ControlCatalog.App>();
            avaloniaContext.RenderScaling = 2.0;

            var mainWindow = new ControlCatalog.MainWindow();
            AvaloniaUrhoContext.MainWindow = mainWindow;
            mainWindow.Show(UI.Root);

            //mainWindow.Position = new Avalonia.PixelPoint(100, 100);
            mainWindow.Position = new Avalonia.PixelPoint(0,0);   // TMS
            // TBD whether one HUD over both viewports, or each viewport has own HUD.
            if (false)//ShowTwoViewports)
            {   // Left half-screen.
                var width0DI = mainWindow.Width;
                var width1DI = ToDIUnits(Graphics.Width);
                mainWindow.Width = (int)(width1DI / 2);
                mainWindow.Height = (int)ToDIUnits(Graphics.Height);
            }
            else
            {   // Full screen.
                mainWindow.Width = (int)ToDIUnits(Graphics.Width);
                mainWindow.Height = (int)ToDIUnits(Graphics.Height);
            }
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

        void CreateScene ()
        {
            Scene = new Scene();

            // Create the Octree component to the scene. This is required before adding any drawable components, or else nothing will
            // show up. The default octree volume will be from (-1000, -1000, -1000) to (1000, 1000, 1000) in world coordinates; it
            // is also legal to place objects outside the volume but their visibility can then not be checked in a hierarchically
            // optimizing manner
            Scene.CreateComponent<Octree>();


            // Create a child scene node (at world origin) and a StaticModel component into it. Set the StaticModel to show a simple
            // plane mesh with a "stone" material. Note that naming the scene nodes is optional. Scale the scene node larger
            // (100 x 100 world units)
            var planeNode = Scene.CreateChild("Plane");
            planeNode.Scale = new Vector3(100, 1, 100);
            var planeObject = planeNode.CreateComponent<StaticModel>();
            planeObject.Model = ResourceCache.GetModel("Models/Plane.mdl");
            planeObject.SetMaterial(ResourceCache.GetMaterial("Materials/StoneTiled.xml"));


            // Create a directional light to the world so that we can see something. The light scene node's orientation controls the
            // light direction; we will use the SetDirection() function which calculates the orientation from a forward direction vector.
            // The light will use default settings (white light, no shadows)
            var lightNode = Scene.CreateChild("DirectionalLight");
            lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f)); // The direction vector does not need to be normalized
            var light = lightNode.CreateComponent<Light>();
            light.LightType = LightType.Directional;


            // Create skybox. The Skybox component is used like StaticModel, but it will be always located at the camera, giving the
            // illusion of the box planes being far away. Use just the ordinary Box model and a suitable material, whose shader will
            // generate the necessary 3D texture coordinates for cube mapping
            var skyNode = Scene.CreateChild("Sky");
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
            var rand = new Random();
            const int NMushrooms = 20; //200
            for (int i = 0; i < NMushrooms; i++)
            {
                var mushroom = Scene.CreateChild("Mushroom");
                mushroom.Position = new Vector3(rand.Next(90) - 45, 0, rand.Next(90) - 45);
                mushroom.Rotation = new Quaternion(0, rand.Next(360), 0);
                mushroom.SetScale(0.5f + rand.Next(20000) / 10000.0f);
                var mushroomObject = mushroom.CreateComponent<StaticModel>();
                mushroomObject.Model = ResourceCache.GetModel("Models/Mushroom.mdl");
                mushroomObject.SetMaterial(ResourceCache.GetMaterial("Materials/Mushroom.xml"));
            }

            SetupCameras();
        }


        #region "-- First camera and viewport --"
        private void SetupCameras()
        {

            Node parent;
            if (ShowTwoViewports)
            {
                CameraWorldBaseNode = Scene.CreateChild("cameraBase");

                parent = CameraWorldBaseNode;
                // This node gets the position.
                CameraPositionNode = CameraWorldBaseNode;
            }
            else
            {   // One camera - create it directly in the scene.
                parent = Scene;
                CameraPositionNode = CameraNode;
            }

            // Set an initial position (for the camera node(s)) above the plane
            CameraPositionNode.Position = new Vector3(0, 5, 0);

            // Create a scene node for the camera, which we will move around
            // The camera will use default settings (1000 far clip distance, 45 degrees FOV, set aspect ratio automatically)
            CameraNode = parent.CreateChild("camera");
            Camera = CameraNode.CreateComponent<Camera>();

            if (ShowTwoViewports)
            {
                SetupSecondCamera();
            }
        }

        void SetupOneViewport()
        {
            // Set up a viewport to the Renderer subsystem so that the 3D scene can be seen. We need to define the scene and the camera
            // at minimum. Additionally we could configure the viewport screen size and the rendering path (eg. forward / deferred) to
            // use, but now we just use full screen and default render path configured in the engine command line options
            Renderer.SetViewport(0, new Viewport(Context, Scene, Camera, null));
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
