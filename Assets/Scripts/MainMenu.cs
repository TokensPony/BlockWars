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
		this.GetComponent<AudioSource> ().Play ();
		loading.gameObject.SetActive (true);
		switch (input) {
		case 0:
			
			SceneManager.LoadScene ("Local2Player");
			break;
		case 1:
			SceneManager.LoadScene ("Network");
			break;
		default:
			break;
		}
	}

	public void startAIGame(int diff){
		this.GetComponent<AudioSource> ().Play ();
		PlayerPrefs.SetInt ("diffLev", diff);
		loading.gameObject.SetActive (true);
		SceneManager.LoadScene("AIScene");
	}

	public void levelMenu(bool op){
		this.GetComponent<AudioSource> ().Play ();
		levelSelect.gameObject.SetActive (op);
	}
}
