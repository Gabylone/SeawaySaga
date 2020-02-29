using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Cosh : Skill {

	public float knockedOutChance = 0.3f;

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();


		fighter.TargetFighter.AddStatus (Fighter.Status.KnockedOut);

//		if ( Random.value < knockedOutChance) {
//			fighter.TargetFighter.AddStatus (Fighter.Status.KnockedOut);
//		} else {
//			fighter.combatFeedback.Display ( "Raté !" );
//		}
//
		fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack, 1f);


		EndSkill ();

	}

	public override bool MeetsRestrictions (CrewMember member)
	{
		return base.MeetsRestrictions (member) && ( member.GetEquipment(CrewMember.EquipmentPart.Weapon) == null || member.GetEquipment(CrewMember.EquipmentPart.Weapon).spriteID == 1);
	}


	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;
		
		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
			if (item.HasStatus(Fighter.Status.KnockedOut) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
