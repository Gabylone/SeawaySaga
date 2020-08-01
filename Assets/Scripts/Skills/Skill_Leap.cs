using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Leap: Skill {

	bool secondPart = false;

	private Fighter delayFighter;

	public override void InvokeSkill ()
	{
		if (secondPart) {
			fighter.crewMember.energy += energyCost;
		}

		base.InvokeSkill ();
	}

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

        if (!secondPart)
        {
            string str = "You wait and see... I'm gonna smash your brains out";
            fighter.Speak(str);

            fighter.AddStatus(Fighter.Status.PreparingAttack);

            fighter.onSkillDelay += HandleOnSkillDelay;

            EndSkill();
        }

	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        if ( secondPart)
        {
            fighter.animator.SetTrigger("leap");
        }
        else
        {
            fighter.animator.SetBool("preparingToLeap", true);
            fighter.iconVisual.SetMadFace();
        }
    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		if (secondPart) {

            fighter.animator.SetBool("preparingToLeap", false);

            secondPart = false;
			hasTarget = false;
			goToTarget = false;

            fighter.iconVisual.RemoveMadFace();

            fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack , 2.2f);

			EndSkill ();
			//

		}

	}

	void HandleOnSkillDelay (Fighter _delayFighter)
	{
		this.delayFighter = _delayFighter;

		delayFighter.combatFeedback.Display (Fighter.Status.PreparingAttack, Color.white);

		secondPart = true;
		hasTarget = true;
		goToTarget = true;

		Trigger (delayFighter);
	}

    public override void EndSkill()
    {
        base.EndSkill();


    }

    public override bool MeetsRestrictions (CrewMember member)
	{
        return base.MeetsRestrictions(member) && member.HasMeleeWepon();
	}


	public override bool MeetsConditions (CrewMember member)
	{
		bool meetsChances = Random.value < 0.5f;

		return meetsChances && base.MeetsConditions (member);
	}
}
