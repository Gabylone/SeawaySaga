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

    public Transform pearlDestination;

    public DisplayPearls displayPearls;

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
    }

    public void OpenChest()
    {
        /*if (KeepOnLoad.Instance != null && KeepOnLoad.Instance.mapName != "")
        {
            pearlAmount = KeepOnLoad.Instance.price;
        }
        else
        {
            pearlAmount = 100;
        }

        animator.SetTrigger("open");

        displayPearls.transform.SetParent(this.transform);

        displayPearls.Show();

        Invoke("ShowPearls", showPearlsDelay );*/

        animator.SetTrigger("open");

        CancelInvoke("CloseTreasure");
        Invoke("CloseTreasure", 1f);
    }

    void CloseTreasure()
    {
        animator.SetTrigger("close");

        CancelInvoke("ShowMessage");
        Invoke("ShowMessage", 1f);

        animator.transform.DOMove( animator.transform.position - Vector3.up * 15f , 1f );
    }

    void ShowPearls()
    {
        StartCoroutine(ShowPearlsCoroutine());
    }

    IEnumerator ShowPearlsCoroutine()
    {
        int a = pearlAmount;
        int r = 20;

        while (a > 0)
        {

            for (int i = 0; i < r; i++)
            {
                GameObject pearl = Instantiate(pearlPrefab, pearlAppearAnchor) as GameObject;

                Vector3 p = new Vector3(Random.Range(-rangeX, rangeX), 0f, Random.Range(-rangeY, rangeY));

                pearl.GetComponent<RectTransform>().localPosition = p;

                Vector3 halfway = ( p + (pearlDestination.position - p) / 2f ) + Random.insideUnitSphere * halfWayDecal;

                pearl.transform.DOMove(halfway, pearlDuration);
                pearl.transform.DOMove(pearlDestination.position, pearlDuration).SetDelay(pearlDuration);
                yield return new WaitForEndOfFrame();

                PlayerInfo.Instance.AddPearl(r);


            }

            yield return new WaitForSeconds(pearlDuration);


            a -= r;
        }

        yield return new WaitForSeconds(showPearlsDelay);

        animator.SetTrigger("close");

        yield return new WaitForSeconds(showPearlsDelay);

        displayPearls.Hide();

        yield return new WaitForSeconds(pearlDuration);

        // add next map / or / finish game
        ShowMessage();

    }

    void ShowMessage()
    {
        MessageDisplay.Instance.onValidate += EndGame;

        if (MapGenerator.mapParameters.id == 4)
        {
            MessageDisplay.Instance.Show("Well done ! You finished the game");
        }
        else
        {
            if (CrewCreator.Instance.GetApparenceItem(ApparenceType.map, MapGenerator.mapParameters.id + 1).locked)
            {
                CrewCreator.Instance.GetApparenceItem(ApparenceType.map, MapGenerator.mapParameters.id + 1).finished = true;
                PlayerInfo.Instance.AddApparenceItem(CrewCreator.Instance.GetApparenceItem(ApparenceType.map, MapGenerator.mapParameters.id + 1));
                MessageDisplay.Instance.Show("You unlocked the next story !");
            }
            else
            {
                MessageDisplay.Instance.Show("The next story is already unlocked");
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
