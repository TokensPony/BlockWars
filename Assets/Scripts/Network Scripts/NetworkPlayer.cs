using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkPlayer : NetworkBehaviour {

	public GameObject block;
	public GameObject[,] blocks = new GameObject[32,9];
	public bool player1;

	public GameObject bar;
	public GameObject handManager;

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
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		player1 = (players.GetLength (0) == 1) ? true : false;
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraControls> ().setCamera (player1);
		turnCount = 0;
		populatePile ();


	}

	[Command]
	public void CmdCreateBlock(int xPos, int yPos, bool p1){
		int randIndex = Random.Range (0, textures.Count);
		//newBlock.GetComponent<Renderer> ().material = textures[randIndex];
		//newBlock.GetComponent<BlockData> ().color = randIndex;
		//Vector3 spawnPoint = new Vector3 (Mathf.Floor (Random.value * boardWidth - (boardWidth/2f)), Mathf.Floor (Random.value * 10f) + spawnHeight++, 0f);
		Vector3 spawnPoint = new Vector3 (xPos - (boardWidth/2f), 0f, 0f);
		Debug.Log (spawnPoint.x);
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
		//newBlock.transform.position = spawnPoint;
		//Debug.Log (spawnPoint);
		var newBlock = (GameObject)Instantiate (
			block,
			spawnPoint,
			block.transform.rotation);
		newBlock.GetComponent<NetBlockData> ().playerOne = player1;
		newBlock.GetComponent<NetBlockData> ().gridCoord = new Vector2 (xPos, yPos);
		newBlock.GetComponent<ConstantForce>().force *= (player1) ? 1f : -1f;
		newBlock.GetComponent<Renderer> ().material = textures[randIndex];
		newBlock.transform.parent = this.gameObject.transform;
		blocks [yPos, xPos] = newBlock;
		NetworkServer.Spawn (newBlock);
		RpcCreateBlock (newBlock, newBlock.transform.parent.gameObject,
			newBlock.transform.localPosition, newBlock.transform.localRotation,
			newBlock.GetComponent<ConstantForce>().force, randIndex);
	}

	[ClientRpc]
	public void RpcCreateBlock(GameObject nb, GameObject parent, Vector3 localPos, Quaternion r, Vector3 cForce, int matIndex){
		nb.transform.parent = parent.transform;
		nb.transform.localPosition = localPos;
		nb.transform.rotation = r;
		nb.GetComponent<ConstantForce>().force = cForce;
		nb.GetComponent<NetBlockData> ().playerOne = player1;
		nb.GetComponent<Renderer> ().material = textures[matIndex];
	}

	public void populatePile(){
		if (player1) {	
			for (int x = 0; x < blocks.GetLength (1); x++) {
				for (int y = 0; y < maxPile; y++) {
					//.25f
					if (Random.value > .1f) {
						CmdCreateBlock (x, y, true);
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
						CmdCreateBlock (x, y, false);
						//StartCoroutine (waiting (x, y));
					} else {
						break;
					}
				}
			}
		}
	}

	public bool matchMade(GameObject startBlock){
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
			matchMade (blocks [(int)pos.y - 1, (int)pos.x]);
			found = true;
			matchCount++;
		}

		/*Left*/
		if(pos.x > 0f &&
			blocks [(int)pos.y, (int)pos.x - 1] != null &&
			startMat == blocks [(int)pos.y, (int)pos.x-1].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y, (int)pos.x-1].GetComponent<BlockData> ().marked){
			matchMade (blocks [(int)pos.y, (int)pos.x-1]);
			found = true;
			matchCount++;
		}

		/*Up*/
		if (pos.y < blocks.GetLength(0)-1 &&
			blocks [(int)pos.y + 1, (int)pos.x] != null &&
			startMat == blocks [(int)pos.y + 1, (int)pos.x].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y + 1, (int)pos.x].GetComponent<BlockData> ().marked) {
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
			matchMade (blocks [(int)pos.y, (int)pos.x+1]);
			found = true;
			matchCount++;
		}

		return found;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
