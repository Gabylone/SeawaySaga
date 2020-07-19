using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HelpMate : Skill {

	public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		string str = "Cheer up, " + fighter.TargetFighter.crewMember.MemberName + " ! You can do it, son";
		fighter.Speak (str);

	}

	public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

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
