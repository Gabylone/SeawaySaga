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
            MessageDisplay.Instance.Display("You cannot travel while on an island. Get on your boat !");
            return;
        }

        Chunk chunk = Chunk.GetChunk(MinimapChunk.currentMinimapChunk.coords);

        if (chunk.state != ChunkState.VisitedIsland)
        {
            MessageDisplay.Instance.Display("Visit this island before traveling there !");
            return;
        }


        string IslandName = chunk.GetIslandData(MinimapChunk.currentMinimapChunk.index).storyManager.storyHandlers[0].Story.displayName;

        // get travel info
        int trips_Food = GetFoodTrips();
        int trips_Hunger = GetMemberTrips();

        // get trips
        Vector2 current = (Vector2)Coords.current;
        Vector2 target = (Vector2)MinimapChunk.currentMinimapChunk.coords;
        float dis = Vector2.Distance(current, target);
        int trips = Mathf.RoundToInt(dis);

        // display travel info
        DisplayFastTravelInfo.Instance.Display(trips, trips_Food , trips_Hunger);

        // deselect current minimap chunk
        if ( MinimapChunk.currentMinimapChunk != null)
        {
            MinimapChunk.currentMinimapChunk.Deselect();
        }

        if (trips_Food + trips_Hunger < trips)
        {
            MessageDisplay.Instance.Display("You don't have enough food, or you crew has not eaten enough.\nIt would take <color=red>" + trips + "</color> trips to get to the <size=32><b>" + IslandName + "</b></size>");
            DisplayFastTravelInfo.Instance.SetRed();
        }
        else
        {
            MessageDisplay.Instance.Display("Travel to the " + IslandName + " ?");

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


        NavigationManager.Instance.ChangeChunk(MinimapChunk.currentMinimapChunk.coords, trips);

        SoundManager.Instance.PlayRandomSound("Swipe");

        Invoke("ChangeChunkDelay", 0.5f);

    }

    private int GetMemberTrips()
    {
        int trips = 0;

        foreach (var item in Crews.playerCrew.CrewMembers)
        {
            int memberAvailableTrips = item.MaxHunger - item.CurrentHunger;

            trips += memberAvailableTrips;
        }

        return trips;
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
