using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item{
	
	public string mTitle;
	public string mIcon;
	public float mWeight;

	public Item(){

	}

	public Item(string title, string icon, float weight){
		mTitle = title;
		mIcon = icon;
		mWeight = weight;
	}
}
