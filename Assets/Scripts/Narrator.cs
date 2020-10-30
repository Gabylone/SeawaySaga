using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Narrator : MonoBehaviour {

	public static Narrator Instance;

	[Header("Narrator")]
	[SerializeField] private Text narratorText;
	[SerializeField] private GameObject narratorObj;
	[SerializeField] private GameObject narratorButtonObj;

    public bool visible = false;

    void Awake () {
		Instance = this;
	}

	void Start () {

        StoryInput.Instance.onPressInput += HandleOnPressInput;

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

	void HandleOnPressInput ()
	{
        if (!visible)
            return;

        Invoke("HandleOnPressInputDelay", 0.0001f);   
    }

    void HandleOnPressInputDelay()
    {
        HideNarrator();
        StoryReader.Instance.ContinueStory();
    }

    bool previousActive = false;
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
	public void ShowNarratorTimed (string text) {

        narratorButtonObj.SetActive(false);

        ShowNarrator(text);


		Invoke ("HideNarrator" , 2.5f );
	}
    public void ShowNarratorInput(string text)
    {
        ShowNarrator(text);
        StoryInput.Instance.WaitForInput();
    }
    public void ShowNarrator (string text) {

        //InGameMenu.Instance.HideMenuButtons();

        //Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Hidden);
        //Crews.enemyCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Hidden);

        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        narratorButtonObj.SetActive(true);

        visible = true;

        Tween.Bounce (narratorObj.transform , 0.1f , 1.01f);

		narratorObj.SetActive (true);

		narratorText.text = NameGeneration.CheckForKeyWords (text);

        foreach (var layoutGroup in GetComponentsInChildren<VerticalLayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
	}
	public void HideNarrator () {

        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        //InGameMenu.Instance.ShowMenuButtons();

        narratorObj.SetActive (false);

        visible = false;
	}
	#endregion
}
