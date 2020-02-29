using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Goad : Skill {

	public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		string str = fighter.TargetFighter.crewMember.MemberName + " a dit que vous étiez moches";
		fighter.Speak (str);

	}

	public override void ApplyEffect ()
	{

		base.ApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Provoking,3);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

//		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
		float highestHealth = CombatManager.Instance.getCurrentFighters(member.side)[0].crewMember.Health;
		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {
			if (item.HasStatus(Fighter.Status.Provoking) == false ) {
				if (item.crewMember.Health > highestHealth) {
					highestHealth = item.crewMember.Health;
					hasTarget = true;
					preferedTarget = item;
				}
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
