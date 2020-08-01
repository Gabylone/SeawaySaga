using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_RhumRound : Skill {

	public int healthToHeal = 60;

    public float timeToGetRhumOut = 1f;
    public float timeToDrink = 1f;
    public float timeToApplyEffects = 1f;

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

		string str = "Tonight we feast !";

		fighter.Speak (str);
	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        string str = "CHEERS !";
        fighter.Speak(str);

        Invoke("StartAnimationDelay" , timeToGetRhumOut);
    }

    void StartAnimationDelay()
    {
        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            item.animator.SetTrigger("throw");
        }
    }

    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            item.AttachItemToHand(item.iconVisual.rhumBottle_Transform);
        }
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        Invoke("Drink", timeToDrink);

    }

    void Drink()
    {
        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            item.animator.SetTrigger("drink");
        }

        Invoke("HandleOnApplyEffect", timeToApplyEffects);

    }
    public override void HandleOnApplyEffect()
    {
        base.HandleOnApplyEffect();

        foreach (var targetFighter in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            targetFighter.Heal(25);
        }

        EndSkill();

        Invoke("HandleOnApplyEffectDelay", 1f);

    }

    void HandleOnApplyEffectDelay()
    {
        foreach (var targetFighter in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            targetFighter.iconVisual.rhumBottle_Transform.gameObject.SetActive(false);
        }
    }

    public override bool MeetsConditions (CrewMember member)
	{

		bool allyInHelp = false;

		int count = 0;

		foreach (var item in Crews.getCrew(member.side).CrewMembers) {
			if (item.Health < healthToHeal) {
				++count;
				if ( count > 1 )
					allyInHelp = true;
			}
		}

		return allyInHelp && base.MeetsConditions (member);
	}
}
