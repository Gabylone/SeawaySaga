using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum InventoryActionType {

	Eat,
	Equip,

    PurchaseAndEquip,
    Unequip,

    Throw,
	Sell,
	Buy,
	PickUp,

}

public class LootUI : MonoBehaviour {

	public static LootUI Instance;

	public RectTransform scollViewRectTransform;

	private Loot handledLoot;

	private Item selectedItem = null;

	public delegate void OnSetSelectedItem ();
	public static OnSetSelectedItem onSetSelectedItem;

    public void ClearSelectedItem()
    {
        selectedItemDisplay.Hide();
    }

    public Item SelectedItem {
		
		get {

			return selectedItem;
		}

		set {
			
			selectedItem = value;

            selectedItemDisplay.Show(value);

			UpdateActionButton (value);

			if ( onSetSelectedItem != null  ) {
				onSetSelectedItem ();
			}

		}
	}

	private ItemCategory currentCat;

	public delegate void UseInventory ( InventoryActionType actionType );
	public static UseInventory useInventory;

	[SerializeField]
	private GameObject lootObj;

	public GameObject closeButton;
	public GameObject switchToPlayer;
	public GameObject switchToTrade;
	public GameObject switchToLoot;
    public GameObject takeAllButton;

    public bool visible = false;

    [Header("Item Buttons")]
    public DisplayItem_Loot[] displayItems = new DisplayItem_Loot[0];

    public DisplayItem_Loot displayWeaponItem;
    public DisplayItem_Loot displayClotheItem;

    public DisplayItem_Selected selectedItemDisplay;

	[Header("Categories")]
	[SerializeField] private Button[] categoryButtons;
	private CategoryContent categoryContent;
	public CategoryContentType categoryContentType;
	public Transform selectedParent;
	public Transform initParent;

	[Header("Pages")]
	[SerializeField] private GameObject previousPageButton;
	[SerializeField] private GameObject nextPageButton;
	private int currentPage 	= 0;
	private int maxPage 		= 0;

	[Header("Actions")]
	[SerializeField]
	public ActionGroup actionGroup;

	void Awake () {
		Instance = this;

        onSetSelectedItem = null;
        onHideLoot = null;
        onShowLoot = null;
        useInventory = null;

		Init ();
	}

	void Start () {

		InGameMenu.onRemoveItemFromMember += HandleOnRemoveItemFromMember;

		RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;
	}

	void HandleOnTouchRayBlocker ()
	{
	}

	public void DeselectCurrentItem(){
		if (DisplayItem_Loot.selectedDisplayItem != null) {
			DisplayItem_Loot.selectedDisplayItem.Deselect ();
		}
	}

	private void Init () {

		int a = 0;
		foreach ( DisplayItem_Loot itemButton in displayItems ) {
			itemButton.index = a;
			++a;
		}

		Hide ();
	}

	#region show / hide
	public Crews.Side currentSide;
	public delegate void OnShowLoot ();
	public static OnShowLoot onShowLoot;
	public void Show (CategoryContentType _catContentType,Crews.Side side) {

        currentPage = 0;

        ClearSelectedItem();

		currentSide = side;

		handledLoot = LootManager.Instance.getLoot(side);

		categoryContent = LootManager.Instance.GetCategoryContent(_catContentType);
		categoryContentType = _catContentType;

		InitButtons ();
		InitCategory();

        closeButton.SetActive(OnOtherLoot());

        visible = true;
        lootObj.SetActive(true);

        UpdateLootUI();

        if (onShowLoot != null)
			onShowLoot ();

        OtherInventory.Instance.LerpIn();

        InGameMenu.Instance.Open();

    }

    public void Close()
    {
        InGameMenu.Instance.Hide();
        DisplayCrew.Instance.Hide();

        visible = false;

        if (onHideLoot != null)
        {
            onHideLoot();
        }

        OtherInventory.Instance.LerpOut();

        Hide();
    }

    public void TakeAll()
    {
        StartCoroutine(TakeAllCoroutine());
    }

