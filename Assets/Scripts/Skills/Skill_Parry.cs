using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Parry : Skill {

	

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

//		fighter.AddStatus (Fighter.Status.Parrying);

		EndSkill ();

	}
//
//	public override bool MeetsConditions (CrewMember member)
////	{
//		bool parrying = CombatManager.Instance.currentFighter.HasStatus (Fighter.Status.Parrying);
//
//		return !parrying && base.MeetsConditions (member);
//	}
}
