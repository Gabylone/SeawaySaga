using UnityEngine;

[System.Serializable]
public class Quest {

	public static Quest currentQuest;

	public enum QuestState {
		Started,
		Returning,
		Finished
	}

	public int questID = 0;

	public int goldValue = 0;

	public int experience = 0;

	public int level = 0;

	public int layer = 0;
	public int row = 0;
	public int col = 0;


    public int originID = 0;
    public Coords originCoords;

    public int previousID = 0;
    public Coords previousCoords;

    public int targetID = 0;
	public Coords targetCoords;

	public Node nodeWhenCompleted;
	public Node newQuest_FallbackNode;
	public Node checkQuest_FallbackNode;

	public Member giver;

	public bool accomplished = false;

	public delegate void ShowQuestOnMap (Quest quest);
	public static ShowQuestOnMap showQuestOnMap;

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

		questID = StoryLoader.Instance.getStoryIndexFromPercentage (StoryType.Quest);

		Node targetNode = Story.GetNode ("debut");

		StoryReader.Instance.SetNewStory (Story, StoryType.Quest, targetNode, newQuest_FallbackNode);

	}

	public void Init ()
	{
		goldValue = level * 20 + Random.Range(1,9);

		level = Random.Range(Crews.playerCrew.captain.Level -1, Crews.playerCrew.captain.Level+4);
		level = Mathf.Clamp (level, 1, 10);

		experience = 15;

        originCoords = Boats.Instance.playerBoatInfo.coords;
        originID = IslandManager.Instance.currentIsland.id;

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
		if (showQuestOnMap != null)
			showQuestOnMap (this);
	}

	public void SetRandomCoords () {
		Coords _targetCoords = Coords.GetClosest (Boats.Instance.playerBoatInfo.coords);
		SetTargetCoords (_targetCoords);
	}
	#endregion

	#region nodes
	public void GetNewQuestnode () {

		string s = StoryFunctions.Instance.CellParams;

		s = s.Remove (0,2);

		string[] parts = s.Split (',');

		newQuest_FallbackNode = StoryReader.Instance.GetNodeFromText ( parts[0] );

		if ( parts.Length > 1 ) {
			nodeWhenCompleted = StoryReader.Instance.GetNodeFromText (parts [1]);
		}
	}
	#endregion

	public Story Story {
		get {
			return StoryLoader.Instance.Quests [questID];
		}
	}

	public delegate void OnSetTargetCoords ( Quest quest );
	public static OnSetTargetCoords onSetTargetCoords;

	public void SetTargetCoords ( Coords coords ) {

		previousCoords = targetCoords;
		targetCoords = coords;

		if ( onSetTargetCoords != null ) {
			onSetTargetCoords (this);
		}
	}
	//
}