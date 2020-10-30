using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class IslandData {
    
    // 
	public Vector2 worldPosition;
    public float worldRotation = 0f;

    public int index;
    public Coords coords;

    public bool containsFormula = false;

	public StoryManager storyManager;

	public IslandData ()
	{
        
    }

    public IslandData (StoryType storyType )
	{
		storyManager = new StoryManager ();

        worldPosition = NavigationManager.Instance.GetRandomIslandPosition();
        worldRotation = Random.Range( 0, 360f );
    }
}
