using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;


public class BlockData : NetworkBehaviour{

	public int color;
	//public Vector3 position;
	[SyncVar]
	public Vector2 gridCoord;
	[SyncVar]
	public Vector3 handPos;
	public GameObject handM;

	GameObject bar;

	public BoutManager manager;
	public Scene scene;

	public float boardWidth;
	private float xOffset;
	private float offset = .15f;
	public float dragOffset;

	public float barOffset;

	public bool marked;
	[SyncVar]
	public bool playerOne;

	[SyncVar]
	public Vector3 cForce;
	[SyncVar]
	public bool cForceActive;

	public void Start(){
		manager = GameObject.Find("BoutManager").GetComponent<BoutManager>();
		marked = false;
		boardWidth = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().boardWidth;
		xOffset = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().xOffset;
		//bar = GameObject.FindGameObjectWithTag ("Finish");
		/*if (!playerOne) {
			this.GetComponent<ConstantForce> ().force *= -1f; 
		}*/

		scene = SceneManager.GetActiveScene ();
		//Debug.Log (scene);
	}

	void Update(){
		if (marked) {
			//Destroy (this.gameObject);
		}
		if (bar == null && GameObject.FindGameObjectWithTag("Finish") != null) {
			//Debug.Log ("Set Bar");
			bar = GameObject.FindGameObjectWithTag ("Finish");
		}

		//this.GetComponent<ConstantForce> ().enabled = cForceActive;
		//this.GetComponent<ConstantForce> ().force = cForce;
		//onMouseDown ();

	}

	public void blockSetup(int xPos, int yPos, bool p1, List<Material> textures, float spawnHeight, float xOffset, float yOffset, float bw){
		//Debug.Log (scene.name);
		/*if (string.Equals (SceneManager.GetActiveScene().name, "Network")) {
			Debug.Log ("In Network Scene");
			//this.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
			CmdBlockSetup (xPos, yPos, p1, spawnHeight, xOffset, yOffset, bw);
		} else {*/
			//Debug.Log ("Not in Network Scene");
			boardWidth = bw;
			int randIndex = Random.Range (0, textures.Count);
			this.GetComponent<Renderer> ().material = textures [randIndex];
			color = randIndex;
			//Vector3 spawnPoint = new Vector3 (Mathf.Floor (Random.value * boardWidth - (boardWidth/2f)), Mathf.Floor (Random.value * 10f) + spawnHeight++, 0f);
			Vector3 spawnPoint = new Vector3 (xPos - (boardWidth / 2f), 0f, 0f);
			//Debug.Log (xPos);
			if (p1) {
				spawnPoint.y = yPos + spawnHeight + (yPos * yOffset);
				//Debug.Log ("P1 Block: " + spawnPoint.y);
			} else {
				spawnPoint.y = (yPos - spawnHeight) - ((31 - yPos) * yOffset);
				//Debug.Log ("P2 Block: " + spawnPoint.y);
			}
			//Debug.Log (spawnPoint.y);
			if (spawnPoint.x > 0f) {
				spawnPoint.x += (xOffset * spawnPoint.x);
			} else if (spawnPoint.x < 0f) {
				spawnPoint.x -= (xOffset * Mathf.Abs (spawnPoint.x));
			}
			this.transform.position = spawnPoint;
			//Debug.Log (spawnPoint);
			gridCoord = new Vector2 (xPos, yPos);
			playerOne = p1;
			if (!playerOne) {
				this.GetComponent<ConstantForce> ().force *= -1f;
				//cForce *= -1f;
			}
		//}
	}

