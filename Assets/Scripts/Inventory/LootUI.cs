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

    public bool selectingEquipment = false;


    // pour quand on revient sur les lieux d'un combat pour loot, il faut pas que l'histoire avance
    public bool preventAdvanceStory = false;

    public Crews.Side currentSide;
    public delegate void OnShowLoot();
    public static OnShowLoot tutorialEvent;

    

	private ItemCategory currentCat;

	public delegate void UseInventory ( InventoryActionType actionType );
	public static UseInventory useInventory;

	[SerializeField]
	private GameObject lootObj;

	public GameObject closeButton;
	public GameObject switchToPlayer;
	public GameObject switchToTrade;
    public Text switchButton_Text;
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
        tutorialEvent = null;
        useInventory = null;

		Init ();
	}

    void Start()
    {

        InGameMenu.onRemoveItemFromMember += HandleOnRemoveItemFromMember;

        RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;

        HideDelay();
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
    public void Show()
    {
        Show(categoryContentType, currentSide);
    }
	public void Show (CategoryContentType _catContentType,Crews.Side side) {

        currentPage = 0;

        ClearSelectedItem();

		currentSide = side;

		handledLoot = LootManager.Instance.getLoot(side);

		categoryContent = LootManager.Instance.GetCategoryContent(_catContentType);
		categoryContentType = _catContentType;

		InitButtons ();
		InitCategory();

        visible = true;

        SoundManager.Instance.PlaySound("Open Chest");

        if (tutorialEvent != null)
        {
            tutorialEvent();
        }

        lootObj.SetActive(true);

        OtherInventory.Instance.LerpIn();

        DisplayCrew.Instance.Show(CrewMember.GetSelectedMember);

        InGameMenu.Instance.Open();

        UpdateLootUI();
    }

    void HideDelay()
    {
        lootObj.SetActive(false);

    }

    public void Close()
    {
        if (!visible)
        {
            return;
        }

        InGameMenu.Instance.Hide();
        DisplayCrew.Instance.Hide();

        SoundManager.Instance.PlaySound("Close Chest");

        Hide();
    }

    public void Hide()
    {
        if (!visible)
        {
            return;
        }

        if (OnOtherLoot())
        {
            if (preventAdvanceStory)
            {
                preventAdvanceStory = false;
                Debug.Log("setting prevent story to FALSE");
            }
            else
            {
                StoryReader.Instance.NextCell();
            }

            StoryReader.Instance.UpdateStory();

            InGameMenu.Instance.Hide();

            Crews.getCrew(Crews.Side.Player).UpdateCrew(Crews.PlacingType.World);

            OtherInventory.Instance.type = OtherInventory.Type.None;

        }

        OtherInventory.Instance.LerpOut();

        Invoke("HideDelay", OtherInventory.Instance.lootTransition_Duration);

        visible = false;

    }

    public void HideAllSwitchButtons()
    {
        switchToTrade.SetActive(false);
        switchToLoot.SetActive(false);
        switchToPlayer.SetActive(false);
        takeAllButton.SetActive(false);

        previousPageButton.SetActive(false);
        nextPageButton.SetActive(false);

        DisplayCrew.Instance.switchGroup.SetActive(false);
    }

    public void InitButtons ()
	{
        HideAllSwitchButtons();

        switch (categoryContentType) {

		    case CategoryContentType.PlayerTrade:
                switchToTrade.SetActive(true);
                switchButton_Text.text = "Sell";
                break;
            case CategoryContentType.PlayerLoot:
			    switchToLoot.SetActive(true);
                switchButton_Text.text = "Throw";
                break;

		    case CategoryContentType.OtherTrade:
                switchButton_Text.text = "Sell";
                switchToPlayer.SetActive(true);
                break;
            case CategoryContentType.OtherLoot:
                switchButton_Text.text = "Drop";
                switchToPlayer.SetActive(true);

                if ( handledLoot.AllItems[(int)currentCat].Count > 0)
                {
                    takeAllButton.SetActive(true);
                }

                break;
            case CategoryContentType.Combat:
                break;
            case CategoryContentType.Inventory:
                DisplayCrew.Instance.switchGroup.SetActive(true);
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
    bool OnOtherLoot ()
    {
        return OtherInventory.Instance.type == OtherInventory.Type.Loot || OtherInventory.Instance.type == OtherInventory.Type.Trade;
    }
    #endregion

    #region take all
    public void TakeAll()
    {
        StartCoroutine(TakeAllCoroutine());
    }

    IEnumerator TakeAllCoroutine()
    {
        takeAllButton.SetActive(false);

        DisplayCrew.Instance.switchGroup.SetActive(false);

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
    #endregion

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
    void HandleOnRemoveItemFromMember(Item item)
    {
        UpdateLootUI();
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

        if (selectedItemDisplay.DisplayedItem != null)
        {
            selectedItemDisplay.UpdateUI();
        }
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

        SoundManager.Instance.PlayRandomSound("button_tap_light");

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

    public void NextPage()
    {
        currentPage++;
        UpdateLootUI();

        SoundManager.Instance.PlaySound("paper tap 01");
    }

    public void PreviousPage()
    {
        --currentPage;
        UpdateLootUI();

        SoundManager.Instance.PlaySound("paper tap 02");

    }

    public void OpenInventory()
    {
        //Show(categoryContentType, currentSide);
        Show(CategoryContentType.Inventory, Crews.Side.Player);

        /*if (visible)
        {
            UpdateLootUI();
        }
        else
        {
            Show(CategoryContentType.Inventory, Crews.Side.Player);
        }*/

        DisplayCrew.Instance.OnSwitchInventory();
        
    }

    public void OpenMemberLoot(CrewMember targetCrewMember)
    {
        
    }

    public void ClearSelectedItem()
    {
        selectedItemDisplay.Hide();
    }

    public Item SelectedItem
    {

        get
        {

            return selectedItem;
        }

        set
        {

            selectedItem = value;

            selectedItemDisplay.Show(value);

            UpdateActionButton(value);

            if (onSetSelectedItem != null)
            {
                onSetSelectedItem();
            }

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
