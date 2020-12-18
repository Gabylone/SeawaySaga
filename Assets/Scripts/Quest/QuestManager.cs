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

    public bool launchClueQuest = false;

	void Awake () {

		Instance = this;

        Quest.currentQuest = null;

        onNewQuest = null;
    }

    void Start () {

		StoryFunctions.Instance.getFunction+= HandleGetFunction;

		Crews.Instance.onCrewMemberKilled += HandleOnCrewMemberKilled;

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
            case FunctionType.NewClueQuest:
                HandleClueNewQuest();
                break;
            case FunctionType.CheckQuest:
			ContinueQuest ();
			break;
		case FunctionType.SendPlayerBackToGiver:
			SendPlayerBackToGiver ();
			break;
		case FunctionType.ShowQuestOnMap:
                Story_ShowQuestOnMap();
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

    void Story_ShowQuestOnMap()
    {
        DisplayMinimap.Instance.continueStoryOnClose = true;
        Quest.currentQuest.ShowOnMap();
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
        Quest quest = Quest.currentQuest;

        if ( quest == null ) {
			Debug.LogError ("QUEST IS NULL : accomplish quest");
			return;
		}

        DisplayCombatResults.Instance.onConfirm += AccomplishQuest_HandleOnConfirm;
        DisplayCombatResults.Instance.Display("Quest Accomplished !", "Go back to " + quest.giver.Name + " to receive gold and experience...");

        quest.accomplished = true;

        quest.SetTargetIsland(quest.GetOriginIslandData());

        //Story_ShowQuestOnMap();

	}
    void AccomplishQuest_HandleOnConfirm()
    {
        Story_ShowQuestOnMap();
    }
    #endregion

    #region new quest
    void HandleClueNewQuest()
    {
        launchClueQuest = true;

        HandleNewQuest();
    }
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

        DialogueManager.Instance.OtherSpeak(phrase);

        //Invoke("HandleCompletedQuest_Delay", DialogueManager.Instance.DisplayTime);
	}
    void HandleOnEndDialogue()
    {
        DialogueManager.Instance.onEndDialogue -= HandleOnEndDialogue;
        StoryReader.Instance.GoToNode(Coords_CheckForFinishedQuest.newQuest_FallbackNode);
    }
	#endregion

	void ContinueQuest () {

		Quest quest = currentQuests.Find (x => x.GetTargetIslandData() == IslandManager.Instance.GetCurrentIslandData());

		if ( quest != null) {

            // si le joueur retourne sur l'ile d'origine, dans le premier layer d'histoire, continuer quete
            if ( quest.GetTargetIslandData() != quest.GetOriginIslandData()
                && StoryReader.Instance.currentStoryLayer == 0) {
				quest.Continue ();
				return;
			}
		}

        // sinon on ignore, et il returnToGiver quand il recroise le donneur de quete
			
		StoryReader.Instance.NextCell();
		StoryReader.Instance.UpdateStory ();
	}

	public delegate void OnFinishQuest (Quest quest);
	public OnFinishQuest onFinishQuest;
	void FinishQuest ()
	{
		Quest quest = Quest.currentQuest;

		

        DisplayCombatResults.Instance.onConfirm += FinishQuest_HandleOnConfirm;
        DisplayCombatResults.Instance.Display("Quest Finished !", "In addition to receiving gold and experience, you just made a good action ! Your karma just got better");
        DisplayCombatResults.Instance.DisplayResults(quest.goldValue, quest.experience);

        
	}

    void FinishQuest_HandleOnConfirm()
    {
        Quest quest = Quest.currentQuest;

        Karma.Instance.AddKarma(1);

        
        if ( quest.nodeWhenCompleted != null ) {
			StoryReader.Instance.CurrentStoryHandler.fallbackNode = quest.nodeWhenCompleted;
		}


        GoldManager.Instance.AddGold(quest.goldValue);

        foreach (CrewMember member in Crews.playerCrew.CrewMembers)
        {
            member.AddXP(quest.experience);
        }

        if (onFinishQuest != null)
        {
            onFinishQuest(quest);
        }

        finishedQuests.Add(quest);
        currentQuests.Remove(quest);

        CancelInvoke("FinishQuest_HandleOnConfirmDelay");
        Invoke("FinishQuest_HandleOnConfirmDelay", 1f);
    }

    void FinishQuest_HandleOnConfirmDelay()
    {
        StoryReader.Instance.NextCell();
        StoryReader.Instance.UpdateStory();
    }


    public delegate void OnGiveUpQuest ( Quest quest );
	public OnGiveUpQuest onGiveUpQuest;
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
	public void SendPlayerBackToGiver () {
		Debug.LogError ("SEND PLAYER BACK TO GIVER DOESNT EXIST ANYMORE");
	}

    #endregion

	Quest CheckForQuest_OriginCoords {

		get {

			int storyLayer = StoryReader.Instance.currentStoryLayer;

			if (StoryReader.Instance.CurrentStoryHandler.storyType == StoryType.Quest ) {
				storyLayer = StoryReader.Instance.previousStoryLayer;
			}

			return currentQuests.Find ( x => x.giver.SameAs(Crews.enemyCrew.captain.MemberID));
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
