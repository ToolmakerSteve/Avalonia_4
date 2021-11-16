// Copyright (c) 2020-2021 Eli Aloni (a.k.a  elix22)
// Copyright (c) 2008-2015 the Urho3D project.
// Copyright (c) 2015 Xamarin Inc
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Diagnostics;
using System.Globalization;
using Urho.Resources;
using Urho.Gui;
using Urho;

namespace AvaloniaSample
{
	public class Sample : Application
	{
		UrhoConsole console;
		DebugHud debugHud;
		ResourceCache cache;
		Sprite logoSprite;
		UI ui;

        protected const bool ShowTwoViewports = true;//true;   // TMS
        protected const bool GroundSpeedMultByAltitude = true;   // TMS - otherwise, when high up, camera move feels very slow.
        protected const bool MovementIgnoresPitch = true;   // Instead of following "nose" of camera, WASD are along ground plane.

        protected const float TouchSensitivity = 2;
		protected float Yaw { get; set; }
		protected float Pitch { get; set; }
		protected bool TouchEnabled { get; set; }

        protected Node CameraNode { get; set; }
        protected Node CameraNode2 { get; set; }
        /// <summary>
        /// Only used when two cameras (two viewports).
        /// So can move both cameras in world coords, instead of relative to camera orientation.
        /// </summary>
        protected Node CameraWorldBaseNode { get; set; }
        // When MovementIgnoresPitch, Camera1 WASD applied to this node.
        protected Node CameraYawNode;
        // Set to the node that is positioned by keys. When one camera, is CameraNode.
        // When two cameras, is CameraWorldBaseNode or CameraYawNode.
        protected Node CameraPositionNode;

        protected MonoDebugHud MonoDebugHud { get; set; }

		[Preserve]
		protected Sample(ApplicationOptions options = null) : base(options) {}

		static Sample()
		{
			Urho.Application.UnhandledException += Application_UnhandledException1;
		}

		static void Application_UnhandledException1(object sender, Urho.UnhandledExceptionEventArgs e)
		{
			if (Debugger.IsAttached && !e.Exception.Message.Contains("BlueHighway.ttf"))
				Debugger.Break();
			e.Handled = true;
		}

		protected bool IsLogoVisible
		{
			get { return logoSprite.Visible; }
			set { logoSprite.Visible = value; }
		}
		
		protected static readonly Random random = new Random(12345678);
		/// Return a random float between 0.0 (inclusive) and 1.0 (exclusive.)
		public static float NextRandom() { return (float)random.NextDouble(); }
		/// Return a random float between 0.0 and range, inclusive from both ends.
		public static float NextRandom(float range) { return (float)random.NextDouble() * range; }
		/// Return a random float between min and max, inclusive from both ends.
		public static float NextRandom(float min, float max) { return (float)((random.NextDouble() * (max - min)) + min); }
		/// Return a random integer between min and max - 1.
		public static int NextRandom(int min, int max) { return random.Next(min, max); }

		/// <summary>
		/// Joystick XML layout for mobile platforms
		/// </summary>
		protected virtual string JoystickLayoutPatch => string.Empty;

		protected override void Start ()
		{
            Urho.Avalonia.UrhoAvaloniaElement.SceneInputHandlerMethod = _HandleUserInput; // najak-HACK - to permit MouseInput to go through Avalonia transparencies.

			if (Platform != Platforms.Android)
            {
				// TBD elix22 ,  crashing on Android
                Log.LogMessage += e => Debug.WriteLine($"[{e.Level}] {e.Message}");
            }
			
			base.Start();
			if (Platform == Platforms.Android || 
				Platform == Platforms.iOS || 
				Options.TouchEmulation)
			{
				InitTouchInput();
			}
			Input.Enabled = true;
			MonoDebugHud = new MonoDebugHud(this);
			MonoDebugHud.Show();

			CreateLogo ();
			SetWindowAndTitleIcon ();
			CreateConsoleAndDebugHud ();
			Input.KeyDown += HandleKeyDown;
		}

