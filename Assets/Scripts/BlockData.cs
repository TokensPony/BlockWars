using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class BlockData : MonoBehaviour{

	public int color;
	//public Vector3 position;
	public Vector2 gridCoord;
	public Vector3 handPos;
	public GameObject handM;

	GameObject bar;

	public BoutManager manager;
	public Scene scene;

	public float boardWidth;
	private float xOffset;
	private float offset = .15f;

	public float barOffset;

	public bool marked;
	public bool playerOne;

	public void Start(){
		manager = GameObject.Find("BoutManager").GetComponent<BoutManager>();
		marked = false;
		boardWidth = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().boardWidth;
		xOffset = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().xOffset;
		bar = GameObject.Find ("Bar");
		if (!playerOne) {
			this.GetComponent<ConstantForce> ().force *= -1f; 
		}
		scene = SceneManager.GetActiveScene ();
	}

	void Update(){
		if (marked) {
			//Destroy (this.gameObject);
		}

		//onMouseDown ();
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
		if ((playerOne && manager.p1Turn || !playerOne && !manager.p1Turn) && (string.Equals(scene.name, "AIScene") && !playerOne)) {
			dragBlock (Input.mousePosition, true);
		} else {
			this.transform.position = handPos;
		}
		//dragBlock (new Vector2(0f, 10f));
	}

	public void dragBlock(Vector2 inputPos, bool human){
		if (!GameObject.Find ("Bar").GetComponent<BarScript> ().locked && !handM.GetComponent<HandManager> ().handLocked) {
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
		if (this.tag == "Block") {
			this.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 1, 0);
		}
		if(this.tag == "inHand"){
			if ((this.transform.position.y >= .5f && playerOne) || this.transform.position.y <= 32f && !playerOne) {
				this.tag = "Block";
				this.GetComponent<ConstantForce> ().enabled = true;
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
