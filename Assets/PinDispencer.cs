using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PinDispencer : Displayable
{
    public bool pointerInside = false;

    public Pin.ColorType colorType;

    public Image[] pinImages;

    public GameObject deletePinGroup;
    public GameObject placePinGroup;

    public Transform deletePin_Transform;
    public Outline deleteOutline;

    private static PinDispencer[] pinDispencers = new PinDispencer[(int)Pin.ColorType.None];

    public static PinDispencer GetPinDispencer(Pin.ColorType colorType)
    {
        return pinDispencers[(int)colorType];
    }

    public override void Start()
    {
        base.Start();

        pinDispencers[(int)colorType] = this;

        foreach (var item in pinImages)
        {
            item.color = PinManager.Instance.GetPinColor(colorType);
        }

        SetPlaceMode();
    }

    public void CreatePin()
    {
        // sound
        SoundManager.Instance.PlayRandomSound("click_light");

        PinManager.Instance.CreatePin(colorType);
    }

    public void OnPointerEnter()
    {
        deleteOutline.enabled = true;
        Tween.Scale(deletePin_Transform, 0.2f, 1.05f);

        pointerInside = true;
    }

    public void OnPointerExit()
    {
        deleteOutline.enabled = false;
        Tween.Scale(deletePin_Transform, 0.2f, 1f);

        pointerInside = false;
    }

    public void SetPlaceMode()
    {
        deletePinGroup.SetActive(false);
        placePinGroup.SetActive(true);
    }

    public void SetDeleteMode()
    {
        deletePinGroup.SetActive(true);
        placePinGroup.SetActive(false);
    }
}
