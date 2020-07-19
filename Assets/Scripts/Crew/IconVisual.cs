using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class IconVisual : MonoBehaviour
{
    public List<GameObject> outline_objs = new List<GameObject>();

    public BodyVisual bodyVisual;

    public Image weaponImage;
    public Image handImage;
    public Image[] images;

    public float highlightSpeed = 1f;

    private bool tainted = false;
    private Color targetHighlightColor;
    private bool loopTaint = false;

    public float taintOnceDuration = 0.5f;

    private Member currentMember;

    float lerp = 0f;
    public float range = 0.3f;


    float timer = 0f;

    private void Update()
    {
        if (tainted)
        {
            Color c = targetHighlightColor;

            if (loopTaint)
            {
                lerp = Mathf.PingPong(Time.time * highlightSpeed, range);
            }
            else
            {
                lerp = Mathf.Lerp(1f, 0f, timer / taintOnceDuration);
                timer += Time.deltaTime;
            }

            GetImage(ApparenceType.hair).color = LerpColor(ApparenceType.hair, ApparenceType.hairColor);
            GetImage(ApparenceType.beard).color = LerpColor(ApparenceType.beard, ApparenceType.hairColor);
            GetImage(ApparenceType.eyebrows).color = LerpColor(ApparenceType.eyebrows, ApparenceType.hairColor);

            bodyVisual.GetImage(BodyVisual.ID.Top).color = LerpColor(BodyVisual.ID.Top, ApparenceType.topColor);
            bodyVisual.GetImage(BodyVisual.ID.Pants).color = LerpColor(BodyVisual.ID.Pants, ApparenceType.pantColor);
            bodyVisual.GetImage(BodyVisual.ID.Shoes).color = LerpColor(BodyVisual.ID.Shoes, ApparenceType.shoesColor);

            GetImage(ApparenceType.nose).color = LerpColor(ApparenceType.nose, ApparenceType.skinColor);
            bodyVisual.GetImage(BodyVisual.ID.Face).color = LerpColor(BodyVisual.ID.Face, ApparenceType.skinColor);
            bodyVisual.GetImage(BodyVisual.ID.Skin).color = LerpColor(BodyVisual.ID.Skin, ApparenceType.skinColor);
            bodyVisual.GetImage(BodyVisual.ID.RightArm).color = LerpColor(BodyVisual.ID.RightArm, ApparenceType.skinColor);
            bodyVisual.GetImage(BodyVisual.ID.LeftArm).color = LerpColor(BodyVisual.ID.LeftArm, ApparenceType.skinColor);

            weaponImage.color = Color.Lerp(Color.white, targetHighlightColor, lerp);
            handImage.color = Color.Lerp(GetColor(ApparenceType.skinColor), targetHighlightColor, lerp);

        }
    }
    private Color LerpColor(BodyVisual.ID spriteId, ApparenceType colorId)
    {
        return Color.Lerp(GetColor(colorId), targetHighlightColor, lerp);
    }
    private Color LerpColor(ApparenceType spriteId, ApparenceType colorId)
    {
        return Color.Lerp(GetColor(colorId), targetHighlightColor, lerp);
    }

    public void InitVisual(Member _member)
    {
        currentMember = _member;

        InitVisual();
    }

    public void InitVisual()
    {
        if (currentMember == null)
        { return; }

        // hair
        for (int i = 0; i <= (int)ApparenceType.nose; i++)
        {
            ApparenceType type = (ApparenceType)i;
            Sprite spr = CrewCreator.Instance.GetApparenceItem(type, currentMember.GetCharacterID(type)).GetSprite();
            if (spr == null)
            {
                images[i].enabled = false;
            }
            else
            {
                images[i].enabled = true;
                images[i].sprite = CrewCreator.Instance.GetApparenceItem(type, currentMember.GetCharacterID(type)).GetSprite();
            }
        }

        Color c = CrewCreator.Instance.hairColors[currentMember.GetCharacterID(ApparenceType.hairColor)];

        GetImage(ApparenceType.hair).color = c;
        GetImage(ApparenceType.eyebrows).color = c;
        GetImage(ApparenceType.beard).color = c;

        Color skinColor = GetColor(ApparenceType.skinColor);
        GetImage(ApparenceType.nose).color = skinColor;
        handImage.color = skinColor;

        UpdateWeaponSprite(currentMember);

        bodyVisual.InitVisual(currentMember);

    }

    public Color GetColor(ApparenceType apparenceType)
    {
        int colorID = currentMember.GetCharacterID(apparenceType);
        return CrewCreator.Instance.GetApparenceItem(apparenceType, colorID).color;
    }

    public void UpdateWeaponSprite(Member member)
    {
        if (CombatManager.Instance != null && !CombatManager.Instance.fighting || member.equipedWeapon == null)
        {
            weaponImage.enabled = false;
            handImage.enabled = false;
            return;
        }

        handImage.sprite = CrewCreator.Instance.handSprites[(int)member.equipedWeapon.weaponType];
        weaponImage.sprite = CrewCreator.Instance.weaponSprites[member.equipedWeapon.spriteID];

        weaponImage.color = Color.white;
    }

    public void TaintLoop ( Color c)
    {
        tainted = true;

        loopTaint = true;

        targetHighlightColor = c;
    }

    public void TaintOnce(Color c)
    {
        tainted = true;

        timer = 0f;

        loopTaint = false;

        targetHighlightColor = c;
    }

    public void ResetTaint()
    {
        tainted = false;

        InitVisual();
    }

    public Image GetImage(ApparenceType apparenceType)
    {
        return images[(int)apparenceType];
    }

    public void SetDeadEyes()
    {
        GetImage(ApparenceType.eyes).sprite = CrewCreator.Instance.deadEyes_Sprite;
    }

    public void RemoveDeadEyes()
    {
        InitVisual();
    }
}
