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

	public int matchCount;

	// Use this for initialization
	void Start () {
		aihand = handObject.GetComponent<HandManager> ();
		waiting = false;
		//board = boardMan.GetComponent<BoutManager> ().blocks;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.T)){
			//placeBlock();
			greedyHill();
		}
		if (boardMan.GetComponent<BoutManager> ().p1Turn && !waiting && !handObject.GetComponent<HandManager> ().handLocked) {
			StartCoroutine (AIDelay ());
		}
	}

	private IEnumerator AIDelay(){
		waiting = true;
		yield return new WaitForSecondsRealtime (1.5f);
		//placeBlock ();
		greedyHill();
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

	public bool greedyHill(){
		Debug.Log ("Place Block");
		GameObject[,] board = boardMan.GetComponent<BoutManager> ().blocks;
		int tempHand = 0;
		Vector2 tempPos = Vector2.zero;
		int currentMax = 0;

		for (int h = 0; h < aihand.hand.Count; h++) {
			Material startMat = aihand.hand [h].GetComponent<Renderer> ().sharedMaterial;
			for (int x = 0; x < board.GetLength (1); x++) {
				for (int y = 0; y < board.GetLength (0) - 1; y++) {
					aihand.hand [h].GetComponent<BlockData> ().gridCoord = new Vector2 (x, y);
					if (board [y, x] == null && y != board.GetLength (1) - 1) {
						if (((x - 1 >= 0 && board [y, x - 1] != null && board [y, x - 1].GetComponent<Renderer> ().sharedMaterial == startMat) ||
							(y - 1 >= 0 && board [y - 1, x] != null && board [y - 1, x].GetComponent<Renderer> ().sharedMaterial == startMat) ||
							(x + 1 < board.GetLength (1) - 1 && board [y, x + 1] != null && board [y, x + 1].GetComponent<Renderer> ().sharedMaterial == startMat))){
							//Debug.Log (y + "," + x + "," + startMat);
							Debug.Log ("Match Count: " + matchCount);
							matchMade (aihand.hand [h]);
							boardMan.GetComponent<BoutManager> ().unmark ();
							if (matchCount > currentMax) {
								currentMax = matchCount;
								tempHand = h;
								tempPos = new Vector2 (x, y);
							}
							matchCount = 0;
							break;
							/*Vector3 temp = (y > 0) ? board [y - 1, x].transform.position : new Vector3 (x-4f, 0f, 0f);
							temp.x = (temp.x < 0) ? Mathf.Ceil (temp.x) : Mathf.Floor (temp.x);
							temp.y += 2f;
							aihand.hand [h].GetComponent<BlockData> ().dragBlock (temp, false);
							aihand.hand [h].GetComponent<BlockData> ().release ();
							return true;*/
						} else {
							matchCount = 0;
							break;
						}
					}
				}
			}
		}
		if (currentMax > 0) {
			Vector3 temp = (tempPos.y > 0f) ? board [(int)tempPos.y - 1, (int)tempPos.x].transform.position : new Vector3 (tempPos.x - 4f, 0f, 0f);
			temp.x = (temp.x < 0) ? Mathf.Ceil (temp.x) : Mathf.Floor (temp.x);
			temp.y += 2f;
			aihand.hand [tempHand].GetComponent<BlockData> ().dragBlock (temp, false);
			aihand.hand [tempHand].GetComponent<BlockData> ().release ();
			return true;
		}
		randomDrop (board);
		return false;
	}

	public bool matchMade(GameObject startBlock){
		Debug.Log ("Looked for match");
		GameObject[,] blocks = boardMan.GetComponent<BoutManager> ().blocks;
		Material startMat = startBlock.GetComponent<Renderer>().sharedMaterial;
		//Debug.Log (startMat);
		startBlock.GetComponent<BlockData> ().marked = true;
		Vector2 pos = startBlock.GetComponent<BlockData> ().gridCoord;
		bool found = false;
		/*Down*/
		if (pos.y > 0f &&
			blocks [(int)pos.y - 1, (int)pos.x] != null &&
			startMat == blocks [(int)pos.y - 1, (int)pos.x].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y - 1, (int)pos.x].GetComponent<BlockData> ().marked) {
			//Debug.Log ("Matched");
			Debug.Log("Down");
			matchMade (blocks [(int)pos.y - 1, (int)pos.x]);
			found = true;
			matchCount++;
		}

		/*Left*/
		if(pos.x > 0f &&
			blocks [(int)pos.y, (int)pos.x - 1] != null &&
			startMat == blocks [(int)pos.y, (int)pos.x-1].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y, (int)pos.x-1].GetComponent<BlockData> ().marked){
			Debug.Log("Left");
			matchMade (blocks [(int)pos.y, (int)pos.x-1]);
			found = true;
			matchCount++;
		}

		/*Up*/
		if (pos.y < blocks.GetLength(0)-1 &&
			blocks [(int)pos.y + 1, (int)pos.x] != null &&
			startMat == blocks [(int)pos.y + 1, (int)pos.x].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y + 1, (int)pos.x].GetComponent<BlockData> ().marked) {
			Debug.Log("Up");
			//Debug.Log ("Matched");
			matchMade (blocks [(int)pos.y + 1, (int)pos.x]);
			found = true;
			matchCount++;
		}

		/*Right*/
		if(pos.x < blocks.GetLength(1)-1 &&
			blocks [(int)pos.y, (int)pos.x + 1] != null &&
			startMat == blocks [(int)pos.y, (int)pos.x+1].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y, (int)pos.x+1].GetComponent<BlockData> ().marked){
			Debug.Log("Right");
			matchMade (blocks [(int)pos.y, (int)pos.x+1]);
			found = true;
			matchCount++;
		}

		return found;
	}
}
