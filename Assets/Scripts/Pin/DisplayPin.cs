using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DisplayPin : Displayable
{
    public DisplayPinInfo displayPinInfo;

    public override void Start()
    {
        base.Start();
    }

    public void OnPointerClick()
    {
        Select();
    }

    public void Select()
    {
        Tween.Bounce(GetRectTransform());

        if (displayPinInfo.visible)
        {
            HideInfo();
        }
        else
        {
            ShowInfo();
        }
        
    }

    public void HideInfo()
    {
        displayPinInfo.Hide();
    }

    public void ShowInfo()
    {
        displayPinInfo.Show();
    }

    public void DeletePin()
    {
        PinManager.Instance.pins.Remove(displayPinInfo.pin);

        Hide();
    }
}
