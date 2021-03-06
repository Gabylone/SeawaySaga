﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Wallop : Skill {

    public float secondSoundDelay = 0.2f;

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

        SoundManager.Instance.PlayRandomSound("slash");
        SoundManager.Instance.PlayRandomSound("Blunt");

        List<Fighter> fighters = CombatManager.Instance.getCurrentFighters(Crews.otherSide(fighter.crewMember.side));
        for (int i = fighters.Count - 1; i > -1; i--)
        {
            fighters[i].GetHit (fighter, fighter.crewMember.Attack , 0.5f);
        }

		EndSkill ();

	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("wallop");

        SoundManager.Instance.PlayRandomSound("Swipe");
        SoundManager.Instance.PlayRandomSound("Whoosh");

    }

    void StartAnimationDelay()
    {
        SoundManager.Instance.PlayRandomSound("Swipe");
        SoundManager.Instance.PlayRandomSound("Whoosh");
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
