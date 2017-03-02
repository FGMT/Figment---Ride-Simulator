using UnityEngine;
using System.Collections;

public class VRCamera_Constraint : MonoBehaviour {

	public Transform constrainToTransform;
	public Transform constrainLookAt;

	public bool constrainTranslation = true;
	public bool constrainRotation = false;
	public bool lockCameraRoll = true;

	// Use this for initialization
	void Start() {



	}

	// Update is called once per frame
	void Update() {
		if (constrainTranslation == true) {
			this.transform.position = constrainToTransform.position;
		}
		if (constrainRotation == true) {
			this.transform.rotation = constrainToTransform.rotation;

            if (lockCameraRoll == true) {
				this.transform.LookAt(constrainLookAt, Vector3.up);
			}
		}

	}
}
