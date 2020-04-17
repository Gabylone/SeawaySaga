using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapChunk : MonoBehaviour {

	public delegate void OnTouchMinimapChunk (Chunk chunk, Transform tr);
	public static OnTouchMinimapChunk onTouchMinimapChunk;

	public Coords coords;

	public GameObject islandGroup;

	public Image image;

	public GameObject questGroup;

    private int id = 0;

	public void InitChunk (int _id, Coords worldCoords)
	{
        id = _id;

		Chunk chunk = Chunk.GetChunk (worldCoords);

		IslandData islandData = chunk.GetIslandData(id);

		image.sprite = Island.minimapSprites[islandData.storyManager.storyHandlers [0].Story.param];

		coords = worldCoords;

        switch (chunk.state)
        {
            case ChunkState.UndiscoveredIsland:
                SetUndiscovered();
                break;
            case ChunkState.DiscoveredIsland:
                SetDiscovered();
                break;
            case ChunkState.VisitedIsland:
                SetVisited();
                break;
            default:
                break;
        }

		if (QuestManager.Instance.currentQuests.Find (x => x.targetCoords == worldCoords) != null) {
			ShowQuestFeedback ();
		} else {
			HideQuestFeedback ();
		}
	}


	public void ShowQuestFeedback () {
		questGroup.SetActive (true);
	}

	public void HideQuestFeedback() {
		questGroup.SetActive (false);
	}

    public void Bounce()
    {
        Tween.Bounce(transform);
    }

    public void SetVisited ()
    {
        gameObject.SetActive(true);
		image.color = Color.white;
    }

	public void SetDiscovered ()
    {
        gameObject.SetActive(true);
		image.color = new Color( 0.5f,0.5f,0.5f );
	}

    void SetUndiscovered()
    {
        gameObject.SetActive(false);
        image.color = Color.clear;
    }

    public void TouchMinimapChunk () {
		
		Tween.Bounce (islandGroup.transform);

        Chunk chunk = Chunk.GetChunk(coords);
        IslandData islandData = chunk.GetIslandData(id);
        string str = "";

        if (islandData.storyManager.CurrentStoryHandler.Story.name.StartsWith("Maison"))
        {
            str = "Maison";
        }
        else if (chunk.state == ChunkState.VisitedIsland)
        {
            str = chunk.GetIslandData(id).storyManager.CurrentStoryHandler.Story.name;
        }
        else
        {
            str = "?";
        }

        IslandInfo.Instance.DisplayIslandInfo(str);
        IslandInfo.Instance.ShowAtTransform(islandGroup.transform);
    }
}
