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
using Global;
using Urho;
using Urho.Gui;
using Urho.Resources;
using N = System.Numerics;
using OU;   // For Vector3Exts.
using U = OU.Utils;

namespace AvaloniaSample
{
    public class Sample : Application
	{
		UrhoConsole console;
		DebugHud debugHud;
		ResourceCache cache;
		Sprite logoSprite;
		UI ui;

        protected const bool ThirdPersonPerspective = true;
        protected const bool ShowTwoViewports = true;//true;   // TMS
        // Camera not only goes up as terrain rises, it also goes down as terrain falls.
        protected const bool TrackAltitude = true;
        // Shift key not down multiplies speed by this.
        protected const float NoShiftSpeedMult = 1;
        // Shift key down multiplies speed by this.
        protected const float ShiftSpeedMult = 3;
        protected const bool GroundSpeedMultByAltitude = true;   // TMS - otherwise, when high up, camera move feels very slow.
        protected const bool MovementIgnoresPitch = true;   // Instead of following "nose" of camera, WASD are along ground plane.
        protected const float MinimumAltitude1AboveTerrain = 1;
        protected const float MinimumCameraDistance = 3;
        protected const float SlowCameraDistance = 15;
        protected const float MinimumAltitude2AboveTerrain = 10;
        protected float CurrentMinimumAltitudeAboveTerrain => OverViewport2 ? MinimumAltitude2AboveTerrain : MinimumAltitude1AboveTerrain;
        protected float OtherMinimumAltitudeAboveTerrain => OverViewport2 ? MinimumAltitude1AboveTerrain : MinimumAltitude2AboveTerrain;
        // When ThirdPerson, want camera significantly farther away than the FirstPerson relAltitude.
        protected float _extraCameraDistance = 100f;


        protected const float TouchSensitivity = 2;
        // Degrees.
		protected float Yaw { get; set; }
		protected float Pitch { get; set; }
		protected bool TouchEnabled { get; set; }

		protected Camera Camera1;
		protected Camera Camera2;
		// Used by Water Scene.
		public Terrain Terrain;

        // Camera1 WASD applied to this node.
        // When !MovementIgnoresPitch, = Camera1FinalNode. 
        protected Node Camera1MainNode;
        // The camera is attached to this.
        protected Node Camera1FinalNode;
        // So "Translate" is not affected by the downward orientation of this camera.
        protected Node Camera2MainNode;
        // The camera is attached to this.
        protected Node Camera2FinalNode;
        // So WASD keys know which camera to affect.
        protected bool OverViewport2;
        // WASD keys applied to this node.
        protected Node CurrentCameraMainNode => OverViewport2 ? Camera2MainNode : Camera1MainNode;
        protected Node CurrentCameraFinalNode => OverViewport2 ? Camera2FinalNode : Camera1FinalNode;
        // After moving one camera, apply XZ to other camera.
        protected Node OtherCameraMainNode => OverViewport2 ? Camera1MainNode : Camera2MainNode;

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
			if (Input.GetKeyDown(Key.W)) Camera1FinalNode.Translate( Vector3.UnitY * moveSpeed * timeStep);
			if (Input.GetKeyDown(Key.S)) Camera1FinalNode.Translate(-Vector3.UnitY * moveSpeed * timeStep);
			if (Input.GetKeyDown(Key.A)) Camera1FinalNode.Translate(-Vector3.UnitX * moveSpeed * timeStep);
			if (Input.GetKeyDown(Key.D)) Camera1FinalNode.Translate( Vector3.UnitX * moveSpeed * timeStep);

            if (Input.GetKeyDown(Key.PageUp))
			{
				Camera camera = Camera1;
				camera.Zoom = camera.Zoom * 1.01f;
			}

