using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Inventory{
	
	public struct Row{
		public string itemTitle;
		public string iconName;
		public int quantity;
		public int credits;

		public Row(string title, string icon, int num, int credits){
			itemTitle = title;
			iconName = icon;
			quantity = num;
			this.credits = credits;
		}

		public bool isEmpty(){
			return quantity == 0;
		}
	}

	public List<Row> stockPile;

	public Inventory(){
		stockPile = new List<Row>();
	}

	public void AddItem(string itemName, string iconName, int quantity, int price){
		stockPile.Add(new Row(itemName, iconName, quantity, price));
	}

}
