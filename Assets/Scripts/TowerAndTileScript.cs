using UnityEngine;
using System.Collections;
using System.Collections.Generic;

	
public class TowerAndTileScript : MonoBehaviour
{

	public Object Tower;
	public int range = 3;
	
	private Color startcolor = Color.black;
	public bool hasTower;

	private SceneLogic scn;
	

	void Start()
	{
		GetComponent<Renderer>().material.color = startcolor;
		Tower = Resources.Load("Prefabs/Tower");
		hasTower = false;
		scn = GameObject.Find ("GameLogic").GetComponent<SceneLogic> ();

	}
	
	void OnMouseDown()
	{
		GameObject tower = CreateTower (0);
		if(tower != null) {
			scn.Towers.Add(tower);
		}
	}

	void OnMouseEnter()
	{
		GetComponent<Renderer>().material.color = Color.green;
	}
	
	void OnMouseExit()
	{
		GetComponent<Renderer>().material.color = startcolor;
	}

	public GameObject CreateTower(float ran){
		if (!hasTower) {
			GameObject tower = (GameObject)Instantiate(Tower, transform.position, Quaternion.identity);
			if(ran == 0) {
				tower.transform.localScale = new Vector3 (1, Random.Range (0, range)+0.5f, 1);
			} else {
				tower.transform.localScale = new Vector3 (1, ran, 1);
			}
			tower.transform.position = new Vector3 (tower.transform.position.x, 
			                                        tower.transform.position.y + tower.GetComponent<MeshRenderer>().bounds.size.y/2, 
			                                        tower.transform.position.z);
			tower.transform.name = "Tower_" + tower.transform.position.x + "_" + tower.transform.position.z;
			hasTower = true;
			return tower;
		} else {
			Debug.Log("There is allready a tower in this tile, you little fuck!");
			return null;
		}
	}

}
	