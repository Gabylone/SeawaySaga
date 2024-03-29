﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapChunk : MonoBehaviour
{
    public static MinimapChunk currentMinimapChunk;

	public delegate void OnTouchMinimapChunk (Chunk chunk, Transform tr);
	public static OnTouchMinimapChunk onTouchMinimapChunk;

	public Coords coords;

    public int index = 0;

	public GameObject islandGroup;

    public RectTransform rectTransform;

	public Image image;

	public GameObject questGroup;

    public Outline outline;

    public Text uiText_IslandCount;

    private void Start()
    {
        Deselect();
    }

    public void InitChunk (Coords worldCoords,int islandID)
	{
		Chunk chunk = Chunk.GetChunk (worldCoords);

		IslandData islandData = chunk.GetIslandData(islandID);

        image.sprite = IslandManager.Instance.GetIcon(islandData.storyManager.storyHandlers[0].Story.param);
        image.SetNativeSize();

        coords = worldCoords;

        this.index = islandID;

        if (islandData.storyManager.hasBeenPlayed)
        {
            SetVisited();
        }
        else if ( chunk.state == ChunkState.UndiscoveredIsland)
        {
            SetUndiscovered();
        }
        else
        {
            SetDiscovered();
        }
	}


	public void ShowQuestFeedback () {
		questGroup.SetActive (true);
        rectTransform.SetAsLastSibling();

    }

    public void HideQuestFeedback() {
		questGroup.SetActive (false);
	}

    public void Bounce()
    {
        Tween.Bounce(rectTransform);
    }

    public void SetVisited ()
    {
        gameObject.SetActive(true);
		image.color = Color.white;
    }

	public void SetDiscovered ()
    {
        Chunk chunk = Chunk.GetChunk(coords);

        IslandData islandData = chunk.GetIslandData(index);

        if ( islandData != null && islandData.storyManager.hasBeenPlayed)
        {
            SetVisited();
            return;
        }

        chunk.state = ChunkState.DiscoveredIsland;
        gameObject.SetActive(true);
        image.color = new Color( 0.5f,0.5f,0.5f );
	}

    void SetUndiscovered()
    {
        image.color = Color.clear;
    }

    public void TouchMinimapChunk () {

        Chunk chunk = Chunk.GetChunk(coords);

        if ( chunk.state == ChunkState.UndiscoveredIsland
            || chunk.state == ChunkState.UndiscoveredSea)
        {
            return;
        }

        if ( currentMinimapChunk != null)
        {
            if (currentMinimapChunk == this)
            {
                currentMinimapChunk.Deselect();
                currentMinimapChunk = null;
                return;
            }

            currentMinimapChunk.Deselect();
        }

		Tween.Bounce (rectTransform);

        string str = "";

        IslandData islandData = chunk.GetIslandData(index);

        if (islandData.storyManager.hasBeenPlayed)
        {
            /*int a = 0;
            foreach (var item in islandData.storyManager.storyHandlers)
            {
                if (item.storyType == StoryType.Quest)
                    continue;

                if (a > 0)
                {
                    str += "\n";
                    str += item.Story.displayName;
                }
                else
                {
                    str += item.Story.displayName;
                }

                ++a;
            }*/

            str = islandData.storyManager.storyHandlers[0].Story.displayName;
        }
        else
        {
            str = "?";
        }

        IslandInfo.Instance.DisplayIslandInfo(str);
        IslandInfo.Instance.ShowAtTransform(rectTransform);

        if (chunk.coords != Coords.current)
        {
            FastTravelButton.Instance.Display(rectTransform);
        }
        else
        {
            FastTravelButton.Instance.HideDelay();
        }

        SoundManager.Instance.PlaySound("button_tap_light 05");

        currentMinimapChunk = this;

        outline.enabled = true;

        rectTransform.SetAsLastSibling();
    }

    public void GetIslandNames()
    {

    }

    public void Deselect()
    {
        FastTravelButton.Instance.Hide();
        IslandInfo.Instance.Hide();

        outline.enabled = false;
    }

    public void OnPointerClick()
    {
        if (!DisplayMinimap.Instance.fullyDisplayed)
        {
            return;
        }

        TouchMinimapChunk();
    }
}
