using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_RatPoison : Skill {

	

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Poisonned, 3);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
//		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {
			if (item.HasStatus(Fighter.Status.Poisonned) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