			if (Input.GetKeyDown(Key.PageDown))
			{
				Camera camera = Camera1;
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
        protected bool MoveCamera3DFirstOrThirdPerson(float timeStep, float moveSpeed = 10.0f, bool overViewport2 = false)
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
                if (Input.GetKeyDown(Key.Shift))
                    moveMult *= ShiftSpeedMult;
                else
                    moveMult *= NoShiftSpeedMult;
                float terrainAltitude = Terrain.GetHeight(CurrentCameraMainNode.Position);
                if (GroundSpeedMultByAltitude)
                {
                    float sceneAltitude = CurrentCameraMainNode.Position.Altitude();
                    float relAltitude = sceneAltitude - terrainAltitude;
                    float beginHighAltitude = overViewport2 ? 60 : 20;
                    if (relAltitude > beginHighAltitude)
                    {
                        // Move faster at high altitudes.
                        if (overViewport2)
                            moveMult *= relAltitude / beginHighAltitude;
                        else
                            moveMult *= (float)Math.Sqrt(relAltitude / beginHighAltitude);
                    }
                }
                //Debug.WriteLine($"--- deltaTime={deltaTime}, mult={moveMult}, elapsed={Time.ElapsedTime}, step={Time.TimeStep}, over2={overViewport2} ---");

                bool enforceMaxAltitude = TrackAltitude && !altitudeMove.HasValue;
                float currentMaxRelAltitude = enforceMaxAltitude ?
                        CurrentCameraMainNode.Position.Altitude() - terrainAltitude :
                        float.MaxValue;
                float otherMaxAltitude = TrackAltitude ?
                        OtherCameraMainNode.Position.Altitude() - terrainAltitude :
                        float.MaxValue;

                // Move current camera.
                if (OverViewport2)
                    CurrentCameraMainNode.Translate(allAxesMove * moveMult);
                else
                {
                    CurrentCameraMainNode.Translate(cameraPlaneMove * moveMult);
                    if (altitudeMove.HasValue)
                    {
                        var deltaAltitude = altitudeMove.Value.Altitude() * moveMult;
                        ApplyDeltaAltitudeToCameraDistance(deltaAltitude);
                    }
                }
                EnforceMinimumAltitudeAboveTerrain(CurrentCameraMainNode, CurrentMinimumAltitudeAboveTerrain, currentMaxRelAltitude);

                if (ShowTwoViewports)
                {
                    CopyXZ(CurrentCameraMainNode, OtherCameraMainNode);
                    EnforceMinimumAltitudeAboveTerrain(OtherCameraMainNode, OtherMinimumAltitudeAboveTerrain, otherMaxAltitude);
                }

                MaybeApplyThirdPersonPerspective();
            }


            // TMS TODO: Should either of these sometimes intercept mouse button?
            if (UI.FocusElement != null)
                return didMove;
            else
                _HandleUserInput(timeStep, moveSpeed);
            return didMove;
        }

        private void ApplyDeltaAltitudeToCameraDistance(float deltaAltitude)
        {
            if (_extraCameraDistance < SlowCameraDistance)
                deltaAltitude *= 0.333f;

            _extraCameraDistance += deltaAltitude;
            EnsureMinimumCameraDistance();
        }

        protected void MaybeApplyThirdPersonPerspective()
        {
            if (ThirdPersonPerspective)
            {   // Camera1 - show Third Person perspective.
                // TEST: When ONLY Yaw, looks like its trying to maintain focus at the center.
                // EITHER parameter by itself works well. There is just some mistake when do both.
                //Pitch = 0;   // ttttt

                // "LookAt" Camera1Main.WorldPosition, projected to Terrain.
                Vector3 lookAt_World = Camera1MainNode.WorldPosition;
                float mainAltitude = lookAt_World.Altitude();
                float terrainAltitude = Terrain.GetHeight(lookAt_World);
                lookAt_World.SetAltitude(terrainAltitude);
                
                // Move camera1Final away from that point, oriented according to pitch/yaw.
                // IDEA: Fake it. We've already oriented the camera;
                // figure out the appropriate position, such that the result is looking at what we want.
                Vector3 relPosition = CalcCameraRelativePosition(mainAltitude - terrainAltitude + _extraCameraDistance);
                var camera_World = lookAt_World + relPosition;
                // TODO WRONG: Not taking into account parent's (Yaw) rotation.
                //var camera_RelParent = camera_World - Camera1MainNode.WorldPosition;
                var camera_RelParent = Camera1MainNode.WorldToLocal(camera_World);

                Camera1FinalNode.Position = camera_RelParent;
                //var camera_World_verify = Camera1FinalNode.WorldPosition;
                //if (!camera_World_verify.NearlyEquals3(camera_World, 0.0001f))
                //    U.Trouble();

                if (false)   //ttttt
                {
                    var Y = U.Round3(Yaw);
                    var P = U.Round3(Pitch);
                    var M = U.Round3(Camera1MainNode.WorldPosition);
                    var L = U.Round3(lookAt_World);
                    var R = U.Round3(relPosition);
                    var CW = U.Round3(camera_World);
                    var CP = U.Round3(camera_RelParent);
                    U.DebugWriteLineIfChange($"--- Y={Y}, P={P}, M={M}, L={L}, R={R}, CW={CW}, CP={CP} ---");
                }
            }
        }

