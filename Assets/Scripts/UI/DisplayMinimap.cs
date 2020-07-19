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

    [Header("Enemy Boat Icons")]
    public GameObject enemyIconParent;
    public GameObject enemyBoatIconPrefab;
    public Vector2 enemyBoatIconDecal;
    public List<MinimapBoat> minimapBoatIcons = new List<MinimapBoat>();

    [Header("Player Boat Icons")]
    // boat feedback
    public RectTransform playerBoat_World_RectTransform;
    public RectTransform playerBoat_Local_RectTransform;
    public float centerTweenDuration = 0.5f;

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

    public RectTransform fullDisplay_Parent;
    public float initPosY = 0f;
    public float initPosX = 0f;
	public float initScaleY = 0f;
	public float initScaleX = 0f;

    public float rangeX = 10f;
    public float rangeY = 10f;

    public bool fullyDisplayed = false;
    ///

    private bool continueStoryOnClose = false;

	public Image outlineImage;

	public GameObject mapCloseButton;

    public float decal_X = 0f;
    public float decal_Y = 0f;


    void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		rectTransform = GetComponent<RectTransform> ();

		// subscribe

		CombatManager.Instance.onFightStart += FadeOut;
		CombatManager.Instance.onFightEnd += FadeIn;

		StoryFunctions.Instance.getFunction += HandleOnGetFunction;

		// quest feedback
		Quest.onSetTargetCoords += HandleOnSetTargetCoords;

		QuestManager.Instance.onFinishQuest += HandleOnFinishQuest;
		QuestManager.Instance.onGiveUpQuest += HandleOnGiveUpQuest;

        StoryLauncher.Instance.onPlayStory += HandlePlayStoryEvent;

		initScaleX = rectTransform.sizeDelta.x;
        initScaleY = rectTransform.sizeDelta.y;

        previousParent = transform.parent;

        targetScale = zoom_NormalScale;

		Show ();
		HideCloseButton ();
	}

    private void Update()
    {
        UpdatePlayerBoatIcon();
        UpdateZoom();
        if (fullyDisplayed)
        {
            UpdateZoomControl();
        }

        /*CenterOnBoat();
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crews.maxHunger = 20000;
        }*/
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

    void UpdatePlayerBoatIcon()
    {
        float x = rangeX * PlayerBoat.Instance.GetTransform.position.x / NavigationManager.Instance.maxX;
        float y = rangeY * PlayerBoat.Instance.GetTransform.position.z / NavigationManager.Instance.maxY;

        Vector2 pos = new Vector2(x, y);

        playerBoat_Local_RectTransform.anchoredPosition = pos;

        Vector3 dir = PlayerBoat.Instance.GetTransform.forward;
        dir = new Vector3(dir.x, dir.z, 0f);

        playerBoat_Local_RectTransform.up = dir;
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
        FullDisplay();

        // wait for map to open
        yield return new WaitForSeconds(fullDisplay_Duration);

        mapCloseButton.SetActive(false);

        yield return new WaitForSeconds(0.3f);

        targetScale = 1f;

        // pause
        yield return new WaitForSeconds(1);

        CenterOnBoat();
        UpdateRange(5);

        // wait for map to center
        yield return new WaitForSeconds(0.5f);

        continueStoryOnClose = true;

        mapCloseButton.SetActive(true);

    }

    #region update minimap chunks
    public MinimapChunk GetCurrentMinimapChunk()
    {
        int id = 0;

        if (IslandManager.Instance.currentIsland != null)
        {
            id = IslandManager.Instance.currentIsland.id;
        }

        return GetMinimapChunk(Coords.current, id);
    }

    public MinimapChunk GetMinimapChunk ( Coords coords , int targetIslandID)
    {
        MinimapChunk minimapChunk = minimapChunks.Find( x => x.coords == coords && x.islandID == targetIslandID);

        if ( minimapChunk == null)
        {
            Debug.LogError("coulfn't find minimap chunk at coords : " + coords.ToString());
        }
         
        return minimapChunk;
    }

	void HandleOnSetTargetCoords (Quest quest)
	{
		if (quest.targetCoords == quest.originCoords) {

            quest.GetOriginChunk().HideQuestFeedback();
        }

        quest.GetTargetChunk().ShowQuestFeedback();
    }
     
	void HandleOnGiveUpQuest (Quest quest)
	{
        MinimapChunk targetMinimapChunk = GetMinimapChunk(quest.targetCoords, quest.targetID);

        if ( targetMinimapChunk != null)
        {
            Debug.Log("hide target feedback quest on mini map chunck");
            targetMinimapChunk.HideQuestFeedback();
        }
        else
        {
            Debug.LogError("ERROR : not found : hide target feedback quest on mini map chunck");

        }
    }

	void HandleOnFinishQuest (Quest quest)
	{
        MinimapChunk targetMinimapChunk = GetMinimapChunk(quest.targetCoords, quest.targetID);
        if (targetMinimapChunk != null)
        {
            Debug.Log("hide target feedback quest on mini map chunck");
            targetMinimapChunk.ShowQuestFeedback();
        }
        else
        {
            Debug.LogError("ERROR : not found : hide target feedback quest on mini map chunck");
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

        MovePlayerIcon();

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
        FullDisplay();


        // wait for map to open
        yield return new WaitForSeconds(fullDisplay_Duration);

        mapCloseButton.SetActive(false);

        // pause
        yield return new WaitForSeconds(0.3f);

        CenterOnMap_Tween(quest.GetTargetChunk().rectTransform);

        // wait for map to center
        yield return new WaitForSeconds(centerTweenDuration);

        MinimapCenterFeedback.Instance.CenterOnMap(quest.targetCoords);

        yield return new WaitForSeconds(centerTweenDuration);

        mapCloseButton.SetActive(true);

        continueStoryOnClose = true;

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

	}

    #region center
    void CenterOnBoat() {
        Debug.Log("centering on boat");
		CenterOnMap_Tween (playerBoat_World_RectTransform);
	}

    public void CenterOnBoat_Quick()
    {
        Canvas.ForceUpdateCanvases();

        Vector2 p =
            (Vector2)scrollRect.transform.InverseTransformPoint(overallRectTranfsorm.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(playerBoat_World_RectTransform.position);

        p = p + scrollViewRectTransform.rect.size / 2f;

        overallRectTranfsorm.anchoredPosition = p; 
    }

	public void CenterOnMap_Tween (RectTransform target)
	{
        /*float x = (float)coords.x / MapGenerator.Instance.GetMapHorizontalScale;
        float y = (float)coords.y / MapGenerator.Instance.GetMapVerticalScale;*/

        Canvas.ForceUpdateCanvases();

        Vector2 p =
            (Vector2)scrollRect.transform.InverseTransformPoint(overallRectTranfsorm.position)
            - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

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
                        break;
                    case ChunkState.UndiscoveredIsland:

                        chunk.state = ChunkState.DiscoveredIsland;
                        chunk.Save(c);

                        for (int i = 0; i < chunk.IslandCount; i++)
                        {
                            MinimapChunk minimapChunk = GetMinimapChunk(c, i);
                            minimapChunk.SetDiscovered();
                        }

                        break;
                    case ChunkState.DiscoveredIsland:
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

            minimapChunks.Add(minimapChunk);

            ++islandID;
        }

    }
	#endregion

	#region boatIcons
	void InitBoatIcons() {
		playerBoat_World_RectTransform.sizeDelta = Vector2.one * minimapChunkScale;
	}
	public Vector2 getPosFromCoords (Coords coords) {

		return new Vector2 ((minimapChunkScale / 2f) + coords.x * minimapChunkScale, (minimapChunkScale / 2f) + coords.y * minimapChunkScale);

	}
	void MovePlayerIcon () {

		Vector2 boatPos = getPosFromCoords (Boats.Instance.playerBoatInfo.coords);
        playerBoat_World_RectTransform.DOAnchorPos(boatPos, centerTweenDuration - 0.2f).SetDelay(0.2f);
		Tween.Bounce (playerBoat_World_RectTransform.transform);
    }

    private Vector2 GetIconPos(Coords c)
    {
        Vector2 targetPos = getPosFromCoords(c);

        if (Chunk.GetChunk(c).HasIslands())
        {
            targetPos.y += enemyBoatIconDecal.y;
        }

        if (c == Boats.Instance.playerBoatInfo.coords)
        {
            targetPos.x -= enemyBoatIconDecal.x;
        }

        return targetPos;
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

	void PlaceOtherBoatIcon (OtherBoatInfo boatInfo) {

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

    }
    #endregion

    void Show () {
		scrollViewRectTransform.gameObject.SetActive (true);
	}

	void Hide () {
		scrollViewRectTransform.gameObject.SetActive (false);
	}

	#region full display
	public void FullDisplay ()
	{
		Transitions.Instance.ScreenTransition.FadeIn (fullDisplay_Duration/2f);

        CancelInvoke("ShowCloseButton");
        CancelInvoke("FullDisplayDelay");
        Invoke ("ShowCloseButton",fullDisplay_Duration);
		Invoke ("FullDisplayDelay", fullDisplay_Duration/2f);
	}
    void FullDisplayDelay()
    {
        transform.SetParent(targetParent);
        transform.SetAsFirstSibling();

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

        fullyDisplayed = true;

        CenterOnBoat_Quick();
    }

	public void ExitFullDisplay ()
	{
		if (fullDisplay_Exiting)
			return;

        targetScale = zoom_NormalScale;

		Tween.Bounce (mapCloseButton.transform, 0.2f , 1.1f);

		Transitions.Instance.ScreenTransition.FadeIn (fullDisplay_Duration/2f);

		fullDisplay_Exiting = true;

		Invoke ("HideCloseButton",0.2f);
		Invoke ("ExitFullDisplayDelay", fullDisplay_Duration/2f);
	}
	void ExitFullDisplayDelay() {

        transform.SetParent(previousParent);
        transform.SetAsLastSibling();

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

        if (continueStoryOnClose)
        {
            CancelInvoke("ContinueStory");
            Invoke("ContinueStory", 0.3f);
        }
	}

    void ContinueStory()
    {
        StoryReader.Instance.NextCell();
        StoryReader.Instance.UpdateStory();

        continueStoryOnClose = false;
    }

	void ShowCloseButton ()
	{
		Tween.Bounce (mapCloseButton.transform, 0.2f , 1.1f);
		mapCloseButton.SetActive (true);
	}
	void HideCloseButton ()
	{
		fullDisplay_Exiting = false;
		mapCloseButton.SetActive (false);
	}
	void FadeOut ()
	{
        rectTransform.DOAnchorPos(new Vector2(hiddenPos, 0f), hideDuration);
	}
	void FadeIn ()
	{
        rectTransform.DOAnchorPos(new Vector2(initPosX, initPosY), hideDuration);
	}
	#endregion
}
