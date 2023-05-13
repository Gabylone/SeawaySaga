using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapButtonRayblocker : MonoBehaviour, IPointerClickHandler
{
    public Map targetMap;

    public void OnPointerClick(PointerEventData eventData)
    {
        targetMap.CancelLaunchMap();
    }
}
