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

	public void InitChunk (Coords worldCoords)
	{
		Chunk chunk = Chunk.GetChunk (worldCoords);
		IslandData islandData = chunk.IslandData;

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

        IslandInfo.Instance.DisplayIslandInfo(Chunk.GetChunk(coords));
        IslandInfo.Instance.ShowAtTransform(islandGroup.transform);
    }
}
