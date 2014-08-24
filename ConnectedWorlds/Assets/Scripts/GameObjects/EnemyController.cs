	using UnityEngine;
	using System.Collections;
	using ConnectedWorldsEngine;

	public class EnemyController : CWMonoBehaviour {

		private enum EnemyState{
			IDLE,
			HUNTING,
			ATTACKING,
			DIEING
		}

		public GameObject PREFAB_PROJECTILE;

		#region Editor-designed variables

		public Vector2 gunOffset;
		private Vector2 gunPosition{
			get {
				if(!mRightFacing){
					return (Vector2)transform.position + gunOffset;
				} else {
					Vector2 tOffset = gunOffset;
					tOffset.x *= -1;
					return (Vector2)transform.position + tOffset;
				}
			}
		}

		#endregion

		#region Member variables

		// Visuals
		private bool mRightFacing = true;
		private Vector2 mLastPosition;

		// State handling
		private EnemyState mState;

		// Movement handling
		public float mMaxSpeed = 10.0f;

		// Combat handling
		private GameObject mCurrentTarget;
		[Tooltip("Maximum distance to fire")]
		public float mTargetMaxRange = 10.0f;
		[Tooltip("The delay in seconds between shots")]
		public float mFireRate = 1.0f;
		private float mLastShot = -1.0f;

		#endregion

		void Awake(){
			setFlags(ObjectFlags.SHIP | ObjectFlags.TEAM_ENEMY);
			mState = EnemyState.IDLE;
		}

		// Use this for initialization
		void Start () {

			PerceptionTriggerController ptc = GetComponentInChildren<PerceptionTriggerController>();
			ptc.doPerceptEnter = perceptionEnter;
			ptc.doPerceptExit = perceptionExit;
		}

		// Update is called once per frame
		void Update () {
			switch(mState){
				case EnemyState.IDLE:
					updateIdle();
					break;
				case EnemyState.HUNTING:
					updateHunting();
					break;
				case EnemyState.ATTACKING:
					updateAttacking();
					break;
				case EnemyState.DIEING:
					break;
			}
			if(mHealth <= 0.0f && mAlive){
				Debug.Log("This enemy is dead " + gameObject.name);
				mAlive = false;
				mState = EnemyState.DIEING;
			}
			if(mLastPosition.x - transform.position.x != 0){
				mRightFacing = mLastPosition.x > transform.position.x;
			}
			Vector2 localScale = gameObject.transform.localScale;
			localScale.x = (mRightFacing)?-1:1;
			gameObject.transform.localScale = localScale;
			mLastPosition = transform.position;
		}

		#region Update methods

		void moveToPoint(Vector3 dest){
			moveToPoint((Vector2)dest);
		}

		void moveToPoint(Vector2 dest){
			if(rigidbody2D.velocity.sqrMagnitude <= (mMaxSpeed * mMaxSpeed)){
				Vector2 dirToMove = dest - (Vector2)transform.position;
				dirToMove.Normalize();
				rigidbody2D.AddForce(dirToMove);
			} 
		}

		void updateIdle(){

		}

		void updateHunting(){

		}

		void updateAttacking(){
			Vector2 dirFromTarget = (transform.position - mCurrentTarget.transform.position).normalized;
			// Aim specific number of units short of the target
			Vector2 shortPosition = ((Vector2)mCurrentTarget.transform.position + dirFromTarget * 5.0f);
			moveToPoint(shortPosition);

			if(mLastShot + mFireRate < Time.time){
				FireAt(mCurrentTarget);
				mLastShot = Time.time;
			}

			Debug.DrawLine(transform.position, shortPosition, Color.green);
			Debug.DrawLine(gunPosition, mCurrentTarget.transform.position, Color.red);
		}

		#endregion

		void FireAt(GameObject target){
			float bulletVelocity = 50.0f;
			Vector3 randomLead = Vector3.zero;
			float distance = (target.transform.position - transform.position).magnitude;

			Rigidbody2D targetRB2D = target.GetComponent<Rigidbody2D>();
			if(targetRB2D != null){
				randomLead = targetRB2D.velocity.normalized;
			}
			Vector3 dir = ((target.transform.position + randomLead) - transform.position) / distance;
			GameObject proj = (GameObject)GameObject.Instantiate(PREFAB_PROJECTILE, gunPosition, Quaternion.identity);
			float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			proj.GetComponent<Projectile>().Initialize((Vector2)dir, bulletVelocity, 5.0f, ObjectFlags.TEAM_ENEMY, 0, rot);
			Physics2D.IgnoreCollision(proj.collider2D, gameObject.collider2D);
		}

		void perceptionEnter(GameObject other){
			Debug.Log("Perception enter for the Enemy: " +  other.name);
			CWMonoBehaviour cwmb = other.GetComponent<CWMonoBehaviour>();
			if(cwmb != null){
				if(cwmb.checkFlags(ObjectFlags.PLAYER)){
					mCurrentTarget = other;
					mState = EnemyState.ATTACKING;
				}
			}
		}

		void perceptionExit(GameObject other){
			CWMonoBehaviour cwmb = other.GetComponent<CWMonoBehaviour>();
			if(cwmb != null){
				if(cwmb.checkFlags(ObjectFlags.PLAYER)){
					Debug.Log("Lost player. Going into hunt mode");
					mState = EnemyState.HUNTING;
				}
			}
		}
	}
