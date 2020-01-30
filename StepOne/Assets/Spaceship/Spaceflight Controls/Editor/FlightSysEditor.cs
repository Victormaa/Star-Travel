using UnityEngine;
using System.Collections;
using UnityEditor;


[CustomEditor(typeof(PlayerFlightControl))]
public class FlightControllerEditor : Editor 
{
	public override void OnInspectorGUI()
	{

		PlayerFlightControl flightCC = (PlayerFlightControl)target;
		
		EditorGUILayout.Separator();
		GUILayout.Label(new GUIContent("Objects", "For the main ship Game Object and weapons"));
		
		flightCC.actual_model = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Ship Game Object", "Point this to the Game Object that actually contains the mesh for the ship. Generally, this is the first child of the empty container object this controller is placed in."), flightCC.actual_model, typeof(GameObject), true);
		flightCC.weapon_hardpoint_1 = (Transform) EditorGUILayout.ObjectField(new GUIContent("Weapon Hardpoint", "Transform for the barrel of the weapon"), flightCC.weapon_hardpoint_1, typeof(Transform), true);
		flightCC.bullet = (GameObject) EditorGUILayout.ObjectField(new GUIContent("Projectile Game Object", "Projectile that will be fired from the weapon hardpoint."), flightCC.bullet, typeof(GameObject), true);
        flightCC.LandPoint = (Transform)EditorGUILayout.ObjectField(new GUIContent("Player get off the Plane Point"), flightCC.LandPoint, typeof(Transform), true);

		//new GUIContent("", "")
		EditorGUILayout.Separator();
		GUILayout.Label(new GUIContent("Core Movement", "Controls for the various speeds for different operations."));
		
		flightCC.speed = EditorGUILayout.FloatField(new GUIContent("Base Speed", "Primary flight speed, without afterburners or brakes"), flightCC.speed);	
		flightCC.slow_speed = EditorGUILayout.FloatField(new GUIContent("Brake Speed", "Speed when the button for negative thrust is being held down"), flightCC.slow_speed);	
		flightCC.afterburner_speed = EditorGUILayout.FloatField(new GUIContent("Afterburner Speed", "Speed when the button for positive thrust is being held down"), flightCC.afterburner_speed);
		flightCC.thrust_transition_speed = EditorGUILayout.FloatField(new GUIContent("Thrust Transition Speed", "How quickly afterburners/brakes will reach their maximum effect"), flightCC.thrust_transition_speed);			
		flightCC.turnspeed = EditorGUILayout.FloatField(new GUIContent("Turn/Roll Speed", "How fast turns and rolls will be executed"), flightCC.turnspeed);
		flightCC.rollSpeedModifier = EditorGUILayout.FloatField(new GUIContent("Roll Speed", "Multiplier for roll speed. Base roll is determined by turn speed"), flightCC.rollSpeedModifier);				
		flightCC.pitchYaw_strength = EditorGUILayout.FloatField(new GUIContent("Pitch/Yaw Multiplier", "Controls the intensity of pitch and yaw inputs"), flightCC.pitchYaw_strength);			
		
		EditorGUILayout.Separator();

		GUILayout.Label(new GUIContent("Banking", "Visuals only--has no effect on actual movement"));


		flightCC.use_banking = EditorGUILayout.BeginToggleGroup(new GUIContent("Use Banking", "The ship will bank when doing turns."), flightCC.use_banking);			
		
		flightCC.bank_rotation_speed = EditorGUILayout.FloatField(new GUIContent("Bank Rotation Speed", "Rotation speed along the Z axis when yaw is applied. Higher values will result in snappier banking."), flightCC.bank_rotation_speed);			
		flightCC.bank_rotation_multiplier = EditorGUILayout.FloatField(new GUIContent("Bank Rotation Multiplier", "Bank amount along the Z axis when yaw is applied."), flightCC.bank_rotation_multiplier);			
		flightCC.bank_angle_clamp = EditorGUILayout.FloatField(new GUIContent("Bank Angle Clamp", "Maximum angle the spacecraft can rotate along the Z axis."), flightCC.bank_angle_clamp);			
		EditorGUILayout.EndToggleGroup();
		
		
		EditorGUILayout.Separator();
		
		flightCC.screen_clamp = EditorGUILayout.FloatField(new GUIContent("Screen Clamp (Pixels)", "Once the pointer is more than this many pixels from the center, the input in that direction(s) will be treated as the maximum value."), flightCC.screen_clamp);			
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
}



[CustomEditor(typeof(CameraFlightFollow))]
public class FlightCameraEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		CameraFlightFollow cam = (CameraFlightFollow)target;
	
	
		EditorGUILayout.Separator();
		cam.control = (PlayerFlightControl) EditorGUILayout.ObjectField(new GUIContent("Flight Controller", "Drag the container object that has the Player Flight Controller here."), cam.control, typeof(PlayerFlightControl), true);
		
		cam.target = (Transform) EditorGUILayout.ObjectField(new GUIContent("Look Target", "The transform that the camera will focus on. Generally, this will be an empty game object in front of the ship a few units on the Z axis."), cam.target, typeof(Transform), true);
		EditorGUILayout.Separator();
		
