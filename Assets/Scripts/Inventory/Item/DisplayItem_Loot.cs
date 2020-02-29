using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItem_Loot : DisplayItem {

    [Header("UI elements")]
    public Button button;
    public Image image;

    public GameObject group;

	public static DisplayItem_Loot selectedDisplayItem = null;

	public Image itemImage;

	public int index = 0;

	public bool selected = false;

    private void Awake()
    {
        selectedDisplayItem = null;
    }

    public override void Start()
    {
        base.Start();

        itemImage.transform.rotation = Quaternion.Euler (new Vector3 (0, 0, Random.Range (-30, 30)));

	}

    public void EquipmentSelect()
    {
        if (LootUI.Instance.currentSide == Crews.Side.Enemy)
            return;
        Select();
    }

	public void Select () {

		if ( selected ) {
			Deselect ();
			return;
		}

        // select
		if (selectedDisplayItem != null) {
			selectedDisplayItem.Deselect ();
		}

        selectedDisplayItem = this;
        selected = true;

		SoundManager.Instance.PlaySound (SoundManager.Sound.Select_Small);

		LootUI.Instance.SelectedItem = HandledItem;
		LootUI.Instance.selectedItemDisplay.transform.position = (Vector2)transform.position + LootUI.Instance.selectedItemDisplay.decalToItem;

		Tween.Bounce (transform);

		button.image.color = LootManager.Instance.selectedButtonColor;

	}

	public void Deselect () {

		selectedDisplayItem = null;

		selected = false;

		LootUI.Instance.ClearSelectedItem();

		SoundManager.Instance.PlaySound (SoundManager.Sound.Select_Small);

		UpdateColor ();

	}

	void UpdateColor ()
	{
		if (HandledItem == null ) {
			return;
		}

		float a = 0.7f;

        /*if (CrewMember.GetSelectedMember.GetEquipment(HandledItem.EquipmentPart) == HandledItem && index == 0 )
        {
            if (HandledItem.category == ItemCategory.Clothes || HandledItem.category == ItemCategory.Weapon)
            {
                image.sprite = LootUI.Instance.equipedItemSprite;

                //image.color = LootManager.Instance.item_EquipedColor;
                image.color = Color.white;

                return;
            }
        }

        image.sprite = LootUI.Instance.itemSprite;*/

        if ( HandledItem.level > CrewMember.GetSelectedMember.Level ) {

            image.color = LootManager.Instance.item_SuperiorColor;
            image.color = new Color(1f, a, a);

        } else if ( HandledItem.level < CrewMember.GetSelectedMember.Level && HandledItem.level > 0 ) {

            image.color = LootManager.Instance.item_InferiorColor;
            image.color = new Color(a, 1f, a);

        } else {

            image.color = LootManager.Instance.item_DefaultColor;

        }
	}

    public void UpdateBackGroundColor(Item handledItem)
    {
        if ( handledItem.category == ItemCategory.Clothes || handledItem.category == ItemCategory.Weapon)
        {
            CrewMember.EquipmentPart part = handledItem.category == ItemCategory.Weapon ? CrewMember.EquipmentPart.Weapon : CrewMember.EquipmentPart.Clothes;

            if (CrewMember.GetSelectedMember.GetEquipment(part) == null)
            {
                return;
            }

            if (CrewMember.GetSelectedMember.GetEquipment(part) == handledItem)
            {
                Color myColor = new Color();
                ColorUtility.TryParseHtmlString("#BB79BEFF", out myColor);
                image.color = myColor;
            }
            else
            {
                Color myColor = new Color();
                ColorUtility.TryParseHtmlString("#B2884FFF", out myColor);
                image.color = myColor;

            }
        }
    }

    public override Item HandledItem {
		get {
			return base.HandledItem;
		}
		set {
			
			base.HandledItem = value;

			if (value == null) {
				group.SetActive (false);
				return;
			}

			group.SetActive (true);

			UpdateColor ();

            itemImage.enabled = true;
            itemImage.sprite = LootManager.Instance.getItemSprite(value.category, value.spriteID);

            /*if (value.spriteID < 0) {
				itemImage.enabled = false;
			} else {
			}*/

		}
	}
}
