using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class Projectile : CWMonoBehaviour {

	// All projectiles should override this, lest they never exist
	protected float mTimeToLive = 10.0f;
	protected float mVelocity;
	protected float mAcceleration;
	protected Vector2 mDirection;
	protected float mBirthTime = -1.0f;

    protected LayerMask mMaskToHit = LayerMask.NameToLayer("ProjectileTargets");
    protected LayerMask mPlayerMask = LayerMask.NameToLayer("Player");

	protected void AwakeSetUp(){
		setFlags(ObjectFlags.PROJECTILE);
		mBirthTime = Time.time;
	}

	void Awake(){
		AwakeSetUp();
	}

	public void Initialize(Vector2 direction, float velocity, float timeToLive,
						   uint flags, float acceleration=0.0f, float rotation = 0.0f){
		mDirection = direction;
		mVelocity = velocity;
		mAcceleration = acceleration;
		mTimeToLive = timeToLive;
		setFlags(flags);
		transform.GetChild(0).localRotation = Quaternion.Euler(0.0f, 0.0f, rotation);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time > mBirthTime + mTimeToLive){
			removeProjectile();
		}
	}

    void FixedUpdate(){
    	rigidbody2D.MovePosition((Vector2)transform.position + mDirection * mVelocity * Time.deltaTime);
    }



	protected virtual void removeProjectile(){
		GameObject.Destroy(gameObject);
	}
}
