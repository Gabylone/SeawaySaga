using UnityEngine;
using System.Collections;

public class StickToRectTransform : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<BoxCollider2D> ().size = new Vector2(GetComponent<RectTransform> ().rect.width,GetComponent<RectTransform> ().rect.height);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
