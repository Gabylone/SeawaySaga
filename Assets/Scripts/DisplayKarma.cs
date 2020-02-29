using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class DisplayKarma : MonoBehaviour {

    public static DisplayKarma Instance;

    public GameObject group;

    public float max = 88.5f;

    public RectTransform targetRectTranform;

    public float tweenDuration = 0.3f;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
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

        //targetRectTranform.anchoredPosition = Vector2.right * x;

        Tween.Bounce(group.transform);

    }

    public void OnPointerClick()
    {
        KarmaFeedback.Instance.DisplayKarma();

        Tween.Bounce(transform);
    }
}
