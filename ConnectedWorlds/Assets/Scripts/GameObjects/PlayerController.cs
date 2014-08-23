using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	#region Gameobject References

	// The main camera
	public GameObject GO_mainCamera;
	public GameObject GO_sasInside;
	public GameObject GO_sasLabel;

	public GameObject GO_DEBUG;

	#endregion

	#region Member variables

	private UILabel DEBUGLABEL;

	private bool mSAS_ENABLED = false;
	private bool mTempSAS_DISABLE = false;

	private Color kDisableColor = Color.Lerp(Color.green, Color.black, 0.8f);
	private Color kTempDisableColor = Color.Lerp(Color.green, Color.grey, 0.5f);
	private Color kEnableColor = Color.green;

	#endregion

	#region Consts

	private const float kCamZ = -10;

	#endregion

	// Use this for initialization
	void Start () {
		DEBUGLABEL = GO_DEBUG.GetComponent<UILabel>();
		DEBUGLABEL.text = "";
	}
	
	// Update is called once per frame
	void Update () {

		// Controls
		if(Input.GetKeyDown(KeyCode.R))
		{
			mSAS_ENABLED = !mSAS_ENABLED;
		}
		mTempSAS_DISABLE = (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E));

		toggleSASButton();

		// Translation
		Vector2 thrustVec = new Vector2(Input.GetAxis("thrust"), Input.GetAxis ("strafe"));
		float thrustScale = 5.0f;
		rigidbody2D.AddForce(transform.right * thrustVec.x * thrustScale);
		rigidbody2D.AddForce(transform.up * thrustVec.y * thrustScale);

		// Rotation
		if(!mSAS_ENABLED || mTempSAS_DISABLE){
			float rotScale = 0.35f;
			float rotAmount = Input.GetAxis("rotate");
			rigidbody2D.AddTorque(rotAmount * rotScale);
		} else {
				rigidbody2D.AddTorque(-(rigidbody2D.angularVelocity * 0.05f));
		}

		// Update the camera
		GO_mainCamera.transform.position = Vector3.zero;
		GO_mainCamera.transform.Translate(transform.position.x, transform.position.y, kCamZ);

		DrawDebug();
	}

	private void DrawDebug(){
		Debug.DrawRay(transform.position, rigidbody2D.velocity, Color.green);
		Debug.DrawRay(transform.position, transform.up * 4.0f, Color.white);
		Debug.DrawRay(transform.position, transform.right * 4.0f, Color.yellow);
	}

	private void toggleSASButton(){
		GO_sasInside.GetComponent<UISprite>().color = (!mTempSAS_DISABLE && mSAS_ENABLED)?kEnableColor:(mTempSAS_DISABLE && mSAS_ENABLED)?kTempDisableColor:kDisableColor;
		GO_sasLabel.GetComponent<UILabel>().color = (!mTempSAS_DISABLE && mSAS_ENABLED)?Color.white:kDisableColor;
	}
}
