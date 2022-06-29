using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class InGameMenu : MonoBehaviour
{

    public static InGameMenu Instance;

    public delegate void OnOpenMenu();
    public OnOpenMenu onOpenMenu;
    public delegate void OnCloseMenu();
    public OnCloseMenu onCloseMenu;

    public delegate void OnDisplayCrewMember(CrewMember member);
    public OnDisplayCrewMember onDisplayCrewMember;

    [Header("Groups")]
    [SerializeField]
    public bool canOpen = true;

    public MenuButtons menuButtons;

    public void Lock()
    {
        canOpen = false;
    }
    public void Unlock()
    {
        canOpen = true;
    }

    [SerializeField]
    private Transform crewCanvas;

    public bool opened = false;

    void Awake()
    {
        Instance = this;

        onCloseMenu = null;
        onOpenMenu = null;
    }

    public void Init()
    {

        LootUI.useInventory += HandleUseInventory;
    }

    #region event handling
    void HandleUseInventory(InventoryActionType actionType)
    {
        switch (actionType)
        {
            case InventoryActionType.Eat:
                EatItem();
                break;
            case InventoryActionType.Equip:
                EquipItem();
                break;
            case InventoryActionType.Unequip:
                UnequipItem();
                break;
            case InventoryActionType.Throw:
                ThrowItem();
                break;
            case InventoryActionType.Sell:
                SellItem();
                break;
            default:
                break;
        }

    }
    #endregion

    #region button action
    public void EatItem()
    {
        Item foodItem = LootUI.Instance.SelectedItem;

        EatItem(foodItem, CrewMember.GetSelectedMember);

    }
    public void EatItem(Item foodItem, CrewMember crewMember)
    {

        // can buy
        if (OtherInventory.Instance.type == OtherInventory.Type.Trade)
        {
            if (!GoldManager.Instance.CheckGold(foodItem.price))
            {
                return;
            }

            SoundManager.Instance.PlayRandomSound("Bag");
            SoundManager.Instance.PlayRandomSound("Coins");

            GoldManager.Instance.RemoveGold(foodItem.price);
        }
        //

        int health = 0;

        switch (foodItem.spriteID)
        {
            // légume
            case 0:
                health = 25;
                break;
            // poisson
            case 1:
                health = 50;
                break;
            // viande
            case 2:
                health = 75;
                break;
            default:
                break;
        }


        crewMember.AddHealth(health);
        crewMember.CurrentHunger -= foodItem.value;

        /*if ( CrewMember.GetSelectedMember.CurrentHunger <= 0)
        {
            CrewMember.GetSelectedMember.AddHealth(health);
        }
        else
        {
            CrewMember.GetSelectedMember.CurrentHunger -= hunger;
        }*/

        if (OtherInventory.Instance.type == OtherInventory.Type.None)
            LootManager.Instance.PlayerLoot.RemoveItem(foodItem);
        else
            LootManager.Instance.OtherLoot.RemoveItem(foodItem);

    }

    IEnumerator FoodFeedbackCoroutine()
    {
        SoundManager.Instance.PlayRandomSound("Bag");
        SoundManager.Instance.PlayRandomSound("Food Eat");
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlayRandomSound("Bag");
        SoundManager.Instance.PlayRandomSound("Food Eat");
        yield return new WaitForSeconds(0.2f);
        SoundManager.Instance.PlayRandomSound("Food Eat");
    }

    void EquipItem()
    {
        Item item = LootUI.Instance.SelectedItem;

        if (CrewMember.GetSelectedMember.CheckLevel(item.level) == false)
        {
            return;
        }

        if (OtherInventory.Instance.type == OtherInventory.Type.Loot)
        {
            if (!WeightManager.Instance.CheckWeight(item.weight))
            {
                return;
            }
        }

        // equip during trade
        if (OtherInventory.Instance.type == OtherInventory.Type.Trade)
        {
            if (!WeightManager.Instance.CheckWeight(item.weight))
            {
                return;
            }

            if (!GoldManager.Instance.CheckGold(LootUI.Instance.SelectedItem.price))
            {
                return;
            }

            SoundManager.Instance.PlayRandomSound("Bag");
            SoundManager.Instance.PlayRandomSound("Coins");

            GoldManager.Instance.RemoveGold(LootUI.Instance.SelectedItem.price);
        }

        // equip 
        SoundManager.Instance.PlayRandomSound("Foley Armour");
        SoundManager.Instance.PlayRandomSound("Anvil");

        // replace equipemnt
        if (CrewMember.GetSelectedMember.GetEquipment(item.EquipmentPart) != null)
        {
            LootManager.Instance.PlayerLoot.AddItem(CrewMember.GetSelectedMember.GetEquipment(item.EquipmentPart));
        }

        CrewMember.GetSelectedMember.SetEquipment(item);

        if (OtherInventory.Instance.type == OtherInventory.Type.None)
        {
            LootManager.Instance.PlayerLoot.RemoveItem(item);
        }
        else
        {
            LootManager.Instance.OtherLoot.RemoveItem(item);
        }

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

    public void ThrowItem()
    {

        SoundManager.Instance.PlayRandomSound("Whoosh");

        LootManager.Instance.PlayerLoot.RemoveItem(LootUI.Instance.SelectedItem);
    }

    public void SellItem()
    {

        SoundManager.Instance.PlayRandomSound("Coins");
        SoundManager.Instance.PlayRandomSound("coin");

        int price = 1 + (int)(LootUI.Instance.SelectedItem.price / 3f);

        GoldManager.Instance.AddGold(price);
        LootManager.Instance.OtherLoot.AddItem(LootUI.Instance.SelectedItem);
        LootManager.Instance.PlayerLoot.RemoveItem(LootUI.Instance.SelectedItem);

    }
    #endregion

    #region properties
    public void Open()
    {

        if (!canOpen)
        {
            return;
        }

        if (opened)
        {
            return;
        }

        HideMenuButtons();

        // event
        onOpenMenu();

        // set bool
        opened = true;

        //Boats.Instance.WithdrawBoats();

        SoundManager.Instance.PlaySound("button_tap_light 01");
        SoundManager.Instance.PlaySound("Whoosh 01");

    }
    public void Hide()
    {

        if (!opened)
        {
            return;
        }

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
    public void ShowMenuButtons()
    {
        menuButtons.Show();
    }
    public void HideMenuButtons()
    {
        menuButtons.Hide();
    }
    #endregion

}
