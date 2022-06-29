using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Transition : MonoBehaviour {

	// lerp
	public Color targetColor;

	[SerializeField]
	private Image targetImage;

	[SerializeField]
	private GameObject transitionCanvas;

	float timer = 0f;
    public float current_duration = 1f;
	bool lerping = false;
	
	public enum State
    {
		FadingIn,
		FadingOut,
    }
	public State state;

    private void Update()
    {
		UpdateLerp();
	}

	void StartLerp()
    {
		transitionCanvas.SetActive(true);

		if( state == State.FadingIn)
        {
			targetImage.color = Color.clear;
        }
        else
        {
			targetImage.color = targetColor;
        }

		lerping = true;
		timer = 0f;
    }

	void UpdateLerp()
	{
		if (!lerping)
			return;

		float lerp = timer / current_duration;
		Color startColor = state == State.FadingIn ? Color.clear : targetColor;
		Color finishColor = state == State.FadingIn ? targetColor : Color.clear;
		targetImage.color = Color.Lerp(startColor, finishColor, lerp);

		if (timer >= current_duration)
		{
			ExitLerp();
		}

		timer += Time.deltaTime;
	}

	void ExitLerp()
    {
		lerping = false;
		if ( state == State.FadingOut)
        {
			transitionCanvas.SetActive(false);
        }
	}

	public void FadeIn (float duration)
	{
        state = State.FadingIn;
		current_duration = duration;
		StartLerp();
	}
	public void FadeOut (float duration)
	{
		state = State.FadingOut;
		current_duration = duration;
		StartLerp();
	}

}
