using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class QuestMenu : MonoBehaviour {

	public static QuestMenu Instance;

	[SerializeField]
	private GameObject buttonPrefab;

    private List<QuestButton> questButtons = new List<QuestButton>();

	[SerializeField]
	private float buttonDecal = 0f;

	[SerializeField]
	private Transform anchor;

    public CanvasGroup canvasGroup;

	[SerializeField]
	private RectTransform contentTransform;

	[SerializeField]
	private GameObject menuGroup;
	[SerializeField]
	private GameObject openButton;

	public Text displayQuestText;

    public GameObject exclamationMark_Obj;

	public delegate void OnOpenQuestMenu ();
	public static OnOpenQuestMenu onOpenQuestMenu;

    public Animator animator;

    public QuestButton mainQuestButton;

	public bool opened = false;

    bool closing = false;

	void Awake () {
		Instance = this;

        onOpenQuestMenu = null;
        
	}

	void Start () {
		
		QuestManager.Instance.onGiveUpQuest += HandleOnGiveUpQuest;
//		CrewInventory.Instance.closeInventory += HandleCloseInventory;

		RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;

		HideMenu ();
	}

	void HandleOnTouchRayBlocker ()
	{
		if ( opened )
			Close ();
	}

	public void Init () {
		InitButtons ();
	}

	void HandleOnGiveUpQuest (Quest quest)
	{
		UpdateButtons ();
	}

	public void Open () {

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);

        InGameMenu.Instance.Open();

        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        animator.SetTrigger("open");

        menuGroup.SetActive (true);

        QuestMenu.Instance.exclamationMark_Obj.SetActive(false);

        DisplayQuestAmount();

		UpdateButtons ();

        DisplayQuest.Instance.Hide();

        if (onOpenQuestMenu != null)
			onOpenQuestMenu ();

		opened = true;

	}

	public void Close () {

        if ( closing)
        {
            return;
        }

        SoundManager.Instance.PlayRandomSound("Book");
        SoundManager.Instance.PlayRandomSound("Page");

        closing = true;

        animator.SetTrigger("close");

        canvasGroup.DOFade(0f, 0.5f).SetDelay(0.5f);

        Invoke("HideMenu", 1f);
    }

    void DisplayQuestAmount () {

		/*if (QuestManager.Instance.currentQuests.Count == 0) {
			displayQuestText.text = "aucune quêtes";
		} else {
			displayQuestText.text = QuestManager.Instance.currentQuests.Count + " quêtes en cours";
		}*/

	}
	void HideMenu() {

        closing = false;

        opened = false;

        InGameMenu.Instance.Hide();

        menuGroup.SetActive (false);
	}

	void InitButtons () {

		/// CREATE BUTTONS
		for (int buttonIndex = 0; buttonIndex < QuestManager.Instance.maxQuestAmount; buttonIndex++) {

			GameObject newButton = Instantiate (buttonPrefab, anchor) as GameObject;
			questButtons.Add(newButton.GetComponent<QuestButton> ());
		}

	}

	void UpdateButtons () {

        mainQuestButton.Deselect();

		foreach (var item in questButtons) {
			item.gameObject.SetActive (false);
            item.Deselect();
		}

		/// UPDATE BUTTON TO QUESTS
		for (int questIndex = 0; questIndex < questButtons.Count; questIndex++) {

			if (questIndex < QuestManager.Instance.currentQuests.Count) {

                questButtons[questIndex].gameObject.SetActive (true);
                questButtons[questIndex].GetComponent<QuestButton> ().SetQuest (questIndex);
			}
		}
	}

}
