using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Attack : Skill {

    public int hitTypes = 2;

	public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack, 1f);
		EndSkill ();
	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetInteger("hitType", (int)Random.Range(0, hitTypes) );
        fighter.animator.SetTrigger("hit");
    }

}
