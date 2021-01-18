using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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

	public ScrollRect scrollRect;

	private Loot handledLoot;

    public Image background;
    public Color background_PlayerSideColor;
    public Color background_OtherSideColor;

	private Item selectedItem = null;

    public GameObject categoryObj;

    public GameObject lootTitle_Obj;
    public GameObject tradeTitle_Obj;

    public CanvasGroup lootFade_Up;
    public CanvasGroup lootFade_Down;
    public Image lootFade_Image_Up;
    public Image lootFade_Image_Down;
    public Color lootFade_OtherSideColor;
    public Color lootFade_PlayerSideColor;
    public float lootFade_Speed = 1f;
    public float lootFade_Buffer = 0.1f;

    List<Item> displayedItems = new List<Item>();

	public delegate void OnSetSelectedItem ();
	public static OnSetSelectedItem onSetSelectedItem;

    public bool selectingEquipment = false;

    // pour quand on revient sur les lieux d'un combat pour loot, il faut pas que l'histoire avance
    public bool preventAdvanceStory = false;

    public Crews.Side currentSide;
    public delegate void OnShowLoot();
    public static OnShowLoot tutorialEvent;

	private ItemCategory currentItemCategory;

	public delegate void UseInventory ( InventoryActionType actionType );
	public static UseInventory useInventory;

	[SerializeField]
	private GameObject lootObj;

	public GameObject closeButton;
	public CanvasGroup closeButton_canvasGroup;

	public GameObject switchToPlayer;
    public GameObject switchToTrade;
    public GameObject switchToLoot;

    public CanvasGroup switchToPlayer_CanvasGroup;
    public CanvasGroup switchToTrade_CanvasGroup;
    public CanvasGroup switchToLoot_CanvasGroup;

    public GameObject takeAllButton;

    public bool visible = false;

    [Header("Item Buttons")]
    public List<DisplayItem_Loot> displayItems = new List<DisplayItem_Loot>();
    public DisplayItem_Loot displayItem_Prefab;
    public RectTransform displayItem_Parent;

    public DisplayItem_Loot displayWeaponItem;
    public DisplayItem_Loot displayClotheItem;

    public DisplayItem_Selected selectedItemDisplay;

	[Header("Categories")]
	[SerializeField] private Button[] categoryButtons;
    private Transform[] categoryButtons_Transforms;
    private CanvasGroup[] categoryButtons_CanvasGroup;
    private CategoryContent categoryContent;
	public CategoryContentType categoryContentType;

	[Header("Actions")]
	[SerializeField]
	public ActionGroup actionGroup;

	void Awake () {
		Instance = this;

        onSetSelectedItem = null;
        tutorialEvent = null;
        useInventory = null;

        categoryButtons_Transforms = new Transform[categoryButtons.Length];
        categoryButtons_CanvasGroup = new CanvasGroup[categoryButtons.Length];
        for (int i = 0; i < categoryButtons_Transforms.Length; i++)
        {
            categoryButtons_Transforms[i] = categoryButtons[i].GetComponent<Transform>();
            categoryButtons_CanvasGroup[i] = categoryButtons[i].GetComponent<CanvasGroup>();
        }

        Init();
	}

    void Start()
    {

        InGameMenu.onRemoveItemFromMember += HandleOnRemoveItemFromMember;

        RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;

        HideDelay();

    }

    private void Update()
    {
        if (visible)
        {
            if ( scrollRect.verticalNormalizedPosition > 1 - lootFade_Buffer )
            {
                lootFade_Down.alpha = Mathf.Lerp( lootFade_Down.alpha , 1f , lootFade_Speed * Time.deltaTime );
            }
            else
            {
                lootFade_Down.alpha = Mathf.Lerp(lootFade_Down.alpha, 0f, lootFade_Speed * Time.deltaTime);

            }

            if (scrollRect.verticalNormalizedPosition < lootFade_Buffer)
            {
                lootFade_Up.alpha = Mathf.Lerp(lootFade_Up.alpha, 1f, lootFade_Speed * Time.deltaTime);
            }
            else
            {
                lootFade_Up.alpha = Mathf.Lerp(lootFade_Up.alpha, 0f, lootFade_Speed * Time.deltaTime);

            }
        }
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

        scrollRect.verticalNormalizedPosition = 1f;

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

    public void InitButtons ()
	{
        // hide skill / / inventory
        DisplayCrew.Instance.HideSkillSwitchGroup();

        switchToLoot.SetActive(false);
        switchToTrade.SetActive(false);
        switchToPlayer.SetActive(false);

        lootTitle_Obj.SetActive(false);
        tradeTitle_Obj.SetActive(false);
        categoryObj.SetActive(false);

        takeAllButton.SetActive(false);

        background.color = background_PlayerSideColor;

        lootFade_Image_Down.color = lootFade_PlayerSideColor;
        lootFade_Image_Up.color = lootFade_PlayerSideColor;

        switch (categoryContentType) {

            case CategoryContentType.PlayerTrade:

                switchToPlayer.SetActive(true);
                switchToPlayer_CanvasGroup.alpha = 0.5f;

                switchToTrade.SetActive(true);
                switchToTrade_CanvasGroup.alpha = 1f;

                categoryObj.SetActive(true);

                break;
            case CategoryContentType.PlayerLoot:

                switchToPlayer.SetActive(true);
                switchToPlayer_CanvasGroup.alpha = 0.5f;

                switchToLoot.SetActive(true);
                switchToLoot_CanvasGroup.alpha = 1f;

                categoryObj.SetActive(true);
                break;

            case CategoryContentType.OtherTrade:

                switchToPlayer.SetActive(true);
                switchToPlayer_CanvasGroup.alpha = 1f;

                switchToTrade.SetActive(true);
                switchToTrade_CanvasGroup.alpha = 0.5f;

                tradeTitle_Obj.SetActive(true);

                background.color = background_OtherSideColor;
                lootFade_Image_Down.color = lootFade_OtherSideColor;
                lootFade_Image_Up.color = lootFade_OtherSideColor;


                break;
            case CategoryContentType.OtherLoot:

                switchToPlayer.SetActive(true);
                switchToPlayer_CanvasGroup.alpha = 1f;

                switchToLoot_CanvasGroup.alpha = 0.5f;
                switchToLoot.SetActive(true);


                background.color = background_OtherSideColor;
                lootFade_Image_Down.color = lootFade_OtherSideColor;
                lootFade_Image_Up.color = lootFade_OtherSideColor;

                lootTitle_Obj.SetActive(true);

                if ( displayedItems.Count > 0)
                {
                    takeAllButton.SetActive(true);
                }
                else
                {
                    takeAllButton.SetActive(false);
                }

                break;
            case CategoryContentType.Combat:
                break;
            case CategoryContentType.Inventory:

                categoryObj.SetActive(true);

                DisplayCrew.Instance.ShowSkillSwitchGroup();
                switchToPlayer.SetActive(false);
                switchToTrade.SetActive(false);
                switchToLoot.SetActive(false);
                break;
		default:
			break;
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

        closeButton.SetActive(false);

        while (displayedItems.Count > 0)
        {
            Item targetItem = displayedItems[0];

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
    void DisplayMemberEquipement()
    {
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
        if (clothe != null)
        {
            displayClotheItem.Show(clothe);
        }
        else
        {
            displayClotheItem.gameObject.SetActive(false);
        }
    }
	public void UpdateItemButtons (ItemCategory[] itemCategories) {

        DisplayMemberEquipement();

        for (int i = 0; i < displayItems.Count; i++)
        {
            displayItems[i].Hide();
        }

        int itemIndex = 0;

        displayedItems.Clear();

        foreach (var category in itemCategories)
        {
            foreach (var item in handledLoot.GetCategory(category))
            {

                DisplayItem_Loot displayItem;

                if ( itemIndex >= displayItems.Count)
                {
                    displayItem = NewDisplayItem();
                }
                else
                {
                    displayItem = displayItems[itemIndex];
                }

                displayItem.Show(item);

                itemIndex++;

                displayedItems.Add(item);
            }
        }

        /*for (int i = 0; i < handledLoot.AllItems[(int)currentCat].Count; ++i ) {

			DisplayItem_Loot displayItem = displayItems [i];

            Item item = handledLoot.AllItems[(int)currentCat][i];

            displayItem.Show(item);
		}*/

    }

    private DisplayItem_Loot NewDisplayItem()
    {
        DisplayItem_Loot displayItem_Loot = Instantiate(displayItem_Prefab, displayItem_Parent);

        displayItems.Add(displayItem_Loot);

        return displayItem_Loot;
    }

    void HandleOnRemoveItemFromMember(Item item)
    {
        UpdateLootUI();
    }
    #endregion

    #region category navigation
    void InitCategory ()
	{
        if ( currentSide == Crews.Side.Enemy)
        {
            categoryObj.SetActive(false);

            if ( OtherInventory.Instance.type == OtherInventory.Type.Loot)
            {
                lootTitle_Obj.SetActive(true);
                tradeTitle_Obj.SetActive(false);
            }
            else
            {
                lootTitle_Obj.SetActive(false);
                tradeTitle_Obj.SetActive(true);
            }

            return;
        }

        lootTitle_Obj.SetActive(false);
        categoryObj.SetActive(true);

        /*for (int catIndex = 0; catIndex < categoryButtons.Length; catIndex++) {

			if ( handledLoot.AllItems[catIndex].Count > 0 ) {
				currentItemCategory = (ItemCategory)catIndex;
				return;
			}

		}*/

        Invoke("SwitchToCurrentCategory", 0.01f);

    }
    void SwitchToCurrentCategory()
    {
        SwitchCategory(currentItemCategory);
    }

    public void SwitchCategory ( int cat ) {
		SwitchCategory ((ItemCategory)cat);
	}
	public void SwitchCategory ( ItemCategory cat ) {

		if ( DisplayItem_Loot.selectedDisplayItem != null )
			DisplayItem_Loot.selectedDisplayItem.Deselect ();

        categoryButtons_CanvasGroup[(int)currentItemCategory].alpha = 1f;
 
		currentItemCategory = cat;

        ClearSelectedItem();

		UpdateLootUI ();

        scrollRect.verticalNormalizedPosition = 1f;

        foreach (var item in categoryButtons_CanvasGroup)
        {
            item.alpha = 1f;
        }

        Tween.Bounce(categoryButtons_Transforms[(int)currentItemCategory], 0.2f, 1.1f);
        categoryButtons_CanvasGroup[(int)currentItemCategory].alpha = 0.5f;
    }

	public void UpdateLootUI () {

		if (!visible)
			return;

        if ( currentSide == Crews.Side.Enemy)
        {
            ItemCategory[] cats = new ItemCategory[4] { ItemCategory.Provisions, ItemCategory.Weapon, ItemCategory.Clothes, ItemCategory.Misc };

            UpdateItemButtons(cats);
        }
        else
        {
            UpdateItemButtons(new ItemCategory[1] { currentItemCategory });
        }

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

		categoryButtons [(int)currentItemCategory].interactable = false;
        foreach (var item in categoryButtons[(int)currentItemCategory].GetComponentsInChildren<Image>())
        {
            item.color = Color.white;
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

		actionGroup.UpdateButtons (CategoryContent.catButtonType[(int)item.category].buttonTypes);

	}
	#endregion

    public void NextPage()
    {
        UpdateLootUI();

        SoundManager.Instance.PlaySound("paper tap 01");
    }

    public void PreviousPage()
    {
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
