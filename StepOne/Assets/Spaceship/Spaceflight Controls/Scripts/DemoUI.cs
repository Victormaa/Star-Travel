using UnityEngine;
using System.Collections;

public class DemoUI : MonoBehaviour {

	bool cursorlock = true;

	// Use this for initialization
	void Start () {
		
	
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		/*
		//Uncomment for Unity 5 to get rid of the warnings.
		if (cursorlock)
			Cursor.lockState = CursorLockMode.Locked;
		else
			Cursor.lockState = CursorLockMode.None;
		*/
		
		//Delete this statement for Unity 5.
		if (cursorlock)
			Screen.lockCursor = true;
		else
			Screen.lockCursor = false;		
		
		
		if (Input.GetKeyDown(KeyCode.Escape))
			cursorlock = !cursorlock;
		
		if (Input.GetKeyDown(KeyCode.C)) {
			CustomPointer.instance.pointer_returns_to_center =  !CustomPointer.instance.pointer_returns_to_center;
			
		}
		
		if (Input.GetKeyDown(KeyCode.L)) {
			CustomPointer.instance.center_lock =  !CustomPointer.instance.center_lock;
			
		}		
		
		if (Application.loadedLevel != 3) {
			if (Input.GetKeyDown(KeyCode.Equals)) {
				CameraFlightFollow.instance.follow_distance++;
			}
			
			if (Input.GetKeyDown(KeyCode.Minus)) {
				CameraFlightFollow.instance.follow_distance--;
			}
			
			if (Input.GetKeyDown(KeyCode.Comma)) {
				CameraFlightFollow.instance.follow_tightness--;
			}
			if (Input.GetKeyDown(KeyCode.Period)) {
				CameraFlightFollow.instance.follow_tightness++;
			}
		}
		
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			Application.LoadLevel(0);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			Application.LoadLevel(1);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			Application.LoadLevel(2);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			Application.LoadLevel(3);
		}
	
	}
	
	

	void OnGUI() {
	
		if (Application.loadedLevel != 3)
		GUI.Label(new Rect(10,10, 650,250), "Controls: W/S for thrust, A/D for roll, mouse for pitch/yaw." +
			          "\n-/+ to increase or decrease camera follow distance. </> to increase or decrease follow tightness.\nC to enable or disable pointer centering.\nL to toggle center lock\n1-4 to change scenes\nESC to unlock cursor");
		
		else
		GUI.Label(new Rect(10,10, 650,250), "Controls: W/S for thrust, A/D for roll, mouse for pitch/yaw." +
			          "\nC to toggle pointer centering.\nL to toggle center lock\n1-4 to change scenes\nESC to unlock cursor");
		
	
	}
	

	
}
