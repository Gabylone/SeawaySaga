using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class InventoryActionButton : MonoBehaviour {

	public InventoryActionType inventoryActionType;

    Transform _transform;

    public CanvasGroup canvasGroup;

	public float tweenDuration = 0.2f;

    private bool locked = false;

    public bool visible = false;

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        visible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        visible = false;
    }

    public void Lock()
    {
        canvasGroup.alpha = 0.5f;

        locked = true;
    }

    public void Unlock()
    {
        canvasGroup.alpha = 1f;

        locked = false;
    }

    public void OnPointerDown () {

        if ( locked)
        {
            return;
        }

        TriggerAction ();
    }

	void TriggerAction () {
		
		Tween.Bounce (_transform);

        LootUI.Instance.InventoryAction(inventoryActionType);

    }
}
