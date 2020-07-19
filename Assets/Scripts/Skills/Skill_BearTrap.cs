using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_BearTrap : Skill {

	public GameObject beapTrapPrefab;
	public Transform target;

	public float healthLost = 15f;

	public Vector2 decalToFighter = new Vector2(130,70);

	public override void Trigger (Fighter fighter)
	{
		base.Trigger (fighter);

		string str = "Come if you have the gusts !";
		fighter.Speak (str);
	}

	public override void HandleOnApplyEffect ()
	{
		if (fighter.HasStatus (Fighter.Status.BearTrapped))
			return;

		base.HandleOnApplyEffect ();

		fighter.AddStatus (Fighter.Status.BearTrapped);

		GameObject bearTrapObj = Instantiate (beapTrapPrefab, fighter.transform.parent) as GameObject;

		if (fighter.crewMember.side == Crews.Side.Enemy) {
			bearTrapObj.transform.localPosition = new Vector2 (-decalToFighter.x , decalToFighter.y );
		} else {
			bearTrapObj.transform.localPosition = new Vector2 (decalToFighter.x , decalToFighter.y );
		}
		bearTrapObj.transform.localScale = Vector3.one;

		Tween.Bounce (bearTrapObj.transform);

		fighter.onRemoveStatus += bearTrapObj.GetComponent<BearTrap> ().HandleOnRemoveFighterStatus;
		bearTrapObj.GetComponent<BearTrap> ().fighter = fighter;

		EndSkill ();

	}

	public override bool MeetsRestrictions (CrewMember member)
	{
		bool bearTrapped = CombatManager.Instance.currentFighter.HasStatus (Fighter.Status.BearTrapped);

		return bearTrapped == false && base.MeetsRestrictions (member);
	}

	public override bool MeetsConditions (CrewMember member)
	{
		bool bearTrapped = CombatManager.Instance.currentFighter.HasStatus (Fighter.Status.BearTrapped);

		return !bearTrapped && base.MeetsConditions (member);
	}
}
