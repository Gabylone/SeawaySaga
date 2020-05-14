using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PledgeOfFeast : Skill {

	public int energyAmount = 10;

	public override void OnSetTarget ()
	{
		base.OnSetTarget ();

		string str = "Tonight, " + fighter.TargetFighter.crewMember.MemberName + ", I'm opening a new bottle for you !";
		fighter.Speak (str);

	}

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		fighter.TargetFighter.crewMember.AddEnergy (energyAmount);
		fighter.TargetFighter.ShowInfo ();

		EndSkill ();

	}
}
