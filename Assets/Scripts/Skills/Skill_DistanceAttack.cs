using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DistanceAttack : Skill {

	public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack, 1f);

		EndSkill ();

	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("shoot");
    }
}
