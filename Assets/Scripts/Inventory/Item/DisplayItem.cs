using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

public class DisplayItem : MonoBehaviour {

    public Transform _transform;

	private protected Item handledItem;

    public virtual void Start()
    {
        _transform = GetComponent<Transform>();
    }

    public virtual void Show(Item item)
    {
        DisplayedItem = item;
    }

    public virtual void Hide()
    {

    }

    #region params
    public virtual Item DisplayedItem {
		get {
			return handledItem;
		}
		set {
            handledItem = value;
		}
	}
	#endregion
}
