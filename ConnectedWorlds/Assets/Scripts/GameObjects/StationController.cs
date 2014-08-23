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
		Debug.Log(other.gameObject.name + " entered trigger zone");
		if (other.GetComponent<CWMonoBehaviour>().checkFlags(ObjectFlags.SHIP))
		{
			addShip(other.gameObject);
		}
	}

	void OnTriggerExit2D(Collider2D other){
		Debug.Log (other.gameObject.name + " leaving trigger zone");
		removeShip(other.gameObject);
	}

	void addShip(GameObject other){
		mNearbyShips.Add(other);
		mLineRenderers.Add((GameObject)GameObject.Instantiate(GO_LRPREFAB, transform.position, transform.rotation));
	}

	void removeShip(GameObject other){
		int shipIndex = mNearbyShips.IndexOf(other);
		mNearbyShips.RemoveAt(shipIndex);
		GameObject objToDestroy = mLineRenderers[shipIndex];
		mLineRenderers.RemoveAt(shipIndex);
		GameObject.Destroy(objToDestroy);
	}

}
