using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Flag : MonoBehaviour {

	public static Flag Instance;

    [SerializeField]
    private GameObject group = null;

    public Camera cam;

    public bool visible = false;

    SpriteRenderer spriteRenderer;

    public LayerMask layerMask;

	void Awake () {
		Instance = this;
	}

    // Use this for initialization
    void Start()
    {
        WorldTouch.onPointerDown += HandleOnPointerDown;
        WorldTouch.onPointerExit += HandleOnPointerExit;

        NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    private void Update()
    {
        //if ( WorldTouch.Instance.touching )
        if (WorldTouch.Instance.touching && Swipe.Instance.timer > Swipe.Instance.minimumTime )
        {
            if (!visible)
                Show();

            UpdateFlagPos();
        }
    }

    private void UpdateFlagPos()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, layerMask))
        {
            transform.position = hit.point;
            PlayerBoat.Instance.SetTargetPos(hit.point);
        }
        else
        {
            Debug.Log("failed to update flag position");
        }
    }

    void HandleChunkEvent ()
	{
        Show();
        transform.localPosition = Vector3.zero;
        PlayerBoat.Instance.SetTargetPos(transform.position);
    }

    public void HandleOnEndMovement()
    {
        Tween.Bounce(transform);

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

        Tween.Bounce(transform);

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
