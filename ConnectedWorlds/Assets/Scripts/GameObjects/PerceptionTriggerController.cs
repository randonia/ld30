using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;


public class PerceptionTriggerController : CWMonoBehaviour {

	public delegate void dOnPerceptEnter(GameObject other);
	public delegate void dOnPerceptExit(GameObject other);

	public dOnPerceptEnter doPerceptEnter;
	public dOnPerceptExit doPerceptExit;

	void Awake(){
		setFlags(ObjectFlags.PERCEPTION_SHIP);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnTriggerEnter2D(Collider2D other){
		doPerceptEnter(other.gameObject);
	}

	public void OnTriggerExit2D(Collider2D other){
		doPerceptExit(other.gameObject);
	}
}
