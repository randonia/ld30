using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ConnectedWorldsEngine
{
	public static class ObjectFlags{
		public const uint NONE = 0;
		public const uint PLAYER = 1;
		public const uint SHIP = 2;
		public const uint STATION = 4;
		public const uint OBSTACLE = 8;
		public const uint PROJECTILE = 16;
		public const uint DRONE = 32;
		public const uint TEAM_NEUTRAL = 64;
		public const uint TEAM_PLAYER = 128;
		public const uint TEAM_ENEMY = 512;
		public const uint PERCEPTION_SHIP = 1024;
	}
}

public class CWMonoBehaviour : MonoBehaviour{
	protected uint mObjectFlags;

	public bool checkFlags(uint flagsToCheck){
		return (mObjectFlags & flagsToCheck) == flagsToCheck;
	}

	public void setFlags(uint flagsToSet){
		mObjectFlags |= flagsToSet;
	}

	public void clearFlags(uint flagsToClear){
		mObjectFlags = (flagsToClear ^ uint.MaxValue) & mObjectFlags;
	}

}

public class GameController : MonoBehaviour {

	private enum GameState{
		Menu,
		Playing,
		Paused
	}

	public bool isPaused {get{return mState == GameState.Paused;}}

	public GameObject GO_Player;
	private PlayerController mPlayer;

	private GameState mState;

	// Use this for initialization
	void Start () {
		mState = GameState.Playing;
		mPlayer = GO_Player.GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P)){
			mState = (mState == GameState.Paused)?GameState.Playing:GameState.Paused;
		}
		switch(mState){
			case GameState.Menu:
				break;
			case GameState.Playing:
				Time.timeScale = 1.0f;
				if (mPlayer){

				}
				break;
			case GameState.Paused:
				Time.timeScale = 0.0f;
				break;
		}
	}

	public void UnPauseButtonClick(){
		Debug.Log("Button Pressed");
		mState = GameState.Playing;
	}
}
