using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Jag : Skill {

//	public float healthAdded = 
	public float healthNeeded = 50;

	public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		CrewMember member = fighter.TargetFighter.crewMember;

        string str = "Take this, " + member.MemberName + " you'll feel better, I promise";
        fighter.Speak(str);
    }

	public override void HandleOnApplyEffect ()
	{

		base.HandleOnApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Jagged, 3);
		fighter.TargetFighter.RemoveStatus (Fighter.Status.Poisonned, 3);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {

			bool targetIsntJagged = item.HasStatus (Fighter.Status.Jagged) == false;
			bool targetNeedsHealing = item.crewMember.Health < healthNeeded;

			if ( targetIsntJagged && targetNeedsHealing ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
