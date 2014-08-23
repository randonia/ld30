using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	#region Gameobject References

	// The main camera
	public GameObject GO_mainCamera;

	// UI Components
	public GameObject GO_sasInside;
	public GameObject GO_sasLabel;
	public GameObject GO_tcvInside;
	public GameObject GO_tcvLabel;

	public GameObject GO_DEBUG;

	#endregion

	#region Member variables

	private UILabel DEBUGLABEL;

	// Rotational control/auto
	private bool mSAS_ENABLED = false;
	private bool mTempSAS_DISABLE = false;
	// Thrust vector control
	private bool mTVC_ENABLED = false;
	private bool mTempTVC_DISABLE = false;

	private Color kSASDisableColor = Color.Lerp(Color.green, Color.black, 0.8f);
	private Color kSASTempDisableColor = Color.Lerp(Color.green, Color.grey, 0.5f);
	private Color kSASEnableColor = Color.green;

	private Color kTVCDisableColor = Color.Lerp(Color.yellow, Color.black, 0.8f);
	private Color kTVCTempDisableColor = Color.Lerp(Color.yellow, Color.grey, 0.5f);
	private Color kTVCEnableColor = Color.yellow;

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
		if(Input.GetKeyDown(KeyCode.T))
		{
			mTVC_ENABLED = !mTVC_ENABLED;
		}
		mTempTVC_DISABLE = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));

		toggleSASButton();
		toggleTVCButton();

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
		GO_sasInside.GetComponent<UISprite>().color = (!mTempSAS_DISABLE && mSAS_ENABLED)?kSASEnableColor:(mTempSAS_DISABLE && mSAS_ENABLED)?kSASTempDisableColor:kSASDisableColor;
		GO_sasLabel.GetComponent<UILabel>().color = (!mTempSAS_DISABLE && mSAS_ENABLED)?Color.white:kSASDisableColor;
	}

	private void toggleTVCButton(){
		GO_tcvInside.GetComponent<UISprite>().color = (!mTempTVC_DISABLE && mTVC_ENABLED)?kTVCEnableColor:(mTempTVC_DISABLE && mTVC_ENABLED)?kTVCTempDisableColor:kTVCDisableColor;
		GO_tcvLabel.GetComponent<UILabel>().color = (!mTempTVC_DISABLE && mTVC_ENABLED)?Color.white:kTVCDisableColor;
	}

}
