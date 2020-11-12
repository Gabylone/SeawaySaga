using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLevel : MonoBehaviour {

	public Image backGround;
	public Image fillImage;

	public Text text;

	// Use this for initialization
	void Start () {

		CrewMember.onWrongLevel += HandleOnWrongLevelEvent;
        InGameMenu.Instance.onDisplayCrewMember += HandleOnDisplayCrewMemberDisplay;
        SkillManager.Instance.onLevelUpStat += UpdateUI;

        HandleOnDisplayCrewMemberDisplay(CrewMember.GetSelectedMember);

	}

	void HandleOnDisplayCrewMemberDisplay(CrewMember member)
	{
		UpdateUI ();
	}

	void UpdateUI (){

		if (CrewMember.GetSelectedMember == null)
			return;

		CrewMember member = CrewMember.GetSelectedMember;

		// INFO
//		text.text = "niveau " + member.Level.ToString ();
		text.text = member.Level.ToString ();

		float l = (float)member.CurrentXp / (float)member.xpToLevelUp;
		float width = -backGround.rectTransform.rect.width + backGround.rectTransform.rect.width * l;

		Vector2 v = new Vector2 (width, fillImage.rectTransform.sizeDelta.y);
		fillImage.rectTransform.sizeDelta = v;

		Tween.Bounce (transform);
//
	}

	#region level icons
	void HandleOnWrongLevelEvent ()
	{
		Tween.Bounce (transform);
		TaintLevelImage ();
	}
	void TaintLevelImage() {
		
		text.color = Color.red;
		Invoke ("UntaintLevelImage",1f);
	}
	void UntaintLevelImage () {
		text.color = Color.white;
	}
	#endregion

}
