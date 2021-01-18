using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Flag : MonoBehaviour {

	public static Flag Instance;

    [SerializeField]
    private GameObject group = null;

    Transform _transform;

    public Camera cam;

    public bool visible = false;

    SpriteRenderer spriteRenderer;

    public LayerMask layerMask;

    public float minX;
    public float maxX = 1f;
    public float minY;
    public float maxY = 1f;

    public float currX;
    public float currY;

    public Directions direction;

    public Sprite[] sprites;

    bool placingFlag = false;

    void Awake () {
		Instance = this;
	}

    // Use this for initialization
    void Start()
    {
        _transform = GetComponent<Transform>();

        WorldTouch.onPointerDown += HandleOnPointerDown;
        WorldTouch.onPointerExit += HandleOnPointerExit;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        // NEW 
        if (WorldTouch.Instance.touching)
        {
            if ( !placingFlag)
            {
                SoundManager.Instance.PlaySound("click_med 04");
                placingFlag = true;
            }

            if (!visible)
                Show();

            UpdateFlagPos();
        }
        else
        {
            placingFlag = false;
        }
    }

    private void UpdateFlagPos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            PlayerBoat.Instance.SetTargetPos(hit.point);
        }
    }

    public void SetPos(Vector3 pos)
    {
        _transform.position = pos;
    }

    public void HandleOnEndMovement()
    {
        Tween.Bounce(_transform);

        spriteRenderer.DOKill();
        spriteRenderer.DOFade(0f, Tween.defaultDuration);

        CancelInvoke("Hide");
        Invoke("Hide", Tween.defaultDuration);
        
    }

	void HandleOnTouchIsland ()
	{
		Hide ();
	}

    private void HandleOnPointerDown()
    {

    }

    void HandleOnPointerExit()
    {
        Show();
        UpdateFlagPos();

        Tween.Bounce(_transform);

    }

    void Show()
    {
        spriteRenderer.DOKill();
        CancelInvoke("Hide");
        spriteRenderer.color = Color.white;

        if (visible)
            return;

        visible = true;

        CancelInvoke();

        group.SetActive(true);
    }

    public void Hide()
    {
        visible = false;

        group.SetActive(false);
    }
}
