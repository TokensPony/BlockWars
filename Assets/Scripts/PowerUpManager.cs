using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour {

	public GameObject currentPowUp;
	public List<GameObject> pUpList;

	public Vector3 powPos;

	void Start () {
		currentPowUp = Instantiate (currentPowUp);
		currentPowUp.transform.position = powPos;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addPowerUp(string type){
		if (currentPowUp != null) {

		}
	}
}
