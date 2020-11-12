using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {

	public static MapGenerator Instance;

    // parameters
    public static MapParameters mapParameters;
    public MapParameters defaultMapParameters;

    private string textFile_STR = "";

    public Dictionary<Coords, string> islandNames = new Dictionary<Coords, string>();

    public int GetMapHorizontalScale
    {
        get
        {
            return mapParameters.MapScale_X;
        }
    }

    public int GetMapVerticalScale
    {
        get
        {
            return mapParameters.MapScale_Y;
        }
    }

    public void SetMapScale_X ( int i )
    {
        mapParameters.SetMapScale_X(i);
    }

    public void SetMapScale_Y(int i)
    {
        mapParameters.SetMapScale_Y(i);
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

        treasureName = "The " + fullName;

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

        for (int x = 0; x < GetMapHorizontalScale; x++)
        {
            for (int y = 0; y < GetMapVerticalScale; y++)
            {
                Coords c = new Coords(x, y);
                Chunk.chunks.Add(c, new Chunk(c));
            }
        }

        LoadingScreen.Instance.StartLoading("Chargement îles", GetMapHorizontalScale * GetMapVerticalScale);

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

                    bool formulaIsland = false;

                    for (int storyAmount = 0; storyAmount < storyNames.Length; storyAmount++)
                    {
                        bool foundStory = false;

                        string storyName = storyNames[storyAmount];
                        storyName = storyName.TrimEnd('\r', '\n', '\t');

                        if (storyName.Contains("/"))
                        {
                            // it's a clue island
                            storyName = storyName.Split('/')[0];

                            // create new clue
                            Formula newFormula = new Formula();
                            newFormula.name = NameGeneration.Instance.randomWord;
                            newFormula.coords = c;
                            FormulaManager.Instance.formulas.Add(newFormula);


                            formulaIsland = true;
                        }
                        else
                        {
                            formulaIsland = false;

                        }

                        storyName = storyName.TrimEnd('\r', '\n', '\t');

                        for (int storyTypeIndex = 0; storyTypeIndex < 4; storyTypeIndex++)
                        {
                            StoryType storyType = (StoryType)storyTypeIndex;

                            int storyIndex = StoryLoader.Instance.FindIndexByName(storyName, storyType);

                            if (storyIndex < 0)
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
                                        //
                                        break;
                                    default:
                                        break;
                                }

                                CreateIsland(c, storyType, storyAmount, storyIndex, formulaIsland);

                            }
                            
                        }

                        if (!foundStory)
                        {
                            Debug.LogError("couldn't find story : " + storyName + " (" + storyName.Length + ") at " + GetCellLoc(rowIndex,colIndex));
                        }

                    }

                }
            }

        }

        yield return new WaitForEndOfFrame();

        SaveManager.Instance.CreateFirstSave();

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

		for (int x = 0; x < GetMapHorizontalScale; x++) {

            for (int y = 0; y < GetMapVerticalScale; y++)
            {
                Coords c = new Coords(x, y);

                Chunk.chunks.Add(c, new Chunk(c));
            }

		}

		foreach (var item in discoveredCoords.coords) {

			Chunk.GetChunk (item).state = ChunkState.DiscoveredSea;

		}
	}

    public void CreateIsland(Coords c, StoryType type, int storyAmount, int storyIndex, bool containsFormula)
    {
        IslandData newIslandData = new IslandData(type);

        newIslandData.containsFormula = containsFormula;

        newIslandData.storyManager.InitHandler(type, storyIndex);

        Chunk.GetChunk(c).AddIslandData(newIslandData);

    }
    #endregion

    #region tools
    public Coords RandomCoords{
		get {
			return new Coords (Random.Range ( 0, GetMapHorizontalScale ),Random.Range ( 0, GetMapVerticalScale ));
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