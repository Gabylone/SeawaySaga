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
    public DisplayPin currentDisplayedPin;

    public float distanceToDeletePin = 1f;

    public Color[] colors;

    public bool pointerInside = false;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        DisplayPin displayPin = DisplayPin.currentDraggedPin;

        if (displayPin != null)
        {
            PinDispencer pinDispencer = PinDispencer.GetPinDispencer(displayPin.displayPinInfo.pin.colorType);

            float dis = Vector2.Distance(displayPin.GetRectTransform.position,pinDispencer.GetRectTransform.position);

            if (pinDispencer.pointerInside)
            {
                if (dis > distanceToDeletePin)
                {
                    pinDispencer.OnPointerExit();
                }
            }
            else
            {
                if ( dis < distanceToDeletePin)
                {
                    pinDispencer.OnPointerEnter();
                }
            }
        }
    }

    public void LoadPins()
    {
        foreach (var pin in SaveManager.Instance.GameData.pins)
        {
            GameObject go = Instantiate(prefab, firstParent);

            DisplayPin newDisplayPin = go.GetComponent<DisplayPin>();

            pins.Add(pin);

            newDisplayPin.displayPinInfo.pin = pin;
            newDisplayPin.displayPinInfo.inputField.text = pin.content;
            newDisplayPin.displayPinInfo.Hide();

            newDisplayPin.canBeDraggedOnMap = true;

            newDisplayPin.GetRectTransform.SetParent(secondParent);
            newDisplayPin.GetRectTransform.localScale = Vector3.one;

            float x = pin.save_X;
            float y = pin.save_Y;
            Vector2 p = new Vector2(x, y);

            newDisplayPin.GetRectTransform.anchoredPosition = p;

        }
    }

    public void DeletePin(DisplayPin displayPin)
    {
        pins.Remove(displayPin.displayPinInfo.pin);

        displayPin.Hide();
    }

    public void CreatePin(Pin.ColorType colorType)
    {
        GameObject go = Instantiate(prefab, null);

        DisplayPin newDisplayPin = go.GetComponent<DisplayPin>();

        // data
        Pin newPin = new Pin();
        newPin.colorType = colorType;

        pins.Add(newPin);
        newDisplayPin.displayPinInfo.pin = newPin;
        newDisplayPin.displayPinInfo.HideDelay();

        newDisplayPin.TakePin();
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

    public Color GetPinColor(Pin.ColorType colorType)
    {
        return colors[(int)colorType];
    }

}