using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatGroup : MonoBehaviour {

	[SerializeField]
	private GameObject group;

	public Image levelImage;
	public Text levelText;

	public Image jobImage;

	// Use this for initialization
	void Start () {
		Hide ();
	}

	void Show () {
		group.SetActive (true);
	}
	void Hide () 
	{
		group.SetActive (false);
	}

	public void Display (CrewMember member) {

		CancelInvoke ();
		Invoke("Hide" , 1f);

		Show ();
		Tween.Bounce (transform);

		levelImage.color = member.GetLevelColor ();
		levelText.text = "" + member.Level;
		jobImage.sprite = SkillManager.jobSprites [(int)member.job];



	}
}
