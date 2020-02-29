using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class CombatFeedback : MonoBehaviour {

	public Text text;
	public Image statusImage;
	public GameObject group;
	public Image backgroundImage;

	public float fadeDecal = 1f;
	public float fadeDuration = 1f;

	public CombatFeedback secondCombatFeedback;
	private float secondFeedbackDelay = 1.2f;

	private bool displaying = false;

	private Vector3 initPos;

	// Use this for initialization
	void Start () {
		initPos = transform.localPosition;
		Hide ();
	}

	// display
	public void ShowFeedbackInfo(float delay) {
		Invoke ("ShowFeedbackInfo", delay);
	}
	void ShowFeedbackInfo() {
		
		// state
		displaying = true;

		// show
		Show ();

		// tween
		Tween.ClearFade (transform);
		CancelInvoke ("Fade");
		CancelInvoke ("Hide");

		// pos
		transform.localPosition = initPos;
        transform.DOLocalMove(initPos + Vector3.up * fadeDecal, fadeDuration);

		// invokes
		Invoke ("Fade",fadeDuration/2f);
		Invoke ("Hide", fadeDuration);
	}

	#region display status
	public void Display (Fighter.Status status, Color color) {

		if ( displaying && secondCombatFeedback != null) {
			secondCombatFeedback.Display (status, color);
			return;
		}

		SetFeedbackInfo (status, color);
		ShowFeedbackInfo ();
	
	}
	void SetFeedbackInfo (Fighter.Status status, Color color)
	{
		// bg
		backgroundImage.color = color;

		// ui text
		text.gameObject.SetActive (false);

		// status
		statusImage.gameObject.SetActive (true);
		statusImage.sprite = SkillManager.statusSprites [(int)status];
	}
	#endregion

	#region display content
	public void Display (string content, Color color) {

		if ( displaying && secondCombatFeedback != null) {
			secondCombatFeedback.Display (content, color);
			return;
		}

		SetFeedbackInfo (content, color);
		ShowFeedbackInfo ();

	}
	void SetFeedbackInfo (string content, Color color)
	{
		// bg
		backgroundImage.color = color;

		// ui text
		text.gameObject.SetActive (true);
		text.text = content;

		// status
		statusImage.gameObject.SetActive (false);
	}
	#endregion

	// tools
	void Fade() {
		Tween.Fade (transform, fadeDuration/2f);
	}

	void Show() {
		group.SetActive (true);
		Tween.Bounce (transform);
	}

	void Hide () {
		displaying = false;
		group.SetActive (false);
	}

}
