using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {
	public Vector3 p1Position;
	public Vector3 p1Rotation;

	public Vector3 p2Position;
	public Vector3 p2Rotation;
	// Use this for initialization
	void Start () {
		this.transform.position = p1Position;
		this.transform.rotation = Quaternion.Euler(p1Rotation);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setCamera(bool p1){
		if (p1) {
			this.transform.position = p1Position;
			this.transform.rotation = Quaternion.Euler (p1Rotation);
		} else {
			this.transform.position = p2Position;
			this.transform.rotation = Quaternion.Euler (p2Rotation);
		}
	}
}
