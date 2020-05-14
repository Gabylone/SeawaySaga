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

    public Text uiText_IslandCount;

	public void InitChunk (Coords worldCoords)
	{
		Chunk chunk = Chunk.GetChunk (worldCoords);

		IslandData islandData = chunk.GetIslandData(0);

		image.sprite = Island.minimapSprites[islandData.storyManager.storyHandlers [0].Story.param];

        //uiText_IslandCount.text = "";

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
        //uiText_IslandCount.text = "" + Chunk.GetChunk(coords).islandDatas.Length;

        gameObject.SetActive(true);
		image.color = Color.white;
    }

	public void SetDiscovered ()
    {
        //uiText_IslandCount.text = "";
        gameObject.SetActive(true);
        image.color = new Color( 0.5f,0.5f,0.5f );
	}

    void SetUndiscovered()
    {
        //uiText_IslandCount.text = "";

        //gameObject.SetActive(false);
        image.color = Color.clear;
    }

    public void TouchMinimapChunk () {
		
		Tween.Bounce (islandGroup.transform);

        Chunk chunk = Chunk.GetChunk(coords);
        string str = "";

        if (chunk.state == ChunkState.VisitedIsland)
        {
            //str = chunk.GetIslandData(0).storyManager.CurrentStoryHandler.Story.displayName;
            for (int i = 0; i < chunk.islandDatas.Length; i++)
            {
                IslandData islandData = chunk.GetIslandData(i);

                if ( islandData.storyManager.hasBeenPlayed)
                {
                    str += islandData.storyManager.CurrentStoryHandler.Story.displayName;
                }
                else
                {
                    str += "?";
                }

                if ( i < chunk.islandDatas.Length -1)
                {
                    str += "\n";
                }
            }
        }
        else
        {
            str = "?";
        }

        IslandInfo.Instance.DisplayIslandInfo(str);
        IslandInfo.Instance.ShowAtTransform(islandGroup.transform);
    }
}
