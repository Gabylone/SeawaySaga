
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour {

	public int id = 0;

    public bool mainQuest = false;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text goldText;

	[SerializeField]
	private Text giverText;

	[SerializeField]
	private Text experienceText;

	[SerializeField]
	private Text levelText;

	[SerializeField]
	private GameObject achievedFeedback;

	public void Select () {

		Tween.Bounce ( transform );

        if ( mainQuest)
        {
            DisplayQuest.Instance.DisplayMainQuest();
        }
        else
        {
            DisplayQuest.Instance.Display(QuestManager.Instance.currentQuests[id]);
        }

    }

	public void SetQuest ( int id ) {

		this.id = id;

		Quest quest = QuestManager.Instance.currentQuests [id];

        nameText.text = quest.Story.name;

    }
}
