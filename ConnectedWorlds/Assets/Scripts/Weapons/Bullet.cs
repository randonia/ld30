using UnityEngine;
using System.Collections;
using ConnectedWorldsEngine;

public class Bullet : Projectile {

	void Awake(){
		AwakeSetUp();
	}

	void OnTriggerEnter2D(Collider2D other){
		CWMonoBehaviour cwmb = other.GetComponent<CWMonoBehaviour>();
		if(cwmb != null){
			Debug.Log("I hit " + other.gameObject.name);
			if(cwmb.checkFlags(ObjectFlags.PLAYER)){
				Debug.Log("I hit the player, woo!");
				Debug.DrawRay(transform.position, transform.right * 2, Color.red, 2.0f);
				Debug.DrawRay(transform.position, transform.up * 2, Color.red, 2.0f);
			}
		}
	}

}
