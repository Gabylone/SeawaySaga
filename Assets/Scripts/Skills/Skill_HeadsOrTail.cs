using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HeadsOrTail : Skill {

	private bool heads = false;

    public Coin coin;

    private bool flipped = false;

    public float delay = 1.2f;

    bool firstPart = false;

    public override void Trigger(Fighter fighter)
    {
        base.Trigger(fighter);

        firstPart = true;
    }

    public override void OnSetTarget()
    {

        string[] strs = new string[2]
        {
            "Heads or tails?",
            "Time to flip a coin!"
        };

        string str = strs[Random.Range(0, strs.Length)];

        fighter.Speak(str);

        fighter.animator.SetTrigger("throw");

        SoundManager.Instance.PlayRandomSound("Whoosh");

        Invoke("OnSetTargetDelay" , delay);
    }

    void OnSetTargetDelay()
    {
        base.OnSetTarget();

        firstPart = false;

        SoundManager.Instance.PlayRandomSound("Swipe");

    }

    public override void StartAnimation()
    {
        base.StartAnimation();

        SoundManager.Instance.PlayRandomSound("Whoosh");

        fighter.animator.SetTrigger("hit");
    }

    #region animation event
    public override void AnimationEvent_1()
    {
        base.AnimationEvent_1();

        if (firstPart)
        {
            SoundManager.Instance.PlayRandomSound("Bag");
        }
    }

    public override void AnimationEvent_2()
    {
        base.AnimationEvent_2();

        if (firstPart)
        {
            Vector3 p = fighter.BodyTransform.transform.position;
            p.z -= 2f;
            coin._transform.position = p;

            heads = Random.value < 0.5f;

            coin.heads = heads;

            coin.Flip();
        }
    }
    #endregion

	public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

        SoundManager.Instance.PlayRandomSound("slash");
        SoundManager.Instance.PlayRandomSound("Sword");


        if (heads) {

            SoundManager.Instance.PlaySound("ui_correct");

			fighter.combatFeedback.Display ("Bam !",Color.green);
			fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack , 2f);
		} else {

            SoundManager.Instance.PlaySound("ui_deny");

            fighter.combatFeedback.Display ("Miss !", Color.red);
		}

		EndSkill ();

	}
}
