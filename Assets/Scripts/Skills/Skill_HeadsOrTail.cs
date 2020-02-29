using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_HeadsOrTail : Skill {

	public GameObject coinPrefab;

	bool heads = false;

	public override void OnSetTarget ()
	{
//		base.OnSetTarget ();

		CreateCoin ();

		Invoke ("OnSetTargetDelay",1.2f);
	}

	void OnSetTargetDelay () {
		base.OnSetTarget ();
	}

	void CreateCoin ()
	{
		GameObject coin = Instantiate (coinPrefab, fighter.transform.parent) as GameObject;

		Vector3 p = fighter.BodyTransform.transform.position;

		p.z = -2f;
		coin.transform.position = p;

		heads = Random.value < 0.5f;

		coin.GetComponent<Coin> ().heads = heads;
	}

	public override void ApplyEffect ()
	{
		base.ApplyEffect ();

		if (heads) {
			fighter.combatFeedback.Display ("Bim !",Color.green);
			fighter.TargetFighter.GetHit (fighter, fighter.crewMember.Attack , 2f);
		} else {
			fighter.combatFeedback.Display ("Raté !", Color.red);
		}

		EndSkill ();

	}
}
