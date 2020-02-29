using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusFeedback : MonoBehaviour {

	public Image statusImage_BG;
	public Image statusImage_Fill;

    public Image backgroundImage_BG;
    public Image backgroundImage_Fill;

    public float tweenDur = 0.5f;
	public float tweenScaleAmount = 1.2f;

	public Fighter.Status status;

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
        statusImage_BG.sprite = SkillManager.statusSprites [(int)status];
	}

    private void UpdateUI(int i)
    {
        float lerp = i / max;

        statusImage_Fill.fillAmount = lerp;
        backgroundImage_Fill.fillAmount = lerp;
    }

	public void SetColor (Color color)
	{
		statusImage_Fill.color = color;
		backgroundImage_Fill.color = color;

		/*int a = 0;
		foreach (var item in GetComponentsInChildren<Image>()) {
			if ( a > 0 )
				HOTween.To ( item , tweenDur , "color" , Color.black );
			++a;
		}*/
	}

	public void Hide ()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

	public delegate void OnTouchStatusFeedback ( Fighter.Status status);
	public static OnTouchStatusFeedback onTouchStatusFeedback;
	public void OnPointerDown () 
	{
		Tween.Bounce (transform);

		if (onTouchStatusFeedback != null)
			onTouchStatusFeedback (status);
	}
}
