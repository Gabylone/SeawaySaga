using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cuss : Skill {

	public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		string str = "Tu puerais pas un peu toi ?";
		fighter.Speak (str);

	}

	public override void ApplyEffect ()
	{

		base.ApplyEffect ();

		if ( fighter.TargetFighter.HasStatus(Fighter.Status.Toasted) ) {
			fighter.TargetFighter.RemoveStatus (Fighter.Status.Toasted,3);
		}

		fighter.TargetFighter.AddStatus (Fighter.Status.Cussed,3);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
			if (item.HasStatus(Fighter.Status.Cussed) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
