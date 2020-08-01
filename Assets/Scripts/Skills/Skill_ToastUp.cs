using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Skill_ToastUp : Skill {

    public float throwDuration = 1f;

    public float upDecal = 1f;
    public Transform rumBottle_Transform;

    private bool throwing = false;

    public float rotateSpeed = 1f;
    public float timeToDrink = 1f;

    public float timeToApplyEffect = 1f;
    public float timetoTriggerCatchAnim = 0.2f;

    public override void Start()
    {
        base.Start();

        rumBottle_Transform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (throwing)
        {
            rumBottle_Transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
    }

    public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		CrewMember member = fighter.TargetFighter.crewMember;

        rumBottle_Transform.gameObject.SetActive(false);

        string str = "Drink up, " + member.MemberName + " , you'll fight like a mad man";
        fighter.Speak(str);
    }

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("throw");
    }

    #region annimation event
    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        fighter.TargetFighter.animator.SetBool("waitingToCatch", true);

        fighter.AttachItemToHand(rumBottle_Transform);

        Tween.Bounce(rumBottle_Transform);
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        StartCoroutine(DrinkCoroutine());

    }

    IEnumerator DrinkCoroutine()
    {
        rumBottle_Transform.SetParent(fighter.BodyTransform);

        Transform targetTransform = fighter.TargetFighter.iconVisual.bodyVisual.itemAnchor;

        Vector2 midPoint = (Vector2)targetTransform.position + Vector2.up * upDecal;

        throwing = true;

        rumBottle_Transform.DOMove(midPoint, throwDuration);
        rumBottle_Transform.DOMove(targetTransform.position, throwDuration).SetDelay(throwDuration);

        yield return new WaitForSeconds(timetoTriggerCatchAnim);

        fighter.TargetFighter.animator.SetTrigger("catch");
        fighter.TargetFighter.animator.SetBool("waitingToCatch", false);

        yield return new WaitForSeconds(throwDuration * 2f);

        throwing = false;

        fighter.TargetFighter.AttachItemToHand(rumBottle_Transform);

        Tween.Bounce(rumBottle_Transform);

        yield return new WaitForSeconds(timeToDrink);

        fighter.TargetFighter.animator.SetTrigger("drink");

        yield return new WaitForSeconds(timeToApplyEffect);

        HandleOnApplyEffect();

        yield return new WaitForSeconds(2f);

        rumBottle_Transform.gameObject.SetActive(false);


    }
    #endregion
    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		if ( fighter.TargetFighter.HasStatus(Fighter.Status.Cussed) ) {
			fighter.TargetFighter.RemoveStatus (Fighter.Status.Cussed,3);
		}

		fighter.TargetFighter.AddStatus (Fighter.Status.Toasted);

		EndSkill ();

	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		//		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
		foreach (var item in CombatManager.Instance.getCurrentFighters(member.side) ) {
			if (item.HasStatus(Fighter.Status.Toasted) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
