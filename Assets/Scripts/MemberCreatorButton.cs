using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberCreatorButton : MonoBehaviour {

    public Text pearlPriceUIText;

    public ApparenceItem apparenceItem;

    public Transform initParent;

    public Image image;

    public bool selected = false;

    public float scaleAmount = 2f;

    public MemberCreationScrollView scrollView;

    public static MemberCreatorButton lastSelected;

    public Outline outline;

    public virtual void Start()
    {
        UpdateImage();

        if (Crews.playerCrew.captain.MemberID.GetCharacterID(apparenceItem.apparenceType) == apparenceItem.id)
        {
            Select();
        }
    }

    #region select & deselect
    public void OnPointerDown () {

        if (selected)
        {
            Deselect();
        }
        else
        {
            Select();
        }

    }

    public virtual void OnPointerUp()
    {
        initParent = transform.parent;

        if (apparenceItem.locked)
        {
            DisplayPurchase.Instance.Display(apparenceItem, transform);
            return;
        }

    }

    public virtual void Deselect()
    {
        selected = false;

        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public virtual void Select()
    {
        if ( scrollView == null)
        {
            if (lastSelected != null )
            {
                lastSelected.Deselect();
            }

            lastSelected = this;

        }
        else
        {
            if (scrollView.lastSelected != null)
            {
                scrollView.lastSelected.Deselect();
            }

            scrollView.lastSelected = this;

        }

        Tween.Bounce(transform);

        selected = true;

        if (outline != null)
        {
            outline.enabled = true;
        }

    }
    #endregion

    #region image
    public virtual void UpdateImage() {

        // get member id 
		Member member = Crews.playerCrew.captain.MemberID;

        // 
        apparenceItem = CrewCreator.Instance.GetApparenceItem(apparenceItem.apparenceType, apparenceItem.id);

        if (apparenceItem.apparenceType == ApparenceType.hairColor ||
            apparenceItem.apparenceType == ApparenceType.skinColor ||
                apparenceItem.apparenceType == ApparenceType.topColor ||
            apparenceItem.apparenceType == ApparenceType.pantColor ||
            apparenceItem.apparenceType == ApparenceType.shoesColor)
        {
            image.enabled = false;
            GetComponent<Image>().color = apparenceItem.color;
        }
        else
        {
            if (apparenceItem.GetSprite() == null)
            {
                if ( apparenceItem.apparenceType == ApparenceType.bodyType)
                {
                    image.enabled = false;
                    GetComponentInChildren<Text>().text = "" + (apparenceItem.id + 1);
                }
                else
                {
                    image.sprite = CrewCreator.Instance.noImage_Sprite;
                }
            }
            else
            {
                image.sprite = apparenceItem.GetSprite();
                image.enabled = true;
            }
        }

        if (apparenceItem.locked)
        {
            ShowLockGroup();
        }
        else
        {
            HideLockGroup();
        }
    }
    public void ShowLockGroup()
    {
        //lockGroup.SetActive(true);

        pearlPriceUIText.text = "" + apparenceItem.price;
    }
    public void HideLockGroup()
    {
        //lockGroup.SetActive(false);
    }
    #endregion

    #region enable disable
    void Enable () {
		gameObject.SetActive (true);
	}

    void Disable()
    {
        gameObject.SetActive(false);
    }   
    #endregion
}
