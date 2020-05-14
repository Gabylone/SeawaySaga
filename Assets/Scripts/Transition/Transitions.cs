﻿using UnityEngine;
using System.Collections;

public class Transitions : MonoBehaviour {

	public static Transitions Instance;

	[SerializeField]
	private Transition screenTransition;

	public float defaultTransition = 0.3f;

	void Awake () {
		Instance = this;
	}

	void Start () {

        screenTransition.FadeOut( defaultTransition );

		if ( StoryFunctions.Instance )
		StoryFunctions.Instance.getFunction += HandleGetFunction;

		if (CombatManager.Instance) {
			CombatManager.Instance.onFightEnd += HandleFightEnding;
		}

	}

	void HandleFightEnding ()
	{
		// non parce que du coup quand ils s'enfuient ça reste noir
//		actionTransition.Fade = true;
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {

            case FunctionType.Fade:


			    StoryReader.Instance.NextCell ();

			    /*FadeScreen ();
                StoryReader.Instance.Wait (defaultTransition);*/

                StoryReader.Instance.UpdateStory();

                break;
		}
	}

	public Transition ScreenTransition {
		get {
			return screenTransition;
		}
		set {
			screenTransition = value;
		}
	}

	public void FadeScreen () {
		StartCoroutine (FadeCoroutine ());
	}
	IEnumerator FadeCoroutine () {

//		bool fadePlayer = Crews.playerCrew.captain.Icon.CurrentPlacingType == Crews.PlacingType.Discussion;
//		bool fadeOther = Crews.enemyCrew.CrewMembers.Count > 0;
//		if (fadePlayer)
//			Crews.playerCrew.HideCrew ();
//		if ( fadeOther )
//			Crews.enemyCrew.HideCrew ();

		ScreenTransition.FadeIn (defaultTransition);

		yield return new WaitForSeconds (defaultTransition);

		ScreenTransition.FadeOut (defaultTransition);

		yield return new WaitForSeconds (defaultTransition);

//		if (fadePlayer)
//			Crews.playerCrew.ShowCrew ();
//		if ( fadeOther )
//			Crews.enemyCrew.ShowCrew ();
	}
}
