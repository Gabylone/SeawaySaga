using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyVisual : MonoBehaviour
{
    public enum BodyID
    {
        LeftArm,
        Skin,
        Shoes,
        Pants,
        Top,
        RightArm,
        Face
    }

    public Image[] images;
    public IconVisual IconVisual;

    public Transform faceGroup;

    public Transform itemAnchor;

    public delegate void OnApplyEffect();
    public OnApplyEffect onApplyEffect;

    public void InitVisual(Member memberID)
    {
        int bodyID = memberID.GetCharacterID(ApparenceType.bodyType);

        CrewCreator.BodySet bodySet = CrewCreator.Instance.GetBodySet(bodyID);

        for (int i = 0; i < bodySet.sprites.Length; i++)
        {
            BodyID id = (BodyID)i;

            if (bodySet.sprites[i] != null)
            {
                GetImage(id).enabled = true;
                GetImage(id).sprite = bodySet.sprites[i];
            }
            else
            {
                GetImage(id).enabled = false;
            }

            int index = System.Array.FindIndex(bodySet.layerOrder, x => x == id);
            GetImage(id).rectTransform.SetSiblingIndex(index);
        }

        int faceIndex = faceGroup.GetSiblingIndex() - 1;
        faceGroup.SetSiblingIndex(faceIndex);

        // skin color
        Color skinColor = IconVisual.GetColor(ApparenceType.skinColor);
        GetImage(BodyID.Skin).color = skinColor;
        GetImage(BodyID.LeftArm).color = skinColor;
        GetImage(BodyID.RightArm).color = skinColor;
        GetImage(BodyID.Face).color = skinColor;

        // clothe color
        GetImage(BodyID.Top).color = IconVisual.GetColor(ApparenceType.topColor);
        GetImage(BodyID.Pants).color = IconVisual.GetColor(ApparenceType.pantColor);
        GetImage(BodyID.Shoes).color = IconVisual.GetColor(ApparenceType.shoesColor);
    }

    public void ApplyEffect()
    {
        if (onApplyEffect != null)
        {
            onApplyEffect();
        }
    }

    public void AnimationEvent_1()
    {
        if (GetComponentInParent<Fighter>() == SkillManager.Instance.currentSkill.fighter)
        {
            SkillManager.Instance.currentSkill.AnimationEvent_1();
        }
    }

    public void AnimationEvent_2()
    {
        if (GetComponentInParent<Fighter>() == SkillManager.Instance.currentSkill.fighter)
        {
            SkillManager.Instance.currentSkill.AnimationEvent_2();
        }
    }

    public Image GetImage(BodyID id)
    {
        return images[(int)id];
    }

    #region sounds
    public void PlayStepSound_1()
    {
        SoundManager.Instance.PlaySound("button_tap_light 02");
    }
    public void PlayStepSound_2()
    {
        SoundManager.Instance.PlaySound("button_tap_light 03");
    }
    #endregion
}
