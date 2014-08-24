using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class DroneFighterController : CWMonoBehaviour {

	private enum DroneState{
		Idle,
		Attacking
	}

	public GameObject PREFAB_BULLET;
	public GameObject PREFAB_EXPLOSION;
	public GameObject GO_Player;

	public Vector2[] mGunPositions;
	public Vector2 LASER;
	private int mGunIndex = 0;
	private Vector2 gunPosition {
		get {
			Vector2 offset = mGunPositions[mGunIndex];
			mGunIndex = (mGunIndex + 1) % mGunPositions.Length;
			return transform.TransformPoint(offset);
		}
	}


	// The "path" to follow
	private float mPathTime = 0.0f;
	public float mPathSpeed = 1.0f;
	public float mRadius = 8.0f;
	public float mLifeSpan = 15.0f;
	public float mBirthTime;

	// Properties
	private DroneState mState;

	// Combat stuff
	public float mFireRate;
	private float mLastFire = -1.0f;
	private GameObject mCurrentTarget;

	void Awake(){
		setFlags(ObjectFlags.DRONE | ObjectFlags.TEAM_PLAYER);
		mPathTime = Random.Range(0.0f, Mathf.PI * 2.0f);
		mPathSpeed = (Random.Range(0.9f, 1.1f) * ((Random.value < 0.5f)?-1:1));
		GO_Player = (GameObject)GameObject.Find("Player");
		mBirthTime = Time.time;
	}

	// Use this for initialization
	void Start () {
		PerceptionTriggerController ptc = GetComponentInChildren<PerceptionTriggerController>();
		ptc.doPerceptEnter = perceptionEnter;
		ptc.doPerceptExit = perceptionExit;
	}
	
	// Update is called once per frame
	void Update () {
		if(mCurrentTarget != null && mLastFire + mFireRate < Time.time)
		{
			FireAt(mCurrentTarget);
			mLastFire = Time.time;
		}
		if(Time.time > mBirthTime + mLifeSpan){
			GameObject splosion = (GameObject)GameObject.Instantiate(PREFAB_EXPLOSION, transform.position, transform.rotation);
			splosion.transform.localScale = new Vector2(0.2f, 0.2f);
			GameObject.Destroy(splosion, 8);
			GameObject.Destroy(gameObject);
		}
	}

	void FixedUpdate(){
		float xC = mRadius * Mathf.Cos(mPathTime) + Mathf.Sin(-mPathTime) * 2.0f;
		float yC = mRadius * Mathf.Sin(mPathTime) + Mathf.Cos(-mPathTime) * 2.0f;

		mPathTime = (mPathTime + Time.deltaTime * mPathSpeed) % (2 * Mathf.PI);

		transform.position = (Vector2)GO_Player.transform.position + new Vector2(xC, yC);
		float rotation = Mathf.Atan2(yC, xC) * Mathf.Rad2Deg;
		transform.localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
	}

	void FireAt(GameObject target){
		float bulletVelocity = 100.0f;
		Vector3 dir = (target.transform.position - transform.position).normalized;
		GameObject proj = (GameObject)GameObject.Instantiate(PREFAB_BULLET, gunPosition, Quaternion.identity);
		Debug.DrawRay(transform.position, dir * 5.0f, Color.yellow);
		float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		proj.GetComponent<Projectile>().Initialize((Vector2)dir, bulletVelocity, 5.0f, ObjectFlags.TEAM_PLAYER, 0, rot);
		Debug.DrawRay(proj.transform.position, dir * 5.0f, Color.red);
		Physics2D.IgnoreCollision(proj.collider2D, GO_Player.collider2D);
		Physics2D.IgnoreCollision(proj.collider2D, collider2D);
	}

	void perceptionEnter(GameObject other){
		CWMonoBehaviour cwmb = other.GetComponent<CWMonoBehaviour>();
		if(cwmb != null){
			if(cwmb.checkFlags(ObjectFlags.TEAM_ENEMY | ObjectFlags.SHIP)){
				mCurrentTarget = other;
			}
		}
	}

	void perceptionExit(GameObject other){
		CWMonoBehaviour cwmb = other.GetComponent<CWMonoBehaviour>();
		if(cwmb != null){
			if(cwmb.checkFlags(ObjectFlags.TEAM_ENEMY | ObjectFlags.SHIP)){
				mCurrentTarget = null;
			}
		}
	}
}
