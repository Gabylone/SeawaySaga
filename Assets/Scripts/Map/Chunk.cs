
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
	private IslandData islandData;

	public Chunk () {
		
	}

	public void InitIslandData (IslandData _islandData) {
		state = ChunkState.UndiscoveredIsland;
		islandData = _islandData;
	}
	public void SetIslandData (IslandData _islandData ) {
		islandData = _islandData;
	}

	public IslandData IslandData {
		get {
			return islandData;
		}
		set {
			islandData = value;
		}
	}

	public static Chunk currentChunk {
		get {
			return chunks[Boats.playerBoatInfo.coords];
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

