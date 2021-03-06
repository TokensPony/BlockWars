﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class NetBarScript : NetworkBehaviour {

	[SyncVar]
	public Vector3 sVelocity;
	[SyncVar]
	public Vector3 fastVelocity;
	[SyncVar]
	public Vector3 finalVelocity;
	//public Vector3 extraVelocity;
	[SyncVar]
	public Vector3 gForce;
	public Vector3 maxHeight;
	public Rigidbody rb;

	[SyncVar]
	public bool p1Turn;
	[SyncVar]
	public float incSpeed;

	[SyncVar]
	public bool onP1;
	public GameObject gameOver;
	[SyncVar]
	public bool locked;
	[SyncVar]
	public bool waiting;

	// Use this for initialization
	void Start () {
		gameOver = Instantiate (gameOver);//GameObject.FindGameObjectWithTag ("GameController").GetComponent<BoutManager>().boutUI;
		p1Turn = true;
		this.GetComponent<ConstantForce> ().force = gForce;
		locked = false;
		//rb = this.GetComponent<Rigidbody> ();
		finalVelocity = (p1Turn)? sVelocity: sVelocity * -1f;
		gameObject.GetComponent<Rigidbody> ().velocity = finalVelocity;
		maxHeight = new Vector3(0,16.8f,0);
		fastVelocity = sVelocity * 1.5f;
		waiting = false;
		onP1 = true;
		setSelf ();
	}

	public void setSelf(){
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players) {
			player.GetComponent<NetworkPlayer> ().bar = this.gameObject;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if ((p1Turn && this.transform.position.y > 16f || !p1Turn && this.transform.position.y < 16f) && !waiting) {
			//Debug.Log (" Speed");
			finalVelocity = (p1Turn) ? fastVelocity : fastVelocity * -1f;
			rb.velocity = finalVelocity;
		} else if (!waiting) {
			finalVelocity = (p1Turn) ? sVelocity : sVelocity * -1f;
			rb.velocity = finalVelocity;
		}
		//Debug.Log (rb.velocity);
		//rb.velocity = finalVelocity;
	}

	IEnumerator waitForBoost(bool p1){
		//fastVelocity *= -1f;
		//sVelocity *= -1f;

		finalVelocity = (!p1)? sVelocity : sVelocity*-1f;
		waiting = true;
		lockHands (true);

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
		//Vector3 temp = sVelocity;
		/*if (p1Turn != onP1) {
			Debug.Log ("Push made when not in view");
			finalVelocity = sVelocity;
		}*/
		p1Turn = !p1;



		/*if (p1 && this.transform.position.y < 16f || !p1 && this.transform.position.y < 16f) {
			temp.y *= 2f;
		}*/
		rb.velocity = finalVelocity;
		waiting = false;
		lockHands (false);
	}
		
	public void pushAway(float yForce, bool p1){
		//rb = this.GetComponent<Rigidbody> ();
		//rb.useGravity = true;
		this.GetComponent<AudioSource>().Play();
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
			//GameObject.FindGameObjectWithTag("BoutUI").SetActive (true);
			//gameOver.transform.GetChild(0).gameObject.SetActive(true);
			gameOver.SetActive (true);
			locked = true;
		} 
	}

	public void increaseSpeed(){
		//sVelocity.y += (sVelocity.y > 0f)? incSpeed: -incSpeed;
		sVelocity.y -= incSpeed;
		fastVelocity = sVelocity * 1.5f;
		incSpeed *= .9f;
		rb.velocity = sVelocity;
	}

	public void lockHands(bool handSet){
		GameObject[] hands = GameObject.FindGameObjectsWithTag ("HandManager");
		foreach (GameObject hand in hands) {
			hand.GetComponent<HandManager> ().handLocked = handSet;
		}
	}
}