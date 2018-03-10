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
		if (boardMan.GetComponent<BoutManager> ().p1Turn && !waiting && !handObject.GetComponent<HandManager> ().handLocked) {
			StartCoroutine (AIDelay ());
		}
	}

	private IEnumerator AIDelay(){
		waiting = true;
		yield return new WaitForSecondsRealtime (1.5f);
		placeBlock ();
		waiting = false;
	}

	public bool placeBlock(){
		Debug.Log ("Place Block");
		GameObject[,] board = boardMan.GetComponent<BoutManager> ().blocks;

		for (int h = 0; h < aihand.hand.Count; h++) {
			Material startMat = aihand.hand [h].GetComponent<Renderer> ().sharedMaterial;
			for (int x = 0; x < board.GetLength (1); x++) {
				for (int y = 0; y < board.GetLength (0) - 1; y++) {
					if (board [y, x] == null && y != board.GetLength (1) - 1) {
						if (x - 1 >= 0 && board [y, x - 1] != null && board [y, x - 1].GetComponent<Renderer> ().sharedMaterial == startMat ||
						    y - 1 >= 0 && board [y - 1, x] != null && board [y - 1, x].GetComponent<Renderer> ().sharedMaterial == startMat ||
						    x + 1 < board.GetLength (1) - 1 && board [y, x + 1] != null && board [y, x + 1].GetComponent<Renderer> ().sharedMaterial == startMat) {
							Debug.Log (y + "," + x + "," + startMat);
							Vector3 temp = (y > 0) ? board [y - 1, x].transform.position : new Vector3 (x-4f, 0f, 0f);
							temp.x = (temp.x < 0) ? Mathf.Ceil (temp.x) : Mathf.Floor (temp.x);
							temp.y += 2f;
							aihand.hand [h].GetComponent<BlockData> ().dragBlock (temp, false);
							aihand.hand [h].GetComponent<BlockData> ().release ();
							//Debug.Log ("Match Found: " + startMat + ", " + board [y, x].GetComponent<Renderer> ().sharedMaterial);
							//break;
							return true;
						} else {
							break;
						}
					}
					/*if (board [y, x] != null && y != board.GetLength (1) - 1 && board [y + 1, x] == null &&
					    board [y, x].GetComponent<Renderer> ().sharedMaterial == startMat) {
						Vector3 temp = board [y, x].transform.position;
						temp.x = (temp.x < 0) ? Mathf.Ceil (temp.x) : Mathf.Floor (temp.x);
						temp.y += 2f;
						aihand.hand [h].GetComponent<BlockData> ().dragBlock (temp, false);
						aihand.hand [h].GetComponent<BlockData> ().release ();
						Debug.Log ("Match Found: " + startMat +", "+ board [y, x].GetComponent<Renderer> ().sharedMaterial);
						//break;
						return true;
					}*/
				}
			}
		}
		randomDrop (board);
		return false;
	}

	public void randomDrop(GameObject[,] board){
		int randX = Random.Range (0, board.GetLength (1) - 1);
		int randHand = Random.Range (0, aihand.hand.Count);

		for (int y = 0; y < board.GetLength (0) - 1; y++) {
			if (board [y, randX] != null && y != board.GetLength (1) - 1 && board [y + 1, randX] == null) {
				Vector3 temp = board [y, randX].transform.position;
				temp.x = (temp.x < 0) ? Mathf.Ceil (temp.x) : Mathf.Floor (temp.x);
				temp.y += 2f;
				aihand.hand [randHand].GetComponent<BlockData> ().dragBlock (temp, false);
				aihand.hand [randHand].GetComponent<BlockData> ().release ();
				//Debug.Log ("Match Found: " + startMat +", "+ board [y, x].GetComponent<Renderer> ().sharedMaterial);
				Debug.Log("Random Drop");
				break;
			}
		}
	}
}
