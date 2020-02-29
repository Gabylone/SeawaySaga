using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusGroup : MonoBehaviour {

	int statusCount = 0;

	StatusFeedback [] statusFeedbacks;

	public GameObject group;

	public GameObject statusFeedbackPrefab;

	// Use this for initialization
	void Start () {
		
		CombatManager.Instance.onFightStart += HandleFightStarting;

//		print ((int)Fighter.Status.None);
		for (int i = 0; i < (int)Fighter.Status.None; i++) {
			GameObject statusFeedbackObj = Instantiate (statusFeedbackPrefab, group.transform) as GameObject;
			statusFeedbackObj.transform.localScale = Vector3.one;
			//statusFeedbackObj.GetComponent<StatusFeedback> ().SetStatus ((Fighter.Status)i);
		}

		statusFeedbacks = GetComponentsInChildren<StatusFeedback> (true);

		Reset ();

	}

	void HandleFightStarting ()
	{
		Reset ();
	}

	void Reset() {
		foreach (var item in statusFeedbacks) {
			item.gameObject.SetActive (false);
		}
	}

	public void HandleOnAddStatus (Fighter.Status status, int count)
	{
		if ((int)status >= statusFeedbacks.Length) {
			print (status.ToString () + " doesct fit in feedbacks ( L : " + statusFeedbacks.Length + ")");
		}

        //Debug.Log("adding status");

        statusFeedbacks[(int)status].Show();
        statusFeedbacks[(int)status].SetStatus(status);
        statusFeedbacks[(int)status].SetMax(count);
        statusFeedbacks[(int)status].SetCount(count);

        statusFeedbacks[(int)status].SetColor (GetStatusColor (status));

        statusFeedbacks [(int)status].transform.localScale = Vector3.one;

		Tween.Bounce (statusFeedbacks [(int)status].transform);
	}

	public void HandleOnRemoveStatus (Fighter.Status status , int count)
	{
		statusFeedbacks [(int)status].SetCount (count);

		if (count == 0) {
			statusFeedbacks [(int)status].Hide ();
		}
	}

	public Color goodEffectColor;
	public Color badEffectColor;
	public Color neutralEffectColor;
	Color GetStatusColor ( Fighter.Status status ) {
		
		Color color = Color.white;

		switch (status) {
		case Fighter.Status.KnockedOut:
		case Fighter.Status.Cussed:
		case Fighter.Status.Poisonned:
			
			// red
			color = badEffectColor;

			break;

		case Fighter.Status.PreparingAttack:
		case Fighter.Status.Provoking:
		
			// white
			color = neutralEffectColor;

			break;

		case Fighter.Status.Jagged:
//		case Fighter.Status.Parrying:
		case Fighter.Status.Protected:
		case Fighter.Status.Toasted:
		case Fighter.Status.Enraged:
		case Fighter.Status.BearTrapped:

			// green
			color = goodEffectColor;

			break;

		default:
			break;
		}

		return color;
	}


}
