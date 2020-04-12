using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boats : MonoBehaviour {

	public static Boats Instance;

	public static PlayerBoatInfo playerBoatInfo;

	private BoatData boatData;

    public GameObject enemyBoatGroup;
    public EnemyBoat currentEnemyBoat;
    public int currentBoatAmount = 0;
    public EnemyBoat[] enemyBoats;

    public bool meetingPlayer = false;

    public delegate void OnMeetPlayer();
    public OnMeetPlayer onMeetPlayer;

    public delegate void OnLeavePLayer();
    public OnLeavePLayer onLeavePlayer;

	public List<OtherBoatInfo> getBoats {
		//= new List<OtherBoatInfo> ();
		get {
			return boatData.boats;
		}
	}


	[Header("Movement")]
	[SerializeField]
	private float chanceOfMoving = 0.5f;

	[SerializeField]
	private float timeToMove = 20f;

	private float timer = 0f;

	void Awake () {

        playerBoatInfo = null;

		Instance = this;
	}

	void Start () {

        enemyBoats = enemyBoatGroup.GetComponentsInChildren<EnemyBoat>(true );

        Karma.onChangeKarma += HandleOnChangeKarma;

		NavigationManager.Instance.EnterNewChunk += HandleOnChunkEvent;

		StoryFunctions.Instance.getFunction += HandleGetFunction;
	}

    public void RandomizeBoats( ) {

		playerBoatInfo = new PlayerBoatInfo ();
		playerBoatInfo.Init ();
		playerBoatInfo.Randomize ();

		boatData = new BoatData ();
		boatData.boats = new List<OtherBoatInfo> ();

		for (int i = 0; i < MapGenerator.mapParameters.boatAmount; i++) {
            OtherBoatInfo newBoat = CreateNewBoat();
        }
    }

	#region story

    public void MeetPlayer()
    {
        meetingPlayer = true;

        if (onMeetPlayer != null)
        {
            onMeetPlayer();
        }
    }

    public void LeaveOtherBoat()
    {
        meetingPlayer = false;

        if (onLeavePlayer != null)
        {
            onLeavePlayer();
        }
    }

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		if ( func == FunctionType.DestroyShip ) {
            DestroyCurrentShip();
		}
	}

    void HandleOnChunkEvent()
    {
        foreach (var enemyBoat in enemyBoats)
        {
            enemyBoat.Hide();
        }

        currentBoatAmount = 0;

        foreach (var item in boatData.boats)
        {
            item.HandleChunkEvent();

            if (item.coords == Boats.playerBoatInfo.coords)
            {
                EnemyBoat enemyBoat = enemyBoats[currentBoatAmount];

                enemyBoat.id = currentBoatAmount;

                enemyBoat.Show(item);

                ++currentBoatAmount;

                if (currentBoatAmount == enemyBoats.Length)
                {
                    Debug.LogError( "reached max boat amout on screen" );
                    break;
                }

            }
        }

        SaveBoats();
    }

    void DestroyCurrentShip()
    {
        boatData.boats.Remove(currentEnemyBoat.boatInfo);

        currentEnemyBoat.Hide();

        Debug.Log("destroying boat");

        DisplayMinimap.Instance.UpdateOtherBoatsMinimapIcon();

        StoryReader.Instance.NextCell();
        StoryReader.Instance.UpdateStory();

        Debug.Log("destroying ship : boat manager");
    }
	#endregion

	#region karma
	void HandleOnChangeKarma (int previousKarma, int newKarma)
	{
		if (previousKarma > newKarma) {
			if (Karma.Instance.CurrentKarma > -Karma.Instance.maxKarma) {
				AddImperialBoat ();
			}
		} else {
			if (Karma.Instance.CurrentKarma < 0) {
				RemoveImperialBoat ();
			}
		}
	}

	void AddImperialBoat ()
	{
        OtherBoatInfo newBoat = CreateNewBoat();

		int imperialID = StoryLoader.Instance.FindIndexByName ("Impériaux",StoryType.Boat);

		newBoat.storyManager.storyHandlers[0].storyID = imperialID;
	}

    public OtherBoatInfo CreateNewBoat()
    {
        OtherBoatInfo newBoat = new OtherBoatInfo();
        newBoat.Init();
        newBoat.Randomize();

        newBoat.id = boatData.boats.Count;

        boatData.boats.Add(newBoat);

        return newBoat;
    }

	void RemoveImperialBoat ()
	{
		boatData.boats.RemoveAt(getBoats.Count-1);
	}
	#endregion

	#region save & load
	public void SaveBoats () {

		SaveManager.Instance.GameData.playerBoatInfo = playerBoatInfo;

		SaveTool.Instance.SaveToCurrentMap ("boat data", boatData);

	}

	public void LoadBoats () {
		
		playerBoatInfo = SaveManager.Instance.GameData.playerBoatInfo;
		playerBoatInfo.Init ();

		boatData = SaveTool.Instance.LoadFromCurrentMap ("boat data.xml", "BoatData") as BoatData;
		foreach (var item in getBoats) {
            item.Init();
		}
	}
	#endregion
}

public class BoatData {
	
	public List<OtherBoatInfo>	boats;

	public BoatData() {
		//
	}

	public BoatData ( List<OtherBoatInfo> _boats ) {
		this.boats = _boats;
	}
}
