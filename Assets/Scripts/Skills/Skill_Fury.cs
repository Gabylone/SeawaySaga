using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Fury: Skill {

	public int energyPerTurnAdded = 20;

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

		string str = "You've got me MAD !";
		fighter.Speak (str);
	}

	public override void ApplyEffect ()
	{

		base.ApplyEffect ();

		fighter.AddStatus (Fighter.Status.Enraged, 3);

		fighter.onRemoveStatus += HandleOnRemoveStatus;

		fighter.crewMember.energyPerTurn += energyPerTurnAdded;

		EndSkill ();

	}

	void HandleOnRemoveStatus (Fighter.Status status, int count)
	{
		if ( status == Fighter.Status.Enraged && count == 0 ) {
//			fighter.onRemoveStatus -= 
			energyPerTurnAdded -= energyPerTurnAdded;
		}
	}

}
