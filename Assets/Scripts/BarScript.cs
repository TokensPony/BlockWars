using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BarScript : MonoBehaviour {

	public Vector3 sVelocity;
	//public Vector3 extraVelocity;
	public Vector3 gForce;
	public Vector3 maxHeight;
	public Rigidbody rb;
	public bool p1Turn;

	public GameObject gameOver;

	public bool locked;

	// Use this for initialization
	void Start () {
		p1Turn = true;
		this.GetComponent<ConstantForce> ().force = gForce;
		locked = false;
		rb = this.GetComponent<Rigidbody> ();
		this.GetComponent<Rigidbody> ().velocity = sVelocity;
		maxHeight = new Vector3(0,16.8f,0);
	}
	
	// Update is called once per frame
	void Update () {
		/*if (p1Turn && this.transform.position.y < 16.8f || !p1Turn && this.transform.position.y > 16.8f) {
			Debug.Log ("Normal Speed");
			rb.velocity = sVelocity;
		}*/
		//Debug.Log (rb.velocity);
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
		Vector3 temp = sVelocity;
		/*if (p1 && this.transform.position.y < 16.8f || !p1 && this.transform.position.y < 16.8f) {
			temp.y *= 2f;
		}*/
		rb.velocity = temp;
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
		p1Turn = !p1Turn;
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.tag == "Block") {
			Debug.Log ("Touched Block");
			sVelocity.y = 0f;
			//GameObject.Find ("GameOver").SetActive (true);
			//gameOver.transform.GetChild(0).gameObject.SetActive(true);
			gameOver.SetActive(true);
			locked = true;
		}
	}

	public void increaseSpeed(){
		sVelocity.y += .05f;
		rb.velocity = sVelocity;
	}
}
