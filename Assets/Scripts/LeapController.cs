using UnityEngine;
using System.Collections;
using Leap;

public class LeapController : MonoBehaviour {

	Controller controller;
	public GameObject cursor;
	private Object CursorPrefab;
	private bool isCursorAssigned;

	Vector3 palmPosition;
	Vector3 palmDirection;
	//Vector3 palmNormal;
	float planeYOffset;

	void Awake() {
		//CursorPrefab = Resources.Load("Prefabs/SphereCursor");
		//cursor = (GameObject)Instantiate (CursorPrefab, GameObject.Find ("Main Camera").transform.position, Quaternion.identity);
	}

	// Use this for initialization
	void Start () {
		controller = new Controller ();
		palmPosition = new Vector3 ();
		palmDirection = new Vector3 ();
		//palmNormal = new Vector3 ();
		isCursorAssigned = false;
		planeYOffset = GameObject.Find ("GameLogic").GetComponent<SceneLogic> ().getPlaneYPosition ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isCursorAssigned) {
			if (controller.IsConnected) {
				//Debug.Log ("Controller is connected");
				Frame frame = controller.Frame ();
				HandList handList = frame.Hands;
				foreach (Hand h in handList) {
					if (h.Equals (handList.Frontmost)) {
						palmPosition.x = h.StabilizedPalmPosition.x / 25;
						palmPosition.y = (h.StabilizedPalmPosition.y / 25 + (planeYOffset * 2) - 3.5f);
						palmPosition.z = -1 * (h.StabilizedPalmPosition.z / 25);
						palmDirection = new Vector3(h.Direction.x * -1.0f, h.Direction.y * -1.0f, h.Direction.z);
						//palmNormal = new Vector3(h.PalmNormal.x, h.PalmNormal.y, h.PalmNormal.z);
					}
				}
				if(palmPosition != Vector3.zero)
				cursor.transform.position = palmPosition;
				if(palmDirection != Vector3.zero)
				cursor.transform.rotation = Quaternion.LookRotation(palmDirection);
				//cursor.transform.rotation = Quaternion.LookRotation(palmNormal);
			}
		}
	}

	public void InitCursor(){
		//UnityEngine.Screen.showCursor = false;
		isCursorAssigned = true;
	}
}
