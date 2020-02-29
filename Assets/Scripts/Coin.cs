using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour {

	public float tweenDuration = 1f;
	public float rotationSpeed = 90f;

	private Transform mTransform;

	public float decalUp = 1f;

	private Vector3 initPos;

	bool rot = true;

	public bool heads = false;

	// Use this for initialization
	void Start () {
		mTransform = GetComponent<Transform> ();

		initPos = mTransform.localPosition;

        mTransform.DOLocalMove(initPos + Vector3.up * decalUp, tweenDuration / 2f);
        mTransform.DOLocalMove(initPos, tweenDuration / 2f).SetDelay(tweenDuration/2f);

		Invoke ("Stop", tweenDuration);
	}
	
	// Update is called once per frame
	void Update () {
		if ( rot )
			mTransform.Rotate (Vector3.right * rotationSpeed * Time.deltaTime);
	}

	void Stop () {
		rot = false;

		Invoke ("StopDelay",0.1f);
	}

	void StopDelay () {
		Tween.Bounce (mTransform);

		if (!heads) {
            mTransform.forward = -Vector3.forward;
		} else {
            mTransform.forward = Vector3.forward;
		}

		Destroy (gameObject, 2f);
	}
}
