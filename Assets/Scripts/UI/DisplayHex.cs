using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayHex : MonoBehaviour {

	Island island;

	void Start () {
		
	}

	public void UdpateHex ( Coords coords ) {
	
		island = GetComponentInChildren<Island> ();

		Chunk chunk = Chunk.GetChunk (Boats.playerBoatInfo.coords);

//		island.UpdatePositionOnScreen (coords);
	}
}
