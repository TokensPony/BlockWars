using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour {

	public GameObject test;
	public bool player1;

	// Use this for initialization
	void Start () {
		
		//test = GameObject.FindGameObjectWithTag ("Finish");
		//test.transform.parent = this.gameObject.transform;
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		if (players.GetLength (0) == 1) {
			GameObject.Find ("BoutManager").GetComponent<BoutManager> ().manualStart ();
			player1 = true;
			GameObject[] blocks = GameObject.FindGameObjectsWithTag ("Block");
			for (int x = 0; x < blocks.GetLength (0); x++) {
				if (blocks [x].GetComponent<BlockData> ().playerOne) {
					blocks [x].transform.parent = this.gameObject.transform;
				}
			}
			blocks = GameObject.FindGameObjectsWithTag ("inHand");
			for (int x = 0; x < blocks.GetLength (0); x++) {
				if (blocks [x].GetComponent<BlockData> ().playerOne) {
					blocks [x].transform.parent = this.gameObject.transform;
				}
			}
			GameObject.Find("HandManager").transform.parent = this.gameObject.transform;
		} else {
			player1 = false;
			//GameObject.Find ("BoutManager").GetComponent<BoutManager> ().manualStart ();
			GameObject[] blocks = GameObject.FindGameObjectsWithTag ("Block");
			for (int x = 0; x < blocks.GetLength (0); x++) {
				if (!blocks [x].GetComponent<BlockData> ().playerOne) {
					blocks [x].transform.parent = this.gameObject.transform;
				}
			}
			blocks = GameObject.FindGameObjectsWithTag ("inHand");
			for (int x = 0; x < blocks.GetLength (0); x++) {
				if (!blocks [x].GetComponent<BlockData> ().playerOne) {
					blocks [x].transform.parent = this.gameObject.transform;
				}
			}
			GameObject.Find("HandManager (1)").transform.parent = this.gameObject.transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
