using UnityEngine;
using System.Collections;

public enum ThrustDirection {
	None, Forward, Reverse, Up, Down
}

public class ThrustController : MonoBehaviour {

	public ThrustDirection mDirection;

	private SpriteRenderer[] mAllRenderers;

	// Use this for initialization
	void Awake () {
		mAllRenderers =  GetComponentsInChildren<SpriteRenderer>();
	}

	void Start() {

	}

	public void EnableThruster(){
		foreach (SpriteRenderer sr in mAllRenderers)
		{
			sr.enabled = true;
		}
	}

	public void DisableThruster(){
		foreach (SpriteRenderer sr in mAllRenderers)
		{
			sr.enabled = false;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
