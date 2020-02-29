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

	public virtual void UpdatePosition () {

	}

    public virtual void SetCoords(Coords newCoords)
    {
        previousCoords = coords;

        coords = newCoords;

        coords.x = Mathf.Clamp(newCoords.x, 0, MapGenerator.Instance.MapScale - 1);
        coords.y = Mathf.Clamp(newCoords.y, 0, MapGenerator.Instance.MapScale - 1);
    }

	public void Move ( Directions dir ) {
		currentDirection = dir;
		SetCoords(coords + NavigationManager.Instance.getNewCoords(currentDirection));
	}
}
