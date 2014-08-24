using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ConnectedWorldsEngine;

public class StationController : CWMonoBehaviour {


	#region PREFABS
	public Object GO_LRPREFAB;
	#endregion

	// The offset of the actual "Dock Point" where a player can dock
	public Vector2 mDockOffset;
	[Tooltip("Number of meters required to be within to start the dock. 1 Unity Unit == 10 meters")]
	public int mAutoDockingDistance = 10;


	private List<GameObject> mNearbyShips;
	private List<GameObject> mLineRenderers;

	void Awake(){
		setFlags(ObjectFlags.STATION | ObjectFlags.TEAM_NEUTRAL);
	}

	// Use this for initialization
	void Start () {
		mNearbyShips = new List<GameObject>();
		mLineRenderers = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < mNearbyShips.Count; ++i){
			mLineRenderers[i].GetComponent<LineRenderer>().SetPosition(0, getDockPosition());
			mLineRenderers[i].GetComponent<LineRenderer>().SetPosition(1, mNearbyShips[i].transform.position);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("Object " + other.gameObject.name + " entered station trigger zone");
		CWMonoBehaviour cwmbOther = other.gameObject.GetComponent<CWMonoBehaviour>();
		if (cwmbOther.checkFlags(ObjectFlags.PLAYER))
		{
			addShip(other.gameObject);
		}
		if(cwmbOther.checkFlags(ObjectFlags.PERCEPTION_SHIP))
		{
			other.GetComponent<PerceptionTriggerController>().OnTriggerEnter2D(collider2D);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		Debug.Log ("Object " + other.gameObject.name + " leaving station trigger zone");
		CWMonoBehaviour cwmbOther = other.gameObject.GetComponent<CWMonoBehaviour>();
		if(cwmbOther.checkFlags(ObjectFlags.PLAYER)){
			removeShip(other.gameObject);
		}
		if(cwmbOther.checkFlags(ObjectFlags.PERCEPTION_SHIP)){
			other.GetComponent<PerceptionTriggerController>().OnTriggerExit2D(collider2D);
		}
	}

	// Gets the position of the actual dock point
	public Vector3 getDockPosition(){
		return transform.position + (Vector3)mDockOffset;
	}

	// Gets the squared distance required to dock
	public float getDistanceSqr(){
		return (mAutoDockingDistance * mAutoDockingDistance) / 100.0f;
	}

	void addShip(GameObject other){
		mNearbyShips.Add(other);
		mLineRenderers.Add((GameObject)GameObject.Instantiate(GO_LRPREFAB, transform.position, transform.rotation));
	}

	void removeShip(GameObject other){
		int shipIndex = mNearbyShips.IndexOf(other);
		Debug.Log (shipIndex);
		mNearbyShips.RemoveAt(shipIndex);
		GameObject objToDestroy = mLineRenderers[shipIndex];
		mLineRenderers.RemoveAt(shipIndex);
		GameObject.Destroy(objToDestroy);
	}

}
