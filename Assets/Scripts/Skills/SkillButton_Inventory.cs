using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class SkillButton_Inventory : SkillButton
{
    private RectTransform _rectTransform;

    public GameObject padlockObj;
    public Text padlockText;

    bool canInteract = false;

    public CanvasGroup canvasGroup;
    public CanvasGroup padlock_CanvasGroup;

    public GameObject skillPointGroup;

    public Animator padlockAnimator;

    public delegate void OnUnlockSkill();
    public static OnUnlockSkill onUnlockSkill;

    public override void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        base.Start();

        onUnlockSkill += UpdateUI;
        SkillManager.Instance.onLevelUpStat += UpdateUI;
    }

    void UpdateUI()
    {
        if ( CrewMember.GetSelectedMember == null)
        {
            Debug.Log("selected member");
            return;
        }

        if (HasSkill())
        {
            skillPointGroup.SetActive(false);
            padlockObj.SetActive(false);
        }
        else
        {
            padlockObj.SetActive(true);

            if (CrewMember.GetSelectedMember.SkillPoints > 0)
            {
                skillPointGroup.SetActive(true);

                padlockText.text = "" + GetSkillCost();

                if (CrewMember.GetSelectedMember.SkillPoints < GetSkillCost())
                {
                    padlockText.color = Color.red;
                }
                else
                {
                    padlockText.color = Color.white;
                }
            }
            else
            {
                skillPointGroup.SetActive(false);
            }
            
        }

    }

    public void OnPointerDown()
    {
        Tween.Bounce(_rectTransform);

        if (
            HasSkill()
            ||
            CrewMember.GetSelectedMember.SkillPoints < GetSkillCost()
            )
        {
            SoundManager.Instance.PlaySound("ui_wrong");
            SoundManager.Instance.PlayRandomSound("Tribal");

            padlockAnimator.SetTrigger("giggle");
            return;
        }

        SoundManager.Instance.PlaySound("click_med 01");
        SoundManager.Instance.PlayRandomSound("Bag");

        CrewMember.GetSelectedMember.SkillPoints -= GetSkillCost();

        CrewMember.GetSelectedMember.AddSkill(skill);

        if (onUnlockSkill != null)
            onUnlockSkill();

        SoundManager.Instance.PlayRandomSound("Alchemy");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Potion");

        UpdateUI();

    }

    public override void SetSkill(Skill _skill)
    {
        base.SetSkill(_skill);

        UpdateUI();
    }

    public bool HasSkill()
    {
        return CrewMember.GetSelectedMember.SpecialSkills.Find(x => x.type == skill.type) != null;
    }

    private int GetSkillCost()
    {
        return (int)(CrewMember.GetSelectedMember.SpecialSkills.Count * 1.5f);
    }

}
