using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterSortingOrder : MonoBehaviour
{
    public RectTransform rectTransform;

    public Canvas canvas;

    public int minScreenHeight = 300;
    public int maxScreenHeight = 300;

    public int minSortingOrder = 0;
    public int maxSortingOrder = 100;

    public float lerp = 0f;

    // Update is called once per frame
    void Update()
    {
        lerp = Mathf.InverseLerp(minScreenHeight, maxScreenHeight, rectTransform.anchoredPosition.y);

        int sortingOrder = (int)Mathf.Lerp(minSortingOrder, maxSortingOrder, lerp);

        canvas.sortingOrder = maxSortingOrder - sortingOrder;


    }
}
