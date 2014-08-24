using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class AsteroidController : CWMonoBehaviour {

	void Awake(){
		setFlags(ObjectFlags.OBSTACLE);
	}

	// Use this for initialization
	void Start () {
		float randScale = Random.Range(0.8f, 1.4f);
		float randomTorque = Random.Range(10.0f, 100.0f);

		transform.rotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f));
		transform.localScale = transform.localScale * randScale;
		rigidbody2D.mass = rigidbody2D.mass * randScale;
		bool ccwOrCw = Random.value > 0.5f;
		rigidbody2D.AddTorque(randomTorque * ((ccwOrCw)?-1:1));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D other){
		Debug.Log("I collided with: " + other.gameObject.name);
	}
}
