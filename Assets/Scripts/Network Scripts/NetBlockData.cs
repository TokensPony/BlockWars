﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetBlockData : NetworkBehaviour{
	public int color;
	//public Vector3 position;
	[SyncVar]
	public Vector2 gridCoord;
	[SyncVar]
	public Vector3 handPos;
	public GameObject handM;

	public GameObject bar;

	public BoutManager manager;
	public Scene scene;

	public float boardWidth;
	private float xOffset;
	private float offset = .15f;

	public float barOffset;

	public bool marked;
	[SyncVar]
	public bool playerOne;

	[SyncVar]
	public Vector3 cForce;
	[SyncVar]
	public bool cForceActive;

	// Use this for initialization
	void Start () {
		boardWidth = 8;
		xOffset = .15f;
	}
	
	// Update is called once per frame
	void Update () {
		if (bar == null && GameObject.FindGameObjectWithTag("Finish") != null) {
			//Debug.Log ("Set Bar");
			bar = GameObject.FindGameObjectWithTag ("Finish");
		}
	}

	void OnMouseDrag(){
		if (this.tag == "inHand" || true /*&& (playerOne && manager.p1Turn || !playerOne && !manager.p1Turn)*/) {
			dragBlock (Input.mousePosition, true);
		} 
	}

	public void dragBlock(Vector2 inputPos, bool human){
		if (true || !GameObject.FindGameObjectWithTag ("Finish").GetComponent<BarScript> ().locked && !handM.GetComponent<HandManager> ().handLocked) {
			//THIS TRUE SHIT IS TEMPORARY
			if (this.tag == "inHand" || true) {
				//Debug.Log (inputPos.x + ", " + inputPos.y);
				float distance_to_screen = Camera.main.WorldToScreenPoint (gameObject.transform.position).z;
				Vector3 pos_move = (human)?Camera.main.ScreenToWorldPoint (new Vector3 (inputPos.x, inputPos.y, distance_to_screen)):new Vector3 (inputPos.x, inputPos.y, distance_to_screen);
				float snapPosition = ((pos_move.x - Mathf.Floor (pos_move.x)) > .5f) ? Mathf.Ceil (pos_move.x) : Mathf.Floor (pos_move.x);
				snapPosition = (Mathf.Abs (snapPosition) > boardWidth / 2) ? transform.position.x : snapPosition;
				Debug.Log (snapPosition);
				this.gridCoord = new Vector2 (Mathf.Floor (Mathf.Abs (snapPosition + (boardWidth / 2))), 10);
				//this.gridCoord = new Vector2 (snapPosition + (boardWidth/2), 10);
				//Debug.Log (this.gridCoord.x);
				if (snapPosition != 0f && Mathf.Abs (snapPosition) + (xOffset * Mathf.Abs (snapPosition)) < (boardWidth / 2) + xOffset * boardWidth) {
					snapPosition += (xOffset * snapPosition);
				}
				float ySnap = 0f;
				if (playerOne) { 
					ySnap = (pos_move.y > bar.transform.position.y - 1f) ? bar.transform.position.y - 1f : pos_move.y;
				} else {
					ySnap = (pos_move.y < bar.transform.position.y + 1f) ? bar.transform.position.y + 1f : pos_move.y;
				}
				transform.position = new Vector3 (snapPosition, ySnap, -2f);
			}
		}
	}

	public void release(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
		if(this.tag == "inHand" || true){
			if ((this.transform.position.y >= .5f && playerOne) || this.transform.position.y <= 31.5f && !playerOne) {
				this.tag = "Block";
				this.GetComponent<ConstantForce> ().enabled = true;
				//cForceActive = true;
				////manager.addBlock (this.gameObject);
				////manager.collapseBoard ();
				//this.GetComponent<Rigidbody> ().useGravity = true;

				this.GetComponent<Rigidbody> ().velocity = (playerOne)? new Vector3 (0, -1, 0) : new Vector3 (0, 1, 0);
				marked = true;
				//StartCoroutine (waitToCollide ());
			} else {
				this.transform.position = handPos;
			}
		}
	}
		
	void OnMouseUp(){
		release ();
	}

	void OnMouseDown(){
		RaycastHit hitInfo = new RaycastHit();
		bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
		if (hit) 
		{
			GameObject target = hitInfo.transform.gameObject;
			Debug.Log("Hit " + target.name);
			if (hitInfo.transform.gameObject.tag == "Block"){
				
			} else {
				//Debug.Log ("nopz");
			}
		} else {
			//Debug.Log("No hit");
		}
	}

}