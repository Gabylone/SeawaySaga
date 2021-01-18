using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cosh : Skill {

	public float knockedOutChance = 0.3f;
    public float secondSoundDelay = 0.5f;

    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Swipe");
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Swipe");
    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

        fighter.TargetFighter.GetHit(fighter, fighter.crewMember.Attack, 1f);

        if (!fighter.dodged)
        {
            fighter.TargetFighter.KnockOut();
        }

        EndSkill ();

	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("knock out");

        SoundManager.Instance.PlayRandomSound("Whoosh");

        Invoke("StartAnimationDelay", secondSoundDelay);
    }

    void StartAnimationDelay()
    {
        SoundManager.Instance.PlayRandomSound("Whoosh");
    }

    public override bool MeetsRestrictions (CrewMember member)
	{
		return base.MeetsRestrictions (member) && member.HasMeleeWepon();
	}


	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;
		
		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
			if (item.HasStatus(Fighter.Status.KnockedOut) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
