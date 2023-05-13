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

    public float posY = 0f;
    int sortOrder = 0;

    public int mult = 100;

    // Update is called once per frame
    void Update()
    {
        if (CombatManager.Instance.fighting)
        {
            posY = rectTransform.position.y - CombatManager.Instance.bottom.transform.position.y;
            lerp = posY / 8;
            sortOrder = (int)Mathf.Lerp(minSortingOrder, maxSortingOrder, lerp);
            canvas.sortingOrder = maxSortingOrder - sortOrder;
        }
    }
}
