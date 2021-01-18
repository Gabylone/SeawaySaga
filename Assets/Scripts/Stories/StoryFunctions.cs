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
    TellClue,
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
    GoToNextDay,
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
    SetBG,
    HasRoomOnBoat,

	// quest
	NewQuest,
    NewClueQuest,
	CheckQuest,
    CheckIfFormulaIsland,
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

    public bool debug = false;

	public string cellParams = "";

	public delegate void GetFunction (FunctionType func, string cellParameters );
	public GetFunction getFunction;

	void Awake () {
		Instance = this;
	}

	public void Read ( string content ) {

        if (debug)
        {
            Debug.Log("reading cell");
        }

		if (content.Length == 0) {
			
			string text = "cell is empty on story " + StoryReader.Instance.CurrentStoryHandler.Story.dataName + "" +
				"\n at row : " + (StoryReader.Instance.Col+2) + "" +
				"\n and collumn : " + StoryReader.Instance.Row;

			Debug.LogError (text);

			StoryLauncher.Instance.EndStory ();
			return;
		}

//		// GET DECAL
		if ( content[0] == '[' ) {

            if ( debug)
            {
                Debug.Log("going through node " + content);
            }

            int decal = StoryReader.Instance.CurrentStoryHandler.GetDecal();
            if (decal >= 0)
            {
                if (debug)
                {
                    Debug.Log("switching : decal " + decal);
                }

                StoryReader.Instance.NextCell();
                StoryReader.Instance.SetDecal(decal);
                StoryReader.Instance.UpdateStory();
            }
            else
            {
                StoryReader.Instance.NextCell();
                StoryReader.Instance.UpdateStory();
            }

            //
            return;
        }


        foreach ( FunctionType func in System.Enum.GetValues(typeof(FunctionType)) ) {

			if ( content.StartsWith(func.ToString()) ){
			//if ( content.Contains(func.ToString()) ){

				cellParams = content.Remove (0, func.ToString().Length);

                if (debug)
                {
                    Debug.Log("func : " + func.ToString());
                }

                if (getFunction != null)
					getFunction (func,cellParams);

                return;
			}

		}

        Debug.LogError(
            "cell (" + content + ") returns no function (" + StoryCheck.GetCellName(StoryReader.Instance.Row, StoryReader.Instance.Col) + ")");

        StoryLauncher.Instance.EndStory ();

	}

}
