using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour {

	public GameObject block;
	public GameObject[,] blocks = new GameObject[32,9];
	[SyncVar]
	public bool player1;

	//Hand variables
	public List<GameObject> hand;// = new List<GameObject>(5);
	public bool handLocked;

	public GameObject bar;
	//public GameObject handManager;

	public GameObject boutUI;

	public float boardWidth;
	public float xOffset = .15f;
	public float yOffset = .15f;
	private float spawnHeight = .56f;

	public int minMatch;
	public float minForce;
	public int boostCount;
	public float boostBase;
	public int maxPile;

	public List<Material> textures;
	public List<string> colorNames;

	private int matchCount;
	private int turnCount;
	// Use this for initialization
	void Start () {
		if (!isLocalPlayer) {
			return;
		}
		//hand = new List<GameObject>(5);
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		player1 = (players.GetLength (0) == 1) ? true : false;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraControls> ().setCamera (player1);

		turnCount = 0;
		Debug.Log (Network.player.ipAddress);
		StartCoroutine (waitForSecondPlayer ());
	}

	IEnumerator waitForSecondPlayer(){
		while(NetworkManager.singleton.numPlayers < 2 && NetworkManager.singleton.numPlayers != 0){

			//Debug.Log (NetworkManager.singleton.numPlayers);
			yield return null;
		}
		/*if (player1) {
			bar = Instantiate (bar);
			NetworkServer.Spawn (bar);
		} else {
			//Debug.Log
			bar = GameObject.FindGameObjectWithTag ("Finish");
		}*/
		//bar = GameObject.FindGameObjectWithTag ("Finish");
		//Debug.Log ("Found Second Player");
		populatePile ();
		CmdCreateHand ();
		//clearStart ();
		printGrid ();
	}

	// Update is called once per frame
	void Update () {
		if (!isLocalPlayer) {
			return;
		}

		if (hand.Count != 0) {
			for (int x = 0; x < 5; x++) {
				if (hand[x] != null && hand [x].tag == "Block") {
					hand [x] = null;
					drawBlock (x);
				}
			}
		}
	}

	public void drawBlock(int x){
		CmdCreateBlock (0,0, player1, true, x);
	}

	[Command]
	public void CmdCreateHand(){
		for (int x = 0; x < 5; x++){
			CmdCreateBlock (0, 0, player1, true, x);
		}
	}

	[Command]
	public void CmdCreateBlock(int xPos, int yPos, bool p1, bool drawnBlock, int hPosX){
		int randIndex = Random.Range (0, textures.Count);
		Vector3 spawnPoint;// = new Vector3 (xPos - (boardWidth/2f), 0f, 0f);
		//Debug.Log (spawnPoint.x);
		if (!drawnBlock) {
			spawnPoint = new Vector3 (xPos - (boardWidth/2f), 0f, 0f);
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
		} else {
			float tempHpos = hPosX + (Mathf.Abs (hPosX * .15f));
			spawnPoint = (p1)? new Vector3 (tempHpos, -1f, 0) : new Vector3 (-tempHpos, 32f, 0);
		}
		//newBlock.transform.position = spawnPoint;
		//Debug.Log (spawnPoint);
		var newBlock = (GameObject)Instantiate (
			block,
			spawnPoint,
			block.transform.rotation);
		newBlock.GetComponent<NetBlockData> ().handPos = (drawnBlock) ? new Vector2(spawnPoint.x, spawnPoint.y) : Vector2.zero;
		newBlock.GetComponent<NetBlockData> ().playerOne = player1;
		newBlock.GetComponent<NetBlockData> ().gridCoord = (!drawnBlock)? new Vector2 (xPos, yPos): Vector2.zero;
		newBlock.GetComponent<ConstantForce>().force *= (player1) ? 1f : -1f;
		newBlock.tag = (drawnBlock) ? "inHand" : "Block";
		newBlock.GetComponent<ConstantForce> ().enabled = !drawnBlock;
		newBlock.GetComponent<Renderer> ().material = textures[randIndex];
		newBlock.transform.parent = this.gameObject.transform;
		Debug.Log (yPos + ", " + xPos);
		if (!drawnBlock) {
			blocks [yPos, xPos] = newBlock;
		} else {
			if (hand.Count < 5) {
				hand.Add (newBlock);
			} else {
				hand [hPosX] = newBlock;
			}
		}
		NetworkServer.Spawn (newBlock);
		RpcCreateBlock (newBlock, newBlock.transform.parent.gameObject,
			newBlock.transform.localPosition, newBlock.transform.localRotation,
			newBlock.GetComponent<ConstantForce>().force, randIndex, xPos, yPos, drawnBlock, hPosX);
	}

	[ClientRpc]
	public void RpcCreateBlock(GameObject nb, GameObject parent, Vector3 localPos, Quaternion r, Vector3 cForce, int matIndex, int xPos, int yPos, bool drawnBlock, int hPosX){
		nb.GetComponent<NetBlockData> ().handPos = (drawnBlock) ? new Vector2(localPos.x, localPos.y): Vector2.zero;
		nb.transform.parent = parent.transform;
		nb.transform.localPosition = localPos;
		nb.transform.rotation = r;
		nb.GetComponent<ConstantForce>().force = cForce;
		nb.GetComponent<ConstantForce> ().enabled = !drawnBlock;
		nb.tag = (drawnBlock) ? "inHand" : "Block";
		nb.GetComponent<NetBlockData> ().playerOne = player1;
		nb.GetComponent<Renderer> ().material = textures[matIndex];
		if (!drawnBlock) {
			blocks [yPos, xPos] = nb;
		} else {
			if (hand.Count < 5) {
				hand.Add (nb);
			} else {
				hand [hPosX] = nb;
			}
		}
	}

	public void addBlock(GameObject dropBlock){
		int xPos = Mathf.RoundToInt(dropBlock.GetComponent<NetBlockData> ().gridCoord.x);
		if (player1) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, xPos] == null) {
					if (y > 0 && player1 
						&& dropBlock.transform.position.y < blocks [y - 1, xPos].transform.position.y + 1f
					){
						dropBlock.transform.position = blocks [y - 1, xPos].transform.position + new Vector3 (0, 1.1f, 0);
					}
					dropBlock.GetComponent<NetBlockData> ().gridCoord.y = y;
					blocks [y, xPos] = dropBlock;
					break;
				}
			}
		} else {
			for (int y = blocks.GetLength (0) - 1; y >= 0; y--) {
				if (blocks [y, xPos] == null) {
					if (y < 31 && !player1 
						&& dropBlock.transform.position.y > blocks [y + 1, xPos].transform.position.y - 1f
					) {
						dropBlock.transform.position = blocks [y + 1, xPos].transform.position - new Vector3 (0, 1.1f, 0);
					}
					dropBlock.GetComponent<NetBlockData> ().gridCoord.y = y;
					blocks [y, xPos] = dropBlock;
					break;
				}
			}
		}
	}

	public void populatePile(){
		if (player1) {	
			for (int x = 0; x < blocks.GetLength (1); x++) {
				for (int y = 0; y < maxPile; y++) {
					//.25f
					if (Random.value > .1f) {
						CmdCreateBlock (x, y, player1, false, 0);
						//StartCoroutine (waiting (x, y));
					} else {
						break;
					}
				}
			}
		} else {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				for (int y = blocks.GetLength (0) - 1; y >= blocks.GetLength (0) - maxPile; y--) {
					if (Random.value > .1f) {
						CmdCreateBlock (x, y, false, false, 0);
						//StartCoroutine (waiting (x, y));
					} else {
						break;
					}
				}
			}
		}
	}

	/*Prints a text representation of the 2D array to the console. Used for debugging*/
	public void printGrid(){
		string grid = "";
		for (int y = blocks.GetLength (0)-1; y >= 0; y--) {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				if (blocks [y, x] != null) {
					grid += 'X';
				} else {
					grid += 'O';
				}
			}
			grid += "\n";
		}
		Debug.Log (grid);
	}

	public void clearStart(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null) {
					matchCount = 1;
					boostCount = 0;
					if (matchMade (blocks [y, x]) && matchCount >= minMatch) {
						removeMarked (false);
					} else {
						matchCount = 1;
						//blocks [y, x].GetComponent<BlockData> ().marked = false;
						unmark();
					}
				}
			}
		}
	}

	/*Checks if any blocks in the grid have been "marked". This is being used as a temporary
	means of triggering the match search.*/
	public void isMarked(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null && blocks [y, x].GetComponent<NetBlockData> ().marked) {
					matchCount = 1;
					boostCount = 0;
					if (matchMade (blocks [y, x]) && matchCount >= minMatch) {
						//if (matchMade (blocks [y, x])) {
						/*if (ai != null) {
							Debug.Log ("Have AI");
							ai.GetComponent<AIOpponent> ().setHMC (matchCount-1);
						}*/
						boostCount = (matchCount > minMatch)?matchCount-minMatch:0;
						removeMarked (true);
					} else {
						matchCount = 1;
						boostCount = 0;
						unmark();
					}
				}
			}
		}
	}

	[Command]
	void CmdSetAuthority (NetworkIdentity grabID, NetworkIdentity playerID){
		grabID.AssignClientAuthority (playerID.connectionToClient);
	}

	[Command]
	void CmdRemoveAuthority (NetworkIdentity grabID, NetworkIdentity playerID){
		grabID.RemoveClientAuthority (playerID.connectionToClient);
	}

	[Command]
	void CmdPush(float forceApplied, bool player1){
		bar.GetComponent<NetBarScript> ().pushAway (forceApplied, player1);
	}

	[Command]
	void CmdDestroy(GameObject tBD){
		NetworkServer.Destroy (tBD);
	}


	public void removeMarked(bool inGame){
		int tempColor = 0;
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null && blocks[y,x].GetComponent<NetBlockData>().marked) {
					tempColor = blocks [y, x].GetComponent<NetBlockData> ().color;
					//NetworkServer.Destroy (blocks [y, x].gameObject);
					CmdDestroy(blocks [y, x].gameObject);
					blocks [y, x] = null;
				}
			}
		}
		collapseBoard ();
		if (inGame) {
			Debug.Log ("Boosted: " + (minForce + (boostCount * boostBase)));
			float forceApplied = minForce + (boostCount * boostBase);
			forceApplied *= (player1) ? 1f : -1f;
			//bar = GameObject.FindGameObjectWithTag ("Finish");
			//bar = GameObject.Find("NetBar(Clone)");
			//CmdSetAuthority (bar.GetComponent<NetworkIdentity> (), this.GetComponent<NetworkIdentity> ());
			Debug.Log ("PUSHED");
			//bar.GetComponent<NetBarScript> ().pushAway (forceApplied, player1);
			CmdPush(forceApplied, player1);
			//CmdRemoveAuthority (bar.GetComponent<NetworkIdentity> (), this.GetComponent<NetworkIdentity> ());
			//GameObject.Find ("PowerUpManager").GetComponent<PowerUpManager> ().addPowerUp (tempColor);
			GameObject[] managers;
			managers = GameObject.FindGameObjectsWithTag ("PowerUpManager");
			/*foreach (GameObject man in managers) {
				if (man.GetComponent<PowerUpManager> ().playerOne == p1Turn) {
					man.GetComponent<PowerUpManager> ().addPowerUp (tempColor);
					break;
				}
			}*/
			//p1Turn = !p1Turn;
			if (++turnCount % 4 == 0) {
				//bar.GetComponent<BarScript> ().increaseSpeed ();
				addRows ();
				//recolor ();
			}
		}
	}

	/*Readjusts the positions of the objects in the Grid after a move has been made.*/
	public void collapseBoard(){
		GameObject[,] temp = new GameObject[32,9];
		int yIndex = 0;
		if (player1) {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				yIndex = 0;
				for (int y = 0; y < blocks.GetLength (0); y++) {
					if (blocks [y, x] != null) {
						if (blocks [y, x].GetComponent<NetBlockData> ().playerOne) {
							temp [yIndex, x] = blocks [y, x];
							temp [yIndex, x].GetComponent<NetBlockData> ().gridCoord = new Vector2 (x, yIndex);
						} else {
							temp [y, x] = blocks [y, x];
						}
						yIndex++;
					}
				}
			}
		} else {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				yIndex = blocks.GetLength (0)-1;
				for (int y = blocks.GetLength (0)-1; y >= 0; y--) {
					if (blocks [y, x] != null) {
						if (!blocks [y, x].GetComponent<NetBlockData> ().playerOne) {
							temp [yIndex, x] = blocks [y, x];
							temp [yIndex, x].GetComponent<NetBlockData> ().gridCoord = new Vector2 (x, yIndex);
						} else {
							temp [y, x] = blocks [y, x];
						}
						yIndex--;
					}
				}
			}
		}


		blocks = temp;
		printGrid ();
	}

	public bool matchMade(GameObject startBlock){
		Material startMat = startBlock.GetComponent<Renderer>().sharedMaterial;
		//Debug.Log (startMat);
		startBlock.GetComponent<NetBlockData> ().marked = true;
		Vector2 pos = startBlock.GetComponent<NetBlockData> ().gridCoord;
		bool found = false;
		/*Down*/
		if (pos.y > 0f &&
			blocks [(int)pos.y - 1, (int)pos.x] != null &&
			startMat == blocks [(int)pos.y - 1, (int)pos.x].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y - 1, (int)pos.x].GetComponent<NetBlockData> ().marked) {
			//Debug.Log ("Matched");
			matchMade (blocks [(int)pos.y - 1, (int)pos.x]);
			found = true;
			matchCount++;
		}

		/*Left*/
		if(pos.x > 0f &&
			blocks [(int)pos.y, (int)pos.x - 1] != null &&
			startMat == blocks [(int)pos.y, (int)pos.x-1].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y, (int)pos.x-1].GetComponent<NetBlockData> ().marked){
			matchMade (blocks [(int)pos.y, (int)pos.x-1]);
			found = true;
			matchCount++;
		}

		/*Up*/
		if (pos.y < blocks.GetLength(0)-1 &&
			blocks [(int)pos.y + 1, (int)pos.x] != null &&
			startMat == blocks [(int)pos.y + 1, (int)pos.x].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y + 1, (int)pos.x].GetComponent<NetBlockData> ().marked) {
			//Debug.Log ("Matched");
			matchMade (blocks [(int)pos.y + 1, (int)pos.x]);
			found = true;
			matchCount++;
		}

		/*Right*/
		if(pos.x < blocks.GetLength(1)-1 &&
			blocks [(int)pos.y, (int)pos.x + 1] != null &&
			startMat == blocks [(int)pos.y, (int)pos.x+1].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y, (int)pos.x+1].GetComponent<NetBlockData> ().marked){
			matchMade (blocks [(int)pos.y, (int)pos.x+1]);
			found = true;
			matchCount++;
		}

		return found;
	}

	public void findBlock(GameObject bl){
		Debug.Log ("Trying to Find Block");
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {

				if (blocks[y,x] != null && bl.GetInstanceID () == blocks [y, x].GetInstanceID()) {
					bl.GetComponent<NetBlockData> ().marked = true;
					isMarked ();
				}
			}
		}
	}

	public void unmark(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null) {
					blocks [y, x].GetComponent<NetBlockData> ().marked = false;
				}
			}
		}
	}

	public void addRows(){
		GameObject[,] newGrid = new GameObject[32,9];
		GameObject[,] tempGrid = blocks;
		for (int x = 0; x < tempGrid.GetLength (1); x++) {
			for (int y = 0; y < tempGrid.GetLength (0); y++) {
				if (tempGrid [y, x] != null) {
					Vector3 temp = tempGrid [y, x].transform.position;
					temp.y += (tempGrid[y,x].GetComponent<NetBlockData>().playerOne)? 1.2f:-1.3f;
					Vector2 coord = tempGrid [y, x].GetComponent<NetBlockData> ().gridCoord;
					coord.y += (tempGrid[y,x].GetComponent<NetBlockData>().playerOne)? 1: -1;
					tempGrid [y, x].GetComponent<NetBlockData> ().gridCoord = coord;
					tempGrid [y, x].transform.position = temp;
					if (tempGrid [y, x].GetComponent<NetBlockData> ().playerOne && y < 31) {
						Debug.Log (y);
						newGrid [y + 1, x] = tempGrid [y, x];
					} else {
						newGrid [y - 1, x] = tempGrid [y, x];
					}
				}
			}

			blocks = newGrid;
			if (player1) {
				//NetworkServer.Destroy (blocks [0, x]);
				CmdDestroy(blocks[0, x].gameObject);
				blocks [0, x] = null;
				CmdCreateBlock (x, 0, true, false, 0);
			} else {
				//NetworkServer.Destroy (blocks [31, x]);
				CmdDestroy(blocks[31, x].gameObject);
				blocks [31, x] = null;
				//Destroy (blocks [31, x]);
				//blocks [31, x] = null;
				CmdCreateBlock (x, 31, false, false, 0);
			}
		}
		printGrid ();
	}

	public void restart(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void recolor(){
		if (!isLocalPlayer) {
			return;
		}

		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null) {
					int randIndex = Random.Range (0, textures.Count);
					blocks[y,x].GetComponent<Renderer> ().material = textures[randIndex];
					blocks[y,x].GetComponent<NetBlockData> ().color = randIndex;
				}
			}
		}
	}
	

}
