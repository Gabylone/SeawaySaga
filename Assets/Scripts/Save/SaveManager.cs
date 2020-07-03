using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class SaveManager : MonoBehaviour
{
	public static SaveManager Instance;

    public int loadLimit = 10;

    public float timeBetweenFrames = 0.2f;

	private GameData gameData;

	public GameData GameData {
		get {
			return gameData;
		}
	}

	void Awake () {
		Instance = this;
	}

	void Start () {
		
        /*if ( Directory.Exists(SaveTool.Instance.GetSaveFolderPath("PlayerInfo")))
        {
            Debug.Log("save file exists");

        }
        else
        {
            Debug.Log("save file does not exist");
        }*/

        if ( SaveTool.Instance.FileExists ("PlayerInfo" , "player info"))
        {
            PlayerInfo.Instance = SaveTool.Instance.LoadFromSpecificPath("PlayerInfo", "player info.xml", "PlayerInfo") as PlayerInfo;
        }
        else
        {
            PlayerInfo.Instance = new PlayerInfo();
        }

		gameData = new GameData ();

        if ( NavigationManager.Instance)
        {
            NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;
            Crews.Instance.onCrewMemberKilled += HandleOnCrewMemberKilled;
            StoryLauncher.Instance.onPlayStory += HandlePlayStoryEvent;
            StoryLauncher.Instance.onEndStory += HandleEndStoryEvent;
        }

    }
    
    void HandlePlayStoryEvent()
    {
        SaveCurrentIsland();
        SaveGameData();
    }

    void HandleEndStoryEvent()
    {
        SaveCurrentIsland();
        SaveGameData();
    }

    void HandleOnCrewMemberKilled (CrewMember crewMember)
	{
		SaveGameData ();
	}

	void HandleChunkEvent ()
	{
		SaveGameData ();
	}

	#region load game data
	public void LoadGame () {

		LoadGameData ();

		LoadAllIslands ();

        Boats.Instance.LoadBoats();
		
	}

	void LoadGameData () {

		// GAME DATA
		gameData = SaveTool.Instance.LoadFromCurrentMap ("game data.xml" , "GameData") as GameData;

		// player crew
		Crews.Instance.LoadPlayerCrew ();

		FormulaManager.Instance.LoadFormulas ();

		// player loot
		LootManager.Instance.SetLoot (Crews.Side.Player, gameData.playerLoot);

		QuestManager.Instance.currentQuests = gameData.currentQuests;
		QuestManager.Instance.finishedQuests = gameData.finishedQuests;

		Member.globalID = gameData.globalID;

		// gold
		GoldManager.Instance.LoadGold();

		Karma.Instance.LoadKarma ();

		TimeManager.Instance.Load ();

        MapGenerator.Instance.treasureName = gameData.treasureName;


	}
	#endregion

	#region save game data
	public void SaveGameData () {

		FormulaManager.Instance.SaveFormulas ();

		Crews.Instance.SavePlayerCrew ();

		gameData.playerLoot = LootManager.Instance.getLoot (Crews.Side.Player);

		gameData.currentQuests = QuestManager.Instance.currentQuests;
		gameData.finishedQuests = QuestManager.Instance.finishedQuests;

		gameData.globalID = Member.globalID;

		GameData.playerGold = GoldManager.Instance.goldAmount;

        gameData.treasureName = MapGenerator.Instance.treasureName;

		// karma
		Karma.Instance.SaveKarma ();

		TimeManager.Instance.Save ();

		SaveTool.Instance.SaveToCurrentMap ("game data",gameData);

		SaveTool.Instance.SaveToCurrentMap ("discovered coords", MapGenerator.Instance.discoveredCoords);

	}
	#endregion

	/// <summary>
	/// load
	/// </summary>
	#region Load island data
	public void LoadAllIslands () {

//		LoadAllIslandCoroutine ();
		StartCoroutine(LoadAllIslandCoroutine ());
	}

	IEnumerator LoadAllIslandCoroutine () {
//	void LoadAllIslandCoroutine () {

		MapGenerator.Instance.LoadMap ();

		string pathToFolder = SaveTool.Instance.GetSaveFolderPath() + "/Islands";

		var folder = new DirectoryInfo (pathToFolder);
		var files = folder.GetFiles ();

		LoadingScreen.Instance.StartLoading ("Chargement îles", (int)((float)files.Length/2f));

		int l = 0;

        Debug.Log("number of files : " + files.Length);

		foreach (var item in files) {

			string pathToFile = "Islands/"+item.Name;
			if (pathToFile [pathToFile.Length - 1] == 'a') {
				continue;
			}

			Chunk chunkToLoad = SaveTool.Instance.LoadFromCurrentMap (pathToFile,"Chunk") as Chunk;

			Coords chunkCoords = GetCoordsFromFile (item.Name.Remove ( item.Name.Length - 4 ));

			// attation aux choses qui se passent dans "set island data"
			Chunk.SetChunk(chunkCoords,chunkToLoad);

			yield return new WaitForEndOfFrame ();

			++l;
			LoadingScreen.Instance.Push (l);
		}

		yield return new WaitForEndOfFrame ();

		LoadingScreen.Instance.End ();

		DisplayMinimap.Instance.Init ();

		NavigationManager.Instance.ChangeChunk (Directions.None);
	}

	public void SaveAllIslands () {

        Debug.Log("saving all island...");

        StartCoroutine(SaveAllIslandsCoroutine ());
//		SaveAllIslandsCoroutine ();
	}
	IEnumerator SaveAllIslandsCoroutine () {
//	void SaveAllIslandsCoroutine () {

//		LoadingScreen.Instance.StartLoading ("Sauvegarde îles", MapGenerator.Instance.MapScale * MapGenerator.Instance.MapScale);
		LoadingScreen.Instance.StartLoading ("Sauvegarde îles", MapGenerator.Instance.MapScale_X * MapGenerator.Instance.IslandsPerCol);

		yield return new WaitForEndOfFrame ();

		SaveTool.Instance.ResetIslandFolder ();

		yield return new WaitForEndOfFrame ();

		int l = 0;

        int currentLoadLimit = 0;

		for ( int y = 0; y < MapGenerator.Instance.MapScale_Y ; ++y ) {

			for (int x = 0; x < MapGenerator.Instance.MapScale_X; ++x ) {

				Coords c = new Coords ( x , y );

				Chunk targetChunk = Chunk.GetChunk (c);

				if (!targetChunk.HasIslands())
					continue;

				string fileName = "chk" + "x" + c.x + "y" + c.y;
				string path = "Islands/" + fileName;

				Coords pathedCoords = GetCoordsFromFile (fileName);

				SaveTool.Instance.SaveToCurrentMap (path,targetChunk);

				++l;

                ++currentLoadLimit;

                if( currentLoadLimit == loadLimit)
                {
                    //yield return new WaitForEndOfFrame();
                    yield return new WaitForSeconds(timeBetweenFrames);
                    LoadingScreen.Instance.Push (l);
                    currentLoadLimit = 0;
                }


			}



		}

		LoadingScreen.Instance.End ();

		yield return new WaitForEndOfFrame ();

		DisplayMinimap.Instance.Init ();

		NavigationManager.Instance.ChangeChunk (Directions.None);

        Vector3 islandPos = IslandManager.Instance.islands[0].transform.position + new Vector3(-2f, 0, -1.5f);

        PlayerBoat.Instance.transform.position = islandPos;
        PlayerBoat.Instance.SetTargetPos(islandPos);
        CamBehavior.Instance.RefreshCamOnPlayer();
	}
	public void SaveCurrentIsland () {
		//
	}
	public Coords GetCoordsFromFile ( string str ) {

		string s = str.Remove (0, 4);

		string xString = s.Remove(s.IndexOf ('y'));
		string yString = s.Remove (0, s.IndexOf ('y') + 1);

		return new Coords (int.Parse (xString), int.Parse (yString));
	}
	#endregion

}

