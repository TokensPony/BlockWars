using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

	public GameObject currentPowUp;
	public List<GameObject> pUpList;

	public Vector3 p1PowPos;
	public Vector3 p2PowPos;

	public bool playerOne;

	void Start () {
		currentPowUp = pUpList [1];
		initializePowUp ();
		//currentPowUp = Instantiate (currentPowUp);
		//currentPowUp.transform.position = powPos;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void initializePowUp(){
		currentPowUp = Instantiate (currentPowUp);
		currentPowUp.GetComponent<PowerUp> ().setPlayer (playerOne);
		currentPowUp.transform.position = (playerOne)? p1PowPos:p2PowPos;
	}

	public void addPowerUp(int type){
		Debug.Log ("Attempted Add: " + type);
		if (currentPowUp == null) {
			//Debug.Log ("Not Null");
			switch (type) {
			case 0:
			case 1:
				currentPowUp = pUpList [type];
				initializePowUp ();
				break;
			default:
				break;
			}
		}
	}
}
