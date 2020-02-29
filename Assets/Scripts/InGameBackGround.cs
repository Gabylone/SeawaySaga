using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InGameBackGround : MonoBehaviour {

    public enum Type
    {
        Island,
        House,
        Tavern,
        Cave,
        Forest,
        Village,
        Boat,
    }

    public Sprite[] sprites;

	public GameObject group;

    bool visible = false;

	public Image backGroundImage;
	public Image darkImage;

	public float fadeDuration = 1f;

    public Type previousType;
    public Type currentType;

	// Use this for initialization
	void Start () {

        StoryLauncher.Instance.onPlayStory += HandleOnPlayStory;
        StoryLauncher.Instance.onEndStory += HandleOnEndStory;

        Hide();
	}

    void HandleOnPlayStory()
    {
        FadeIn();

        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.boat)
        {
            SetSprite(Type.Boat);
        }
        else
        {
            SetSprite(Type.Island);
        }
    }

    void HandleOnEndStory()
    {
        FadeOut();
    }

    void FadeOut()
    {
        backGroundImage.DOFade(0, fadeDuration);
        darkImage.DOFade(0, fadeDuration);

        Invoke("Hide", fadeDuration);
    }

    public void FadeIn()
    {
        Show();

        darkImage.color = Color.clear;
        backGroundImage.color = Color.clear;

        backGroundImage.DOColor(Color.white, fadeDuration);
        darkImage.DOFade(1, fadeDuration);
    }

    void SetSprite( Type type )
    {
        previousType = currentType;
        currentType = type;

        if ( visible )
        {
            backGroundImage.color = Color.clear;
            backGroundImage.DOFade(0f , fadeDuration);

            Invoke("SetSpriteDelay", fadeDuration);
        }
        else
        {
            backGroundImage.sprite = sprites[(int)currentType];
        }
    }

    void SetSpriteDelay()
    {
        backGroundImage.sprite = sprites[(int)currentType];

        backGroundImage.DOColor(Color.white, fadeDuration);
    }

    void Show()
    {
        group.SetActive(true);
        visible = true;
    }

	void Hide ()
    {
        group.SetActive(false);
        visible = false;
	}
}
