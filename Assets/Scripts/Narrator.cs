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

    void Awake () {
		Instance = this;
	}

	void Start () {
		StoryFunctions.Instance.getFunction+= HandleGetFunction;

		InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		InGameMenu.Instance.onCloseMenu += HandleCloseInventory;

		StoryInput.onPressInput += HandleOnPressInput;

        HideNarrator();

	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.Narrator:
			ShowNarrator (cellParameters.Remove (0, 2));
			break;
		}
	}

	void HandleOnPressInput ()
	{
		HideNarrator ();
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
	public void ShowNarrator (string text) {

        //InGameMenu.Instance.HideMenuButtons();

        //Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Hidden);
        //Crews.enemyCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Hidden);

        narratorButtonObj.SetActive(true);

        Tween.Bounce (narratorObj.transform , 0.1f , 1.01f);

		narratorObj.SetActive (true);

		narratorText.text = NameGeneration.CheckForKeyWords (text);

        foreach (var layoutGroup in GetComponentsInChildren<VerticalLayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
	}
	public void HideNarrator () {

        //InGameMenu.Instance.ShowMenuButtons();

        narratorObj.SetActive (false);
	}
	#endregion
}
