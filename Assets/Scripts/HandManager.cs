﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HandManager : NetworkBehaviour {

	public List<GameObject> hand;
	public GameObject block;
	public int handSize = 5;
	public List<Material> hackerTextures;
	public List<Material> securityTextures;
	public List<string> colorNames;

	public bool p1;
	public bool handLocked;

	void Start () {
		//textures = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().textures;
		block = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().block;
		//colorNames = GameObject.Find ("BoutManager").GetComponent<BoutManager> ().colorNames;
		handLocked = false;
		//populateHand ();
	}
	
	// Update is called once per frame
	void Update () {
		if (hand.Count != 0) {
			for (int x = 0; x < handSize; x++) {
				if (hand [x].tag == "Block") {
					hand [x] = drawBlock (x + (Mathf.Abs (x * .15f)));
				}
			}
		}
	}

	public GameObject drawBlock(float xPos){
		//Debug.Log ("HM Parent: " + this.gameObject.transform.root);
		GameObject newBlock = Instantiate (block);
		int randIndex = Random.Range (0, 5);
		newBlock.GetComponent<Renderer> ().material = (p1)? hackerTextures[randIndex] : securityTextures[randIndex];
		newBlock.GetComponent<BlockData> ().color = randIndex;
		newBlock.GetComponent<BlockData> ().sColor = colorNames [randIndex];
		//newBlock.GetComponent<Rigidbody> ().useGravity = false;
		newBlock.tag = "inHand";
		Vector3 finalPos = (p1) ? new Vector3 (xPos, -1f, 0) : new Vector3 (-xPos, 32f, 0);
		newBlock.GetComponent<BlockData> ().handPos = finalPos;
		newBlock.transform.position = finalPos;
		//newBlock.transform.SetParent (this.gameObject.transform.root);
		//newBlock.GetComponent<BlockData> ().cForceActive = false;
		newBlock.GetComponent<ConstantForce>().enabled = false;
		newBlock.GetComponent<BlockData> ().playerOne = p1;
		if (!p1) {
			//newBlock.GetComponent<BlockData> ().cForce *= -1f;
			newBlock.GetComponent<ConstantForce>().force *= -1f;
		}
		newBlock.GetComponent<BlockData> ().handM = this.gameObject;
		if (string.Equals (SceneManager.GetActiveScene().name, "Network") && NetworkServer.active) {
			//Debug.Log ("Server Spawn");
			NetworkServer.Spawn (newBlock);
		}
		return newBlock;
	}

	public void populateHand(){
		for (int x = 0; x < handSize; x++){
			hand.Add(drawBlock (x + (Mathf.Abs(x * .15f))));
		}
	}
}
