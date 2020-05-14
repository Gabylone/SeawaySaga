using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestFeedback : MonoBehaviour {

	[SerializeField]
	private GameObject group;

	[SerializeField]
	private Text uiText;

	public float duration = 2f;

	// Use this for initialization
	void Start () {
		
		QuestManager.Instance.onNewQuest += HandleNewQuestEvent;
		QuestManager.onFinishQuest += HandleOnFinishQuest;
		QuestManager.onGiveUpQuest += HandleOnGiveUpQuest;
		StoryFunctions.Instance.getFunction += HandleGetFunction;

		NameGeneration.onDiscoverFormula += HandleOnDiscoverFormula;
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		if ( func == FunctionType.AccomplishQuest ) {
			Display ("Quest " + Quest.currentQuest.Story.displayName + " is completed !");
		}
	}

	void HandleOnGiveUpQuest (Quest quest)
	{
		Display ("Quest " + quest.Story.displayName + " abandoned !");
	}

	void HandleOnFinishQuest (Quest quest)
	{
		Display ("Quest " + quest.Story.displayName + " finished !");
	}

	void HandleOnDiscoverFormula (Formula Formula)
	{
		Display ("New Clue !");
	}

	void HandleNewQuestEvent ()
	{
		if (QuestManager.Instance.currentQuests.Count == QuestManager.Instance.maxQuestAmount) {
			Display ("Max quests amount reached");
		} else {
			Display ("New Quest !");
		}
	}

	void Display ( string str ) {
		
		Show ();

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
