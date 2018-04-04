using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public GameObject levelSelect;
	public GameObject loading;

	// Use this for initialization
	void Start () {
		Screen.fullScreen = false;
		Screen.orientation = ScreenOrientation.Portrait;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void startGame(int input){
		switch (input) {
		case 0:
			loading.gameObject.SetActive (true);
			SceneManager.LoadScene ("Local2Player");
			break;
		default:
			break;
		}
	}

	public void startAIGame(int diff){
		PlayerPrefs.SetInt ("diffLev", diff);
		loading.gameObject.SetActive (true);
		SceneManager.LoadScene("AIScene");
	}

	public void levelMenu(bool op){
		levelSelect.gameObject.SetActive (op);
	}
}
