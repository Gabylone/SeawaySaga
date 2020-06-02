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

    public float delay = 1f;

	private bool displaying = false;

	private Vector3 initPos;

	// Use this for initialization
	void Start () {
		initPos = transform.localPosition;
		Hide ();
	}
	void ShowFeedbackInfo() {
		
		// state
		displaying = true;

		// show
		Show ();

		// tween
		Tween.ClearFade (transform);

		// pos
		transform.localPosition = initPos;
        transform.DOLocalMove(initPos + Vector3.up * fadeDecal, fadeDuration);

        // invokes
        CancelInvoke("Fade");
        CancelInvoke("Hide");
        Invoke ("Fade",fadeDuration/2f);
		Invoke ("Hide", fadeDuration);
	}

	#region display status
	public void Display (Fighter.Status status, Color color) {

		if ( displaying && secondCombatFeedback != null) {
			secondCombatFeedback.Display (status, color);
			return;
		}

        text.gameObject.SetActive(false);

        // status
        statusImage.color = color;
        statusImage.gameObject.SetActive(true);
        statusImage.sprite = SkillManager.statusSprites[(int)status];

        CancelInvoke("ShowFeedbackInfo");
        Invoke("ShowFeedbackInfo", delay);
	}
	#endregion

	#region display content
	public void Display (string content, Color color) {

		if ( displaying && secondCombatFeedback != null) {
			secondCombatFeedback.Display (content, color);
			return;
		}

        Debug.Log("displaying : " + content);

        // ui text
        text.gameObject.SetActive(true);
        text.color = color;

        text.text = content;

        // status
        statusImage.gameObject.SetActive(false);

        CancelInvoke("ShowFeedbackInfo");
        Invoke("ShowFeedbackInfo", delay);

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
        text.gameObject.SetActive(false);
        statusImage.gameObject.SetActive(false);
		group.SetActive (false);
	}

}
