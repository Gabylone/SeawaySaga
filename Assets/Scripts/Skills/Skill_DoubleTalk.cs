using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DoubleTalk : Skill {

	public int healthToFlee = 60;

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

		} else {

            SoundManager.Instance.PlaySound("ui_wrong");

            fighter.combatFeedback.Display("Miss !",Color.red);
            EndSkill ();
		}

        DiceManager.Instance.onEndThrow -= HandleOnEndThrow;

		
	}

    void Escape()
    {
        fighter.combatFeedback.Display("Fled !", Color.green);

        string str = "Catch us if you can !";
        fighter.Speak(str);

        Invoke("EscapeDelay", escapeDelay);
    }

    void EscapeDelay()
    {
        for (int fighterIndex = 0; fighterIndex < CombatManager.Instance.getCurrentFighters(fighter.crewMember.side).Count; fighterIndex++)
        {
            Fighter targetFighter = CombatManager.Instance.getCurrentFighters(fighter.crewMember.side)[fighterIndex];

            targetFighter.escaped = true;
            //CombatManager.Instance.getCurrentFighters(fighter.crewMember.side)[0].EndTurn();
            targetFighter.Fade();

            
        }

        int l = CombatManager.Instance.getCurrentFighters(fighter.crewMember.side).Count;

        for (int fighterIndex = 0; fighterIndex < l; fighterIndex++)
        {
            Fighter targetFighter = CombatManager.Instance.getCurrentFighters(fighter.crewMember.side)[0];
            CombatManager.Instance.DeleteFighter(targetFighter);

        }

    }

    public override bool MeetsConditions (CrewMember member)
	{

		bool allyInHelp = false;

		int count = 0;

		foreach (var item in Crews.getCrew(member.side).CrewMembers) {
			if (item.Health < healthToFlee) {
				++count;
				if ( count > 1 )
					allyInHelp = true;
			}
		}

		return allyInHelp && base.MeetsConditions (member);
	}
}
