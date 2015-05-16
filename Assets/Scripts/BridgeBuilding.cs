using UnityEngine;
using System.Collections;

public class BridgeBuilding : MonoBehaviour {


	public bool creatingBridge;
	public GameObject towerFrom;
	public GameObject towerTo;
	private Object Bridge;
	private GameObject bridge;
	Vector3 cameraPosition;

	public float bridgeLength = 5.0f;

	//position of bridge
	Vector3 position;
	//scale of bridge
	float scale;

	// Use this for initialization
	void Start () {
		creatingBridge = false;
		Bridge = Resources.Load ("Prefabs/Bridge");
		cameraPosition = GameObject.Find ("Main Camera").transform.position;
		cameraPosition.z = cameraPosition.z - 3;
		bridge = (GameObject)Instantiate (Bridge, cameraPosition, Quaternion.Euler(90, 0, 0));
		position = new Vector3 ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (creatingBridge) {
			if(towerTo == null ){

				//scaling in X axis
				scale = Mathf.Sqrt(Mathf.Pow(this.transform.position.x - towerFrom.transform.position.x, 2) + 
				                   Mathf.Pow(this.transform.position.y - (towerFrom.transform.position.y + towerFrom.GetComponent<MeshRenderer>().bounds.size.y/2), 2) +
				                   Mathf.Pow(this.transform.position.z - towerFrom.transform.position.z, 2));
				//scale = (scale < bridgeLength) ? scale : bridgeLength;
				if(scale>bridgeLength) {
					bridge.GetComponent<Renderer>().material.color = Color.red;
				} else {
					bridge.renderer.material.color = Color.white;
				}
				bridge.transform.localScale = new Vector3(scale, 1, 0.1f);

				position = (this.transform.position + new Vector3(towerFrom.transform.position.x, 
				                                                  GameObject.Find("GameLogic").GetComponent<SceneLogic>().getPlaneYPosition() + towerFrom.transform.localScale.y * 2.0f,
				                                                  towerFrom.transform.position.z)) / 2;
				bridge.transform.position = position;


				bridge.transform.LookAt(this.transform.position);
				bridge.transform.Rotate(new Vector3(90, 90, 0));;

			}
		}
	}

	public bool IsTowerFromAssigned(){
		return (towerFrom != null) ?  true :  false;
	}

	public bool IsTowerToAssigned(){
		return (towerTo != null) ?  true :  false;
	}

	public void ResetBridge(){
		bridge.transform.position = cameraPosition;
	}

	public bool IsBridgeShortEnaugh(){
		return (scale<bridgeLength);
	}
	

	public void BuildBridgeBetweenTwoTowers(){
		if (!towerTo.Equals (null)) {
			scale = Mathf.Sqrt (Mathf.Pow (towerTo.transform.position.x - towerFrom.transform.position.x, 2) + 
			                    Mathf.Pow ((towerTo.transform.position.y + towerTo.GetComponent<MeshRenderer>().bounds.size.y/2) - (towerFrom.transform.position.y + towerFrom.GetComponent<MeshRenderer>().bounds.size.y/2), 2) +
				Mathf.Pow (towerTo.transform.position.z - towerFrom.transform.position.z, 2));
			bridge.transform.localScale = new Vector3 (scale, 1, 0.1f);
		
			position = ((new Vector3 (towerTo.transform.position.x, 
		                      		  GameObject.Find ("GameLogic").GetComponent<SceneLogic> ().getPlaneYPosition () + towerTo.transform.localScale.y * 2.0f,
		                        	  towerTo.transform.position.z))
						+ new Vector3 (towerFrom.transform.position.x, 
		                              GameObject.Find ("GameLogic").GetComponent<SceneLogic> ().getPlaneYPosition () + towerFrom.transform.localScale.y * 2.0f,
		                              towerFrom.transform.position.z)) / 2;
			bridge.transform.position = position;
		
		
			bridge.transform.LookAt (new Vector3 (towerTo.transform.position.x, 
		                                     GameObject.Find ("GameLogic").GetComponent<SceneLogic> ().getPlaneYPosition () + towerTo.transform.localScale.y * 2.0f,
		                                     towerTo.transform.position.z));
			bridge.transform.Rotate (new Vector3 (90, 90, 0));
			;
		}
	}

	public void SetBridgeLength(float l){
		bridgeLength = l;
	}
}
