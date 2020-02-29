using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Dynamite : Skill {

	public override void ApplyEffect ()
	{

		base.ApplyEffect ();

		List<Fighter> fighters = CombatManager.Instance.getCurrentFighters (Crews.otherSide (fighter.crewMember.side));
		for (int fighterIndex = 0; fighterIndex < fighters.Count; fighterIndex++) {

			fighters[fighterIndex].GetHit (fighter, fighter.crewMember.Attack , 0.8f);

		}

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{

		bool moreThanOneMember = CombatManager.Instance.getCurrentFighters (Crews.otherSide (member.side)).Count > 1;

		return moreThanOneMember && base.MeetsConditions (member);
	}

}
