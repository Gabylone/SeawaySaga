using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public static MapGenerator Instance;

    // parameters
    public static MapParameters mapParameters;
    public MapParameters defaultMapParameters;

    string textFile_STR = "";

    public int test_row = 0;
    public int test_col = 0;

    public Dictionary<Coords, string> islandNames = new Dictionary<Coords, string>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log( GetCellLoc(test_row,test_col ));
        }
    }

    public int MapScale_X
    {
        get
        {
            return mapParameters.mapScale_X;
        }
    }

    public int MapScale_Y
    {
        get
        {
            return mapParameters.mapScale_Y;
        }
    }

    public void SetMapScale_X ( int i )
    {
        mapParameters.mapScale_X = i;
    }

    public void SetMapScale_Y(int i)
    {
        mapParameters.mapScale_Y = i;
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

    public string treasureName = "";

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

        string str_name = CrewCreator.Instance.boatNames[Random.Range(0, CrewCreator.Instance.boatNames.Length)];
        string str_adj = CrewCreator.Instance.boatAdjectives[Random.Range(0, CrewCreator.Instance.boatAdjectives.Length)];
        string fullName = str_adj + " " + str_name;

        treasureName = "The " + fullName + "s";

        LoadMapFromFile();

	}

    public void UpdateMapScale()
    {
        TextAsset textAsset = Resources.Load("Maps/" + mapParameters.mapName) as TextAsset;

        if (textAsset == null)
        {
            Debug.LogError("coulnd't find map : " + mapParameters.mapName + " in resources");
        }

        string[] rows = textAsset.text.Split('\n');
        string[] firstRowCells = rows[0].Split(';');

        // set map scale
        SetMapScale_X(firstRowCells.Length);
        SetMapScale_Y(rows.Length);
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
        TextAsset textAsset = Resources.Load("Maps/" + mapParameters.mapName) as TextAsset;

        if (textAsset == null)
        {
            Debug.LogError("coulnd't find map : " + mapParameters.mapName + " in resources");
        }

        string[] rows = textAsset.text.Split('\n');
        string[] firstRowCells = rows[0].Split(';');

        UpdateMapScale();

        // init chunks
        Chunk.chunks.Clear();

        for (int x = 0; x < MapScale_X; x++)
        {
            for (int y = 0; y < MapScale_Y; y++)
            {
                Coords c = new Coords(x, y);
                Chunk.chunks.Add(c, new Chunk());
            }
        }

        LoadingScreen.Instance.StartLoading("Chargement îles", MapScale_X * MapScale_Y);

        for (int rowIndex = 0; rowIndex < rows.Length; rowIndex++)
        {
            string row = rows[rowIndex].TrimEnd('\n', '\r', '\t');

            string[] cells = row.Split(';');

            for (int colIndex = 0; colIndex < cells.Length; colIndex++)
            {
                if (cells[colIndex].Length > 0)
                {
                    Coords c = new Coords(colIndex , (rows.Length-1) - rowIndex);

                    string[] storyNames = cells[colIndex].Split(',');

                    for (int storyAmount = 0; storyAmount < storyNames.Length; storyAmount++)
                    {
                        bool foundStory = false;

                        string storyName = storyNames[storyAmount];
                        storyName = storyName.TrimEnd('\r', '\n', '\t');

                        for (int storyTypeIndex = 0; storyTypeIndex < 4; storyTypeIndex++)
                        {
                            StoryType storyType = (StoryType)storyTypeIndex;

                            int storyID = StoryLoader.Instance.FindIndexByName(storyName, storyType);

                            if (storyID < 0)
                            {
                                
                            }
                            else
                            {
                                foundStory = true;

                                switch (storyType)
                                {
                                    case StoryType.Treasure:
                                        SaveManager.Instance.GameData.treasureCoords = c;
                                        break;
                                    case StoryType.Home:
                                        SaveManager.Instance.GameData.homeCoords = c;
                                        break;
                                    case StoryType.Clue:
                                        Formula newFormula = new Formula();
                                        newFormula.name = NameGeneration.Instance.randomWord;
                                        newFormula.coords = c;
                                        FormulaManager.Instance.formulas.Add(newFormula);
                                        break;
                                    default:
                                        break;
                                }

                                CreateIsland(c, storyType);
                                Chunk.GetChunk(c).GetIslandData(storyAmount).storyManager.InitHandler(storyType, storyID);
                            }

                            
                        }

                        if (!foundStory)
                        {
                            Debug.LogError("couldn't find story : " + storyName + " at " + GetCellLoc(rowIndex,colIndex));
                        }

                    }

                }
            }

        }

        yield return new WaitForEndOfFrame();

        SaveManager.Instance.SaveAllIslands();

    }

    public string GetCellLoc ( int row, int col)
    {
        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        string letter;

        int div = col / alphabet.Length;
        if (div >0)
        {
            int mod = col % alphabet.Length;
            letter = "" + alphabet[div-1] + alphabet[mod];
        }
        else
        {
            letter = "" + alphabet[col % alphabet.Length];
        }

        return "col : " + letter + " row : " + (row+1);
    }

    public void LoadMap() {

        UpdateMapScale();

		discoveredCoords = SaveTool.Instance.LoadFromCurrentMap ("discovered coords.xml", "DiscoveredCoords") as DiscoveredCoords;

		Chunk.chunks.Clear ();

		for (int x = 0; x < MapScale_X; x++) {

            for (int y = 0; y < MapScale_Y; y++)
            {
                Coords c = new Coords(x, y);

                Chunk.chunks.Add(c, new Chunk());
            }

		}

		foreach (var item in discoveredCoords.coords) {

			Chunk.GetChunk (item).state = ChunkState.DiscoveredSea;

		}
	}

	public void InitChunks() {
		
		Chunk.chunks.Clear ();

		for (int x = 0; x < MapScale_X; x++) {
			for (int y = 0; y < MapScale_Y; y++) {

				Coords c = new Coords (x, y);
				Chunk.chunks.Add (c, new Chunk ());
			}
		}
	}

    public void CreateIsland( Coords c , StoryType type )
    {
        Chunk.GetChunk(c).AddIslandData(new IslandData(type));

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


		int max = (int)((float)(IslandsPerCol * MapScale_X));
		LoadingScreen.Instance.StartLoading ("Création îles",max );

		int l = 0;
		int a = 0;

		for ( int y = 0; y < MapScale_Y ; ++y ) {

			for (int i = 0; i < IslandsPerCol; ++i ) {

				int x = Random.Range ( 0, MapScale_X );

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
        for (int y = 0; y < MapScale_Y; y++)
        {
            for (int x = 0; x < MapScale_X; x++)
            {
                Coords c = new Coords(x, y);

                for (int i = 0; i < Chunk.GetChunk(c).islandDatas.Length; i++)
                {
                    textFile_STR += Chunk.GetChunk(c).GetIslandData(i).storyManager.storyHandlers[0].Story.dataName;
                    if ( i < Chunk.GetChunk(c).islandDatas.Length - 1)
                    {
                        textFile_STR += ",";
                    }
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
			return new Coords (Random.Range ( 0, MapScale_X ),Random.Range ( 0, MapScale_Y ));
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