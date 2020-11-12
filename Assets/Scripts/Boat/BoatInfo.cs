using UnityEngine;

[System.Serializable]
public class BoatInfo {


	public string Name = "bateau";

	public Coords previousCoords;
	public Coords coords;

	public Directions currentDirection;


    public BoatInfo () {
		
	}

	public virtual void Init () {
        //
        
	}

	public virtual void Randomize () {
		//
	}

	public virtual void TryMoveOnMap () {

	}

    public virtual void SetCoords(Coords newCoords)
    {
        previousCoords = coords;

        coords = newCoords;

        coords.x = Mathf.Clamp(newCoords.x, 0, MapGenerator.Instance.GetMapHorizontalScale - 1);
        coords.y = Mathf.Clamp(newCoords.y, 0, MapGenerator.Instance.GetMapVerticalScale - 1);
    }

    public virtual void SetDirection ( Directions dir)
    {
        currentDirection = dir;
    }

	public void Move ( Directions dir ) {
        SetDirection(dir);
		SetCoords(coords + NavigationManager.Instance.getNewCoords(currentDirection));
	}
}
