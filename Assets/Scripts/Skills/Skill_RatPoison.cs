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
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        StartCoroutine(DrinkCoroutine());
    }

    IEnumerator DrinkCoroutine()
    {
        poisonBottle_Transform.SetParent(fighter.BodyTransform);

        Transform targetTransform = fighter.TargetFighter.GetTransform;

        Vector2 midPoint = (Vector2)targetTransform.position + Vector2.up * upDecal;

        throwing = true;

        poisonBottle_Transform.DOMove(midPoint, throwDuration);
        poisonBottle_Transform.DOMove(targetTransform.position, throwDuration).SetDelay(throwDuration);

        yield return new WaitForSeconds(throwDuration * 2f);

        throwing = false;

        poisonBottle_Transform.position = targetTransform.position;
        poisonBottle_Transform.gameObject.SetActive(false);

        HandleOnApplyEffect();

    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		fighter.TargetFighter.AddStatus (Fighter.Status.Poisonned, 3);

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
