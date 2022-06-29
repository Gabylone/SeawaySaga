using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeletePinButton : Displayable
{
    public static DeletePinButton Instance;

    public Outline outline;

    public bool pointerInside = false;

    private void Awake()
    {
        Instance = this;

        outline.enabled = false;
    }

    public void OnPointerEnter()
    {
        pointerInside = this;
        outline.enabled = true;
        Tween.Scale(GetRectTransform, 0.2f, 1.05f);
    }

    public void OnPointerExit()
    {
        pointerInside = false;
        outline.enabled = false;
        Tween.Scale(GetRectTransform, 0.2f, 1f);

    }

}
