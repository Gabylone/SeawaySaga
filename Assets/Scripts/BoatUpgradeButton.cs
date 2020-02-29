using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatUpgradeButton : MonoBehaviour {

    public BoatUpgradeManager.UpgradeType upgradeType;

    public Button goldButton;
    public Text goldText;

    public RectTransform fillImage;
    public RectTransform backgroundImage;

    public int max = 4;

    public void UpdateUI(int fill)
    {

        float l = (float)fill / max;

        float width = -backgroundImage.rect.width + backgroundImage.rect.width * l;

        Vector2 v = new Vector2(width, fillImage.sizeDelta.y);
        fillImage.sizeDelta = v;

        // trading 
        if (BoatUpgradeManager.Instance.trading)
        {
            goldButton.gameObject.SetActive(true);

            if (fill == max)
            {
                goldButton.interactable = false;
                goldText.text = "MAX";
            }
            else
            {
                goldButton.interactable = true;
                goldText.text = "" + BoatUpgradeManager.Instance.GetPrice(upgradeType);
            }
        }
        else
        {
            goldButton.gameObject.SetActive(false);
        }
    }

    public void Upgrade()
    {
        Tween.Bounce(transform);
        BoatUpgradeManager.Instance.Upgrade(upgradeType);
    }

}
