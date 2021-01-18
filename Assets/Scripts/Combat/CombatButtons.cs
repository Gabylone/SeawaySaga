using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class CombatButtons : MonoBehaviour {

	SkillButton[] defaultSkillButtons;
    SkillButton[] skillButtons;

    Vector2 initPos;

    public Vector2 decal;

    private CanvasGroup canvasGroup;

    private RectTransform rectTransform;


    public Transform openSkilkButton_transform;
	public Button openSkillButton;
	public Image jobImage;

    public float tweenDuration = 0.5f;

    public GameObject group;

	public GameObject defaultGroup;
	public GameObject skillGroup;

    bool faded = false;
    
	void Start () {

        rectTransform = GetComponent<RectTransform>();
        initPos = rectTransform.position;
        canvasGroup = GetComponent<CanvasGroup>();

		CombatManager.Instance.onChangeState += HandleOnChangeState;

        CombatManager.Instance.onFightStart+= Show;
        CombatManager.Instance.onFightEnd += Hide;

		defaultSkillButtons = defaultGroup.GetComponentsInChildren<SkillButton> (true);
		skillButtons = skillGroup.GetComponentsInChildren<SkillButton> (true);

        HideButtons();

        FadeOut();

	}

    public void FadeIn()
    {
        if ( !faded)
        {
            return;
        }

        faded = false;

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, tweenDuration);

        rectTransform.position = initPos - decal;
        rectTransform.DOMove(initPos, tweenDuration);
    }

    public void FadeOut()
    {
        if (faded)
        {
            return;
        }

        faded = true;

        rectTransform.position = initPos;
        rectTransform.DOMove(initPos - decal, tweenDuration);

        canvasGroup.DOFade(0f, tweenDuration);
    }

    void Show()
    {
        group.SetActive(true);
    }

    void Hide()
    {
        HideButtons();
        group.SetActive(false);
    }

	void HandleOnChangeState (CombatManager.States currState, CombatManager.States prevState)
	{
        HideButtons();

        if (CombatManager.Instance.fighting == false)
            return;

		if ( currState == CombatManager.States.PlayerActionChoice ) {

			OpenDefaultButtons ();
            FadeIn();
		}
        else
        {
            FadeOut();
        }

    }

    void HideButtons()
    {
        defaultGroup.SetActive(false);
        skillGroup.SetActive(false);
    }

	public void OpenSkills () {

        SoundManager.Instance.PlaySound("click_med 03");

        defaultGroup.SetActive(false);
        skillGroup.SetActive(true);

        UpdateSkillButtons ();

		foreach (var item in skillButtons) {
			Tween.Bounce (item._transform);
		}

	}

	public void CloseSkills () {

            SoundManager.Instance.PlaySound("click_med 03");

        skillGroup.SetActive (false);

		OpenDefaultButtons ();

	}

	void OpenDefaultButtons () {

        defaultGroup.SetActive (true);

		UpdateDefaultButtons ();
	}

	void UpdateSkillButtons ()
	{
		CrewMember member = CombatManager.Instance.GetCurrentFighter.crewMember;

        Tween.Bounce(openSkilkButton_transform);

		int skillIndex = 0;

		foreach (var item in skillButtons) {

			if (skillIndex < member.SpecialSkills.Count) {

				item.gameObject.SetActive (true);

				item.SetSkill (member.SpecialSkills[skillIndex]);

			} else {
				item.gameObject.SetActive (false);
			}

			skillIndex++;
		}

	}

	void UpdateDefaultButtons ()
	{
		// check if player has enought energy
		CrewMember member = CombatManager.Instance.GetCurrentFighter.crewMember;

		int a = 0;
		foreach (var item in defaultSkillButtons) {
			
			item.SetSkill (member.DefaultSkills[a]);
			Tween.Bounce (item._transform);

			++a;
		}

		ResetOpenSkillButtons ();

		jobImage.sprite = SkillManager.jobSprites[(int)member.job];
	}

	void ResetOpenSkillButtons ()
	{
		CrewMember member = CombatManager.Instance.GetCurrentFighter.crewMember;

		openSkillButton.interactable = false;
		foreach (var item in member.SpecialSkills ) {
			if ( member.energy >= item.energyCost ) {
				openSkillButton.interactable = true;
				break;
			}
		}
	}
}
