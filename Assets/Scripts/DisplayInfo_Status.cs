using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayInfo_Status : DisplayInfo {

	public override void Start ()
	{
		base.Start ();

		StatusFeedback.onTouchStatusFeedback += HandleOnTouchStatusFeedback;
	}

	void HandleOnTouchStatusFeedback (Fighter.Status status)
	{
		string str = "";

		switch (status) {
		case Fighter.Status.KnockedOut:
			str = "Skips the next turn";
			break;
		case Fighter.Status.PreparingAttack:
			str = "After waiting for 1 turn, deals 3 times the normal damage";
			break;
		case Fighter.Status.Enraged:
			str = "Gains full energy at the start of the turn and deals x1.5 damage";
			break;
		case Fighter.Status.Jagged:
			str = "Gets 10 health points back at the start of the next 3 turns";
			break;
		case Fighter.Status.Poisonned:
			str = "Loses 10 health points at the start of the next 3 turns";
			break;
		case Fighter.Status.Provoking:
			str = "Becomes the target of the enemy's next 3 attacks";
		    break;
        case Fighter.Status.Parrying:
            str = "Halves the damage taken from the next attack";
            break;
		case Fighter.Status.Protected:
			str = "Halves the damage taken from the next 2 attacks";
			break;
		case Fighter.Status.Toasted:
                str = "Deals double damage for the next 2 attacks";
			break;
		case Fighter.Status.BearTrapped:
			str = "Deals 30 damage to the next melee attacker who triggers the trap";
			break;
		case Fighter.Status.Cussed:
			str = "Deals half damage for the next 2 attacks";
			break;
		case Fighter.Status.None:
			break;
		default:
			break;
		}

		confirmGroup.SetActive (true);

		Display (GetTitle(status),str);

		Move (Corner.None);

	}

    public string GetTitle(Fighter.Status status)
    {
        string str = "";

        switch (status)
        {
            case Fighter.Status.KnockedOut:
                str = "Knocked Out";
                break;
            case Fighter.Status.PreparingAttack:
                str = "Focused";
                break;
            case Fighter.Status.Enraged:
                str = "Enraged";
                break;
            case Fighter.Status.Jagged:
                str = "Healed";
                break;
            case Fighter.Status.Poisonned:
                str = "Poisoned";
                break;
            case Fighter.Status.Provoking:
                str = "Chewing";
                break;
            case Fighter.Status.Parrying:
                str = "Parrying";
                break;
            case Fighter.Status.Protected:
                str = "Pumped Up";
                break;
            case Fighter.Status.Toasted:
                str = "Tipsy";
                break;
            case Fighter.Status.BearTrapped:
                str = "Trap";
                break;
            case Fighter.Status.Cussed:
                str = "Insulted";
                break;
            case Fighter.Status.None:
                break;
            default:
                break;
        }

        return str;
    }
}
