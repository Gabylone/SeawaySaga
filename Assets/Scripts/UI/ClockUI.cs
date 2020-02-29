using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ClockUI : MonoBehaviour {

	[SerializeField]
	private Transform clockBackground;

    public float duration = 0.6f;

//	[SerializeField]
//	private Transform minuteNeedle;

	float startAngle = 0f;

	[SerializeField]
	private Image nightImage;

	// Use this for initialization
	void Start ()
	{
//		CrewInventory.Instance.openInventory += HandleOpenInventory;

		TimeManager.onNextHour += UpdateNeedle;
//		NavigationManager.Instance.EnterNewChunk += UpdateNeedle;
		StoryFunctions.Instance.getFunction += HandleGetFunction;

		InitClock ();
		UpdateNeedle ();
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.ChangeTimeOfDay:
		case FunctionType.SetWeather:
			UpdateNeedle ();
			break;
		}
	}

	void InitClock ()
	{
		/*float angle = (float)TimeManager.Instance.nightStartTime * 360f / (float)TimeManager.Instance.dayDuration;

		nightImage.transform.eulerAngles = new Vector3 (0,0,180-angle);

		int nightDuration = (TimeManager.Instance.dayDuration - TimeManager.Instance.nightStartTime) + TimeManager.Instance.nightEndTime;

		nightImage.fillAmount = (float)nightDuration / (float)TimeManager.Instance.dayDuration;*/
	}


	void UpdateNeedle ()
	{
		float angle = TimeManager.Instance.timeOfDay * 360f / TimeManager.Instance.dayDuration;
		Vector3 targetAngles = new Vector3 (0,0, angle);

        clockBackground.Rotate(targetAngles, duration);
		//clockBackground.eulerAngles = targetAngles;
	}
}
