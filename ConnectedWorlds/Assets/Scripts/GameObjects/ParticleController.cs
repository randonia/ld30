using UnityEngine;
using System.Collections;

public class ParticleController : MonoBehaviour {

	#region GameObjects

	public GameObject GO_Player;
	public GameObject[] GO_ParticleSystems;

	#endregion

	#region Member variables

	private ParticleSystem[] mParticleSystems;
	private Vector3 mPlayerStart;
	#endregion

	// Use this for initialization
	void Start () {
		mPlayerStart = GO_Player.transform.position;

		mParticleSystems = new ParticleSystem[GO_ParticleSystems.Length];
		for(int i = 0; i < GO_ParticleSystems.Length; ++i)
		{
			mParticleSystems[i] = GO_ParticleSystems[i].GetComponent<ParticleSystem>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 vec_diff = GO_Player.transform.position - mPlayerStart;

		float len = GO_ParticleSystems.Length;
		for(int i = 0; i < len; ++i)
		{
			GO_ParticleSystems[i].transform.position = ((Vector2)GO_Player.transform.position) - (((i+1) / len) * vec_diff * 0.05f);
		}
	}
}
