using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class BlockData : MonoBehaviour{

	public string color;
	//public Vector3 position;
	public Vector2 gridCoord;
	public Vector3 handPos;

	public GameObject bar;

	public BoutManager manager;
	//public float spawnY;
	//public List<Material> textures;

	public float boardWidth;
	private float xOffset;
	private float offset = .15f;

	public bool marked;

	public void Start(){
		manager = GameObject.Find("BoutManager").GetComponent<BoutManager>();
		marked = false;
		boardWidth = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().boardWidth;
		xOffset = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().xOffset;
		bar = GameObject.Find ("Plane");
	}

	void Update(){
		if (marked) {
			//Destroy (this.gameObject);
		}
		//onMouseDown ();
	}

	void OnMouseDrag()
	{
		if(this.tag == "inHand"){
			float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
			Vector3 pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
			float snapPosition = ((pos_move.x - Mathf.Floor (pos_move.x)) > .5f) ? Mathf.Ceil (pos_move.x) : Mathf.Floor (pos_move.x);
			snapPosition = (Mathf.Abs (snapPosition) > boardWidth / 2) ? transform.position.x : snapPosition;
			//Debug.Log (snapPosition);
			this.gridCoord = new Vector2 (Mathf.Floor(Mathf.Abs(snapPosition + (boardWidth/2))), 10);
			//this.gridCoord = new Vector2 (snapPosition + (boardWidth/2), 10);
			Debug.Log (this.gridCoord.x);
			if (snapPosition != 0f && Mathf.Abs(snapPosition) + (xOffset* Mathf.Abs(snapPosition)) < (boardWidth/2)+ xOffset*boardWidth) {
				snapPosition += (xOffset * snapPosition);
			}
			float ySnap = (pos_move.y > bar.transform.position.y - .5f) ? bar.transform.position.y : pos_move.y;

			transform.position = new Vector3 (snapPosition, ySnap, -2f);
		}
	}

	/*public void OnDrag(PointerEventData ped){
		float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
		Vector3 pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (ped.position.x, ped.position.y, distance_to_screen));
		float snapPosition = ((pos_move.x - Mathf.Floor (pos_move.x)) > .5f) ? Mathf.Ceil (pos_move.x) : Mathf.Floor (pos_move.x);
		snapPosition = (Mathf.Abs (snapPosition) > boardWidth / 2) ? transform.position.x : snapPosition;
		if (snapPosition != 0f && Mathf.Abs(snapPosition) + (xOffset* Mathf.Abs(snapPosition)) < (boardWidth/2)+ xOffset*boardWidth) {
			snapPosition += (xOffset * snapPosition);
		}
		transform.position = new Vector3 (snapPosition, pos_move.y, -2f);
	}*/

	void OnMouseUp(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
		if (this.tag == "Block") {
			this.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 1, 0);
		}
		if(this.tag == "inHand"){
			if (this.transform.position.y >= bar.transform.position.y) {
				manager.addBlock (this.gameObject);
				manager.collapseBoard ();
				//this.GetComponent<Rigidbody> ().velocity = new Vector3(0,1,0);
				this.GetComponent<Rigidbody> ().useGravity = true;
				this.tag = "Block";
				this.GetComponent<Rigidbody> ().velocity = new Vector3 (0, -1, 0);
				//GameObject.Find("HandManager").GetComponent<HandManager>().drawBlock(
				marked = true;
				StartCoroutine (waitToCollide ());
			} else {
				this.transform.position = handPos;
			}
		}
	}

	/*Coroutine to delay the activation of the match checking until after
	 * the block has fallen and hit the ground.*/
	IEnumerator waitToCollide(){
		while (this.GetComponent<Rigidbody> ().velocity.y < 0f) {
			
			yield return null;
		}
		manager.isMarked();
		//yield return null;
	}

	/*Slow down velocity upon hitting a block*/
	void onCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "Block"){
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
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
				Debug.Log ("nopz");
			}
		} else {
			Debug.Log("No hit");
		}
	}
}
