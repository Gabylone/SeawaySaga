using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TurnFeedback : MonoBehaviour {

	RectTransform rectTransform;

	public float duration = 0.3f;

	float initZAngle = 0f;
	Vector3 initPos;
	float timer = 0f;

	bool lerping = false;

	// Use this for initialization
	void Start () {

		rectTransform = GetComponent<RectTransform> ();

		initZAngle = rectTransform.eulerAngles.z;

		CombatManager.Instance.onFightStart += HandleFightStarting;
		CombatManager.Instance.onFightEnd += HandleFightEnding;
		CombatManager.Instance.onChangeState += HandleOnChangeState;

		Hide ();

	}

	void Update () {
		if (lerping) {
			rectTransform.position = Vector3.Lerp (initPos, CombatManager.Instance.currentFighter.arrowAnchor.position, timer / duration);

			timer += Time.deltaTime;

//			if (timer >= duration)
//				lerping = false;
		}
	}
//
	void HandleOnChangeState (CombatManager.States currState, CombatManager.States prevState)
	{
		if ( currState == CombatManager.States.StartTurn ) {

			if ( CombatManager.Instance.currentFighter.crewMember.side == Crews.Side.Player ) {
                rectTransform.DORotate(Vector3.forward * -initZAngle, duration);
			} else {
                rectTransform.DORotate(Vector3.forward * initZAngle, duration);
			}

			initPos = rectTransform.position;
			timer = 0f;
//			lerping = true;

		}
	}

	void HandleFightEnding ()
	{
		Hide ();
		lerping = false;
	}

	void HandleFightStarting ()
	{
		Show ();
		lerping = true;
	}
	
	void Show () {
		gameObject.SetActive (true);
	}

	void Hide () {
		gameObject.SetActive (false);
	}
}
