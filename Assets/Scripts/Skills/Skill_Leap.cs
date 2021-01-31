using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Leap: Skill {

    private Fighter delayFighter;

    private bool secondPart = false;

    public override void InvokeSkill()
    {
        if (secondPart)
        {
            fighter.crewMember.AddEnergy(energyCost);
        }

        base.InvokeSkill();
    }

    public override void OnSetTarget()
    {
        base.OnSetTarget();

        SoundManager.Instance.PlayRandomSound("Swipe");
    }

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

        if (!secondPart)
        {
            SoundManager.Instance.PlaySound("Tribal 01");
            SoundManager.Instance.PlayRandomSound("voice_mad");

            string str = "You wait and see... I'm gonna smash your brains out";

            fighter.Speak(str);

            // delayed stuff
            fighter.AddStatus(Fighter.Status.PreparingAttack);

            EndSkill();
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

            Invoke("HandleOnApplyEffectDelay", 0.5f);

        }

	}

    void HandleOnApplyEffectDelay()
    {
        fighter.SetTurn();
    }

    public override void ContinueSkill()
    {
        base.ContinueSkill();

        CombatManager.Instance.GetCurrentFighter.combatFeedback.Display(Fighter.Status.PreparingAttack, Color.white);

        secondPart = true;
		goToTarget = true;
        hasTarget = true;

        Trigger(CombatManager.Instance.GetCurrentFighter);
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
