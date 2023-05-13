using UnityEngine;
using System.Collections;

public class Crews : MonoBehaviour {

    /// <summary>
	public static int maxHunger_MinConstitution = 10;
	public static int maxHunger_MaxConstitution = 20;

    public static Crews Instance;

    public float reducedDamage = 50f;

    public delegate void OnCrewMemberKilled(CrewMember crewMember);
    public OnCrewMemberKilled onCrewMemberKilled;

	public enum Side {
		Player,
		Enemy,
	}

	public static Side otherSide (Side side) {
		return side == Side.Player ? Side.Enemy : Side.Player;
	}
			
	private Side[] sides = new Side[2] {Side.Player,Side.Enemy};
	public Side[] Sides {get {return sides;}}

	public enum PlacingType {

		Portraits,
		MemberCreation,
		Inventory,
		World,
		Hidden,

		None

	}

	public static CrewManager[] crews = new CrewManager[2];

    public bool firstFight = true;

	[Range(1,4)]
	public int startMemberAmount = 1;
	[Range(1,10)]
	public int startLevel = 1;
    public Job startJob;

	void Awake () {
		Instance = this;

        // empty things
        CrewMember.SetSelectedMember(null);
        CrewMember.onWrongLevel = null;
    }

    public void Init () {
		crews [0] = GetComponentsInChildren<CrewManager> () [0];
		crews [1] = GetComponentsInChildren<CrewManager> () [1];

		StoryFunctions.Instance.getFunction += HandleGetFunction;

		InGameMenu.Instance.onCloseMenu += HandleCloseInventory;

		Canvas.ForceUpdateCanvases ();

	}

