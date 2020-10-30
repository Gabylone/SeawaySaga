using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapChunk : MonoBehaviour
{

	public delegate void OnTouchMinimapChunk (Chunk chunk, Transform tr);
	public static OnTouchMinimapChunk onTouchMinimapChunk;

	public Coords coords;

    public int index = 0;

	public GameObject islandGroup;

    public RectTransform rectTransform;

	public Image image;

	public GameObject questGroup;

    public Text uiText_IslandCount;

    private void Start()
    {
        HideQuestFeedback();
    }

    public void InitChunk (Coords worldCoords,int islandID)
	{
		Chunk chunk = Chunk.GetChunk (worldCoords);

		IslandData islandData = chunk.GetIslandData(islandID);

        image.sprite = IslandManager.Instance.GetIcon(islandData.storyManager.storyHandlers[0].Story.param);
        image.SetNativeSize();

        coords = worldCoords;

        this.index = islandID;

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
            IslandData islandData = chunk.GetIslandData(index);

            if (islandData.storyManager.hasBeenPlayed)
            {
                int a = 0;
                foreach (var item in chunk.GetIslandData(index).storyManager.storyHandlers)
                {

                    if ( a > 0)
                    {
                        str += "\n";
                        str += item.Story.displayName;
                    }
                    else
                    {
                        str += item.Story.displayName;
                    }

                    ++a;
                }
            }
            else
            {
                str = "?";
            }
        }
        else
        {
            str = "?";
        }

        IslandInfo.Instance.DisplayIslandInfo(str);
        IslandInfo.Instance.ShowAtTransform(islandGroup.transform);

        SoundManager.Instance.PlaySound("button_tap_light 05");
    }

    public void OnPointerClick()
    {
        TouchMinimapChunk();
    }
}