        /// <summary>
        /// Project world position into parent's space.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="pt_world"></param>
        /// <returns></returns>
        //protected Vector3 ProjectToParentSpace(Node parent, Vector3 pt_world)
        //{
        //    // 
        //    Matrix3x4 parentFullMat = parent.WorldTransform;
        //    // 3x4 harder to work with; omit translation for our purposes.
        //    Matrix3 parentFullRotate = parentFullMat.ToMatrix3();

        //    //MISSING Matrix3 inversePFR = parentFullRotate.Inverse();
        //    Matrix3 inversePFR = parentFullMat.Inverse().ToMatrix3();

        //    Quaternion parentFullQ = parent.WorldRotation;
        //    Quaternion inversePFQ0 = Quaternion.Invert(parentFullQ);

        //    N.Quaternion pfq = NQuaternionFrom(parentFullQ);
        //    N.Quaternion inversePFQ1 = NQuaternionFrom(inversePFQ0);
        //    N.Quaternion inversePFQ2 = N.Quaternion.Inverse(pfq);
        //    N.Matrix4x4 pfm = N.Matrix4x4.CreateFromQuaternion(pfq);
        //}

        /// <summary>
        /// In world space.
        /// </summary>
        /// <param name="cameraDistance"></param>
        /// <returns></returns>
        private Vector3 CalcCameraRelativePosition(float cameraDistance)
        {
            Vector3 heading = HeadingPitchThenYaw();

            // "-": "pull back" from the look-at, so inverse direction.
            Vector3 relPosition = -cameraDistance * heading;
            return relPosition;
        }

        //private Vector3 HeadingPitchThenYaw()
        //{
        //    // N has needed methods.
        //    N.Vector3 heading = N.Vector3.UnitZ;
        //    heading = N.Vector3.Transform(heading, NQuaternionFrom(PitchQuaternion()));
        //    heading = N.Vector3.Transform(heading, NQuaternionFrom(YawQuaternion()));
        //    return new Vector3(heading.X, heading.Y, heading.Z);
        //}

        private Vector3 HeadingPitchThenYaw()
        {
            Vector3 heading = Vector3.UnitZ;
            Vector4 vec4 = Vector3.Transform(heading, Matrix4.Rotate(PitchQuaternion()));
            vec4 = Vector4.Transform(vec4, Matrix4.Rotate(YawQuaternion()));
            return new Vector3(vec4.X, vec4.Y, vec4.Z);
        }

        private static N.Quaternion NQuaternionFrom(Quaternion q)
        {
            return new N.Quaternion(q.X, q.Y, q.Z, q.W);
        }

