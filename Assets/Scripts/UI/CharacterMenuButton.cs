using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuButton : MonoBehaviour {

    public static CharacterMenuButton Instance;

	public Image jobImage;

	public Text jobText;

    public Animator animator;

    public Text uiText;

    public GameObject exclamationPoint_Obj;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    void HandleOpenInventory ()
	{
		UpdateUI ();
	}

    public void OnPointerClick()
    {
        SkillMenu.Instance.Show();
    }

	public void UpdateUI ()
	{
		if (CrewMember.GetSelectedMember == null)
        {
            return;
        }

        CrewMember member = CrewMember.GetSelectedMember;

        jobImage.sprite = SkillManager.jobSprites [(int)member.job];

		UpdateSkillPoints ();
	}

	void UpdateSkillPoints ()
	{
        if (CrewMember.GetSelectedMember.SkillPoints > 0)
        {
            animator.SetBool("hasSkills", true);
            exclamationPoint_Obj.SetActive(true);

            uiText.color = Color.cyan;

            if (CrewMember.GetSelectedMember.SkillPoints > 1)
            {
                uiText.text = CrewMember.GetSelectedMember.SkillPoints + " skill points left";
            }
            else
            {
                uiText.text = CrewMember.GetSelectedMember.SkillPoints + " skill point left";
            }
        }
        else
        {

            uiText.text = CrewMember.GetSelectedMember.MemberName + "'s Skills";

            uiText.color = Color.white;

            animator.SetBool("hasSkills", false);
            exclamationPoint_Obj.SetActive(false);
        }
    }
}
