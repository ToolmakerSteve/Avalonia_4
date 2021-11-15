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

        protected const bool ShowTwoViewports = true;//true;

        protected const float TouchSensitivity = 2;
		protected float Yaw { get; set; }
		protected float Pitch { get; set; }
		protected bool TouchEnabled { get; set; }
        /// <summary>
        /// Only used when two cameras (two viewports).
        /// So can move both cameras in world coords, instead of relative to camera orientation.
        /// </summary>
        protected Node CameraWorldBaseNode { get; set; }
        // Set to the node that is positioned by keys. When one camera, is CameraNode. When two cameras, is CameraWorldBaseNode.
        protected Node CameraPositionNode;
        protected Node CameraNode { get; set; }
        protected Node CameraNode2 { get; set; }

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
		
		static readonly Random random = new Random();
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

		/// <summary>
		/// Move camera for 3D samples
		/// </summary>
		protected void SimpleMoveCamera3D (float timeStep, float moveSpeed = 10.0f)
		{
            Vector3 unitMove = Vector3.Zero;
            // najak-HACK - Let's Urho ALWAYS Handle Keyboard input  --- NOTE: We need another Hack for this to detect when Avalonia wants Exclusive Keyboard focus (e.g. TextBox, Chat, etc)
            if (Input.GetKeyDown(Key.W))
                unitMove += Vector3.UnitZ;
            if (Input.GetKeyDown(Key.S))
                unitMove -= Vector3.UnitZ;
            if (Input.GetKeyDown(Key.A))
                unitMove -= Vector3.UnitX;
            if (Input.GetKeyDown(Key.D))
                unitMove += Vector3.UnitX;
            // TBD: Intent is "X Down, Z Up". Might need to swap signs.
            if (Input.GetKeyDown(Key.X))
                unitMove -= Vector3.UnitY;
            if (Input.GetKeyDown(Key.Z))
                unitMove += Vector3.UnitY;

            if (unitMove != Vector3.Zero)
            {
                CameraPositionNode?.Translate(unitMove * moveSpeed * timeStep);
                if (CameraPositionNode == null)
                    CameraNode.Translate(unitMove * moveSpeed * timeStep);
            }


            if (UI.FocusElement != null)
                return;
            else
                _HandleUserInput(timeStep, moveSpeed);
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

                CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
            }
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
					CameraNode.Rotation = new Quaternion(Pitch, Yaw, 0);
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
