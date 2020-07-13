using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

public class PinManager : Singleton<PinManager>
{
    public GameObject prefab;

    public List<Pin> pins = new List<Pin>();

    public RectTransform firstParent;
    public RectTransform secondParent;

    protected override void Awake()
    {
        base.Awake();
    }

    public void LoadPins()
    {
        foreach (var pin in SaveManager.Instance.GameData.pins)
        {
            GameObject go = Instantiate(prefab, firstParent);
            RectTransform rectTransform = go.GetComponent<RectTransform>();

            float x = pin.x;
            float y = pin.y;
            Vector2 p = new Vector2(x, y);

            rectTransform.anchoredPosition = p;

            DisplayPin newDisplayPin = go.GetComponent<DisplayPin>();

            pins.Add(pin);

            newDisplayPin.displayPinInfo.pin = pin;
            newDisplayPin.displayPinInfo.inputField.text = pin.content;
            newDisplayPin.displayPinInfo.Hide();

            newDisplayPin.GetRectTransform().SetParent(secondParent);
            newDisplayPin.GetRectTransform().localScale = Vector3.one;


        }
    }

    public void PlacePin()
    {
        if (!DisplayMinimap.Instance.fullyDisplayed)
        {
            return;
        }

        GameObject go = Instantiate(prefab, firstParent);

        Vector2 inputPos = InputManager.Instance.GetInputPosition();

        RectTransform rectTransform = go.GetComponent<RectTransform>();

        float x = inputPos.x * firstParent.rect.width / Screen.width;
        float y = inputPos.y * firstParent.rect.height / Screen.height;

        Vector2 p = new Vector2(x, y);

        rectTransform.anchoredPosition = p;

        DisplayPin newDisplayPin = go.GetComponent<DisplayPin>();

        Pin newPin = new Pin();
        newPin.x = x;
        newPin.y = y;

        pins.Add(newPin);

        newDisplayPin.displayPinInfo.pin = newPin;

        newDisplayPin.displayPinInfo.inputField.ActivateInputField();

        newDisplayPin.GetRectTransform().SetParent(secondParent);

        newDisplayPin.GetRectTransform().DOMove(newDisplayPin.GetRectTransform().position + Vector3.up * 0.25f, 0.3f);
        newDisplayPin.GetRectTransform().DOMove(newDisplayPin.GetRectTransform().position, 0.1f).SetDelay(0.3f);

    }

}