		GUILayout.Label(new GUIContent("Parameters", ""));
		cam.follow_distance = EditorGUILayout.FloatField(new GUIContent("Follow Distance", "Controls how far behind the targeter the camera will follow."), cam.follow_distance);			
		cam.camera_elevation = EditorGUILayout.FloatField(new GUIContent("Camera Height", "Controls how high the camera will sit above the target's y coordinate."), cam.camera_elevation);		
		cam.follow_tightness = EditorGUILayout.FloatField(new GUIContent("Follow Tightness", "Higher values will result in a snappier camera, lower values will have a camera that takes a while to catch up to the ship's movements."), cam.follow_tightness);			
		cam.rotation_tightness = EditorGUILayout.FloatField(new GUIContent("Rotation Tightness", "Higher values will result in snappier camera rotations, lower values will have a camera that takes a while to catch up to the ship's rotations."), cam.rotation_tightness);			
		
		
		cam.shake_on_afterburn = EditorGUILayout.BeginToggleGroup(new GUIContent("Shake on Afterburner", "The camera will shake when afterburners are engaged."), cam.shake_on_afterburn);			
		
		cam.afterburner_Shake_Amount = EditorGUILayout.FloatField(new GUIContent("Shake Amount", "Amount the camera will shake when afterburners are active."), cam.afterburner_Shake_Amount);			
		
		EditorGUILayout.EndToggleGroup();
		
		cam.yawMultiplier = EditorGUILayout.FloatField(new GUIContent("Yaw Multiplier", "Affects the amount of the camera's yaw when the player turns left or right. This will usually be very small number (0.005)"), cam.yawMultiplier);			
		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
}




[CustomEditor(typeof(CustomPointer))]
public class CustomPointerEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		CustomPointer pointer = (CustomPointer)target;
	
		pointer.pointerTexture = (Texture) EditorGUILayout.ObjectField(new GUIContent("Pointer Texture", "The image for the pointer, generally a crosshair or dot."), pointer.pointerTexture, typeof(Texture), true);
		
	
		GUILayout.Label(new GUIContent("Input Systems (Uncheck for options)", "Choose which input the pointer will use."));
		if (pointer.use_mouse_input) {
			pointer.use_mouse_input = EditorGUILayout.Toggle(new GUIContent("Use Mouse", "Pointer will use mouse controls."), pointer.use_mouse_input);			
	
		} else if (pointer.use_gamepad_input) {		
			pointer.use_gamepad_input = EditorGUILayout.Toggle(new GUIContent("Use Gamepad", "Pointer will use gamepad joystick controls."), pointer.use_gamepad_input);
		
		} else {
			pointer.use_mouse_input = EditorGUILayout.Toggle(new GUIContent("Use Mouse", "Pointer will use mouse controls. Uncheck to choose another input option."), pointer.use_mouse_input);			
			pointer.use_gamepad_input = EditorGUILayout.Toggle(new GUIContent("Use Gamepad", "Pointer will use gamepad joystick controls. Uncheck to choose another input option."), pointer.use_gamepad_input);
		}
		
		EditorGUILayout.Separator();
		GUILayout.Label(new GUIContent("Deadzone and Centering", ""));
		
		pointer.deadzone_radius = EditorGUILayout.FloatField(new GUIContent("Deadzone Radius", "Size of the area in the center of the screen where input will be ignored."), pointer.deadzone_radius);			
		pointer.center_lock = EditorGUILayout.Toggle(new GUIContent("Visual center lock", "The pointer will be visually locked to the center. This is visual only: the cursor still functions as expected in the background, however the cursor will be drawn in the center of the screen. Recommended to be used with the 'Pointer Snaps to Center' option"), pointer.center_lock);			
		
		pointer.pointer_returns_to_center = EditorGUILayout.BeginToggleGroup(new GUIContent("Pointer snaps to center", "The pointer will snap back into the deadzone radius."), pointer.pointer_returns_to_center);			
		
		pointer.center_speed = EditorGUILayout.FloatField(new GUIContent("Snap Speed", "How quickly the pointer will move towards the screen center, even when input is being received. Generally, this should only be used with controllers."), pointer.center_speed);			
		pointer.instant_snapping = EditorGUILayout.Toggle(new GUIContent("Instant Snap on Idle", "If there is no input, the pointer will return to the screen center instantly. WARNING: FOR JOYSTICKS ONLY"), pointer.instant_snapping);
		
		EditorGUILayout.EndToggleGroup();
		
		EditorGUILayout.Separator();
		
		pointer.invert_y_axis = EditorGUILayout.Toggle(new GUIContent("Invert Y Axis", "Inverts Y input."), pointer.invert_y_axis);
		
	
		EditorGUILayout.Separator();
		GUILayout.Label(new GUIContent("Sensitivity", ""));
		pointer.mouse_sensitivity_modifier = EditorGUILayout.FloatField(new GUIContent("Mouse Speed Multiplier", "Affects how sensitive mouse input is."), pointer.mouse_sensitivity_modifier);			
		pointer.thumbstick_speed_modifier = EditorGUILayout.FloatField(new GUIContent("Thumbstick Speed Multiplier", "Affects how sensitive thumbstick input is."), pointer.thumbstick_speed_modifier);			
		
		if (GUI.changed)
			EditorUtility.SetDirty (target);
	}
}