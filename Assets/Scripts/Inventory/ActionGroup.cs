using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class ActionGroup : MonoBehaviour {

	[SerializeField]
	private InventoryActionButton[] inventoryActionButtons;

	public enum ButtonType {

		Eat,
		Equip,
        PurchaseAndEquip,
        Unequip,
        Throw,
		Purchase,
		Sell,
		PickUp,

		None
	}

    public InventoryActionButton GetActionButton(ButtonType buttonType)
    {
        return inventoryActionButtons[(int)buttonType];
    }

    public void UpdateButtons(ButtonType[] buttonTypes) {

        HideAll();

        Item item = CrewMember.GetSelectedMember.GetEquipment(LootUI.Instance.SelectedItem.EquipmentPart);

        if (item != null
            &&
            LootUI.Instance.SelectedItem == item
            &&
            LootUI.Instance.currentSide == Crews.Side.Player
            &&
            LootUI.Instance.selectingEquipment
            )
        {
            GetActionButton(ButtonType.Unequip).Show();
            return;
        }
        else
        {

            foreach (var buttonType in buttonTypes)
            {
                GetActionButton(buttonType).Unlock();
                GetActionButton(buttonType).Show();
            }

            if (GetActionButton(ButtonType.Eat).visible)
            {
                if (!CrewMember.GetSelectedMember.HasHunger() && CrewMember.GetSelectedMember.HasMaxHealth())
                {
                    GetActionButton(ButtonType.Eat).Lock();
                }
            }
        }

        /*inventoryActionButtons[buttonTypeIndex].Unlock();
        inventoryActionButtons[buttonTypeIndex].Show();

		if (buttonTypes.Length > 1 && canThrow) {
            inventoryActionButtons[(int)buttonTypes[1]].Show();
		}*/
	}

    public void HideAll()
    {
        foreach (var item in inventoryActionButtons)
        {
            item.Hide();
        }
    }
}
