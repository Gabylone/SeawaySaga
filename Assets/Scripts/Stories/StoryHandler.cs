using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StoryType {
	Normal,
	Treasure,
	Home,
	Clue,
	Boat,
	Quest
}

[System.Serializable]
public class StoryManager  {

	public List<StoryHandler> storyHandlers = new List<StoryHandler>();

	public StoryManager () {
		
	}

	public void AddStoryHandler (StoryHandler handler ) {
		storyHandlers.Add (handler);
	}

	public void InitHandler ( StoryType storyType , int storyID ) {
		
		storyHandlers.Clear ();

		StoryHandler handler = new StoryHandler (storyID,storyType);
		storyHandlers.Add (handler);

	}

	public void InitHandler ( StoryType storyType ) {

		int storyId = StoryLoader.Instance.getStoryIndexFromPercentage (storyType);

		InitHandler (storyType, storyId);
	}

	public StoryHandler CurrentStoryHandler {
		get {
			if ( StoryReader.Instance.currentStoryLayer >= storyHandlers.Count ) {
				Debug.LogError ("Le layer d'histoire est au dessus du nombre de story handlers");
				return storyHandlers [0];
			}
			return storyHandlers [StoryReader.Instance.currentStoryLayer];
		}
	}
}

[System.Serializable]
public class StoryHandler {

	public int row = 0;
	public int col = 0;

		// serialisation
	public int fallBackLayer = 0;

	public Node 				fallbackNode;

	public int 					storyID 			= 0;
	public StoryType 			storyType;
	public List<contentDecal> 	contentDecals 		= new List<contentDecal>();
	public List<Loot> 			loots 				= new List<Loot> ();
	public List<Crew> 			crews 				= new List<Crew>();
		//

	public StoryHandler () {
		//
	}

	public StoryHandler (int _storyID,StoryType _storyType) {
		storyID = _storyID;
		storyType = _storyType;
	}

	public Story Story {
		get {
			switch (storyType) {
			case StoryType.Normal:
				return StoryLoader.Instance.IslandStories[storyID];
			case StoryType.Treasure:
				return StoryLoader.Instance.TreasureStories[storyID];
			case StoryType.Home:
				return StoryLoader.Instance.HomeStories[storyID];
			case StoryType.Clue:
				return StoryLoader.Instance.ClueStories[storyID];
			case StoryType.Boat:
				return StoryLoader.Instance.BoatStories[storyID];
			case StoryType.Quest:
				return StoryLoader.Instance.Quests[storyID];
			default:
				return StoryLoader.Instance.IslandStories[storyID];
			}

		}
	}

	public Loot GetLoot ( int x , int y ) {
		return loots.Find (loot => (loot.row == x) && (loot.col== y) );
	}

	public void SetLoot (Loot targetLoot) {
		loots.Add (targetLoot);
	}

	public Crew GetCrew ( int x , int y ) {
		return crews.Find (crew => (crew.row == x) && (crew.col== y) );
	}

	public void SetCrew (Crew targetCrew) {
		crews.Add (targetCrew);
		if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.boat)
			Debug.Log ("adding crew to BOAT story: " + targetCrew.MemberIDs [0].Name);
	
	}

	public void SaveDecal (int decal) {

		SaveDecal (decal, StoryReader.Instance.Row, StoryReader.Instance.Col);

	}

	public void SaveDecal (int _decal , int _row , int _col ) {
		contentDecals.Add (new contentDecal(_row, _col , _decal) );
	}

	public int GetDecal() {
		contentDecal cDecal = contentDecals.Find ((contentDecal obj) => (obj.row == StoryReader.Instance.Row) && (obj.col == StoryReader.Instance.Col) );

		if (cDecal == default(contentDecal)) {
			return -1;
		}

		return cDecal.decal;
	}
}

[System.Serializable]
public struct contentDecal {
	
	public int row;

	public int col;

	public int decal;

	public contentDecal (int x,int y, int decal) {
		this.row 		= x;
		this.col 		= y;
		this.decal 	= decal;
	}

	public static bool operator ==( contentDecal cd1, contentDecal cd2) 
	{
		return cd1.row == cd2.row && cd1.col == cd2.col && cd1.decal == cd2.decal;
	}
	public static bool operator != (contentDecal cd1 , contentDecal cd2) 
	{
		return !(cd1==cd2);
	}

}
