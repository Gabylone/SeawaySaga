//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using System.Collections.Generic;
//using DG.Tweening;
//
//public class MapImage : MonoBehaviour {
//
//	public static MapImage Instance;
//
//	[Header("General")]
//	[SerializeField]
//	private Image targetImage;
//	[SerializeField]
//	private bool revealMap = false;
//	[SerializeField]
//	[Range(0,1)]
//	private float mapTransparency = 0.5f;
//
//	[Header("Colors")]
//	[SerializeField]
//	private Color discoveredIsland_Color;
//	[SerializeField]
//	private Color visitedIsland_Color;
//	[SerializeField]
//	private Color discoveredSea_Color;
//	[SerializeField]
//	private Color undiscoveredSea_Color;
//	[SerializeField]
//	private Color boatPositionColor;
//
//	[Header("UI Elements")]
//	[SerializeField]
//	private GameObject openButton_Obj;
//	[SerializeField]
//	private GameObject mapGroup;
//	[SerializeField]
//	private GameObject closeButton_Obj;
//
//	private bool opened = false;
//
//	[SerializeField]
//	private float maxImagePosition = 250f;
//
//	[SerializeField]
//	private int pixelFactor = 2;
//
//	[Header ("Center On Boat")]
//	[SerializeField]
//	private RectTransform mapTransform;
//	[SerializeField]
//	private RectTransform contentTransform;
//	[SerializeField]
//	private float maxContentPosition = 150f;
//
//	[Header("Island Buttons")]
//	// islandButtons
//	[SerializeField]
//	private GameObject islandButtonPrefab;
//	private float islandButtonFactor = 0f;
//	private Dictionary<Coords,IslandButton> islandButtons = new Dictionary<Coords, IslandButton>();
//
//	void Awake() {
//		Instance = this;
//	}
//
//	void Update () {
//		if (timer >= 0) {
//			timer -= Time.deltaTime;
//		}
//	}
//	#region initialization
//	public void Init () {
//		NavigationManager.Instance.EnterNewChunk += UpdateBoatSurroundings;
//		PlayerLoot.Instance.openInventory += HandleOpenInventory;;
//		CombatManager.Instance.fightStarting += HideButton;
//		CombatManager.Instance.fightEnding += ShowButton;
//		StoryInput.onPressInput += HandleOnPressInput;
//	}
//
//	void HandleOnPressInput ()
//	{
//		Close ();
//	}
//
//	void HandleOpenInventory (CrewMember member)
//	{
//		Close ();
//	}
//
//	public void Reset ()
//	{
//
//	}
//
//	public void InitImage () {
//
//		islandButtons.Clear ();
//
//		islandButtonFactor = targetImage.GetComponent<RectTransform>().rect.width / (float)MapGenerator.Instance.MapScale;
//
//		Texture2D texture = new Texture2D (MapGenerator.Instance.MapScale, MapGenerator.Instance.MapScale);
//		//
//		int clueIndex = 0;
//
//		for ( int x = 0; x < MapGenerator.Instance.MapScale; ++x ) {
//
//			for (int y = 0; y < MapGenerator.Instance.MapScale; ++y ) {
//
//				Coords c = new Coords (x, y);
//
//				Chunk chunk = MapGenerator.Instance.GetChunk(c);
//
//				SetPixel (texture,c, revealMap ? getChunkColor_Reveal (chunk) : getChunkColor (chunk));
//
//				if (chunk.State == ChunkState.UndiscoveredIsland
//					|| chunk.State == ChunkState.VisitedIsland
//					|| chunk.State == ChunkState.DiscoveredIsland) {
//
//					CreateIslandButton (new Coords (x,y));
//
//				}
//
//			}
//
//		}
//
//		UpdateTexture (texture);
//
//		OverallMapOpened = false;
//
//		UpdateBoatSurroundings ();
//	}
//	#endregion
//
//	#region island button
//	void CreateIslandButton ( Coords c) {
//
//		GameObject button = Instantiate (islandButtonPrefab, targetImage.transform) as GameObject;
//
//		button.transform.localScale = Vector3.one;
//
//		button.GetComponent<RectTransform>().sizeDelta = Vector2.one * islandButtonFactor;
//
//		float w = targetImage.rectTransform.rect.width/2f;
//
//		float pX = (-w) + (islandButtonFactor / 2f) + (c.x * islandButtonFactor);
//		float pY = (-w) + (islandButtonFactor / 2f) + (c.y * islandButtonFactor);
//
//		button.GetComponent<RectTransform>().localPosition = new Vector2 (pX,pY);
//
//		button.GetComponent<IslandButton> ().Init (c);
//
//		if (islandButtons.ContainsKey (c)) {
//			return;
//		}
//
//		islandButtons.Add (c, button.GetComponent<IslandButton> ());
//	}
//	#endregion
//
//	#region update boat surroundings
//	public void UpdateBoatSurroundings () {
//
//		Texture2D texture = (Texture2D)targetImage.mainTexture;
//
//		int shipRange = Boats.Instance.PlayerBoatInfo.ShipRange;
//
//		int mapScale = MapGenerator.Instance.MapScale;
//
//		Chunk previousChunk = MapGenerator.Instance.GetChunk(Boats.Instance.PlayerBoatInfo.PreviousCoords);
//		SetPixel (texture,NavigationManager.PreviousCoords, getChunkColor (previousChunk));
//
//		for (int x = -shipRange; x <= shipRange; ++x ) {
//
//			for (int y = -shipRange; y <= shipRange; ++y ) {
//
//				Coords c = NavigationManager.CurrentCoords + new Coords (x, y);
//
//				if ( c.x < mapScale && c.x >= 0 &&
//					c.y < mapScale && c.y >= 0) {
//
//					Chunk chunk = MapGenerator.Instance.GetChunk(c);
//
//					Color color = Color.red;
//
//					switch (chunk.State) {
//					case ChunkState.UndiscoveredSea:
//						chunk.State = ChunkState.DiscoveredSea;
//						break;
//					case ChunkState.UndiscoveredIsland:
//						chunk.State = ChunkState.DiscoveredIsland;
//						break;
//					}
//
//					SetPixel (texture,c, getChunkColor (chunk));
//
//					if (islandButtons.ContainsKey(c))
//						islandButtons [c].Visible = true;
//
//
//				}
//
//			}
//
//		}
//
//		SetPixel (texture,NavigationManager.CurrentCoords, Color.red);
//
//		UpdateTexture (texture);
//
//		CheckForBoats ();
//
//	}
//
//	public void CheckForBoats ()
//	{
//		return;
//
//		Texture2D texture = (Texture2D)targetImage.mainTexture;
//
//		foreach ( OtherBoatInfo boatInfo in Boats.Instance.OtherBoatInfos ) {
//			if ( boatInfo.CurrentCoords <= NavigationManager.CurrentCoords + Boats.Instance.PlayerBoatInfo.ShipRange
//				&& boatInfo.CurrentCoords >= NavigationManager.CurrentCoords - Boats.Instance.PlayerBoatInfo.ShipRange ) {
//				SetPixel (texture,boatInfo.CurrentCoords, Color.green);
//			} else {
//				SetPixel (texture,boatInfo.PreviousCoords, getChunkColor(MapGenerator.Instance.GetChunk(boatInfo.PreviousCoords) ));
//				SetPixel (texture,boatInfo.CurrentCoords, getChunkColor(MapGenerator.Instance.GetChunk(boatInfo.CurrentCoords) ));
//			}
//		}
//
//		UpdateTexture (texture);
//	}
//
//	public void UpdatePixel ( Coords coords , Color color )
//	{
//		Texture2D texture = (Texture2D)targetImage.mainTexture;
//
//		SetPixel (texture,coords.x,coords.y, color);
//
//		UpdateTexture (texture);
//	}
//
//	private Color getChunkColor_Reveal (Chunk chunk) {
//
//		switch (chunk.State) {
//		case ChunkState.UndiscoveredSea:
//			return discoveredSea_Color;
//			break;
//		case ChunkState.DiscoveredSea:
//			return discoveredSea_Color;
//			break;
//		case ChunkState.UndiscoveredIsland:
//			return discoveredIsland_Color;
//			break;
//		case ChunkState.DiscoveredIsland:
//			return discoveredIsland_Color;
//			break;
//		case ChunkState.VisitedIsland:
//			return visitedIsland_Color;
//			break;
//		default:
//			return Color.black;
//			break;
//		}
//	}
//	private Color getChunkColor (Chunk chunk) {
//
//		switch (chunk.State) {
//		case ChunkState.UndiscoveredSea:
//			return undiscoveredSea_Color;
//			break;
//		case ChunkState.DiscoveredSea:
//			return discoveredSea_Color;
//			break;
//		case ChunkState.UndiscoveredIsland:
//			return undiscoveredSea_Color;
//			break;
//		case ChunkState.DiscoveredIsland:
//			return discoveredIsland_Color;
//			break;
//		case ChunkState.VisitedIsland:
//			return visitedIsland_Color;
//			break;
//		default:
//			return Color.black;
//			break;
//		}
//	}
//	#endregion
//
//	#region image
//	private void SetPixel (Texture2D text, int x,int y,Color c) {
//
//		SetPixel (text, new Coords (x, y), c);
//	}
//	private void SetPixel (Coords coords) {
//		Texture2D texture = (Texture2D)targetImage.mainTexture;
//		SetPixel (texture, coords, getChunkColor (MapGenerator.Instance.GetChunk(coords) ));
//		UpdateTexture (texture);
//	}
//	private void SetPixel (Texture2D text, Coords coords) {
//		SetPixel (text, coords, getChunkColor (MapGenerator.Instance.GetChunk(coords)));
//	}
//	private void SetPixel (Texture2D text, Coords coords,Color color) {
//		for (int iX = 0; iX < pixelFactor; iX++) {
//			for (int iY = 0; iY < pixelFactor; iY++) {
//				text.SetPixel ((pixelFactor*coords.x)+iX,(pixelFactor*coords.y)+iY, color);
//			}
//		}
//	}
//	private void UpdateTexture (Texture2D texture) {
//
//		texture.filterMode = FilterMode.Point;
//		texture.anisoLevel = 0;
//		texture.mipMapBias = 0;
//		texture.wrapMode = TextureWrapMode.Clamp;
//
//		texture.Apply ();
//
//		targetImage.sprite = Sprite.Create ( texture, new Rect (0, 0, MapGenerator.Instance.MapScale,  MapGenerator.Instance.MapScale) , Vector2.one * 0.5f );
//	}
//	public void CenterOnBoat () {
//		CenterOnCoords (NavigationManager.CurrentCoords);
//	}
//
//	public void CenterOnCoords (Coords coords) {
//
//		Vector2 pos = new Vector2 (coords.x , coords.y);
//
//		pos = (pos * maxContentPosition) / MapGenerator.Instance.MapScale;
//
//		pos -= Vector2.one * (maxContentPosition / 2);
//
//		pos = -pos;
//
//		contentTransform.localPosition = pos;
//	}
//
//	public void HighlightPixel ( Coords c ) {
//
//		SetPixel (c);
//		islandButtons [c].Highlight ();
//		CenterOnCoords (c);
//	}
//	#endregion
//
//	#region properties
//	public Image TargetImage {
//		get {
//			return targetImage;
//		}
//	}
//	public delegate void ShowIslandInfo ( string info, Vector2 p );
//	public ShowIslandInfo showIslandInfo;
//	#endregion
//
//	#region open / Close
//	public void OpenMap () {
//
//		mapGroup.SetActive (true);
//
//		closeButton_Obj.SetActive (true);
//
//		HideButton ();
//
//		Tween.Bounce ( mapGroup.transform , 0.2f , 1.05f );
//
//		PlayerLoot.Instance.HideInventory ();
//
//		CenterOnBoat ();
//	}
//
//	public void Close () {
//
//		mapGroup.SetActive (false);
//
//		closeButton_Obj.SetActive (false);
//
//		ShowButton ();
//	}
//
//	void HideButton () {
//		openButton_Obj.SetActive (false);
//		//
//	}
//
//	void ShowButton () {
//		openButton_Obj.SetActive (true);
//		//
//	}
//	#endregion
//
//	#region touch to close
//	float timeToClose = 0.15f;
//	bool closeOnTouch = false;
//	public void OnPointerDown() {
//		closeOnTouch = true;
//		Invoke ("OnPointerDownDelay",timeToClose);
//	}
//	public void OnPointerDownDelay () {
//		closeOnTouch = false;
//	}
//	public void OnPointerUp () {
//		if (closeOnTouch) {
//			Close ();
//			closeOnTouch = false;
//		}
//	}
//	#endregion
//
//	#region overall map
//	float timer = 0f;
//	float duration = 1f;
//	bool overallMapOpened = false;
//
//	Vector2 initPos = Vector2.zero;
//	float initScale = 0f;
//
//	public bool OverallMapOpened {
//		get {
//			return overallMapOpened;
//		}
//		set {
//
//			return;
//
//			if (overallMapOpened == value)
//				return;
//
//			overallMapOpened = value;
//
//
//			if ( value == true ) {
//
//				initPos = mapTransform.localPosition;
//				initScale = mapTransform.rect.width;
//
//				mapTransform.anchorMax = Vector2.one;
//				mapTransform.sizeDelta = Vector2.zero;
//
//				mapTransform.localPosition = Vector2.zero;
//
//			} else {
//
//				mapTransform.anchorMax = Vector2.zero;
//				mapTransform.sizeDelta = new Vector2 (initScale,initScale);
//
//				mapTransform.localPosition = initPos;
//
//			}
//
//
//
//		}
//	}
//	public void TouchMap () {
//		//}
//
//		timer = 0.2f;
//	}
//	public void Grow () {
//
//		if (OverallMapOpened == false) {
//
//			if (timer >= 0) {
//
//				OverallMapOpened = true;
//
//			}
//		} else {
//
//			OverallMapOpened = false;
//
//		}
//
//	}
//	#endregion
//}
//
