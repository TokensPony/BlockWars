using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void OnMouseDown(){
		RaycastHit hitInfo = new RaycastHit ();
		bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
		if (hit) {
			GameObject target = hitInfo.transform.gameObject;
			Debug.Log ("Hit " + target.name);
			if (hitInfo.transform.gameObject.tag == "Power Up") {
				
			} else {
				//Debug.Log ("nopz");
			}
		} else {
			//Debug.Log("No hit");
		}
	}

	public virtual void Action(){

	}
}
