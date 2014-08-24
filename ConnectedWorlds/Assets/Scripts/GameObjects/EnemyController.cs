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
		private float mTargetRange = 10.0f;

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
			Debug.DrawLine(transform.position, shortPosition, Color.green);
			Debug.DrawLine(gunPosition, mCurrentTarget.transform.position, Color.red);
		}

		#endregion

		void perceptionEnter(GameObject other){
			Debug.Log("Perception enter for the Enemy");
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
