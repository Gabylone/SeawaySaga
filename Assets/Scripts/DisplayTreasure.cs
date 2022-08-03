using System.Collections;
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
        MessageDisplay.Instance.onValidate -= CloseTreasure;

        animator.SetTrigger("close");

        SoundManager.Instance.PlaySound("Close Chest");

        animator.transform.DOMove( animator.transform.position - Vector3.up * 15f , 1f );

        Invoke("EndGame", 1f);
    }

    void ShowMessage()
    {
        MessageDisplay.Instance.onValidate += CloseTreasure;

        string currentMapKey = "map_data" + MapGenerator.mapParameters.id;
        //string nextMapKey = "map_data" + (MapGenerator.mapParameters.id + 1);

        //string currentMap_data = PlayerPrefs.GetString(currentMapKey, "locked");
        //string nextMap_Data = PlayerPrefs.GetString(nextMapKey, "locked");

        PlayerPrefs.SetString(currentMapKey, "finished");

        if (MapGenerator.mapParameters.id == 3)
        {
            MessageDisplay.Instance.Display("Well done ! You finished the game");
            MessageDisplay.Instance.HideCancelButton();

        }
        else
        {
            //Debug.Log("next map KEY is : " + nextMapKey);
            //Debug.Log("next map data is : " + nextMap_Data);

            MessageDisplay.Instance.Display(
                "Congratulations on finishing the first story! " +
                "Thanks so much for playing the game, if you want to support us, " +
                "you can buy one of the other stories " +
                "with a bigger world and other exiting quests!");

            MessageDisplay.Instance.HideCancelButton();

            /*if (nextMap_Data == "locked")
            {
                PlayerPrefs.SetString(nextMapKey, "unlocked");

                MessageDisplay.Instance.Display("You unlocked the next story !");
                MessageDisplay.Instance.HideCancelButton();

            }
            else
            {
                MessageDisplay.Instance.Display("The next story is already unlocked");
                MessageDisplay.Instance.HideCancelButton();
            }*/


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