		protected override void OnUpdate(float timeStep)
		{
			MoveCameraByTouches(timeStep);
			base.OnUpdate(timeStep);
		}

		/// <summary>
		/// Move camera for 2D samples
		/// </summary>
		protected void SimpleMoveCamera2D (float timeStep)
		{
			// Do not move if the UI has a focused element (the console)
			if (UI.FocusElement != null)
				return;

			// Movement speed as world units per second
			const float moveSpeed = 4.0f;

			// Read WASD keys and move the camera scene node to the corresponding direction if they are pressed
			if (Input.GetKeyDown(Key.W)) CameraNode.Translate( Vector3.UnitY * moveSpeed * timeStep);
			if (Input.GetKeyDown(Key.S)) CameraNode.Translate(-Vector3.UnitY * moveSpeed * timeStep);
			if (Input.GetKeyDown(Key.A)) CameraNode.Translate(-Vector3.UnitX * moveSpeed * timeStep);
			if (Input.GetKeyDown(Key.D)) CameraNode.Translate( Vector3.UnitX * moveSpeed * timeStep);

            if (Input.GetKeyDown(Key.PageUp))
			{
				Camera camera = CameraNode.GetComponent<Camera>();
				camera.Zoom = camera.Zoom * 1.01f;
			}

			if (Input.GetKeyDown(Key.PageDown))
			{
				Camera camera = CameraNode.GetComponent<Camera>();
				camera.Zoom = camera.Zoom * 0.99f;
			}
		}

        uint PreviousTime = 0;


