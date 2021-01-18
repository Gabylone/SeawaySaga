using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Skill_Dynamite : Skill {

    public Transform dynamite_Transform;

    public GameObject dynamiteExplosion_Obj;
    public Transform dynamiteExplosion_Transform;

    public float upDecal = 50f;
    public float throwDuration = 1f;

    public override void Trigger(Fighter fighter)
    {
        base.Trigger(fighter);

        string str = "Fire in the hole !";

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

        fighter.AttachItemToHand(dynamite_Transform);

        SoundManager.Instance.PlayLoop("fuse");
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        StartCoroutine(ThrowCoroutine());
    }

    IEnumerator ThrowCoroutine()
    {
        dynamite_Transform.SetParent(fighter.BodyTransform);

        List<Fighter> fighters = CombatManager.Instance.getCurrentFighters(Crews.otherSide(fighter.crewMember.side));
        Vector3 targetPos = (Vector2)fighters[0].GetTransform.position;

        Vector3 midPos = fighter.GetTransform.position + (targetPos - fighter.GetTransform.position) / 2f;
        midPos += Vector3.up * upDecal;

        dynamite_Transform.DOMove(midPos, throwDuration);

        yield return new WaitForSeconds(throwDuration);

        dynamite_Transform.DOMove(targetPos, throwDuration);

        yield return new WaitForSeconds(throwDuration);

        HandleOnApplyEffect();

        SoundManager.Instance.StopLoop("fuse");

        dynamite_Transform.gameObject.SetActive(false);


    }
    #endregion



    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

		List<Fighter> fighters = CombatManager.Instance.getCurrentFighters (Crews.otherSide (fighter.crewMember.side));

        // before removing anything
        switch (fighter.crewMember.side)
        {
            case Crews.Side.Player:
                dynamiteExplosion_Transform.position = fighters[0].GetTransform.position;
                break;
            case Crews.Side.Enemy:
                dynamiteExplosion_Transform.position = fighters[1].GetTransform.position;
                break;
            default:
                break;
        }


        for (int i = fighters.Count-1; i > -1; i--)
        {
            fighters[i].Hurt(value);
        }

        
        dynamiteExplosion_Obj.SetActive(true);

        Invoke("HideExplositionEffect", 1f);


        SoundManager.Instance.PlaySound("explosion");

		EndSkill ();

	}


    void HideExplositionEffect()
    {
        dynamiteExplosion_Obj.SetActive(false);

    }

    public override bool MeetsConditions (CrewMember member)
	{

		bool moreThanOneMember = CombatManager.Instance.getCurrentFighters (Crews.otherSide (member.side)).Count > 1;

		return moreThanOneMember && base.MeetsConditions (member);
	}

}


