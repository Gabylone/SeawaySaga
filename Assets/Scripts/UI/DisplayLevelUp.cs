using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLevelUp : MonoBehaviour {

	[SerializeField]
	private GameObject group;

	[SerializeField]
	private Text statText;

    MemberIcon memberIcon;

	// Use this for initialization
	void Start () {

        memberIcon = GetComponentInParent<MemberIcon>();

        Invoke("StartDelay", 0.001f);

        InitEvents();

	}

    void StartDelay()
    {
        if (memberIcon.member.SkillPoints > 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

	void HandleOnUnlockSkill ()
	{
		if (GetComponentInParent<MemberIcon> ().member == CrewMember.GetSelectedMember) {
			UpdateStatText (CrewMember.GetSelectedMember);
		}
	}

	void HandleOnLevelUp (CrewMember member)
	{
		Show ();
	}

	void HandleOnLevelUpStat (CrewMember member)
	{
		UpdateStatText (member);
	}

	void UpdateStatText (CrewMember member)
	{
		if (member.SkillPoints == 0) {
			Hide ();
			return;
		}

		statText.text = member.SkillPoints.ToString();
	}

	void Show () {
		group.SetActive (true);
		UpdateStatText (CrewMember.GetSelectedMember);
	}

	void Hide () {
		group.SetActive (false);
	}

	void InitEvents ()
	{
		GetComponentInParent<MemberIcon> ().member.onLevelUp 		+= HandleOnLevelUp;
		GetComponentInParent<MemberIcon> ().member.onLevelUpStat 	+= HandleOnLevelUpStat;
		SkillButton_Inventory.onUnlockSkill 						+= HandleOnUnlockSkill;
	}

	void OnDestroy ()
	{
//		GetComponentInParent<MemberIcon> ().member.onLevelUp 		-= HandleOnLevelUp;
//		GetComponentInParent<MemberIcon> ().member.onLevelUpStat 	-= HandleOnLevelUpStat;
		SkillButton_Inventory.onUnlockSkill 						-= HandleOnUnlockSkill;
	}
}
