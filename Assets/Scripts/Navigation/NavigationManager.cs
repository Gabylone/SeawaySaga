using UnityEngine;
using UnityEngine.UI;

public enum Directions {
	North,
	NorthEast,
	East,
	SouthEast,
	South,
	SouthWest,
	West,
	NorthWest,
	None
}

public class NavigationManager : MonoBehaviour {

		// singleton
	public static NavigationManager Instance;

	[SerializeField]
	private GameObject navigationTriggers;

	[SerializeField]
	private Transform[] anchors;

	public delegate void OnMoveToChunk ();
	public OnMoveToChunk tutorial_OnMoveChunk;

    public delegate void OnUpdateChunk();
    public OnUpdateChunk onUpdateCurrentChunk;

    public int chunksTravelled = 0;

    public float minX = -40f;
    public float maxX = 40f;
    public float minY = -40f;
    public float maxY = 40f;

    public Vector2 GetRandomIslandPosition()
    {
        return new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
    }

    void Awake () {
		Instance = this;
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            ChangeChunk(Directions.South);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            ChangeChunk(Directions.North);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeChunk(Directions.West);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeChunk(Directions.East);
        }
    }

    #region movement
    public void ChangeChunk(Coords coords, int trips)
    {
        Boats.Instance.playerBoatInfo.SetCoords(coords);

        bool somebodyAteSomething = false;

        string[] memberFoods = new string[4] { "" , "" , "" , ""};

        for (int i = 0; i < trips; i++)
        {
            Crews.playerCrew.AddToStates();

            // feeding member while travelling

            Loot loot = LootManager.Instance.PlayerLoot;


            int memberIndex = 0;
            foreach (var crewMember in Crews.playerCrew.CrewMembers)
            {
                if (crewMember.CurrentHunger == crewMember.MaxHunger)
                {
                    if (loot.AllItems[0].Count == 0)
                    {
                        break;
                    }

                    Item foodItem = loot.AllItems[0][0];

                    string foodName = foodItem.englishName;
                    switch (foodItem.spriteID)
                    {
                        case 0:
                            foodName = "<color=red>" + foodItem.englishName + "</color>";
                            break;
                        case 1:
                            foodName = "<color=blue>" + foodItem.englishName + "</color>";
                            break;
                        case 2:
                            foodName = "<color=green>" + foodItem.englishName + "</color>";
                            break;
                        default:
                            break;
                    }

                    if (memberFoods[memberIndex].Length > 2)
                    {
                        memberFoods[memberIndex] += ", some " + foodName;
                    }
                    else
                    {
                        memberFoods[memberIndex] += "<b>" + crewMember.MemberName + "</b> ate some " + foodName;
                    }

                    somebodyAteSomething = true;
                    InGameMenu.Instance.EatItem(foodItem, crewMember);

                }

                ++memberIndex;
            }

            if ( somebodyAteSomething)
            {
                Chunk chunk = Chunk.GetChunk(MinimapChunk.currentMinimapChunk.coords);
                string IslandName = chunk.GetIslandData(MinimapChunk.currentMinimapChunk.index).storyManager.storyHandlers[0].Story.displayName;

                string str = "On the way to the " + IslandName + "\n";

                foreach (var item in memberFoods)
                {
                    if ( item.Length > 2)
                    {
                        str += item + "\n";
                    }

                }

                Narrator.Instance.ShowNarrator(str);
            }

            chunksTravelled++;
            TimeManager.Instance.NextHour();
            Boats.Instance.HandleOnMoveToChunk();
        }

        MinimapChunk.currentMinimapChunk.Deselect();

        SaveManager.Instance.SaveGameData();
        Boats.Instance.SaveBoats();

        ClockUI.Instance.UpdateUI();

        UpdateCurrentChunk();

    }
    public void ChangeChunk ( Directions newDirection ) {

		Boats.Instance.playerBoatInfo.Move (newDirection);

        chunksTravelled++;

        TimeManager.Instance.NextHour();
        ClockUI.Instance.UpdateUI();

        Crews.playerCrew.InvokeAddToStates();

        CallTutorial();

        Boats.Instance.HandleOnMoveToChunk();

        SaveManager.Instance.SaveGameData();
        Boats.Instance.SaveBoats();

        UpdateCurrentChunk();
    }

    void CallTutorial()
    {
        if (tutorial_OnMoveChunk != null)
        {
            tutorial_OnMoveChunk();
        }
    }

    public void UpdateCurrentChunk()
    {
        if ( onUpdateCurrentChunk != null)
        {
            onUpdateCurrentChunk();
        }

        Boats.Instance.HandleOnUpdateCurrentChunk();

        DisplayMinimap.Instance.HandleChunkEvent();

    }
    #endregion

    public Item[] GetTripsFood(int trips)
    {
        Loot loot = LootManager.Instance.PlayerLoot;

        int memberIndex = 0;
        foreach (var crewMember in Crews.playerCrew.CrewMembers)
        {
            int tmpHunger = crewMember.CurrentHunger;

            for (int i = 0; i < trips; i++)
            {
                if (tmpHunger >= crewMember.MaxHunger)
                {
                    if (loot.AllItems[0].Count == 0)
                    {
                        break;
                    }

                    Item foodItem = loot.AllItems[0][0];

                    tmpHunger -= foodItem.value;
                }

            }

            ++memberIndex;
        }


        // j'ai pas réussi c'était trop compliqué 
        return null;
        
    }

    #region tools
    public Directions getDirectionFromVector ( Vector2 dir ) {

		for (int i = 0; i < 8; ++i ) {
			if ( Vector2.Angle ( NavigationManager.Instance.getDir( (Directions)i ) , dir ) < 22f ) {
				return (Directions)i;
			}
		}
		return Directions.None;
	}
	public Directions getDirectionToPoint ( Vector2 point ) {

		Vector2 direction = point - (Vector2)Boats.Instance.playerBoatInfo.coords;

		for (int i = 0; i < 8; ++i ) {
			if ( Vector2.Angle ( direction , NavigationManager.Instance.getDir((Directions)i) ) < 45f ) {
				return (Directions)i;
			}

		}

		Debug.Log ("coun't find a direction");
		return Directions.None;
	}
	public string getDirName ( Directions dir ) {

		switch (dir) {
		case Directions.North:
			return "north";
		case Directions.NorthEast:
			return "north east";
		case Directions.East:
			return "east";
		case Directions.SouthEast:
			return "south east";
		case Directions.South:
			return "south";
		case Directions.SouthWest:
			return "south west";
		case Directions.West:
			return "west";
		case Directions.NorthWest:
			return "north west";
		case Directions.None:
			return "nowhere";
		}

		return "nulle part";

	}
	public Coords getNewCoords ( Directions dir ) {

		switch (dir) {
		case Directions.North:
			return new Coords ( 0 , 1 );
		case Directions.NorthEast:
			return new Coords ( 1 , 1 );
		case Directions.East:
			return new Coords ( 1 , 0 );
		case Directions.SouthEast:
			return new Coords ( 1 , -1 );
		case Directions.South:
			return new Coords ( 0 , -1 );
		case Directions.SouthWest:
			return new Coords ( -1 , -1 );
		case Directions.West:
			return new Coords ( -1 , 0 );
		case Directions.NorthWest:
			return new Coords ( -1 , 1 );
		case Directions.None:
			return new Coords ( 0 , 0 );
		}

		return new Coords ();

	}
	public Vector2 getDir ( Directions dir ) {

		switch (dir) {
		case Directions.North:
			return new Vector2 (0, 1);
		case Directions.NorthEast:
			return new Vector2 (1, 1);
		case Directions.East:
			return new Vector2 (1, 0);
		case Directions.SouthEast:
			return new Vector2 (1, -1);
		case Directions.South:
			return new Vector2 (0, -1);
		case Directions.SouthWest:
			return new Vector2 (-1, -1);
		case Directions.West:
			return new Vector2 (-1, 0);
		case Directions.NorthWest:
			return new Vector2 (-1, 1);
		case Directions.None:
			return new Vector2 (0, 0);
		}

		return Vector2.zero;

	}
    public Transform GetAnchor(Directions direction)
    {
        return anchors[(int)direction];
    }
    public LayerMask layerMask;
    public Vector3 GetOppositeCornerPosition(Directions dir)
    {
        return GetCornerPosition(GetOppositeDirection(dir));
    }
    public Vector3 GetCornerPosition(Directions dir)
    {
        return anchors[(int)dir].transform.position;
    }

    #endregion

    #region properties
    public GameObject NavigationTriggers {
		get {
			return navigationTriggers;
		}
	}
	#endregion

	public Transform[] Anchors {
		get {
			return anchors;
		}
	}

	public static Coords CurrentCoords {
		get {
			return Boats.Instance.playerBoatInfo.coords;
		}
	}

	public static Directions GetOppositeDirection ( Directions direction ) {

		switch (direction) {
		case Directions.North:
			return Directions.South;
		case Directions.NorthEast:
			return Directions.SouthWest;
		case Directions.East:
			return Directions.West;
		case Directions.SouthEast:
			return Directions.NorthWest;
		case Directions.South:
			return Directions.North;
		case Directions.SouthWest:
			return Directions.NorthEast;
		case Directions.West:
			return Directions.East;
		case Directions.NorthWest:
			return Directions.SouthEast;
		case Directions.None:
			return Directions.North;
		default:
			return Directions.None;
		}

	}

}

