using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PowerUp : MonoBehaviour {

	protected bool mouseDown;
	protected GameObject board;
	protected GameObject bar;

	// Use this for initialization
	protected virtual void Start () {
		mouseDown = false;
		board = GameObject.Find ("BoutManager").gameObject;
		bar = GameObject.Find ("Bar").gameObject;
	}
	
	// Update is called once per frame
	protected virtual void Update () {
		
	}

	public virtual void OnMouseDown(){
		RaycastHit hitInfo = new RaycastHit ();
		bool hit = Physics.Raycast (Camera.main.ScreenPointToRay (Input.mousePosition), out hitInfo);
		if (hit) {
			GameObject target = hitInfo.transform.gameObject;
			//Debug.Log ("Hit " + target.name);
			if (hitInfo.transform.gameObject.tag == "Power Up") {
				Debug.Log ("Power Up");
				mouseDown = true;
			} else {
				//Debug.Log ("nopz");
			}
		} else {
			//Debug.Log("No hit");
		}
	}

	public virtual void OnMouseUp(){
		mouseDown = false;
	}


	public virtual void Action(){
		GameObject.Find ("PowerUpManager").GetComponent<PowerUpManager> ().currentPowUp = null;
		Destroy (this.gameObject);
	}
}
