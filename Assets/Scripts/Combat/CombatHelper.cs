using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatHelper : MonoBehaviour {

	public GameObject group;
	public Text uiText;

	// Use this for initialization
	void Start () {
		CombatManager.Instance.onChangeState += HandleOnChangeState;
		Hide ();
	}

	void HandleOnChangeState (CombatManager.States currState, CombatManager.States prevState)
	{
		Hide ();

		if ( currState == CombatManager.States.PlayerActionChoice ) {
			DisplayFeedback ("Choisir action");
		}

		if ( currState == CombatManager.States.PlayerMemberChoice ) {
			DisplayFeedback ("Choisir cible");
		}
	}

	void DisplayFeedback (string str) {
		Show ();
		uiText.text = str;
		Tween.Bounce (group.transform);
	}

	void Show () {
		group.SetActive (true);
	}

	void Hide () {
		group.SetActive (false);
	}
}