        /// <summary>
        /// Move camera for 3D samples.
        /// </summary>
        /// <param name="timeStep"></param>
        /// <param name="moveSpeed"></param>
        /// <param name="overViewport2"></param>
        /// <returns>true if did move (a move key was down)</returns>
        protected bool SimpleMoveCamera3D(float timeStep, float moveSpeed = 10.0f, bool overViewport2 = false)
		{
            uint CurrentTime = Time.SystemTime;
            uint deltaTime = 0;
            if (PreviousTime != 0)
                deltaTime = CurrentTime - PreviousTime;
            // For next call. Don't reference PreviousTime after this; use deltaTime.
            PreviousTime = CurrentTime;

            bool didMove = false;

            // Might be multiple keys held down, so sum them.
            Vector3 cameraPlaneMove = Vector3.Zero;
            // najak-HACK - Let's Urho ALWAYS Handle Keyboard input  --- NOTE: We need another Hack for this to detect when Avalonia wants Exclusive Keyboard focus (e.g. TextBox, Chat, etc)
            if (Input.GetKeyDown(Key.W))
                cameraPlaneMove += Vector3.UnitZ;
            if (Input.GetKeyDown(Key.S))
                cameraPlaneMove -= Vector3.UnitZ;
            if (Input.GetKeyDown(Key.A))
                cameraPlaneMove -= Vector3.UnitX;
            if (Input.GetKeyDown(Key.D))
                cameraPlaneMove += Vector3.UnitX;

            Vector3? altitudeMove = null;
            // TBD: Intent is "X Down, Z Up". Might need to swap signs.
            if (Input.GetKeyDown(Key.X))
                altitudeMove = -Vector3.UnitY;
            else if (Input.GetKeyDown(Key.Z))
                altitudeMove = Vector3.UnitY;

            bool movingInPlane = cameraPlaneMove != Vector3.Zero;
            var allAxesMove = cameraPlaneMove;
            if (altitudeMove.HasValue)
                allAxesMove += altitudeMove.Value;

            if (movingInPlane || altitudeMove.HasValue)
            {
                didMove = true;
                var moveMult = moveSpeed * timeStep;
                if (GroundSpeedMultByAltitude)
                {
                    var altitude = CameraPositionNode.Position.Y;
                    if (overViewport2)
                        altitude += CameraNode2.Position.Y;
                    else
                        altitude += CameraNode.Position.Y;
                    float beginHighAltitude = overViewport2 ? 60 : 20;
                    if (altitude > beginHighAltitude)
                    {
                        // Move faster at high altitudes.
                        if (overViewport2)
                            moveMult *= altitude / beginHighAltitude;
                        else
                            moveMult *= (float)Math.Sqrt(altitude / beginHighAltitude);
                    }
                }
                //Debug.WriteLine($"--- deltaTime={deltaTime}, mult={moveMult}, elapsed={Time.ElapsedTime}, step={Time.TimeStep}, over2={overViewport2} ---");

                if (ShowTwoViewports && CameraPositionNode != null)
                {
                    if (overViewport2)
                    {
                        // This moves both cameras in world ground plane.
                        CameraPositionNode.Translate(cameraPlaneMove * moveMult);
                        if (altitudeMove.HasValue)
                        {
                            // Move camera2 in Altitude, which is Y above;
                            // but in CameraNode2's orientation (pointing down), it is "-Z".
                            altitudeMove = Vector3.UnitZ * -Math.Sign(altitudeMove.Value.Y);
                            CameraNode2.Translate(altitudeMove.Value * moveMult);
                        }
                    }
                    else
                    {
                        // Find the equivalent world-move of this camera-oriented move.
                        var worldPositionBefore = CameraNode.WorldPosition;
                        //CameraNode.Translate(allAxesMove * moveMult);
                        Node cameraPlaneNode = MovementIgnoresPitch ? CameraYawNode : CameraNode;
                        cameraPlaneNode.Translate(cameraPlaneMove * moveMult);
                        if (altitudeMove.HasValue)
                            CameraNode.Translate(altitudeMove.Value * moveMult);
                        var worldPositionAfter = CameraNode.WorldPosition;
                        // Undo the move. Will instead apply equivalent to CameraPositionNode.
                        //CameraNode.Translate(allAxesMove * -moveMult);
                        cameraPlaneNode.Translate(cameraPlaneMove * -moveMult);
                        if (altitudeMove.HasValue)
                            CameraNode.Translate(altitudeMove.Value * -moveMult);

                        var worldDelta = worldPositionAfter - worldPositionBefore;
                        //var altitudeAfter = worldPositionAfter.Y;
                        var altitudeDelta = worldDelta.Y;
                        //Debug.WriteLine($"--- {allAxesMove} -> {worldPositionAfter}, worldDelta {worldDelta}, altitude {altitudeAfter} ---");

                        // HACK: I don't know why the camera moves so slowly when change world position to world Translate.
                        moveMult *= 10;

                        // Move both cameras in world coords.
                        // TODO: Why so much slower than when apply directly to either CameraNode?
                        CameraPositionNode.Translate(worldDelta * moveMult);
                        // Cancel out Camera 2's altitude change. (convert Y change to -Z, then negate to +Z to compensate for change to CameraPositionNode.)
                        CameraNode2.Translate(new Vector3(0, 0, altitudeDelta * moveMult));
                    }
                }
                else
                {   // There is only one camera. Apply all axes to it.
                    CameraNode.Translate(allAxesMove * moveMult);
                }
            }


            if (UI.FocusElement != null)
                return didMove;
            else
                _HandleUserInput(timeStep, moveSpeed);
            return false;
        }

        private void _HandleUserInput()// najak-HACK - to permit MouseInput to go through Avalonia transparencies.
        {
            _HandleUserInput(0.02f, 10f);//najak-TODO - make the timeStep 'real'
        }
        private void _HandleUserInput(float timeStep, float moveSpeed)// najak-HACK - to permit MouseInput to go through Avalonia transparencies.
        {
            const float mouseSensitivity = .1f;

            if (Input.GetMouseButtonDown(MouseButton.Left))
            {
                var mouseMove = Input.MouseMove;
                Yaw += mouseSensitivity * mouseMove.X;
                Pitch += mouseSensitivity * mouseMove.Y;
                Pitch = MathHelper.Clamp(Pitch, -90, 90);

                ApplyPitchYawToCamera();
            }
        }

