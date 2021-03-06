﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class IslandInfo : MonoBehaviour {

    public static IslandInfo Instance;

    public Transform _transform;

	[Header("Island Info")]
	[SerializeField]
	private GameObject obj;
	[SerializeField]
	private Text uiText;

	public float decal = 2f;

	bool displaying = false;

	Transform currentTransform;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        _transform = GetComponent<Transform>();

        Hide ();
	}

	void Update () 
	{
		if ( displaying ) {
			UpdatePosition ();
		}
	}

    public void DisplayBoatInfo(OtherBoatInfo boatInfo)
    {
        if (boatInfo.alreadyMet)
        {
            string storyName = boatInfo.storyManager.storyHandlers[0].Story.displayName;
            uiText.text = storyName;
        }
        else
        {
            uiText.text = "?";
        }
    }

	public void DisplayIslandInfo (string str)
	{

        uiText.text = str;
	}

    public void ShowAtTransform(Transform tr)
    {
        Show();

        Tween.Bounce(_transform, 0.2f, 1.05f);

        currentTransform = tr;
        UpdatePosition();

        CancelInvoke();
    }

	void UpdatePosition ()
	{
		_transform.position = currentTransform.position + Vector3.up * decal;

	}

	public void Show () {
		displaying = true;
		obj.SetActive (true);
	}
	public void Hide() {
		displaying = false;
		obj.SetActive (false);
	}
}
