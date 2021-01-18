using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Skill_BearTrap : Skill {

	public float healthLost = 15f;

    public Vector2 decalToFighter = new Vector2(130, 70);

    public float upDecal = 50f;

    public float throwDuration = 1f;

    public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

		string str = "Come if you have the guts !";
		fighter.Speak (str);
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


        fighter.AttachItemToHand(fighter.iconVisual.bearTrap_Transform);
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        SoundManager.Instance.PlayRandomSound("Whoosh");

        StartCoroutine(ThrowCoroutine());
    }

    IEnumerator ThrowCoroutine()
    {
        fighter.iconVisual.bearTrap_Transform.SetParent(fighter.BodyTransform);

        Vector2 decal = decalToFighter;

        if ( fighter.crewMember.side == Crews.Side.Enemy)
        {
            decal.x = -decalToFighter.x;
        }

        Vector3 targetPos = (Vector2)fighter.GetTransform.position + decal;

        fighter.iconVisual.bearTrap_Transform.DOMove(targetPos + Vector3.up * upDecal, throwDuration);

        yield return new WaitForSeconds(throwDuration);

        fighter.iconVisual.bearTrap_Transform.DORotate(Vector3.zero, throwDuration);
        fighter.iconVisual.bearTrap_Transform.DOMove(targetPos, throwDuration);

        yield return new WaitForSeconds(throwDuration);

        fighter.iconVisual.bearTrap_Transform.GetComponent<Animator>().SetBool("opened", true);

        SoundManager.Instance.PlaySound("beartrap_arm");

        yield return new WaitForSeconds(throwDuration);

        HandleOnApplyEffect();

    }
    #endregion

    public override void HandleOnApplyEffect ()
	{
        base.HandleOnApplyEffect ();

        fighter.AddStatus(Fighter.Status.BearTrapped);

		EndSkill ();

	}

	public override bool MeetsRestrictions (CrewMember member)
	{
		bool bearTrapped = CombatManager.Instance.GetCurrentFighter.HasStatus (Fighter.Status.BearTrapped);

		return bearTrapped == false && base.MeetsRestrictions (member);
	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool bearTrapped = CombatManager.Instance.GetCurrentFighter.HasStatus (Fighter.Status.BearTrapped);

		return !bearTrapped && base.MeetsConditions (member) && Random.value < 0.5f;
    }
}
