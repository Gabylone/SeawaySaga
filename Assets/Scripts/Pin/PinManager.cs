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

    private DisplayPin previousDisplayedPin;
    private DisplayPin currentDisplayedPin;

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

            DisplayPin newDisplayPin = go.GetComponent<DisplayPin>();

            pins.Add(pin);

            newDisplayPin.displayPinInfo.pin = pin;
            newDisplayPin.displayPinInfo.inputField.text = pin.content;
            newDisplayPin.displayPinInfo.Hide();

            newDisplayPin.GetRectTransform().SetParent(secondParent);
            newDisplayPin.GetRectTransform().localScale = Vector3.one;

            float x = pin.save_X;
            float y = pin.save_Y;
            Vector2 p = new Vector2(x, y);

            rectTransform.anchoredPosition = p;

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
        

        Debug.Log("x : " + x);

        // 
        pins.Add(newPin);

        // 
        newDisplayPin.displayPinInfo.pin = newPin;

        // 
        newDisplayPin.displayPinInfo.inputField.ActivateInputField();

        // parent
        newDisplayPin.GetRectTransform().SetParent(secondParent);
        newDisplayPin.GetRectTransform().localScale = Vector3.one;

        // save info ( après parent )
        newPin.save_X = rectTransform.anchoredPosition.x;
        newPin.save_Y = rectTransform.anchoredPosition.y;

        // anim
        newDisplayPin.GetRectTransform().DOMove(newDisplayPin.GetRectTransform().position + Vector3.up * 0.25f, 0.3f);
        newDisplayPin.GetRectTransform().DOMove(newDisplayPin.GetRectTransform().position, 0.1f).SetDelay(0.3f);

    }

    public void SetDisplayedPin( DisplayPin displayPin)
    {
        previousDisplayedPin = currentDisplayedPin;

        if (previousDisplayedPin!= null)
        {
            previousDisplayedPin.HideInfo();
        }

        currentDisplayedPin = displayPin;

    }

}