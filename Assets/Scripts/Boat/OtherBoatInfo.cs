using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class OtherBoatInfo : BoatInfo {

    public int id = 0;

	public StoryManager storyManager;

	private float changeOfChangeDirection = 0.2f;

    public bool alreadyMet = false;

    public bool moveNextTime = false;

    public Color color;

    public float timeToShowBoat = 1.5f;

	public override void Init ()
	{
		base.Init ();
	}

	public override void Randomize ()
	{
		base.Randomize ();

        // random coords, but not player coords
		SetCoords(MapGenerator.Instance.RandomCoords);

        color = Random.ColorHSV();

		SetDirection((Directions)Random.Range(0, 8));

		// assign story
		storyManager = new StoryManager ();
		storyManager.InitHandler (StoryType.Boat);
	}

	public override void TryMoveOnMap ()
	{
		base.TryMoveOnMap ();

        if ( moveNextTime)
        {
            moveNextTime = false;

            MoveToOtherChunk();
        }
        else
        {
            if (Random.value < 0.25f)
            {
                moveNextTime = true;
            }
        }
    }

	public void MoveToOtherChunk ()
	{
		Coords newCoords = coords + NavigationManager.Instance.getNewCoords (currentDirection);

		SetCoords(newCoords);

        newCoords = coords + NavigationManager.Instance.getNewCoords(currentDirection);

        if (newCoords.x >= MapGenerator.Instance.GetMapHorizontalScale - 1)
        {

            newCoords.x = coords.x;
            SwitchDirection();

        }
        else if (newCoords.x < 0)
        {

            newCoords.x = coords.x;
            SwitchDirection();
            //
        }
        else if (newCoords.y >= MapGenerator.Instance.GetMapVerticalScale - 1)
        {

            newCoords.y = coords.y;
            SwitchDirection();

        }
        else if (newCoords.y < 0)
        {

            newCoords.y = coords.y;
            SwitchDirection();

        }
        else
        {
            if (Random.value < changeOfChangeDirection)
            {
                // currentDirection = (Directions)Random.Range (0, 8);
            }

        }

    }

	private void SwitchDirection () {

        SetDirection(NavigationManager.GetOppositeDirection(currentDirection));

	}
}
