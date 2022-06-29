using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class DisplayPinDispencers : MonoBehaviour
{
    public static DisplayPinDispencers Instance;

    public PinDispencer pinDispencer_Prefab;

    public Transform parent;

    public RectTransform rectTransform;

    Vector2 initPos;

    public float slideAmount = 200f;
    public float slideDur = 0.2f;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        initPos = rectTransform.anchoredPosition;

        int id = 0;

        foreach (var sprite in PinManager.Instance.sprites)
        {
            PinDispencer pinDispencer = Instantiate(pinDispencer_Prefab, parent);

            pinDispencer.Display(id, sprite);

            PinManager.Instance.pinDispencers.Add(pinDispencer);

            ++id;
        }
    }

    public void Slide()
    {
        rectTransform.DOAnchorPos(initPos + Vector2.right * slideAmount,slideDur);
    }

    public void GoBack()
    {
        rectTransform.DOAnchorPos(initPos, slideDur);
    }
}
