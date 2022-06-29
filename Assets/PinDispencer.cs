using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PinDispencer : Displayable
{
    public int iconType;

    public Image image;

    //public GameObject deletePinGroup;
    public GameObject placePinGroup;

    int id = 0;

    public static PinDispencer GetPinDispencer(int i)
    {
        return PinManager.Instance.pinDispencers[i];
    }

    public override void Start()
    {
        base.Start();

        SetPlaceMode();
    }

    public void Display(int _id, Sprite _sprite)
    {
        image.sprite = _sprite;
        id = _id;
    }

    public void CreatePin()
    {
        Tween.Bounce
            (GetRectTransform);

        // sound
        SoundManager.Instance.PlayRandomSound("click_light");

        PinManager.Instance.CreatePin(id);
    }

    public void SetPlaceMode()
    {
        DeletePinButton.Instance.Hide();
        //deletePinGroup.SetActive(false);
        placePinGroup.SetActive(true);
        DisplayPinDispencers.Instance.GoBack();

    }

    public void SetDeleteMode()
    {
        DisplayPinDispencers.Instance.Slide();
        DeletePinButton.Instance.Show();
        //deletePinGroup.SetActive(true);
        placePinGroup.SetActive(false);
    }

    /*public void OnPointerClick(PointerEventData eventData)
    {
        CreatePin();
    }*/


}
