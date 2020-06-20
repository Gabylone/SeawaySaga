using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Leap: Skill {

	bool onDelay = false;

	public int healthToAttack = 30;
	public override void InvokeSkill ()
	{
		if (onDelay) {
			print ("lui donne de l'energie");
			fighter.crewMember.energy += energyCost;
		}
		base.InvokeSkill ();
	}

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

		if (onDelay == false) {
			string str = "You wait and see... I'm gonna smash your brains out";
            fighter.Speak (str);
		}

	}

	public override void ApplyEffect ()
	{

		base.ApplyEffect ();

		if (onDelay) {

			onDelay = false;
			hasTarget = false;
			goToTarget = false;
			playAnim = false;

			fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack , 2.2f);

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
		this.delayFighter = _delayFighter;
		delayFighter.combatFeedback.Display (Fighter.Status.PreparingAttack, Color.white);

		onDelay = true;
		hasTarget = true;
		goToTarget = true;
		playAnim = true;

		Trigger (delayFighter);
	}

	public override bool MeetsRestrictions (CrewMember member)
	{
        return base.MeetsRestrictions(member) && member.HasMeleeWepon();
	}


	public override bool MeetsConditions (CrewMember member)
	{
		bool meetsChances = Random.value < 0.5f;

		return meetsChances && base.MeetsConditions (member);
	}
}
