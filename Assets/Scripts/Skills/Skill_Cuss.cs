using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cuss : Skill {

    public float applyEffectDelay = 0.5f;

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("combat speak");

        Invoke("StartAnimationDelay", 0.15f);
        Invoke("HandleOnApplyEffect", applyEffectDelay);

    }

    void StartAnimationDelay()
    {
        SoundManager.Instance.PlayRandomSound("Whoosh");

    }

    public override void OnSetTarget ()
	{
		base.OnSetTarget ();

        SoundManager.Instance.PlayRandomSound("voice_mad");

		string[] strs = new string[7]
		{
			"What's that smell? It’s YOU?!",
			"You look like the offspring of a teapot and a seagull!",
			"You're nothing but trash, shameful bandit!",
			"You stampcrab, sea vermin mollusc!",
			"Come and get me, prove you're more than a smelly clam!",
			"Your parents should have used you as shark bait!",
			"I can smell your stinky stupid face from here! Ever heard of the word bath?"
		};
		fighter.Speak (strs[Random.Range(0,strs.Length)]);

	}

	public override void HandleOnApplyEffect ()
	{

		base.HandleOnApplyEffect ();

		if ( fighter.TargetFighter.HasStatus(Fighter.Status.Toasted) ) {
			fighter.TargetFighter.RemoveStatus (Fighter.Status.Toasted,3);
		}

        SoundManager.Instance.PlayRandomSound("voice_sad");

        fighter.TargetFighter.AddStatus (Fighter.Status.Cussed,2);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
			if (item.HasStatus(Fighter.Status.Cussed) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
