using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {

	public Image skillImage;

	public GameObject descriptionGroup;

	public Text uiText_SkillName;
	public Text uiText_Description;

	public Skill skill;

	public Button button;

	public float timeToShowDescription = 0.5f;

	public virtual void Start () {
		//button = GetComponentInChildren<Button> ();
	}

	#region description
	public void ShowDescription ()
	{
		Tween.Bounce (transform);

		descriptionGroup.SetActive (true);

 		uiText_SkillName.text = skill.skillName;
		uiText_Description.text = skill.description;
	}

	public void HideDescription ()
	{
		descriptionGroup.SetActive (false);
	}
	#endregion

	public virtual void SetSkill (Skill _skill)
	{
		skill = _skill;

		skillImage.sprite = SkillManager.skillSprites [(int)skill.type];

        uiText_SkillName.text = _skill.skillName;
	}

	public void Show () {
		gameObject.SetActive (true);
	}

	public void Hide () 
	{
		gameObject.SetActive (false);
	}

}
