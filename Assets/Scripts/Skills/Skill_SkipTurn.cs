using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_SkipTurn: Skill {

    public override void Trigger(Fighter fighter)
    {
        if (fighter.killed)
        {
            Debug.LogError("d'ou il est mort il trigger un skill");
        }

        Debug.Log("triggering skip turn");

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
