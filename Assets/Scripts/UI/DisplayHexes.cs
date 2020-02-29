using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHexes : MonoBehaviour {

	public GameObject hexPrefab;

	int scale = 10;

	float hexHeight;

	float hexDecal = 432f;

	// Use this for initialization
	void Start () {

		hexHeight = hexPrefab.GetComponent<Image> ().rectTransform.rect.height;

		for (int x = -scale; x < scale; x++) {
			
			for (int y = -scale; y < scale; y++) {

				PlaceHex (new Coords (x, y));


			}

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

		pos = new Vector3 ( c.x * hexDecal ,yPos , 0f);

		hexObj.transform.localPosition = pos;

		hexObj.GetComponentInChildren<Text> ().text = "" +
			"X : " + c.x + "\n" +
			"Y : " + c.y;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
