using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AIOpponent : MonoBehaviour {

	public GameObject handObject;
	HandManager aihand;

	public GameObject boardMan;
	GameObject[,] board;

	public float moveDelay;

	public bool waiting;

	// Use this for initialization
	void Start () {
		aihand = handObject.GetComponent<HandManager> ();
		waiting = false;
		//board = boardMan.GetComponent<BoutManager> ().blocks;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T)){
			//Debug.Log ("Pressed");
			//aihand.hand[0].GetComponent<BlockData>().dragBlock(new Vector2(4f, 8f), false);
			//aihand.hand[0].GetComponent<BlockData> ().release ();
			placeBlock();
		}
		if (boardMan.GetComponent<BoutManager> ().p1Turn && !waiting) {
			StartCoroutine (AIDelay ());
		}
	}

	private IEnumerator AIDelay(){
		waiting = true;
		yield return new WaitForSecondsRealtime (1f);
		placeBlock ();
		waiting = false;
	}

	public void placeBlock(){
		Debug.Log ("Place Block");
		GameObject[,] board = boardMan.GetComponent<BoutManager> ().blocks;

		for (int h = 0; h < aihand.GetComponent<HandManager> ().hand.Count; h++) {
			Material startMat = aihand.hand [h].GetComponent<Renderer> ().sharedMaterial;
			for (int x = 0; x < board.GetLength (1); x++) {
				for (int y = 0; y < board.GetLength (0) - 1; y++) {
					if (board [y, x] != null && y != board.GetLength (1) - 1 && board [y + 1, x] == null &&
					    board [y, x].GetComponent<Renderer> ().sharedMaterial == startMat) {
						Debug.Log ("Match Found");
						Vector3 temp = board [y, x].transform.position;
						temp.x = (temp.x < 0) ? Mathf.Ceil (temp.x) : Mathf.Floor (temp.x);
						temp.y += 2f;
						aihand.hand [0].GetComponent<BlockData> ().dragBlock (temp, false);
						aihand.hand [0].GetComponent<BlockData> ().release ();
						break;
						//return;
					}
				}
			}
		}
	}
}
