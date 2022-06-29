using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using DG.Tweening;

public class DisplayPin : Displayable
{
    public Pin pin;

    public static DisplayPin currentDraggedPin;
    public static bool draggingPin = false;

    public Image image;

    public Vector2 dragDecal = Vector2.zero;

    public float dragSpeed = 1f;

    private bool dragging = false;

    public bool canBeDraggedOnMap = false;

    public override void Start()
    {
        base.Start();
    }

    public void Display ( Sprite sprite)
    {
        image.sprite = sprite;
    }

    public void OnBeginDrag()
    {
        if (!DisplayMinimap.Instance.fullyDisplayed)
        {
            return;
        }

        if (!canBeDraggedOnMap)
        {
            return;
        }

        TakePin();
    }

    public void TakePin()
    {
        image.raycastTarget = false;

        GetRectTransform.SetParent(PinManager.Instance.firstParent);
        GetRectTransform.localScale = PinManager.Instance.secondParent.localScale;

        UpdatePos_Quick();

        dragging = true;

        currentDraggedPin = this;
        draggingPin = true;

        PinDispencer.GetPinDispencer(pin.id).SetDeleteMode();
    }

    private void Update()
    {
        if (dragging)
        {
            UpdatePos();

            if (InputManager.Instance.Touch_Exit())
            {
                OnEndDrag();
            }
        }
    }

    public void OnEndDrag()
    {
        if (!DisplayMinimap.Instance.fullyDisplayed)
        {
            return;
        }

        // stop dragging update
        dragging = false;

        Tween.Bounce(GetRectTransform);

        SoundManager.Instance.PlayRandomSound("click_light");

        PinDispencer.GetPinDispencer(pin.id).SetPlaceMode();

        if (DeletePinButton.Instance.pointerInside)
        {
            PinManager.Instance.DeletePin(this);
            return;
        }

        // anim
        GetRectTransform.DOMove(GetRectTransform.position + Vector3.up * 0.25f, 0.3f);
        GetRectTransform.DOMove(GetRectTransform.position, 0.1f).SetDelay(0.3f);

        // sound

        // parent
        GetRectTransform.SetParent(PinManager.Instance.secondParent);
        GetRectTransform.localScale = Vector3.one;

        // save info ( après parent )
        pin.save_X = GetRectTransform.anchoredPosition.x;
        pin.save_Y = GetRectTransform.anchoredPosition.y;

        image.raycastTarget = true;

        canBeDraggedOnMap = true;

        ShowInfo();

    }

    public void OnPointerClick()
    {
       

        Select();
    }

    public void UpdatePos()
    {
        GetRectTransform.anchoredPosition = Vector2.Lerp(GetRectTransform.anchoredPosition, GetPos(), dragSpeed * Time.deltaTime);
    }

    public void UpdatePos_Quick()
    {
        GetRectTransform.anchoredPosition = GetPos();
    }

    public Vector2 GetPos()
    {
        Vector2 inputPos = InputManager.Instance.GetInputPosition();
        float x = inputPos.x * PinManager.Instance.firstParent.rect.width / Screen.width;
        float y = inputPos.y * PinManager.Instance.firstParent.rect.height / Screen.height;
        Vector2 p = new Vector2(x, y);

        return p;
    }

    private void Select()
    {
        if (!DisplayMinimap.Instance.fullyDisplayed)
        {
            return;
        }

        Tween.Bounce(GetRectTransform);

        ShowInfo();

        /*if (displayPinInfo.visible)
        {
            SoundManager.Instance.PlaySound("button_big 01");

            HideInfo();
        }
        else
        {
            SoundManager.Instance.PlaySound("button_big 02");

            ShowInfo();
        }

        displayPinInfo.inputField.ActivateInputField();*/

        GetRectTransform.SetAsLastSibling();
        
    }

    public void HideInfo()
    {
        //displayPinInfo.Hide();
    }

    public void ShowInfo()
    {
        PinManager.Instance.SetDisplayedPin(this);

        //displayPinInfo.Show();
    }
}
