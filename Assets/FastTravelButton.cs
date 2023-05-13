using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using DG.Tweening;

public class FastTravelButton : MonoBehaviour, IPointerClickHandler
{
    public static FastTravelButton Instance;

    public Transform _transform;

    public Vector2 decal;

    public CanvasGroup CanvasGroup;
    public float fadeDuration = 0.5f;

    bool displaying = false;

    int foodNeeded;
    int foodAvailable;

    private Transform targetTransform;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _transform = GetComponent<Transform>();

        HideDelay();
    }

    private void Update()
    {
        if (displaying)
        {
            UpdatePosition();
        }
    }

    void UpdatePosition()
    {
        _transform.position = (Vector2)targetTransform.position + decal;
    }

    public void Display ( Transform _targetTransform)
    {
        targetTransform = _targetTransform;
        displaying = true;
        Show();

        UpdatePosition();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (MinimapChunk.currentMinimapChunk == null)
        {
            return;
        }

        Tween.Bounce(_transform);

        SoundManager.Instance.PlayRandomSound("Bag");

        if ( StoryLauncher.Instance.PlayingStory)
        {
            MessageDisplay.Instance.Display("You can't fast travel until someone takes the helm!");
            return;
        }

        if (TimeManager.Instance.raining)
        {
            MessageDisplay.Instance.Display("You cannot fast travel in this dangerous storm!");
            return;
        }

        Chunk chunk = Chunk.GetChunk(MinimapChunk.currentMinimapChunk.coords);

        if (chunk.state != ChunkState.VisitedIsland)
        {
            MessageDisplay.Instance.Display("You need to visit this island first to be able to fast travel to it!");
            return;
        }


        string IslandName = chunk.GetIslandData(MinimapChunk.currentMinimapChunk.index).storyManager.storyHandlers[0].Story.displayName;

        

        // get trips
        Vector2 current = (Vector2)Coords.current;
        Vector2 target = (Vector2)MinimapChunk.currentMinimapChunk.coords;
        float dis = Vector2.Distance(current, target);
        int trips = Mathf.RoundToInt(dis);

        // get travel info
        foodAvailable = GetFoodTrips();
        foodNeeded = trips * Crews.playerCrew.CrewMembers.Count;

        // display travel info
        DisplayFastTravelInfo.Instance.Display(foodNeeded, foodAvailable );

        // deselect current minimap chunk
        if ( MinimapChunk.currentMinimapChunk != null)
        {
            MinimapChunk.currentMinimapChunk.Deselect();
        }

        if (foodAvailable < foodNeeded)
        {
            MessageDisplay.Instance.Display("You don't have enough food to fast travel.\nIt would take <color=red>" + trips + "</color> trips to get to the <size=32><b>" + IslandName + "</b></size>");
            DisplayFastTravelInfo.Instance.SetRed();
        }
        else
        {
            MessageDisplay.Instance.Display("Travel to the " + IslandName + "?", true);

            DisplayFastTravelInfo.Instance.SetBlack();
            MessageDisplay.Instance.onValidate += HandleOnValidate_CanTravel;

        }


    }

    void HandleOnValidate_CanTravel()
    {
        SoundManager.Instance.PlayRandomSound("Bag");


        Transitions.Instance.ScreenTransition.FadeIn(0.5f);

        Invoke("ChangeChunk", 0.5f);
}

    void ChangeChunk()
    {
        Vector2 current = (Vector2)Coords.current;

        Vector2 target = (Vector2)MinimapChunk.currentMinimapChunk.coords;

        float dis = Vector2.Distance(current, target);

        int trips = Mathf.RoundToInt(dis);


        NavigationManager.Instance.ChangeChunk(MinimapChunk.currentMinimapChunk.coords, trips, foodNeeded, foodAvailable);

        SoundManager.Instance.PlayRandomSound("Swipe");

        Invoke("ChangeChunkDelay", 0.5f);

    }

    public int GetFoodTrips()
    {
        Loot loot = LootManager.Instance.getLoot(Crews.Side.Player);

        int foodTrips = 0;

        foreach (var foodItem in loot.AllItems[0])
        {
            switch (foodItem.spriteID)
            {
                // légume
                case 0:
                    foodTrips += 4;
                    break;
                // poisson
                case 1:
                    foodTrips += 6;
                    break;
                // viande
                case 2:
                    foodTrips += 8;
                    break;
                default:
                    break;
            }
        }

        return foodTrips;
    }

    void ChangeChunkDelay()
    {
        Transitions.Instance.ScreenTransition.FadeOut(0.5f);
    }

    public void Show()
    {
        CancelInvoke("HideDelay");

        gameObject.SetActive(true);

        Tween.Bounce(_transform);

        CanvasGroup.alpha = 0f;
        CanvasGroup.DOFade(1f, fadeDuration);
    }

    public void Hide()
    {
        CanvasGroup.DOFade(0f, fadeDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", fadeDuration);
    }

    public void HideDelay()
    {
        gameObject.SetActive(false);
    }
}
