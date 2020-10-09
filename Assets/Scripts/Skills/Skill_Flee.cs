using System.Collections;
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
		if ( DiceManager.Instance.HighestResult == 6 ) {

            SoundManager.Instance.PlaySound("ui_correct");

            Escape();

		} else if ( DiceManager.Instance.HighestResult == 1 ) {

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
        
        string str = "See you, sucker !";
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
        fighter.combatFeedback.Display("Knocked Out !", Color.magenta);
        fighter.AddStatus(Fighter.Status.KnockedOut);

        fighter.crewMember.energy = 0;

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
