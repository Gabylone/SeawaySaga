using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHunger_CrewMenu : DisplayHunger {

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

	void OnDestroy () {
		LootUI.useInventory -= HandleUseInventory;
	}
}
