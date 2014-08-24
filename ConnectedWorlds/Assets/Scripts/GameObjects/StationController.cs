using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ConnectedWorldsEngine;

public class StationController : CWMonoBehaviour {


	#region PREFABS
	public Object GO_LRPREFAB;
	#endregion

	public Vector2 mBeamOffset;

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
			mLineRenderers[i].GetComponent<LineRenderer>().SetPosition(0, ((Vector2)gameObject.transform.position + mBeamOffset));
			mLineRenderers[i].GetComponent<LineRenderer>().SetPosition(1, mNearbyShips[i].transform.position);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("Object " + other.gameObject.name + " entered station trigger zone");
		CWMonoBehaviour cwmbOther = other.gameObject.GetComponent<CWMonoBehaviour>();
		if (cwmbOther.checkFlags(ObjectFlags.SHIP))
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
		if(cwmbOther.checkFlags(ObjectFlags.SHIP)){
			removeShip(other.gameObject);
		}
		if(cwmbOther.checkFlags(ObjectFlags.PERCEPTION_SHIP)){
			other.GetComponent<PerceptionTriggerController>().OnTriggerExit2D(collider2D);
		}
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