	[Command]
	public void CmdBlockSetup(int xPos, int yPos, bool p1, float spawnHeight, float xOffset, float yOffset, float bw){
		boardWidth = bw;
		//int randIndex = Random.Range (0, textures.Count);
		//this.GetComponent<Renderer> ().material = textures[randIndex];
		//color = randIndex;
		//Vector3 spawnPoint = new Vector3 (Mathf.Floor (Random.value * boardWidth - (boardWidth/2f)), Mathf.Floor (Random.value * 10f) + spawnHeight++, 0f);
		Vector3 spawnPoint = new Vector3 (xPos - (boardWidth/2f), 0f, 0f);
		Debug.Log (xPos);
		if (p1) {
			spawnPoint.y = yPos + spawnHeight + (yPos * yOffset);
			//Debug.Log ("P1 Block: " + spawnPoint.y);
		} else {
			spawnPoint.y = (yPos - spawnHeight) - ((31 - yPos) * yOffset);
			//Debug.Log ("P2 Block: " + spawnPoint.y);
		}
		//Debug.Log (spawnPoint.y);
		if (spawnPoint.x > 0f) {
			spawnPoint.x += (xOffset*spawnPoint.x);
		} else if (spawnPoint.x < 0f) {
			spawnPoint.x -= (xOffset*Mathf.Abs(spawnPoint.x));
		}
		this.transform.position = spawnPoint;
		//Debug.Log (spawnPoint);
		gridCoord = new Vector2 (xPos, yPos);
		playerOne = p1;
		if (!playerOne) {
			this.GetComponent<ConstantForce> ().force *= -1f; 
		}
		Debug.Log (this.GetComponent<ConstantForce> ().force.y);
	}

	[Command]
	public void CmdSetAuthority(){
		//NetworkServer.AddPlayerForConnection(this.GetComponent<NetworkIdentity>().connectionToClient, this.gameObject, 0);
	}

	/*Controls what happens when dragging a block. If the item being dragged is "inHand", it
	calculates the move position based on worldToScreenPoint Coords from the camera. It then
	creates a "SnapPosition for the x axis, which causes the block to "Snap" into an x position
	that is perfectly in-line with the rest of the blocks in the pile. It also calculates an offset
	added onto the snap position to account for spacing. It also floors the absolute value of the
	SnapPosition and sets it as the x value for the grid coord. which dictates where in the grid the
	block will be placed, if the block is dropped. It also creates a ySnap that snaps the block to
	the bar when it is close to it.*/

	void OnMouseDrag()
	{
		//Debug.Log ("Trying to Drag");
		if (string.Equals (scene.name, "Local2Player")) {
			if (this.tag == "inHand" && (playerOne && manager.p1Turn || !playerOne && !manager.p1Turn)) {
				dragBlock (Input.mousePosition, true);
			} else if (this.tag == "inHand") {
				this.transform.position = handPos;
			}
		} else if (string.Equals (scene.name, "AIScene")) {
			if (this.tag == "inHand" && !playerOne) {
				dragBlock (Input.mousePosition, true);
			} else if (this.tag == "inHand") {
				this.transform.position = handPos;
			}
		} else if (string.Equals (scene.name, "Network") /*&& string.Equals(transform.root.gameObject.name, "Player(Clone)")*/) {
			Debug.Log ("Network Scene");
			if (this.tag == "inHand" && (playerOne && manager.p1Turn || !playerOne && !manager.p1Turn)) {
				dragBlock (Input.mousePosition, true);
			} else if (this.tag == "inHand") {
				this.transform.position = handPos;
			}
		}
	}

	public void dragBlock(Vector2 inputPos, bool human){
		if (!GameObject.FindGameObjectWithTag ("Finish").GetComponent<BarScript> ().locked && !handM.GetComponent<HandManager> ().handLocked) {
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
			}
		}
	}

	public void release(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
		if (this.tag == "Block") {
			//this.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 1, 0);
		}
		if(this.tag == "inHand"){
			if ((this.transform.position.y >= .5f && playerOne) || this.transform.position.y <= 31.5f && !playerOne) {
				this.tag = "Block";
				this.GetComponent<ConstantForce> ().enabled = true;
				//cForceActive = true;
				manager.addBlock (this.gameObject);
				manager.collapseBoard ();
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
		handM.GetComponent<HandManager> ().handLocked = true;
		if (playerOne) {
			while (this.GetComponent<Rigidbody> ().velocity.y < 0f) {
				yield return null;
			}
		} else {
			while (this.GetComponent<Rigidbody> ().velocity.y > 0f) {
				yield return null;
			}
		}
		handM.GetComponent<HandManager> ().handLocked = false;
		manager.isMarked();
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
			if (hitInfo.transform.gameObject.tag == "Block"){
			} else {
				//Debug.Log ("nopz");
			}
		} else {
			//Debug.Log("No hit");
		}
	}
}
