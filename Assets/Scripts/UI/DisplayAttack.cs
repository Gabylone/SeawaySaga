using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayAttack : MonoBehaviour {

	public Text uiText;

	// Use this for initialization
	void Start () {

        InGameMenu.Instance.onDisplayCrewMember += HandleOnDisplayCrewMember;
		LootUI.useInventory += HandleUseInventory;
        SkillManager.Instance.onLevelUpStat += HandleOnClickStatButton;
		InGameMenu.onRemoveItemFromMember += HandleOnRemoveItemFromMember;

		UpdateUI (CrewMember.GetSelectedMember);
	}

	void HandleOnRemoveItemFromMember (Item item)
	{
		UpdateUI (CrewMember.GetSelectedMember);	
	}

	void HandleOnClickStatButton ()
	{
		UpdateUI (CrewMember.GetSelectedMember);
	}

	void HandleOnDisplayCrewMember (CrewMember crewMember)
	{
		UpdateUI (crewMember);
	}

	void UpdateUI (CrewMember crewMember) {
		uiText.text = crewMember.Attack.ToString ();
		Tween.Bounce (transform);
	}

	void HandleUseInventory (InventoryActionType actionType)
	{

		switch (actionType) {
		case InventoryActionType.Equip:
		case InventoryActionType.PurchaseAndEquip:
			UpdateUI (CrewMember.GetSelectedMember);
			break;
		default:
			break;
		}
	}
}
