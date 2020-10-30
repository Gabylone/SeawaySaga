using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class DisplayStoryItem : MonoBehaviour {

    public static DisplayStoryItem Instance;

	[SerializeField]
	private DisplayItem_Selected displayItem;

	[SerializeField]
	private GameObject group;

    public bool visible = false;

    public CanvasGroup canvasGroup;

    public float fadeDuration = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        Hide();
        StoryInput.Instance.onPressInput += HandleOnPressInput;

    }

    void HandleOnPressInput ()
	{
        if (!visible)
        {
            return;
        }

        Hide();

        Invoke("HandleOnPressInputDelay", fadeDuration + 0.1f);
    }

    void HandleOnPressInputDelay()
    {
        StoryReader.Instance.ContinueStory();
    }

    public void DisplayItem(Item item)
    {
        displayItem.Show(item);

        Show();

        Tween.Bounce(displayItem.transform);

        Invoke("DisplayItemDelay", fadeDuration);
    }

    void DisplayItemDelay()
    {
        StoryInput.Instance.WaitForInput();
    }

    void Show()
    {
        canvasGroup.alpha = 0f;

        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, fadeDuration);

        group.SetActive(true);

        CancelInvoke("HideDelay");
        CancelInvoke("ShowDelay");
        Invoke("ShowDelay" , fadeDuration);
    }

    void ShowDelay()
    {
        visible = true;
    }

    void Hide()
    {
        visible = false;

        canvasGroup.DOFade(0f, fadeDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", fadeDuration);
    }

    void HideDelay()
    {
        group.SetActive(false);
    }
}
