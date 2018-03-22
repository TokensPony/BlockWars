using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class NetworkControls : NetworkManager{

	public override void OnServerConnect(NetworkConnection connection){
		Debug.Log ("Client Connected");
		GameObject.Find ("BoutManager").GetComponent<BoutManager> ().manualStart ();
	}
}
