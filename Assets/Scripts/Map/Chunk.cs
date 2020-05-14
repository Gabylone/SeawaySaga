
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public enum ChunkState {
	//
	UndiscoveredSea,
	DiscoveredSea,
	UndiscoveredIsland,
	DiscoveredIsland,
	VisitedIsland

}

public class Chunk
{
	public static Dictionary<Coords,Chunk> chunks = new Dictionary<Coords, Chunk>();

	public ChunkState state;

    // c'est ici que tout se passe
    public IslandData[] islandDatas = new IslandData[0];

	public Chunk () {
		
	}

	public void AddIslandData (IslandData _newIslandData) {

        if ( islandDatas == null)
        {
            islandDatas = new IslandData[0];
        }

        IslandData[] tmp_IslandDatas = new IslandData[islandDatas.Length + 1];

        for (int i = 0; i < islandDatas.Length; i++)
        {
            tmp_IslandDatas[i] = islandDatas[i];
        }

        tmp_IslandDatas[tmp_IslandDatas.Length - 1] = _newIslandData;

        islandDatas = tmp_IslandDatas;

		state = ChunkState.UndiscoveredIsland;
	}

    public bool HasIslands()
    {
        return islandDatas != null && islandDatas.Length > 0;
    }

    public IslandData GetIslandData(int id)
    {
        return islandDatas[id];
    }

	public static Chunk currentChunk {
		get {
			return chunks[Boats.Instance.playerBoatInfo.coords];
		}
	}

	public static Chunk GetChunk (Coords c) {
		
		if (chunks.ContainsKey (c) == false) {
			Debug.LogError ("chunk " + c.ToString() + " does not exist");
			return chunks [new Coords ()];
		}

		return chunks [c];
	}

	public static void SetChunk ( Coords c , Chunk chunk ) {

		chunks [c] = chunk;

	}

	public void Save (Coords c)
	{
		string fileName = "chk" + "x" + c.x + "y" + c.y;	

		SaveTool.Instance.SaveToCurrentMap ( "Islands/"+fileName, this );
	}
}

