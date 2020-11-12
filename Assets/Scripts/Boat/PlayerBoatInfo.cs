using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerBoatInfo : BoatInfo {
	
	public int shipRange = 1;
	public int crewCapacity = 2;
	public int cargoLevel = 1;
	public int level = 1;

	public int GetCargoCapacity () {
		return 100 + ( (cargoLevel-1) * 50);
	}

	// seulement lors d'une novelle partie
	public override void Randomize ()
	{
		base.Randomize ();

		SetCoords(SaveManager.Instance.GameData.homeCoords);
//		coords = Coords.Zero;

	}

    public override void SetDirection(Directions dir)
    {
        base.SetDirection(dir);
    }
}
