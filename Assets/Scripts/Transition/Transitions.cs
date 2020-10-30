using UnityEngine;
using System.Collections;

public class Transitions : MonoBehaviour {

	public static Transitions Instance;

	[SerializeField]
	private Transition screenTransition;

    public Transition actionTransition;

    public float defaultTransition = 0.3f;

	void Awake () {
		Instance = this;
	}

	void Start () {

        screenTransition.FadeOut( defaultTransition );
        actionTransition.FadeOut(defaultTransition);

        if (StoryFunctions.Instance!=null)
		StoryFunctions.Instance.getFunction += HandleGetFunction;

	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {

            case FunctionType.Fade:
                StoryReader.Instance.ContinueStory();
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
