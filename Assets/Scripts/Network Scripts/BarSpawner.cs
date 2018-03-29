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
		StartCoroutine (waitForSecondPlayer());
	}

	IEnumerator waitForSecondPlayer(){
		while(NetworkManager.singleton.numPlayers < 2){
			
			//Debug.Log (NetworkManager.singleton.numPlayers);
			yield return null;
		}
		SpawnBar ();
	}

	void SpawnBar(){
		var newBar = (GameObject)Instantiate (bar);
		NetworkServer.Spawn (newBar);
		//newBar.GetComponent<NetworkIdentity> ().AssignClientAuthority (Network.connections [0]);
	}

}
