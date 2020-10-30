using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayPinInfo : Displayable
{
    public InputField inputField;

    public Pin pin;

    public void OnEndEdit()
    {
        Tween.Bounce(GetRectTransform);

        pin.content = inputField.text;

        Hide();
    }
}
