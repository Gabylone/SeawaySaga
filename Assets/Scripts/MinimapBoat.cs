using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class MinimapBoat : MonoBehaviour
{
    public BoatInfo linkedBoatInfo;

    public Transform targetBoatTransform;

    public RectTransform world_RectTransform;
    public RectTransform local_RectTransform;

    public float tweenDuration = 0.5f;
    public float gapDuration = 0.25f;

    public virtual void Start()
    {
        world_RectTransform.sizeDelta = Vector2.one * DisplayMinimap.Instance.minimapChunkScale;
    }

    public virtual void TweenToCoords( Coords coords)
    {
        Vector2 boatPos = DisplayMinimap.Instance.GetPositionFromCoords(linkedBoatInfo.coords);

        world_RectTransform.DOAnchorPos(boatPos, tweenDuration - gapDuration).SetDelay(gapDuration);

        Tween.Bounce(world_RectTransform);
    }

    public void MoveToCoords(Coords coords)
    {
        Vector2 boatPos = DisplayMinimap.Instance.GetPositionFromCoords(linkedBoatInfo.coords);

        world_RectTransform.anchoredPosition = boatPos;

        Tween.Bounce(world_RectTransform);
    }

    public virtual void Update()
    {
        float x = DisplayMinimap.Instance.rangeX * targetBoatTransform.position.x / NavigationManager.Instance.maxX;
        float y = DisplayMinimap.Instance.rangeY * targetBoatTransform.position.z / NavigationManager.Instance.maxY;

        Vector2 pos = new Vector2(x, y);

        local_RectTransform.anchoredPosition = pos;

        
    }

    public void Show(BoatInfo boatInfo)
    {
        linkedBoatInfo = boatInfo;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
