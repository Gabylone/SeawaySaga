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

    public float bounce_dur = 0.3f;
    public float bounce_amount = 1.3f;

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

        WorldTouch.Instance.onSelectSomething += Hide;

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

        if ( _transform.position.x < -33f || _transform.position.x > 33f ) {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }

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
        Bounce();

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

        Bounce();

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

    public void Bounce()
    {
        Tween.Bounce(_transform, bounce_dur, bounce_amount);
    }
}
