using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLevelUp : MonoBehaviour {

	[SerializeField]
	private GameObject group;

	[SerializeField]
	private Text statText;

	int memberID = 0;

	// Use this for initialization
	void Start () {

		Hide ();

		memberID = GetComponentInParent<MemberIcon> ().member.MemberID.id;

		InitEvents ();

	}

	void HandleOnShowCharacterStats ()
	{
		if (CrewMember.GetSelectedMember.SkillPoints > 0
		    &&
		    GetComponentInParent<MemberIcon> ().member.MemberID.id == CrewMember.GetSelectedMember.MemberID.id) {

			Show ();

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
		SkillMenu.onShowSkillMenu 							+= HandleOnShowCharacterStats;
		SkillMenu.onHideSkillMenu							+= Hide;
	}

	void OnDestroy ()
	{
//		GetComponentInParent<MemberIcon> ().member.onLevelUp 		-= HandleOnLevelUp;
//		GetComponentInParent<MemberIcon> ().member.onLevelUpStat 	-= HandleOnLevelUpStat;
		SkillButton_Inventory.onUnlockSkill 						-= HandleOnUnlockSkill;
		SkillMenu.onShowSkillMenu 							-= HandleOnShowCharacterStats;
		SkillMenu.onHideSkillMenu							-= Hide;
	}
}
