﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class Narrator : MonoBehaviour {

	public static Narrator Instance;

	[Header("Narrator")]
	[SerializeField] private Text narratorText;
	[SerializeField] private GameObject narratorObj;
    [SerializeField] private GameObject narratorButtonObj;
    public CanvasGroup canvasGroup;
    public Transform _transform;

    public float scale = 1.01f;
    public float dur = 0.1f;

    public bool continueStory = false;

    VerticalLayoutGroup[] verticalLayoutGroups;
    RectTransform[] rectTransforms;

    private bool previousActive = false;

    public delegate void OnCloseNarrator();
    public OnCloseNarrator onCloseNarrator;

    public bool visible = false;

    void Awake () {
		Instance = this;
	}

	void Start () {

        _transform = GetComponent<Transform>();
        verticalLayoutGroups = GetComponentsInChildren<VerticalLayoutGroup>();
        rectTransforms = new RectTransform[verticalLayoutGroups.Length];
        for (int i = 0; i < verticalLayoutGroups.Length; i++)
        {
            rectTransforms[i] = verticalLayoutGroups[i].GetComponent<RectTransform>();
        }

        StoryFunctions.Instance.getFunction+= HandleGetFunction;

		InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		InGameMenu.Instance.onCloseMenu += HandleCloseInventory;

        HideNarrator();

	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		if ( func == FunctionType.Narrator)
        {
            ShowNarratorInput(cellParameters.Remove(0, 2));
        }
    }

	void HandleOpenInventory ()
	{
		if (narratorObj.activeSelf) {

			narratorObj.SetActive (false);

			previousActive = true;
		}
	}

	void HandleCloseInventory ()
	{
		if ( previousActive ) {

			narratorObj.SetActive (true);

			previousActive = false;	
		}
	}

	#region narrator
    public void ShowNarratorInput(string text)
    {
        ShowNarrator(text);
        continueStory = true;

        //StoryInput.Instance.WaitForInput();
    }
    public void ShowNarratorNoneStoryInput (string text)
    {
        CanHide = true;
        ShowNarrator(text);
    }
    public void ShowNarrator (string text) {

        Tween.Bounce(_transform, dur, scale);
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, dur);

        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        narratorButtonObj.SetActive(true);

        visible = true;

		narratorObj.SetActive (true);

		narratorText.text = NameGeneration.CheckForKeyWords (text);

        int i = 0;
        foreach (var layoutGroup in verticalLayoutGroups)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransforms[i]);
            ++i;
        }
	}
    public void HideNarrator()
    {
        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        narratorObj.SetActive(false);
        visible = false;

        if (onCloseNarrator != null)
        {
            onCloseNarrator();
        }

        if (continueStory)
        {
            continueStory = false;

            HideNarrator();
            StoryReader.Instance.ContinueStory();
        }

        /*canvasGroup.DOKill();
        canvasGroup.DOFade(0f, dur);

        CancelInvoke("HideNarratorDelay");
        Invoke("HideNarratorDelay", dur);*/
    }

    public void HideNarratorDelay()
    {
        

    }
    #endregion

    private bool canHide = false;

    public bool CanHide
    {
        get
        {
            return canHide;
        }

        set
        {
            canHide = value;
        }
    }

    public void TryHide()
    {
        HideNarrator();

        if (CanHide)
        {
            CanHide = false;
        }
    }
}
