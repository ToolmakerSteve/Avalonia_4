-- Static 3D scene example.
-- This sample demonstrates:
--     - Creating a 3D scene with static content
--     - Displaying the scene using the Renderer subsystem
--     - Handling keyboard and mouse input to move a freelook camera

require "LuaScripts/Utilities/Sample"

function Start()
    -- Execute the common startup for samples
    SampleStart()

    -- Create the scene content
    CreateScene()

    -- Create the UI content
   -- CreateInstructions()

    -- Setup the viewport for displaying the scene
    SetupViewport()

    -- Hook up to the frame update events
    SubscribeToEvents()
end
local planeObject = nil
function CreateScene()
    scene_ = Scene()

    -- Create the Octree component to the scene. This is required before adding any drawable components, or else nothing will
    -- show up. The default octree volume will be from (-1000, -1000, -1000) to (1000, 1000, 1000) in world coordinates it
    -- is also legal to place objects outside the volume but their visibility can then not be checked in a hierarchically
    -- optimizing manner
    scene_:CreateComponent("Octree")

    -- Create a child scene node (at world origin) and a StaticModel component into it. Set the StaticModel to show a simple
    -- plane mesh with a "stone" material. Note that naming the scene nodes is optional. Scale the scene node larger
    -- (100 x 100 world units)
    local planeNode = scene_:CreateChild("Plane")
    planeNode.scale = Vector3(100.0, 1.0, 100.0)
	
    planeObject = planeNode:CreateComponent("StaticModel")
    planeObject.model = cache:GetResource("Model", "Models/TiledPlane.mdl")
    planeObject.material = cache:GetResource("Material", "Materials/ParallaxStonesDemoVer.xml")
	
	local monkeyNode = scene_:CreateChild("Monkey")
    monkeyNode.scale = Vector3(10.0, 10.0, 10.0)
	monkeyNode.position = Vector3(0, 5, 20)
	
    local monkeyObject = monkeyNode:CreateComponent("StaticModel")
    monkeyObject.model = cache:GetResource("Model", "Models/Suzanne.mdl")
    monkeyObject.material = cache:GetResource("Material", "Materials/ParallaxStonesDemoVer.xml")

    -- Create a directional light to the world so that we can see something. The light scene node's orientation controls the
    -- light direction we will use the SetDirection() function which calculates the orientation from a forward direction vector.
    -- The light will use default settings (white light, no shadows)
    local lightNode = scene_:CreateChild("DirectionalLight")
    lightNode.direction = Vector3(0.6, -1.0, 0.8) -- The direction vector does not need to be normalized
    local light = lightNode:CreateComponent("Light")
    light.lightType = LIGHT_DIRECTIONAL

    -- Create more StaticModel objects to the scene, randomly positioned, rotated and scaled. For rotation, we construct a
    -- quaternion from Euler angles where the Y angle (rotation about the Y axis) is randomized. The mushroom model contains
    -- LOD levels, so the StaticModel component will automatically select the LOD level according to the view distance (you'll
    -- see the model get simpler as it moves further away). Finally, rendering a large number of the same object with the
    -- same material allows instancing to be used, if the GPU supports it. This reduces the amount of CPU work in rendering the
    -- scene.
   

    -- Create a scene node for the camera, which we will move around
    -- The camera will use default settings (1000 far clip distance, 45 degrees FOV, set aspect ratio automatically)
    cameraNode = scene_:CreateChild("Camera")
    cameraNode:CreateComponent("Camera")

    -- Set an initial position for the camera scene node above the plane
    cameraNode.position = Vector3(0.0, 5.0, 0.0)
end

function CreateInstructions()
    -- Construct new Text object, set string to display and font to use
    local instructionText = ui.root:CreateChild("Text")
    instructionText:SetText("Use WASD keys and mouse to move")
    instructionText:SetFont(cache:GetResource("Font", "Fonts/Anonymous Pro.ttf"), 15)

    -- Position the text relative to the screen center
    instructionText.horizontalAlignment = HA_CENTER
    instructionText.verticalAlignment = VA_CENTER
    instructionText:SetPosition(0, ui.root.height / 4)
end

