using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class PlayerController : CWMonoBehaviour {

	private enum PlayerState { 
		Navigation,
		Docking,
		Docked
	}

	#region Gameobject References

	// The main camera
	public GameObject GO_mainCamera;

	// UI Components
	public GameObject GO_tcvInside;
	public GameObject GO_tcvLabel;
	public GameObject GO_dockInside;
	public GameObject GO_dockLabel;

	public GameObject GO_DEBUG;

	#endregion

	#region Member variables

	private PlayerState mState;

	private UILabel DEBUGLABEL;

	// Rendering
	private bool mRightFacing = true;
	private Vector2 mLastPosition;

	// Thrust vector control
	private bool mTVC_ENABLED = false;
	private bool mTempTVC_DISABLE = false;
	// Docking control
	private bool mDOCK_ENABLED = false;
	private bool mCanDock = false;
	private StationController mClosestStation;

	private ThrustController[] mThrusters;


	// Colors
	private Color kTVCDisableColor = Color.Lerp(Color.yellow, Color.black, 0.8f);
	private Color kTVCTempDisableColor = Color.Lerp(Color.yellow, Color.grey, 0.5f);
	private Color kTVCEnableColor = Color.yellow;

	private Color kDOCKDisableColor = Color.Lerp(Color.blue, Color.black, 0.8f);
	private Color kDOCKTempDisableColor = Color.Lerp(Color.blue, Color.grey, 0.5f);
	private Color kDOCKEnableColor = Color.blue;

	#endregion

	#region Consts

	private const float kCamZ = -10;

	#endregion

	void Awake() {
		setFlags(ObjectFlags.SHIP | ObjectFlags.TEAM_PLAYER | ObjectFlags.PLAYER);
		mState = PlayerState.Navigation;
	}

	// Use this for initialization
	void Start () {
		DEBUGLABEL = GO_DEBUG.GetComponent<UILabel>();
		DEBUGLABEL.text = "";

		mThrusters = GetComponentsInChildren<ThrustController>();
		foreach(ThrustController thruster in mThrusters){
			thruster.DisableThruster();
		}

		PerceptionTriggerController ptc = GetComponentInChildren<PerceptionTriggerController>();
		ptc.doPerceptEnter = perceptionEnter;
		ptc.doPerceptExit = perceptionExit;
	}
	
	// Update is called once per frame
	void Update () {

		// Controls
		if(Input.GetKeyDown(KeyCode.T))
		{
			mTVC_ENABLED = !mTVC_ENABLED;
		}
		mTempTVC_DISABLE = (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D));

		if(Input.GetKeyDown(KeyCode.G))
		{
			mDOCK_ENABLED = !mDOCK_ENABLED;
			if(mState == PlayerState.Docked || mState == PlayerState.Docking){
				mState = PlayerState.Navigation;
			}
		}

		mDOCK_ENABLED = mDOCK_ENABLED && mCanDock && rigidbody2D.velocity.sqrMagnitude < 0.25f;
		// Move to docking state
		if(mDOCK_ENABLED) {
			rigidbody2D.velocity = Vector2.zero;
			if(mState != PlayerState.Docked){
				mState = PlayerState.Docking;
			}
		}

		toggleTVCButton();
		toggleDockButton();

		// Update movements
		if(((Vector2)transform.position - mLastPosition).x != 0 &&
			Input.GetAxis("horizontal") != 0.0f)
		{
			mRightFacing = Input.GetAxis("horizontal") < 0.0f;
		}
		Vector2 localScale = transform.localScale;
		localScale.x = (mRightFacing)?-1:1;
		transform.localScale = localScale;
		mLastPosition = transform.position;

		// Update the camera
		GO_mainCamera.transform.position = Vector3.zero;
		GO_mainCamera.transform.Translate(transform.position.x, transform.position.y, kCamZ);

		if(mClosestStation != null){
			DEBUGLABEL.text = "State: " + mState +
				"\nDistance: " + (transform.position - mClosestStation.getDockPosition()).sqrMagnitude + 
					", Required: " + (mClosestStation.getDistanceSqr()) +
					"\nSpeed: " + rigidbody2D.velocity.sqrMagnitude +
					"\nCalculating docked: " + (mClosestStation.getDockPosition() - transform.position).sqrMagnitude;
		}
		DrawDebug();
	}

	void FixedUpdate(){
		switch (mState){
			case PlayerState.Docking:
				updateDocking();
				break;
			case PlayerState.Navigation:
				updateNavigation();
				break;
			case PlayerState.Docked:
				updateDocked();
				break;
			default:
				Debug.Log("Uh oh");
				break;
		}
	}

	#region Update methods

	void updateDocked(){

	}

	void updateDocking(){
		// Move towards the point
		rigidbody2D.MovePosition(transform.position + (mClosestStation.getDockPosition() - transform.position) * Time.deltaTime);
		if ((mClosestStation.getDockPosition() - transform.position).sqrMagnitude < 0.01f){
			mState = PlayerState.Docked;
		}
	}

	void updateNavigation(){
		// Translation
		float thrustScale = 5.0f;
		if(!mTVC_ENABLED || mTempTVC_DISABLE){
			Vector2 thrustVec = new Vector2(Input.GetAxis("horizontal"), Input.GetAxis ("vertical"));
			rigidbody2D.AddForce(thrustVec * thrustScale);
		} else {
			rigidbody2D.AddForce(-rigidbody2D.velocity * thrustScale * 0.2f);
		}

		if(mClosestStation != null){
			mCanDock = (transform.position - mClosestStation.getDockPosition()).sqrMagnitude <= (mClosestStation.getDistanceSqr());
		}
	}

	#endregion

	#region Collision Triggers

	void perceptionEnter(GameObject other){
		Debug.Log("Player entering perception zone for " + other.name);
		if (other.GetComponent<CWMonoBehaviour>().checkFlags(ObjectFlags.STATION))
		{
			mClosestStation = other.GetComponent<StationController>();
		}
	}

	void perceptionExit(GameObject other){
		Debug.Log ("Player leaving perception zone for " + other.name);
		if (mCanDock && other.GetComponent<CWMonoBehaviour>().checkFlags(ObjectFlags.STATION))
		{
			mClosestStation = null;
		}
	}

	#endregion

	private void DrawDebug(){
		Debug.DrawRay(transform.position, rigidbody2D.velocity, Color.green);
		Debug.DrawRay(transform.position, transform.up * 4.0f, Color.white);
		Debug.DrawRay(transform.position, transform.right * 4.0f, Color.yellow);
	}

	#region UI Toggles

	private void toggleTVCButton(){
		GO_tcvInside.GetComponent<UISprite>().color = (!mTempTVC_DISABLE && mTVC_ENABLED)?kTVCEnableColor:(mTempTVC_DISABLE && mTVC_ENABLED)?kTVCTempDisableColor:kTVCDisableColor;
		GO_tcvLabel.GetComponent<UILabel>().color = (!mTempTVC_DISABLE && mTVC_ENABLED)?Color.white:kTVCDisableColor;
	}

	private void toggleDockButton(){
		GO_dockInside.GetComponent<UISprite>().color = (mDOCK_ENABLED)?kDOCKEnableColor:(mCanDock)?kDOCKTempDisableColor:kDOCKDisableColor;
		GO_dockLabel.GetComponent<UILabel>().color = (mDOCK_ENABLED)?Color.white:kDOCKDisableColor;
	}

	#endregion
}
