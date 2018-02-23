using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BarScript : MonoBehaviour {

	public Vector3 sVelocity;
	public Vector3 gForce;
	public Vector3 maxHeight;
	public Rigidbody rb;

	public GameObject gameOver;

	public bool locked;

	// Use this for initialization
	void Start () {
		this.GetComponent<ConstantForce> ().force = gForce;
		locked = false;
		rb = this.GetComponent<Rigidbody> ();
		this.GetComponent<Rigidbody> ().velocity = sVelocity;
		maxHeight = new Vector3(0,16.8f,0);
	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator waitForBoost(bool p1){
		if (p1) {
			while (rb.velocity.y >= 0) {
				yield return null;
			}
		} else {
			while (rb.velocity.y <= 0) {
				yield return null;
			}
		}
		//rb.useGravity = false;
		this.GetComponent<ConstantForce>().enabled= false;
		sVelocity *= -1f;
		rb.velocity = sVelocity;
		GameObject.Find ("Main Camera").GetComponent<CameraControls> ().setCamera (!p1);
	}
		

	public void pushAway(float yForce, bool p1){
		//rb = this.GetComponent<Rigidbody> ();
		//rb.useGravity = true;
		this.GetComponent<ConstantForce> ().force = (p1)?gForce:gForce*-1f;
		
		this.GetComponent<ConstantForce>().enabled= true;
		rb.velocity = Vector3.zero;
		Debug.Log ("Pushed");
		rb.AddForce(new Vector3(0, yForce, 0), ForceMode.Impulse);
		StartCoroutine (waitForBoost (p1));
		Debug.Log ("Force Applied");
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Block") {
			Debug.Log ("Touched Block");
			sVelocity.y = 0f;
			//GameObject.Find ("GameOver").SetActive (true);
			gameOver.transform.GetChild(0).gameObject.SetActive(true);
			locked = true;
		}
	}

	public void increaseSpeed(){
		sVelocity.y += .05f;
		rb.velocity = sVelocity;
	}
}
