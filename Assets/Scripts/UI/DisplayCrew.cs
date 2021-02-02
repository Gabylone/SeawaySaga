using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DisplayCrew : MonoBehaviour {

    public static DisplayCrew Instance;

	public GameObject targetGameObject;

	public RectTransform rectTransform;

	public float duration = 1f;
	public float decal = 200f;

	private Vector2 initPos;

    public GameObject switchGroup;

    public CanvasGroup skillCanvasGroup;
    public CanvasGroup inventoryCanvasGroup;

    public Transform skillTransform;
    public Transform inventoryTransform;

    public bool visible = false;

    public bool onSkills = false;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        HideDelay();

		initPos = rectTransform.anchoredPosition;
	}

    public void Show(CrewMember member)
    {
        DisplayItem_Loot.DeselectSelectedItem();

        member.ShowInInventory();

        rectTransform.DOAnchorPos(initPos, duration);

        targetGameObject.SetActive(true);

        CancelInvoke("Hide");


        if ( onSkills)
        {
            OnSwitchSkills();
        }
        else
        {
            OnSwitchInventory();
        }

        visible = true;

        CharacterMenuButton.Instance.UpdateUI();
        PlayerIcons.Instance.HandleOpenInventory();

        DisplayMinimap.Instance.fullDisplay_ButtonObj.SetActive(false);
        if (!StoryLauncher.Instance.PlayingStory)
        {
            DisplayMinimap.Instance.FadeOut();
        }
    }

    public void Hide()
    {
        DisplayMinimap.Instance.fullDisplay_ButtonObj.SetActive(true);
        if (!StoryLauncher.Instance.PlayingStory)
        {
            DisplayMinimap.Instance.FadeIn();
        }

        rectTransform.DOAnchorPos(Vector2.up * decal, duration);

        PlayerIcons.Instance.HandleCloseInventory();

        CancelInvoke("HideDelay");
        Invoke("HideDelay", duration);

        visible = false;

        onSkills = false;
    }

	void HideDelay () {
		targetGameObject.SetActive (false);
	}

    public void SwitchToSkills()
    {
        if (onSkills)
        {
            return;
        }

        LootUI.Instance.Hide();
        SkillMenu.Instance.Show();

        Tween.Bounce(skillTransform);

        OnSwitchSkills();
    }

    public void SwitchToInventory()
    {
        if (!onSkills)
        {
            return;
        }

        SkillMenu.Instance.Hide();
        LootUI.Instance.OpenInventory();

        Tween.Bounce(inventoryTransform);

        OnSwitchInventory();

    }

    public void ShowSkillSwitchGroup()
    {
        switchGroup.SetActive(true);
    }

    public void HideSkillSwitchGroup()
    {
        switchGroup.SetActive(false);
    }

    public void OnSwitchInventory()
    {
        onSkills = false;

        inventoryCanvasGroup.alpha = 0.5f;
        skillCanvasGroup.alpha = 1f;
    }

    public void OnSwitchSkills()
    {
        onSkills = true;

        inventoryCanvasGroup.alpha = 1f;
        skillCanvasGroup.alpha = 0.5f;
    }

}
