using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutManager : MonoBehaviour {

	public GameObject block;
	public GameObject[,] blocks = new GameObject[12,9];

	public float boardWidth;
	public float xOffset = .15f;
	public float yOffset = .15f;
	private float spawnHeight = 1f;

	public List<Material> textures;

	// Use this for initialization
	void Start () {
		/*for (int x = 0; x < 10; x++) {
			createBlock ();
		}*/
		populatePile ();
		printGrid ();
	}
	
	// Update is called once per frame
	void Update () {
		isMarked ();
	}

	public void createBlock(int xPos, int yPos){
		GameObject newBlock = Instantiate (block);
		newBlock.GetComponent<Renderer> ().material = textures[Random.Range(0, textures.Count)];
		//Vector3 spawnPoint = new Vector3 (Mathf.Floor (Random.value * boardWidth - (boardWidth/2f)), Mathf.Floor (Random.value * 10f) + spawnHeight++, 0f);
		Vector3 spawnPoint = new Vector3 (xPos - (boardWidth/2f), yPos + spawnHeight + (yPos*yOffset), 0f);
		//Debug.Log (spawnPoint.y);
		if (spawnPoint.x > 0f) {
			spawnPoint.x += (xOffset*spawnPoint.x);
		} else if (spawnPoint.x < 0f) {
			spawnPoint.x -= (xOffset*Mathf.Abs(spawnPoint.x));
		}
		newBlock.transform.position = spawnPoint;
		newBlock.GetComponent<BlockData> ().gridCoord = new Vector2 (xPos, yPos);
		//Debug.Log (newBlock.GetComponent<BlockData> ().spawnY);
		//Debug.Log ("X: " + xPos + "Y: " + yPos);
		blocks [yPos, xPos] = newBlock;
	}

	/*Populates the board by columns. It starts at the bottom of a column and randomly
	generateds a number. if it's greater than .5f, then a block is spawned, then it moves
	to the next row above that slot. If it's less than .5f then it breaks out of that
	row and moves onto the next one. */
	public void populatePile(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < 5; y++) {
				//.25f
				if (Random.value > .25f) {
					createBlock (x, y);
					//StartCoroutine (waiting (x, y));
				} else {
					break;
				}
			}
		}
	}
		
	IEnumerator waiting(int x, int y){
		yield return new WaitForSeconds (1 * y);
		createBlock (x, y);
	}

	void OnMouseDown(){
		
	}

	/*Prints a text representation of the 2D array to the console. Used for debugging*/
	public void printGrid(){
		string grid = "";
		for (int y = blocks.GetLength (0)-1; y >= 0; y--) {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				if (blocks [y, x] != null) {
					grid += 'X';
				} else {
					grid += 'O';
				}
			}
			grid += "\n";
		}
		Debug.Log (grid);
	}

	public void isMarked(){
		for (int y = blocks.GetLength (0)-1; y >= 0; y--) {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				if (blocks [y, x] != null && blocks[y,x].GetComponent<BlockData>().marked) {
					//Debug.Log ("Before: " + blocks [y, x]);
					Destroy (blocks [y, x].gameObject);
					blocks [y, x] = null;
					//Debug.Log ("After: " + blocks [y, x]);
					printGrid ();
				} else {
				}
			}
		}
	}
}
