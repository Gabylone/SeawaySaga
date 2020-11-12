using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

public class FormulaManager : MonoBehaviour {

	public static FormulaManager Instance;

    public List<Formula> formulas = new List<Formula>();

    public List<int> clueIndexesFound = new List<int>();

    // formula input
    [SerializeField]
	private GameObject formulaGroup;
	[SerializeField]
	private InputField inputField;
    public Image inputBackground;
    public Transform inputTransform;
    public CanvasGroup inputCanvasGroup;
    public float tweenDuration = 0f;

    private bool previousActive = false;

    void Awake () {
		Instance = this;
	}

    void Start()
    {
        StoryFunctions.Instance.getFunction += HandleGetFunction;

        InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
        InGameMenu.Instance.onCloseMenu += HandleCloseInventory;

        HideFormulaCheckDelay();
    }

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.CheckClues:
			StartFormulaCheck ();
			break;
        case FunctionType.CheckIfFormulaIsland:
                CheckIfFormulaIsland();
            break;
        }
	}

	#region inv
	void HandleOpenInventory ()
	{
		if (formulaGroup.activeSelf) {

			formulaGroup.SetActive (false);

			previousActive = true;
		}
	}

	void HandleCloseInventory ()
	{
		if ( previousActive ) {

			formulaGroup.SetActive (true);

			previousActive = false;	
		}
	}
	#endregion

	public void Init () {

	}

	#region formula check
	void StartFormulaCheck () {

        ShowFormulaCheck();
	}

    void ShowFormulaCheck()
    {
        SoundManager.Instance.PlayRandomSound("Bag");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Tribal");

        formulaGroup.SetActive(true);

        inputCanvasGroup.alpha = 0f;
        inputCanvasGroup.DOFade(1f, 0.4f);
        inputBackground.color = Color.white;
        Tween.Bounce(inputTransform);
    }

    void HideFormulaCheck()
    {
        inputCanvasGroup.DOFade(0f, 0.4f);

        CancelInvoke("HideFormulaCheckDelay");
        Invoke("HideFormulaCheckDelay", 0.5f);
    }

    void HideFormulaCheckDelay()
    {
        formulaGroup.SetActive(false);
    }

    void CheckIfFormulaIsland()
    {
        StoryReader.Instance.NextCell();

        if (Chunk.currentChunk.IsFormulaIsland())
        {
            StoryReader.Instance.SetDecal(1);
        }

        StoryReader.Instance.UpdateStory();
    }


    public void CheckFormula () {

        string stringToCheck = inputField.text.ToLower();
        Formula containedFormula = formulas.Find(x => stringToCheck.Contains(x.name.ToLower()));

        Tween.Bounce(inputTransform);

        if (containedFormula == null)
        {
            // NOT CORRECT
            SoundManager.Instance.PlaySound("ui_wrong");
            SoundManager.Instance.PlayRandomSound("Bag");
            SoundManager.Instance.PlayRandomSound("Magic Chimes");

            inputBackground.color = Color.red;
        }
        else if (containedFormula.verified)
        {
            SoundManager.Instance.PlaySound("ui_wrong");
            SoundManager.Instance.PlayRandomSound("Bag");
            SoundManager.Instance.PlayRandomSound("Magic Chimes");

            inputBackground.color = Color.red;
        }
        else
        {
            SoundManager.Instance.PlaySound("ui_correct");
            SoundManager.Instance.PlayRandomSound("Bag");
            SoundManager.Instance.PlayRandomSound("Magic Chimes");
            SoundManager.Instance.PlaySound("Confirm_Big");

            inputBackground.color = Color.blue;
        }

        HideFormulaCheck();

        Invoke("CheckFormulaDelay", 0.5f);

    }

    void CheckFormulaDelay()
    {
        string stringToCheck = inputField.text.ToLower();
        inputField.text = "";

        Formula containedFormula = formulas.Find(x => stringToCheck.Contains(x.name.ToLower()));

        StoryReader.Instance.NextCell();

        if (containedFormula == null)
        {
            DialogueManager.Instance.PlayerSpeak("Nothing seems to happen, I must have spoke something wrong");
            StoryReader.Instance.SetDecal(0);
        }
        else if (containedFormula.verified)
        {
            // ALREADY SPOKE
            DialogueManager.Instance.PlayerSpeak("I already spoke this word, and it already worked");
            StoryReader.Instance.SetDecal(0);
        }
        else
        {
            containedFormula.verified = true;

            bool allFormulasHaveBeenVerified = true;

            foreach (var formula in formulas)
            {
                if (formula.verified == false)
                {
                    allFormulasHaveBeenVerified = false;
                }
            }

            if (allFormulasHaveBeenVerified)
            {
                DialogueManager.Instance.PlayerSpeak("Look like something happened !*The door moved a little, but did not opened*I need to keep looking");
                StoryReader.Instance.SetDecal(2);
            }
            else
            {
                DialogueManager.Instance.PlayerSpeak("Look like something happened !*The door is OPENING COMPLETLY !");
                StoryReader.Instance.SetDecal(1);
            }
        }

        StoryReader.Instance.PreviousCell();
    }
    #endregion

    public string getDirectionToFormula () {

		Directions dir = NavigationManager.Instance.getDirectionToPoint (FormulaManager.Instance.GetNextClueIslandPos);
		string directionPhrase = NavigationManager.Instance.getDirName (dir);

		return directionPhrase;
	}

	public Vector2 GetNextClueIslandPos {
		
		get {

			foreach (var form in formulas) {

				if ( !form.found ) {
					return (Vector2)form.coords;
				}
			}

			return (Vector2)SaveManager.Instance.GameData.treasureCoords;

		}
	}

	public void LoadFormulas ()
	{
        formulas = SaveManager.Instance.GameData.formulas;
        clueIndexesFound = SaveManager.Instance.GameData.clueIndexesFound;
	}

	public void SaveFormulas () {
		SaveManager.Instance.GameData.formulas = formulas;
        SaveManager.Instance.GameData.clueIndexesFound = clueIndexesFound;
	}
}

[System.Serializable]
public class Formula {
	public string 	name;
	public Coords 	coords;
	public bool 	verified	= false;
	public bool 	found		= false;

	public Formula () {
		
	}
}