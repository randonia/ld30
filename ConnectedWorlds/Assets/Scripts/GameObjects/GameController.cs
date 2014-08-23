﻿using UnityEngine;
using System.Collections;

namespace ConnectedWorldsEngine
{
	public static class ObjectFlags{
		public const uint NONE = 0;
		public const uint SHIP = 2;
		public const uint STATION = 4;
		public const uint ENVIRONMENT = 8;
		public const uint PROJECTILE = 16;
		public const uint TEAM_NEUTRAL = 32;
		public const uint TEAM_PLAYER = 64;
		public const uint TEAM_ENEMY = 128;
	}
}

public class GameController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