        protected void EnforceMinimumAltitudeAboveTerrain(Node cameraMainNode, float minimumRelativeAltitude, float maxRelativeAltitude = float.MaxValue)
        {
            if (minimumRelativeAltitude > 0)
            {
                float sceneAltitude = cameraMainNode.Position.Altitude();
                float terrainAltitude = Terrain.GetHeight(cameraMainNode.Position);
                float relAltitude = sceneAltitude - terrainAltitude;
                float excess = relAltitude - minimumRelativeAltitude;
                if (excess < 0)
                {
                    // Below what we need. "-" to add the missing altitude.
                    var missing = -excess;
                    sceneAltitude += missing;
                    //relAltitude += missing;
                    cameraMainNode.Position = U.WithAltitude(cameraMainNode.Position, sceneAltitude);
                    MaybeShortenCameraDistance(cameraMainNode, missing);
                }
                else if (maxRelativeAltitude < float.MaxValue && maxRelativeAltitude > minimumRelativeAltitude)
                {
                    if (relAltitude > maxRelativeAltitude)
                    {
                        sceneAltitude = terrainAltitude + maxRelativeAltitude;
                        cameraMainNode.Position = U.WithAltitude(cameraMainNode.Position, sceneAltitude);
                    }
                }
            }
        }

        private void MaybeShortenCameraDistance(Node cameraMainNode, float missing)
        {
            if (ThirdPersonPerspective && ReferenceEquals(cameraMainNode, Camera1MainNode))
            {
                // We stopped LookAt from moving down this much.
                // Instead, move camera closer (zoom in).
                // Zoom slower when close.
                ApplyDeltaAltitudeToCameraDistance(-missing);
            }
        }

        private void EnsureMinimumCameraDistance()
        {
            if (_extraCameraDistance < MinimumCameraDistance)
                _extraCameraDistance = MinimumCameraDistance;
        }

        /// <summary>
        /// TBD: Move to some Urho Utilities class.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dst"></param>
        protected void CopyXZ(Node src, Node dst)
        {
            dst.Position = U.CopyXZ(src.Position, dst.Position);
        }

        protected void _HandleUserInput()// najak-HACK - to permit MouseInput to go through Avalonia transparencies.
        {
            _HandleUserInput(0.02f, 10f);//najak-TODO - make the timeStep 'real'
        }
        protected void _HandleUserInput(float timeStep, float moveSpeed)// najak-HACK - to permit MouseInput to go through Avalonia transparencies.
        {
            const float mouseSensitivity = .1f;

            if (Input.GetMouseButtonDown(MouseButton.Right))
            {
                var mouseMove = Input.MouseMove;
                Yaw += mouseSensitivity * mouseMove.X;
                Pitch += mouseSensitivity * mouseMove.Y;
                Pitch = MathHelper.Clamp(Pitch, -90, 90);

                ApplyPitchYawToCamera();
                MaybeApplyThirdPersonPerspective();
            }
        }

        protected void ApplyPitchYawToCamera()
        {
            if (MovementIgnoresPitch)
            {
                Camera1MainNode.Rotation = YawQuaternion();
                Camera1FinalNode.Rotation = PitchQuaternion();
            }
            else
                Camera1FinalNode.Rotation = PitchYawQuaternion();
        }

        protected Quaternion YawQuaternion()
        {
            return new Quaternion(0, Yaw, 0);
        }

        protected Quaternion PitchQuaternion()
        {
            return new Quaternion(Pitch, 0, 0);
        }

        protected Quaternion PitchYawQuaternion()
        {
            return new Quaternion(Pitch, Yaw, 0);
        }

        protected void MoveCameraByTouches (float timeStep)
		{
			if (!TouchEnabled || Camera1FinalNode == null)
				return;

			var input = Input;
			for (uint i = 0, num = input.NumTouches; i < num; ++i)
			{
				TouchState state = input.GetTouch(i);
				if (state.TouchedElement != null)
					continue;

				if (state.Delta.X != 0 || state.Delta.Y != 0)
				{
					var camera = Camera1;
					if (camera == null)
						return;

					var graphics = Graphics;
					Yaw += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.X;
					Pitch += TouchSensitivity * camera.Fov / graphics.Height * state.Delta.Y;
                    ApplyPitchYawToCamera();
                    MaybeApplyThirdPersonPerspective();
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
			SimpleCreateInstructions("WASD=move, RIGHT mouse/touch=rotate" + extra);
		}
	
		protected void SimpleCreateInstructions(string text = "")
		{
			var textElement = new Text()
				{
					Value = text,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Top
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
