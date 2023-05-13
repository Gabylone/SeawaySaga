using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class QuestManager : MonoBehaviour {

	public static QuestManager Instance;

	public List<Quest> currentQuests = new List<Quest>();
	public List<Quest> finishedQuests = new List<Quest>();

    public delegate void OnFinishQuest(Quest quest);
    public OnFinishQuest onFinishQuest;

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

    public void ShowQuestsOnMap()
    {
        foreach (var quest in currentQuests)
        {
            MinimapChunk minimapChunk = quest.GetTargetChunk();
            minimapChunk.ShowQuestFeedback();
            minimapChunk.SetDiscovered();
        }
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
                ContinueQuest();
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
			//Debug.LogError ("QUEST IS NULL : CheckIfQuestIsAccomplished");
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
			//Debug.LogError ("QUEST IS NULL : accomplish quest");
			return;
		}

        DisplayCombatResults.Instance.onConfirm += AccomplishQuest_HandleOnConfirm;
        DisplayCombatResults.Instance.Display("Quest Fulfilled!", "Go back to " + quest.giver.Name + " to receive gold and experience...");

        quest.accomplished = true;

        quest.updated = true;

        quest.SetTargetIsland(quest.GetOriginIslandData());

        //Story_ShowQuestOnMap();

	}
    void AccomplishQuest_HandleOnConfirm()
    {
        DisplayCombatResults.Instance.onConfirm -= AccomplishQuest_HandleOnConfirm;

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

        Quest.currentQuest.updated = true;

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
		if (!quest.accomplished) {
			string[] strs = new string[5]
			{
				"I've told you I have a problem, but you gave up on me!",
				"You said you would help me but you gave up?! What am I going to do now?",
				"I was really counting on you but you just thought it wasn’t worth it, huh? How disappointing.",
				"I see, so you decided not to help me after all. I get it, we all have our own problems.",
				"You can’t give up like that! I wish I could do it myself but I’m stuck here.Ma’s right, I’m useless!"
			};

			string str = strs[UnityEngine.Random.Range(0, strs.Length)];

			DialogueManager.Instance.OtherSpeak_Story(str);
		}
		else
        {
			string[] strs = new string[5]
			{
				"I'll never thank you enough for helping me.",
				"Thank you so much for your help, we need more people like you!",
				"You’re a lifesaver, my friend. Thanks so much for helping me!",
				"You did it, amazing! I’m truly grateful for your aid!",
				"Great job, you have no idea how happy I am right now. Thanks!"
			};

			string str = strs[UnityEngine.Random.Range(0, strs.Length)];

			DialogueManager.Instance.OtherSpeak_Story(str);
		}
	}
    void HandleOnEndDialogue()
    {
        DialogueManager.Instance.onEndDialogue -= HandleOnEndDialogue;
        StoryReader.Instance.GoToNode(Coords_CheckForFinishedQuest.newQuest_FallbackNode);
    }
	#endregion

	void ContinueQuest () {

		Quest quest = currentQuests.Find (
            x =>
            x.GetTargetIslandData().coords == IslandManager.Instance.GetCurrentIslandData().coords
            &&
            x.GetTargetIslandData().index == IslandManager.Instance.GetCurrentIslandData().index
            );

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

	
	void FinishQuest ()
	{
		Quest quest = Quest.currentQuest;

        DisplayCombatResults.Instance.onConfirm += FinishQuest_HandleOnConfirm;
        DisplayCombatResults.Instance.Display("Quest Complete!", "In addition to receiving gold and experience, your karma also increased thanks to your good actions!");
        DisplayCombatResults.Instance.DisplayResults(quest.goldValue, quest.experience);
	}

    void FinishQuest_HandleOnConfirm()
    {
        DisplayCombatResults.Instance.onConfirm -= FinishQuest_HandleOnConfirm;

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

        quest.updated = true;

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

        quest.updated = true;

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
		//Debug.LogError ("SEND PLAYER BACK TO GIVER DOESNT EXIST ANYMORE");
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
