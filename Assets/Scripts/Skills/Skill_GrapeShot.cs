using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_GrapeShot : Skill {
	
	public int attackCount = 4;

    int currentCount = 0;

	public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

        Fighter targetFighter = CombatManager.Instance.getCurrentFighters(Crews.otherSide(fighter.crewMember.side))
                [Random.Range(0, CombatManager.Instance.getCurrentFighters(Crews.otherSide(fighter.crewMember.side)).Count)];
        targetFighter.GetHit(fighter, fighter.crewMember.Attack, 0.4f);

        ++currentCount;

        if ( currentCount == attackCount)
        {
            EndSkill();
        }


    }

    public override void StartAnimation()
    {
        base.StartAnimation();

        currentCount = 0;

        fighter.animator.SetTrigger("grapeShot");
    }

	public override bool MeetsRestrictions (CrewMember member)
	{
		if (member.GetEquipment (CrewMember.EquipmentPart.Weapon) == null)
			return false;

		return base.MeetsRestrictions (member) && member.GetEquipment(CrewMember.EquipmentPart.Weapon).weaponType == Item.WeaponType.Distance;
	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool moreThanOneMember = CombatManager.Instance.getCurrentFighters (Crews.otherSide (member.side)).Count > 1;

		return moreThanOneMember && base.MeetsConditions (member);
	}
}
