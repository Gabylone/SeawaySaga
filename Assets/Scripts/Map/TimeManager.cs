using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeManager : MonoBehaviour {

	public static TimeManager Instance;

	[Range(0,24)]
	public int startTime = 6;
	public int timeOfDay = 0;
	public int dayDuration = 24;

	public int currentRain = 0;
    public int rainRate = 80;

    public bool warned = false;

    public int rainRate_Min = 75;
    public int rainRate_Max = 130;

	public int rainDuration = 10;

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

    public delegate void OnSetRain();
    public static OnSetRain onSetRain;
    public delegate void OnSetTimeOfDay(DayState dayState);
    public static OnSetTimeOfDay onSetTimeOfDay;

    public enum DayState {
		Day,
		Night,
	}


	void Awake () {
		Instance = this;

        onSetRain = null;
        onSetTimeOfDay = null;
	}

	void Start () {
		
		StoryFunctions.Instance.getFunction += HandleGetFunction;

        Invoke("StartDelay", 0.01f);
	}

    void StartDelay()
    {
        UpdateRain();
        UpdateWeather();
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
	void RandomizeRainRate ()
	{
		rainRate = Random.Range ( rainRate_Min , rainRate_Max );
	}

	void UpdateRain ()
	{
		if ( raining ) {
			if (currentRain >= rainDuration) {
				StopRain ();
			}
		} else {

            if (currentRain == (rainRate-5) && !warned)
            {
                Invoke("ShowNarrator", 2f);
            }

            if ( currentRain >= rainRate ) {
				SetRain ();
			}

		}
	}

    void ShowNarrator()
    {
        warned = true;
        string str = "Black clouds begin to emerge from the horizon. CAPITAINE hears thunder rumbling at a distance. The sky darkens and the air is thickening";

        MessageDisplay.Instance.Display(str);
        MessageDisplay.Instance.onValidate += HandleOnValidate;

        PlayerBoat.Instance.EndMovenent();
    }

    void HandleOnValidate()
    {
        string str = "A storm is coming. Better bring the crew and NOMTRESOR to safety at an inn, or camp in a nearby island.";
        MessageDisplay.Instance.onValidate -= HandleOnValidate;
        MessageDisplay.Instance.Display(str);
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

        SoundManager.Instance.PlaySound("Mystick Tap");

        StoryReader.Instance.Wait(1f);
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

        ClockUI.Instance.UpdateUI();

        Transitions.Instance.ScreenTransition.FadeOut(0.5f);
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
	public void NextHour () {

		++timeOfDay;
        ++currentRain;

		if (timeOfDay == dayDuration)
        {
            foreach (var item in Crews.playerCrew.CrewMembers)
            {
                item.daysOnBoard++;
            }

            timeOfDay = 0;
        }


        UpdateRain();
        UpdateTimeOfDay ();

        UpdateWeather();
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
                StartCoroutine(GoToWeather(DayState.Day));
			break;
            case "Night":
                StartCoroutine(GoToWeather(DayState.Night));
                break;
		case "Rain":
			SetRain ();
			break;
            default :
			Debug.LogError ("Set Weather : <" + str + "> doesnt go in any label ?");
			break;
		}

        StoryReader.Instance.Wait(1f);

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
    
	void SetRain () {
		
		raining = true;

        warned = false;

        currentRain = 0;

        string str = "CAPITAINE and NOMTRESOR have been hit by a storm, better get to safety quickly before anyone gets hurt.";
        MessageDisplay.Instance.Display(str);

        UpdateWeather();

		if (onSetRain != null)
			onSetRain ();

	}
	void StopRain() {
		
		raining = false;
		currentRain = 0;
        //RandomizeRainRate();

        UpdateWeather();

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
            ShowNight();
        }
        else
        {
            HideNight();
        }

        if (raining)
        {
            ShowRain();
        }
        else
        {
            HideRain();
        }

        SoundManager.Instance.UpdateAmbianceSound();
    }

    public void ShowNight()
    {
        if (InGameBackGround.Instance.IsInterior())
        {
            HideNight();
            return;
        }

        nightImage.gameObject.SetActive(true);
        nightMask_Group.SetActive(true);
    }

    public void HideNight()
    {
        nightImage.gameObject.SetActive(false);
        nightMask_Group.SetActive(false);
    }

    public void ShowRain()
    {
        if (InGameBackGround.Instance.IsInterior())
        {
            HideRain();
            return;
        }

        rainImage.gameObject.SetActive(true);
        rainMask_Group.SetActive(true);

        ClockUI.Instance.ShowStorm();
    }

    public void HideRain()
    {
        rainImage.gameObject.SetActive(false);
        rainMask_Group.SetActive(false);

        ClockUI.Instance.HideStorm();
    }
	#endregion
}