    IEnumerator TakeAllCoroutine()
    {
        takeAllButton.SetActive(false);
        closeButton.SetActive(false);

        HideAllSwitchButtons();

        while (handledLoot.AllItems[(int)currentCat].Count > 0)
        {
            Item targetItem = handledLoot.AllItems[(int)currentCat][0];

            if (!WeightManager.Instance.CheckWeight(targetItem.weight))
            {
                break;
            }

            LootUI.Instance.SelectedItem = targetItem;
            InventoryAction(InventoryActionType.PickUp);

            yield return new WaitForSeconds(0.1f);

        }

        closeButton.SetActive(true);

        InitButtons();
    }

    public void HideAllSwitchButtons()
    {
        switchToTrade.SetActive(false);
        switchToLoot.SetActive(false);
        switchToPlayer.SetActive(false);
        takeAllButton.SetActive(false);

        previousPageButton.SetActive(false);
        nextPageButton.SetActive(false);
    }

    public void InitButtons ()
	{
        HideAllSwitchButtons();

        switch (categoryContentType) {

		    case CategoryContentType.PlayerTrade:
                switchToTrade.SetActive(true);
                break;
            case CategoryContentType.PlayerLoot:
			    switchToLoot.SetActive(true);
			    break;

		    case CategoryContentType.OtherTrade:
                switchToPlayer.SetActive(true);
                break;
            case CategoryContentType.OtherLoot:

                switchToPlayer.SetActive(true);

                if ( handledLoot.AllItems[(int)currentCat].Count > 0)
                {
                    takeAllButton.SetActive(true);
                }
                break;
		default:
			break;
		}

        if (currentPage > 0)
        {
            previousPageButton.SetActive(true);
        }

        if (handledLoot.AllItems[(int)currentCat].Count > ItemPerPage * (currentPage+1) )
        {
            nextPageButton.SetActive(true);
        }

    }

    public delegate void OnHideLoot ();
	public static OnHideLoot onHideLoot;
    public void Hide()
    {
        lootObj.SetActive(false);

        if (OnOtherLoot())
        {

            StoryReader.Instance.NextCell();
            StoryReader.Instance.UpdateStory();

            InGameMenu.Instance.Hide();

            Crews.getCrew(Crews.Side.Player).UpdateCrew(Crews.PlacingType.World);
        }

        OtherInventory.Instance.type = OtherInventory.Type.None;
    }
    bool OnOtherLoot ()
    {
        return OtherInventory.Instance.type == OtherInventory.Type.Loot || OtherInventory.Instance.type == OtherInventory.Type.Trade;
    }
	#endregion

	void HandleOnRemoveItemFromMember (Item item)
	{
		UpdateLootUI ();
	}

	#region item button	
	public void UpdateItemButtons () {

        Item weapon = CrewMember.GetSelectedMember.GetEquipment(CrewMember.EquipmentPart.Weapon);
        if (weapon != null)
        {
            displayWeaponItem.Show(weapon);
        }
        else
        {
            displayWeaponItem.gameObject.SetActive(false);
        }

        Item clothe = CrewMember.GetSelectedMember.GetEquipment(CrewMember.EquipmentPart.Clothes);
        if ( clothe != null)
        {
            displayClotheItem.Show(clothe);
        }
        else
        {
            displayClotheItem.gameObject.SetActive(false);
        }

        int itemIndex = currentPage * ItemPerPage;
        int l = ItemPerPage;

        for (int i = 0; i < l; ++i ) {

			DisplayItem_Loot displayItem = displayItems [i];

			if ( itemIndex < handledLoot.AllItems [(int)currentCat].Count ) {

                Item item = handledLoot.AllItems[(int)currentCat][itemIndex];

                displayItem.Show(item);

            }
            else
            {
                displayItem.Hide();
            }

            itemIndex++;
		}


	}
	#endregion

