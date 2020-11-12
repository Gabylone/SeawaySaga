using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatButton : MonoBehaviour {

    public Text uiText;

    private bool canInteract = false;

    Transform _transform;

    public Animator animator;
    public Image bgImage;
    public Outline outline;
    public GameObject skillPoint_Group;

	[SerializeField]
	private Stat stat;

	// Use this for initialization
	void Start ()
    {
        _transform = GetComponent<Transform>();

        InGameMenu.Instance.onDisplayCrewMember += HandleOnCardUpdate;
		SkillButton_Inventory.onUnlockSkill += UpdateDisplay;
        SkillManager.Instance.onLevelUpStat += UpdateDisplay;

		Display (CrewMember.GetSelectedMember);

	}

	void UpdateDisplay ()
	{
		Display (CrewMember.GetSelectedMember);
	}

	void Disable ()
	{
        canInteract = false;
        animator.enabled = false;
        outline.enabled = false;
        bgImage.color = Color.white;

        skillPoint_Group.SetActive(false);
	}

    void Enable()
    {
        canInteract = true;
        animator.enabled = true;
        outline.enabled = true;

        skillPoint_Group.SetActive(true);

    }

    void HandleOnCardUpdate (CrewMember member)
	{
		Display (member);
	}

	void Display (CrewMember member) {

		if (member == null)
			return;

        if (member.SkillPoints > 0 && member.GetStat(stat) < 6)
        {
            Enable();
        }
        else
        {
            Disable();
        }

		uiText.text = member.GetStat(stat).ToString();
	}

	public void OnClick () {

        Tween.Bounce(_transform);

        if (!canInteract)
        {
            SoundManager.Instance.PlayRandomSound("Bag");
            SoundManager.Instance.PlaySound("ui_wrong");
            return;
        }

        SoundManager.Instance.PlayRandomSound("Alchemy");
        SoundManager.Instance.PlayRandomSound("Bag");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");

		CrewMember.GetSelectedMember.HandleOnLevelUpStat (stat);

        if (SkillManager.Instance.onLevelUpStat != null)
        {
            SkillManager.Instance.onLevelUpStat();
        }

        UpdateDisplay();

    }
}
