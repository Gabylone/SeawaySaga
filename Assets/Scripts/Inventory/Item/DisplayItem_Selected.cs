using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class DisplayItem_Selected : DisplayItem {

    public GameObject group;

    [SerializeField] private Text nameText;
    [SerializeField] private Text paramText;
    public Outline paramOutline;
    [SerializeField] private Text priceText;
    [SerializeField] private Text weightText;
    [SerializeField] private Text descriptionText;

    public CanvasGroup canvasGroup;

    [SerializeField] private GameObject levelObj;
    public Outline levelOutline;
    [SerializeField] private Text levelText;

    public Image itemImage;

    public override void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        base.Start();

        CrewMember.onWrongLevel += HandleOnWrongLevelEvent;

    }

    public void UpdateUI()
    {
        DisplayedItem = DisplayedItem;
    }

    void HandleOnWrongLevelEvent()
    {
        Tween.Bounce(_transform);
    }

    public override Item DisplayedItem
    {
        get
        {
            return base.DisplayedItem;
        }

        set
        {
            base.DisplayedItem = value;

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
                    paramOutline.enabled = true;
                    //paramText.color = Color.green;
                    paramOutline.effectColor = Color.green;
                }
                else
                {
                    if (value.value > CrewMember.GetSelectedMember.GetEquipment(part).value)
                    {
                        paramOutline.enabled = true;
                        paramOutline.effectColor = Color.green;
                        //paramText.color = Color.green;
                    }
                    else if (value.value < CrewMember.GetSelectedMember.GetEquipment(part).value)
                    {
                        paramOutline.enabled = true;
                        paramOutline.effectColor = Color.red; ;
                        //paramText.color = Color.red;
                    }
                    else
                    {
                        paramText.color = Color.white;
                        paramOutline.enabled = false;
                    }
                }
            }
            else
            {
                paramOutline.enabled = false;
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

                if ( value.level > CrewMember.GetSelectedMember.Level)
                {
                    levelOutline.enabled = true;
                }
                else
                {
                    levelOutline.enabled = false;
                }
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

        Tween.Bounce(_transform, 0.1f, 1.025f);

        canvasGroup.DOFade(1f, 0.3f);

        SoundManager.Instance.PlayRandomSound("paper tap");

        CancelInvoke("HideDelay");
    }

    public override void Hide()
    {
        base.Hide();

        canvasGroup.DOFade(0f, 0.3f);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", 0.3f);

    }

    void HideDelay()
    {
        group.SetActive(false);

    }

}
