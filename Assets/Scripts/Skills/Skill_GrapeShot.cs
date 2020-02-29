using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_GrapeShot : Skill {
	
	public int attackCount = 4;

	

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		StartCoroutine (SkillCoroutine ());

	}

	IEnumerator SkillCoroutine () {

		for (int count = 0; count < attackCount; count++) {

			Fighter targetFighter = CombatManager.Instance.getCurrentFighters (Crews.otherSide (fighter.crewMember.side))
				[Random.Range (0, CombatManager.Instance.getCurrentFighters (Crews.otherSide (fighter.crewMember.side)).Count)];

			targetFighter.GetHit (fighter, fighter.crewMember.Attack , 0.4f);

			TriggerAnimation ();

			yield return new WaitForSeconds ( animationDelay );


		}

		yield return new WaitForEndOfFrame ();

		EndSkill ();

	}

	public override bool MeetsRestrictions (CrewMember member)
	{
		if (member.GetEquipment (CrewMember.EquipmentPart.Weapon) == null)
			return false;

		return base.MeetsRestrictions (member) && member.GetEquipment(CrewMember.EquipmentPart.Weapon).spriteID == 0;
	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool moreThanOneMember = CombatManager.Instance.getCurrentFighters (Crews.otherSide (member.side)).Count > 1;

		return moreThanOneMember && base.MeetsConditions (member);
	}
}
