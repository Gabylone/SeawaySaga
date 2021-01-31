﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Fury: Skill {

	public int energyPerTurnAdded = 20;

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

        SoundManager.Instance.PlayLoop("dice_wait");
        SoundManager.Instance.PlayRandomSound("voice_mad");

        string str = "You've got me MAD !";
		fighter.Speak (str);
	}

    public override void StartAnimation()
    {
        base.StartAnimation();

        fighter.animator.SetTrigger("fury");
    }

    public override void HandleOnApplyEffect ()
	{

		base.HandleOnApplyEffect ();

        SoundManager.Instance.StopLoop("dice_wait");

        SoundManager.Instance.PlaySound("Tribal 01");
        SoundManager.Instance.PlaySound("Fury");
        SoundManager.Instance.PlaySound("Fury 2");

        fighter.AddStatus (Fighter.Status.Enraged, 3);

		EndSkill ();

	}

    public override void EndSkill()
    {
        base.EndSkill();
    }

}
