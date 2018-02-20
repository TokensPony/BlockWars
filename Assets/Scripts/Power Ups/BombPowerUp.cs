using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombPowerUp : PowerUp {

	// Use this for initialization
	void Start () {
		base.Start ();
		//board = GameObject.Find ("BoutManager").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnMouseDown(){
		base.OnMouseDown ();
		if (mouseDown) {
			Action ();
		}
	}

	public override void Action(){
		//Debug.Log ("Override Success");
		//GameObject[,] currBlocks = GameObject.Find ("BoutManager").gameObject.GetComponent<BoutManager> ().blocks;
		GameObject[,] currBlocks = board.GetComponent<BoutManager>().blocks;
		for (int x = 0; x < currBlocks.GetLength (1); x++) {
			if (currBlocks [0, x] != null) {
				Destroy (currBlocks [0, x]);
				currBlocks [0, x] = null;
			}
			/*if (currBlocks [31, x] != null) {
				Destroy (currBlocks [31, x]);
				currBlocks [31, x] = null;
			}*/
		}
		board.GetComponent<BoutManager> ().blocks = currBlocks;
		board.GetComponent<BoutManager> ().collapseBoard ();
		base.Action ();
	}
}
