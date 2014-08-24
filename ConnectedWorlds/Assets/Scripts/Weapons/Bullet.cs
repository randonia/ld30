using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class Bullet : Projectile {

	private float mDamage = 0.05f;

	void Awake(){
		AwakeSetUp();
	}

	void OnTriggerEnter2D(Collider2D other){
		CWMonoBehaviour cwmb = other.GetComponent<CWMonoBehaviour>();
		if(cwmb != null){
			Debug.Log("I hit " + other.gameObject.name);
			if(cwmb.checkFlags(ObjectFlags.SHIP)){
				if(!cwmb.checkFlags(mObjectFlags))
				{
					Debug.Log("Inside of the second check");
					cwmb.GetComponent<CWMonoBehaviour>().takeDamage(mDamage);
					Debug.DrawRay(transform.position - transform.right, transform.right * 2, Color.red, 2.0f);
					Debug.DrawRay(transform.position - transform.up, transform.up * 2, Color.red, 2.0f);
				}
			}
		}
	}

}
