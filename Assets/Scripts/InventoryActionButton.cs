using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryActionButton : MonoBehaviour {

	Button button;

	public InventoryActionType inventoryActionType;

	public GameObject descriptionGroup;
	public RectTransform descriptionRectTransform;

	bool touching = false;

	public float timeToShowDescription = 1f;
	public float timeToShowDescriptionFeedback = 0.15f;
	public float targetWidth = 0f;

	public float tweenDuration = 0.2f;

	public GameObject fillGroup;
	public Image fillImage;

    public bool showDescription = true;

    void Start()
    {
        fillGroup.SetActive(false);
        descriptionGroup.SetActive(false);

        //ShowDescription();
    }

    private void OnEnable()
    {
        //ShowDescription();
    }

    public void OnPointerDown () {

        TriggerAction ();

        /*
		touching = true;
         * 
         * 
         * CancelInvoke("OnPointerDownDelay");
		CancelInvoke ("OnPointerDownDelayFeedback");
		Invoke ("OnPointerDownDelay" , timeToShowDescription);
		Invoke ("OnPointerDownDelayFeedback", timeToShowDescriptionFeedback);*/

    }

    void OnPointerDownDelay () {

		if (touching) {
			ShowDescription ();
		}

		touching = false;

	}

	void OnPointerDownDelayFeedback () {
		
		if (!touching)
			return;

		Tween.Bounce (transform);

		fillGroup.SetActive (true);
		fillImage.fillAmount = 0f;

        fillImage.DOKill();
        fillImage.DOFillAmount(1f, timeToShowDescription - timeToShowDescriptionFeedback);

	}

	void HideDescription ()
	{
		descriptionGroup.SetActive (false);
		fillGroup.SetActive (false);
	}

	void ShowDescription () {
        if (!showDescription)
            return;
		Tween.Bounce (transform);

		descriptionGroup.SetActive (true);

		float y = descriptionRectTransform.sizeDelta.y;

		descriptionRectTransform.sizeDelta = new Vector2 ( 0f, y );

		Vector2 targetScale = new Vector2 ( targetWidth , y );

        descriptionRectTransform.DOSizeDelta(targetScale, tweenDuration);

		fillGroup.SetActive (false);
	}

	public void OnPointerUp () {

		if ( touching ) {
			TriggerAction ();
		}

		touching = false;

		HideDescription ();
	}


	/// <summary>
	/// Triggers the action.
	/// </summary>
	void TriggerAction () {
		
		Tween.Bounce (transform);

		TriggerActionDelay ();

		/*CancelInvoke ("TriggerActionDelay");
		Invoke ("TriggerActionDelay" , Tween.defaultDuration);*/

	}

	void TriggerActionDelay () {
		LootUI.Instance.InventoryAction (inventoryActionType);

		//
	}

}
