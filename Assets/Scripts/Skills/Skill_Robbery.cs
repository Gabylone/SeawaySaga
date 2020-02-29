using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Robbery : Skill {

	public int goldStolen = 30;

	public int minimumGoldToSteal = 15;

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		if ( fighter.crewMember.side == Crews.Side.Enemy ) {

			GoldManager.Instance.RemoveGold(goldStolen);

		} else {

			GoldManager.Instance.AddGold(goldStolen);

			//
		}

		fighter.combatFeedback.Display ("+" + goldStolen, Color.yellow);
		fighter.TargetFighter.combatFeedback.Display ("-" + goldStolen, Color.red);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasMinimumGold = false;

		if (GoldManager.Instance.goldAmount > minimumGoldToSteal)
			hasMinimumGold = true;

		return hasMinimumGold && base.MeetsConditions (member);
	}
}
