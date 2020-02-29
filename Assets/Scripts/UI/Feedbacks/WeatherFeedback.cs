using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherFeedback : InfoFeedbacks {

	// Use this for initialization
	public override void Start () {

		base.Start ();

		TimeManager.onSetTimeOfDay += HandleOnSetTimeOfDay;
		TimeManager.onSetRain += HandleOnSetRain;
		QuestManager.onFinishQuest += HandleOnFinishQuest;
		CrewMember.onWrongLevel += HandleOnWrongLevel;
	}

	void HandleOnSetRain ()
	{
		Print ("Pluie !", Color.cyan);
	}

	void HandleOnSetTimeOfDay (TimeManager.DayState dayState)
	{
		switch (dayState) {
		case TimeManager.DayState.Day:
			Print ("Jour", Color.yellow);
			break;
		case TimeManager.DayState.Night:
			Print ("Nuit", Color.blue);
			break;
		default:
			break;
		}
	}

	void HandleOnWrongLevel ()
	{
//		Print ("niveau insuffisant");
		Print ("Niveau trop bas");
	}

	void HandleOnFinishQuest (Quest quest)
	{
		Print ("+" + quest.experience + " xp" , Color.white);
	}
}