[System.Serializable]
public struct Coords {
	
	public int x;
	public int y;

	public Coords (int x,int y) {
		this.x = x;
		this.y = y;
	}

	public static Coords previous {
		get {
			return Boats.Instance.playerBoatInfo.previousCoords;
		}
	}

	public static Coords current {
		get {
			return Boats.Instance.playerBoatInfo.coords;
		}
	}

	public static Coords random {
		get {
			return MapGenerator.Instance.RandomCoords;
		}
	}

	public static Coords Zero {
		get {
			return new Coords (0, 0);
		}
	}

	public bool OutOfMap ()
	{
		return x < 0 || x >= MapGenerator.Instance.GetMapHorizontalScale || y < 0 || y >= MapGenerator.Instance.GetMapVerticalScale;
	}

	// overrides
		// == !=
	public static bool operator ==( Coords c1, Coords c2) 
	{
		return c1.x == c2.x && c1.y == c2.y;
	}
	public static bool operator != (Coords c1, Coords c2) 
	{
		return !(c1 == c2);
	}

		// < >
	public static bool operator < (Coords c1, Coords c2) 
	{
		return c1.x < c2.x && c1.y < c2.y;
	}
	public static bool operator > (Coords c1, Coords c2) 
	{
		return c1.x > c2.x && c1.y > c2.y;
	}
	public static bool operator < (Coords c1, int i) 
	{
		return c1.x < i || c1.y < i;
	}
	public static bool operator > (Coords c1, int i) 
	{
		return c1.x > i || c1.y > i;
	}

