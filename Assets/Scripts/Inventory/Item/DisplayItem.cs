using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using System;

public class DisplayItem : MonoBehaviour {

	private Item handledItem;

    public virtual void Start()
    {

    }

	#region params
	public virtual Item HandledItem {
		get {
			return handledItem;
		}
		set {
            handledItem = value;
		}
	}
	#endregion
}
