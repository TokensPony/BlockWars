using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezePowerUp : PowerUp {

	public float freezeTime;
	// Use this for initialization
	void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	void Update () {
		base.Update ();
	}

	public override void OnMouseDown(){
		base.OnMouseDown ();
		if (mouseDown) {
			Action ();
		}
	}

	public override void Action(){
		StartCoroutine (freezeBar ());
		//base.Action ();
	}

	private IEnumerator freezeBar(){
		Debug.Log ("Start Freeze");
		bar.GetComponent<BarScript> ().waiting = true;
		bar.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		yield return new WaitForSecondsRealtime (freezeTime);
		bar.GetComponent<Rigidbody> ().velocity = bar.GetComponent<BarScript> ().sVelocity;
		bar.GetComponent<BarScript> ().waiting = false;
		base.Action ();
	}
}
