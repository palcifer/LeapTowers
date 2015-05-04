using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class SceneLogic : MonoBehaviour {
	
	float yPlanePosition = -3;
	public int planeXTiles = 8;
	public int planeYTiles = 4;

	private Object TilePrefab;
	public IList<GameObject> Tiles = new List<GameObject>();

	public List<GameObject> Towers = new List<GameObject>();

	private Object NewronkoPrefab;
	public GameObject newronko;
	public float newronkoFinalXPosition = 5;
	private float newronkoScaleY = 1;

	private BridgeBuilding cursor;
	private GameObject initTower;

	private TextAsset savedScenes;
	private Dictionary<string, IList<Vector3>> savedScenesDictionary;
	public Image pathIndicator;

	// Use this for initialization
	void Start () {
		TilePrefab = Resources.Load ("Prefabs/Tile");
		createPlane (planeXTiles, planeYTiles, 2);
		NewronkoPrefab = Resources.Load ("Prefabs/Newronko");
		newronko = (GameObject)Instantiate(NewronkoPrefab, GameObject.Find ("Main Camera").transform.position, Quaternion.identity);
		cursor = GameObject.Find ("LeapController").GetComponent<LeapController> ().cursor.GetComponent<BridgeBuilding> ();
		savedScenes = (TextAsset)Resources.Load ("SavedScenes");
		savedScenesDictionary = new Dictionary<string, IList<Vector3>> ();
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
		Towers.Sort (Compare);
		if (Towers.Count != 0) {
			initTower = Towers.First ();
		} else {
			initTower = GameObject.Find("Main Camera");
		}
		newronko.transform.position = new Vector3(initTower.transform.position.x, 
		                               initTower.transform.position.y + initTower.transform.localScale.y + newronkoScaleY/2, 
		                               initTower.transform.position.z);
		CheckScene ();
	}

	public void SetNewronkoNewPosition(Vector3 vect){
		vect.y = vect.y + newronkoScaleY / 2;

		//newronko.transform.position = vect;
		StartCoroutine("moveNewronko", vect);
	}	

	private IEnumerator moveNewronko(Vector3 finalPosition){
		Vector3 initPosition = newronko.transform.position;
		Vector3 vect = finalPosition - initPosition;
		vect.Normalize ();
		float dist = distanceBetweenTwoPoints (initPosition, finalPosition);
		float speed = 25;
		for (int i = 0; i < speed; i++) {
			newronko.transform.position = newronko.transform.position + (dist / speed) * vect;
			//newronko.transform.Translate(vect, (dist/speed));
			yield return new WaitForEndOfFrame();
		}
		newronko.transform.position = finalPosition;
		if(newronko.transform.position.x == newronkoFinalXPosition){
			print("Hotovooo, vyhral si");
		}
		yield return null;
	}

	private float distanceBetweenTwoPoints(Vector3 initPosition, Vector3 finalPosition){
		return Mathf.Sqrt (Mathf.Pow(initPosition.x - finalPosition.x, 2) +
		                   Mathf.Pow(initPosition.y - finalPosition.y, 2) +
		                   Mathf.Pow(initPosition.z - finalPosition.z, 2));
	}
	
	
	public GameObject GetTileOnCoordinates(float x, float z){
		foreach (GameObject tile in Tiles) {
			if(tile.transform.position.x == x && tile.transform.position.z == z)
				return tile;
		}
		return null;
	}

	public void SetBridgeLength(float l){
		cursor.SetBridgeLength(l);
	}

	public void CheckScene(){
		if (Towers.Count == 0)
			return;
		float bridgeLength = cursor.bridgeLength;
		Towers.Sort (Compare);
		newronkoFinalXPosition = Towers.Last ().transform.position.x;
		foreach (GameObject tower in Towers) {
			if(tower.transform.position.x == newronkoFinalXPosition)
				tower.renderer.material.color = Color.red;
		}
		GameObject[] towers = new GameObject[Towers.Count];
		towers = Towers.ToArray ();
		List<GameObject> workingSet = new List<GameObject> ();
		workingSet.Add (initTower);

		float towerWidth = towers[0].GetComponent<MeshRenderer> ().bounds.size.x / 2;
		float cursorWidth = cursor.GetComponent<MeshRenderer> ().bounds.size.x / 2;
		bridgeLength = bridgeLength + towerWidth + cursorWidth;

		bool addTower = false;
		for (int i = 1; i < towers.Count(); i++) {
			addTower = false;
			foreach (GameObject workingTower in workingSet) {

				if(distanceOfTwoTowers(workingTower, towers[i])<bridgeLength){
					addTower = true;
				}
			}

			if(addTower){
			workingSet.Add(towers[i]);
			}
		}
		//print (workingSet.Last ().name + "    " + newronkoFinalXPosition);
		if (workingSet.Last().transform.position.x == newronkoFinalXPosition) {
			//print("there is a way");
			pathIndicator.color = Color.green;
		} else {
			//print("there is not a way");
			pathIndicator.color = Color.red;
		}

//		//save tower list
//		//List<GameObject> tmpTowers = new List<GameObject> (Towers);
//		//WTF?? link :D 
//		//var tmpTowers = from i in Towers orderby i.transform.position.x select i;
//		List<GameObject> tmpTowers = new List<GameObject> (Towers);
//		tmpTowers.Sort (Compare);
//		List<GameObject> workingSet = new List<GameObject> ();
//
//		workingSet.Add (initTower);
//		tmpTowers.RemoveAt (0);
//
//		List<GameObject>.Enumerator workingSetEnumerator = workingSet.GetEnumerator ();
//		List<GameObject>.Enumerator tmpTowersEnumerator = tmpTowers.GetEnumerator ();
//		GameObject workingTower = new GameObject();
//		GameObject tmpTower = new GameObject();
//		
//		bool addTower = false;
//
//		while (tmpTowersEnumerator.MoveNext()) {
//			while (workingSetEnumerator.MoveNext()) {
//
//				workingTower = workingSetEnumerator.Current;
//				tmpTower = tmpTowersEnumerator.Current;
//
//				if(distanceOfTwoTowers(workingTower, tmpTower) < bridgeLength){
//					addTower = true;
//				}
//
//				if(Mathf.Abs(tmpTower.transform.position.x - workingTower.transform.position.x) < bridgeLength){
//					//workingSetEnumerator.Dispose();
//				}
//			}
//			if(addTower){
//				workingSet.Add(tmpTower);
//				addTower = false;
//			}
//		}
//
//		if (tmpTowers.Count == 0) {
//			print ("everything ok");
//		} else {
//			print ("everything is not ok");
//		}
//
//		//tmpTowers.Sort (Compare);
//		//ArrayList.Adapter (tmpTowers).Sort (comparator);
//		foreach (GameObject item in tmpTowers) {
//			print(item.transform.position);
//		}
	}

	private int Compare(GameObject a, GameObject b){
		if (a.transform.position.x < b.transform.position.x)
			return -1;
		else if (a.transform.position.x > b.transform.position.x)
			return +1;
		else if (a.transform.position.z > a.transform.position.z) 
			return -1;
		else if (a.transform.position.z < a.transform.position.z)
			return 1;
		else 
			return 0;
	}

	private float distanceOfTwoTowers(GameObject towerFrom, GameObject towerTo){
		if (towerFrom == null || towerTo == null)
			return 0;
		return Mathf.Sqrt(Mathf.Pow(towerTo.transform.position.x - towerFrom.transform.position.x, 2) + 
		                  Mathf.Pow((towerTo.transform.position.y + towerTo.GetComponent<MeshRenderer>().bounds.size.y/2) - (towerFrom.transform.position.y + towerFrom.GetComponent<MeshRenderer>().bounds.size.y/2), 2) +
		                  Mathf.Pow(towerTo.transform.position.z - towerFrom.transform.position.z, 2));
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

		savedScenesDictionary.Clear();
		deleteAllTowers ();

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
					savedScenesDictionary.Add(name, towerCoordinatesTemp);
					towerCoordinates.Clear();
				}
			}
		}

		string temp = System.Convert.ToString(i);
		IList<Vector3> towerCoordinates2 = new List<Vector3> ();
		savedScenesDictionary.TryGetValue(temp, out towerCoordinates2);
		foreach (Vector3 vect in towerCoordinates2) {
			foreach (GameObject tile in Tiles) {
				if (tile.transform.position.x == vect.x && tile.transform.position.z == vect.y) {
					GameObject tower = tile.GetComponent<TowerAndTileScript> ().CreateTower (vect.z);
					Towers.Add(tower);
				}
			}
		}
		CreateNewronko ();
	}

	private void deleteAllTowers(){
		GameObject[] towers = new GameObject[Towers.Count];
		towers = Towers.ToArray ();

		for (int i = 0; i < towers.Count(); i++) {
			GameObject tower = towers[i];
			tower.GetComponent<TowerBehavior>().DeleteTower();
		}
	}
}
