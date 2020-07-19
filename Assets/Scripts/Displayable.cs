using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class Displayable : MonoBehaviour
{
    public bool visible = false;

    public GameObject group;

    public float fadeDuration = 0.5f;

    public CanvasGroup canvasGroup;

    private RectTransform rectTransform;

    public virtual void Start()
    {
        if (visible)
        {
            Show();
        }
        else
        {
            HideDelay();
        }
    }

    public void Show()
    {
        visible = true;
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration);

        CancelInvoke("HideDelay");

        group.SetActive(true);
    }

    public void Hide()
    {
        visible = false;
        canvasGroup.DOFade(0f, fadeDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", fadeDuration);
    }

    public virtual void HideDelay()
    {
        group.SetActive(false);
    }

    public RectTransform GetRectTransform()
    {
        if ( rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        return rectTransform;
    }
}
