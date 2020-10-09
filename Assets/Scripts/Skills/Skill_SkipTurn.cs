using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_SkipTurn: Skill {

    public override void Trigger(Fighter fighter)
    {
        base.Trigger(fighter);

        //HandleOnApplyEffect();
    }

    public override void HandleOnApplyEffect ()
	{
		base.HandleOnApplyEffect ();

        //EndSkill ();

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlaySound("End Turn");

        fighter.EndTurn();
		CombatManager.Instance.NextTurn ();
	}

}
