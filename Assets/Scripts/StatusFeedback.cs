using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusFeedback : MonoBehaviour {

	public Image statusImage_Fill;

    public Text text_Count;

    public float tweenDur = 0.5f;
	public float tweenScaleAmount = 1.2f;

	public Fighter.Status status;

    public delegate void OnTouchStatusFeedback(Fighter.Status status);
    public static OnTouchStatusFeedback onTouchStatusFeedback;

    int max = 1;

    public void SetMax(int i)
    {
        max = i;
    }

    public void SetCount ( int count )
    {
        UpdateUI(count);
	}

	public void SetStatus ( Fighter.Status status ) {

        this.status = status;

		statusImage_Fill.sprite = SkillManager.statusSprites [(int)status];
	}

    private void UpdateUI(int i)
    {
        text_Count.text = "" + i;
    }

	public void SetColor (Color color)
	{
		statusImage_Fill.color = color;
        text_Count.color = color;
	}

	public void Hide ()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

	public void OnPointerDown () 
	{
		Tween.Bounce (transform);

		if (onTouchStatusFeedback != null)
			onTouchStatusFeedback (status);
	}
}
