using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class RayBlocker : MonoBehaviour {

    public static RayBlocker Instance;

	public delegate void OnTouchRayBlocker ();
	public static OnTouchRayBlocker onTouchRayBlocker;

	public GameObject group;
	public Image image;

	public float tweenDur = 1f;
	Color initColor;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        onTouchRayBlocker = null;

		initColor = image.color;

		Hide ();

		InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		InGameMenu.Instance.onCloseMenu += HandleCloseInventory;

    }

    public void OnPointerDown () {
		if (onTouchRayBlocker != null)
			onTouchRayBlocker ();
	}

	void HandleCloseInventory ()
	{
//		HOTween.To ( image , tweenDur , "color" , Color.clear );
//
//		Invoke ("Hide" , tweenDur);

		Hide ();
	}

	void HandleOpenInventory ()
	{
//		HOTween.To ( image , tweenDur , "color" , initColor );
//
//		Show ();

		Show ();
	}

	public void Hide ()
	{
		group.SetActive (false);
	}
	public void Show () {
		group.SetActive (true);
	}
}
