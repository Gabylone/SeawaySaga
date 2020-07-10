using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DisplayMinimap : MonoBehaviour {

	RectTransform rectTransform;

	public static DisplayMinimap Instance;

	// minimap chunks
	public GameObject minimapChunkPrefab;
	public GameObject minimapChunkParent;
    public GameObject enemyIconParent;

    public float minimapChunkDecal = 3f;

    // minimap
    public RectTransform overallRectTranfsorm;
	public RectTransform scrollViewRectTransform;
	public Mask viewPortMask;

	// boat feedback
	public RectTransform boatRectTransform;
    public float centerTweenDuration = 0.5f;

    public Color[] boatColords;

    public int childIndex = 0;
    public Transform previousParent;
    public Transform targetParent;

	public float minimapChunkScale;

	public GameObject enemyBoatIconPrefab;
	public Vector2 enemyBoatIconDecal;
    public List<MinimapBoat> minimapBoatIcons = new List<MinimapBoat>();

	public Image rayBlockerImage;

	public float hiddenPos = 0f;
	public float hideDuration = 0.3f;

	/// <summary>
	/// zoom
	/// </summary>
	public float zoomDuration = 0.8f;
    bool unzooming = false;

    public RectTransform zoomParent;
    public float initPosY = 0f;
    public float initPosX = 0f;
	public float initScaleY = 0f;
	public float initScaleX = 0f;

    public GameObject zoomBackground;
    public bool zoomed = false;
    ///

	public Image outlineImage;

	public GameObject mapCloseButton;

    public RectTransform viewport_RectTransofrm;

    public List<MinimapChunk> minimapChunks = new List<MinimapChunk>();

	void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		rectTransform = GetComponent<RectTransform> ();

		// subscribe
		NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;
		Quest.showQuestOnMap += HandleShowQuestOnMap;

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

        childIndex = transform.GetSiblingIndex();
        previousParent = transform.parent;

		Show ();
		HideCloseButton ();

		ClampScrollView ();
	}
    public void Init () {
		InitMap ();
	}

	void HandleOnGetFunction (FunctionType func, string cellParameters)
	{
		if (func == FunctionType.ObserveHorizon) {

			UpdateRange (4);

			StoryReader.Instance.NextCell ();

			StoryReader.Instance.UpdateStory ();

			Tween.Bounce (minimapChunkParent.transform);

		}
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

    public MinimapChunk GetMinimapChunk ( Coords coords , int islandID)
    {
        MinimapChunk minimapChunk = minimapChunks.Find( x => x.coords == coords );

        if ( minimapChunk == null)
        {
            Debug.LogError("coulfn't find minimap chunk at coords : " + coords.ToString());
        }
         
        return minimapChunk;
    }

	void HandleOnSetTargetCoords (Quest quest)
	{
		if (quest.targetCoords == quest.originCoords) {
            MinimapChunk originMinimapChunk = GetMinimapChunk(quest.originCoords, quest.originID);

            if (originMinimapChunk != null)
            {
                originMinimapChunk.HideQuestFeedback();
            }
        }

        MinimapChunk targetMinimapChunk = GetMinimapChunk(quest.targetCoords, quest.targetID);
        if ( targetMinimapChunk != null)
        {
            targetMinimapChunk.ShowQuestFeedback();
        }
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

	void HandleChunkEvent ()
	{
		UpdateRange (GetCurrentShipRange);

		CenterOnBoat ();

        MovePlayerIcon();

        UpdateOtherBoatsMinimapIcon();

	}
    #endregion
        
    void InitMap ()
	{
        overallRectTranfsorm.sizeDelta = new Vector2(minimapChunkScale * MapGenerator.Instance.MapScale_X, minimapChunkScale * MapGenerator.Instance.MapScale_Y);

		for (int x = 0; x < MapGenerator.Instance.MapScale_X; x++) {

			for (int y = 0; y < MapGenerator.Instance.MapScale_Y; y++) {

				Coords chunk = new Coords (x, y);

				if (Chunk.GetChunk (chunk).state == ChunkState.DiscoveredIsland
					|| Chunk.GetChunk (chunk).state == ChunkState.VisitedIsland
                    || Chunk.GetChunk(chunk).state == ChunkState.UndiscoveredIsland) {

					PlaceMapChunk (chunk);

				}
			}
		}

	}

	// QUEST
	#region quest
	void HandleShowQuestOnMap (Quest quest)
	{
        CenterMap(quest.targetCoords);
    }
    #endregion

    #region center
    public float decal_X = 0f;
    public float decal_Y = 0f;

    void CenterOnBoat() {
		CenterMap (Boats.Instance.playerBoatInfo.coords);
	}
	void ClampScrollView() {

		Coords coords = Boats.Instance.playerBoatInfo.coords;

		float x = overallRectTranfsorm.rect.width * (float)coords.x / MapGenerator.Instance.MapScale_X;

        x -= scrollViewRectTransform.rect.width / 2f - (minimapChunkScale / 2f);

        x += decal_X;

		x = Mathf.Clamp (x,0, (overallRectTranfsorm.rect.width - scrollViewRectTransform.rect.width));

		float y = overallRectTranfsorm.rect.height * (float)coords.y / MapGenerator.Instance.MapScale_Y;

        y -= scrollViewRectTransform.rect.height / 2f  - (minimapChunkScale/2f);

        y += decal_Y;

		y = Mathf.Clamp (y,0, overallRectTranfsorm.rect.height - scrollViewRectTransform.rect.height);

		Vector2 targetPos = new Vector2(-x,-y);
		overallRectTranfsorm.anchoredPosition = targetPos;
	}
	public void CenterMap (Coords coords)
	{
		int buffer = 5;

		float x = overallRectTranfsorm.rect.width * (float)coords.x / MapGenerator.Instance.MapScale_X;
		x -= scrollViewRectTransform.rect.width / 2f - (minimapChunkScale/2f);
		x = Mathf.Clamp (x,0-buffer, (overallRectTranfsorm.rect.width - scrollViewRectTransform.rect.width) +buffer);

		float y = overallRectTranfsorm.rect.height * (float)coords.y / MapGenerator.Instance.MapScale_Y;
		y -= scrollViewRectTransform.rect.height / 2f  - (minimapChunkScale/2f);
		y = Mathf.Clamp (y,0-buffer, overallRectTranfsorm.rect.height - scrollViewRectTransform.rect.height + buffer);

		Vector2 targetPos = new Vector2(-x,-y);

        overallRectTranfsorm.DOAnchorPos(targetPos, centerTweenDuration);
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

                        MinimapChunk minimapChunk = GetMinimapChunk(c,0);

                        if (minimapChunk != null)
                        {
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
	void PlaceMapChunk(Coords c) {

        /*if ( minimapChunks.ContainsKey(c))
        {
            Debug.Log("un minimap chunk est déjà présent à : " + c.ToString());
            return;
        }*/

        GameObject minimapChunk_Obj = Instantiate(minimapChunkPrefab, minimapChunkParent.transform);

        // SCALE
        minimapChunk_Obj.transform.localScale = Vector3.one;

        // POS
        float x = (minimapChunkScale / 2) + (c.x * overallRectTranfsorm.rect.width / MapGenerator.Instance.MapScale_X);
        float y = (minimapChunkScale / 2) + (c.y * overallRectTranfsorm.rect.height / MapGenerator.Instance.MapScale_Y);
        Vector2 pos = new Vector2(x, y);

        minimapChunk_Obj.GetComponent<RectTransform>().anchoredPosition = pos;

        MinimapChunk minimapChunk = minimapChunk_Obj.GetComponent<MinimapChunk>();

        minimapChunk.InitChunk(c);

        minimapChunks.Add(minimapChunk);

    }
	#endregion

	#region boatIcons
	void InitBoatIcons() {
		boatRectTransform.sizeDelta = Vector2.one * minimapChunkScale;
	}
	public Vector2 getPosFromCoords (Coords coords) {

		return new Vector2 ((minimapChunkScale / 2f) + coords.x * minimapChunkScale, (minimapChunkScale / 2f) + coords.y * minimapChunkScale);

	}
	void MovePlayerIcon () {

		Vector2 boatPos = getPosFromCoords (Boats.Instance.playerBoatInfo.coords);

        if (Chunk.currentChunk.HasIslands())
        {
            boatPos.y += enemyBoatIconDecal.y;
        }

        if (Boats.Instance.currentBoatAmount > 0)
        {
            boatPos.x += enemyBoatIconDecal.x;
        }

        boatRectTransform.DOAnchorPos(boatPos, centerTweenDuration - 0.2f).SetDelay(0.2f);

		Tween.Bounce (boatRectTransform.transform);


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

	#region zoom / unzoom
	public void Zoom ()
	{
		Transitions.Instance.ScreenTransition.FadeIn (zoomDuration/2f);

        CancelInvoke("ShowCloseButton");
        CancelInvoke("ZoomDelay");
        Invoke ("ShowCloseButton",zoomDuration);
		Invoke ("ZoomDelay",zoomDuration/2f);
	}
	void ZoomDelay () {

        transform.SetParent(targetParent);
        transform.SetAsFirstSibling();

		Vector2 scale = new Vector2(0f,0f);

        rectTransform.anchorMin = new Vector2 ( 0,0 );
        rectTransform.anchorMax = new Vector2 ( 1,1 );

		rectTransform.offsetMin = scale;
		rectTransform.offsetMax = scale;

        outlineImage.gameObject.SetActive(false);

        rayBlockerImage.gameObject.SetActive(true);
		rayBlockerImage.color = Color.black;

		viewPortMask.enabled = false;

		Transitions.Instance.ScreenTransition.FadeOut (zoomDuration/2f);

		ClampScrollView ();

        zoomed = true;
	}


	public void UnZoom ()
	{
		if (unzooming)
			return;

		Tween.Bounce (mapCloseButton.transform, 0.2f , 1.1f);

		Transitions.Instance.ScreenTransition.FadeIn (zoomDuration/2f);

		unzooming = true;

		Invoke ("HideCloseButton",0.2f);
		Invoke ("UnZoomDelay", zoomDuration/2f);
	}
	void UnZoomDelay () {

        transform.SetParent(previousParent);
        transform.SetAsLastSibling();

        rayBlockerImage.gameObject.SetActive(false);

        rectTransform.anchorMin = new Vector2( 0, 0);
        rectTransform.anchorMax = new Vector2( 0, 0);

        rectTransform.sizeDelta = new Vector2( initScaleX , initScaleY );

        rectTransform.anchoredPosition = Vector2.zero;

        viewPortMask.enabled = true;

		ClampScrollView ();

		outlineImage.gameObject.SetActive(true);
		Transitions.Instance.ScreenTransition.FadeOut (zoomDuration/2f);

        zoomed = false;
	}

	void ShowCloseButton ()
	{
		Tween.Bounce (mapCloseButton.transform, 0.2f , 1.1f);
		mapCloseButton.SetActive (true);
	}
	void HideCloseButton ()
	{
		unzooming = false;
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
