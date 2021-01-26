using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HelpMate : Skill {

    public float applyEffectDelay = 0.5f;
    public float targetFighterAnimationDelay = 1.5f;

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("combat speak");

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Swipe");

        Invoke("TargetFighterAnimation" , targetFighterAnimationDelay);

    }

    public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		string str = "Cheer up, " + fighter.TargetFighter.crewMember.MemberName + " ! You can do it, son";
		fighter.Speak (str);
    }

    void TargetFighterAnimation()
    {
        fighter.TargetFighter.animator.SetTrigger("puffed");

        SoundManager.Instance.PlayRandomSound("Swipe");
        SoundManager.Instance.PlayRandomSound("Whoosh");


        Invoke("HandleOnApplyEffect", applyEffectDelay);
    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Protected, 2);

        if (fighter.TargetFighter.HasStatus(Fighter.Status.KnockedOut))
        {
            fighter.TargetFighter.RemoveStatus(Fighter.Status.KnockedOut);
        }

        SoundManager.Instance.PlaySound("Boost");
        SoundManager.Instance.PlaySound("Fury");
        SoundManager.Instance.PlaySound("Tribal 01");

        EndSkill();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {
			if (item.HasStatus(Fighter.Status.Protected) == false ) {

				hasTarget = true;
				preferedTarget = item;

				if (item.HasStatus (Fighter.Status.Provoking))
					break;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