        private void ApplyPitchYawToCamera()
        {
            if (MovementIgnoresPitch)
            {
                CameraYawNode.Rotation = new Quaternion(0, Yaw, 0);
                CameraNode.Rotation = new Quaternion(Pitch, 0, 0);
            }
            else
                CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
        }

        protected void MoveCameraByTouches (float timeStep)
		{
			if (!TouchEnabled || CameraNode == null)
				return;

			var input = Input;
			for (uint i = 0, num = input.NumTouches; i < num; ++i)
			{
				TouchState state = input.GetTouch(i);
				if (state.TouchedElement != null)
					continue;

				if (state.Delta.X != 0 || state.Delta.Y != 0)
				{
					var camera = CameraNode.GetComponent<Camera>();
					if (camera == null)
						return;

					var graphics = Graphics;
					Yaw += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.X;
					Pitch += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.Y;
                    ApplyPitchYawToCamera();
                }
                else
				{
					var cursor = UI.Cursor;
					if (cursor != null && cursor.Visible)
						cursor.Position = state.Position;
				}
			}
		}

		protected void SimpleCreateInstructionsWithWasd (string extra = "")
		{
			SimpleCreateInstructions("Use WASD keys and mouse/touch to move" + extra);
		}
	
		protected void SimpleCreateInstructions(string text = "")
		{
			var textElement = new Text()
				{
					Value = text,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				};
			textElement.SetFont(ResourceCache.GetFont("Fonts/Anonymous Pro.ttf"), 15);
			UI.Root.AddChild(textElement);
		}

		void CreateLogo()
		{
			cache = ResourceCache;
			var logoTexture = cache.GetTexture2D("Textures/LogoLarge.png");

			if (logoTexture == null)
				return;

			ui = UI;
			logoSprite = ui.Root.CreateSprite();
			logoSprite.Texture = logoTexture;
			int w = logoTexture.Width;
			int h = logoTexture.Height;
			logoSprite.SetScale(256.0f / w);
			logoSprite.SetSize(w, h);
			logoSprite.SetHotSpot(0, h);
			logoSprite.SetAlignment(HorizontalAlignment.Left, VerticalAlignment.Bottom);
			logoSprite.Opacity = 0.75f;
			logoSprite.Priority = -100;
		}

		void SetWindowAndTitleIcon()
		{
			var icon = cache.GetImage("Textures/UrhoIcon.png");
			Graphics.SetWindowIcon(icon);
			Graphics.WindowTitle = "AvaloniaSample";
		}

		void CreateConsoleAndDebugHud()
		{
			var xml = cache.GetXmlFile("UI/DefaultStyle.xml");
			console = Engine.CreateConsole();
			console.DefaultStyle = xml;
			console.Background.Opacity = 0.8f;

			debugHud = Engine.CreateDebugHud();
			debugHud.DefaultStyle = xml;
		}

		void HandleKeyDown(KeyDownEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Esc:
					Exit();
					return;
			}

		}

		void InitTouchInput()
		{
			TouchEnabled = true;
			var layout = ResourceCache.GetXmlFile("UI/ScreenJoystick_Samples.xml");
			if (!string.IsNullOrEmpty(JoystickLayoutPatch))
			{
				XmlFile patchXmlFile = new XmlFile();
				patchXmlFile.FromString(JoystickLayoutPatch);
				layout.Patch(patchXmlFile);
			}
			var screenJoystickIndex = Input.AddScreenJoystick(layout, ResourceCache.GetXmlFile("UI/DefaultStyle.xml"));
			Input.SetScreenJoystickVisible(screenJoystickIndex, true);
		}
	}
}
