using UnityEngine;
using System.Collections;

public class Crews : MonoBehaviour {

    /// <summary>
    /// jours avant que le membre ait faim
    /// </summary>
//	public static int maxHunger = 35; // pas assez
	//public static int maxHunger = 10; // trop
	public static int maxHunger = 5;

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

		Map,
		MemberCreation,
		Inventory,
		World,
		Hidden,

		None

	}

	public static CrewManager[] crews = new CrewManager[2];

	[Range(1,4)]
	public int startMemberAmount = 1;
	[Range(1,10)]
	public int startLevel = 1;

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

		InGameMenu.Instance.onCloseMenu+= HandleCloseInventory;

		Canvas.ForceUpdateCanvases ();

	}

	void HandleCloseInventory ()
	{
		if (StoryLauncher.Instance.PlayingStory) {

			if (CrewMember.GetSelectedMember != Crews.playerCrew.captain) {
				CrewMember.GetSelectedMember.Icon.MoveToPoint (Crews.PlacingType.Map);
			}

			Crews.getCrew (Crews.Side.Player).captain.Icon.MoveToPoint (Crews.PlacingType.World);

		} else {
			CrewMember.GetSelectedMember.Icon.MoveToPoint (Crews.PlacingType.Map);
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
		case FunctionType.HideOther:
			enemyCrew.UpdateCrew (PlacingType.Hidden);
			StoryReader.Instance.NextCell ();
			StoryReader.Instance.UpdateStory ();
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

//		Debug.Log ("loading player crew ?");

		playerCrew.managedCrew = SaveManager.Instance.GameData.playerCrew;

		crews [0].SetCrew (playerCrew.managedCrew);
	}
    #endregion

    #region crew tools
    public void CreateNewCrew () {

		StoryReader.Instance.NextCell ();

		Crew storyCrew = Crews.Instance.GetCrewFromCurrentCell ();

		// set decal
		if (storyCrew.MemberIDs.Count == 0) {

			StoryReader.Instance.SetDecal (1);

		} else {

			Crews.enemyCrew.SetCrew (storyCrew);

			if (storyCrew.hostile) {
				
				DialogueManager.Instance.SetDialogueTimed ("He's back, on guard !", Crews.enemyCrew.captain);
				StoryReader.Instance.SetDecal (2);
			
			} else if ( !QuestManager.Instance.metPersonOnIsland ) {

                QuestManager.Instance.metPersonOnIsland = true;

                Quest linkedQuest = QuestManager.Instance.currentQuests.Find (x => x.giver.SameAs(Crews.enemyCrew.captain.MemberID));
				if (linkedQuest != null) {
					linkedQuest.ReturnToGiver ();
				}

			}

			Crews.enemyCrew.captain.Icon.MoveToPoint (Crews.PlacingType.World);
		}


		StoryReader.Instance.Wait (Crews.playerCrew.captain.Icon.moveDuration);
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
			
			CrewParams crewParams = GetCrewFromText (StoryFunctions.Instance.CellParams);

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

			string phrase = "The boat is too small, I can't even put a foot in it";
			DialogueManager.Instance.SetDialogueTimed (phrase, Crews.enemyCrew.captain);

		} else {

			CrewMember targetMember = Crews.enemyCrew.captain;

			CrewCreator.Instance.TargetSide = Crews.Side.Player;
			CrewMember newMember = CrewCreator.Instance.NewMember (targetMember.MemberID);
			Crews.playerCrew.AddMember (newMember);
			Crews.enemyCrew.RemoveMember (targetMember);

			newMember.Icon.MoveToPoint (Crews.PlacingType.Map);

		}

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.Wait (0.8f);

	}
	
	public void RemoveMemberFromCrew () {
		int removeIndex = Random.Range (0,Crews.playerCrew.CrewMembers.Count);
		CrewMember memberToRemove = Crews.playerCrew.CrewMembers [removeIndex];

		memberToRemove.Kill ();

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.Wait (0.5f);
	}
	#endregion

	#region health
	private void AddHealth () {

		string cellParams = StoryFunctions.Instance.CellParams;
		int health = int.Parse ( cellParams );
		Crews.getCrew (Crews.Side.Player).captain.AddHealth (health);

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();
	}
	private void RemoveHealth () {

		string cellParams = StoryFunctions.Instance.CellParams;
		int health = int.Parse ( cellParams );
		Crews.getCrew (Crews.Side.Player).captain.RemoveHealth(health);

		if (Crews.getCrew (Crews.Side.Player).captain.Health <= 0)
			Crews.getCrew (Crews.Side.Player).captain.Kill ();

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();
	}
	#endregion
}
