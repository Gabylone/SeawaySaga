using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuButton : MonoBehaviour {

    public static CharacterMenuButton Instance;

	public Image jobImage;

	public Text jobText;

    public Animator animator;

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
        }
        else
        {
            animator.SetBool("hasSkills", false);
        }
    }
}
