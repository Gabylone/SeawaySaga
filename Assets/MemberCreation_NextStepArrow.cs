using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class MemberCreation_NextStepArrow : MonoBehaviour
{

    public GameObject group;

    public CanvasGroup CanvasGroup;

    public float tweenDuration = 0.2f;

    private void Start()
    {
        //HideDelay();
    }

    public void OnPointerClick()
    {
        Tween.Bounce(transform);

        Hide();
        
        CancelInvoke("OnPointerClickDelay");
        Invoke("OnPointerClickDelay", tweenDuration);
    }

    void OnPointerClickDelay()
    {
        MemberCreator.Instance.Confirm();

    }

    public void Show()
    {
        CanvasGroup.interactable = true;

        group.SetActive(true);

        CanvasGroup.alpha = 0f;
        CanvasGroup.DOFade(1f, tweenDuration);
    }

    public void Hide()
    {
        CanvasGroup.interactable = false;

        CanvasGroup.DOFade(0f, tweenDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", tweenDuration);
    }

    public void HideDelay()
    {
        group.SetActive(false);
    }
}
