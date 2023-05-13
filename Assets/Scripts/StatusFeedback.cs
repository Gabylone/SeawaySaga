using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class StatusFeedback : MonoBehaviour {

	public Image image;
    public Image fillImage;

    public Text text_Count;

    Transform _transform;

    public float tweenDur = 0.5f;
	public float tweenScaleAmount = 1.2f;

	public Fighter.Status status;

    public delegate void OnTouchStatusFeedback(Fighter.Status status);
    public static OnTouchStatusFeedback onTouchStatusFeedback;

    int max = 1;

    private void Start()
    {
        _transform = GetComponent<Transform>();
    }

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

		image.sprite = SkillManager.Instance.statusSprites [(int)status];
	}

    private void UpdateUI(int i)
    {
        //text_Count.text = "" + i;

        float f = (float)i / max;
        fillImage.DOFillAmount(f, 0.5f);
    }

    public void SetColor (Color color)
	{
		image.color = color;

        fillImage.color = Color.Lerp(Color.clear, color, 0.75f);
        //text_Count.color = color;
	}

	public void Hide ()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        Tween.Bounce(_transform);
    }

	public void OnPointerDown () 
	{
		Tween.Bounce (_transform);

		if (onTouchStatusFeedback != null)
			onTouchStatusFeedback (status);
	}
}
