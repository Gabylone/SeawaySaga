using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_SkipTurn: Skill {

	public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

//		EndSkill ();
		fighter.EndTurn();
		CombatManager.Instance.NextTurn ();
	}

}
