using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayName : MonoBehaviour {

	[SerializeField]
	private Text textUI_Name;

	// Use this for initialization
	void Start () {
		InGameMenu.Instance.onDisplayCrewMember += OnDisplayCrewMember;

		if ( CrewMember.GetSelectedMember != null)
		OnDisplayCrewMember (CrewMember.GetSelectedMember);
	}

	void OnDisplayCrewMember (CrewMember member)
	{
		if (CrewMember.GetSelectedMember == null)
			return;

		textUI_Name.text = member.MemberName;
	}
}
