using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayItemPrice : MonoBehaviour {

    public Text uiText;

    private void OnEnable()
    {
        if (LootUI.Instance.SelectedItem != null)
        {
            int price = LootUI.Instance.SelectedItem.price;

            if (LootUI.Instance.categoryContentType != CategoryContentType.OtherTrade)
            {
                price = 1 + (int)(LootUI.Instance.SelectedItem.price / 3f);
            }
            else
            {
                if (LootUI.Instance.SelectedItem.price > GoldManager.Instance.goldAmount)
                {
                    uiText.color = Color.red;
                }
                else
                {
                    uiText.color = Color.white;
                }
            }

            uiText.text = "" + price;
        }
    }
}
