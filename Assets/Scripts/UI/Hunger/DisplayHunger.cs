using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

public class DisplayHunger : MonoBehaviour {

    public GameObject hungerGroup;

    public Image fillImage;
    public Image fillImage_Fast;

	public float maxFillAmountScale = 1f;
	public Image backGroundImage;

    public float tweenDuration = 0.7f;
    public float fastTweenDuration = 0.35f;
    public bool tween = true;

    public virtual void Start () {
		
	}

	public virtual void ShowHunger () {

		hungerGroup.SetActive (true);
	}
	public virtual void HideHunger () {
		hungerGroup.SetActive (false);
	}

	public virtual void UpdateHungerIcon ( CrewMember member ) {

		ShowHunger ();

        fillImage.rectTransform.DOKill();

		float fillAmount = 1f - ((float)member.CurrentHunger / (float)member.MaxHunger);
		Vector2 v = new Vector2 (fillImage.rectTransform.rect.width, (fillAmount * maxFillAmountScale));

        fillImage_Fast.rectTransform.DOSizeDelta(v,fastTweenDuration);
        fillImage.rectTransform.DOSizeDelta(v,tweenDuration).SetDelay(fastTweenDuration);

        Tween.Bounce (transform, 0.2f, 1.1f);

	}
}
