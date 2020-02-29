using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DisplayMinimap : MonoBehaviour {

	RectTransform rectTransform;

	public static DisplayMinimap Instance;

	// minimap chunks
	public GameObject minimapChunkPrefab;
	public GameObject minimapChunkParent;
    public GameObject enemyIconParent;

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

	public Vector2 minimapChunkScale;

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

	public RectTransform zoomParent;
    public float initPosY = 0f;
    public float initPosX = 0f;
	public float initScaleY = 0f;
	public float initScaleX = 0f;

    public GameObject zoomBackground;

	public Image outlineImage;

	public GameObject mapCloseButton;

	public Dictionary<Coords,MinimapChunk> minimapChunks = new Dictionary<Coords, MinimapChunk>();

	void Awake () {
		Instance = this;

        onZoom = null;
	}

	// Use this for initialization
	void Start () {

		rectTransform = GetComponent<RectTransform> ();
		minimapChunkScale = new Vector2(minimapChunkPrefab.GetComponent<RectTransform> ().rect.width,minimapChunkPrefab.GetComponent<RectTransform> ().rect.height);

		// subscribe
		NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;
		Quest.showQuestOnMap += HandleShowQuestOnMap;

		CombatManager.Instance.onFightStart += FadeOut;
		CombatManager.Instance.onFightEnd += FadeIn;

		StoryFunctions.Instance.getFunction += HandleOnGetFunction;

		// quest feedback
		Quest.onSetTargetCoords += HandleOnSetTargetCoords;

		QuestManager.onFinishQuest += HandleOnFinishQuest;
		QuestManager.onGiveUpQuest += HandleOnGiveUpQuest;

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

		InitBoatIcons ();

		//HandleChunkEvent ();

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
	void HandleOnSetTargetCoords (Quest quest)
	{
		if (quest.targetCoords == quest.originCoords) {
			if ( minimapChunks.ContainsKey (quest.previousCoords) )
				minimapChunks [quest.previousCoords].HideQuestFeedback ();
		}

		if ( minimapChunks.ContainsKey(quest.targetCoords) )
			minimapChunks [quest.targetCoords].ShowQuestFeedback ();

	}

	void HandleOnGiveUpQuest (Quest quest)
	{
		if ( minimapChunks.ContainsKey(quest.targetCoords) )
		minimapChunks [quest.targetCoords].HideQuestFeedback ();
	}

	void HandleOnFinishQuest (Quest quest)
	{
		if ( minimapChunks.ContainsKey(quest.targetCoords) )
		minimapChunks [quest.targetCoords].HideQuestFeedback ();
	}

    void HandlePlayStoryEvent()
    {
        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.island)
        {
            minimapChunks[Coords.current].Bounce();
            minimapChunks[Coords.current].SetVisited();
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
		overallRectTranfsorm.sizeDelta = minimapChunkScale * (MapGenerator.Instance.MapScale);

		for (int x = 0; x < MapGenerator.Instance.MapScale; x++) {

			for (int y = 0; y < MapGenerator.Instance.MapScale; y++) {

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
    void CenterOnBoat() {
		CenterMap (Boats.playerBoatInfo.coords);
	}
	void ClampScrollView() {
		int buffer = 0;

		Coords coords = Boats.playerBoatInfo.coords;

		float x = overallRectTranfsorm.rect.width * (float)coords.x / MapGenerator.Instance.MapScale;
		x -= scrollViewRectTransform.rect.width / 2f - (minimapChunkScale.x/2f);
		//		x = Mathf.Clamp (x,0, overallRectTranfsorm.rect.width- scrollViewRectTransform.rect.widt );
		x = Mathf.Clamp (x,0-buffer, (overallRectTranfsorm.rect.width - scrollViewRectTransform.rect.width) +buffer);

		float y = overallRectTranfsorm.rect.height * (float)coords.y / MapGenerator.Instance.MapScale;
		y -= scrollViewRectTransform.rect.height / 2f  - (minimapChunkScale.y/2f);
		//		y = Mathf.Clamp (y,0, overallRectTranfsorm.rect.height - (scrollViewRectTransform.rect.height/2f));
		y = Mathf.Clamp (y,0-buffer, overallRectTranfsorm.rect.height - scrollViewRectTransform.rect.height + buffer);

		//		float x = overallRectTranfsorm.rect.width * (float)coords.x / MapGenerator.Instance.MapScale;
		//		x -= minimapChunkScale.x/2f;
		//		x = Mathf.Clamp (x,0, overallRectTranfsorm.rect.width - scrollViewRectTransform.rect.width);
		//
		//		float y = overallRectTranfsorm.rect.height * (float)coords.y / MapGenerator.Instance.MapScale;
		//		y -= minimapChunkScale.y/2f;
		//		y = Mathf.Clamp (y,0, overallRectTranfsorm.rect.height - scrollViewRectTransform.rect.height);

		Vector2 targetPos = new Vector2(-x,-y);
		overallRectTranfsorm.anchoredPosition = targetPos;
	}
	public void CenterMap (Coords coords)
	{
		int buffer = 5;

		float x = overallRectTranfsorm.rect.width * (float)coords.x / MapGenerator.Instance.MapScale;
		x -= scrollViewRectTransform.rect.width / 2f - (minimapChunkScale.x/2f);
		//		x = Mathf.Clamp (x,0, overallRectTranfsorm.rect.width- scrollViewRectTransform.rect.widt );
		x = Mathf.Clamp (x,0-buffer, (overallRectTranfsorm.rect.width - scrollViewRectTransform.rect.width) +buffer);

		float y = overallRectTranfsorm.rect.height * (float)coords.y / MapGenerator.Instance.MapScale;
		y -= scrollViewRectTransform.rect.height / 2f  - (minimapChunkScale.y/2f);
		//		y = Mathf.Clamp (y,0, overallRectTranfsorm.rect.height - (scrollViewRectTransform.rect.height/2f));
		y = Mathf.Clamp (y,0-buffer, overallRectTranfsorm.rect.height - scrollViewRectTransform.rect.height + buffer);

//		float x = overallRectTranfsorm.rect.width * (float)coords.x / MapGenerator.Instance.MapScale;
//		x -= minimapChunkScale.x/2f;
//		x = Mathf.Clamp (x,0, overallRectTranfsorm.rect.width - scrollViewRectTransform.rect.width);
//
//		float y = overallRectTranfsorm.rect.height * (float)coords.y / MapGenerator.Instance.MapScale;
//		y -= minimapChunkScale.y/2f;
//		y = Mathf.Clamp (y,0, overallRectTranfsorm.rect.height - scrollViewRectTransform.rect.height);

		Vector2 targetPos = new Vector2(-x,-y);

        overallRectTranfsorm.DOAnchorPos(targetPos, centerTweenDuration);
	}
	#endregion

	#region map range
	void UpdateRange (int range)
	{
		for (int x = -range; x <= range; x++) {
			
			for (int y = -range; y <= range; y++) {

				Coords c = Boats.playerBoatInfo.coords + new Coords (x, y);

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
                        minimapChunks[c].SetDiscovered();
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
        
        if ( minimapChunks.ContainsKey(c))
        {
            Debug.Log("un minimap chunk est déjà présent à : " + c.ToString());
            return;
        }

		// INST
		GameObject minimapChunk = Instantiate (minimapChunkPrefab, minimapChunkParent.transform);

		// SCALE
		minimapChunk.transform.localScale = Vector3.one;

		// POS
		float x = (minimapChunkScale.x/2) 	+ (c.x * overallRectTranfsorm.rect.width / MapGenerator.Instance.MapScale);
		float y = (minimapChunkScale.y / 2) + c.y * minimapChunkScale.y;
		Vector2 pos = new Vector2 (x,y);

		minimapChunk.GetComponent<RectTransform>().anchoredPosition = pos;

		minimapChunk.GetComponent<MinimapChunk> ().InitChunk (c);

		minimapChunks.Add (c, minimapChunk.GetComponent<MinimapChunk> ());

	}
	#endregion

	#region boatIcons
	void InitBoatIcons() {
		boatRectTransform.sizeDelta = minimapChunkScale;
	}
	public Vector2 getPosFromCoords (Coords coords) {

		return new Vector2 ((minimapChunkScale.x / 2f) + coords.x * minimapChunkScale.x, (minimapChunkScale.y / 2f) + coords.y * minimapChunkScale.y);

	}
	void MovePlayerIcon () {

		Vector2 boatPos = getPosFromCoords (Boats.playerBoatInfo.coords);

        if (Chunk.currentChunk.IslandData != null)
        {
            boatPos.y += enemyBoatIconDecal.y;
        }

        if (EnemyBoat.Instance.OtherBoatInfo != null)
        {
            boatPos.x += enemyBoatIconDecal.x;
        }

        boatRectTransform.DOAnchorPos(boatPos, centerTweenDuration - 0.2f).SetDelay(0.2f);

		Tween.Bounce (boatRectTransform.transform);


    }

    private Vector2 GetIconPos(Coords c)
    {
        Vector2 targetPos = getPosFromCoords(c);

        if (Chunk.GetChunk(c).IslandData != null)
        {
            targetPos.y += enemyBoatIconDecal.y;
        }

        if (c == Boats.playerBoatInfo.coords)
        {
            targetPos.x -= enemyBoatIconDecal.x;
        }

        return targetPos;
    }

	public int GetCurrentShipRange {
		get {
			int range = Boats.playerBoatInfo.shipRange;

			if (TimeManager.Instance.raining)
				range--;
			
			if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
				range--;

			return range;
		}
	}

	public void UpdateOtherBoatsMinimapIcon()
	{
        foreach (var item in minimapBoatIcons)
        {
            item.gameObject.SetActive(false);
        }

		foreach ( OtherBoatInfo boatInfo in Boats.Instance.getBoats ) {

            PlaceOtherBoatIcon(boatInfo);

            //++boatIndexInRange;

			/*if ( boatInfo.coords <= Boats.playerBoatInfo.coords + (currentShipRange+1) && boatInfo.coords >= Boats.playerBoatInfo.coords - (currentShipRange+1) ) {

				PlaceOtherBoatIcon (boatInfo,boatIndexInRange);

				++boatIndexInRange;

			}*/
        }
	}

	void PlaceOtherBoatIcon (OtherBoatInfo boatInfo) {

        // if new boat appears, instantiate icon

        MinimapBoat targetMinimapBoat = minimapBoatIcons.Find( x => x.boatInfo.id == boatInfo.id );

		if (targetMinimapBoat == null ) {

            GameObject boatIcon = Instantiate(enemyBoatIconPrefab, enemyIconParent.transform);

            targetMinimapBoat = boatIcon.GetComponent<MinimapBoat>();

            targetMinimapBoat.rectTransform.localScale = Vector3.one;

            targetMinimapBoat.rectTransform.sizeDelta = minimapChunkScale;

            targetMinimapBoat.boatInfo = boatInfo;

            //targetMinimapBoat.GetComponentInChildren<Image>().color = boatInfo.color;

            minimapBoatIcons.Add(targetMinimapBoat);

            //Debug.Log("coudn't find boat for ship : ");

        }
        
        // boat approches
        if (boatInfo.coords.x <= Boats.playerBoatInfo.coords.x + (GetCurrentShipRange + 1) &&
            boatInfo.coords.x >= Boats.playerBoatInfo.coords.x - (GetCurrentShipRange + 1) &&
            boatInfo.coords.y <= Boats.playerBoatInfo.coords.y + (GetCurrentShipRange + 1) &&
            boatInfo.coords.y >= Boats.playerBoatInfo.coords.y - (GetCurrentShipRange + 1)
            )
        {

            Vector2 targetPos = GetIconPos(boatInfo.coords);

            if (boatInfo.coords.x <= Boats.playerBoatInfo.coords.x + GetCurrentShipRange &&
                boatInfo.coords.x >= Boats.playerBoatInfo.coords.x - GetCurrentShipRange &&
                boatInfo.coords.y <= Boats.playerBoatInfo.coords.y + GetCurrentShipRange &&
                boatInfo.coords.y >= Boats.playerBoatInfo.coords.y - GetCurrentShipRange
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
	public delegate void OnZoom ();
	public static OnZoom onZoom;
	public void Zoom ()
	{
		Transitions.Instance.ScreenTransition.FadeIn (zoomDuration/2f);

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

//		HOTween.To (outlineImage, zoomDuration /2f , "color" , Color.clear );
		outlineImage.gameObject.SetActive(false);

        //zoomBackground.SetActive(true);


        rayBlockerImage.gameObject.SetActive(true);
		rayBlockerImage.color = Color.black;

		viewPortMask.enabled = false;
//		viewPortRectTransform.
//		HOTween.To (rayBlockerImage, zoomDuration, "color", c, false , EaseType.Linear , zoomDuration);

		Transitions.Instance.ScreenTransition.FadeOut (zoomDuration/2f);

		ClampScrollView ();

		if (onZoom != null)
			onZoom ();
	}

	bool unzooming = false;

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

        //zoomBackground.SetActive(false);

        viewPortMask.enabled = true;

		ClampScrollView ();

		outlineImage.gameObject.SetActive(true);
		Transitions.Instance.ScreenTransition.FadeOut (zoomDuration/2f);
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
