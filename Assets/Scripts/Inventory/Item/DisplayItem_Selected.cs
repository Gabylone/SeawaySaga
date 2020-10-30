using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItem_Selected : DisplayItem {

    public GameObject group;

    [SerializeField] private Text nameText;
    [SerializeField] private Text paramText;
    [SerializeField] private Text priceText;
    [SerializeField] private Text weightText;
    [SerializeField] private Text descriptionText;

    [SerializeField] private GameObject levelObj;
    [SerializeField] private Text levelText;

    public Image itemImage;

    public Vector2 decalToItem;

    public override void Start()
    {
        base.Start();

        CrewMember.onWrongLevel += HandleOnWrongLevelEvent;

    }

    void HandleOnWrongLevelEvent()
    {
        Tween.Bounce(transform);
    }

    public override Item HandledItem
    {
        get
        {
            return base.HandledItem;
        }

        set
        {
            base.HandledItem = value;

            if ( itemImage != null )
            {
                itemImage.enabled = true;
                itemImage.sprite = LootManager.Instance.getItemSprite(value.category, value.spriteID);
            }
            /// NAME
            nameText.text = "" + value.englishName;

            /// DESCRIPTION
            descriptionText.text = "" + value.description;

            /// VALUE
            paramText.text = "" + value.value;

            paramText.gameObject.SetActive(value.value > 0);

            if ( value.category == ItemCategory.Clothes || value.category == ItemCategory.Weapon )
            {
                CrewMember.EquipmentPart part = value.category == ItemCategory.Weapon ? CrewMember.EquipmentPart.Weapon : CrewMember.EquipmentPart.Clothes;

                if (CrewMember.GetSelectedMember.GetEquipment(part) == null)
                {
                    paramText.color = Color.green;
                }
                else
                {
                    if (value.value > CrewMember.GetSelectedMember.GetEquipment(part).value)
                        paramText.color = Color.green;
                    else if (value.value < CrewMember.GetSelectedMember.GetEquipment(part).value)
                        paramText.color = Color.red;
                    else
                        paramText.color = Color.white;
                }
            }
            else
            {
                paramText.color = Color.white;
            }

            /// PRICE 
            int price = 0;

            if (LootUI.Instance.categoryContentType == CategoryContentType.OtherTrade)
            {
                price = value.price;
            }
            else
            {
                price = 1 + (int)(value.price / 3f);
            }

            priceText.text = "" + value.price;

            if (LootUI.Instance.currentSide == Crews.Side.Enemy)
            {
                if (value.price > GoldManager.Instance.goldAmount && OtherInventory.Instance.type == OtherInventory.Type.Trade)
                    priceText.color = Color.red;
                else
                    priceText.color = Color.white;
            }
            else
            {
                priceText.color = Color.white;
            }

            /// WEIGHT 
            weightText.text = "" + value.weight;

            if (LootUI.Instance.currentSide == Crews.Side.Enemy)
            {
                if (value.weight + WeightManager.Instance.CurrentWeight > WeightManager.Instance.CurrentCapacity)
                    weightText.color = Color.red;
                else
                    weightText.color = Color.white;
            }
            else
            {
                weightText.color = Color.white;
            }

            // LEVEL
            if ( value.level > 0)
            {
                levelObj.SetActive(true);
                levelText.text = "" + value.level;
            }
            else
            {
                levelObj.SetActive(false);
            }
        }

    }

    public override void Show(Item item)
    {
        base.Show(item);

        group.SetActive(true);

        SoundManager.Instance.PlayRandomSound("paper tap");
    }

    public override void Hide()
    {
        base.Hide();

        group.SetActive(false);
    }

}
