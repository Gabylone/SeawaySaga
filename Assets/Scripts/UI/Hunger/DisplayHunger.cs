using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using DG.Tweening;

public class DisplayHunger : MonoBehaviour {

	public GameObject hungerGroup;
	public Image fillImage;
	float maxFillAmountScale = 1f;
	public Image backGroundImage;

    public bool tween = true;

    public virtual void Start () {
		maxFillAmountScale = fillImage.rectTransform.rect.height;
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

		float fillAmount = 1f - ((float)member.CurrentHunger / (float)Crews.maxHunger);
		Vector2 v = new Vector2 (fillImage.rectTransform.rect.width, (fillAmount * maxFillAmountScale));

        if ( tween )
        {
            fillImage.rectTransform.DOSizeDelta(v, 0.5f);
        }
        else
        {
            fillImage.rectTransform.sizeDelta = v;
        }

        Tween.Bounce (transform, 0.2f, 1.1f);

	}
}
