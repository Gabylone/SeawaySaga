using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour {

	public static QuestManager Instance;

	public List<Quest> currentQuests = new List<Quest>();
	public List<Quest> finishedQuests = new List<Quest>();

    public bool metPersonOnIsland = false;

	public delegate void OnNewQuest ();
	public OnNewQuest onNewQuest;

	public int maxQuestAmount = 20;

    public bool showingQuestOnMap = false;

	void Awake () {

		Instance = this;

        Quest.onSetTargetCoords = null;
        Quest.currentQuest = null;
        Quest.showQuestOnMap = null;

        onFinishQuest = null;
        onGiveUpQuest = null;
        onNewQuest = null;


    }

    void Start () {

		StoryFunctions.Instance.getFunction+= HandleGetFunction;

		Crews.Instance.onCrewMemberKilled += HandleOnCrewMemberKilled;

        StoryInput.Instance.onPressInput += HandleOnPressInput;

    }

    void HandleOnCrewMemberKilled (CrewMember crewMember)
	{
		if ( crewMember.side == Crews.Side.Enemy ) {

			for (int questIndex = 0; questIndex < currentQuests.Count; questIndex++) {

				Quest item = currentQuests [questIndex];

				if (item.giver.id == crewMember.MemberID.id) {

					GiveUpQuest (item);

				}

			}

		}
	}

	#region event
	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.NewQuest:
			HandleNewQuest ();
			break;
		case FunctionType.CheckQuest:
			ContinueQuest ();
			break;
		case FunctionType.SendPlayerBackToGiver:
			SendPlayerBackToGiver ();
			break;
		case FunctionType.ShowQuestOnMap:
			Story_ShowQuestOnMap ();
            StoryInput.Instance.WaitForInput();
                break;
            case FunctionType.FinishQuest:
			FinishQuest ();
			break;
		case FunctionType.AccomplishQuest:
			AccomplishQuest ();
			break;
		case FunctionType.IsQuestAccomplished:
			CheckIfQuestIsAccomplished ();
			break;
		case FunctionType.GiveUpQuest:
			GiveUpActiveQuest ();
			break;
		case FunctionType.AddCurrentQuest:
			AddNewQuest ();
			break;
		}
	}

    void CheckIfQuestIsAccomplished ()
	{
		if ( Quest.currentQuest == null ) {
			Debug.LogError ("QUEST IS NULL : CheckIfQuestIsAccomplished");
			return;
		}

		int decal = Quest.currentQuest.accomplished ? 0 : 1;

		StoryReader.Instance.NextCell ();

		StoryReader.Instance.SetDecal (decal);

		StoryReader.Instance.UpdateStory ();
	}

	void AccomplishQuest ()
	{
		if ( Quest.currentQuest == null ) {
			Debug.LogError ("QUEST IS NULL : accomplish quest");
			return;
		}

		Quest.currentQuest.accomplished = true;



		Quest.currentQuest.SetTargetCoords (Quest.currentQuest.originCoords);

		Quest.currentQuest.ShowOnMap ();

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();

	}
	#endregion

	#region new quest
	void HandleNewQuest () {

		// CHECK FINISHED QUESTS
		Quest quest = Coords_CheckForFinishedQuest;

		if ( quest != null ) {
			HandleCompletedQuest (quest);
			return;
		}

		// CHECK CURRENT QUESTS
		quest = CheckForQuest_OriginCoords;

		if (quest != null) {
			quest.ReturnToGiver ();
		} else {
			EnterQuest ();
		}
	}
	void EnterQuest () {
		Quest newQuest = new Quest ();
		newQuest.StartStory ();
		// pas besoin d'update la story puisque ça en lance une nouvelle
	}
	void AddNewQuest () {
		// create quest
		if (currentQuests.Count == maxQuestAmount) {
			Invoke ("FallBackDelay",1f);
		} else {
			currentQuests.Add (Quest.currentQuest);
		}

		Quest.currentQuest.Init ();

		if (onNewQuest != null)
			onNewQuest ();

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();
	}
	void FallBackDelay () {
//		Quest newQuest = new Quest ();
//		newQuest.GetNewQuestnode ();
//		StoryReader.Instance.GoToNode (newQuest.newQuest_FallbackNode);

		StoryLauncher.Instance.EndStory ();

	}
	#endregion

	#region completed quest
	void HandleCompletedQuest (Quest quest )
	{
		string phrase = "I've told you I have a problem, but you gave up on me";
		if (quest.accomplished) {
			phrase = "I'll never thank you enough for helping me";
		}

		DialogueManager.Instance.SetDialogueTimed (phrase, Crews.enemyCrew.captain);

		Invoke ("HandleCompletedQuest_Delay", DialogueManager.Instance.DisplayTime);
	}
	void HandleCompletedQuest_Delay () {
		StoryReader.Instance.GoToNode (Coords_CheckForFinishedQuest.newQuest_FallbackNode);
	}
	#endregion

	void ContinueQuest () {

		Quest quest = currentQuests.Find (x => x.targetCoords == Boats.Instance.playerBoatInfo.coords);

		if ( quest != null) {

			if (quest.targetCoords != quest.originCoords && StoryReader.Instance.currentStoryLayer == 0) {

//				Debug.Log ();

				quest.Continue ();

				return;

			}
		}
			
		StoryReader.Instance.NextCell();
		StoryReader.Instance.UpdateStory ();
	}

	public delegate void OnFinishQuest (Quest quest);
	public static OnFinishQuest onFinishQuest;
	void FinishQuest ()
	{
		Quest quest = Quest.currentQuest;
		
		GoldManager.Instance.AddGold(quest.goldValue);

		foreach ( CrewMember member in Crews.playerCrew.CrewMembers ) {
			member.AddXP (quest.experience);
		}

		Karma.Instance.AddKarma (1);

		finishedQuests.Add (quest);
		currentQuests.Remove (quest);

		if (onFinishQuest != null) {
			onFinishQuest (quest);
		}

		if ( quest.nodeWhenCompleted != null ) {
			StoryReader.Instance.CurrentStoryHandler.fallbackNode = quest.nodeWhenCompleted;
		}

		StoryReader.Instance.NextCell();
		StoryReader.Instance.UpdateStory ();
	}

	public delegate void OnGiveUpQuest ( Quest quest );
	public static OnGiveUpQuest onGiveUpQuest;
	public void GiveUpQuest (Quest quest) {
		
		currentQuests.Remove (quest);
		finishedQuests.Add (quest);

		if (onGiveUpQuest != null) {
			onGiveUpQuest(quest);
		}
	}

	void GiveUpActiveQuest ()
	{
		GiveUpQuest (Quest.currentQuest);

		StoryReader.Instance.NextCell();
		StoryReader.Instance.UpdateStory ();
	}

	#region map stuff
	public void Story_ShowQuestOnMap () {

        StoryReader.Instance.NextCell();
        StoryReader.Instance.Wait(1);

        showingQuestOnMap = true;
		Quest.currentQuest.ShowOnMap ();
    }

    public void HideQuestOnMap()
    {
        showingQuestOnMap = false;
    }

	public void SendPlayerBackToGiver () {
		Debug.LogError ("SEND PLAYER BACK TO GIVER DOESNT EXIST ANYMORE");
	}

    void HandleOnPressInput()
    {
        /*if (showingQuestOnMap)
        {
            StoryReader.Instance.ContinueStory();
        }*/
    }

    #endregion

    public Quest Coords_CheckForTargetQuest {
		get {
			return currentQuests.Find ( x=> x.targetCoords == Boats.Instance.playerBoatInfo.coords);
		}
	}

	Quest CheckForQuest_OriginCoords {
		get {

			foreach (Quest quest in currentQuests) {
				print (quest.Story.dataName);
				print (quest.layer);
			}

			int storyLayer = StoryReader.Instance.currentStoryLayer;

			if (StoryReader.Instance.CurrentStoryHandler.storyType == StoryType.Quest ) {
				storyLayer = StoryReader.Instance.previousStoryLayer;
			}

			return currentQuests.Find ( x => x.giver.SameAs(Crews.enemyCrew.captain.MemberID));
//			return currentQuests.Find ( x => 
//				x.originCoords == Boats.Instance.playerBoatInfo.coords && 
//				storyLayer == x.layer &&
//				x.row == StoryReader.Instance.Col &&
//				x.col == StoryReader.Instance.Row
//			);
		}
	}

	Quest Coords_CheckForFinishedQuest {
		get {

			int storyLayer = StoryReader.Instance.currentStoryLayer;

			if (StoryReader.Instance.CurrentStoryHandler.storyType == StoryType.Quest ) {
				storyLayer = StoryReader.Instance.previousStoryLayer;
			}

			return finishedQuests.Find ( x => x.giver.SameAs(Crews.enemyCrew.captain.MemberID));

//			return finishedQuests.Find ( x => 
//				x.originCoords == Boats.Instance.playerBoatInfo.coords && 
//				storyLayer == x.layer &&
//				x.row == StoryReader.Instance.Col &&
//				x.col == StoryReader.Instance.Row
//			);
		}
	}
}
