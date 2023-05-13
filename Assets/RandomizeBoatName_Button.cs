using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RandomizeBoatName_Button : MonoBehaviour, IPointerClickHandler
{
    RectTransform rectTransform;

    public float bounceAmount = 1.03f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Tween.Bounce(rectTransform, 0.2f, bounceAmount);

        SoundManager.Instance.PlaySound("click_med 03");

    }
}