		// >= <=
	public static bool operator >= (Coords c1, Coords c2) 
	{
		return c1.x >= c2.x && c1.y >= c2.y;
	}
	public static bool operator <= (Coords c1, Coords c2) 
	{
		return c1.x <= c2.x && c1.y <= c2.y;
	}
	public static bool operator >= (Coords c1, int i) 
	{
		return c1.x >= i || c1.y >= i;
	}
	public static bool operator <= (Coords c1, int i) 
	{
		return c1.x <= i || c1.y <= i;
	}

		// + -
	public static Coords operator +(Coords c1, Coords c2) 
	{
		return new Coords ( c1.x + c2.x , c1.y + c2.y );
	}
	public static Coords operator -(Coords c1, Coords c2) 
	{
		return new Coords ( c1.x - c2.x , c1.y - c2.y );
	}
	public static Coords operator +(Coords c1, int i) 
	{
		return new Coords ( c1.x + i, c1.y + i );
	}
	public static Coords operator -(Coords c1, int i) 
	{
		return new Coords ( c1.x - i, c1.y - i );
	}

		// vector2 cast

	public static explicit operator Coords(Vector2 v)  // explicit byte to digit conversion operator
	{
		return new Coords ( (int)v.x , (int)v.y );
	}
	public static explicit operator Vector2(Coords c)  // explicit byte to digit conversion operator
	{
		return new Vector2 (c.x, c.y);
	}
//
//		// direction cast
//	public static explicit operator Directions(Coords c)  // explicit byte to digit conversion operator
//	{
//		return new Directions (c.x, c.y);
//	}
	public static explicit operator Coords(Directions dir)  // explicit byte to digit conversion operator
	{
		switch (dir) {
		case Directions.North:
			return new Coords ( 0 , 1 );
		case Directions.NorthEast:
			return new Coords ( 1 , 1 );
		case Directions.East:
			return new Coords ( 1 , 0 );
		case Directions.SouthEast:
			return new Coords ( 1 , -1 );
		case Directions.South:
			return new Coords ( 0 , -1 );
		case Directions.SouthWest:
			return new Coords ( -1 , -1 );
		case Directions.West:
			return new Coords ( -1 , 0 );
		case Directions.NorthWest:
			return new Coords ( -1 , 1 );
		case Directions.None:
			return new Coords ( 0 , 0 );
		}

		return new Coords ();
	}

		// string
	public override string ToString()
	{
		return "X : " + x + " / Y : " + y;
	}

	public static Coords GetClosest ( Coords originCoords ) {
		
		int radius = 1;

		while ( radius < MapGenerator.Instance.GetMapHorizontalScale && radius < MapGenerator.Instance.GetMapVerticalScale) {

			for (int x = -radius; x < radius; x++) {
				for (int y = -radius; y < radius; y++) {

					if (x == 0 && y == 0)
						continue;

					Coords coords = new Coords (originCoords.x + x, originCoords.y + y);

					if (coords.x >= MapGenerator.Instance.GetMapHorizontalScale|| coords.y >= MapGenerator.Instance.GetMapVerticalScale|| coords <= 0) {
						continue;
					}

                    Chunk chunk = Chunk.GetChunk (coords);

					if (chunk.HasIslands()) {
						return coords;
					}


				}
			}

			++radius;

			if (radius > 10) {
				Debug.Log ("Get closest island reached 10 : breaking");
				break;
			}

		}

		Debug.Log ("could not find closest island, returning current");

		return current;

	}
}
