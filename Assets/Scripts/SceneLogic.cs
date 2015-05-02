using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SceneLogic : MonoBehaviour {
	
	float yPlanePosition = -3;
	public int planeXTiles = 8;
	public int planeYTiles = 4;

	private Object TilePrefab;
	public IList<GameObject> Tiles = new List<GameObject>();

	public IList<GameObject> Towers = new List<GameObject>();

	private Object NewronkoPrefab;
	public GameObject newronko;
	private Vector3 newronkoPosition = Vector3.zero;
	public float newronkoFinalXPosition = -30;
	private float newronkoScaleY = 1;

	private BridgeBuilding cursor;
	private GameObject initTower;

	private TextAsset savedScenes;
	private Dictionary<string, IList<Vector3>> savedScenesList;

	// Use this for initialization
	void Start () {
		TilePrefab = Resources.Load ("Prefabs/Tile");
		createPlane (planeXTiles, planeYTiles, 2);
		NewronkoPrefab = Resources.Load ("Prefabs/Newronko");
		newronko = (GameObject)Instantiate(NewronkoPrefab, GameObject.Find ("Main Camera").transform.position, Quaternion.identity);
		cursor = GameObject.Find ("LeapController").GetComponent<LeapController> ().cursor.GetComponent<BridgeBuilding> ();
		savedScenes = (TextAsset)Resources.Load ("SavedScenes");
		savedScenesList = new Dictionary<string, IList<Vector3>> ();
	}
	

	//method for creating a X x Z dimension plane of quads with step scale
	//quads will be in hierarchy set under Empty object called Plane and will have tag "Tile"
	public void createPlane(int x, int z, int step){
		int i = -x;
		int j = -z;
		GameObject plane = new GameObject ("Plane");
		while (i<x) {
			while (j<z) {
				GameObject quad = (GameObject)Instantiate(TilePrefab, new Vector3(i + step/2, yPlanePosition , j + step/2), Quaternion.Euler(90, 0, 0));
				quad.transform.localScale = new Vector3(step, step, 1);
				quad.transform.parent = plane.transform;
				Tiles.Add(quad);
				j = j + step;
			}
			j = -z;
			i = i + step;
		}
	}

	public void createScene(int difficulty){
		int numberOfTiles = (planeXTiles * planeYTiles) / (difficulty + 3);
		for (int i = 0; i < numberOfTiles; i++) {
			GameObject quad = Tiles [Random.Range (0, (planeXTiles * planeYTiles) - 1)];
			GameObject tower = quad.GetComponent<TowerAndTileScript> ().CreateTower (0);
			if (tower == null) {
				i--;
			} else {
				Towers.Add (tower);
			}
		}
	}

	public float getPlaneYPosition(){
		return yPlanePosition;
	}

	public void CreateNewronko(){
		newronkoScaleY = newronko.GetComponent<MeshRenderer> ().bounds.size.y;
		foreach (GameObject tower in Towers) {
			if ((tower.transform.position.x <= newronkoPosition.x)){
				newronkoPosition = new Vector3(tower.transform.position.x, 
				                                      tower.transform.position.y + tower.transform.localScale.y + newronkoScaleY/2, 
				                                      tower.transform.position.z);
				initTower = tower;
			}
			if(tower.transform.position.x>newronkoFinalXPosition) newronkoFinalXPosition = tower.transform.position.x;
		}
		newronko.transform.position = newronkoPosition;
	}

	public void SetNewronkoNewPosition(Vector3 vect){
		vect.y = vect.y + newronkoScaleY / 2;
		newronko.transform.position = vect;
	}	

	public void SetBridgeLength(float l){
		cursor.SetBridgeLength(l);
	}

	public void DijkstraCheckScene(){
		float bridgeLength = cursor.bridgeLength;
		//save tower list
		IList<GameObject> tmpTowers = new List<GameObject> (Towers);

	}

	private float distanceOfTwoTowers(GameObject towerFrom, GameObject towerTo){
		return Mathf.Sqrt(Mathf.Pow(this.transform.position.x - towerFrom.transform.position.x, 2) + 
		          		  Mathf.Pow(this.transform.position.y - (towerFrom.transform.position.y + towerFrom.GetComponent<MeshRenderer>().bounds.size.y/2), 2) +
		        	      Mathf.Pow(this.transform.position.z - towerFrom.transform.position.z, 2));
	}

	public void SaveTowers(int i){
		string str = "";
		//first record
		if (str == "") {
			str = str + "%" + i + "\n";
		} else {
		//not forst record
			//str = str + "\n%" + System.DateTime.Now.ToString ("HH-mm_dd-MM-yyyy") + "\n";
			str = str + "\n%" + i + "\n";
		}
		foreach (GameObject tower in Towers) {
			str = str + tower.transform.position.x + ";" + tower.transform.position.z + ";" + tower.transform.localScale.y + "\n";
		}
		str = str + "&";
		System.IO.File.WriteAllText (Application.dataPath + "/Resources/Save/SavedScene" + System.Convert.ToString(i) + ".txt", str);
	}

	public void LoadTowers(int i){

		savedScenes = (TextAsset)Resources.Load("Save/SavedScene" + System.Convert.ToString(i));
		string str = savedScenes.text;
		string[] lines = str.Split('\n');

		string name = "";
		string[] coordinatesStr = new string[3];
		IList<Vector3> towerCoordinates = new List<Vector3> ();


		foreach (string line in lines) {
			if(line != ""){
				if(line[0]=='%'){
					name = line[1].ToString();
				}
				if((line[0]!='%') && (line[0]!='&')){
					coordinatesStr = line.Split(';');
					Vector3 coordinatesVect = new Vector3(float.Parse(coordinatesStr[0]), 
					                                      float.Parse(coordinatesStr[1]), 
					                                      float.Parse(coordinatesStr[2]));
					towerCoordinates.Add(coordinatesVect);
				}
				if(line[0] == '&'){
					IList<Vector3> towerCoordinatesTemp = new List<Vector3> (towerCoordinates);
					savedScenesList.Add(name, towerCoordinatesTemp);
					towerCoordinates.Clear();
				}
			}
		}

		string temp = System.Convert.ToString(i);
		IList<Vector3> towerCoordinates2 = new List<Vector3> ();
		savedScenesList.TryGetValue(temp, out towerCoordinates2);
		foreach (Vector3 vect in towerCoordinates2) {
			foreach (GameObject tile in Tiles) {
				if (tile.transform.position.x == vect.x && tile.transform.position.z == vect.y) {
					GameObject tower = tile.GetComponent<TowerAndTileScript> ().CreateTower (vect.z);
					Towers.Add(tower);
				}
			}
		}
	}
}
