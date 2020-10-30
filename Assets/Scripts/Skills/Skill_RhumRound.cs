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
        SoundManager.Instance.PlayRandomSound("Potion");

        SoundManager.Instance.PlaySound("Tribal 01");

        Invoke("StartAnimationDelay" , timeToGetRhumOut);
    }

    void StartAnimationDelay()
    {
        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (!item.SkippingTurn())
            {
                item.animator.SetTrigger("throw");
            }
        }
    }

    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (!item.SkippingTurn())
            {
                item.AttachItemToHand(item.iconVisual.rhumBottle_Transform);
            }
        }

        SoundManager.Instance.PlayRandomSound("Potion");
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        Invoke("Drink", timeToDrink);

        SoundManager.Instance.PlayRandomSound("Swipe");


    }

    void Drink()
    {
        SoundManager.Instance.PlayRandomSound("Whoosh");


        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (!item.SkippingTurn())
            {
                item.animator.SetTrigger("drink");
            }
        }

        Invoke("HandleOnApplyEffect", timeToApplyEffects);

    }
    public override void HandleOnApplyEffect()
    {
        base.HandleOnApplyEffect();

        SoundManager.Instance.PlayRandomSound("Potion");
        SoundManager.Instance.PlayRandomSound("Alchemy");
        SoundManager.Instance.PlayRandomSound("Cook");

        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (!item.SkippingTurn())
            {
                item.Heal(25);
            }
        }

        EndSkill();

        Invoke("HandleOnApplyEffectDelay", 1f);

    }

    void HandleOnApplyEffectDelay()
    {
        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (!item.SkippingTurn())
            {
                item.iconVisual.rhumBottle_Transform.gameObject.SetActive(false);
            }
        }
    }

    public override bool MeetsConditions (CrewMember member)
	{

		bool allyInHelp = false;

		int count = 0;

		foreach (var item in Crews.getCrew(member.side).CrewMembers) {
			if (item.Health < healthToHeal) {
				++count;
				if ( count >= 1 )
					allyInHelp = true;
			}
		}

		return allyInHelp && base.MeetsConditions (member);
	}
}
