using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Wallop : Skill {

	

	public override void HandleOnApplyEffect ()
	{

		base.HandleOnApplyEffect ();

		List<Fighter> fighters = CombatManager.Instance.getCurrentFighters (Crews.otherSide (fighter.crewMember.side));
		for (int fighterIndex = 0; fighterIndex < fighters.Count; fighterIndex++) {
			
			fighters[fighterIndex].GetHit (fighter, fighter.crewMember.Attack , 0.5f);

		}

		EndSkill ();

	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("wallop");
    }

    public override bool MeetsRestrictions (CrewMember member)
	{
        return base.MeetsRestrictions(member) && member.HasMeleeWepon();
	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool moreThanOneMember = CombatManager.Instance.getCurrentFighters (Crews.otherSide (member.side)).Count > 1;

		return moreThanOneMember && base.MeetsConditions (member);
	}
}
