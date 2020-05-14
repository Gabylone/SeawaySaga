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
		Print ("Raining !", Color.cyan);
	}

	void HandleOnSetTimeOfDay (TimeManager.DayState dayState)
	{
		switch (dayState) {
		case TimeManager.DayState.Day:
			Print ("Day", Color.yellow);
			break;
		case TimeManager.DayState.Night:
			Print ("Night", Color.blue);
			break;
		default:
			break;
		}
	}

	void HandleOnWrongLevel ()
	{
//		Print ("niveau insuffisant");
		Print ("Insufficient Level");
	}

	void HandleOnFinishQuest (Quest quest)
	{
		Print ("+" + quest.experience + " xp" , Color.white);
	}
}
