using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCenterFeedback : MonoBehaviour {

	public GameObject group;

	public float displayDuration = 1f;

	// Use this for initialization
	void Start () {

        Quest.showQuestOnMap += HandleShowQuestOnMap;

		GetComponent<RectTransform> ().sizeDelta = Vector2.one * DisplayMinimap.Instance.minimapChunkScale;

		Hide ();
	}

	void HandleShowQuestOnMap (Quest quest)
	{
		HandleOnCenterMap (quest.targetCoords);
	}

	void HandleOnCenterMap (Coords coords)
	{
		GetComponent<RectTransform> ().anchoredPosition = DisplayMinimap.Instance.getPosFromCoords (coords);

		Tween.Bounce ( transform );

		CancelInvoke ("Hide");

		Show ();

		Invoke ("Hide",displayDuration);
	}

	void Show () {
		group.SetActive (true);
	}

	void Hide () {
		group.SetActive (false);
	}
}
