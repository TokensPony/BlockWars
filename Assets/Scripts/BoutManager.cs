using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutManager : MonoBehaviour {

	public GameObject block;
	public GameObject[][] blocks;

	public float boardWidth;
	public float xOffset = .15f;
	private float spawnHeight = 1f;

	// Use this for initialization
	void Start () {
		for (int x = 0; x < 3; x++) {
			createBlock ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void createBlock(){
		Instantiate (block);
		Vector3 spawnPoint = new Vector3 (Mathf.Floor (Random.value * boardWidth - (boardWidth/2f)), Mathf.Floor (Random.value * 10f) + spawnHeight++, 0f);
		if (spawnPoint.x > 0f) {
			spawnPoint.x += (xOffset*(boardWidth/2));
		} else if (spawnPoint.x < 0f) {
			spawnPoint.x -= (xOffset*(boardWidth/2));
		}
		block.transform.position = spawnPoint;
	}


}