[System.Serializable]
public class PlayerInfo
{
    [NonSerialized]
    public static PlayerInfo Instance;

    /// <summary>
    /// serializable
    /// </summary>
    public int pearlAmount = 0;

    public List<ApparenceItem> apparenceItems = new List<ApparenceItem>();
    /// <summary>
    /// 
    /// </summary>

    public delegate void OnChangePearlAmount();
    public static OnChangePearlAmount onChangePearlAmount;

    public List<int> mapIDs = new List<int>();

    public void Init()
    {
        Instance = this;
    }

    public PlayerInfo()
    {

    }

    public void RemovePearl(int i)
    {
        pearlAmount -= i;

        onChangePearlAmount();
    }

    public void AddPearl(int i)
    {
        pearlAmount += i;

        onChangePearlAmount();

    }

    public void AddApparenceItem(ApparenceItem apparenceItem)
    {
        apparenceItems.Add(apparenceItem);
       
        CrewCreator.Instance.GetApparenceItem(apparenceItem.apparenceType, apparenceItem.id).locked = false;
    }

    public void Save()
    {
        SaveTool.Instance.SaveToSpecificFolder("PlayerInfo", "player info", this);
    }
}

//[System.Serializable]
public class GameData
{

    public int progression = 0;

	// crew & loot
	public int					globalID = 0;
	public Crew 				playerCrew;
	public Loot 				playerLoot;

	public int 					playerWeight = 0;
	public int 					playerGold = 0;

	public int 					karma = 0;
	public int 					bounty = 0;

    public List<Formula>        formulas = new List<Formula>();

    public PlayerBoatInfo       playerBoatInfo;
	public Coords treasureCoords;
	public Coords homeCoords;

	// quests
	public List<Quest> 			currentQuests;
	public List<Quest>			finishedQuests;

		// time
	public bool 				raining = false;
	public int 					currentRain = 0;

	public bool 				night = false;
	public int 					timeOfDay = 0;

    public string               treasureName = "";


	public GameData()
	{
		// islands ids
	}
}