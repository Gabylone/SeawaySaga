using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LootManager : MonoBehaviour {

	public static LootManager Instance;

	[SerializeField]
	private CategoryContent defaultCategoryContent;
	[SerializeField]
	private CategoryContent tradeCategoryContent_Player;
	[SerializeField]
	private CategoryContent tradeCategoryContent_Other;

	[SerializeField]
	private CategoryContent lootCategoryContent_Player;
	[SerializeField]
	private CategoryContent lootCategoryContent_Other;

	[SerializeField]
	private CategoryContent tradeCategoryContent_Combat;

	[SerializeField]
	private CategoryContent inventoryCategoryContent;

    public Color selectedButtonColor = Color.cyan;

	public delegate void UdpateLoot();
	public UdpateLoot updateLoot;

	private Loot playerLoot;
	private Loot otherLoot;

	[SerializeField]
	private Sprite[] foodSprites;

	[SerializeField]
	private Sprite[] clotheSprites;

	[SerializeField]
	private Sprite[] miscSprites;

    public Color item_DefaultColor;
    public Color item_SuperiorColor;
    public Color item_InferiorColor;
	public Color item_EquipedColor;
    public Color item_EmptyColor;

    public bool debugItems = false;

	void Awake (){
		Instance = this;
	}

	void Start () {

		StoryFunctions.Instance.getFunction += HandleGetFunction;

    }

    void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.AddToInventory:
			AddToInventory ();
                break;
		case FunctionType.RemoveFromInventory:
			RemoveFromInventory ();
                break;
            case FunctionType.CheckInInventory:
			CheckInInventory ();
			break;
		}
	}

	public void CreateNewLoot () {
		Loot playerLoot = new Loot (0, 0);

        if ( debugItems)
        {
            playerLoot.Randomize (ItemLoader.allCategories,10);
            Debug.LogError("debugging items");
        }
        else
        {
            playerLoot.Randomize(new ItemCategory[1] { ItemCategory.Provisions }, 2);

        }

        SetLoot (Crews.Side.Player, playerLoot);
	}

	public Loot PlayerLoot {
		get {
			return playerLoot;
		}
	}

	public Loot OtherLoot {
		get {
			return otherLoot;
		}
	}

	public Loot getLoot (Crews.Side side) {
		return side == Crews.Side.Player ? playerLoot : otherLoot;
	}

	public void SetLoot ( Crews.Side side , Loot targetLoot) {
		if (side == Crews.Side.Player) {
			playerLoot = targetLoot;
		} else {
			otherLoot = targetLoot;
		}
	}

	public Loot GetIslandLoot (int mult, bool fightingLoot) {

        int row = StoryReader.Instance.Row;
        int col = StoryReader.Instance.Col;

        // if the loot is a fight loot, go fetch the crew's data
        if (fightingLoot)
        {
            row = Crews.enemyCrew.managedCrew.row;
            col = Crews.enemyCrew.managedCrew.col;
        }

		var tmpLoot = StoryReader.Instance.CurrentStoryHandler.GetLoot (row, col);

		if (tmpLoot == null) {

			Loot newLoot = new Loot (row , col);

			ItemCategory[] categories = getLootCategoriesFromCell ();

			newLoot.Randomize (categories,mult);

			StoryReader.Instance.CurrentStoryHandler.SetLoot (newLoot);

			return newLoot;

		}

		return tmpLoot;
	}

	public ItemCategory[] getLootCategoriesFromCell () {

		string cellParams = StoryFunctions.Instance.CellParams;

		if ( cellParams.Length < 2 ) {
			return ItemLoader.allCategories;
		}

		string[] cellParts = cellParams.Split ('/');

		ItemCategory[] categories = new ItemCategory[cellParts.Length];

		int index = 0;

		foreach ( string cellPart in cellParts ) {

			categories [index] = getLootCategoryFromString(cellPart);

			++index;
		}

		return categories;
	}

	public ItemCategory getLootCategoryFromString ( string arg ) {

		switch (arg) {
		case "Food":
			return ItemCategory.Provisions;
		case "Weapons":
			return ItemCategory.Weapon;
		case "Clothes":
			return ItemCategory.Clothes;
		case "Misc":
			return ItemCategory.Misc;
		}

		Debug.LogError ("getLootCategoryFromString : couldn't find category in : " + arg);

		return ItemCategory.Misc;

	}

	#region item
	void RemoveFromInventory () {

		string cellParams = StoryFunctions.Instance.CellParams;

		ItemCategory targetCat = getLootCategoryFromString (cellParams.Split('/')[1]);
		StoryReader.Instance.NextCell ();

		if ( LootManager.Instance.getLoot(Crews.Side.Player).AllItems [(int)targetCat].Count == 0 ) {
			Debug.LogError ( "REMOVE IN INVENTORY : la catégorie visée est vide : ignorement" );
			StoryReader.Instance.UpdateStory ();
			return;
		}

		Item item = LootManager.Instance.getLoot(Crews.Side.Player).AllItems [(int)targetCat] [0];
		if (cellParams.Contains ("<")) {
			string itemName = cellParams.Split ('<') [1];
			itemName = itemName.Remove (itemName.Length - 6);
			item = LootManager.Instance.getLoot (Crews.Side.Player).AllItems [(int)targetCat].Find (x => x.frenchName == itemName);
		}

        Narrator.Instance.ShowNarratorInput("You lost the item : " + item.englishName);

        LootManager.Instance.getLoot (Crews.Side.Player).RemoveItem (item);

	}
	void AddToInventory () {

		string cellParams = StoryFunctions.Instance.CellParams;

		ItemCategory targetCat = getLootCategoryFromString (cellParams.Split('/')[1]);

		Item item = null;

		if (cellParams.Contains ("<")) {

			string itemName = cellParams.Split ('<') [1];

            itemName = itemName.Remove(itemName.Length - 6);

            item = ItemLoader.Instance.GetItem(targetCat,itemName);

			if (item == null) {
				Debug.LogError ("item : " + itemName + " was not found, returning random");
				item = ItemLoader.Instance.GetRandomItem (targetCat);
			}
				
		} else {
			item = ItemLoader.Instance.GetRandomItem (targetCat);
		}

        DisplayStoryItem.Instance.DisplayItem( item );

		getLoot (Crews.Side.Player).AddItem (item);
	}

	void CheckInInventory () {
		
		string cellParams = StoryFunctions.Instance.CellParams;

		StoryReader.Instance.NextCell ();

		ItemCategory targetCat = getLootCategoryFromString (cellParams.Split('/')[1]);

		if (cellParams.Contains ("<")) {
			
			string itemName = cellParams.Split ('<') [1];
			itemName = itemName.Remove (itemName.Length - 6);

			Item item = LootManager.Instance.getLoot (Crews.Side.Player).AllItems [(int)targetCat].Find (x => x.frenchName == itemName);

            if (item == null) {
				StoryReader.Instance.SetDecal (1);
			}

		} else {
			if (LootManager.Instance.getLoot (Crews.Side.Player).AllItems [(int)targetCat].Count == 0) {
				StoryReader.Instance.SetDecal (1);
			}
		}

		StoryReader.Instance.UpdateStory ();
	}
	#endregion

	public CategoryContent GetCategoryContent (CategoryContentType catContentType) {

        switch (catContentType) {

            case CategoryContentType.Inventory:

                return inventoryCategoryContent;

            case CategoryContentType.OtherLoot:

                return lootCategoryContent_Other;

            case CategoryContentType.PlayerLoot:

                return lootCategoryContent_Player;

            case CategoryContentType.PlayerTrade:

                return tradeCategoryContent_Player;

            case CategoryContentType.OtherTrade:

                return tradeCategoryContent_Other;

            case CategoryContentType.Combat:

                return tradeCategoryContent_Combat;

		}

		return defaultCategoryContent;
	}

	public Sprite getItemSprite (ItemCategory cat,int id) {

		switch (cat) {
		case ItemCategory.Provisions:

			if (foodSprites.Length == 0) {
				return null;
			}

                return foodSprites[id];


		case ItemCategory.Weapon:
                return CrewCreator.Instance.weaponSprites[id];
		case ItemCategory.Clothes:

			if (clotheSprites.Length == 0) {
				return null;
			}

                return clotheSprites[id];

		case ItemCategory.Misc:

			if (miscSprites.Length == 0) {
				Debug.LogError ("misc sprites est vide");
				return null;
			}

			if (id >= miscSprites.Length) {
				Debug.LogError ("item sprite id (" + id + ") est trop grand pour la liste (" + miscSprites.Length + ")");
				return miscSprites [0];
			}

                return miscSprites[id];

		default:

			if (miscSprites.Length == 0) {
                    return null;

			}

                return miscSprites[id];

		}
	}
}


public enum CategoryContentType {
	PlayerTrade,
	OtherTrade,
	Inventory,
	PlayerLoot,
	OtherLoot,
	Combat,
}
