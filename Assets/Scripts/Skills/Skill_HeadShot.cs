using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HeadShot : Skill {

	bool onDelay = false;

	public int healthToAttack = 30;

	public override void InvokeSkill ()
	{
		if (onDelay) {
			fighter.crewMember.energy += energyCost;
		}
		base.InvokeSkill ();
	}

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

		if (onDelay == false) {
			string str = "Wait 'till you see what I've got in store for you...";
			fighter.Speak (str);
		}

	}

	public override void HandleOnApplyEffect ()
	{

		base.HandleOnApplyEffect ();

		if (onDelay) {

			onDelay = false;
			hasTarget = false;
			playAnim = false;

			fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack , 2.2f);

			fighter.SetTurn ();

			EndSkill ();
			//

		} else {



			fighter.AddStatus (Fighter.Status.PreparingAttack);

			fighter.onSkillDelay += HandleOnSkillDelay;

			EndSkill ();

		}

	}

	Fighter delayFighter;
	void HandleOnSkillDelay (Fighter _delayFighter)
	{
//		Invoke ("TriggerDelay",0.1f);
		this.delayFighter = _delayFighter;
		delayFighter.combatFeedback.Display (Fighter.Status.PreparingAttack, Color.white);

		onDelay = true;
		hasTarget = true;
		playAnim = true;

		Trigger (delayFighter);
	}

//	void TriggerDelay () {
//		onDelay = true;
//		hasTarget = true;
//		playAnim = true;
//
//		Trigger (delayFighter);
//	}

	public override bool MeetsRestrictions (CrewMember member)
	{
		if (member.GetEquipment (CrewMember.EquipmentPart.Weapon) == null)
			return false;

        return base.MeetsRestrictions(member) && member.GetEquipment(CrewMember.EquipmentPart.Weapon).weaponType == Item.WeaponType.Distance;
	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool meetsChances = Random.value < 0.5f;

		return meetsChances && base.MeetsConditions (member);
	}
}