    void HandleCloseInventory ()
	{
		if (StoryLauncher.Instance.PlayingStory) {

			//Crews.getCrew (Crews.Side.Player).captain.Icon.MoveToPoint (Crews.PlacingType.World);
            Crews.getCrew (Crews.Side.Player).UpdateCrew (Crews.PlacingType.World);

		} else {
			CrewMember.GetSelectedMember.Icon.MoveToPoint (Crews.PlacingType.Portraits);
		}
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.NewCrew:
			CreateNewCrew ();
			break;
		case FunctionType.AddMember:
			AddMemberToCrew ();
			break;
		case FunctionType.RemoveMember:
			RemoveMemberFromCrew ();
			break;
		case FunctionType.AddHealth:
			AddHealth ();
			break;
		case FunctionType.RemoveHealth:
			RemoveHealth();
			break;
        case FunctionType.HidePlayer:
            playerCrew.UpdateCrew(PlacingType.Hidden);
            StoryReader.Instance.NextCell();
            StoryReader.Instance.UpdateStory();
            break;
            case FunctionType.HideOther:
			enemyCrew.UpdateCrew (PlacingType.Hidden);
			StoryReader.Instance.NextCell ();
			StoryReader.Instance.UpdateStory ();
			break;
            case FunctionType.HasRoomOnBoat:

                StoryReader.Instance.NextCell();

                if (Crews.playerCrew.CrewMembers.Count >= Crews.playerCrew.CurrentMemberCapacity)
                {
                    StoryReader.Instance.SetDecal(1);
                }

                StoryReader.Instance.UpdateStory();

                break;

        }
	}

	#region get crews
	public static CrewManager getCrew ( Crews.Side targetSide ) {
		return crews [(int)targetSide];
	}

	public static CrewManager playerCrew {
		get {
			return crews[0];
		}
	}

	public static CrewManager enemyCrew {
		get {
			return crews[1];
		}
	}
	#endregion

	#region save / load crews
	public void SavePlayerCrew () {
		SaveManager.Instance.GameData.playerCrew = playerCrew.managedCrew;
	}
	public void RandomizePlayerCrew () {

		CrewParams crewParams = new CrewParams ();
		crewParams.amount = startMemberAmount;
		crewParams.overideGenre = false;
		crewParams.male = false;
		crewParams.level = startLevel;

		Crew playerCrew = new Crew (crewParams,0,0);
		Canvas.ForceUpdateCanvases ();
		crews [0].SetCrew (playerCrew);
	}
	public void LoadPlayerCrew () {

//		//Debug.Log ("loading player crew ?");

		playerCrew.managedCrew = SaveManager.Instance.GameData.playerCrew;

		crews [0].SetCrew (playerCrew.managedCrew);
	}
    #endregion

    #region crew tools
    public void CreateNewCrew () {

        Crew storyCrew = Crews.Instance.GetCrewFromCurrentCell();

        Crews.enemyCrew.SetCrew(storyCrew);
        Crews.enemyCrew.UpdateCrew(Crews.PlacingType.World);

        StoryReader.Instance.NextCell();

		// set decal
		if (storyCrew.MemberIDs.Count == 0 ) {

			StoryReader.Instance.SetDecal (1);

            ////Debug.Log("!!! OPENING COMBAT LOOT !!!");

            if (storyCrew.hostile)
            {
                Loot loot = LootManager.Instance.GetIslandLoot(1, /* fighting loot */ true);

                if (!loot.IsEmpty())
                {
                    LootUI.Instance.preventAdvanceStory = true;

					string[] strs = new string[5]
					{
					"We fought here! *Looks like we forgot to pick some of their stuff... *After the fight...",
					"We fought here! *Looks like there’s still something to grab.",
					"I remember this place, we had a good fight. Seems like there’s still something of use over there.",
					"There’s some loot left it seems! Might have missed that after we fought.",
					"Wait, I remember fighting here! There’s even some remaining loot it seems."
					};

					string str = strs[Random.Range(0, strs.Length)];

					DialogueManager.Instance.PlayerSpeak_Story(str);
                    
					DialogueManager.Instance.onEndDialogue += HandleOnEndDialogue;
                    return;
                }
            }

            StoryReader.Instance.UpdateStory();

        } else {


			DialogueManager.Instance.PlayRandomVoice(Crews.enemyCrew.captain, DialogueManager.Voice.Type.Greetings);

			if (storyCrew.hostile) {
				
				//DialogueManager.Instance.SetDialogueTimed ("He's back, on guard !", Crews.enemyCrew.captain);
				StoryReader.Instance.SetDecal (2);
			
			} else if ( !QuestManager.Instance.metPersonOnIsland ) {

                QuestManager.Instance.metPersonOnIsland = true;

                Quest linkedQuest = QuestManager.Instance.currentQuests.Find (x => x.giver.SameAs(Crews.enemyCrew.captain.MemberID));
				if (linkedQuest != null) {
					linkedQuest.ReturnToGiver ();
                    return;
				}

			}


            StoryReader.Instance.UpdateStory();
        }

	}

    void HandleOnEndDialogue()
    {
        OtherInventory.Instance.StartLooting(true);

        DialogueManager.Instance.onEndDialogue -= HandleOnEndDialogue;
    }

    public void KillMember (CrewMember crewMember)
    {
        if (onCrewMemberKilled != null)
        {
            onCrewMemberKilled(crewMember);
        }
    }

	public Crew GetCrewFromCurrentCell () {

		int row = StoryReader.Instance.Row;
		int col = StoryReader.Instance.Col;

		var tmp = StoryReader.Instance.CurrentStoryHandler.GetCrew (row, col);

		if (tmp == null) {
			
			CrewParams crewParams = GetCrewFromText (StoryFunctions.Instance.cellParams);

			Crew newCrew = new Crew (crewParams, row, col);

			StoryReader.Instance.CurrentStoryHandler.SetCrew (newCrew);

			return newCrew;

		}


		return tmp;

	}

	public CrewParams GetCrewFromText (string text) {

		CrewParams crewParams = new CrewParams ();

		string[] parms = text.Split ('/');

			// crew amount
		if ( parms.Length > 0 ) {

			int parmAmount = 0;
			bool parsable = int.TryParse(parms[0],out parmAmount);

			crewParams.amount = parmAmount;
		}

			// genre
		if ( parms.Length > 1 ) {
			
			crewParams.overideGenre = true;

			if ( parms[1][0] == 'M')
            {
				crewParams.male = true;
            }
            else if (parms[1][0] == 'F')
            {
                crewParams.male = false;
            }
            else
            {
                crewParams.male = Random.value > 0.5f;
            }

        }

        // get crew level
		if ( parms.Length > 2 ) {

            int level = 0;

            if ( parms[2] == "TREASURE")
            {
                level = MapGenerator.mapParameters.endFightLevel;
				crewParams.zombie = true;
            }
            else
            {
                level = int.Parse(parms[2]);
            }

            crewParams.level = level;

		}

		return crewParams;

	}

	public void AddMemberToCrew () {

		if (Crews.playerCrew.CrewMembers.Count >= Crews.playerCrew.CurrentMemberCapacity) {

			string[] strs = new string[3]
			{
				"The boat is too small, I can't even put a foot in it!",
				"There’s not enough room on this boat, it’s ridiculous!",
				"There’s no way we can all fit on the boat, it’s just too cramped!",
			};

			string str = strs[Random.Range(0, strs.Length)];

			DialogueManager.Instance.OtherSpeak_Story(str);

		} else {

			CrewMember targetMember = Crews.enemyCrew.captain;

			CrewCreator.Instance.TargetSide = Crews.Side.Player;

            CrewMember newMember = CrewCreator.Instance.NewMember (targetMember.MemberID);

            Crews.playerCrew.AddMember(newMember);

            string title = newMember.MemberName + " joins the crew!";

            string content = "NOMBATEAU welcomes a new crew member. Under CAPITAINE's orders, " + newMember.MemberName + " will fight, sail and explore. Keep an eye on the food cellar as a new helpful member is also another mouth to feed!";

            DisplayCombatResults.Instance.Display(title, content);
            DisplayCombatResults.Instance.onConfirm += HandleOnConfirm;

			Crews.enemyCrew.RemoveMember (targetMember);

			newMember.Icon.MoveToPoint (Crews.PlacingType.Portraits);

		}

	}

    void HandleOnConfirm()
    {
        DisplayCombatResults.Instance.onConfirm -= HandleOnConfirm;

        StoryReader.Instance.ContinueStory();
    }

    public void RemoveMemberFromCrew () {
		int removeIndex = Random.Range (0,Crews.playerCrew.CrewMembers.Count);
		CrewMember memberToRemove = Crews.playerCrew.CrewMembers [removeIndex];

		memberToRemove.Kill ();

		StoryReader.Instance.Wait (0.5f);
	}
	#endregion

	#region health
	private void AddHealth () {

		string cellParams = StoryFunctions.Instance.cellParams;
		int health = int.Parse ( cellParams );

        foreach (var item in Crews.getCrew(Crews.Side.Player).CrewMembers)
        {
            item.AddHealth(health);
            item.Icon.hungerIcon.DisplayHealthAmount(health);
        }

        StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();
	}
	private void RemoveHealth () {

		string cellParams = StoryFunctions.Instance.cellParams;
		int health = int.Parse ( cellParams );

        for (int i = Crews.getCrew(Crews.Side.Player).CrewMembers.Count-1; i >= 0; i--)
        {
            CrewMember item = Crews.getCrew(Crews.Side.Player).CrewMembers[i];

            item.RemoveHealth(health);
            item.Icon.hungerIcon.DisplayHealthAmount(-health);

            if (item.Health <= 0)
                item.Kill();
        }

        if (Crews.playerCrew.CrewMembers.Count == 0)
        {
            MessageDisplay.Instance.onValidate += HandleOnValidate;
            MessageDisplay.Instance.Display("The crew members of NOMBATEAU have all perished.");
            GameManager.Instance.BackToMenu();
        }

        StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();
	}
    void HandleOnValidate()
    {

    }
    #endregion
}
