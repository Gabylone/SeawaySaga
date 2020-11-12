
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour {

	public int id = 0;

    public static QuestButton currentlySelectedButton;

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

    public CanvasGroup canvasGroup;

	public void Select () {

        if ( currentlySelectedButton != null)
        {
            currentlySelectedButton.Deselect();
        }

		Tween.Bounce ( transform );

        SoundManager.Instance.PlaySound("Quest");
        SoundManager.Instance.PlayRandomSound("click_med");

        canvasGroup.alpha = 0.5f;

        if ( mainQuest)
        {
            DisplayQuest.Instance.DisplayMainQuest();
        }
        else
        {
            DisplayQuest.Instance.Display(QuestManager.Instance.currentQuests[id]);
        }

        currentlySelectedButton = this;
    }

    public void Deselect()
    {
        canvasGroup.alpha = 1f;
    }

    public void SetQuest ( int id ) {

		this.id = id;

		Quest quest = QuestManager.Instance.currentQuests [id];

        nameText.text = quest.Story.displayName;

    }
}
