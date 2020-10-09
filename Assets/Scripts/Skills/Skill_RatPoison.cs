 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Skill_RatPoison : Skill {

    public Transform poisonBottle_Transform;

    public float upDecal = 3f;

    private bool throwing = false;

    public float throwDuration = 2f;

    public override void Start()
    {
        base.Start();

        poisonBottle_Transform.gameObject.SetActive(false);
    }

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("throw");
    }

    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        fighter.AttachItemToHand(poisonBottle_Transform);

        SoundManager.Instance.PlayRandomSound("Potion");

    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        StartCoroutine(DrinkCoroutine());
    }

    IEnumerator DrinkCoroutine()
    {
        poisonBottle_Transform.SetParent(fighter.BodyTransform);

        SoundManager.Instance.PlayRandomSound("Whoosh");

        Transform targetTransform = fighter.TargetFighter.GetTransform;

        Vector3 midPoint = poisonBottle_Transform.position + (targetTransform.position - poisonBottle_Transform.position)/2f;
        midPoint.y += upDecal;

        throwing = true;

        poisonBottle_Transform.DOMoveX(midPoint.x, throwDuration).SetEase(Ease.OutQuad);
        poisonBottle_Transform.DOMoveY(midPoint.y, throwDuration).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(throwDuration);

        SoundManager.Instance.PlayRandomSound("Swipe");

        poisonBottle_Transform.DOMoveX(targetTransform.position.x, throwDuration).SetEase(Ease.OutQuad);
        poisonBottle_Transform.DOMoveY(targetTransform.position.y, throwDuration).SetEase(Ease.InQuad);

        yield return new WaitForSeconds(throwDuration);

        throwing = false;

        poisonBottle_Transform.position = targetTransform.position;
        poisonBottle_Transform.gameObject.SetActive(false);

        HandleOnApplyEffect();

    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Poisonned, 3);

        SoundManager.Instance.PlayRandomSound("Alchemy");
        SoundManager.Instance.PlayRandomSound("Potion");
        SoundManager.Instance.PlaySound("Glass");


        EndSkill ();

    }

	public override bool MeetsConditions (CrewMember member)
	{
		bool hasTarget = false;

		foreach (var item in CombatManager.Instance.getCurrentFighters(Crews.otherSide(member.side)) ) {
			if (item.HasStatus(Fighter.Status.Poisonned) == false ) {
				hasTarget = true;
				preferedTarget = item;
			}
		}

		return hasTarget && base.MeetsConditions (member);
	}
}
