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
		private SpriteRenderer mSprite;

		// State handling
		private EnemyState mState;

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

		void updateIdle(){

		}

		void updateHunting(){

		}

		void updateAttacking(){
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
