using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillButton_Combat : SkillButton {

	public GameObject energyGroup;
    public Text uiText_Energy;

    public CanvasGroup canvasGroup;

	public Text uiText_Restriction;
	public RectTransform rectTransform_Restriction;
	public Vector2 resctriction_InitPos;
	public float restriction_Duration;
	public float restriction_Amount;

	public Outline outline;
	public bool displayOutline = false;

	// charge
	public Image chargeFillImage;
	public GameObject chargeGroup;
	public Text chargeText;

	public float timeToShowDescriptionFeedback = 0.2f;

	private bool canTriggerSkill = true;

	public override void Start ()
	{
		base.Start ();

		uiText_Restriction.color = Color.clear;
	}

	public override void SetSkill (Skill _skill)
	{
		base.SetSkill (_skill);

		CheckSkill ();
	}

	void Enable () {
		canTriggerSkill = true;
		//skillImage.color = Color.black;
        button.interactable = true;

        canvasGroup.alpha = 1f;


		if ( displayOutline)
		{
			outline.enabled = true;
		}
	}

	void Disable()
	{
		canTriggerSkill = false;
		//skillImage.color = new Color ( 1,1,1,0.35f );
		//button.interactable = false;

		canvasGroup.alpha = 0.5f;


        if (displayOutline)
        {
            outline.enabled = false;
        }

    }

	void CheckSkill ()
	{
		if (skill.energyCost == 0) {

			energyGroup.SetActive (false);
		} else {
			energyGroup.SetActive (true);
			uiText_Energy.text = "" + skill.energyCost;
		}

		Fighter fighter = CombatManager.Instance.GetCurrentFighter;

		Enable ();

		// ENERGY
		if (skill.MeetsRestrictions (fighter.crewMember) == false) {
			Disable ();
		}

		if ( skill.energyCost > fighter.crewMember.energy ) {
			skill.currentRestriction = "No energy...";
			Disable ();
		}

		// CHARGE
		int charge = fighter.crewMember.charges[skill.GetSkillIndex(fighter.crewMember)];

		UpdateCharge(charge);

	}

	void UpdateCharge (int charge) {
		if (charge > 0) {
			Disable ();
			skill.currentRestriction = "Ready in " + charge + (charge == 1 ? " turn" : " turns");
			chargeGroup.SetActive (true);
			chargeFillImage.fillAmount = (float)charge / (float)skill.initCharge;
			chargeText.text = "" + charge;
		} else {
			chargeGroup.SetActive (false);
		}
	}

	public void OnPointerUp () {

		Tween.Bounce(_transform);

		if (!canTriggerSkill ) {
			if (!string.IsNullOrEmpty(skill.currentRestriction))
			{
				RestrictionFeedback();
			}
			return;
		}

		skill.Trigger(CombatManager.Instance.GetCurrentFighter);

		chargeFillImage.DOKill();
		CrewMember member = CombatManager.Instance.GetCurrentFighter.crewMember;
		int charge = member.charges[skill.GetSkillIndex(member)];
		UpdateCharge (charge);

        SoundManager.Instance.PlaySound("click_med 02");

    }

	void RestrictionFeedback()
    {
		uiText_Restriction.text = skill.currentRestriction;
		Tween.Bounce(rectTransform_Restriction);
		uiText_Restriction.DOKill();
		uiText_Restriction.color = Color.red;
		uiText_Restriction.DOColor(Color.clear, 0.5f).SetDelay(restriction_Duration);

		if ( resctriction_InitPos == Vector2.zero)
        {
			resctriction_InitPos = rectTransform_Restriction.position;
		}
		rectTransform_Restriction.position = resctriction_InitPos;
		rectTransform_Restriction.DOMove(resctriction_InitPos + Vector2.right * restriction_Amount, 0.5f).SetDelay(restriction_Duration).SetEase(Ease.InBounce);
	}
}
