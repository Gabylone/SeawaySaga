using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

public class DisplayItem : MonoBehaviour {

	private protected Item handledItem;

    public virtual void Start()
    {

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
