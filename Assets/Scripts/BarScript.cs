using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BarScript : MonoBehaviour {

	public Vector3 sVelocity;
	public Vector3 maxHeight;
	public Rigidbody rb;

	public GameObject gameOver;

	public bool locked;

	// Use this for initialization
	void Start () {
		locked = false;
		rb = this.GetComponent<Rigidbody> ();
		this.GetComponent<Rigidbody> ().velocity = sVelocity;
		maxHeight = new Vector3(0,15.8f,0);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			
			pushAway (3f);

		}
		//Debug.Log (rb.velocity.y);

		if (rb.transform.position.y > maxHeight.y) {
			rb.velocity = Vector3.zero;
			rb.transform.position = maxHeight;
			//break;
		}
		/*if (rb.velocity.y < 0) {
			Debug.Log ("Entered");
			rb.useGravity = false;
			this.GetComponent<Rigidbody> ().velocity = sVelocity;
		}*/
		//this.GetComponent<Rigidbody> ().velocity = sVelocity;
	}

	IEnumerator waitForBoost(){
		while (rb.velocity.y >= 0) {
			yield return null;
		}
		rb.useGravity = false;
		rb.velocity = sVelocity;
	}
		

	public void pushAway(float yForce){
		//rb = this.GetComponent<Rigidbody> ();
		rb.useGravity = true;
		rb.velocity = Vector3.zero;
		Debug.Log ("Pushed");
		rb.AddForce(new Vector3(0, yForce, 0), ForceMode.Impulse);
		StartCoroutine (waitForBoost ());
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
}
