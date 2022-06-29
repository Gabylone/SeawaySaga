using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Tween : MonoBehaviour {

	public static float defaultAmount = 1.1f;
	public static float defaultDuration = 0.2f;

	public static void Scale (Transform t, float dur , float amount) {

		Ease eT = amount > 1 ? Ease.OutBounce : Ease.Linear;
        t.DOScale(Vector3.one * amount, dur).SetEase(eT);
	}


	public static void Bounce (Transform t) {
		Bounce ( t, defaultDuration , defaultAmount );
	}

	public static void Bounce (Transform t, float dur , Vector3 targetScale , float amount ) {
        t.DOScale(targetScale * amount, dur/2f).SetEase(Ease.OutBounce);
        t.DOScale(targetScale, dur / 2f).SetDelay(dur / 2f);
	}

	public static void Bounce (Transform t, float dur , float amount) {
        t.DOScale(Vector3.one * amount, dur / 2f).SetEase(Ease.OutBounce);
        t.DOScale(Vector3.one, dur / 2f).SetDelay(dur / 2f);
	}

	public static void Fade (Transform t, float dur ) {

		foreach ( Image image in t.GetComponentsInChildren<Image>(true) ) {
			Color c = image.color;
			c.a = 0f;
            image.DOFade(0, dur);
		}

		foreach ( Text text in t.GetComponentsInChildren<Text>(true) ) {
			Color c = text.color;
            c.a = 0f;
            text.DOFade(0f, dur);
		}

	}

	public static void ClearFade (Transform t ) {

		foreach ( Image image in t.GetComponentsInChildren<Image>(true) ) {
            image.DOKill();
			Color c = image.color;
			c.a = 1f;
			image.color = c;
		}

		foreach ( Text text in t.GetComponentsInChildren<Text>(true) ) {
            text.DOKill();
			Color c = text.color;
			c.a = 1f;
			text.color = c;
		}
	}
}