function SetupViewport()
    -- Set up a viewport to the Renderer subsystem so that the 3D scene can be seen. We need to define the scene and the camera
    -- at minimum. Additionally we could configure the viewport screen size and the rendering path (eg. forward / deferred) to
    -- use, but now we just use full screen and default render path configured in the engine command line options
    local viewport = Viewport:new(scene_, cameraNode:GetComponent("Camera"))
    renderer:SetViewport(0, viewport)
end

function MoveCamera(timeStep)
    -- Do not move if the UI has a focused element (the console)
    if ui.focusElement ~= nil then
        return
    end

    -- Movement speed as world units per second
    local MOVE_SPEED = 20.0
    -- Mouse sensitivity as degrees per pixel
    local MOUSE_SENSITIVITY = 0.1

    -- Use this frame's mouse motion to adjust camera node yaw and pitch. Clamp the pitch between -90 and 90 degrees
    local mouseMove = input.mouseMove
    yaw = yaw +MOUSE_SENSITIVITY * mouseMove.x
    pitch = pitch + MOUSE_SENSITIVITY * mouseMove.y
    pitch = Clamp(pitch, -90.0, 90.0)

    -- Construct new orientation for the camera scene node from yaw and pitch. Roll is fixed to zero
    cameraNode.rotation = Quaternion(pitch, yaw, 0.0)

    -- Read WASD keys and move the camera scene node to the corresponding direction if they are pressed
    -- Use the Translate() function (default local space) to move relative to the node's orientation.
    if input:GetKeyDown(KEY_W) then
        cameraNode:Translate(Vector3(0.0, 0.0, 1.0) * MOVE_SPEED * timeStep)
    end
    if input:GetKeyDown(KEY_S) then
        cameraNode:Translate(Vector3(0.0, 0.0, -1.0) * MOVE_SPEED * timeStep)
    end
    if input:GetKeyDown(KEY_A) then
        cameraNode:Translate(Vector3(-1.0, 0.0, 0.0) * MOVE_SPEED * timeStep)
    end
    if input:GetKeyDown(KEY_D) then
        cameraNode:Translate(Vector3(1.0, 0.0, 0.0) * MOVE_SPEED * timeStep)
    end
end
local numIters = 2
local displacement = .075
local offsetlimit = .5
local parallaxOn = true
function ChangeSettings(timeStep)
	if input:GetKeyPress(KEY_SHIFT) then
		numIters = 2
		displacement = .075
		offsetlimit = .5
	end
	if input:GetKeyPress(KEY_CTRL) then
		numIters = 5
		displacement = .105
		offsetlimit = .35
	end

	if input:GetKeyPress(KEY_I) then
		numIters = numIters + 1
	end
	planeObject.material:SetShaderParameter("ParallaxIters", Variant(numIters))
	if input:GetKeyPress(KEY_O) then
		numIters = numIters - 1
		if numIters < 0 then numIters = 0 end
	end
	planeObject.material:SetShaderParameter("ParallaxIters", Variant(numIters))
	if input:GetKeyDown(KEY_J) then
		displacement = displacement + .025 * timeStep
	end
	if input:GetKeyDown(KEY_K) then
		displacement = displacement - .025 * timeStep
		if displacement < 0 then displacement = 0 end
	end
	if input:GetKeyPress(KEY_SPACE) then parallaxOn = not parallaxOn end
	if parallaxOn then
		planeObject.material:SetShaderParameter("ParallaxDisplacement", Variant(displacement))
	else
		planeObject.material:SetShaderParameter("ParallaxDisplacement", Variant(0))
	end
	
	if input:GetKeyDown(KEY_N) then
		offsetlimit = offsetlimit + .2 * timeStep
		if offsetlimit > 1 then offsetlimit = 1 end
	end
	if input:GetKeyDown(KEY_M) then
		offsetlimit = offsetlimit - .2 * timeStep
	end
	planeObject.material:SetShaderParameter("OffsetLimit", Variant(offsetlimit))
end

function SubscribeToEvents()
    -- Subscribe HandleUpdate() function for processing update events
    SubscribeToEvent("Update", "HandleUpdate")
end

function HandleUpdate(eventType, eventData)
    -- Take the frame time step, which is stored as a float
    local timeStep = eventData:GetFloat("TimeStep")

    -- Move the camera, scale movement with time step
    MoveCamera(timeStep)
	
	ChangeSettings(timeStep)
end
