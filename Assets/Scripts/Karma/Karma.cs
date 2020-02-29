using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Karma : MonoBehaviour {

	public static Karma Instance;

    public enum KarmaStep
    {
        Best,
        Good,
        Neutral,
        Bad,
        Worst
    }

    public KarmaStep karmaStep;

	private bool visible = false;

	private int currentKarma = 0;
	private int previousKarma = 0;

	public int bounty = 0;

	[Header("Params")]
	[SerializeField]
	private int bountyStep = 10;

	public int maxKarma = 10;

	[Header("UI")]
	[SerializeField]
	private GameObject group;
	[SerializeField]
	private Sprite[] sprites;
	[SerializeField]
	private Image feedbackImage;
	[SerializeField]
	private Image progressionImage;

	[Header("Sound")]
	[SerializeField] private AudioClip karmaGoodSound;
	[SerializeField] private AudioClip karmaBadSound;


	void Awake () {
        Instance = this;
        onChangeKarma = null;
	}

    void Start()
    {

        StoryFunctions.Instance.getFunction += HandleGetFunction;

        UpdateKarmaStep();

    }

    void HandleOpenInventory (CrewMember member)
	{
		Show ();
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.CheckKarma:
			CheckKarma ();
			break;
		case FunctionType.AddKarma:
			AddKarma_Story();
			break;
		case FunctionType.RemoveKarma:
			RemoveKarma_Story();
			break;
		case FunctionType.PayBounty:
			PayBounty();
			break;
		}
	}



	public void CheckKarma () {

		int decal = (int)karmaStep;

        switch (karmaStep)
        {
            case KarmaStep.Best:
                decal = 0;
                break;
            case KarmaStep.Good:
                decal = 1;
                break;
            case KarmaStep.Neutral:
            case KarmaStep.Bad:
                decal = 2;
                break;
            case KarmaStep.Worst:
                decal = 3;
                break;
            default:
                break;
        }

        StoryReader.Instance.NextCell ();
		StoryReader.Instance.SetDecal (decal);

		StoryReader.Instance.UpdateStory ();

	}

	public void AddKarma (int i) {
		
		CurrentKarma += i;

		if (onChangeKarma != null)
			onChangeKarma (previousKarma, currentKarma);
		//
	}
	public void RemoveKarma (int i) {
		
		CurrentKarma -= i;

		bounty += (bountyStep*i);

		if (onChangeKarma != null)
			onChangeKarma (previousKarma, currentKarma);

	}
	public void AddKarma_Story () {

		AddKarma (1);

        StoryReader.Instance.NextCell();
        StoryReader.Instance.Wait(1f);
    }

	public void RemoveKarma_Story () {

		RemoveKarma (2);

        StoryReader.Instance.NextCell();
        StoryReader.Instance.Wait(1f);
	}

	public void PayBounty () {

		StoryReader.Instance.NextCell ();

		if ( GoldManager.Instance.CheckGold (bounty) ) {

			CurrentKarma = -2;

			GoldManager.Instance.RemoveGold (bounty);

		} else {

			StoryReader.Instance.SetDecal (1);

			RemoveKarma (1);

		}

		StoryReader.Instance.UpdateStory ();

	}

	public delegate void OnChangeKarma ( int previousKarma , int newKarma );
	public static OnChangeKarma onChangeKarma;
	public int CurrentKarma {
		get {
			return currentKarma;
		}
		set {

			previousKarma = CurrentKarma;

            currentKarma = Mathf.Clamp(value, -maxKarma, maxKarma);

            UpdateKarmaStep();

            DisplayKarma.Instance.UpdateUI();
		}
	}

	public void FeedbackKarma () {
		//
	}

    void UpdateKarmaStep()
    {
        if (CurrentKarma == maxKarma )
        //if (CurrentKarma > (float)(maxKarma / 1.5f))
        {
            karmaStep = KarmaStep.Best;
            // un exemple de moralité
        }
        else if (CurrentKarma > 0)
        {
            karmaStep = KarmaStep.Good;
            // un mec bien
        }
        else if (CurrentKarma == 0)
        {
            karmaStep = KarmaStep.Neutral;
            // rien à signaler
        }
        else if (CurrentKarma > -maxKarma)
        {
            karmaStep = KarmaStep.Bad;
            // un mec louche
        }
        else
        {
            karmaStep = KarmaStep.Worst;
            // une sous merde
        }
    }

	public void Show () {
		Visible = true;
	}

	public void Hide () {
		Visible = false;
	}

	public bool Visible {
		get {
			return visible;
		}
		set {
			visible = value;

			group.SetActive (value);
		}
	}

	public void SaveKarma ()
	{
		SaveManager.Instance.GameData.karma = CurrentKarma;
		SaveManager.Instance.GameData.bounty = bounty;
	}

	public void LoadKarma ()
	{
		CurrentKarma = SaveManager.Instance.GameData.karma;
		bounty = SaveManager.Instance.GameData.bounty;
	}
}