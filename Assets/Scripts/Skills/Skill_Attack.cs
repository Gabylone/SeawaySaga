using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Attack : Skill {

    public int hitTypes = 2;

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        SoundManager.Instance.PlayRandomSound("slash");

    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

        SoundManager.Instance.PlayRandomSound("Sword");

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
