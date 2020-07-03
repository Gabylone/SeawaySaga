using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Attack : Skill {

	public override void ApplyEffect ()
	{

		base.ApplyEffect ();

		fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack, 1f);
		EndSkill ();

	}

}
