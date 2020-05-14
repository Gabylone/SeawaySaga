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

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        StoryInput.Instance.onPressInput += HandleOnPressInput;

        Hide();
        
	}

	void HandleOnPressInput ()
	{
        if (visible)
        {
            Hide();
            StoryReader.Instance.ContinueStory();
        }
    }

	public void DisplayItem (Item item)
	{
        displayItem.Show(item);

        StoryInput.Instance.WaitForInput();

        Show();

		Tween.Bounce (displayItem.transform);
	}

    void Show()
    {
        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1f, 0.2f);

        group.SetActive(true);

        Invoke("ShowDelay" , 0.2f);
    }

    void ShowDelay()
    {
        visible = true;
    }

    void Hide()
    {
        visible = false;

        canvasGroup.DOFade(0f, 0.2f);

        Invoke("HideDelay", 0.2f);
    }

    void HideDelay()
    {
        group.SetActive(false);
    }
}
