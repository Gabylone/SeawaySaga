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
            SoundManager.Instance.PlaySound("button_big 01");

            HideInfo();
        }
        else
        {
            SoundManager.Instance.PlaySound("button_big 02");

            ShowInfo();
        }
        
    }

    public void HideInfo()
    {
        displayPinInfo.Hide();
    }

    public void ShowInfo()
    {

        PinManager.Instance.SetDisplayedPin(this);

        displayPinInfo.Show();
    }

    public void DeletePin()
    {
        PinManager.Instance.pins.Remove(displayPinInfo.pin);

        Hide();
    }
}
