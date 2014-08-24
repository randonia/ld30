using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class PlayerController : CWMonoBehaviour {

	private enum PlayerState { 
		Navigation,
		Docking,
		Docked,
		Dead,
		FuelBoned
	}

	#region Gameobject References

	// The main camera
	public GameObject GO_mainCamera;

	// UI Components
	public GameObject GO_tcvInside;
	public GameObject GO_tcvLabel;
	public GameObject GO_dockInside;
	public GameObject GO_dockLabel;

	public GameObject PREFAB_DRONE;
	public GameObject PREFAB_EXPLOSION;


	public GameObject GO_indicator;
	public bool shouldIndicate {get{return mDestinationStation != null && !mCanDock && !mDOCK_ENABLED;}}
	public GameObject mDestinationStation;

	#endregion

	#region Member variables

	// For data binding
	public string VelocityString {get{return rigidbody2D.velocity.magnitude.ToString("F2") + "m/s";}}
	public float VelocityFloatForCamera {get{float mag = rigidbody2D.velocity.magnitude; return Mathf.Min(Mathf.Max(7.5f, Mathf.Pow(mag, 0.5f) + 3), 15.0f);}}
	private PlayerState mState;

	public bool mShowToolTip = false;

	public bool mShowBuyMenu = true;


	public float mFuelLevel = 1.0f;
	public bool CanBuyFuel {get{return mCredits >= 15;}}
	public bool CanBuyDrones {get{return mCredits >= 25;}}
	public int NumDrones {get{return mDroneCount;}}
	public int mDroneCount;
	public float healthPercent {get{return mHealth;}}
	private UILabel DEBUGLABEL;
	public int mCredits = 100;
	public Inventory.Row mCurrentItem;
	public int CargoHold {get{return mCurrentItem.quantity;}}
	public float mHealCredits = 0.0f;

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
		if(!mAlive){
			mState = PlayerState.Dead;
			return;
		}
		// Controls
		if(Input.GetKeyDown(KeyCode.F)){
			deployDrone();
		}

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

		if(mShowBuyMenu && mState != PlayerState.Docked && mClosestStation != null){
			mClosestStation.GetComponent<StationController>().tearDownShopPanel();
		}
		mShowBuyMenu = mState == PlayerState.Docked;

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
			case PlayerState.FuelBoned:
				updateFuelBoned();
				break;
			case PlayerState.Dead:
				updateDead();
				break;
			default:
				break;
		}
	}

	#region Update methods

	void updateFuelBoned(){
		if(!mShowToolTip){
			mShowToolTip = true;
			GameObject.Find("info_label").GetComponent<UILabel>().text = "You have run out of fuel. Now you will drift through space until someone blows you up. CLick Okay to restart";
			GameController mGameController = GameObject.Find("GameController").GetComponent<GameController>();
			EventDelegate.Set(GameObject.Find("tooltip_okay_btn").GetComponent<UIButton>().onClick, mGameController.restartGame);
			rigidbody2D.fixedAngle = false;
			rigidbody2D.AddTorque(10.0f);
		}

	}

	void updateDead(){
		if(PREFAB_EXPLOSION != null){
			GameObject.Instantiate(PREFAB_EXPLOSION, transform.position + transform.up * 2, transform.rotation);
			GameObject.Instantiate(PREFAB_EXPLOSION, transform.position + transform.up * -2, transform.rotation);
			GameObject.Instantiate(PREFAB_EXPLOSION, transform.position + transform.right * 3, transform.rotation);
			GameObject.Instantiate(PREFAB_EXPLOSION, transform.position + transform.right * -2, transform.rotation);
			gameObject.rigidbody2D.velocity = Vector2.zero;
			GameObject.Destroy(gameObject, 2.0f);
			PREFAB_EXPLOSION = null;
			mShowToolTip = true;
			GameObject.Find("info_label").GetComponent<UILabel>().text = "You have lost. Press okay to restart the game";
			GameController mGameController = GameObject.Find("GameController").GetComponent<GameController>();
			EventDelegate.Set(GameObject.Find("tooltip_okay_btn").GetComponent<UIButton>().onClick, mGameController.restartGame);

		}
	}

	void updateDocked(){
		if(mHealth < 1.0f){
			if(mCredits >= 10){
				mHealth = Mathf.Min(mHealth + 0.001f, 1.0f);
				mHealCredits -= 0.1f;
				if(mHealCredits <= 0){
					mCredits -= 10;
					mHealCredits = 10.0f;
				}
			}
		}
		if(mCurrentItem.quantity > 0){
			mCredits += (mCurrentItem.quantity / 10) * mCurrentItem.credits;
			mCurrentItem.quantity = 0;
		}
	}

	void updateDocking(){
		// Move towards the point
		rigidbody2D.MovePosition(transform.position + (mClosestStation.getDockPosition() - transform.position) * Time.deltaTime);
		if ((mClosestStation.getDockPosition() - transform.position).sqrMagnitude < 0.01f){
			mState = PlayerState.Docked;
			mClosestStation.GetComponent<StationController>().setUpShopPanel();
		}
	}

	void updateNavigation(){
		// Translation
		float thrustScale = 5.0f;
		Vector2 thrustVec = Vector2.zero;
		if(!mTVC_ENABLED || mTempTVC_DISABLE){
			thrustVec = new Vector2(Input.GetAxis("horizontal"), Input.GetAxis ("vertical"));
			rigidbody2D.AddForce(thrustVec * thrustScale);
		} else {
			thrustVec = -rigidbody2D.velocity * thrustScale * 0.2f;
			rigidbody2D.AddForce(thrustVec);
			thrustVec.Normalize();
		}
		mFuelLevel -= (thrustVec.sqrMagnitude * 0.000083f);
		if(mFuelLevel <= 0.0f){
			mState = PlayerState.FuelBoned;
		}
		if(mClosestStation != null && mClosestStation.gameObject == mDestinationStation){
			mCanDock = (transform.position - mClosestStation.getDockPosition()).sqrMagnitude <= (mClosestStation.getDistanceSqr());
		} else {
			mCanDock = false;
		}

		if(mDestinationStation != null){
			Vector3 dir = (mDestinationStation.transform.position - transform.position).normalized;
			GO_indicator.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90.0f);
			GO_indicator.transform.localPosition = dir * 175.0f;
		}
	}

	#endregion

	#region Collision Triggers

	void perceptionEnter(GameObject other){
		if (other.GetComponent<CWMonoBehaviour>().checkFlags(ObjectFlags.STATION))
		{
			mClosestStation = other.GetComponent<StationController>();
		}
	}

	void perceptionExit(GameObject other){
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

	public void OnBuyDronesClick(){
		if(mCredits >= 25){
			mCredits -= 25;
			mDroneCount += 10;
		}
	}

	public void OnBuyFuelClick(){
		if(mCredits >= 15){
			mCredits -= 15;
			mFuelLevel += 0.05f;
		}
	}

	public bool BuyItem(Inventory.Row itemRow){
		if(mCredits >= itemRow.credits){
			mCurrentItem = itemRow;
			mCredits -= itemRow.credits;
			mState = PlayerState.Navigation;
			mDOCK_ENABLED = false;

			GameObject[] all_stations = GameObject.FindGameObjectsWithTag("Station");
			GameObject oldStation = mDestinationStation;
			while(mDestinationStation == oldStation){
				int randIndex = (int)(Random.value * all_stations.Length);
				mDestinationStation = all_stations[randIndex];
			}
			mClosestStation.GetComponent<StationController>().tearDownShopPanel();
			mClosestStation = null;
			return true;
		}
		return false;
	}

	void deployDrone(){
		if(NumDrones > 0){
			GameObject newDrone = (GameObject)GameObject.Instantiate(PREFAB_DRONE);
			mDroneCount--;
		}
	}

	public void toggleToolTip(){
		mShowToolTip = !mShowToolTip;
	}
	#endregion
}
