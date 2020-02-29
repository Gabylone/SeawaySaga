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
			Display ("Quête " + Quest.currentQuest.Story.name + " accomplie !");
		}
	}

	void HandleOnGiveUpQuest (Quest quest)
	{
		Display ("Quête " + quest.Story.name + " abandonnée !");
	}

	void HandleOnFinishQuest (Quest quest)
	{
		Display ("Quête " + quest.Story.name + " finie !");
	}

	void HandleOnDiscoverFormula (Formula Formula)
	{
		Display ("Nouvel Indice !");
	}

	void HandleNewQuestEvent ()
	{
		if (QuestManager.Instance.currentQuests.Count == QuestManager.Instance.maxQuestAmount) {
			Display ("Nombre maximum de quête atteint");
		} else {
			Display ("Nouvelle Quête");
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
