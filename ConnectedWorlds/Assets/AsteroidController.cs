using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class AsteroidController : CWMonoBehaviour {

	void Awake(){
		setFlags(ObjectFlags.OBSTACLE);
	}

	// Use this for initialization
	void Start () {
		float randomTorque = Random.Range(-100.0f, 100.0f);
		rigidbody2D.AddTorque(randomTorque);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D other){
		Debug.Log("I collided with: " + other.gameObject.name);
	}
}
