using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Leap: Skill {

    private Fighter delayFighter;

    bool secondPart = false;

    public override void OnSetTarget()
    {
        base.OnSetTarget();

        SoundManager.Instance.PlayRandomSound("Swipe");

    }

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

            SoundManager.Instance.PlaySound("Tribal 01");
            SoundManager.Instance.PlayRandomSound("voice_mad");

            fighter.Speak(str);

            // delayed stuff
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

            SoundManager.Instance.PlayRandomSound("Swipe");
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

            SoundManager.Instance.PlayRandomSound("Sword");
            SoundManager.Instance.PlayRandomSound("Blunt");
            SoundManager.Instance.PlayRandomSound("Punch");

            secondPart = false;
			hasTarget = false;
			goToTarget = false;

            fighter.iconVisual.RemoveMadFace();

            fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack , 2.2f);

			EndSkill ();
			//

		}

	}

    public override void EndSkillDelay()
    {
        if (!secondPart)
        {
            fighter.EndTurn();
            CombatManager.Instance.NextTurn();
        }
        else
        {
            base.EndSkillDelay();
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

    public override bool MeetsRestrictions (CrewMember member)
	{
        return base.MeetsRestrictions(member) && member.HasMeleeWepon();
	}


	public override bool MeetsConditions (CrewMember member)
	{
        //bool meetsChances = Random.value < 0.5f;

        Debug.LogError("DEBUGGING THING");
        energyCost = 0;
        bool meetsChances = Random.value < 1f;

		return meetsChances && base.MeetsConditions (member);
	}
}
