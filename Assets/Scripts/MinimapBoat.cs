using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBoat : MonoBehaviour
{
    public OtherBoatInfo boatInfo;

    public Transform targetPosition;

    public RectTransform rectTransform;

    public void OnPointerDown()
    {
        Tween.Bounce(transform);

        IslandInfo.Instance.DisplayBoatInfo(boatInfo);
        IslandInfo.Instance.ShowAtTransform(targetPosition);
    }
}
