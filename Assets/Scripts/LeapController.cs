using UnityEngine;
using System.Collections;
using Leap;

public class LeapController : MonoBehaviour {

	Controller controller;
	public GameObject cursor;
	private Object CursorPrefab;
	private bool isCursorAssigned;

	Vector3 palmPosition;
	float planeYOffset;

	void Awake() {
		CursorPrefab = Resources.Load("Prefabs/SphereCursor");
		cursor = (GameObject)Instantiate (CursorPrefab, GameObject.Find ("Main Camera").transform.position, Quaternion.identity);
	}

	// Use this for initialization
	void Start () {
		controller = new Controller ();
		palmPosition = new Vector3 ();
		isCursorAssigned = false;
		planeYOffset = GameObject.Find ("GameLogic").GetComponent<SceneLogic> ().getPlaneYPosition ();
	}
	
	// Update is called once per frame
	void Update () {
		if (isCursorAssigned) {
			if (controller.IsConnected) {
				//Debug.Log ("Controller is connected");
				Frame frame = controller.Frame ();
				HandList handList = frame.Hands;
				foreach (Hand h in handList) {
					if (h.Equals (handList.Frontmost)) {
						palmPosition.x = h.StabilizedPalmPosition.x / 25;
						palmPosition.y = (h.StabilizedPalmPosition.y / 25 + (planeYOffset * 2) - 1);
						palmPosition.z = -1 * (h.StabilizedPalmPosition.z / 25);
					}
				}
				cursor.transform.position = palmPosition;
			}
		}
	}

	public void InitCursor(){
		//UnityEngine.Screen.showCursor = false;
		isCursorAssigned = true;
	}
}
