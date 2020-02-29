using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StoryLauncher : MonoBehaviour {

	public static StoryLauncher Instance;

	private bool playingStory = false;

	public delegate void PlayStoryEvent ();
	public PlayStoryEvent onPlayStory;
	public delegate void EndStoryEvent ();
	public EndStoryEvent onEndStory;

	public enum StorySource {

		none,

		island,
		boat,
        other,
	}

	private StorySource currentStorySource;

	void Awake() {
		Instance = this;
	}

	void Start () {
		StoryFunctions.Instance.getFunction += HandleGetFunction;
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.Leave:
			EndStory ();
			break;

		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.PageDown)) {
			print ("quittage de force");
			EndStory ();
		}
	}

	#region propeties
	public void PlayStory ( StoryManager storyManager , StoryLauncher.StorySource source) {

		if (playingStory)
			return;

        CurrentStorySource = source;

        if (onPlayStory != null)
            onPlayStory();

        StoryReader.Instance.CurrentStoryManager = storyManager;
        StoryReader.Instance.Reset ();


        playingStory = true;

        PlayStoryDelay();
        //Invoke("PlayStoryDelay", 1f);
	}

    void PlayStoryDelay()
    {
        Transitions.Instance.ActionTransition.FadeIn(0.5f);

        // place captain
        //Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Discussion);
        StoryReader.Instance.UpdateStory();
    }

	public void EndStory () {

		switch (CurrentStorySource) {
		case StorySource.none:
			// kek
			break;
		case StorySource.island:
			Chunk.currentChunk.state = ChunkState.VisitedIsland;
            SaveManager.Instance.GameData.progression++;
			break;
		default:
			break;
		}



		// hides crew when leaving ISLAND AND STORY
		if ( StoryReader.Instance.CurrentStoryHandler.storyType != StoryType.Quest )
			Crews.enemyCrew.UpdateCrew (Crews.PlacingType.Hidden);

		if ( StoryReader.Instance.currentStoryLayer > 0 ) {
			StoryReader.Instance.FallBackToPreviousStory ();
			return;
		}

		playingStory = false;

		Transitions.Instance.ActionTransition.FadeOut (0.5f);

		// place captain
		Crews.playerCrew.captain.Icon.MoveToPoint (Crews.PlacingType.Map);



		if (onEndStory != null)
			onEndStory ();

		Chunk.currentChunk.Save (Coords.current);

	}

	public bool PlayingStory {
		get {
			return playingStory;
		}
	}
	#endregion

	public StorySource CurrentStorySource {
		get {
			return currentStorySource;
		}
		set {
			currentStorySource = value;
		}
	}
}