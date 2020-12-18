using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChoiceManager : MonoBehaviour {

	public static ChoiceManager Instance;

	public Sprite[] feedbackSprites;
	public Sprite[] bubbleSprites;
    public Color[] bubbleColors;

    public bool visible = false;

    public class Choice
    {
        public string content;
        public bool alreadyMade = false;
    }

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

	public void SetChoices (int amount, Choice[] choices) {

        Show();

		for (int i = 0; i < amount ; ++i ) {
            choiceButtons[i].id = i;
            choiceButtons[i].Init(choices[i]);
		}

		//Crews.playerCrew.captain.Icon.MoveToPoint (Crews.PlacingType.World);
		Crews.playerCrew.UpdateCrew(Crews.PlacingType.World);

	}

	public void ResetColors () {
		foreach ( ChoiceButton choiceButton in choiceButtons )
			choiceButton.image.color = Color.white;
	}

	public void Choose (int i) {

        SoundManager.Instance.PlayRandomSound("click_med");

        /// ici, si tu veux tainter les choix que tu as déjà fais.
        //StoryReader.Instance.CurrentStoryHandler.SaveDecal(-2);

        Hide();

		foreach ( ChoiceButton button in choiceButtons ) {
			button.gameObject.SetActive (false);
		}

		StoryReader.Instance.NextCell ();

        //decal 
        StoryReader.Instance.SetDecal(i);

        StoryReader.Instance.CurrentStoryHandler.SaveDecal(1, StoryReader.Instance.Row , StoryReader.Instance.Col-1);

        StoryReader.Instance.UpdateStory ();
	}

	#region dialogues choices
	public void GetChoices () {
		ChoiceManager.Instance.ResetColors ();

		// get amount
		int amount = int.Parse (StoryFunctions.Instance.CellParams);

		// get bubble content
		StoryReader.Instance.NextCell ();

        Choice[] choices = new Choice[amount];

		int tmpDecal = StoryReader.Instance.Row;
		int a = amount;

		int index = 0;
		while ( a > 0 ) {

			if ( StoryReader.Instance.ReadDecal (tmpDecal).Length > 0 ) {

				string content = StoryReader.Instance.ReadDecal (tmpDecal);
                content = content.Remove (0, 9);

                bool alreadyMade = StoryReader.Instance.CurrentStoryHandler.GetDecal(tmpDecal , StoryReader.Instance.Col) == 1;

                Choice choice = new Choice();
                choice.content = content;
                choice.alreadyMade = alreadyMade;
                choices[amount - a] = choice;

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
