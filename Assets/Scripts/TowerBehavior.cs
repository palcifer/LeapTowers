using UnityEngine;
using System.Collections;

public class TowerBehavior : MonoBehaviour {

	private Color startcolor = Color.white;

	private BridgeBuilding cursor;

	private SceneLogic scn;

	//deleting and scaling of towers
	private Vector3 mousePosInit;
	private System.DateTime timeInit;
	private float towerYScaleInit;

	void Start(){

		this.renderer.material.color = startcolor;
		scn = GameObject.Find ("GameLogic").GetComponent<SceneLogic> ();
	}

	void OnMouseUp(){
		System.TimeSpan span = System.DateTime.Now - timeInit;
		if(span.TotalSeconds < 0.2d)
		DeleteTower ();
	}

	void OnMouseDown(){
		timeInit = System.DateTime.Now;
		mousePosInit = Input.mousePosition;
		towerYScaleInit = this.gameObject.transform.localScale.y;
	}

	void OnMouseDrag(){
		float ofset = (mousePosInit.y - Input.mousePosition.y)/(-100.0f);
		//print (ofset);
		ofset = towerYScaleInit + ofset;
		if (ofset <= 0.5f)
			ofset = 0.5f;
		this.gameObject.transform.localScale = new Vector3(1, ofset, 1);
		this.gameObject.transform.position = new Vector3 (this.gameObject.transform.position.x, 
		                                                 scn.getPlaneYPosition() + this.gameObject.GetComponent<MeshRenderer> ().bounds.size.y / 2, 
		                                                 this.gameObject.transform.position.z);
	}

	public void DeleteTower(){
		scn.Towers.Remove (this.gameObject);
		GameObject tile = scn.GetTileOnCoordinates (this.gameObject.transform.position.x, this.gameObject.transform.position.z);
		tile.GetComponent<TowerAndTileScript> ().hasTower = false;
		Destroy (this.gameObject);
	}

	void OnTriggerEnter(Collider collider){
		cursor = collider.GetComponent<BridgeBuilding> ();
		if (collider.tag.Equals ("Cursor")) {

			//No bridge in the scene
			if(!cursor.IsTowerFromAssigned() && !cursor.IsTowerToAssigned()){

				cursor.creatingBridge = true;
				cursor.towerFrom = this.gameObject;

			} else {

				//Bridge between tower From and cursor
				if(cursor.IsTowerFromAssigned() && !cursor.IsTowerToAssigned()){

					//Reseting bridge, if I want begin from another tower
					if(cursor.towerFrom.Equals(this.gameObject)){
						cursor.resetBridge();
						cursor.creatingBridge = false;
						cursor.towerFrom = null;
					} else {

						//Constructing the bridge between towers
						if(cursor.IsBridgeShortEnaugh()){
							cursor.towerTo = this.gameObject;
							cursor.creatingBridge = false;
							cursor.BuildBridgeBetweenTwoTowers();
							SceneLogic scn = GameObject.Find("GameLogic").GetComponent<SceneLogic>();
							//newronko is on tower from
							if((scn.newronko.transform.position.x == cursor.towerFrom.transform.position.x) && (scn.newronko.transform.position.z == cursor.towerFrom.transform.position.z)){
								scn.SetNewronkoNewPosition(new Vector3(cursor.towerTo.transform.position.x, 
								                                       cursor.towerTo.transform.position.y + cursor.towerTo.transform.localScale.y, 
								                                       cursor.towerTo.transform.position.z));
							} else {
								//newronko is on tower to
								if((scn.newronko.transform.position.x == cursor.towerTo.transform.position.x) && (scn.newronko.transform.position.z == cursor.towerTo.transform.position.z)){
									scn.SetNewronkoNewPosition(new Vector3(cursor.towerFrom.transform.position.x, 
									                                       cursor.towerFrom.transform.position.y + cursor.towerFrom.transform.localScale.y, 
									                                       cursor.towerFrom.transform.position.z));
								}
							}
							if(scn.newronko.transform.position.x == scn.newronkoFinalXPosition){
								print("Hotovooo, vyhral si");
							}
						}
					}

				} else {

					//Bridge is build and I want to start a new one
					if(cursor.IsTowerFromAssigned() && cursor.IsTowerToAssigned()){
						cursor.towerFrom = this.gameObject;
						cursor.towerTo = null;
						cursor.creatingBridge = true;
					}

				}
			}
		}

	}

}
