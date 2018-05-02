using System.Collections;
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
	public bool handLocked;

	public BoutManager manager;
	public Scene scene;

	public float boardWidth;
	private float xOffset;
	private float offset = .15f;

	public float barOffset;

	public bool marked;
	[SyncVar]
	public bool playerOne;

	public float dragOffset;
	[SyncVar]
	public Vector3 cForce;
	[SyncVar]
	public bool cForceActive;

	// Use this for initialization
	void Start () {
		handLocked = false;
		boardWidth = 8;
		xOffset = .15f;

		//PlayerPrefs.SetInt ("handLocked", 0);
	}
	
	// Update is called once per frame
	void Update () {
		if (bar == null && GameObject.FindGameObjectWithTag("Finish") != null) {
			//Debug.Log ("Set Bar");
			bar = GameObject.FindGameObjectWithTag ("Finish");
		}

		float currVel = this.GetComponent<Rigidbody> ().velocity.y;


		if (playerOne && currVel > 0 || !playerOne && currVel<0) {
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		}
		//this.GetComponent<ConstantForce> ().enabled = true;

	}

	void OnMouseDrag(){
		if (this.tag == "inHand" && !handLocked) {
			dragBlock (Input.mousePosition, true);
		} 
	}

	//[Command]
	public void dragBlock(Vector2 inputPos, bool human){
		if (!GameObject.FindGameObjectWithTag ("Finish").GetComponent<NetBarScript> ().locked ) {
			//THIS TRUE SHIT IS TEMPORARY
			if (this.tag == "inHand") {
				//Debug.Log (inputPos.x + ", " + inputPos.y);
				float distance_to_screen = Camera.main.WorldToScreenPoint (gameObject.transform.position).z;
				Vector3 pos_move = (human)?Camera.main.ScreenToWorldPoint (new Vector3 (inputPos.x, inputPos.y, distance_to_screen)):new Vector3 (inputPos.x, inputPos.y, distance_to_screen);
				float snapPosition = ((pos_move.x - Mathf.Floor (pos_move.x)) > .5f) ? Mathf.Ceil (pos_move.x) : Mathf.Floor (pos_move.x);
				snapPosition = (Mathf.Abs (snapPosition) > boardWidth / 2) ? transform.position.x : snapPosition;
				//Debug.Log (snapPosition);
				this.gridCoord = new Vector2 (Mathf.Floor (Mathf.Abs (snapPosition + (boardWidth / 2))), 10);
				//this.gridCoord = new Vector2 (snapPosition + (boardWidth/2), 10);
				//Debug.Log (this.gridCoord.x);
				if (snapPosition != 0f && Mathf.Abs (snapPosition) + (xOffset * Mathf.Abs (snapPosition)) < (boardWidth / 2) + xOffset * boardWidth) {
					snapPosition += (xOffset * snapPosition);
				}
				float ySnap = 0f;
				if (playerOne) { 
					ySnap = (pos_move.y + dragOffset > bar.transform.position.y - 1f) ? bar.transform.position.y - 1f : pos_move.y + dragOffset;
				} else {
					ySnap = (pos_move.y - dragOffset < bar.transform.position.y + 1f) ? bar.transform.position.y + 1f : pos_move.y - dragOffset;
				}
				transform.position = new Vector3 (snapPosition, ySnap, -2f);
				//RpcDragBlock (snapPosition, ySnap);
			}
		}
	}

	[ClientRpc]
	public void RpcDragBlock(float snapPosition, float ySnap){
		transform.position = new Vector3 (snapPosition, ySnap, -2f);
	}
		
	public void release(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
		if(this.tag == "inHand"){
			if ((this.transform.position.y >= .5f && playerOne) || this.transform.position.y <= 31.5f && !playerOne) {
				this.tag = "Block";
				this.GetComponent<ConstantForce> ().enabled = true;
				//cForceActive = true;
				this.transform.root.gameObject.GetComponent<NetworkPlayer>().addBlock (this.gameObject);
				this.transform.root.gameObject.GetComponent<NetworkPlayer>().collapseBoard ();
				//this.GetComponent<Rigidbody> ().useGravity = true;

				this.GetComponent<Rigidbody> ().velocity = (playerOne)? new Vector3 (0, -1, 0) : new Vector3 (0, 1, 0);
				marked = true;
				StartCoroutine (waitToCollide ());
			} else {
				this.transform.position = handPos;
			}
		}
	}
		
	void OnMouseUp(){
		release ();
	}

	/*Coroutine to delay the activation of the match checking until after
	 * the block has fallen and hit the ground.*/
	IEnumerator waitToCollide(){
		if (playerOne) {
			while (this.GetComponent<Rigidbody> ().velocity.y < 0f) {
				yield return null;
			}
		} else {
			while (this.GetComponent<Rigidbody> ().velocity.y > 0f) {
				yield return null;
			}
		}

		this.transform.root.gameObject.GetComponent<NetworkPlayer>().isMarked();
		//yield return null;
	}

	/*Slow down velocity upon hitting a block*/
	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "Block" || collision.gameObject.tag == "Floor"){
			//Debug.Log ("Hit");
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			if (collision.gameObject.tag == "Block") {
				collision.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}
		}
		if (collision.gameObject == bar) {
			Debug.Log ("Bar Hit");
		}
	}

	void OnCollisionExit(Collision collision){
		if(collision.gameObject.tag == "Block" || collision.gameObject.tag == "Floor"){
			//Debug.Log ("Hit");
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			if (collision.gameObject.tag == "Block") {
				collision.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			}
		}
	}

	void OnMouseDown(){
		RaycastHit hitInfo = new RaycastHit();
		bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
		if (hit) 
		{
			GameObject target = hitInfo.transform.gameObject;
			Debug.Log("Hit " + target.name);
			/*(if (hitInfo.transform.gameObject.tag == "Block"){
				marked = true;
				this.transform.parent.gameObject.GetComponent<NetworkPlayer> ().isMarked ();
			} else {
				//Debug.Log ("nopz");
			}*/
		} else {
			//Debug.Log("No hit");
		}
	}

}
