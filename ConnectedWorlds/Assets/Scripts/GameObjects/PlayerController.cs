using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	#region Gameobject References

	// The main camera
	public GameObject GO_mainCamera;

	#endregion

	#region Member variables

	#endregion

	#region Consts

	private const float kCamZ = -10;

	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// Translation
		Vector2 thrustVec = new Vector2(Input.GetAxis("thrust"), Input.GetAxis ("strafe"));
		float thrustScale = 5.0f;
		rigidbody2D.AddForce(transform.right * thrustVec.x * thrustScale);
		rigidbody2D.AddForce(transform.up * thrustVec.y * thrustScale);

		// Rotation
		float rotScale = 0.25f;
		float rotAmount = Input.GetAxis("rotate");
		rigidbody2D.AddTorque(rotAmount * rotScale);


		GO_mainCamera.transform.position = Vector3.zero;
		GO_mainCamera.transform.Translate(transform.position.x, transform.position.y, kCamZ);

		DrawDebug();
	}

	private void DrawDebug(){
		Debug.DrawRay(transform.position, rigidbody2D.velocity, Color.green);
		Debug.DrawRay(transform.position, transform.up * 4.0f, Color.white);
		Debug.DrawRay(transform.position, transform.right * 4.0f, Color.yellow);
	}
}
