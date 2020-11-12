using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class StatGroup : MonoBehaviour {

	[SerializeField]
	private GameObject group;

	public Image levelImage;
	public Text levelText;

	public Image jobImage;

    public float fadeDuration = 0.15f;

    public CanvasGroup canvasGroup;

	// Use this for initialization
	void Start () {
		HideDelay ();
	}

    void Show()
    {
        group.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration);

    }
    void Hide()
    {
        canvasGroup.DOFade(0f, fadeDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", fadeDuration);
    }

    public void HideDelay()
    {
        group.SetActive(false);
    }

	public void Display (CrewMember member) {

        CancelInvoke("Hide");
		Invoke("Hide" , 1f);

		Show ();
		Tween.Bounce (transform);

		levelImage.color = member.GetLevelColor ();
		levelText.text = "" + member.Level;
		jobImage.sprite = SkillManager.jobSprites [(int)member.job];



	}
}
