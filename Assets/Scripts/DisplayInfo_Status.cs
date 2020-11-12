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
			str = "Knocked Out by a Brute ! The member passed his next turn";
			break;
		case Fighter.Status.PreparingAttack:
			str = "The member's preparing to attack big, next turn";
			break;
		case Fighter.Status.Enraged:
			str = "The Brute gains 10 energy points at the start of the turn";
			break;
		case Fighter.Status.Jagged:
			str = "The member gains 10 health points at the start of each turn";
			break;
		case Fighter.Status.Poisonned:
			str = "The member loses 10 health points at the start of each turn";
			break;
		case Fighter.Status.Provoking:
			str = "The next 3 attack are directed to this member, as he's being very offensive and he's calling everybody names";
			break;
		case Fighter.Status.Protected:
			str = "Pumped up and looked over by a Cook, the damages directed to this member are halved";
			break;
		case Fighter.Status.Toasted:
                str = "After a cheerful toast with the Cook, the member does significantly more damage";
			break;
		case Fighter.Status.BearTrapped:
			str = "The Filibuster layed a trap, his next attacker loses 30 health points";
			break;
		case Fighter.Status.Cussed:
			str = "Insulted and humiliated by the Crook, the member only does half his damage";
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
                str = "Preparing Attack";
                break;
            case Fighter.Status.Enraged:
                str = "Enraged";
                break;
            case Fighter.Status.Jagged:
                str = "Jagged";
                break;
            case Fighter.Status.Poisonned:
                str = "Poisonned";
                break;
            case Fighter.Status.Provoking:
                str = "Provoking";
                break;
            case Fighter.Status.Protected:
                str = "Protected";
                break;
            case Fighter.Status.Toasted:
                str = "Toasted";
                break;
            case Fighter.Status.BearTrapped:
                str = "Bear Trapped";
                break;
            case Fighter.Status.Cussed:
                str = "Cussed";
                break;
            case Fighter.Status.None:
                break;
            default:
                break;
        }

        return str;
    }
}
