using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public static MapGenerator Instance;

    // parameters
    public static MapParameters mapParameters;
    public MapParameters defaultMapParameters;

    string textFile_STR = "";

    public Dictionary<Coords, string> islandNames = new Dictionary<Coords, string>();

    public int MapScale
    {
        get
        {
            return mapParameters.GetScale();
        }
    }

    public int IslandsPerCol
    {
        get
        {
            return mapParameters.islandPerCol;
        }
    }
    //

    [SerializeField]
	private int loadLimit = 1000;


	public int islandID;

	public DiscoveredCoords discoveredCoords;

	public int islandCreation_LoadLimit = 1;

	void Awake () {
		Instance = this;
	}

	#region map data
	public void CreateNewMap () {

        if (mapParameters == null)
        {
            mapParameters = defaultMapParameters;
        }

		discoveredCoords = new DiscoveredCoords ();

        //InitChunks();

        //GenerateNewMap();

        LoadMapFromFile();

	}

    private void GenerateNewMap()
    {
        SaveManager.Instance.GameData.treasureCoords = RandomCoords;
        CreateIsland(SaveManager.Instance.GameData.treasureCoords, StoryType.Treasure);

        CreateHomeIsland();

        FormulaManager.Instance.CreateNewClues();

        StartCoroutine(CreateNormalIslands());
    }

    private void LoadMapFromFile()
    {
        StartCoroutine(LoadMapFromFileCoroutine());
    }

    IEnumerator LoadMapFromFileCoroutine()
    {
        int l = 0;
        int a = 0;

        TextAsset textAsset = Resources.Load("Maps/" + mapParameters.mapName) as TextAsset;

        if ( textAsset == null)
        {
            Debug.LogError("coulnd't find map : " + mapParameters.mapName + " in resources");
        }

        string[] rows = textAsset.text.Split('\n');

        // set map scale
        mapParameters.SetScale(rows.Length);

        Debug.Log("setting map scale to : ");

        // init chunks
        Chunk.chunks.Clear();

        for (int x = 0; x < MapScale; x++)
        {
            for (int y = 0; y < MapScale; y++)
            {
                Coords c = new Coords(x, y);
                Chunk.chunks.Add(c, new Chunk());
            }
        }

        LoadingScreen.Instance.StartLoading("Chargement îles", MapScale * MapScale);

        for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
        {
            string row = rows[rowIndex].TrimEnd('\n', '\r', '\t');

            string[] cells = row.Split(';');

            for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
            {
                if (cells[cellIndex].Length > 0)
                {
                    Coords c = new Coords(cellIndex , (rows.Length-1) - rowIndex);

                    for (int storyTypeIndex = 0; storyTypeIndex < 4; storyTypeIndex++)
                    {
                        StoryType storyType = (StoryType)storyTypeIndex;

                        int storyID = StoryLoader.Instance.FindIndexByName(cells[cellIndex], storyType);

                        if (storyID < 0)
                        {

                        }
                        else
                        {
                            switch (storyType)
                            {
                                case StoryType.Treasure:
                                    SaveManager.Instance.GameData.treasureCoords = c;
                                    Debug.Log("treasure is at : " + c.ToString());
                                    break;
                                case StoryType.Home:
                                    SaveManager.Instance.GameData.homeCoords = c;
                                    Debug.Log("home is at : " + c.ToString());
                                    break;
                                case StoryType.Clue:
                                    Debug.Log("clue is at : " + c.ToString());
                                    Formula newFormula = new Formula();
                                    newFormula.name = NameGeneration.Instance.randomWord;
                                    newFormula.coords = c;
                                    FormulaManager.Instance.formulas.Add(newFormula);
                                    break;
                                default:
                                    break;
                            }

                            Chunk.GetChunk(c).InitIslandData(new IslandData(storyType));
                            Chunk.GetChunk(c).IslandData.storyManager.InitHandler(storyType, storyID);
                        }

                        ++l;
                        ++a;

                        if (l > islandCreation_LoadLimit)
                        {
                            l = 0;
                            //yield return new WaitForEndOfFrame();
                        }
                        //LoadingScreen.Instance.Push(a);


                    }

                }
            }

        }

        yield return new WaitForEndOfFrame();

        SaveManager.Instance.SaveAllIslands();

    }


	public void LoadMap() {

		discoveredCoords = SaveTool.Instance.LoadFromCurrentMap ("discovered coords.xml", "DiscoveredCoords") as DiscoveredCoords;

		Chunk.chunks.Clear ();

		for (int x = 0; x < MapScale; x++) {
			for (int y = 0; y < MapScale; y++) {
				
				Coords c = new Coords (x, y);

				Chunk.chunks.Add (c, new Chunk ());
				//				Chunk.chunks[c].stae
			}
		}

		foreach (var item in discoveredCoords.coords) {

			Chunk.GetChunk (item).state = ChunkState.DiscoveredSea;

		}
	}

	public void InitChunks() {
		
		Chunk.chunks.Clear ();

		for (int x = 0; x < MapScale; x++) {
			for (int y = 0; y < MapScale; y++) {

				Coords c = new Coords (x, y);
				Chunk.chunks.Add (c, new Chunk ());
			}
		}
	}

    public void CreateIsland( Coords c , StoryType type )
    {
        Chunk.GetChunk(c).InitIslandData(new IslandData(type));

    }

	void CreateHomeIsland () {

        Coords targetCoords = RandomCoords;

        while (targetCoords == SaveManager.Instance.GameData.treasureCoords)
        {
            Debug.Log("home island is same position as other, rolling coords pos again");
            targetCoords = RandomCoords;
        }

        SaveManager.Instance.GameData.homeCoords = targetCoords;
        CreateIsland(SaveManager.Instance.GameData.homeCoords, StoryType.Home);
	}
	IEnumerator CreateNormalIslands () {
//	void CreateNormalIslands () {


		int max = (int)((float)(IslandsPerCol * MapScale));
		LoadingScreen.Instance.StartLoading ("Création îles",max );

		int l = 0;
		int a = 0;

		for ( int y = 0; y < MapScale ; ++y ) {

			for (int i = 0; i < IslandsPerCol; ++i ) {

				int x = Random.Range ( 0, MapScale );

				Coords c = new Coords ( x , y );

				Chunk targetChunk = Chunk.GetChunk (c);

				if (targetChunk.state == ChunkState.UndiscoveredSea) {
                    CreateIsland(c, StoryType.Normal);
				}

				++l;
				++a;
				if ( l > islandCreation_LoadLimit ) {
					l = 0;
					//yield return new WaitForEndOfFrame ();
				}
				LoadingScreen.Instance.Push (a);
//				yield return new WaitForEndOfFrame ();
			}

		}

		yield return new WaitForEndOfFrame ();

        //CreateTextFile();

		if (GameManager.Instance.saveOnStart)
			SaveManager.Instance.SaveAllIslands ();
		else
			LoadingScreen.Instance.End ();

	}

    private void CreateTextFile()
    {
        for (int y = 0; y < MapScale; y++)
        {
            for (int x = 0; x < MapScale; x++)
            {
                Coords c = new Coords(x, y);

                if (Chunk.GetChunk(c).IslandData != null)
                {
                    textFile_STR += Chunk.GetChunk(c).IslandData.storyManager.storyHandlers[0].Story.name;
                }

                textFile_STR += ";";
            }

            textFile_STR += "\n";
        }
        

        System.IO.File.WriteAllText( Application.dataPath + "/Resources/Maps/" + mapParameters.mapName + ".txt", textFile_STR);

        Debug.Log(textFile_STR);
    }
    #endregion

    #region tools
    public Coords RandomCoords{
		get {
			return new Coords (Random.Range ( 0, MapScale ),Random.Range ( 0, MapScale ));
		}
	}
	public int RandomX {
		get {
			return Random.Range ( 0, MapScale );
		}
	}

	public int RandomY {
		get {
			return Random.Range ( 0, MapScale );
		}
	}
	#endregion

}

public class DiscoveredCoords {
	public List<Coords> coords = new List<Coords>();

	public DiscoveredCoords () {
		//
	}
}