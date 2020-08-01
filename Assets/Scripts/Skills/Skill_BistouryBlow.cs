using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Skill_BistouryBlow : Skill {

	public int healAmount = 35;
    public int healthToHeal = 60;

    public float throwDuration = 1f;

    public float upDecal = 1f;

    public Transform pills_Transform;

    private bool throwing = false;

    public float rotateSpeed = 10f;

    public float timeToDrink = 1f;

    public float timeToApplyEffect = 1f;

    public float timetoTriggerCatchAnim = 0.2f;

    public override void Start()
    {
        base.Start();
        pills_Transform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (throwing)
        {
            pills_Transform.Rotate( Vector3.forward * rotateSpeed * Time.deltaTime );
        }
    }

    public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		CrewMember member = fighter.TargetFighter.crewMember;

        pills_Transform.gameObject.SetActive(false);

        string str = "This will make you feel better, " + member.MemberName;
        fighter.Speak(str);
    }

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("throw");

    }

    #region animation event
    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        fighter.TargetFighter.animator.SetBool("waitingToCatch", true);

        fighter.AttachItemToHand(pills_Transform);

        Tween.Bounce(pills_Transform);
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        StartCoroutine(DrinkCoroutine());
        
    }

    IEnumerator DrinkCoroutine()
    {

        pills_Transform.SetParent(fighter.BodyTransform);

        Transform targetTransform = fighter.TargetFighter.iconVisual.bodyVisual.itemAnchor;

        Vector2 midPoint = (Vector2)targetTransform.position + Vector2.up * upDecal;

        throwing = true;

        pills_Transform.DOMove(midPoint, throwDuration);
        pills_Transform.DOMove(targetTransform.position, throwDuration).SetDelay(throwDuration);

        yield return new WaitForSeconds(timetoTriggerCatchAnim);

        fighter.TargetFighter.animator.SetTrigger("catch");
        fighter.TargetFighter.animator.SetBool("waitingToCatch", false);

        yield return new WaitForSeconds(throwDuration * 2f);

        throwing = false;

        fighter.TargetFighter.AttachItemToHand(pills_Transform);

        Tween.Bounce(pills_Transform);

        yield return new WaitForSeconds(timeToDrink);

        fighter.TargetFighter.animator.SetTrigger("drink");

        yield return new WaitForSeconds(timeToApplyEffect);

        HandleOnApplyEffect();

        yield return new WaitForSeconds(2f);

        pills_Transform.gameObject.SetActive(false);


    }
    #endregion

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		fighter.TargetFighter.Heal (healAmount);

        EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		Fighter weakestFighter = CombatManager.Instance.currEnemyFighters [0];
		foreach (var item in CombatManager.Instance.currEnemyFighters) {
			if ( item.crewMember.Health < weakestFighter.crewMember.Health ) {
				weakestFighter = item;
			}
		}

		bool allyInHelp = false;

		if ( weakestFighter.crewMember.Health < healthToHeal ) {
			preferedTarget = weakestFighter;
			allyInHelp = true;
		}

		return allyInHelp && base.MeetsConditions (member);
	}
}
