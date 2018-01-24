using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour {

	public string color;
	public Vector3 position;
	public Vector2 gridCoord;
	public Material texture;

	public float boardWidth;
	private float xOffset;
	private float offset = .15f;

	public void Start(){
		boardWidth = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().boardWidth;
		xOffset = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().xOffset;
	}

	void OnMouseDrag()
	{
		float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
		Vector3 pos_move = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));
		float snapPosition = ((pos_move.x - Mathf.Floor (pos_move.x)) > .5f) ? Mathf.Ceil (pos_move.x) : Mathf.Floor (pos_move.x);
		snapPosition = (Mathf.Abs (snapPosition) > boardWidth / 2) ? transform.position.x : snapPosition;
		/*if (snapPosition > 0f) {
			snapPosition += (xOffset * (boardWidth/2));
		} else if (snapPosition < 0f) {
			snapPosition -= (xOffset * (boardWidth/2));
		}*/
		if (snapPosition != 0f && Mathf.Abs(snapPosition) + (xOffset* Mathf.Abs(snapPosition)) < (boardWidth/2)+ xOffset*boardWidth) {
			snapPosition += (xOffset * snapPosition);
		}
		transform.position = new Vector3 (snapPosition, pos_move.y, -2f);
	}

	void OnMouseUp(){
		transform.position = new Vector3 (transform.position.x, transform.position.y, 0f);
		this.GetComponent<Rigidbody> ().velocity = new Vector3(0,1,0);
	}

	void onCollisionEnter(Collision collision){
		if(collision.gameObject.tag == "Block"){
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		}
	}
}
