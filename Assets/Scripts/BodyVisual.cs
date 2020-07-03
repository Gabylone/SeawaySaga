using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyVisual : MonoBehaviour
{
    public enum ID
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

    public void InitVisual( Member memberID)
    {
        int bodyID = memberID.GetCharacterID(ApparenceType.bodyType);

        CrewCreator.BodySet bodySet = CrewCreator.Instance.GetBodySet(bodyID);

        for (int i = 0; i < bodySet.sprites.Length; i++)
        {
            ID id = (ID)i;

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

        int skinColorID = memberID.GetCharacterID(ApparenceType.skinColor);
        Color skinColor = CrewCreator.Instance.GetApparenceItem(ApparenceType.skinColor, skinColorID).color;
        GetImage(ID.Skin).color = skinColor;
        GetImage(ID.LeftArm).color = skinColor;
        GetImage(ID.RightArm).color = skinColor;
        GetImage(ID.Face).color = skinColor;

        int topColorID = memberID.GetCharacterID(ApparenceType.topColor);
        GetImage(ID.Top).color = CrewCreator.Instance.GetApparenceItem(ApparenceType.topColor, topColorID).color;

        int pantsColor = memberID.GetCharacterID(ApparenceType.pantColor);
        GetImage(ID.Pants).color = CrewCreator.Instance.GetApparenceItem(ApparenceType.pantColor, pantsColor).color;

        int shoesColorID = memberID.GetCharacterID(ApparenceType.shoesColor);
        GetImage(ID.Shoes).color = CrewCreator.Instance.GetApparenceItem(ApparenceType.shoesColor, shoesColorID).color;

    }

    void RandomizeColors()
    {

    }

    public Image GetImage (ID id)
    {
        return images[(int)id];
    }
}
