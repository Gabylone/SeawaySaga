using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DisplayMinimap : MonoBehaviour {

	public static DisplayMinimap Instance;


    private RectTransform rectTransform;

    // minimap chunks
    [Header("Minimap Chunks")]
    public GameObject minimapChunkPrefab;
	public GameObject minimapChunkParent;
    public float minimapChunkDecal = 3f;
    public float minimapChunkScale;
    public List<MinimapChunk> minimapChunks = new List<MinimapChunk>();

    [Header("Minimap Boats")]
    public MinimapBoat enemyBoatIconPrefab;
    public MinimapBoat playerBoatIconPrefab;
    public Transform minimapIconsParent;


    /*public Vector2 enemyBoatIconDecal;
    public List<MinimapBoat> minimapBoatIcons = new List<MinimapBoat>();*/

    [Header("Player Boat Icons")]
    public float centerTweenDuration = 0.5f;

    public float rangeX = 15f;
    public float rangeY = 15f;

    // minimap
    [Header("Components")]
    public RectTransform overallRectTranfsorm;
    public ScrollRect scrollRect;
	public RectTransform scrollViewRectTransform;
	public Mask viewPortMask;

    public Transform previousParent;
    public Transform targetParent;

	public Image rayBlockerImage;

	public float hiddenPos = 0f;
	public float hideDuration = 0.3f;

    [Header("Zoom")]
    public float zoom_Speed = 1f;
    public float zoom_LerpSpeed = 1f;
    public float zoom_Max = 5f;
    public float zoom_Min = 1f;
    public float zoom_NormalScale = 2.7f;
    public float targetScale = 2.7f;

	[Header("Full Display")]
    public float fullDisplay_Duration = 0.8f;
    private bool fullDisplay_Exiting = false;

    public GameObject fullDisplay_ButtonObj;


    public RectTransform fullDisplay_Parent;
    public float initPosY = 0f;
    public float initPosX = 0f;
	public float initScaleY = 0f;
    public float initScaleX = 0f;

    public delegate void OnFullDisplay();
    public OnFullDisplay onFullDisplay;

    public bool fullyDisplayed = false;
    ///

    public bool continueStoryOnClose = false;

	public Image outlineImage;

	public GameObject fullMapGroup;

    public float decal_X = 0f;
    public float decal_Y = 0f;


    void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {

        rectTransform = GetComponent<RectTransform>();

		// subscribe
		CombatManager.Instance.onFightStart += HandleOnFightStart;
		CombatManager.Instance.onFightEnd   += HandleOnFightEnd;

		StoryFunctions.Instance.getFunction += HandleOnGetFunction;
        
		QuestManager.Instance.onFinishQuest += HandleOnFinishQuest;
		QuestManager.Instance.onGiveUpQuest += HandleOnGiveUpQuest;

        StoryLauncher.Instance.onPlayStory  += HandlePlayStoryEvent;

		initScaleX = rectTransform.sizeDelta.x;
        initScaleY = rectTransform.sizeDelta.y;

        previousParent = rectTransform.parent;

        targetScale = zoom_NormalScale;

		Show ();

		HideFullMapGroup ();
    }

    private void Update()
    {
        UpdateZoom();

        if (fullyDisplayed)
        {
            UpdateZoomControl();
        }
    }

    void UpdateZoom()
    {
        if (overallRectTranfsorm.localScale.x >= targetScale + 0.1f)
        {
            overallRectTranfsorm.localScale = Vector3.Lerp(overallRectTranfsorm.localScale, Vector3.one * targetScale, zoom_LerpSpeed * Time.deltaTime);
            CenterOnBoat_Quick();

        }
        else if (overallRectTranfsorm.localScale.x <= targetScale - 0.1f)
        {
            overallRectTranfsorm.localScale = Vector3.Lerp(overallRectTranfsorm.localScale, Vector3.one * targetScale, zoom_LerpSpeed * Time.deltaTime);
            CenterOnBoat_Quick();
        }
    }

    void UpdateZoomControl()
    {

        if (Input.mouseScrollDelta.y >= 0.1f)
        {
            if (targetScale <= zoom_Max)
            {
                targetScale += zoom_Speed * Time.deltaTime;
            }
        }
        else if (Input.mouseScrollDelta.y <= -0.1f)
        {
            if (targetScale >= zoom_Min)
            {
                targetScale -= zoom_Speed * Time.deltaTime;
            }
        }
    }

    public void Init () {
		InitMap ();
	}

	void HandleOnGetFunction (FunctionType func, string cellParameters)
	{
		if (func == FunctionType.ObserveHorizon) {

            StartCoroutine(ObserveHorizonCoroutine());

		}
	}

    IEnumerator ObserveHorizonCoroutine()
    {
        continueStoryOnClose = true;

        FullDisplay();

        // wait for map to open
        yield return new WaitForSeconds(fullDisplay_Duration);

        targetScale = 1f;

        // pause
        yield return new WaitForSeconds(1);

        CenterOnBoat();
        UpdateRange(5);
    }

    #region update minimap chunks
    public MinimapChunk GetCurrentMinimapChunk()
    {
        return GetMinimapChunk(IslandManager.Instance.GetCurrentIslandData());
    }

    public MinimapChunk GetMinimapChunk ( IslandData islandData )
    {
        MinimapChunk minimapChunk = minimapChunks.Find( x => x.coords == islandData.coords && x.index == islandData.index);

        if ( minimapChunk == null)
        {
            Debug.LogError("coulfn't find minimap chunk at coords : " + islandData.coords);
        }
         
        return minimapChunk;
    }
     
	void HandleOnGiveUpQuest (Quest quest)
	{
        MinimapChunk originMinimapChunk = GetMinimapChunk(quest.GetOriginIslandData());
        MinimapChunk targetMinimapChunk = GetMinimapChunk(quest.GetTargetIslandData());

        if (originMinimapChunk != null)
        {
            originMinimapChunk.HideQuestFeedback();
        }

        if ( targetMinimapChunk != null)
        {
            targetMinimapChunk.HideQuestFeedback();
        }
    }

	void HandleOnFinishQuest (Quest quest)
	{
        MinimapChunk originMinimapChunk = GetMinimapChunk(quest.GetOriginIslandData());
        MinimapChunk targetMinimapChunk = GetMinimapChunk(quest.GetTargetIslandData());

        if (originMinimapChunk != null)
        {
            originMinimapChunk.HideQuestFeedback();
        }

        if (targetMinimapChunk != null)
        {
            targetMinimapChunk.HideQuestFeedback();
        }
    }

    void HandlePlayStoryEvent()
    {
        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.island)
        {
            MinimapChunk minimapChunk = GetCurrentMinimapChunk();

            if (minimapChunk != null)
            {
                minimapChunk.Bounce();
                minimapChunk.SetVisited();
            }
        }
    }

	public void HandleChunkEvent ()
	{
		UpdateRange (GetCurrentShipRange);

        UpdateOtherBoatsMinimapIcon();

        //CenterOnBoat();

        CancelInvoke("CenterOnBoat");
        Invoke("CenterOnBoat" , centerTweenDuration);
    }
    #endregion

    #region quest
    public void ShowQuest(Quest quest)
    {
        StartCoroutine(ShowQuestCoroutine(quest));
        
    }

    IEnumerator ShowQuestCoroutine(Quest quest)
    {
        MinimapChunk minimapChunk = quest.GetTargetChunk();

        FullDisplay();

        // pause
        yield return new WaitForSeconds(0.3f);

        CenterOnMap_Tween(minimapChunk.rectTransform);

        // wait for map to center
        yield return new WaitForSeconds(centerTweenDuration);


        MinimapCenterFeedback.Instance.CenterOnMap(minimapChunk.rectTransform.anchoredPosition);

        Chunk chunk = Chunk.GetChunk(minimapChunk.coords);

        switch (chunk.state)
        {
            case ChunkState.UndiscoveredSea:
            case ChunkState.DiscoveredSea:
            case ChunkState.UndiscoveredIsland:
            case ChunkState.DiscoveredIsland:
                minimapChunk.SetDiscovered();
                break;
            case ChunkState.VisitedIsland:
                break;
            default:
                break;
        }

        minimapChunk.ShowQuestFeedback();

    }
    #endregion

    #region events
    void HandleOnFightStart()
    {
        FadeOut();
        fullDisplay_ButtonObj.SetActive(false);
    }

    void HandleOnFightEnd()
    {
        FadeIn();
        fullDisplay_ButtonObj.SetActive(true);
    }
    #endregion

    void InitMap ()
	{
        overallRectTranfsorm.sizeDelta = new Vector2(minimapChunkScale * MapGenerator.Instance.GetMapHorizontalScale, minimapChunkScale * MapGenerator.Instance.GetMapVerticalScale);

		for (int x = 0; x < MapGenerator.Instance.GetMapHorizontalScale; x++) {

			for (int y = 0; y < MapGenerator.Instance.GetMapVerticalScale; y++) {

                Coords coords = new Coords (x, y);

                Chunk chunk = Chunk.GetChunk(coords);


                if (chunk.state == ChunkState.DiscoveredIsland
					|| chunk.state == ChunkState.VisitedIsland
                    || chunk.state == ChunkState.UndiscoveredIsland) {

					PlaceMapChunks (coords);

				}
			}
		}

        QuestManager.Instance.ShowQuestsOnMap();

        CenterOnBoat_Quick();

	}

    #region center
    void CenterOnBoat()
    {
		CenterOnMap_Tween (MinimapBoat_Player.Instance.world_RectTransform);
	}

    public void CenterOnBoat_Quick()
    {
        CenterOnMap_Quick(MinimapBoat_Player.Instance.world_RectTransform);
    }

    public void CenterOnMap_Quick(Transform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 p =
            (Vector2)scrollViewRectTransform.InverseTransformPoint(overallRectTranfsorm.position)
            - (Vector2)scrollViewRectTransform.InverseTransformPoint(target.position);

        p = p + scrollViewRectTransform.rect.size / 2f;

        overallRectTranfsorm.anchoredPosition = p;
    }

    public void CenterOnMap_Tween (RectTransform target)
	{
        /*float x = (float)coords.x / MapGenerator.Instance.GetMapHorizontalScale;
        float y = (float)coords.y / MapGenerator.Instance.GetMapVerticalScale;*/

        Canvas.ForceUpdateCanvases();

        Vector2 p =
            (Vector2)scrollViewRectTransform.InverseTransformPoint(overallRectTranfsorm.position)
            - (Vector2)scrollViewRectTransform.InverseTransformPoint(target.position);

        p = p + scrollViewRectTransform.rect.size / 2f;

        //overallRectTranfsorm.anchoredPosition = p; 
        overallRectTranfsorm.DOAnchorPos(p, centerTweenDuration);
    }
	#endregion

	#region map range
	void UpdateRange (int range)
	{
		for (int x = -range; x <= range; x++) {
			
			for (int y = -range; y <= range; y++) {

				Coords c = Boats.Instance.playerBoatInfo.coords + new Coords (x, y);

				if (c.OutOfMap ())
					continue;

				Chunk chunk = Chunk.GetChunk (c);

				switch (chunk.state) {

                    case ChunkState.UndiscoveredSea:
                        chunk.state = ChunkState.DiscoveredSea;
                        MapGenerator.Instance.discoveredCoords.coords.Add(c);

                        foreach (var item in chunk.islandDatas)
                        {
                            MinimapChunk minimapChunk = GetMinimapChunk(item);
                            minimapChunk.SetDiscovered();
                        }

                        break;
                    case ChunkState.UndiscoveredIsland:

                        chunk.state = ChunkState.DiscoveredIsland;
                        chunk.SaveIslandData(c);

                        foreach (var item in chunk.islandDatas)
                        {
                            MinimapChunk minimapChunk = GetMinimapChunk(item);
                            minimapChunk.SetDiscovered();
                        }

                        break;
                    case ChunkState.DiscoveredIsland:

                        foreach (var item in chunk.islandDatas)
                        {
                            MinimapChunk minimapChunk = GetMinimapChunk(item);
                            minimapChunk.SetDiscovered();
                        }
                        break;
                    case ChunkState.VisitedIsland:
                        break;
                    default:
                        break;



				}

                
            }

		}

        MinimapTexture.Instance.UpdateBackgroundImage();
	}
	#endregion

	#region map chunk
	void PlaceMapChunks(Coords coords) {

        /*if ( minimapChunks.ContainsKey(c))
        {
            Debug.Log("un minimap chunk est déjà présent à : " + c.ToString());
            return;
        }*/

        int islandID = 0;

        Chunk chunk = Chunk.GetChunk(coords);

        foreach (var islandData in chunk.islandDatas)
        {
            GameObject minimapChunk_Obj = Instantiate(minimapChunkPrefab, minimapChunkParent.transform);

            MinimapChunk minimapChunk = minimapChunk_Obj.GetComponent<MinimapChunk>();

            // SCALE
            minimapChunk.rectTransform.localScale = Vector3.one;

            // POS
            float x = (minimapChunkScale / 2) + (coords.x * overallRectTranfsorm.rect.width / MapGenerator.Instance.GetMapHorizontalScale);
            float y = (minimapChunkScale / 2) + (coords.y * overallRectTranfsorm.rect.height / MapGenerator.Instance.GetMapVerticalScale);

            float decalX = rangeX * islandData.worldPosition.x / NavigationManager.Instance.maxX;
            float decalY = rangeY * islandData.worldPosition.y / NavigationManager.Instance.maxY;

            Vector2 pos = new Vector2(x + decalX, y + decalY);

            minimapChunk.rectTransform.anchoredPosition = pos;

            minimapChunk.InitChunk(coords, islandID);

            minimapChunk.HideQuestFeedback();

            minimapChunks.Add(minimapChunk);

            ++islandID;
        }

    }
	#endregion

	#region boatIcons
	public Vector2 GetPositionFromCoords (Coords coords) {

		return new Vector2 ((minimapChunkScale / 2f) + coords.x * minimapChunkScale, (minimapChunkScale / 2f) + coords.y * minimapChunkScale);
	}

	public int GetCurrentShipRange {
		get {
			int range = Boats.Instance.playerBoatInfo.shipRange;

            if (TimeManager.Instance.raining)
				range--;
			
			if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
				range--;

			return range;
		}
	}

    public MinimapBoat CreateMinimapBoat(MinimapBoat prefab, Transform boatTransform, BoatInfo boatInfo)
    {
        MinimapBoat minimapBoat = Instantiate(prefab, minimapIconsParent) as MinimapBoat;

        minimapBoat.linkedBoatInfo = boatInfo;
        minimapBoat.targetBoatTransform = boatTransform;

        return minimapBoat;
    } 

	public void UpdateOtherBoatsMinimapIcon()
	{
        /*foreach (var item in minimapBoatIcons)
        {
            item.gameObject.SetActive(false);
        }

		foreach ( OtherBoatInfo boatInfo in Boats.Instance.getBoats ) {

            PlaceOtherBoatIcon(boatInfo);

            //++boatIndexInRange;

        }*/
	}

	/*void PlaceOtherBoatIcon (OtherBoatInfo boatInfo) {

        // if new boat appears, instantiate icon

        MinimapBoat targetMinimapBoat = minimapBoatIcons.Find( x => x.boatInfo.id == boatInfo.id );

		if (targetMinimapBoat == null ) {

            GameObject boatIcon = Instantiate(enemyBoatIconPrefab, enemyIconParent.transform);

            targetMinimapBoat = boatIcon.GetComponent<MinimapBoat>();

            targetMinimapBoat.rectTransform.localScale = Vector3.one;

            targetMinimapBoat.rectTransform.sizeDelta = Vector2.one * minimapChunkScale;

            targetMinimapBoat.boatInfo = boatInfo;

            //targetMinimapBoat.GetComponentInChildren<Image>().color = boatInfo.color;

            minimapBoatIcons.Add(targetMinimapBoat);

            //Debug.Log("coudn't find boat for ship : ");

        }
        
        // boat approches
        if (boatInfo.coords.x <= Boats.Instance.playerBoatInfo.coords.x + (GetCurrentShipRange + 1) &&
            boatInfo.coords.x >= Boats.Instance.playerBoatInfo.coords.x - (GetCurrentShipRange + 1) &&
            boatInfo.coords.y <= Boats.Instance.playerBoatInfo.coords.y + (GetCurrentShipRange + 1) &&
            boatInfo.coords.y >= Boats.Instance.playerBoatInfo.coords.y - (GetCurrentShipRange + 1)
            )
        {

            Vector2 targetPos = GetIconPos(boatInfo.coords);

            if (boatInfo.coords.x <= Boats.Instance.playerBoatInfo.coords.x + GetCurrentShipRange &&
                boatInfo.coords.x >= Boats.Instance.playerBoatInfo.coords.x - GetCurrentShipRange &&
                boatInfo.coords.y <= Boats.Instance.playerBoatInfo.coords.y + GetCurrentShipRange &&
                boatInfo.coords.y >= Boats.Instance.playerBoatInfo.coords.y - GetCurrentShipRange
                )
            {
                targetMinimapBoat.rectTransform.DOAnchorPos(targetPos, centerTweenDuration);

                targetMinimapBoat.GetComponentInChildren<Image>().color = boatInfo.color;
            }
            else
            {
                targetMinimapBoat.rectTransform.anchoredPosition = targetPos;

                Color c = boatInfo.color;
                c.a = 0.5f;

                targetMinimapBoat.GetComponentInChildren<Image>().color = c;
            }

            targetMinimapBoat.gameObject.SetActive(true);
        }
        else
        {
            targetMinimapBoat.gameObject.SetActive(false);
        }

    }*/
    #endregion

    public void OnPointerClick()
    {
        if (fullyDisplayed)
        {

        }
        else
        {
            Tween.Bounce(scrollViewRectTransform);
            FullDisplay();
        }
    }

    void Show () {
		scrollViewRectTransform.gameObject.SetActive (true);
	}

	void Hide () {
		scrollViewRectTransform.gameObject.SetActive (false);
	}

	#region full display
	public void FullDisplay ()
	{
        fullDisplay_ButtonObj.SetActive(false);

        Transitions.Instance.ScreenTransition.FadeIn (fullDisplay_Duration/2f);

        SoundManager.Instance.PlaySound("click_light 01");
        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");
        fullyDisplayed = true;

        CancelInvoke("FullDisplayDelay");
		Invoke ("FullDisplayDelay", fullDisplay_Duration/2f);
	}
    void FullDisplayDelay()
    {
        ShowFullMapGroup();

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        rectTransform.SetParent(targetParent);
        rectTransform.SetAsFirstSibling();

        Vector2 scale = new Vector2(0f, 0f);

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);

        rectTransform.offsetMin = scale;
        rectTransform.offsetMax = scale;

        outlineImage.gameObject.SetActive(false);

        rayBlockerImage.gameObject.SetActive(true);
        rayBlockerImage.color = Color.black;

        viewPortMask.enabled = false;

        Transitions.Instance.ScreenTransition.FadeOut(fullDisplay_Duration / 2f);


        CenterOnBoat_Quick();

        if (onFullDisplay != null)
        {
            onFullDisplay();
        }
    }

	public void ExitFullDisplay ()
	{
		if (fullDisplay_Exiting)
			return;

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        targetScale = zoom_NormalScale;

		Transitions.Instance.ScreenTransition.FadeIn (fullDisplay_Duration/2f);

		fullDisplay_Exiting = true;

        FastTravelButton.Instance.Hide();
        IslandInfo.Instance.Hide();

        

        HideFullMapGroup();
        CancelInvoke("FullDisplayDelay");
		Invoke ("ExitFullDisplayDelay", fullDisplay_Duration/2f);
    }
	void ExitFullDisplayDelay() {

        fullDisplay_Exiting = false;

        rectTransform.SetParent(previousParent);
        rectTransform.SetAsLastSibling();

        rayBlockerImage.gameObject.SetActive(false);

        rectTransform.anchorMin = new Vector2( 0, 0);
        rectTransform.anchorMax = new Vector2( 0, 0);

        rectTransform.sizeDelta = new Vector2( initScaleX , initScaleY );

        rectTransform.anchoredPosition = Vector2.zero;

        viewPortMask.enabled = true;

        CenterOnBoat_Quick();

        outlineImage.gameObject.SetActive(true);
		Transitions.Instance.ScreenTransition.FadeOut (fullDisplay_Duration/2f);

        fullyDisplayed = false;

        fullDisplay_ButtonObj.SetActive(true);

        if (MinimapChunk.currentMinimapChunk != null)
        {
            MinimapChunk.currentMinimapChunk.Deselect();
        }

        if (StoryLauncher.Instance.PlayingStory)
        {
            FadeOut();
        }

        if (continueStoryOnClose)
        {
            continueStoryOnClose = false;
            CancelInvoke("ContinueStory");
            Invoke("ContinueStory", 0.3f);
        }
	}

    void ContinueStory()
    {
        StoryReader.Instance.NextCell();
        StoryReader.Instance.UpdateStory();

    }

	void ShowFullMapGroup ()
	{
		fullMapGroup.SetActive (true);
	}
	void HideFullMapGroup ()
	{
		fullMapGroup.SetActive (false);
	}
	public void FadeOut ()
	{
        rectTransform.DOAnchorPos(new Vector2(hiddenPos, 0f), hideDuration);
	}
	public void FadeIn ()
	{
        rectTransform.DOAnchorPos(new Vector2(initPosX, initPosY), hideDuration);
	}
	#endregion
}
