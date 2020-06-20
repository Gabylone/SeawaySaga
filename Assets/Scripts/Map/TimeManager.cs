﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour {

	public static TimeManager Instance;

	[Range(0,24)]
	public int startTime = 6;
	public int timeOfDay = 0;
	public int dayDuration = 24;

	public int currentRain = 0;
	public int rainRate_Min = 75;
	public int rainRate_Max = 130;
	public int rainDuration = 10;
	private int rainRate = 0;

    public float minMaskScale = 6f;
    public float maxMaskScale = 10f;


	public int nightStartTime = 21;
	public int nightEndTime = 4;

	public Image nightImage;
	public Image rainImage;

    public GameObject nightMask_Group;
    public GameObject rainMask_Group;

    public DayState dayState = DayState.Day;
	public bool raining = false;

	public enum DayState {
		Day,
		Night,
	}


	void Awake () {
		Instance = this;

        onNextHour = null;
        onSetRain = null;
        onSetTimeOfDay = null;
	}

	void Start () {
		
		StoryFunctions.Instance.getFunction += HandleGetFunction;

		NavigationManager.Instance.EnterNewChunk += NextHour;

		UpdateRainRate ();

        Invoke("UpdateWeather",0.001f);
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            NextHour();
        }
    }

    public void Reset () {
		timeOfDay = startTime;
	}

	#region events
	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.ChangeTimeOfDay:
			ChangeTimeOfDay ();
			break;
		case FunctionType.SetWeather:
			SetWeather (cellParameters);
			break;
		case FunctionType.CheckDay:
			CheckIfDay ();
			break;
		}
	}
	#endregion

	#region rain
	void UpdateRainRate ()
	{
		rainRate = Random.Range ( rainRate_Min , rainRate_Max );
	}

	void UpdateRain ()
	{

		if ( raining ) {
			if (currentRain == rainDuration) {
				HideRain ();
			}
		} else {

			if ( currentRain == rainRate ) {
				SetRain ();
			}

		}
	}
	#endregion

	#region story functions
	void ChangeTimeOfDay ()
	{
		if (dayState == DayState.Day) {
			StartCoroutine (GoToWeather(DayState.Night));
		} else {
			StartCoroutine (GoToWeather(DayState.Day));
		}
	}

	IEnumerator GoToWeather ( DayState targetWeather ) {

        Transitions.Instance.ScreenTransition.FadeIn(0.5f);

		yield return new WaitForSeconds (0.5f);

        int l = 0;
        while (dayState != targetWeather)
        {

            NextHour();

            ++l;

            if (l == 24)
            {
                Debug.LogError("reach limit weather");
                break;
            }
        }

        SoundManager.Instance.UpdateAmbiance();

        Transitions.Instance.ScreenTransition.FadeOut(0.5f);

        StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();
	}

	void CheckIfDay ()
	{
		StoryReader.Instance.NextCell ();

		if (dayState == DayState.Night)
			StoryReader.Instance.SetDecal (1);

		StoryReader.Instance.UpdateStory ();
	}
	#endregion

	#region next hour
	public delegate void OnNextHour ();
	public static OnNextHour onNextHour;
	void NextHour () {


		++timeOfDay;
		//currentRain++;

		if (timeOfDay == dayDuration)
			timeOfDay = 0;

		UpdateTimeOfDay ();

		// rain image
		//UpdateRain();

		if (onNextHour != null)
			onNextHour ();

	}

	void UpdateTimeOfDay ()
	{
		if ( dayState == DayState.Day ) {

			if (timeOfDay >= nightStartTime) {
				SetNight ();
			} else if (timeOfDay < nightEndTime) {
				SetNight();
			}

		} else {

			if (timeOfDay < 12 && timeOfDay >= nightEndTime) {
				SetDay ();
			}
		}
	}
	#endregion

	void SetWeather (string str)
	{
		switch ( str ) {
		case "Day":
			GoToWeather (DayState.Day);
			break;
		case "Night":
			GoToWeather (DayState.Night);
			break;
		case "Rain":
			SetRain ();
			StoryReader.Instance.NextCell ();
			StoryReader.Instance.UpdateStory ();
			break;
		default :
			Debug.LogError ("Set Weather : <" + str + "> doesnt go in any label ?");
			break;
		}
	}

	void SetNight () {
		
		dayState = DayState.Night;

		UpdateWeather ();

		if (onSetTimeOfDay != null)
			onSetTimeOfDay (DayState.Night);
	}
	void SetDay () {
		
		dayState = DayState.Day;

		UpdateWeather ();

		if (onSetTimeOfDay != null)
			onSetTimeOfDay (DayState.Day);

	}

	public delegate void OnSetRain ();
	public static OnSetRain onSetRain;
	public delegate void OnSetTimeOfDay(DayState dayState);
	public static OnSetTimeOfDay onSetTimeOfDay;
	void SetRain () {
		
		raining = true;
		currentRain = 0;

		UpdateRainRate ();
		UpdateWeather ();

		if (onSetRain != null)
			onSetRain ();

	}
	void HideRain() {
		
		raining = false;
		currentRain = 0;

		UpdateWeather ();

	}

	#region save / load
	public void Save () {
		
		SaveManager.Instance.GameData.raining = raining;
		SaveManager.Instance.GameData.night = dayState == DayState.Night;

		SaveManager.Instance.GameData.timeOfDay = timeOfDay;

		SaveManager.Instance.GameData.currentRain = currentRain;
	}

	public void Load () {
		
		raining = SaveManager.Instance.GameData.raining;
		dayState = SaveManager.Instance.GameData.night ? DayState.Night : DayState.Day;
		timeOfDay = SaveManager.Instance.GameData.timeOfDay;
		currentRain = SaveManager.Instance.GameData.currentRain;
	}

	void UpdateWeather() {

        // set mask scales
        int range = Boats.Instance.playerBoatInfo.shipRange;
        float lerp = range - 1 / 3;
        Vector3 scale = Vector3.one * Mathf.Lerp(minMaskScale, minMaskScale, lerp);
        scale.y = nightMask_Group.transform.localScale.y;
        nightMask_Group.transform.localScale = scale;
        rainMask_Group.transform.localScale = scale;

        if (dayState == DayState.Night)
        {
            nightImage.gameObject.SetActive(true);
            nightMask_Group.SetActive(true);
        }
        else
        {
            nightImage.gameObject.SetActive(false);
            nightMask_Group.SetActive(false);
        }

        if (raining)
        {
            rainImage.gameObject.SetActive(true);
            rainMask_Group.SetActive(true);
        }
        else
        {
            rainImage.gameObject.SetActive(false);
            rainMask_Group.SetActive(false);
        }
    }
	#endregion
}
