using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		DrawDebug();
	}

	private void DrawDebug(){
		Debug.DrawRay(transform.position, rigidbody2D.velocity, Color.green);
		Debug.DrawRay(transform.position, transform.up * 15.0f, Color.white);
		Debug.DrawRay(transform.position, transform.right * 15.0f, Color.yellow);
	}
}