	#region category navigation
	void InitCategory ()
	{
		for (int catIndex = 0; catIndex < categoryButtons.Length; catIndex++) {

			if ( handledLoot.AllItems[catIndex].Count > 0 ) {
				currentCat = (ItemCategory)catIndex;
				return;
			}

		}
	}
	public void SwitchCategory ( int cat ) {
		SwitchCategory ((ItemCategory)cat);
	}
	public void SwitchCategory ( ItemCategory cat ) {

		if ( DisplayItem_Loot.selectedDisplayItem != null )
			DisplayItem_Loot.selectedDisplayItem.Deselect ();

		currentCat = cat;

		Tween.Bounce (categoryButtons[(int)cat].transform, 0.2f , 1.1f);

		currentPage = 0;

        ClearSelectedItem();

		UpdateLootUI ();

		scollViewRectTransform.anchoredPosition = Vector2.zero;

	}

	public void UpdateLootUI () {

		if (!visible)
			return;

		UpdateItemButtons ();
		UpdateCategoryButtons ();
        InitButtons();

	}

	public CategoryContent CategoryContent {
		get {

			if (categoryContent == null) {
				Debug.LogError ("pas de category content");
				return null;
			}

			return categoryContent;
		}
			
	}

	private void UpdateCategoryButtons () {

		for (int buttonIndex = 0; buttonIndex < categoryButtons.Length; ++buttonIndex) {

			categoryButtons [buttonIndex].transform.SetParent (initParent);

			if ( handledLoot.AllItems[buttonIndex].Count == 0 ) {

                categoryButtons[buttonIndex].interactable = false;
                foreach (var item in categoryButtons[buttonIndex].GetComponentsInChildren<Image>())
                {
                    item.color = LootManager.Instance.item_EmptyColor;
                }

            }
            else
			{
				categoryButtons [buttonIndex].interactable = true;
                foreach (var item in categoryButtons[buttonIndex].GetComponentsInChildren<Image>())
                {
                    item.color = Color.white;
                }
            }
        }

		categoryButtons [(int)currentCat].interactable = false;
        foreach (var item in categoryButtons[(int)currentCat].GetComponentsInChildren<Image>())
        {
            item.color = Color.white;
        }
		categoryButtons [(int)currentCat].transform.SetParent (selectedParent);
	}
	#endregion

	#region page navigation
	public int ItemPerPage {
		get {
			return displayItems.Length;
		}
	}
	#endregion

	#region action button
	public void InventoryAction ( InventoryActionType inventoryActionType ) {

		if (useInventory != null)
			useInventory (inventoryActionType);
		else
			print ("no function liked to the event : use inventory");

		UpdateLootUI ();

		if (DisplayItem_Loot.selectedDisplayItem != null)
			DisplayItem_Loot.selectedDisplayItem.Deselect ();

        ClearSelectedItem();

	}
	public void UpdateActionButton (Item item) {

		if ( item == null ) {
            actionGroup.HideAll();
			return;
		}

		actionGroup.UpdateButtons (CategoryContent.catButtonType[(int)currentCat].buttonTypes);

	}
	#endregion

	public GameObject LootObj {
		get {
			return lootObj;
		}
		set {
			lootObj = value;
		}
	}

    public void NextPage()
    {
        currentPage++;
        UpdateLootUI();
    }

    public void PreviousPage()
    {
        --currentPage;
        UpdateLootUI();
    }

    public void OpenMemberLoot()
    {
        OpenMemberLoot(CrewMember.GetSelectedMember);
    }

    public void OpenMemberLoot(CrewMember targetCrewMember)
    {
        // basic open the menu stuff
        InGameMenu.Instance.Open();
        DisplayCrew.Instance.Show(targetCrewMember);

        if (BoatUpgradeManager.Instance.opened)
        {
            BoatUpgradeManager.Instance.Close();
        }

        if (visible)
        {
            UpdateLootUI();
        }
        else
        {
            Show(CategoryContentType.Inventory, Crews.Side.Player);
        }
    }

}

[System.Serializable]
public class CategoryContent {
	public CategoryButtonType[] catButtonType = new CategoryButtonType[4];
}

[System.Serializable]
public class CategoryButtonType {
	
	public ActionGroup.ButtonType[] buttonTypes = new ActionGroup.ButtonType[2];

	public ActionGroup.ButtonType this [int i] {
		get {
			return buttonTypes [i];
		}
		set {
			buttonTypes [i] = value;
		}
	}
}
