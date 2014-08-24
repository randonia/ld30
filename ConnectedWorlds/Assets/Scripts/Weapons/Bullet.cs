using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class Bullet : Projectile {

	private float mDamage = 0.05f;

	void Awake(){
		AwakeSetUp();
	}

	void OnCollisionEnter2D(Collision2D other){
		CWMonoBehaviour cwmb = other.gameObject.GetComponent<CWMonoBehaviour>();
		if(cwmb != null){
			Debug.Log("I hit " + other.gameObject.name);
			if(cwmb.checkFlags(ObjectFlags.SHIP)){
				Debug.Log("This is a ship");
				if(!cwmb.checkFlags(mObjectFlags))
				{
					Debug.Log("Inside of the second check");
					cwmb.GetComponent<CWMonoBehaviour>().takeDamage(mDamage);
					Debug.DrawRay(transform.position - transform.right, transform.right * 2, ((checkFlags(ObjectFlags.TEAM_PLAYER))?Color.green:Color.red), 2.0f);
					Debug.DrawRay(transform.position - transform.up, transform.up * 2, ((checkFlags(ObjectFlags.TEAM_PLAYER))?Color.green:Color.red), 2.0f);
					removeProjectile();
					Debug.Log("Remove called. This should be gone");
				}
			}
		} else {
			Debug.Log("cwmb was null for " + other.gameObject.name);
		}
	}

}
