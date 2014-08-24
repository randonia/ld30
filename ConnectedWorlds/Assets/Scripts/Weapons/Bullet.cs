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
			if(cwmb.checkFlags(ObjectFlags.SHIP)){
				if(!cwmb.checkFlags(mObjectFlags))
				{
					cwmb.GetComponent<CWMonoBehaviour>().takeDamage(mDamage);
					Debug.DrawRay(transform.position - transform.right, transform.right * 2, ((checkFlags(ObjectFlags.TEAM_PLAYER))?Color.green:Color.red), 2.0f);
					Debug.DrawRay(transform.position - transform.up, transform.up * 2, ((checkFlags(ObjectFlags.TEAM_PLAYER))?Color.green:Color.red), 2.0f);
					removeProjectile();
				}
			}
		}
	}
}
