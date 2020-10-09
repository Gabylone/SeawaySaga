using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Robbery : Skill {

    public Transform goldBag_Transform;

	public int goldStolen = 30;

    public float targetFighterStopDistance = -3f;

    public float initFighterStopDistance = 0f;

	public int minimumGoldToSteal = 15;

    public override void Trigger(Fighter fighter)
    {
        base.Trigger(fighter);

        ChangeFightPosition();
    }

    public override void OnSetTarget()
    {
        base.OnSetTarget();
    }

    public override void StartAnimation()
    {
        base.StartAnimation();

        FlipFighter();

        fighter.animator.SetTrigger("grab");

        SoundManager.Instance.PlayRandomSound("Swipe");

    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

        fighter.AttachItemToHand(goldBag_Transform);

        SoundManager.Instance.PlayRandomSound("Coins");
        SoundManager.Instance.PlayRandomSound("Coins");
        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Bag");

        if ( fighter.crewMember.side == Crews.Side.Enemy )
        {
			GoldManager.Instance.RemoveGold(goldStolen);
            SoundManager.Instance.PlaySound("ui_deny");
        }
        else
        {
            SoundManager.Instance.PlaySound("ui_correct");
            GoldManager.Instance.AddGold(goldStolen);
		}

		fighter.combatFeedback.Display ("+" + goldStolen, Color.yellow);
		fighter.TargetFighter.combatFeedback.Display ("-" + goldStolen, Color.red);

		EndSkill ();

        ResetFightPosition();

        Invoke("FlipFighter", 0.8f);

        Invoke("HideGoldBag", 4f);

    }

    void HideGoldBag()
    {
        goldBag_Transform.gameObject.SetActive(false); 
    }

    private void FlipFighter()
    {
        Vector3 scale = fighter.BodyTransform.localScale;
        scale.x = -scale.x;
        fighter.BodyTransform.localScale = scale;

        SoundManager.Instance.PlayRandomSound("Whoosh");
    }

    void ChangeFightPosition()
    {
        initFighterStopDistance = fighter.stopDistance;
        fighter.stopDistance = targetFighterStopDistance;
    }

    void ResetFightPosition()
    {
        fighter.stopDistance = initFighterStopDistance;
    }

    public override bool MeetsConditions (CrewMember member)
	{
		bool hasMinimumGold = false;

		if (GoldManager.Instance.goldAmount > minimumGoldToSteal)
			hasMinimumGold = true;

		return hasMinimumGold && base.MeetsConditions (member);
	}
}
