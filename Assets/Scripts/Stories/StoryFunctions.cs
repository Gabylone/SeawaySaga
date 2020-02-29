using UnityEngine;
using System.Collections;

public enum FunctionType {

	Leave,
	ChangeStory,
	Fade,
	CheckFirstVisit,
	RandomPercent,
	RandomRange,
	RandomRedoPercent,
	RandomRedoRange,
	NewCrew,
	ShowPlayer,
	ShowOther,
	HidePlayer,
	HideOther,
	AddMember,
	RemoveMember,
	Narrator,
	SetChoices,
	PlayerSpeak,
	OtherSpeak,
	GiveTip,
	CheckGold,
	RemoveGold,
	AddGold,
	AddToInventory,
	RemoveFromInventory,
	CheckInInventory,
	Loot,
	Trade,
	BoatUpgrades,
	LaunchCombat,
	CheckClues,
	SetWeather,
	ChangeTimeOfDay,
	CheckDay,
	Node,
	Switch,
	CheckStat,
	ThrowDice,
	AddHealth,
	RemoveHealth,
	AddKarma,
	RemoveKarma,
	CheckKarma,
	PayBounty,
	ObserveHorizon,
    EndMap,

	// quest
	NewQuest,
	CheckQuest,
	SendPlayerBackToGiver,
	FinishQuest,
	ShowQuestOnMap,
	AccomplishQuest,
	IsQuestAccomplished,
	GiveUpQuest,
	AddCurrentQuest,

	// ship
	DestroyShip

}

public class StoryFunctions : MonoBehaviour {

	public static StoryFunctions Instance;

	string cellParams = "";

	public delegate void GetFunction (FunctionType func, string cellParameters );
	public GetFunction getFunction;

	public string CellParams {
		get {
			return cellParams;
		}
	}

	void Awake () {
		Instance = this;
	}

	public void Read ( string content ) {

		if (content.Length == 0) {
			
			string text = "cell is empty on story " + StoryReader.Instance.CurrentStoryHandler.Story.name + "" +
				"\n at row : " + (StoryReader.Instance.Col+2) + "" +
				"\n and collumn : " + StoryReader.Instance.Row;

			Debug.LogError (text);

			StoryLauncher.Instance.EndStory ();
			return;
		}

//		// GET DECAL
		int decal = StoryReader.Instance.CurrentStoryHandler.GetDecal();
		if ( decal >= 0 ) {

			StoryReader.Instance.NextCell ();
			StoryReader.Instance.SetDecal (decal);

			StoryReader.Instance.UpdateStory ();
			//
			return;
		}
	
		if ( content[0] == '[' ) {
			
			StoryReader.Instance.NextCell ();
			StoryReader.Instance.UpdateStory ();
			return;
		}

		foreach ( FunctionType func in System.Enum.GetValues(typeof(FunctionType)) ) {

			if ( content.Contains (func.ToString()) ){

				cellParams = content.Remove (0, func.ToString().Length);

				if (getFunction != null)
					getFunction (func,cellParams);

				return;
			}

		}

		Debug.LogError (
			"cell returns no function at decal\n" + StoryReader.Instance.Row + "\n" +
			"index : " + StoryReader.Instance.Col + "\n" +
			"qui contient : " + content);

		StoryLauncher.Instance.EndStory ();

	}

}
