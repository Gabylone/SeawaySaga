﻿using UnityEngine;
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
    public void PlayStory(StoryManager storyManager, StoryLauncher.StorySource source)
    {
        if (playingStory)
            return;

        InGameMenu.Instance.HideMenuButtons();

        CurrentStorySource = source;

        StoryReader.Instance.CurrentStoryManager = storyManager;
        StoryReader.Instance.CurrentStoryManager.hasBeenPlayed = true;
        StoryReader.Instance.Reset();

        WorldTouch.Instance.Lock();

        CamBehavior.Instance.Zoom();

        PlayerBoat.Instance.EndMovenent();
        InGameMenu.Instance.Hide();

        Invoke("DisplayBackground", 1f);

        playingStory = true;

        SoundManager.Instance.UpdateAmbianceSound();
    }

    void DisplayBackground()
    {
        Transitions.Instance.ScreenTransition.FadeIn(0.5f);

        Invoke("StartStory", 1f);
    }

    void StartStory()
    {
        InGameMenu.Instance.ShowMenuButtons();

        QuestManager.Instance.metPersonOnIsland = false;

        InGameBackGround.Instance.ShowBackground();
        Transitions.Instance.ScreenTransition.FadeOut(0.5f);

        playingStory = true;

        if (onPlayStory != null)
            onPlayStory();

        StoryReader.Instance.UpdateStory();
    }

    public void EndStory()
    {

        switch (CurrentStorySource)
        {
            case StorySource.none:
                // kek
                break;
            case StorySource.island:
                Chunk.currentChunk.state = ChunkState.VisitedIsland;
                SaveManager.Instance.GameData.progression++;
                break;
            case StorySource.boat:
                //
                Boats.Instance.currentEnemyBoat.LeaveBoat();
                break;
            default:
                break;
        }

        // hides crew when leaving ISLAND AND STORY
        if (StoryReader.Instance.CurrentStoryHandler.storyType != StoryType.Quest)
            Crews.enemyCrew.UpdateCrew(Crews.PlacingType.Hidden);

        if (StoryReader.Instance.currentStoryLayer > 0)
        {
            StoryReader.Instance.FallBackToPreviousStory();
            return;
        }

        Chunk.currentChunk.SaveIslandData(Coords.current);

        Invoke("EndStoryDelay", 0.5f);

        Transitions.Instance.ScreenTransition.FadeIn( 0.5f );

        Boats.Instance.ResumeBoats();


    }

    void EndStoryDelay()
    {

        SoundManager.Instance.PlaySound("leave port");

        Transitions.Instance.ScreenTransition.FadeOut(0.5f);

        playingStory = false;

        // place captain
        //Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Portraits);
        Crews.playerCrew.UpdateCrew(Crews.PlacingType.Portraits);

        InGameBackGround.Instance.Hide();

        CamBehavior.Instance.UnZoom();

        WorldTouch.Instance.Unlock();

        if (onEndStory != null)
            onEndStory();

        SoundManager.Instance.UpdateAmbianceSound();


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