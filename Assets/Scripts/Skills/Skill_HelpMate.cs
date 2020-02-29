using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HelpMate : Skill {

	public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		string str = "Courage, " + fighter.TargetFighter.crewMember.MemberName + " ! Tu peux le faire, fiston";
		fighter.Speak (str);

	}

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Protected, 2);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {
			if (item.HasStatus(Fighter.Status.Protected) == false ) {
				hasTarget = true;
				preferedTarget = item;

				if (item.HasStatus (Fighter.Status.Provoking))
					break;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
