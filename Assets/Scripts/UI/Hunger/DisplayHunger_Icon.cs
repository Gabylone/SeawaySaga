using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DisplayHunger_Icon : DisplayHunger {

	private MemberIcon linkedIcon;

	public int hungerToAppear = 50;

    public GameObject heartGroup;
    public RectTransform healthBackground;
	public Image heartImage;

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
        if (linkedIcon.member.CurrentHunger >= Crews.maxHunger)
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
        displayFood_Text.text = "- " +i;

        CancelInvoke("HideFoodFeedback");
        Invoke("HideFoodFeedback", displayFood_Delay + 1f);
    }

    void HideFoodFeedback()
    {
        displayFood_Obj.SetActive(false);
    }

    #region health
    void ShowHeart () {
		heartGroup.SetActive (true);
		Tween.Bounce (heartGroup.transform);
		UpdateHeartImage ();
	}
	void HideHeart() {
		heartGroup.SetActive (false);
		//
	}
	void UpdateHeartImage () {

		float l = (float)linkedIcon.member.Health / (float)linkedIcon.member.MemberID.maxHealth;

        float width = -healthBackground.rect.width + healthBackground.rect.width * l;

        Vector2 v = new Vector2(width, heartImage.rectTransform.sizeDelta.y);
        heartImage.rectTransform.sizeDelta = v;

    }
	#endregion

	void HandleEndStoryEvent ()
	{
        ShowHeart();
		UpdateHungerIcon (linkedIcon.member);
	}

	void HandlePlayStoryEvent ()
	{
        HideInfo();
	}

	void HandleCloseInventory ()
	{
		if (StoryLauncher.Instance.PlayingStory == false) {
            ShowHeart();
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
		UpdateHungerIcon (linkedIcon.member);
        HideFoodFeedback();

        if ( NavigationManager.Instance.chunksTravelled > 1)
        {
            CancelInvoke("ShowFoodFeedback");
            Invoke("ShowFoodFeedback", displayFood_Delay);
        }
    }

	public override void UpdateHungerIcon (CrewMember member)
	{
		float fillAmount = 1f - ((float)member.CurrentHunger / (float)Crews.maxHunger);

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
        UpdateHeartImage();
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
