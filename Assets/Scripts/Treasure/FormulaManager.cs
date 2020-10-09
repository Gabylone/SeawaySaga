using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FormulaManager : MonoBehaviour {

	public static FormulaManager Instance;

    public List<Formula> formulas = new List<Formula>();

	[SerializeField]
	private GameObject formulaGroup;

	[SerializeField]
	private InputField inputField;

    private bool previousActive = false;

    void Awake () {
		Instance = this;
	}

	void Start () {
		StoryFunctions.Instance.getFunction += HandleGetFunction;

		InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		InGameMenu.Instance.onCloseMenu += HandleCloseInventory;
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.CheckClues:
                Debug.Log("check clues");
			StartFormulaCheck ();
			break;
        case FunctionType.CheckIfFormulaIsland:
                Debug.Log("checking if clue island");
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
		formulaGroup.SetActive (false);

		Formula containedFormula = formulas.Find ( x => stringToCheck.Contains (x.name.ToLower ()));

		if ( containedFormula == null ) {
			//print ("input field does not contain any formulas");
			StoryReader.Instance.SetDecal (0);
			StoryReader.Instance.UpdateStory ();
			return;
		}

		if ( containedFormula.verified ) {
			//print ("formula is already verified... need another one");
			StoryReader.Instance.SetDecal (0);
			StoryReader.Instance.UpdateStory ();
			return;
		}

		containedFormula.verified = true;
		//print ("la formule est bonne");

		bool allFormulasHaveBeenVerified = true;

		foreach (var formula in formulas) {
			if ( formula.verified== false) {
				allFormulasHaveBeenVerified = false;
			}
		}

		if ( allFormulasHaveBeenVerified ) {
			print ("toutes les formules sont vérifiées, il faut ouvrir la porte");
			StoryReader.Instance.SetDecal (2);
			StoryReader.Instance.UpdateStory ();
		} else {
			print ("il reste des formules à vérifier");
			StoryReader.Instance.SetDecal (1);
			StoryReader.Instance.UpdateStory ();
		}

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
	}

	public void SaveFormulas () {
		SaveManager.Instance.GameData.formulas = formulas;
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