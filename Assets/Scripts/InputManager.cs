
using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	
	public bool mouseDown = false;
	public bool mouseDownValid3dLocation = false;
	public Vector3 hitPoint3d;
	public bool debugInputManager = false;

    public void ManualUpdate() {
        if ((Input.GetMouseButton(0))&&(!Input.GetKey(KeyCode.LeftAlt))) {
			mouseDown = true;
			if (debugInputManager == true) {
				Debug.Log("2d mouse location: " + Input.mousePosition);
			}
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit)) { 
				if (hit.transform != null) 	{
					hitPoint3d = hit.point;
					mouseDownValid3dLocation = true;
					if (debugInputManager == true) {					
						Debug.Log("3d camera ray hit point " + hit.point);
					}
				}
			}          
        } else {
			mouseDown = false;
			mouseDownValid3dLocation = false;
			hitPoint3d = Vector3.zero;
		}
    }
	
	private void OnGUI() {
//		GUI.Label (new Rect(0,0,200,200), Input.GetAxis("Mouse X") + "," + Input.GetAxis("Mouse Y") + ", pos = " + Input.mousePosition);		
	}
}