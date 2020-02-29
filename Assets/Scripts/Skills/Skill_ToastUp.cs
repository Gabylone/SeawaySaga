using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_ToastUp : Skill {

	public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		CrewMember member = fighter.TargetFighter.crewMember;

		if (member.MemberID.Male) {
			string str = "T'as toujours été mon petit préféré, " + member.MemberName;
			fighter.Speak (str);
		} else {
			string str = "T'as toujours été ma petite préférée, " + member.MemberName;
			fighter.Speak (str);
		}
	}


	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		if ( fighter.TargetFighter.HasStatus(Fighter.Status.Cussed) ) {
			fighter.TargetFighter.RemoveStatus (Fighter.Status.Cussed,3);
		}

		fighter.TargetFighter.AddStatus (Fighter.Status.Toasted);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		//		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {
			if (item.HasStatus(Fighter.Status.Toasted) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
