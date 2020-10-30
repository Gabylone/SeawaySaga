using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHunger_CrewMenu : DisplayHunger {

    public Text uiText;

	// Use this for initialization
	public override void Start ()
	{
		base.Start ();

		InGameMenu.Instance.onDisplayCrewMember += HandleOnDisplayCrewMember;
		LootUI.useInventory += HandleUseInventory;

        HandleOnDisplayCrewMember(CrewMember.GetSelectedMember);
	}

	void HandleOnDisplayCrewMember(CrewMember crewMember)
	{
		UpdateHungerIcon (CrewMember.GetSelectedMember);
	}

	void HandleUseInventory (InventoryActionType actionType)
	{
		if ( actionType == InventoryActionType.Eat ) {
			UpdateHungerIcon (CrewMember.GetSelectedMember);
		}
	}

    public override void UpdateHungerIcon(CrewMember member)
    {
        base.UpdateHungerIcon(member);

        int i = (member.MaxHunger - member.CurrentHunger);

        if ( i == 1)
        {
            uiText.text = "" + (member.MaxHunger - member.CurrentHunger) + " trip";
            // ou XXX trips before hunger ?
        }
        else
        {
            uiText.text = "" + (member.MaxHunger - member.CurrentHunger) + " trips";
        }

    }

    void OnDestroy () {
		LootUI.useInventory -= HandleUseInventory;
	}
}
