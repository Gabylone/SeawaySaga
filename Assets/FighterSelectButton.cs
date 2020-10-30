using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FighterSelectButton : MonoBehaviour, IPointerClickHandler
{
    public Fighter fighter;

    public void OnPointerClick(PointerEventData eventData)
    {
        fighter.Select();
    }
}
