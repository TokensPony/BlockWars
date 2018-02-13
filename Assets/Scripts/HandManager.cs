using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour {

	public List<GameObject> hand;
	public GameObject block;
	public int handSize = 5;
	public List<Material> textures;
	public List<string> colorNames;

	void Start () {
		textures = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().textures;
		block = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().block;
		colorNames = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().colorNames;
		populateHand ();
	}
	
	// Update is called once per frame
	void Update () {
		for (int x = 0; x < handSize; x++) {
			if (hand [x].tag == "Block") {
				hand [x] = drawBlock (x + (Mathf.Abs (x * .15f)));
			}
		}
	}

	public GameObject drawBlock(float xPos){
		GameObject newBlock = Instantiate (block);
		int randIndex = Random.Range (0, textures.Count);
		newBlock.GetComponent<Renderer> ().material = textures[randIndex];
		newBlock.GetComponent<BlockData> ().color = randIndex;
		newBlock.GetComponent<Rigidbody> ().useGravity = false;
		newBlock.tag = "inHand";
		newBlock.GetComponent<BlockData> ().handPos = new Vector3 (xPos, -1, 0);
		newBlock.transform.position = new Vector3 (xPos, -1f, 0);
		return newBlock;
	}

	public void populateHand(){
		for (int x = 0; x < handSize; x++){
			hand.Add(drawBlock (x + (Mathf.Abs(x * .15f))));
		}
	}
}
