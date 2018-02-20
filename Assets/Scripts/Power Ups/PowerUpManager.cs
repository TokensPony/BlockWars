using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

	public GameObject currentPowUp;
	public List<GameObject> pUpList;

	public Vector3 powPos;

	void Start () {
		currentPowUp = pUpList [0];
		initializePowUp ();
		//currentPowUp = Instantiate (currentPowUp);
		//currentPowUp.transform.position = powPos;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void initializePowUp(){
		currentPowUp = Instantiate (currentPowUp);
		currentPowUp.transform.position = powPos;
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
