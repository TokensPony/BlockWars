using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoutManager : MonoBehaviour {

	public GameObject block;
	public GameObject[,] blocks = new GameObject[32,9];
	//public GameObject[]
	public GameObject bar;

	public float boardWidth;
	public float xOffset = .15f;
	public float yOffset = .15f;
	private float spawnHeight = .56f;

	public int minMatch;
	public float minForce;
	public int boostCount;
	public float boostBase;
	public int maxPile;

	public List<Material> textures;
	public List<string> colorNames;

	private int matchCount;
	private int turnCount;

	public bool p1Turn;

	// Use this for initialization
	void Start () {
		Screen.fullScreen = false;
		Screen.orientation = ScreenOrientation.Portrait;
		p1Turn = true;
		turnCount = 0;
		//bar = this.
		populatePile ();
		clearStart ();
		printGrid ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			//addRows ();
			//recolor();
			clearStart();
			printGrid ();
		}
	}

	/*Adds a dropped block to the existing board by reading it's X coordinate and placing it
	into the first available Y coordinate.*/
	public void addBlock(GameObject dropBlock){
		int xPos = Mathf.RoundToInt(dropBlock.GetComponent<BlockData> ().gridCoord.x);
		if (p1Turn) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, xPos] == null) {
					if (y > 0 && p1Turn && dropBlock.transform.position.y < blocks [y - 1, xPos].transform.position.y + 1f) {
						dropBlock.transform.position = blocks [y - 1, xPos].transform.position + new Vector3 (0, 1, 0);
					}
					dropBlock.GetComponent<BlockData> ().gridCoord.y = y;
					blocks [y, xPos] = dropBlock;
					break;
				}
			}
		} else {
			for (int y = blocks.GetLength (0) - 1; y >= 0; y--) {
				if (blocks [y, xPos] == null) {
					if (y < 31 && !p1Turn && dropBlock.transform.position.y > blocks [y + 1, xPos].transform.position.y - 1f) {
						dropBlock.transform.position = blocks [y + 1, xPos].transform.position - new Vector3 (0, 1, 0);
					}
					dropBlock.GetComponent<BlockData> ().gridCoord.y = y;
					blocks [y, xPos] = dropBlock;
					break;
				}
			}
		}
	}

	public void createBlock(int xPos, int yPos, bool p1){
		GameObject newBlock = Instantiate (block);
		int randIndex = Random.Range (0, textures.Count);
		newBlock.GetComponent<Renderer> ().material = textures[randIndex];
		newBlock.GetComponent<BlockData> ().color = randIndex;
		//Vector3 spawnPoint = new Vector3 (Mathf.Floor (Random.value * boardWidth - (boardWidth/2f)), Mathf.Floor (Random.value * 10f) + spawnHeight++, 0f);
		Vector3 spawnPoint = new Vector3 (xPos - (boardWidth/2f), 0f, 0f);
		if (p1) {
			spawnPoint.y = yPos + spawnHeight + (yPos * yOffset);
			Debug.Log ("P1 Block: " + spawnPoint.y);
		} else {
			spawnPoint.y = (yPos - spawnHeight) - ((31 - yPos) * yOffset);
			Debug.Log ("P2 Block: " + spawnPoint.y);
		}
		//Debug.Log (spawnPoint.y);
		if (spawnPoint.x > 0f) {
			spawnPoint.x += (xOffset*spawnPoint.x);
		} else if (spawnPoint.x < 0f) {
			spawnPoint.x -= (xOffset*Mathf.Abs(spawnPoint.x));
		}
		newBlock.transform.position = spawnPoint;
		newBlock.GetComponent<BlockData> ().gridCoord = new Vector2 (xPos, yPos);
		newBlock.GetComponent<BlockData> ().playerOne = p1;
		blocks [yPos, xPos] = newBlock;
	}

	/*Populates the board by columns. It starts at the bottom of a column and randomly
	generateds a number. if it's greater than .1f, then a block is spawned, then it moves
	to the next row above that slot. If it's less than .5f then it breaks out of that
	row and moves onto the next one. */
	public void populatePile(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < maxPile; y++) {
				//.25f
				if (Random.value > .1f) {
					createBlock (x, y, true);
					//StartCoroutine (waiting (x, y));
				} else {
					break;
				}
			}
		}

		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = blocks.GetLength (0)-1; y >= blocks.GetLength (0)-maxPile; y--) {
				if (Random.value > .1f) {
					createBlock (x, y, false);
					//StartCoroutine (waiting (x, y));
				} else {
					break;
				}
			}
		}
	}
		
	/*IEnumerator waiting(int x, int y){
		yield return new WaitForSeconds (1 * y);
		createBlock (x, y);
	}*/

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

	public void clearStart(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null) {
					matchCount = 1;
					boostCount = 0;
					if (matchMade (blocks [y, x]) && matchCount >= minMatch) {
						removeMarked (false);
					} else {
						matchCount = 1;
						//blocks [y, x].GetComponent<BlockData> ().marked = false;
						unmark();
					}
				}
			}
		}
	}

	/*Checks if any blocks in the grid have been "marked". This is being used as a temporary
	means of triggering the match search.*/
	public void isMarked(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null && blocks [y, x].GetComponent<BlockData> ().marked) {
					matchCount = 1;
					boostCount = 0;
					if (matchMade (blocks [y, x]) && matchCount >= minMatch) {
						//if (matchMade (blocks [y, x])) {
						boostCount = (matchCount > minMatch)?matchCount-minMatch:0;
						removeMarked (true);
					} else {
						matchCount = 1;
						boostCount = 0;
						//blocks [y, x].GetComponent<BlockData> ().marked = false;
						unmark();
					}
				}
			}
		}
	}

	public void removeMarked(bool inGame){
		int tempColor = 0;
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null && blocks[y,x].GetComponent<BlockData>().marked) {
					tempColor = blocks [y, x].GetComponent<BlockData> ().color;
					Destroy (blocks [y, x].gameObject);
					blocks [y, x] = null;
				}
			}
		}
		collapseBoard ();
		if (inGame) {
			Debug.Log ("Boosted: " + (minForce + (boostCount * boostBase)));
			float forceApplied = minForce + (boostCount * boostBase);
			forceApplied *= (p1Turn) ? 1f : -1f;

			bar.GetComponent<BarScript> ().pushAway (forceApplied, p1Turn);
			//GameObject.Find ("PowerUpManager").GetComponent<PowerUpManager> ().addPowerUp (tempColor);
			GameObject[] managers;
			managers = GameObject.FindGameObjectsWithTag ("PowerUpManager");
			foreach (GameObject man in managers) {
				if (man.GetComponent<PowerUpManager> ().playerOne == p1Turn) {
					man.GetComponent<PowerUpManager> ().addPowerUp (tempColor);
					break;
				}
			}
			p1Turn = !p1Turn;
			if (++turnCount % 8 == 0) {
				bar.GetComponent<BarScript> ().increaseSpeed ();
				addRows ();
				recolor ();
			}
		}
	}

	/*Readjusts the positions of the objects in the Grid after a move has been made.*/
	public void collapseBoard(){
		GameObject[,] temp = new GameObject[32,9];
		int yIndex = 0;
		if (p1Turn) {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				yIndex = 0;
				for (int y = 0; y < blocks.GetLength (0); y++) {
					if (blocks [y, x] != null) {
						if (blocks [y, x].GetComponent<BlockData> ().playerOne) {
							temp [yIndex, x] = blocks [y, x];
							temp [yIndex, x].GetComponent<BlockData> ().gridCoord = new Vector2 (x, yIndex);
						} else {
							temp [y, x] = blocks [y, x];
						}
						yIndex++;
					}
				}
			}
		} else {
			for (int x = 0; x < blocks.GetLength (1); x++) {
				yIndex = blocks.GetLength (0)-1;
				for (int y = blocks.GetLength (0)-1; y >= 0; y--) {
					if (blocks [y, x] != null) {
						if (!blocks [y, x].GetComponent<BlockData> ().playerOne) {
							temp [yIndex, x] = blocks [y, x];
							temp [yIndex, x].GetComponent<BlockData> ().gridCoord = new Vector2 (x, yIndex);
						} else {
							temp [y, x] = blocks [y, x];
						}
						yIndex--;
					}
				}
			}
		}


		blocks = temp;
		printGrid ();
	}

	public bool matchMade(GameObject startBlock){
		Material startMat = startBlock.GetComponent<Renderer>().sharedMaterial;
		//Debug.Log (startMat);
		startBlock.GetComponent<BlockData> ().marked = true;
		Vector2 pos = startBlock.GetComponent<BlockData> ().gridCoord;
		bool found = false;
		/*Down*/
		if (pos.y > 0f &&
			blocks [(int)pos.y - 1, (int)pos.x] != null &&
			startMat == blocks [(int)pos.y - 1, (int)pos.x].GetComponent<Renderer>().sharedMaterial &&
		    !blocks [(int)pos.y - 1, (int)pos.x].GetComponent<BlockData> ().marked) {
			Debug.Log ("Matched");
			matchMade (blocks [(int)pos.y - 1, (int)pos.x]);
			found = true;
			matchCount++;
		}

		/*Left*/
		if(pos.x > 0f &&
			blocks [(int)pos.y, (int)pos.x - 1] != null &&
			startMat == blocks [(int)pos.y, (int)pos.x-1].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y, (int)pos.x-1].GetComponent<BlockData> ().marked){
			matchMade (blocks [(int)pos.y, (int)pos.x-1]);
			found = true;
			matchCount++;
		}

		/*Up*/
		if (pos.y < blocks.GetLength(0)-1 &&
			blocks [(int)pos.y + 1, (int)pos.x] != null &&
			startMat == blocks [(int)pos.y + 1, (int)pos.x].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y + 1, (int)pos.x].GetComponent<BlockData> ().marked) {
			Debug.Log ("Matched");
			matchMade (blocks [(int)pos.y + 1, (int)pos.x]);
			found = true;
			matchCount++;
		}

		/*Right*/
		if(pos.x < blocks.GetLength(1)-1 &&
			blocks [(int)pos.y, (int)pos.x + 1] != null &&
			startMat == blocks [(int)pos.y, (int)pos.x+1].GetComponent<Renderer>().sharedMaterial &&
			!blocks [(int)pos.y, (int)pos.x+1].GetComponent<BlockData> ().marked){
			matchMade (blocks [(int)pos.y, (int)pos.x+1]);
			found = true;
			matchCount++;
		}

		return found;
	}

	public void findBlock(GameObject bl){
		Debug.Log ("Trying to Find Block");
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				
				if (blocks[y,x] != null && bl.GetInstanceID () == blocks [y, x].GetInstanceID()) {
					bl.GetComponent<BlockData> ().marked = true;
					isMarked ();
				}
			}
		}
	}

	public void unmark(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null) {
					blocks [y, x].GetComponent<BlockData> ().marked = false;
				}
			}
		}
	}

	public void addRows(){
		GameObject[,] newGrid = new GameObject[32,9];
		GameObject[,] tempGrid = blocks;
		for (int x = 0; x < tempGrid.GetLength (1); x++) {
			for (int y = 0; y < tempGrid.GetLength (0); y++) {
				if (tempGrid [y, x] != null) {
					Vector3 temp = tempGrid [y, x].transform.position;
					temp.y += (tempGrid[y,x].GetComponent<BlockData>().playerOne)? 1.2f:-1.3f;
					Vector2 coord = tempGrid [y, x].GetComponent<BlockData> ().gridCoord;
					coord.y += (tempGrid[y,x].GetComponent<BlockData>().playerOne)? 1: -1;
					tempGrid [y, x].GetComponent<BlockData> ().gridCoord = coord;
					tempGrid [y, x].transform.position = temp;
					if (tempGrid [y, x].GetComponent<BlockData> ().playerOne && y < 31) {
						Debug.Log (y);
						newGrid [y + 1, x] = tempGrid [y, x];
					} else {
						newGrid [y - 1, x] = tempGrid [y, x];
					}
				}
			}
			blocks = newGrid;
			Destroy (blocks [0, x]);
			blocks [0, x] = null;

			createBlock (x, 0, true);
			Destroy (blocks [31, x]);
			blocks [31, x] = null;
			//Destroy (blocks [31, x]);
			//blocks [31, x] = null;
			createBlock (x, 31, false);
		}
		printGrid ();
	}

	public void restart(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void recolor(){
		for (int x = 0; x < blocks.GetLength (1); x++) {
			for (int y = 0; y < blocks.GetLength (0); y++) {
				if (blocks [y, x] != null) {
					int randIndex = Random.Range (0, textures.Count);
					blocks[y,x].GetComponent<Renderer> ().material = textures[randIndex];
					blocks[y,x].GetComponent<BlockData> ().color = randIndex;
				}
			}
		}
	}
}
