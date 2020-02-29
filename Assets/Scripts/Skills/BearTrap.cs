using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearTrap : MonoBehaviour {

	public Fighter fighter;

	void Start () {
		CombatManager.Instance.onFightEnd += HandleFightEnding;
	}

	void HandleFightEnding ()
	{
		DestroyBearTrap ();
	}

	public void DestroyBearTrap () {
		fighter.onRemoveStatus -= HandleOnRemoveFighterStatus;
		CombatManager.Instance.onFightEnd -= HandleFightEnding;
		Destroy (gameObject);
	}

	public void HandleOnRemoveFighterStatus (Fighter.Status status, int count)
	{
		if ( status == Fighter.Status.BearTrapped && count == 0) {
			GetComponent<Animator> ().SetTrigger ("Crush");
		}
	}

//	void OnDestroy () {
//	}
}
