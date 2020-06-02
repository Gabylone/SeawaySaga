using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IconVisual : MonoBehaviour
{
    public List<GameObject> outline_objs = new List<GameObject>();

    public BodyVisual bodyVisual;

    public void InitVisual(Member memberID)
    {
        // hair
        for (int i = 0; i <= (int)ApparenceType.nose; i++)
        {
            ApparenceType type = (ApparenceType)i;
            Sprite spr = CrewCreator.Instance.GetApparenceItem(type, memberID.GetCharacterID(type)).GetSprite();
            if ( spr == null)
            {
                images[i].enabled = false;
            }
            else
            {
                images[i].enabled = true;
                images[i].sprite = CrewCreator.Instance.GetApparenceItem(type, memberID.GetCharacterID(type)).GetSprite();
            }
        }

        Color c = CrewCreator.Instance.hairColors[memberID.GetCharacterID(ApparenceType.hairColor)];

        GetImage(ApparenceType.hair).color      = c;
        GetImage(ApparenceType.eyebrows).color  = c;
        GetImage(ApparenceType.beard).color     = c;

        int skinColorID = memberID.GetCharacterID(ApparenceType.skinColor);
        Color skinColor = CrewCreator.Instance.GetApparenceItem(ApparenceType.skinColor, skinColorID).color;
        GetImage(ApparenceType.nose).color = skinColor;

        UpdateWeaponSprite(memberID);

        bodyVisual.InitVisual(memberID);

    }

    public void UpdateWeaponSprite(Member member)
    {
        if (member.equipedWeapon == null)
        {
            weaponImage.sprite = CrewCreator.Instance.handSprite;
        }
        else
        {
            weaponImage.sprite = CrewCreator.Instance.weaponSprites[member.equipedWeapon.spriteID];
        }
    }

    [Header("BobyParts")]
    [SerializeField]
    private Image weaponImage;

    public Image[] images;

    public Image GetImage(ApparenceType apparenceType)
    {
        return images[(int)apparenceType];
    }


}
