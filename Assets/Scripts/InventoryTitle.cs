using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InventoryTitle : MonoBehaviour {

    Vector3 initPos;
    public float duration = 0.25f;
    public float decal = 50f;

	// Use this for initialization
	void Start () {
        initPos = transform.position;

        LootUI.onSetSelectedItem += HandleOnSetSelectedItem;
	}

    private void HandleOnSetSelectedItem()
    {
        if (LootUI.Instance.SelectedItem == null)
        {
            transform.DOMove(initPos, duration);
        }
        else
        {
            transform.DOMove(initPos + Vector3.up * decal , duration);
        }
    }
}
