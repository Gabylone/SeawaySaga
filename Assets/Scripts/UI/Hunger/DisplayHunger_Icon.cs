using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DisplayHunger_Icon : DisplayHunger {

	private MemberIcon linkedIcon;

	public int hungerToAppear = 50;

    bool heart_Visible = false;
    public GameObject heartGroup;
    public Transform heart_Transform;
    public CanvasGroup heart_CanvasGroup;
    public RectTransform healthBackground;

	public Image heartImage;
    public Image heartImage_Fast;

    public float hungerToShowLife = 25f;

        // display food
    public GameObject displayFood_Obj;
    public float displayFood_Delay = 1.2f;
    public Vector2 displayFood_InitPos;
    public float displayFood_TargetXPos;
    public Text displayFood_Text;
    public Image displayFood_Image;
    public Sprite displayFood_FoodSprite;
    public Sprite displayFood_HeartSprite;

    public float lerpToHideHunger = 0.3f;


    public override void Start ()
	{
		base.Start ();

		linkedIcon = GetComponentInParent<MemberIcon> ();

		HideHunger ();
		HideHeart ();

        HideFoodFeedback();

        InitEvents();

        displayFood_InitPos = displayFood_Obj.GetComponent<RectTransform>().anchoredPosition;
    }

    void ShowFoodFeedback()
    {
        UpdateHungerIcon(linkedIcon.member);

        if (linkedIcon.member.CurrentHunger >= linkedIcon.member.MaxHunger)
        {

            DisplayHealthAmount(linkedIcon.member.hungerDamage);
        }
        else
        {
            DisplayFoodAmount(1);
        }

    }

    public void DisplayHealthAmount(int i)
    {
        HideHeart();

        displayFood_Obj.SetActive(true);

        if (hungerGroup.activeSelf)
        {
            displayFood_Obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(displayFood_TargetXPos, displayFood_InitPos.y);
        }
        else
        {
            displayFood_Obj.GetComponent<RectTransform>().anchoredPosition = displayFood_InitPos;
        }

        displayFood_Image.sprite = displayFood_HeartSprite;
        displayFood_Text.text = "- " + i;

        CancelInvoke("HideFoodFeedback");
        Invoke("HideFoodFeedback", displayFood_Delay + 1f);
    }

    public void DisplayFoodAmount(int i)
    {
        HideHeart();

        if ( hungerGroup.activeSelf)
        {
            displayFood_Obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(displayFood_TargetXPos, displayFood_InitPos.y);
        }
        else
        {
            displayFood_Obj.GetComponent<RectTransform>().anchoredPosition = displayFood_InitPos;
        }

        displayFood_Obj.SetActive(true);

        displayFood_Image.sprite = displayFood_FoodSprite;
        displayFood_Text.text = "" + (linkedIcon.member.MaxHunger - linkedIcon.member.CurrentHunger);

        CancelInvoke("HideFoodFeedback");
        Invoke("HideFoodFeedback", displayFood_Delay + 1f);
    }

    void HideFoodFeedback()
    {
        UpdateHeartUI();

        displayFood_Obj.SetActive(false);
    }

    #region health
    void ShowHeart()
    {
        heart_Visible = true;

        heart_CanvasGroup.alpha = 0f;
        heart_CanvasGroup.DOFade(1f, 0.2f);
        heartGroup.SetActive(true);
        Tween.Bounce(heart_Transform);
        UpdateHeartImage();
    }
    void UpdateHeartUI () {

        if (linkedIcon.currentPlacingType != Crews.PlacingType.Portraits)
        {
            HideHeart();
        }

        if (linkedIcon.member.HasMaxHealth())
        {
            if (heart_Visible)
            {
                HideHeart();

            }
        }
        else
        {
            if (!heart_Visible)
            {
                ShowHeart();
            }
        }

        UpdateHeartImage();
	}
	void HideHeart() {
        heart_Visible = false;

        heart_CanvasGroup.DOFade(0f, 0.2f);

        CancelInvoke("HideHeartDelay");
        Invoke("HideHeartDelay", 0.2f);
        //
    }
    void HideHeartDelay()
    {
        heartGroup.SetActive(false);

    }

    void UpdateHeartImage () {

		float l = (float)linkedIcon.member.Health / (float)linkedIcon.member.MemberID.maxHealth;

        float width = -healthBackground.rect.width + healthBackground.rect.width * l;

        Vector2 v = new Vector2(width, heartImage.rectTransform.sizeDelta.y);

        heartImage_Fast.rectTransform.sizeDelta = heartImage.rectTransform.sizeDelta;
        heartImage.rectTransform.DOSizeDelta(v, fastTweenDuration);
        heartImage_Fast.rectTransform.DOSizeDelta(v, tweenDuration).SetDelay(fastTweenDuration);
    }
    #endregion

    void HandleEndStoryEvent ()
	{
        UpdateHeartUI();
		UpdateHungerIcon (linkedIcon.member);
	}

	void HandlePlayStoryEvent ()
	{
        HideInfo();
	}

	void HandleCloseInventory ()
	{
		if (StoryLauncher.Instance.PlayingStory == false) {
            UpdateHeartUI();
            UpdateHungerIcon(linkedIcon.member);
        }
	}

    void HandleOnOpenInventory()
    {
        HideInfo();
    }

    public void HideInfo()
    {
        HideHeart();
        HideHunger();
    }

	void HandleOnAddHunger ()
	{
        HideFoodFeedback();

        if ( NavigationManager.Instance.chunksTravelled > 1)
        {
            CancelInvoke("ShowFoodFeedback");
            Invoke("ShowFoodFeedback", displayFood_Delay);
        }
    }

	public override void UpdateHungerIcon (CrewMember member)
	{
		float fillAmount = 1f - ((float)member.CurrentHunger / (float)member.MaxHunger);

        UpdateHeartImage();

        /*if (fillAmount * 100 < hungerToShowLife) {
		    HideHunger ();
        } else */
        if (fillAmount * 100 < hungerToAppear) {
			base.UpdateHungerIcon (member);
		} else {
			HideHunger ();
		}

	}

    public override void ShowHunger()
    {
        base.ShowHunger();
    }

    public override void HideHunger()
    {
        base.HideHunger();
    }

    void HandleOnChangeHealth()
    {
        UpdateHeartUI();
    }

    void InitEvents()
    {
        //NavigationManager.Instance.EnterNewChunk    += HandleOnAddHunger;
        linkedIcon.member.onAddHunger += HandleOnAddHunger;

        StoryLauncher.Instance.onPlayStory          += HandlePlayStoryEvent;
        StoryLauncher.Instance.onEndStory           += HandleEndStoryEvent;

        InGameMenu.Instance.onCloseMenu             += HandleCloseInventory;
        InGameMenu.Instance.onOpenMenu              += HandleOnOpenInventory;

        linkedIcon.member.onChangeHealth            += HandleOnChangeHealth;

        
    }

    void OnDestroy()
    {
        //NavigationManager.Instance.EnterNewChunk    -= HandleOnAddHunger;
        linkedIcon.member.onAddHunger -= HandleOnAddHunger;

        StoryLauncher.Instance.onPlayStory          -= HandlePlayStoryEvent;
        StoryLauncher.Instance.onEndStory           -= HandleEndStoryEvent; 

        InGameMenu.Instance.onCloseMenu             -= HandleCloseInventory;
        InGameMenu.Instance.onOpenMenu              -= HandleOnOpenInventory;

        linkedIcon.member.onChangeHealth            -= HandleOnChangeHealth;

    }
}
