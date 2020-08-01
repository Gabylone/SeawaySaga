﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Jag : Skill {

	public float healthNeeded = 50;

    public Transform needle_Transform;

    public override void Start()
    {
        base.Start();

        needle_Transform.gameObject.SetActive(false);
    }

    public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		CrewMember member = fighter.TargetFighter.crewMember;

        string str = "Take this, " + member.MemberName + " you'll feel better, I promise";
        fighter.Speak(str);
    }

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("throw");
    }

    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        fighter.AttachItemToHand(needle_Transform);
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        HandleOnApplyEffect();
    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

        Tween.Bounce(fighter.TargetFighter.GetTransform, 0.3f, 1.25f);

        fighter.TargetFighter.AddStatus (Fighter.Status.Jagged, 3);
		fighter.TargetFighter.RemoveStatus (Fighter.Status.Poisonned, 3);

        needle_Transform.gameObject.SetActive(false);

        EndSkill ();
	}
     
	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {

			bool targetIsntJagged = item.HasStatus (Fighter.Status.Jagged) == false;
			bool targetNeedsHealing = item.crewMember.Health < healthNeeded;

			if ( targetIsntJagged && targetNeedsHealing ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
