using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySurroundingHexes : MonoBehaviour {

	public GameObject hexPrefab;

	public Transform hexGroup;

	List<DisplayHex> hexDisplays = new List<DisplayHex>();

	[Range(0,10)]
	public int range = 1;

	float hexHeight;

	float hexDecal = 432f;

	Coords[] surroundingCoords = new Coords[6] {
		new Coords ( 0  , 1),
		new Coords ( 1  , 0),
		new Coords ( 1  , -1),
		new Coords ( 0  , -1),
		new Coords ( - 1, -1),
		new Coords ( - 1, 0)
	};

	Coords[] test = new Coords[12] {
		//
		new Coords ( 0  , 2),
		new Coords ( 1  , 1),
		new Coords ( 2  , 1),
		new Coords ( 2  , 0),
		new Coords ( 2, -1),
		new Coords ( 1, -2),
		//
		new Coords ( 0  , -2),
		new Coords ( -1  , -2),
		new Coords ( -2  , -1),
		new Coords ( -2  , 0),
		new Coords ( -2, 1),
		new Coords ( - 1, 1)
	};

	// Use this for initialization
	void Start () {

		hexHeight = hexPrefab.GetComponent<Image> ().rectTransform.rect.height;

		UpdateBoatSurrounding ();

		NavigationManager.Instance.EnterNewChunk += UpdateBoatSurrounding;
	}

	void Clear ()
	{
		for (int i = 0; i < hexDisplays.Count; i++) {

			Destroy (hexDisplays [i].gameObject);


		}

		hexDisplays.Clear ();
	}

	void UpdateBoatSurrounding () {

		Clear ();

		PlaceHex (new Coords(0,0));

		//foreach (var coord in surroundingCoords) {
		//	PlaceHex (new Coords(coord.x,coord.y) );
		//}

		foreach (var coord in surroundingCoords) {
			PlaceHex (new Coords(coord.x,coord.y) );
		}

	}

	public void PlaceHex(Coords c) {

		// INST
		GameObject hexObj = Instantiate (hexPrefab, transform);

		// SCALE
		hexObj.transform.localScale = Vector3.one;

		// POS
		Vector3 pos;

		float yPos = hexHeight * c.y;
		if (c.x < 0) {
			if (-c.x % -2 == 1)
				yPos += hexHeight / 2f;
		} else {
			if (c.x % 2 == 1)
				yPos += hexHeight / 2f;
		}

		pos = new Vector3 ( c.x * hexDecal , - yPos , 0f);

		hexObj.transform.localPosition = pos;

		Coords worldCoords = Boats.playerBoatInfo.coords + new Coords(c.x,-c.y);

		hexObj.GetComponent<DisplayHex> ().UdpateHex (worldCoords);

		hexDisplays.Add(hexObj.AddComponent<DisplayHex>());

//		Boats.PlayerBoatInfo.currentCoords

		//if (hexObj.GetComponentInChildren<Text> () != null) {
		//	hexObj.GetComponentInChildren<Text> ().text = "" +
		//	"X : " + worldCoords.x + "\n" +
		//	"Y : " + worldCoords.y;
		//}
	}
}