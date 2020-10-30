using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class InGameMenu : MonoBehaviour {

	public static InGameMenu Instance;

	public delegate void OnOpenMenu ();
	public OnOpenMenu onOpenMenu;
	public delegate void OnCloseMenu ();
	public OnCloseMenu onCloseMenu;

    public delegate void OnDisplayCrewMember(CrewMember member);
    public OnDisplayCrewMember onDisplayCrewMember;

	[Header("Groups")]
	[SerializeField]
	public bool canOpen = true;

	public MenuButtons menuButtons;

	public void Lock () {
		canOpen = false;
	}
	public void Unlock () {
		canOpen = true;
	}

	[SerializeField]
	private Transform crewCanvas;

	public bool opened = false;

	void Awake () {
		Instance = this;

        onCloseMenu = null;
        onOpenMenu = null;
        StatButton.onClickStatButton = null;
	}

	public void Init () {

		LootUI.useInventory += HandleUseInventory;
	}

	#region event handling
	void HandleUseInventory (InventoryActionType actionType)
	{
		switch (actionType) {
		case InventoryActionType.Eat:
			EatItem ();
			break;
		case InventoryActionType.Equip:
			EquipItem ();
			break;
        case InventoryActionType.Unequip:
            UnequipItem();
            break;
            case InventoryActionType.Throw:
			ThrowItem ();
			break;
		case InventoryActionType.Sell:
			SellItem ();
			break;
		default:
			break;
		}

	}
    #endregion

    #region button action
    public void EatItem () {

		Item foodItem = LootUI.Instance.SelectedItem;

		int hunger = 0;
		int health = 0;

		switch (foodItem.spriteID) {
		// légume
		case 0:
            hunger = 4;
			health = 25;
			break;
		// poisson
		case 1:
            hunger = 6;
			health = 50;
			break;
		// viande
		case 2:
            hunger = 8;
			health = 75;
			break;
		default:
			break;
		}

        // les membres perde la faim seulement lorsque la barre de vie est pleine //
        /*if (CrewMember.GetSelectedMember.Health >= CrewMember.GetSelectedMember.MemberID.maxHealth - 10) {
			CrewMember.GetSelectedMember.CurrentHunger -= hunger;
		}*/

        SoundManager.Instance.PlayRandomSound("Food Eat");

        CrewMember.GetSelectedMember.CurrentHunger -= hunger;

        // les membres prennent la vie en meme temps qu'ils perde la faim //
        //CrewMember.GetSelectedMember.AddHealth(health);

        if ( CrewMember.GetSelectedMember.CurrentHunger <= 0)
        {
            CrewMember.GetSelectedMember.AddHealth(health);
        }

        if (OtherInventory.Instance.type == OtherInventory.Type.None)
			LootManager.Instance.PlayerLoot.RemoveItem (LootUI.Instance.SelectedItem);
		else
			LootManager.Instance.OtherLoot.RemoveItem (LootUI.Instance.SelectedItem);

	}

	void EquipItem ()
	{
		Item item = LootUI.Instance.SelectedItem;

        if (CrewMember.GetSelectedMember.CheckLevel(item.level) == false)
        {
            return;
        }

        SoundManager.Instance.PlayRandomSound("Foley Armour");
        SoundManager.Instance.PlayRandomSound("Anvil");

		if (CrewMember.GetSelectedMember.GetEquipment(item.EquipmentPart) != null)
			LootManager.Instance.PlayerLoot.AddItem (CrewMember.GetSelectedMember.GetEquipment(item.EquipmentPart) );

		CrewMember.GetSelectedMember.SetEquipment (item);

		if ( OtherInventory.Instance.type == OtherInventory.Type.None )
			LootManager.Instance.PlayerLoot.RemoveItem (item);
		else 
			LootManager.Instance.OtherLoot.RemoveItem (item);

	}

    public delegate void OnRemoveItemFromMember(Item item);
    public static OnRemoveItemFromMember onRemoveItemFromMember;

    private void UnequipItem()
    {
        SoundManager.Instance.PlayRandomSound("Foley Armour");

        LootManager.Instance.getLoot(Crews.Side.Player).AddItem(LootUI.Instance.SelectedItem);

        CrewMember.GetSelectedMember.RemoveEquipment(LootUI.Instance.SelectedItem.EquipmentPart);

        if (onRemoveItemFromMember != null)
            onRemoveItemFromMember(LootUI.Instance.SelectedItem);
    }

    public void ThrowItem () {

        SoundManager.Instance.PlayRandomSound("Whoosh");

		LootManager.Instance.PlayerLoot.RemoveItem (LootUI.Instance.SelectedItem);
	}

	public void SellItem () {

        SoundManager.Instance.PlayRandomSound("Coins");
        SoundManager.Instance.PlayRandomSound("coin");

		int price = 1 + (int)(LootUI.Instance.SelectedItem.price / 3f);

		GoldManager.Instance.AddGold (price);
		LootManager.Instance.OtherLoot.AddItem (LootUI.Instance.SelectedItem);
		LootManager.Instance.PlayerLoot.RemoveItem (LootUI.Instance.SelectedItem);

	}
	#endregion

	#region properties
	public void Open () {

		if (!canOpen) {
			return;
		}

        HideMenuButtons();

		    // event
		onOpenMenu ();

            // set bool
        opened = true;

        //Boats.Instance.WithdrawBoats();

        SoundManager.Instance.PlaySound("button_tap_light 01");
        SoundManager.Instance.PlaySound("Whoosh 01");

    }
	public void Hide () {

        ShowMenuButtons();

        // event
        onCloseMenu();

            // set bool
        opened = false;

        SoundManager.Instance.PlaySound("button_tap_light 01");
        SoundManager.Instance.PlaySound("Whoosh 07");

    }
    #endregion

    #region menu buttons
    public void ShowMenuButtons () {
		menuButtons.Show ();
	}
	public void HideMenuButtons () {
		menuButtons.Hide();
	}
	#endregion

}
