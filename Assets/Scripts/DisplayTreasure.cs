﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

using UnityEngine.SceneManagement;

public class DisplayTreasure : MonoBehaviour {

    public Animator animator;

    public GameObject group;

    public GameObject pearlPrefab;

    public Transform pearlAppearAnchor;

    public float showPearlsDelay = 1f;
    public int pearlAmount = 10;

    public float pearlDuration = 0.2f;
    public float rangeX = 0f;
    public float rangeY = 0f;

    public float halfWayDecal = 1f;

    bool opened = false;
    public bool canInteract = false;

    public Transform pearlDestination;

	// Use this for initialization
	void Start () {
        StoryFunctions.Instance.getFunction += HandleOnGetFunction;

        group.SetActive(false);
	}

    private void HandleOnGetFunction(FunctionType func, string cellParameters)
    {
        if (func == FunctionType.EndMap)
        {
            ShowTreasure();
        }
    }

    private void ShowTreasure()
    {
        group.SetActive(true);

        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlaySound("Big Tap");
        SoundManager.Instance.PlaySound("Mystick Tap");

        Invoke("ShowTreasureDelay", 1f);
    }

    void ShowTreasureDelay()
    {
        canInteract = true;
    }

    public void OnPointerClick()
    {
        if (!canInteract)
        {
            return;
        }

        OpenChest();

        if (opened)
        {

        }
        else
        {

        }
    }

    public void OpenChest()
    {
        canInteract = false;

        animator.SetTrigger("open");

        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlaySound("Big Tap");
        SoundManager.Instance.PlaySound("Mystick Tap");
        SoundManager.Instance.PlaySound("Open Chest");

        CancelInvoke("ShowMessage");
        Invoke("ShowMessage", 1f);
    }

    public void CloseTreasure()
    {
        animator.SetTrigger("close");

        SoundManager.Instance.PlaySound("Close Chest");

        animator.transform.DOMove( animator.transform.position - Vector3.up * 15f , 1f );

        Invoke("EndGame", 1f);
    }

    void ShowMessage()
    {
        MessageDisplay.Instance.onValidate += CloseTreasure;

        if (MapGenerator.mapParameters.id == 3)
        {
            MessageDisplay.Instance.Show("Well done ! You finished the game");
            MessageDisplay.Instance.HideCancelButton();

        }
        else
        {
            if (CrewCreator.Instance.GetApparenceItem(ApparenceType.map, MapGenerator.mapParameters.id + 1).locked)
            {
                CrewCreator.Instance.GetApparenceItem(ApparenceType.map, MapGenerator.mapParameters.id).finished = true;

                PlayerInfo.Instance.AddApparenceItem(CrewCreator.Instance.GetApparenceItem(ApparenceType.map, MapGenerator.mapParameters.id));
                PlayerInfo.Instance.AddApparenceItem(CrewCreator.Instance.GetApparenceItem(ApparenceType.map, MapGenerator.mapParameters.id + 1));

                MessageDisplay.Instance.Show("You unlocked the next story !");
                MessageDisplay.Instance.HideCancelButton();

            }
            else
            {
                MessageDisplay.Instance.Show("The next story is already unlocked");
                MessageDisplay.Instance.HideCancelButton();
            }

        }

        PlayerInfo.Instance.Save();
    }

    void EndGame()
    {
        Transitions.Instance.ScreenTransition.FadeIn(1f);

        Invoke("EndGameDelay", 1f);
    }

    void EndGameDelay()
    {
        SceneManager.LoadScene("Menu");
    }
}
