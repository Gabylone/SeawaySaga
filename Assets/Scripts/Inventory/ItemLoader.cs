using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ItemCategory {
	Provisions,
	Weapon,
	Clothes,
	Misc,
}

public class ItemLoader : MonoBehaviour {

	public static ItemLoader Instance;

	[SerializeField]
	private int categoryAmount = 5;

	ItemCategory currentType = ItemCategory.Provisions;
	public static ItemCategory[] allCategories;

	[SerializeField]
	private string pathToCSVs = "Items/CSVs";
	private TextAsset[] files;

	private Item[][] items;

	[Header("Chances")]
	[SerializeField]
	private int[] amountRange_Min;

	[SerializeField]
	private int[] amountRange_Max;

	int currentID = 0;

	void Awake () {
		Instance = this;
	}

	public void Init () {

		allCategories = new ItemCategory[categoryAmount];

		for (int i = 0; i < allCategories.Length; ++i ) {

			allCategories [i] = (ItemCategory)i;

		}

		items = new Item[categoryAmount][];

		files = new TextAsset[Resources.LoadAll (pathToCSVs, typeof(TextAsset)).Length];
		int index = 0;
		foreach ( TextAsset textAsset in Resources.LoadAll (pathToCSVs, typeof(TextAsset) )) {
			files[index] = textAsset;
			++index;
		}

		foreach ( TextAsset file in files ) {
			LoadItems (file);
			++currentType;
		}
	}

	void LoadItems (TextAsset data) {

		string[] rows = data.text.Split ('\n');

		items[(int)currentType] = new Item[rows.Length-2];

		string maxLevelTxt = rows [rows.Length - 2].Split (';')[5];
		int maxLevel = int.Parse (maxLevelTxt);


		int currentLevel = 0;

		for ( int i = 1; i < items[(int)currentType].Length+1 ;++i ) {

			string[] cells = rows[i].Split (';');

			Item newItem =
				new Item (
				currentID,
				cells[0],// french name
				cells[1],// english name
                cells[2],// description
				int.Parse(cells[3]),// value
				int.Parse(cells[4]),// price
				int.Parse(cells[5]),// weight
				int.Parse(cells[6]),// level
				int.Parse(cells[7]),

				currentType // category
				);
			
			items[(int)currentType][i-1] = newItem;

			currentID++;
		}
	}

	#region random items
	public Item[] getRandomCategoryOfItem ( ItemCategory category , int mult ) {

		int itemType = (int)category;

		int itemAmount = Random.Range (amountRange_Min[itemType],amountRange_Max[itemType]) * mult;

		Item[] tmpItems = new Item[itemAmount];

		// reset mult
		for (int i = 0; i < itemAmount; ++i)
		{

			int level = 0;
			if (Crews.playerCrew.CrewMembers.Count > 0) {

				if ( (ItemCategory)itemType == ItemCategory.Misc || (ItemCategory)itemType == ItemCategory.Provisions ) {
					tmpItems [i] = GetRandomItem ((ItemCategory)itemType);
				} else {
					// il ne peut prendre le capitaine ennemi que si otherinventory.trade ( ou loot ) == true
					if (OtherInventory.Instance.type != OtherInventory.Type.None) {
						level = Random.Range (Crews.enemyCrew.captain.Level - 2, Crews.enemyCrew.captain.Level + 2);
					} else {
						level = Random.Range (Crews.playerCrew.captain.Level - 2, Crews.playerCrew.captain.Level + 2);
					}
					level = Mathf.Clamp (level, 1, 10);
					tmpItems[i] = GetRandomItemOfCertainLevel ((ItemCategory)itemType,level);
				}

			} else {
				tmpItems [i] = GetRandomItem ((ItemCategory)itemType);

			}

		}

		return tmpItems;
	}

	public Item GetRandomItem ( ItemCategory category ) {

		int l = items [(int)category].Length;
		int index = Random.Range (0, l);

		return items[(int)category][index];
	}

	public Item GetRandomItemOfCertainLevel ( ItemCategory category , int targetLevel = 0 ) {

//		Debug.Log ( items [(int)category] [2].ID + " AAAAAAAAAAAAAA" );
		Item[] tmpItems = System.Array.FindAll (items [(int)category], x => x.level == targetLevel);

		return tmpItems [Random.Range (0, tmpItems.Length)];
	}

	public Item[] getItems ( ItemCategory itemType ) {
		return items[(int)itemType];
	}

	public Item GetItem ( int itemID ) {

		for (int i = 0; i < 4; i++) {

			if (itemID < GetMaximumRange ((ItemCategory)i))
				return items[i][itemID-GetMinimumRange ((ItemCategory)i)];

		}

		Debug.Log ("did not find item");

		return new Item ();

	}

	public Item GetItem ( ItemCategory itemType , int itemID ) {
		if ( itemID >= items[(int)itemType].Length ) {
			Debug.LogError ( "item id out of range " + " ID : " + itemID + " / cat : " + itemType + " L : " + items[(int)itemType].Length );
			return items [(int)itemType][0];
		}
		return items[(int)itemType][itemID];
	}

	public Item[][] Items {
		get {
			return items;
		}
	}
	#endregion

	public int CategoryAmount {
		get {
			return categoryAmount;
		}
	}

	public int GetMinimumRange ( ItemCategory cat ) {

		switch (cat) {
		case ItemCategory.Provisions:
			return 0;
		case ItemCategory.Weapon:
			return items[0].Length;
		case ItemCategory.Clothes:
			return items[0].Length + items[1].Length;
		case ItemCategory.Misc:
			return items[0].Length + items[1].Length + items[2].Length;
		default:
			return 0;
		}
	}

	public int GetMaximumRange ( ItemCategory cat ) {

		switch (cat) {
		case ItemCategory.Provisions:
			return items[0].Length;
		case ItemCategory.Weapon:
			return items[0].Length + items[1].Length;
		case ItemCategory.Clothes:
			return items[0].Length + items[1].Length + items[2].Length;
		case ItemCategory.Misc:
			return items[0].Length + items[1].Length + items[2].Length + items[3].Length;
		default:
			return 0;
		}
	}
}

