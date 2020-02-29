using UnityEngine;
using System.Collections;

public class SetOrder : MonoBehaviour {

	Transform getTransform;

	private int orderMin = -100;
	private int orderMax = 0;

	private int worldMin = -200;
	private int worldMax = 200;

	public int factor = 5990;

	public int start = 2;

	private int screenHeight = 0;

	void Start () {
		getTransform = GetComponent<Transform> ();

		screenHeight = (int)Screen.height / 2;
	}
	
	// Update is called once per frame
	void LateUpdate () {

		Vector3 pos = getTransform.localPosition;

		int posY = (int)(screenHeight/2f) + ( (int)pos.y + screenHeight);

		pos.z = ( factor * posY ) / (screenHeight * 2);

		transform.localPosition = pos;
	}
}
