using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCenterFeedback : MonoBehaviour {

    public static MinimapCenterFeedback Instance;

	public GameObject group;

    public float displayDuration = 1f;

    RectTransform rectTransform;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        

        rectTransform = GetComponent<RectTransform>();

		Hide ();
	}

	void HandleShowQuestOnMap (Quest quest)
	{
		
	}

	public void CenterOnMap (Coords coords)
	{
		rectTransform.anchoredPosition = DisplayMinimap.Instance.getPosFromCoords (coords);

		Tween.Bounce ( transform );

		Show ();

		CancelInvoke ("Hide");
		Invoke ("Hide",displayDuration);
    }

	void Show () {
		group.SetActive (true);
	}

	void Hide () {
		group.SetActive (false);
	}
}
