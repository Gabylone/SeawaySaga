using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestFeedback : MonoBehaviour {

    public static QuestFeedback Instance;

	[SerializeField]
	private GameObject group;

	[SerializeField]
	private Text uiText;

	public float duration = 2f;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
		
		QuestManager.Instance.onNewQuest += HandleNewQuestEvent;
		QuestManager.Instance.onFinishQuest += HandleOnFinishQuest;
		QuestManager.Instance.onGiveUpQuest += HandleOnGiveUpQuest;
        StoryFunctions.Instance.getFunction += HandleGetFunction;

		NameGeneration.onDiscoverFormula += HandleOnDiscoverFormula;
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		if ( func == FunctionType.AccomplishQuest ) {
            AccomplishQuest();
		}
	}

    void AccomplishQuest()
    {
        Display("Quest " + Quest.currentQuest.Story.displayName + " is completed !");

        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlaySound("Boost 02");
        SoundManager.Instance.PlaySound("Tribal 01");

        SoundManager.Instance.PlaySound("ui_correct");
    }

    void HandleOnGiveUpQuest(Quest quest)
    {
        Display("Quest " + quest.Story.displayName + " abandoned !");


        SoundManager.Instance.PlaySound("Tribal 03");
        SoundManager.Instance.PlaySound("ui_deny");


    }

    void HandleOnFinishQuest (Quest quest)
	{
		Display ("Quest " + quest.Story.displayName + " finished !");

        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlaySound("Boost 02");
        SoundManager.Instance.PlaySound("Tribal 01");

    }

    void HandleOnDiscoverFormula (Formula Formula)
	{
		Display ("New Clue !");

        SoundManager.Instance.PlaySound("Mystick Tap");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");

    }

    void HandleNewQuestEvent ()
	{
		if (QuestManager.Instance.currentQuests.Count == QuestManager.Instance.maxQuestAmount) {
            HandleOnMaxQuest();
		} else {
            HandleOnNewQuest();
		}
	}

    void HandleOnNewQuest()
    {
        Display("New Quest !");

        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlaySound("Boost 02");
        SoundManager.Instance.PlaySound("Tribal 01");

        SoundManager.Instance.PlaySound("ui_jauge_up");
    }

    public void HandleOnNewRumor()
    {
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlaySound("Boost 02");
        SoundManager.Instance.PlaySound("Tribal 01");

        Display("New Rumor !");
    }

    void HandleOnMaxQuest()
    {
        Display("Max quests amount reached");

        SoundManager.Instance.PlaySound("ui_deny");
    }

	void Display ( string str ) {
		
		Show ();

        SoundManager.Instance.PlayRandomSound("Writing");

        Tween.Bounce (transform);

		uiText.text = str;

		Invoke ("Hide",duration);

	}

	void Show () {
		group.SetActive (true);
	}

	void Hide () {
		group.SetActive (false);
	}
}
