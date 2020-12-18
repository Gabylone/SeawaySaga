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

	public void FadeIn (float duration)
	{
        transitionCanvas.SetActive(true);

        CancelInvoke("FadeOutDelay");
        targetImage.DOKill();

        targetImage.color = Color.clear;
        targetImage.DOColor(targetColor, duration);
	}
	public void FadeOut (float duration)
	{
        transitionCanvas.SetActive(true);

        CancelInvoke("FadeOutDelay");
        targetImage.DOKill();

        targetImage.color = targetColor;
        targetImage.DOFade(0, duration);
		Invoke ("FadeOutDelay", duration);
	}
	void FadeOutDelay () {
		transitionCanvas.SetActive (false);
	}

}
