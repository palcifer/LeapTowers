using UnityEngine;
using System.Collections;

public class TowerBehavior : MonoBehaviour {

	private Color startcolor = Color.white;

	private BridgeBuilding cursor;

	void Start(){

		this.renderer.material.color = startcolor;

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
