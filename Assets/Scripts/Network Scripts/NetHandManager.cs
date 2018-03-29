using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetHandManager : NetworkBehaviour {

	public GameObject[] hand = new GameObject[5];
	public GameObject block;
	public int handSize = 5;
	public List<Material> textures;
	public List<string> colorNames;

	public bool p1;
	public bool handLocked;

	void Start () {
		//hand = new List<GameObject> (handSize);
		//textures = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().textures;
		//block = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().block;
		//colorNames = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().colorNames;
		handLocked = false;
		populateHand ();
	}

	// Update is called once per frame
	void Update () {
		//if (hand.Count != 0) {
			for (int x = 0; x < handSize; x++) {
				if (hand [x].tag == "Block") {
					CmdDrawBlock (x + (Mathf.Abs (x * .15f)), x);
				}
			}
		//}
	}

	[Command]
	public void CmdDrawBlock(float xPos, int index){
		Debug.Log ("I'M HERE YO!");
		//Debug.Log ("HM Parent: " + this.gameObject.transform.root);
		GameObject newBlock = Instantiate (block);
		int randIndex = Random.Range (0, textures.Count);
		newBlock.GetComponent<Renderer> ().material = textures[randIndex];
		newBlock.GetComponent<NetBlockData> ().color = randIndex;
		//newBlock.GetComponent<Rigidbody> ().useGravity = false;
		newBlock.tag = "inHand";
		Vector3 finalPos = (p1) ? new Vector3 (xPos, -1f, 0) : new Vector3 (-xPos, 32f, 0);
		newBlock.GetComponent<NetBlockData> ().handPos = finalPos;
		newBlock.transform.position = finalPos;
		newBlock.transform.SetParent (this.gameObject.transform.root);
		//newBlock.GetComponent<BlockData> ().cForceActive = false;
		newBlock.GetComponent<ConstantForce>().enabled = false;
		newBlock.GetComponent<NetBlockData> ().playerOne = p1;
		if (!p1) {
			newBlock.GetComponent<ConstantForce>().force *= -1f;
		}
		newBlock.GetComponent<NetBlockData> ().handM = this.gameObject;
		hand [index] = newBlock;
		NetworkServer.Spawn (newBlock);
		RpcDrawBlock (newBlock, newBlock.transform.parent.gameObject,
			newBlock.transform.localPosition, newBlock.transform.localRotation,
			newBlock.GetComponent<ConstantForce> ().force, randIndex);
	}

	[ClientRpc]
	public void RpcDrawBlock(GameObject nb, GameObject parent, Vector3 localPos, Quaternion r, Vector3 cForce, int matIndex){
		nb.transform.parent = parent.transform;
		nb.transform.localPosition = localPos;
		nb.transform.rotation = r;
		nb.GetComponent<ConstantForce>().force = cForce;
		nb.GetComponent<NetBlockData> ().playerOne = p1;
		nb.GetComponent<Renderer> ().material = textures[matIndex];
	}

	public void populateHand(){
		for (int x = 0; x < handSize; x++){
			CmdDrawBlock (x + (Mathf.Abs(x * .15f)), x);
		}
	}
}
