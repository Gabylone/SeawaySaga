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

	public void Init ()
	{
		//allSprites = GetComponentsInChildren<SpriteRenderer> (true);
		GetSpriteColors ();
	}

	public void UpdateSprites ( Member memberID ) {

		ResetColors ();

        // hair
        for (int i = 0; i < (int)ApparenceType.nose; i++)
        {
            ApparenceType type = (ApparenceType)i;
            Sprite spr = CrewCreator.Instance.GetApparenceItem(type, memberID.GetCharacterID(type)).GetSprite();
            if (spr == null)
            {
                allSprites[i].enabled = false;

            }
            else
            {
                allSprites[i].enabled = true;
                allSprites[i].sprite = CrewCreator.Instance.GetApparenceItem(type, memberID.GetCharacterID(type)).GetSprite();
            }
        }

		allSprites[(int)SpriteIndex.hair].color = CrewCreator.Instance.hairColors[memberID.GetCharacterID(ApparenceType.hairColor)];
		allSprites[(int)SpriteIndex.beard].color = CrewCreator.Instance.hairColors[memberID.GetCharacterID(ApparenceType.hairColor)];
		allSprites[(int)SpriteIndex.eyebrows].color = CrewCreator.Instance.hairColors[memberID.GetCharacterID(ApparenceType.hairColor)];

		// body
		//allSprites[(int)SpriteIndex.body].sprite = CrewCreator.Instance.BodySprites[memberID.Male ? 0:1];

		if (memberID.equipedWeapon == null) {
			print ("member ID weapond is null ?  " + memberID.equipedWeapon.name);
			weaponSprite.sprite = CrewCreator.Instance.handSprite;
		}
		else
            weaponSprite.sprite = CrewCreator.Instance.weaponSprites [memberID.equipedWeapon.spriteID];

	}



	#region sprite colors
	void GetSpriteColors () {
		fade_InitColors = new Color[allSprites.Length];

		for (int i = 0; i < fade_InitColors.Length; i++) {
			fade_InitColors [i] = allSprites [i].color;
		}
	}

	void ResetColors ()
	{
		int a = 0;
		foreach ( SpriteRenderer sprite in allSprites ) {
			sprite.color = fade_InitColors [a];
			++a;
		}
	}
	#endregion

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
