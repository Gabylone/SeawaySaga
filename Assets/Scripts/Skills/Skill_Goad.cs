using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Goad : Skill {

    public float applyEffectDelay = 0.5f;

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("combat speak");

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Swipe");
        SoundManager.Instance.PlayRandomSound("voice_mad");

        Invoke("HandleOnApplyEffect", applyEffectDelay);

    }

    public override void OnSetTarget ()
	{
		base.OnSetTarget ();

        string str;

        if ( fighter.TargetFighter == fighter)
        {
            str = "You're nothing but a scoundrel ! Come'n get me if you're not a wuss";
        }
        else
        {
            str = "I heard " + fighter.TargetFighter.crewMember.MemberName + " says you're nothing but pirate scum !";
        }


        fighter.Speak (str);

	}

	public override void HandleOnApplyEffect ()
	{

		base.HandleOnApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Provoking,3);

        SoundManager.Instance.PlayRandomSound("Goad");

        EndSkill();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

//		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
		float highestHealth = CombatManager.Instance.getCurrentFighters(member.side)[0].crewMember.Health;
		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {
			if (item.HasStatus(Fighter.Status.Provoking) == false ) {
				if (item.crewMember.Health > highestHealth) {
					highestHealth = item.crewMember.Health;
					hasTarget = true;
					preferedTarget = item;
				}
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
