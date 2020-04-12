using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenuButton : MonoBehaviour {

	public Image jobImage;

	public Text jobText;

	public GameObject skillPointsGroup;
	public Text skillPointsText;

	public GameObject group;

    void Start()
    {

        InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
        SkillMenu.Instance.onHideSkillMenu += UpdateUI;

        UpdateUI();

    }


	void HandleOpenInventory ()
	{
		UpdateUI ();
	}

	void UpdateUI ()
	{
		if (CrewMember.GetSelectedMember == null)
			return;

		CrewMember member = CrewMember.GetSelectedMember;

		if (SkillManager.jobSprites.Length <= (int)member.job)
			print ("skill l : " + SkillManager.jobSprites.Length + " / member job " + (int)member.job);

		jobImage.sprite = SkillManager.jobSprites [(int)member.job];
		//jobText.text = SkillManager.jobNames [(int)member.job];

		Tween.Bounce (jobImage.transform);

		UpdateSkillPoints ();
	}

	void UpdateSkillPoints ()
	{
		if (CrewMember.GetSelectedMember.SkillPoints > 0) {

			skillPointsGroup.SetActive (true);

			skillPointsText.text = CrewMember.GetSelectedMember.SkillPoints.ToString ();

			Tween.Bounce (skillPointsGroup.transform);

		} else {
			skillPointsGroup.SetActive (false);
			//
		}
	}
}
