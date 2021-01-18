using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class DisplayKarma : MonoBehaviour {

    public static DisplayKarma Instance;

    public GameObject group;

    public float max = 88.5f;

    public RectTransform targetRectTranform;

    public float tweenDuration = 0.3f;

    public Transform _transform;
    public float jaugeWidth = 70f;

    public RectTransform jauge_RectTransform;
    public Image jauge_Image;
    public Color badColor;
    public Color goodColor;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        _transform = GetComponent<Transform>();

        UpdateUI();
    }

    public void UpdateUI()
    {
        float x = 0f;

        switch (Karma.Instance.karmaStep)
        {
            case Karma.KarmaStep.Best:
                x = -max;
                break;
            case Karma.KarmaStep.Good:
                x = -max / 2;
                break;
            case Karma.KarmaStep.Neutral:
                x = 0f;
                break;
            case Karma.KarmaStep.Bad:
                x = max / 2;
                break;
            case Karma.KarmaStep.Worst:
                x = max;
                break;
            default:
                break;
        }

        targetRectTranform.DOAnchorPos(Vector2.right * x, tweenDuration);

        if ( Karma.Instance.CurrentKarma >= 0)
        {
            float w = (float)Karma.Instance.CurrentKarma / Karma.Instance.maxKarma * jaugeWidth;

            jauge_RectTransform.pivot = new Vector2(0, 0.5f);
            jauge_RectTransform.anchorMin = new Vector2(0f, 0.5f);
            jauge_RectTransform.anchorMax = new Vector2(0f, 0.5f);

            jauge_RectTransform.anchoredPosition = Vector2.zero;

            jauge_RectTransform.sizeDelta = new Vector2(w, jauge_RectTransform.sizeDelta.y);
            jauge_Image.color = goodColor;
        }
        else
        {
            float w = (float)Karma.Instance.CurrentKarma / Karma.Instance.maxKarma * jaugeWidth;

            jauge_RectTransform.pivot = new Vector2(1, 0.5f);
            jauge_RectTransform.anchorMin = new Vector2(1f, 0.5f);
            jauge_RectTransform.anchorMax = new Vector2(1f, 0.5f);

            jauge_RectTransform.anchoredPosition = Vector2.zero;

            jauge_RectTransform.sizeDelta = new Vector2(-w, jauge_RectTransform.sizeDelta.y);
            jauge_Image.color = badColor;
        }

        //targetRectTranform.anchoredPosition = Vector2.right * x;

        Tween.Bounce(_transform);

    }

    public void OnPointerClick()
    {
        KarmaFeedback.Instance.DisplayKarma();

        Tween.Bounce(_transform);
    }
}
