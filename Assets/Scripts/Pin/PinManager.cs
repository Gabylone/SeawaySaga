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

    public DisplayPin currentDraggedPin;

    public bool drappingPin = false;

    public GameObject deletePinGroup;
    public GameObject placePinGroup;

    public bool pointerInside = false;

    protected override void Awake()
    {
        base.Awake();

        DragPin_Exit();
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

            newDisplayPin.canBeDraggeedOnMap = true;

            newDisplayPin.GetRectTransform.SetParent(secondParent);
            newDisplayPin.GetRectTransform.localScale = Vector3.one;

            float x = pin.save_X;
            float y = pin.save_Y;
            Vector2 p = new Vector2(x, y);

            rectTransform.anchoredPosition = p;

            Debug.Log("LOADING PIN : at " + p);

        }
    }

    public void DeletePin(DisplayPin displayPin)
    {
        pins.Remove(displayPin.displayPinInfo.pin);

        displayPin.Hide();
    }

    public void CreatePin()
    {
        GameObject go = Instantiate(prefab, null);

        DisplayPin newDisplayPin = go.GetComponent<DisplayPin>();

        // data
        Pin newPin = new Pin();
        pins.Add(newPin);
        newDisplayPin.displayPinInfo.pin = newPin;

        newDisplayPin.displayPinInfo.HideDelay();

        // parent
        /*newDisplayPin.GetRectTransform.SetParent(secondParent);
        newDisplayPin.GetRectTransform.localScale = Vector3.one;*/

        newDisplayPin.TakePin();

        // sound
        SoundManager.Instance.PlayRandomSound("click_light");
    }

    public void OnPointerEnter()
    {
        pointerInside = true;
    }

    public void OnPointerExit()
    {
        pointerInside = false;
    }

    public void DragPin_Start(DisplayPin displayPin)
    {
        currentDraggedPin = displayPin;

        drappingPin = true;

        deletePinGroup.SetActive(true);
        placePinGroup.SetActive(false);
    }

    public void DragPin_Exit()
    {
        drappingPin = false;

        deletePinGroup.SetActive(false);
        placePinGroup.SetActive(true);
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