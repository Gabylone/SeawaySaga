using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PledgeOfFeast : Skill {

	public int energyAmount = 10;

    public float triggerAnimDelay = 1f;

    public float applyEffectDelay = 1f;

    public float showFoodDelay = 1f;

    public override void StartAnimation()
    {
        base.StartAnimation();

        StartCoroutine(EatCoroutine());
    }

    IEnumerator EatCoroutine()
    {
        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Swipe");

        // cook speaks
        fighter.animator.SetTrigger("combat speak");

        string[] strs = new string[2]
        {
            "Come on, lads! Let's grab a bite and smash them!",
            "Eat up everyone! Now go and get them!"

        };

        string str = strs[Random.Range(0, strs.Length)];

        fighter.Speak(str);

        yield return new WaitForSeconds(showFoodDelay);

        // all waiting too catch
        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (item != fighter)
            {
                item.animator.SetBool("waitingToCatch", true);
            }
        }

        SoundManager.Instance.PlayRandomSound("Whoosh");


        yield return new WaitForSeconds(showFoodDelay);

        // food appears
        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (item != fighter)
            {
                SoundManager.Instance.PlayRandomSound("Whoosh");
                SoundManager.Instance.PlayRandomSound("Swipe");

                item.animator.SetTrigger("catch");
                item.animator.SetBool("waitingToCatch", false);
                item.AttachItemToHand(item.iconVisual.food_Obj.transform);

                yield return new WaitForSeconds(0.3f);
            }
        }

        yield return new WaitForSeconds(triggerAnimDelay);

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Swipe");

        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (item != fighter)
            {
                item.animator.SetTrigger("drink");
            }

        }

        yield return new WaitForSeconds(applyEffectDelay/3f);
        SoundManager.Instance.PlayRandomSound("Food Eat");
        yield return new WaitForSeconds(applyEffectDelay/3f);
        SoundManager.Instance.PlayRandomSound("Food Eat");
        yield return new WaitForSeconds(applyEffectDelay/3f);
        SoundManager.Instance.PlayRandomSound("Food Eat");

        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            if (item != fighter)
            {
                item.crewMember.AddEnergy(energyAmount);
                item.card.ShowEnergy();
                item.combatFeedback.Display("ENERGY!", Color.green);
                item.card.UpdateEnergyBar(item.crewMember);

                yield return new WaitForSeconds(0.3f);


            }
        }

        SoundManager.Instance.PlayRandomSound("Tribal");


        EndSkill();

        yield return new WaitForSeconds(2.5f);

        foreach (var item in CombatManager.Instance.getCurrentFighters(fighter.crewMember.side))
        {
            item.card.HideEnergy();
            item.iconVisual.food_Obj.SetActive(false);
        }
    }
}
