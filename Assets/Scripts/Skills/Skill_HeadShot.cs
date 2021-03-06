﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HeadShot : Skill {

    public int healthToAttack = 30;

    private bool secondPart = false;

    public override void InvokeSkill ()
	{
		if (secondPart) {
            fighter.crewMember.AddEnergy(energyCost);
		}

		base.InvokeSkill ();
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
            SoundManager.Instance.PlayRandomSound("voice_mad");
            SoundManager.Instance.PlayRandomSound("Tribal");

            string str = "Don't you move, I'm gonna shoot you right between the eyes";

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

        if (secondPart)
        {
            fighter.animator.SetTrigger("headShot");

            SoundManager.Instance.PlayRandomSound("Swipe");
        }
        else
        {
            fighter.animator.SetBool("aiming", true);
            fighter.iconVisual.SetAimingEyes();
        }
    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		if (secondPart) {

            fighter.animator.SetBool("aiming", false);

            SoundManager.Instance.PlayRandomSound("shoot");

            secondPart = false;
            hasTarget = false;
            goToTarget = false;

            fighter.iconVisual.RemoveAimingEyes();

			fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack , 2.2f);

            fighter.SetTurn();

        }

    }

    void HandleOnApplyEffectDelay()
    {
    }

    public override void ContinueSkill()
    {
        base.ContinueSkill();

        CombatManager.Instance.GetCurrentFighter.combatFeedback.Display (Fighter.Status.PreparingAttack, Color.white);

        secondPart = true;
		hasTarget = true;

        Trigger(CombatManager.Instance.GetCurrentFighter);
	}

    public override bool MeetsRestrictions (CrewMember member)
	{
		if (member.GetEquipment (CrewMember.EquipmentPart.Weapon) == null)
			return false;

        return base.MeetsRestrictions(member) && member.GetEquipment(CrewMember.EquipmentPart.Weapon).weaponType == Item.WeaponType.Distance;
	}

	public override bool MeetsConditions (CrewMember member)
	{
        bool meetsChances = Random.value < 0.5f;

        return meetsChances && base.MeetsConditions (member);
	}

}
