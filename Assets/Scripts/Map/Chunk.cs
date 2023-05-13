
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public enum ChunkState {
	//
	UndiscoveredSea,
	DiscoveredSea,
	UndiscoveredIsland,
	DiscoveredIsland,
	VisitedIsland,
    UndiscoveredVoid,
    DiscoveredVoid,

}

public class Chunk
{
	public static Dictionary<Coords,Chunk> chunks = new Dictionary<Coords, Chunk>();

    public ChunkState state;

    public Coords coords;

    // c'est ici que tout se passe
    public IslandData[] islandDatas = new IslandData[0];

    public Chunk (Coords c)
    {
        this.coords = c;
    }

	public Chunk () {
		
	}

	public void AddIslandData (IslandData newIslandData) {

        if ( islandDatas == null)
        {
            islandDatas = new IslandData[0];
        }

        IslandData[] tmp_IslandDatas = new IslandData[islandDatas.Length + 1];

        for (int i = 0; i < islandDatas.Length; i++)
        {
            tmp_IslandDatas[i] = islandDatas[i];
        }

        int index = tmp_IslandDatas.Length - 1;

        newIslandData.index = index;
        newIslandData.coords = coords;

        tmp_IslandDatas[index] = newIslandData;


        islandDatas = tmp_IslandDatas;

		state = ChunkState.UndiscoveredIsland;
	}

    public bool HasIslands()
    {
        return islandDatas != null && islandDatas.Length > 0;
    }
    
    public bool IsFormulaIsland()
    {
        if (StoryReader.Instance.CurrentStoryHandler.Story.dataName == "Jeu")
        {
            //Debug.Log("c'est l'histoire du jeu");
            bool secondLayer = StoryReader.Instance.currentStoryLayer == 1;
            return secondLayer && IslandManager.Instance.currentIsland.islandData.containsFormula;
        }
        else
        {
            bool firstLayer = StoryReader.Instance.currentStoryLayer == 0;
            return firstLayer && IslandManager.Instance.currentIsland.islandData.containsFormula;
        }
        
    }

    public IslandData GetIslandData(int id)
    {
        return islandDatas[id];
    }

    public int IslandCount
    {
        get
        {
            if (islandDatas != null)
            {
                return islandDatas.Length;
            }
            else
            {
                return 0;
            }
        }
    }

	public static Chunk currentChunk {
		get {
			return chunks[Boats.Instance.playerBoatInfo.coords];
		}
	}

	public static Chunk GetChunk (Coords c) {
		
		if (chunks.ContainsKey (c) == false) {
			//Debug.LogError ("chunk " + c.ToString() + " does not exist");
			return chunks [new Coords ()];
		}

		return chunks [c];
	}

	public static void SetChunk ( Coords c , Chunk chunk ) {

		chunks [c] = chunk;

	}

	public void SaveIslandData (Coords c)
	{
		string fileName = "chk" + "x" + c.x + "y" + c.y;	

		SaveTool.Instance.SaveToCurrentMap ( "Islands/"+fileName, this );
	}
}

