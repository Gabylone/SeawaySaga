using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DisplayHeart : MonoBehaviour {

	public RectTransform backGround;
	public Image fillImage;
	public Image fillImage_Fast;

    public float tweenDuration = 0.75f;
    public float tweenDuration_Fast = 0.4f;

    // Use this for initialization
    void Start () {

		InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		LootUI.useInventory += HandleUseInventory;

		UpdateUI ();
	}

	void HandleOpenInventory ()
	{
		UpdateUI ();
	}

	void UpdateUI () {

		CrewMember member = CrewMember.GetSelectedMember;

        float l = (float)member.Health / (float)member.MemberID.maxHealth;
        float width = -backGround.rect.width + backGround.rect.width * l;

        Vector2 v = new Vector2(width, fillImage.rectTransform.sizeDelta.y);

        //fillImage.rectTransform.sizeDelta = v;

        fillImage.rectTransform.DOSizeDelta(v, tweenDuration_Fast);
        fillImage_Fast.rectTransform.DOSizeDelta(v, tweenDuration).SetDelay(tweenDuration_Fast);
    }

	void HandleUseInventory (InventoryActionType actionType)
	{
		if ( actionType == InventoryActionType.Eat ) {
			UpdateUI ();
		}
	}
}
