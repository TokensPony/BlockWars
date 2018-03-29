using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class BarSpawner : NetworkBehaviour {

	public GameObject bar;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public override void OnStartServer(){
		var newBar = (GameObject)Instantiate (bar);
		NetworkServer.Spawn (newBar);
	}

}
