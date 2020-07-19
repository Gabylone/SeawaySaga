using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapEvents : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    public float onPointerDownDelay = 0.1f;

    private bool canPlacePin = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        canPlacePin = true;

        CancelInvoke("OnPointerDownDelay");
        Invoke("OnPointerDownDelay", onPointerDownDelay);
    }

    void OnPointerDownDelay()
    {
        canPlacePin = false;
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if ( canPlacePin)
        {
            PinManager.Instance.PlacePin();
        }
    }
}
