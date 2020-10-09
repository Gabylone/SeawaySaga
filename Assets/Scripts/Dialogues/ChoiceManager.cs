using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChoiceManager : MonoBehaviour {

	public static ChoiceManager Instance;

	public Sprite[] feedbackSprites;
	public Sprite[] bubbleSprites;
    public Color[] bubbleColors;

    public bool visible = false;

    public string[] bubblePhrases = new string[8] {
		"(partir)",
		"(attaquer)",
		"(trade)",
		"(autre)",
		"(dormir)",
		"(membre)",
		"(loot)",
		"(quete)"
	};

	[Header("Choices")]
	[SerializeField]
	private ChoiceButton[] choiceButtons;

	[SerializeField]
	private Color[] statColor;

	[SerializeField]
	private GameObject choiceGroup;

    private bool activeWhenInventoryOpens = false;

    void Awake () {
		Instance = this;
	}

    void Start()
    {
        Invoke("StartDelay", 0.001f);
    }

    void StartDelay()
    {
        feedbackSprites = Resources.LoadAll<Sprite>("Graph/ChoiceBubbleFeedbackSprites");

        StoryFunctions.Instance.getFunction += HandleGetFunction;

        InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
        InGameMenu.Instance.onCloseMenu += HandleCloseInventory;
    }

    public void Show()
    {
        choiceGroup.SetActive(true);

        visible = true;
    }

    public void Hide()
    {
        choiceGroup.SetActive(false);

        visible = false;
    }

	void HandleOpenInventory ()
	{
		if (visible) {


            Hide();

			activeWhenInventoryOpens = true;
		}
	}

	void HandleCloseInventory ()
	{

        if ( activeWhenInventoryOpens ) {


            Show();

			activeWhenInventoryOpens = false;	
		}
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.SetChoices:
			GetChoices ();
			break;
        }
	}

	public void SetChoices (int amount, string[] content) {
        Show();

		for (int i = 0; i < amount ; ++i ) {
            choiceButtons[i].id = i;
            choiceButtons[i].Init(content[i]);
		}

		//Crews.playerCrew.captain.Icon.MoveToPoint (Crews.PlacingType.World);
		Crews.playerCrew.UpdateCrew(Crews.PlacingType.World);

	}

	public void ResetColors () {
		foreach ( ChoiceButton choiceButton in choiceButtons )
			choiceButton.image.color = Color.white;
	}

	public void TaintChoice (int buttonIndex , int statIndex) {

		choiceButtons [buttonIndex].GetComponentInChildren<Image> ().color = statColor [statIndex];

	}

	public void Choose (int i) {

        SoundManager.Instance.PlayRandomSound("click_med");

        StoryReader.Instance.SetDecal (i);

        /// ici, si tu veux tainter les choix que tu as déjà fais.
        //StoryReader.Instance.CurrentStoryHandler.SaveDecal(-2);

        Hide();

		foreach ( ChoiceButton button in choiceButtons ) {
			button.gameObject.SetActive (false);
		}

		StoryReader.Instance.NextCell ();
		StoryReader.Instance.UpdateStory ();
	}

	#region dialogues choices
	public void GetChoices () {
		ChoiceManager.Instance.ResetColors ();

		// get amount
		int amount = int.Parse (StoryFunctions.Instance.CellParams);

		// get bubble content
		StoryReader.Instance.NextCell ();

		string[] choices = new string[amount];

		int tmpDecal = StoryReader.Instance.Row;
		int a = amount;

		int index = 0;
		while ( a > 0 ) {

			if ( StoryReader.Instance.ReadDecal (tmpDecal).Length > 0 ) {

				string choice = StoryReader.Instance.ReadDecal (tmpDecal);

				choice = choice.Remove (0, 9);

				int i = 0;

				string[] stats = new string[] { "(str)", "(dex)", "(cha)", "(con)" };
				foreach ( string stat in stats ) {

					if ( choice.Contains ( stat ) ) {

						ChoiceManager.Instance.TaintChoice (index, i);
						choice = choice.Replace (stat, "");

					}

					++i;

				}

				choices [amount - a] = choice;

				--a;
				++index;
			}

			++tmpDecal;

			if ( tmpDecal > 60 ) {
				Debug.LogError ("set choice reached limit");
				break;
			}

			if (a <= 0)
				break;
		}

		SetChoices (amount, choices);
	}
	#endregion

    public ChoiceButton[] ChoiceButtons {
		get {
			return choiceButtons;
		}
	}
	
}
