using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skil_Defend : Skill
{
    public override void Trigger(Fighter fighter)
    {
        base.Trigger(fighter);
    }

    public override void HandleOnApplyEffect()
    {
        base.HandleOnApplyEffect();

        fighter.AddStatus(Fighter.Status.Parrying);

        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlaySound("End Turn");

        fighter.EndTurn();
        CombatManager.Instance.NextTurn();
        //EndSkill();
    }

    public override void StartAnimation()
    {
        base.StartAnimation();
    }

    public override bool MeetsConditions(CrewMember member)
    {
        return base.MeetsConditions(member) && Random.value < 0.5f && member.Health < 30;
    }
}
