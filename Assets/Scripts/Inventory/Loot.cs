using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

[System.Serializable]
public class Loot {

	public int row = 0;
	public int col = 0;

	public List<List<int>> ids = new List<List<int>>();

	[NonSerialized]
	List<List<Item>> allItems = new List<List<Item>>();

	public List<List<Item>> AllItems {
		get {
			return allItems;
		}
		set {
			allItems = value;
		}
	}

	public int weight = 0;

	public Loot()
	{
		
	}

	public Loot(int _row, int _col)
	{
		row = _row;
		col = _col;

		for (int i = 0; i < 4; i++) {
			AllItems.Add (new List<Item> ());
			ids.Add (new List<int> ());
		}

	}

	public void Randomize ( ItemCategory[] categories, int mult) {

		// for each categories in cell
		foreach (var category in categories) {
			
			Item[] items = ItemLoader.Instance.getRandomCategoryOfItem (category, mult);

			foreach ( Item item in items ) {
				AddItem (item);
			}

		}


	}

	#region add & remove items
	public void AddItem ( Item newItem ) {

		AllItems [(int)newItem.category].Add (newItem);
		ids[(int)newItem.category].Add (newItem.ID);

		weight += newItem.weight;

		if ( LootManager.Instance.updateLoot != null )
			LootManager.Instance.updateLoot ();

	}

	public void RemoveItem ( Item itemToRemove ) {

		AllItems [(int)itemToRemove.category].Remove (itemToRemove);
		ids[(int)itemToRemove.category].Remove (itemToRemove.ID);

		weight -= itemToRemove.weight;

		if ( LootManager.Instance.updateLoot != null )
			LootManager.Instance.updateLoot ();

	}
//	public List<Item> GetItemsFromCategory ( ItemCategory cat ) {
//		return allItems [(int)cat];
//	}
	public void EmptyCategory ( ItemCategory cat ) {
//		for (int i = 0; i < allItems[(int)cat].Count; i++) {
//			RemoveItem (allItems [(int)cat][i]);
//		}

	}
	#endregion

	public bool IsEmpty ()
	{
		foreach (var item in AllItems) {
			if (item.Count > 0)
				return false;
		}

		return true;
	}
}
