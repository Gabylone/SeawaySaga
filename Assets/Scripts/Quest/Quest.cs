using UnityEngine;

using System.Collections;

[System.Serializable]
public class Quest {

	public static Quest currentQuest;

	public enum QuestState {
		Started,
		Returning,
		Finished
	}

	public int storyID = 0;

	public int goldValue = 0;

	public int experience = 0;

	public int level = 0;

	public int layer = 0;
	public int row = 0;
	public int col = 0;

    private IslandData targetIslandData;
    private IslandData originIslandData;

	public Node nodeWhenCompleted;
	public Node newQuest_FallbackNode;
	public Node checkQuest_FallbackNode;

	public Member giver;

	public bool accomplished = false;

    public delegate void OnSetTargetCoords(Quest quest);
    public static OnSetTargetCoords onSetTargetCoords;

    public Quest () {
		//
	}
	public void StartStory () 
	{
		// ID
		layer = StoryReader.Instance.currentStoryLayer;
		row = StoryReader.Instance.Col;
		col = StoryReader.Instance.Row;

		GetNewQuestnode ();
		currentQuest = this;

		if ( QuestManager.Instance.launchClueQuest )
        {
            storyID = StoryLoader.Instance.Quests.Find(x => x.dataName == "Clue Quest").id;
            QuestManager.Instance.launchClueQuest = false;
            Debug.Log("la quête de l'indice devrait se lancer là");
        }
        else
        {
            storyID = StoryLoader.Instance.getStoryIndexFromPercentage(StoryType.Quest);
        }

        Node targetNode = Story.GetNode ("debut");

		StoryReader.Instance.SetNewStory (Story, StoryType.Quest, targetNode, newQuest_FallbackNode);

	}

	public void Init ()
	{
		goldValue = level * 20 + Random.Range(1,9);

		level = Random.Range(Crews.playerCrew.captain.Level -1, Crews.playerCrew.captain.Level+4);
		level = Mathf.Clamp (level, 1, 10);

		experience = 15;
        
        SetOriginIsland(IslandManager.Instance.GetCurrentIslandData());

		giver = Crews.enemyCrew.captain.MemberID;

		SetRandomCoords ();

	}

	public void ReturnToGiver() {
		currentQuest = this;
		StoryReader.Instance.SetNewStory (Story, StoryType.Quest, Story.GetNode("fin") , newQuest_FallbackNode);
	}

	public void Continue ()
	{
		string nodeText = StoryFunctions.Instance.CellParams;

		nodeText = nodeText.Remove (0, 2);

		checkQuest_FallbackNode = StoryReader.Instance.GetNodeFromText ( nodeText );

		currentQuest = this;
		StoryReader.Instance.SetNewStory (Story, StoryType.Quest, Story.GetNode("suite"), checkQuest_FallbackNode);

	}

	#region map & coords
	public void ShowOnMap ()
	{
        DisplayMinimap.Instance.ShowQuest(this);
    }

	public void SetRandomCoords () {

		SetTargetIsland (IslandManager.Instance.GetRandomIslandDataForQuest());

        //Debug.Log("target island : " + targetIslandData.storyManager.storyHandlers[0].Story.displayName);
	}

    public IslandData GetTargetIslandData()
    {
        return targetIslandData;
    }

    public void SetTargetIsland (IslandData islandData)
    {
        // disable quest feedback on island if there's already a target island
        if ( targetIslandData != null)
        {
            GetTargetChunk().HideQuestFeedback();
        }

        targetIslandData = islandData;
    }

    public IslandData GetOriginIslandData()
    {
        return originIslandData;
    }
    public void SetOriginIsland (IslandData islandData)
    {
        originIslandData = islandData;
    }
	#endregion

	#region nodes
	public void GetNewQuestnode () {

		string s = StoryFunctions.Instance.CellParams;

		s = s.Remove (0,2);

		string[] parts = s.Split (',');

		newQuest_FallbackNode = StoryReader.Instance.GetNodeFromText ( parts[0] );

		if ( parts.Length > 1 ) {
            nodeWhenCompleted = StoryReader.Instance.GetNodeFromText(parts[1]);
		}
	}
	#endregion

	public Story Story {
		get {
			return StoryLoader.Instance.Quests [storyID];
		}
	}

    public MinimapChunk GetOriginChunk()
    {
        return DisplayMinimap.Instance.GetMinimapChunk(originIslandData);
    }

    public MinimapChunk GetTargetChunk()
    {
        return DisplayMinimap.Instance.GetMinimapChunk(targetIslandData);
    }
	//
}