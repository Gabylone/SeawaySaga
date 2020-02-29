using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_DoubleTalk : Skill {

	public int healthToFlee = 60;

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		DiceManager.Instance.onEndThrow += HandleOnEndThrow;

		DiceManager.Instance.ThrowDice (DiceTypes.DEX, fighter.crewMember.GetStat(Stat.Dexterity));

	}

	void HandleOnEndThrow ()
	{
		if ( DiceManager.Instance.HighestResult == 6 ) {

			fighter.combatFeedback.Display("Fuite !", Color.green);

			for (int fighterIndex = 0; fighterIndex < CombatManager.Instance.getCurrentFighters(fighter.crewMember.side).Count; fighterIndex++) {
				
				CombatManager.Instance.getCurrentFighters (fighter.crewMember.side) [0].escaped = true;
				CombatManager.Instance.getCurrentFighters (fighter.crewMember.side) [0].EndTurn ();
				CombatManager.Instance.getCurrentFighters(fighter.crewMember.side)[0].Fade();

				CombatManager.Instance.DeleteFighter (CombatManager.Instance.getCurrentFighters(fighter.crewMember.side)[0]);
			}

		} else {

			fighter.combatFeedback.Display("Raté !",Color.red);

		}

		DiceManager.Instance.onEndThrow -= HandleOnEndThrow;

		EndSkill ();
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
