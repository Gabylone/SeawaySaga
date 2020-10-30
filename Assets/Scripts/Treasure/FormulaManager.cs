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
    public CanvasGroup inputCanvasGroup;

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

		formulaGroup.SetActive (true);
		StoryReader.Instance.NextCell ();
	}

    void ShowFormulaCheck()
    {
        inputCanvasGroup.alpha = 0f;
        inputCanvasGroup.DOFade(1f, 0.4f);
        inputBackground.color = Color.white;

        Tween.Bounce(inputBackground.rectTransform);
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
		inputField.text = "";

		Formula containedFormula = formulas.Find ( x => stringToCheck.Contains (x.name.ToLower ()));

        Tween.Bounce(inputBackground.rectTransform);

        if ( containedFormula == null ) {

            inputBackground.color = Color.red;

			StoryReader.Instance.SetDecal (0);
		} else if ( containedFormula.verified ) {

            inputBackground.color = Color.red;

            StoryReader.Instance.SetDecal (0);
		}
        else
        {

            inputBackground.color = Color.green;

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
                StoryReader.Instance.SetDecal(2);
            }
            else
            {
                StoryReader.Instance.SetDecal(1);
            }

        }

        Invoke("CheckFormulaDelay", 1f);

    }

    void CheckFormulaDelay()
    {
        StoryReader.Instance.UpdateStory();

        Invoke("HideFormulaCheck", 0.5f);

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