﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Flee : Skill {

    public float escapeDelay = 1f;

	public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		DiceManager.Instance.onEndThrow += HandleOnEndThrow;

		DiceManager.Instance.ThrowDice (DiceTypes.DEX, fighter.crewMember.GetStat(Stat.Dexterity));

	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("throw dice");
    }

    void HandleOnEndThrow ()
	{
		if ( DiceManager.Instance.result == DiceManager.Result.Success ) {

            SoundManager.Instance.PlaySound("ui_correct");

            Escape();

		} else if ( DiceManager.Instance.result == DiceManager.Result.CriticalFailure ) {

            SoundManager.Instance.PlaySound("ui_wrong");

            CriticalFailure();

		} else {

            SoundManager.Instance.PlaySound("ui_deny");

            Failure();

		}

		DiceManager.Instance.onEndThrow -= HandleOnEndThrow;

	}

    void Escape()
    {
        fighter.combatFeedback.Display("Fled !", Color.green);

        fighter.escaped = true;

        string[] strs = new string[2]
        {
            "See you, suckers!",
            "I’m out of here!"
        };

        string str = strs[Random.Range(0, strs.Length)];

        fighter.Speak(str);
        fighter.Fade();

        Invoke("EscapeDelay", escapeDelay);
    }

    void EscapeDelay()
    {
        fighter.EndTurn();

        CombatManager.Instance.DeleteFighter(fighter);
        CombatManager.Instance.NextTurn();
    }

    void CriticalFailure()
    {
        fighter.KnockOut();
        Invoke("EndSkill", escapeDelay);
    }

    void Failure()
    {
        fighter.combatFeedback.Display("Miss !", Color.red);

        Invoke("EndSkill" , escapeDelay);
    }


    public override bool MeetsConditions (CrewMember member)
	{
		return false;

//		bool fewHealth = member.Health < healthToFlee;
//
//		bool meetsChance = Random.value < 0.65f;
//
//		return meetsChance && fewHealth && base.MeetsConditions (member);
	}
}
