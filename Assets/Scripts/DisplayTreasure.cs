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

        string[] strs = new string[4]
            {
                "Congratulations on finishing the first story, and thanks so much for playing the game!\nIf you want to support us, you can buy the other stories. Each one contains a bigger map full of more challenging and exciting adventures.\nTry out the Archipelago next!",
                "Great job, you completed the Archipelago!\nIf you’re looking for something even more challenging now, you can try out the next story, the Long Journey!\nJust make sure to bring enough resources to sail through the boundless and deadly sea!",
                "Congratulations on surviving the perilous Long Journey!\nNow the last and most difficult challenge awaits, the Labyrinth!\nUncover the tangled secrets of this fabled place in the final arduous and enigmatic story.\nGood luck!",
                "Huzzah! You did it, you finished Seaway Saga!\nThis means a lot to us and we’re extremely grateful that you bought and played through the other stories.\nThank you so much, and congratulations!\nGabriel & Romain"
            };

        string str = strs[MapGenerator.mapParameters.id];

        MessageDisplay.Instance.Display(str);

        MessageDisplay.Instance.HideCancelButton();

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
