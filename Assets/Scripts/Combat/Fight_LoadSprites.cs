using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Fight_LoadSprites : MonoBehaviour {

	public enum SpriteIndex {
		beard,
		eyebrows,
		eyes,
        hair,
		mouth,
		nose,
    }

	public SpriteRenderer[] allSprites;
    public SpriteRenderer weaponSprite;
	float fade_Duration;
	Color[] fade_InitColors;
	bool fading = false;
	float timer = 0f;

	#region fade
	public void FadeSprites (float dur) {

		foreach ( SpriteRenderer sprite in allSprites ) {
            sprite.DOFade(0f, dur);
		}
	}
	#endregion

	#region sprite order
	public void UpdateOrder (int fighterIndex)
	{
		fighterIndex = 4 - fighterIndex;

		foreach ( SpriteRenderer sprite in allSprites ) {
			sprite.sortingOrder += 11 * (fighterIndex+1);
		}

	}
	#endregion
}
