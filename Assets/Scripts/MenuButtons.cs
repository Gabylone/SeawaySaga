﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuButtons : MonoBehaviour {

	public GameObject group;
	public GameObject[] buttonObjs;
	public float tweenDuration = 1f;
	public float timeBetweenDisplay = 0.15f;
	public float timeBetweenDisplay_Hide = 0.03f;
	public RectTransform rectTransform;

    public CanvasGroup canvasGroup;

	public bool opened = false;

	void Start () {
        Show();

        CombatManager.Instance.onFightStart += Hide;
        //CombatManager.Instance.onFightEnd += Show;
    }

    public void Hide()
    {
        opened = false;

        canvasGroup.DOFade(0f, tweenDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay" , tweenDuration);
    }

    void HideDelay()
    {
        group.SetActive(false);
    }

	public void Show ()
	{
		opened = true;
		group.SetActive (true);

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, tweenDuration);

        CancelInvoke("HideDelay");

    }
}
