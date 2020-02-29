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

		string str = "Essayes de venir un coup pour voir !";
		fighter.Speak (str);
	}

	public override void ApplyEffect ()
	{
		if (fighter.HasStatus (Fighter.Status.BearTrapped))
			return;

		base.ApplyEffect ();

		fighter.AddStatus (Fighter.Status.BearTrapped);

		GameObject bearTrapObj = Instantiate (beapTrapPrefab, fighter.transform.parent) as GameObject;

		if (fighter.crewMember.side == Crews.Side.Enemy) {
			bearTrapObj.transform.localPosition = new Vector2 (-decalToFighter.x , decalToFighter.y );
		} else {
			bearTrapObj.transform.localPosition = new Vector2 (decalToFighter.x , decalToFighter.y );
		}
		bearTrapObj.transform.localScale = Vector3.one;

		foreach (var item in GetComponentsInChildren<SpriteRenderer>()) {
			item.sortingOrder = fighter.fightSprites.allSprites [0].sortingOrder;
		}

//		bearTrapObj.transform.position = new Vector3 (target.position.x , fighter.transform.position.y ,0f);

		Tween.Bounce (bearTrapObj.transform);

//		foreach (var bearTrapImage in  bearTrapObj.GetComponentsInChildren<Image>()) {
//			
//		}


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
