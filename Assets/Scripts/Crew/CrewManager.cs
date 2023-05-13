using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CrewManager : MonoBehaviour {

		// managed crew
	public Crew managedCrew;

	List<CrewMember> crewMembers = new List<CrewMember> ();

	[SerializeField] private Crews.Side side;

    [SerializeField] private Transform[] crewAnchors;

    public Transform[] inventoryAnchors;
    public Transform[] worldAnchord;

    public int CurrentMemberCapacity {
		get {
			return Boats.Instance.playerBoatInfo.crewCapacity;
		}
		set {
			Boats.Instance.playerBoatInfo.crewCapacity = value;
		}
	}

	public int maxMemberCapacity = 4;

	private bool placingCrew = false;

	[SerializeField]
	private float placingDuration = 0.5f;

	void Start () {
		if (side == Crews.Side.Player) {
			StoryFunctions.Instance.getFunction += HandleGetFunction;
		}
		CombatManager.Instance.onFightStart += HandleFightStarting;

        //UpdateCrew(Crews.PlacingType.Map);
	}

	void HandleFightStarting ()
	{
		foreach (var item in crewMembers) {
			item.ResetSkills ();
		}
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		if (func == FunctionType.ChangeTimeOfDay)
			InvokeAddToStates ();

	}

	public void InvokeAddToStates ()
	{
        CancelInvoke("AddToStates");
        Invoke("AddToStates", 1f);
	}

    public void AddToStates()
    {
        for (int i = CrewMembers.Count-1; i >= 0; i--)
        {
            CrewMembers[i].AddHunger();
        }

        Invoke("CheckRain", 2f);
    }

    void CheckRain()
    {
        if (TimeManager.Instance.raining)
        {
            for (int i = CrewMembers.Count - 1; i >= 0; i--)
            {
                CrewMembers[i].RemoveHealth(10);
                CrewMembers[i].Icon.hungerIcon.DisplayHealthAmount(-10, true);
            }

            SoundManager.Instance.PlaySound("Tribal 01");

            if (Crews.playerCrew.CrewMembers.Count == 0)
            {
                MessageDisplay.Instance.Display("All of your crew members died in the storm.");
                MessageDisplay.Instance.onValidate += HandleOnValidate;
            }

        }


        
    }

    void HandleOnValidate()
    {
            MessageDisplay.Instance.onValidate -= HandleOnValidate;
        GameManager.Instance.BackToMenu();
    }

    #region crew placement
    public void UpdateCrew ( Crews.PlacingType placingType ) {

        for (int i = 0; i < crewMembers.Count; i++)
        {
            CrewMember crewMember = crewMembers[i];
            crewMember.Icon.indexInList = i;
			crewMember.Icon.MoveToPoint (placingType);
		}

        if ( side == Crews.Side.Player)
        {
            switch (placingType)
            {
                case Crews.PlacingType.Portraits:
                case Crews.PlacingType.Inventory:
                    PlayerIcons.Instance.Show();
                    break;
                case Crews.PlacingType.MemberCreation:
                    break;
                case Crews.PlacingType.World:
                    PlayerIcons.Instance.Hide();
                    break;
                case Crews.PlacingType.Hidden:
                    break;
                case Crews.PlacingType.None:
                    break;
                default:
                    break;
            }
        }
	}
	public float PlacingDuration {
		get {
			return placingDuration;
		}
		set {
			placingDuration = value;
		}
	}

	public Transform[] CrewAnchors {
		get {
			return crewAnchors;
		}
	}
	#endregion

	#region crew list
	public void AddMember ( CrewMember member )
        {

            if (crewMembers.Count == CurrentMemberCapacity)
			return;

		managedCrew.Add (member.MemberID);
		crewMembers.Add (member);

        PlayerIcons.Instance.Show();
	}

    public void RemoveMember(int i)
    {
        RemoveMember(CrewMembers[i]);
    }

    public void RemoveMember ( CrewMember member ) {

		if ( member.Icon != null )
			Destroy (member.Icon.gameObject);

		managedCrew.Remove (member.MemberID);
		crewMembers.Remove (member);

        PlayerIcons.Instance.Show();
	}

	public List<CrewMember> CrewMembers {
		get {
			return crewMembers;
		}
	}
	public CrewMember captain {
		get {
			return crewMembers [0];
		}
	}

	public void DeleteCrew () {

		foreach ( CrewMember member in CrewMembers )
			Destroy (member.Icon.gameObject);

		crewMembers.Clear ();
	}

	#endregion

	#region creation
	public void SetCrew (Crew crew) {


		DeleteCrew ();

		CrewCreator.Instance.TargetSide = side;

		for (int memberIndex = 0; memberIndex < crew.MemberIDs.Count; ++memberIndex ) {

			Member memberID = crew.MemberIDs [memberIndex];

			CrewMember member = CrewCreator.Instance.NewMember (memberID);
			CrewMembers.Add (member);

		}

		managedCrew = crew;

        UpdateCrew(Crews.PlacingType.Portraits);
	}
	#endregion
}
