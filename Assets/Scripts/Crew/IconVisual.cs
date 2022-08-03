using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class IconVisual : MonoBehaviour
{
    public RectTransform rectTransform;

    public BodyVisual bodyVisual;

    private Member currentMember;

    /// <summary>
    ///  images
    /// </summary>
    public Image weaponImage;
    public Image handImage;

    public GameObject[] bodyParts;

    public Image[] images;

    /// <summary>
    ///  taint
    /// </summary>
    public float highlightSpeed = 1f;
    private bool tainted = false;
    private Color targetHighlightColor;
    private bool loopTaint = false;
    public float taintOnceDuration = 0.5f;

    public GameObject poisonPuddle_Obj;
    public GameObject poisonEffect_Obj;
    public Animator healEffect_Anim;
    public Animator hitEffect_Anim;
    public float hitEffect_Decal = 1f;
    public float hitEffect_CircleRange = 1f;
    public Transform hitEffect_Transform;
    public GameObject food_Obj;
    public Transform rhumBottle_Transform;
    public Transform bearTrap_Transform;

    /// <summary>
    /// override skin color
    /// </summary>
    public bool overrideSkinColor_Active = false;
    public Color overrideSkinColor_Color;

    // face effects
    public bool aiming = false;
    public bool knockedOut = false;
    public bool mad = false;
    public bool happy = false;
    public bool sad = false;

    private float lerp = 0f;
    public float range = 0.3f;

    private float timer = 0f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (tainted)
        {
            UpdateTaint();
        }
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

        // hair types...
        SetSprite(ApparenceType.hair);
        SetSprite(ApparenceType.beard);
        SetSprite(ApparenceType.eyebrows);

        // hait types color
        Color c = GetColor(ApparenceType.hairColor);
        GetImage(ApparenceType.hair).color = c;
        GetImage(ApparenceType.eyebrows).color = c;
        GetImage(ApparenceType.beard).color = c;

        SetSprite(ApparenceType.eyes);

        // skin
        SetSprite(ApparenceType.mouth);
        SetSprite(ApparenceType.nose);

        // skin color
        Color skinColor = GetColor(ApparenceType.skinColor);
        GetImage(ApparenceType.nose).color = skinColor;
        handImage.color = skinColor;

        // expressions
        if ( knockedOut)
        {
            GetImage(ApparenceType.eyes).sprite = CrewCreator.Instance.deadEyes_Sprite;
        }

        if (sad)
        {
            GetImage(ApparenceType.eyes).sprite = CrewCreator.Instance.sadEyes_Sprite;
            GetImage(ApparenceType.eyebrows).sprite = CrewCreator.Instance.sadEyebrows_Sprite;
            GetImage(ApparenceType.mouth).sprite = CrewCreator.Instance.sadMouth_Sprite;
        }

        if (happy)
        {
            GetImage(ApparenceType.eyes).sprite = CrewCreator.Instance.happyEyes_Sprite;
            GetImage(ApparenceType.eyebrows).sprite = CrewCreator.Instance.happyEyes_Sprite;
            GetImage(ApparenceType.mouth).sprite = CrewCreator.Instance.happyMouth_Sprite;
        }

        if (mad)
        {
            GetImage(ApparenceType.eyes).sprite = CrewCreator.Instance.madEyes_Sprite;
            GetImage(ApparenceType.eyebrows).sprite = CrewCreator.Instance.madEyebrows_Sprite;
            GetImage(ApparenceType.mouth).sprite = CrewCreator.Instance.madMouth_Sprite;
        }

        if (aiming)
        {
            GetImage(ApparenceType.eyes).sprite = CrewCreator.Instance.aimingEyes_Sprite;
        }

        // weapons
        UpdateWeaponSprite(currentMember);

        bodyVisual.InitVisual(currentMember);

    }

    public void ChangeEyes(Sprite sprite)
    {

    }

    #region taint
    private void UpdateTaint()
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

        // hair etc...
        GetImage(ApparenceType.hair).color = LerpColor(ApparenceType.hair, ApparenceType.hairColor);
        GetImage(ApparenceType.beard).color = LerpColor(ApparenceType.beard, ApparenceType.hairColor);
        GetImage(ApparenceType.eyebrows).color = LerpColor(ApparenceType.eyebrows, ApparenceType.hairColor);

        // clothes
        bodyVisual.GetImage(BodyVisual.BodyID.Top).color = LerpColor(BodyVisual.BodyID.Top, ApparenceType.topColor);
        bodyVisual.GetImage(BodyVisual.BodyID.Pants).color = LerpColor(BodyVisual.BodyID.Pants, ApparenceType.pantColor);
        bodyVisual.GetImage(BodyVisual.BodyID.Shoes).color = LerpColor(BodyVisual.BodyID.Shoes, ApparenceType.shoesColor);

        // skin
        GetImage(ApparenceType.nose).color = LerpColor(ApparenceType.nose, ApparenceType.skinColor);
        bodyVisual.GetImage(BodyVisual.BodyID.Face).color = LerpColor(BodyVisual.BodyID.Face, ApparenceType.skinColor);
        bodyVisual.GetImage(BodyVisual.BodyID.Skin).color = LerpColor(BodyVisual.BodyID.Skin, ApparenceType.skinColor);
        bodyVisual.GetImage(BodyVisual.BodyID.RightArm).color = LerpColor(BodyVisual.BodyID.RightArm, ApparenceType.skinColor);
        bodyVisual.GetImage(BodyVisual.BodyID.LeftArm).color = LerpColor(BodyVisual.BodyID.LeftArm, ApparenceType.skinColor);
        handImage.color = Color.Lerp(GetColor(ApparenceType.skinColor), targetHighlightColor, lerp);

        // weapon
        weaponImage.color = Color.Lerp(Color.white, targetHighlightColor, lerp);

    }


    public void TaintLoop(Color c)
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

    private Color LerpColor(BodyVisual.BodyID spriteId, ApparenceType colorId)
    {
        return Color.Lerp(GetColor(colorId), targetHighlightColor, lerp);
    }
    private Color LerpColor(ApparenceType spriteId, ApparenceType colorId)
    {
        return Color.Lerp(GetColor(colorId), targetHighlightColor, lerp);
    }
    #endregion

    #region skin color
    public void OverrideSkinColor ( Color color)
    {
        overrideSkinColor_Active = true;
        overrideSkinColor_Color = color;

        InitVisual();
    }

    public void ResetSkinColor()
    {
        overrideSkinColor_Active = false;

        InitVisual();
    }
    #endregion

    public void SetSprite(ApparenceType apparenceType)
    {
        int id = currentMember.GetCharacterID(apparenceType);
        SetSprite(apparenceType, id);
    }

    public void SetRandomSprite(ApparenceType apparenceType)
    {
        if (currentMember.IsZombie())
        {
            return;
        }

        int l= CrewCreator.Instance.apparenceGroups[(int)apparenceType].items.Count;
        int randomID = Random.Range(0, l);
        if (apparenceType == ApparenceType.eyes && randomID == 3)
            randomID -= 1;
        SetSprite(apparenceType, randomID);
    }

    public void SetSprite(ApparenceType apparenceType, int id)
    {
        if (currentMember.IsZombie())
        {
            switch (apparenceType)
            {
                case ApparenceType.eyes:
                    GetImage(apparenceType).sprite = CrewCreator.Instance.zombie_EyesSprite[id];
                    return;
                case ApparenceType.mouth:
                    GetImage(apparenceType).sprite = CrewCreator.Instance.zombie_MouthSprite[id];
                    return;
                case ApparenceType.nose:
                    GetImage(apparenceType).sprite = CrewCreator.Instance.zombie_NoseSprite[id];
                    return;
                default:
                    break;
            }
        }

        Sprite spr = CrewCreator.Instance.GetApparenceItem(apparenceType, id).GetSprite(); ;


        if (spr == null)
        {
            GetImage(apparenceType).enabled = false;
        }
        else
        {
            GetImage(apparenceType).enabled = true;
            //GetImage(apparenceType).sprite = CrewCreator.Instance.GetApparenceItem(apparenceType, currentMember.GetCharacterID(apparenceType)).GetSprite();
            GetImage(apparenceType).sprite = spr;
        }
    }

    public Color GetColor(ApparenceType apparenceType)
    {
        if ( apparenceType == ApparenceType.skinColor)
        {
            if (overrideSkinColor_Active)
            {
                return overrideSkinColor_Color;
            }

            if (currentMember.IsZombie())
            {
                // mettre le meme nombre d'id de peaux zombies 
                // que de peaux normales parce que le code est ECALATE
                // ne pas hésiter à mettre des doubles
                return CrewCreator.Instance.zombie_SkinColors[currentMember.GetCharacterID(ApparenceType.skinColor)];
            }
        }

        int colorID = currentMember.GetCharacterID(apparenceType);
        return CrewCreator.Instance.GetApparenceItem(apparenceType, colorID).color;
    }

    #region weapons
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
    #endregion

    #region expressions
    /// <summary>
    /// mad
    /// </summary>
    public void SetMadFace()
    {
        mad = true;

        InitVisual();
    }

    public void RemoveMadFace()
    {
        mad = false;

        InitVisual();
    }

    public void ResetEffects()
    {
        poisonPuddle_Obj.SetActive(false);
        poisonEffect_Obj.SetActive(false);
        healEffect_Anim.gameObject.SetActive(false);
        hitEffect_Transform.gameObject.SetActive(false);
        food_Obj.SetActive(false);
        rhumBottle_Transform.gameObject.SetActive(false);
        bearTrap_Transform.gameObject.SetActive(false);
}

    /// <summary>
    /// knocked out
    /// </summary>
    public void SetDeadEyes()
    {
        knockedOut = true;
        InitVisual();
    }

    public void RemoveDeadEyes()
    {
        knockedOut = false;
        InitVisual();
    }

    /// <summary>
    /// aiming
    /// </summary>
    public void SetAimingEyes()
    {
        aiming = true;
        InitVisual();
    }

    public void RemoveAimingEyes()
    {
        aiming = false;
        InitVisual();
    }

    /// <summary>
    ///  happy
    /// </summary>
    public void SetHappyFace()
    {
        happy = true;
        InitVisual();
    }

    public void RemoveHappyFace()
    {
        happy = false;
        InitVisual();
    }

    /// <summary>
    /// sad
    /// </summary>
    public void SetSadFace()
    {
        sad = true;
        InitVisual();
    }

    public void RemoveSadFace()
    {
        sad = false;
        InitVisual();
    }

    /// <summary>
    /// 
    /// </summary>
    public void ResetFaceExpressions()
    {
        knockedOut = false;
        aiming = false;
        sad = false;
        happy = false;
        mad = false;

        InitVisual();
    }
    #endregion

    public Image GetImage(ApparenceType apparenceType)
    {
        return images[(int)apparenceType];
    }
}
