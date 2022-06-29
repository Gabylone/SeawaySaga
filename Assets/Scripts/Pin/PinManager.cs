using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PinManager : Singleton<PinManager>
{
    public GameObject prefab;

    public List<Pin> pins = new List<Pin>();

    public RectTransform firstParent;
    public RectTransform secondParent;

    public List<PinDispencer> pinDispencers = new List<PinDispencer>();

    public Sprite[] sprites;

    private DisplayPin previousDisplayedPin;
    public DisplayPin currentDisplayedPin;

    public float distanceToDeletePin = 1f;

    public bool pointerInside = false;

    protected override void Awake()
    {
        base.Awake();
    }

    /*public List<RaycastResult> RaycastMouse()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            pointerId = -1,
        };

        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }*/

    private void Update()
    {
        DisplayPin displayPin = DisplayPin.currentDraggedPin;

        if (displayPin != null)
        {
            RectTransform rect_transform = DeletePinButton.Instance.GetRectTransform;

            float dis = Vector2.Distance(displayPin.GetRectTransform.position, rect_transform.position);

            if (DeletePinButton.Instance.pointerInside)
            {
                if (dis > distanceToDeletePin)
                {
                    DeletePinButton.Instance.OnPointerExit();
                }
            }
            else
            {
                if ( dis < distanceToDeletePin)
                {
                    DeletePinButton.Instance.OnPointerEnter();
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

            newDisplayPin.pin = pin;
            /*newDisplayPin.displayPinInfo.pin = pin;
            newDisplayPin.displayPinInfo.inputField.text = pin.content;
            newDisplayPin.displayPinInfo.Hide();*/
            newDisplayPin.Display(sprites[pin.id]);

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
        pins.Remove(displayPin.pin);

        displayPin.Hide();
    }

    public void CreatePin(int id)
    {
        GameObject go = Instantiate(prefab, null);

        DisplayPin newDisplayPin = go.GetComponent<DisplayPin>();
        newDisplayPin.Display(sprites[id]);

        // data
        Pin newPin = new Pin();
        newPin.id = id;

        pins.Add(newPin);
        newDisplayPin.pin = newPin;
        //newDisplayPin.HideDelay();

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

}