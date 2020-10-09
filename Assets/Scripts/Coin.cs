using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour {

    public GameObject group;

	public float tweenDuration = 1f;
	public float rotationSpeed = 90f;

	public Transform _transform;

	public float decalUp = 1f;

	private Vector3 initPos;

	bool rot = true;

	public bool heads = false;

    private void Start()
    {
        _transform = GetComponent<Transform>();

        HideDelay();
    }

    // Use this for initialization
    public void Flip () {

        Show();

        SoundManager.Instance.PlayLoop("dice_wait");

        SoundManager.Instance.PlayRandomSound("click_ligh");
        SoundManager.Instance.PlayRandomSound("coin");

        _transform.rotation = Quaternion.identity;

        rot = true;

		initPos = _transform.localPosition;

        _transform.DOLocalMove(initPos + Vector3.up * decalUp, tweenDuration / 2f);
        _transform.DOLocalMove(initPos, tweenDuration / 2f).SetDelay(tweenDuration/2f);

		Invoke ("Stop", tweenDuration);
	}
	
	// Update is called once per frame
	void Update () {
		if ( rot )
			_transform.Rotate (Vector3.right * rotationSpeed * Time.deltaTime);
	}

	void Stop () {
		rot = false;

		Invoke ("StopDelay",0.1f);
	}

	void StopDelay () {
		Tween.Bounce (_transform);

		if (!heads) {
            SoundManager.Instance.PlaySound("ui_deny");
            _transform.forward = -Vector3.forward;
		} else {
            SoundManager.Instance.PlaySound("ui_correct");
            _transform.forward = Vector3.forward;
		}

        Invoke("Hide" , 2f);

        SoundManager.Instance.StopLoop("dice_wait");

        SoundManager.Instance.PlayRandomSound("Blunt");
        SoundManager.Instance.PlaySound("Dice Settle");

        SoundManager.Instance.PlayRandomSound("Coins");

    }

    void Show()
    {
        group.SetActive(true);

        _transform.localScale = Vector3.zero;
        _transform.DOScale(1f, 0.2f);
    }

    void Hide()
    {
        _transform.DOScale(0f, 0.2f);

        Invoke("HideDelay", 0.2f);
    }

    void HideDelay()
    {
        group.SetActive(false);

    }
}
