using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class DisplayInfo : MonoBehaviour {

	public GameObject group;

	public RectTransform parentRectTransform;
	public RectTransform rectTransform;

	public Text titleText;
	public Text descriptionText;

	public GameObject confirmGroup;

	public float tweenDuration = 1f;

    public CanvasGroup canvasGroup;

	public enum Corner {

		None,

		TopLeft,
		BottomLeft,
		BottomRight,
		TopRight
	}

	public virtual void Start () {

		HideDelay ();
	}

	public virtual void Move ( Corner corner ) {

		Canvas.ForceUpdateCanvases ();
		LayoutRebuilder.ForceRebuildLayoutImmediate (rectTransform);
    }

	public void Display ( string title, string description ) {
		Show ();
		titleText.text = title;
		descriptionText.text = description;
	}

	public void Show () {

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, tweenDuration);

        group.transform.DOKill();
        rectTransform.DOKill();
		CancelInvoke ("Hide");
        CancelInvoke("HideDelay");

        Tween.ClearFade (group.transform);
		group.SetActive (true);

		Tween.Bounce (group.transform);
	}

	public void Hide () {

        canvasGroup.DOFade(0f, tweenDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", tweenDuration);
	}

    void HideDelay()
    {
        group.SetActive(false);
        confirmGroup.SetActive(false);
    }

    public virtual void Confirm () {
		confirmGroup.SetActive (false);
		Hide ();
	}

}
