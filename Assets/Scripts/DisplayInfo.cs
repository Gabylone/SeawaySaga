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

	public enum Corner {

		None,

		TopLeft,
		BottomLeft,
		BottomRight,
		TopRight
	}

	public virtual void Start () {

		Hide ();
	}

	public virtual void Move ( Corner corner ) {

		Canvas.ForceUpdateCanvases ();
		LayoutRebuilder.ForceRebuildLayoutImmediate (rectTransform);

		/*switch (corner) {
		case Corner.None:
			float x = (parentRectTransform.rect.width / 2f) - (rectTransform.rect.width/2f);
			float y = -(parentRectTransform.rect.height / 2f) + (rectTransform.rect.height/2f);
                //HOTween.To (rectTransform, tweenDuration, "anchoredPosition", new Vector2 (x,y)  );

                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                break;

		case Corner.TopLeft:

                //HOTween.To (rectTransform, tweenDuration, "anchoredPosition", new Vector2 (250f, -60f) );

                rectTransform.anchorMax = new Vector2(0, 1);
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.pivot = new Vector2(0, 1);

                break;

		case Corner.BottomLeft:
                //HOTween.To (rectTransform, tweenDuration, "anchoredPosition", new Vector2 (250f, -parentRectTransform.rect.height + (rectTransform.rect.height+60f) ) );
                //			HOTween.To (rectTransform, tweenDuration, "anchoredPosition", new Vector2 (200f,- parentRectTransform.rect.height) );

                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 0);

                break;

		case Corner.BottomRight:

                //HOTween.To (rectTransform, tweenDuration, "anchoredPosition", new Vector2 (parentRectTransform.rect.width - rectTransform.rect.width - 10 , -parentRectTransform.rect.height + (rectTransform.rect.height+60f) )  );

                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(1, 0);

                break;

		case Corner.TopRight:

                //HOTween.To (rectTransform, tweenDuration, "anchoredPosition", new Vector2 (parentRectTransform.rect.width - rectTransform.rect.width - 10 ,-60f) );

                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(1, 1);

                break;

		}*/

        //HOTween.To(rectTransform, tweenDuration, "anchoredPosition", Vector2.zero);
        //rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Fade() {

		Tween.Bounce (group.transform);
		Tween.Fade (group.transform, Tween.defaultDuration);

		Invoke ("Hide",Tween.defaultDuration);
	}

	public void Display ( string title, string description ) {
		Show ();
		titleText.text = title;
		descriptionText.text = description;
	}

	public void Show () {

        group.transform.DOKill();
        rectTransform.DOKill();
		CancelInvoke ("Hide");

		Tween.ClearFade (group.transform);
		group.SetActive (true);

		Tween.Bounce (group.transform);
	}

	public void Hide () {

		group.SetActive (false);
		confirmGroup.SetActive (false);
	}

	public virtual void Confirm () {
		confirmGroup.SetActive (false);
		Fade ();

	}

}
