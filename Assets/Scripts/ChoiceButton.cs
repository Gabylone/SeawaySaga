using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    public enum Type
    {
        Leave,
        Attack,
        Trade,
        Other,
        Sleep,
        NewMember,
        Loot,
        Quest,

        Normal,
    }

    private Transform _transform;

    public Image image;
    public Text text;

    public Type type;

    public int startIndex = 20;

    public Image feedbackImage;

    public int id = 0;

    public void Init(string str)
    {
        gameObject.SetActive(true);

        _transform = GetComponent<Transform>();

        type = GetBubbleType(str);

        if ( type != Type.Normal && str.Length != ChoiceManager.Instance.bubblePhrases[(int)type].Length)
        {
            str = str.Remove(str.Length - ChoiceManager.Instance.bubblePhrases[(int)type].Length);
        }

        //str = FitText(str);

        str = NameGeneration.CheckForKeyWords(str);
                text.color = Color.black;
        text.text = str;

        // image
        if (str.StartsWith("("))
        {
            image.sprite = ChoiceManager.Instance.bubbleSprites[0];
        }
        else
        {
            image.sprite = ChoiceManager.Instance.bubbleSprites[1];
        }

        // bubble color
        image.color = ChoiceManager.Instance.bubbleColors[(int)type];

        // bubble type
        if (type == Type.Normal)
        {
            feedbackImage.enabled = false;
        }
        else
        {
            feedbackImage.enabled = true;
            feedbackImage.sprite = ChoiceManager.Instance.feedbackSprites[(int)type];

            if ( type == Type.Attack)
            {
                text.color = Color.white;
            }
        }

        // tween
        Tween.Bounce(_transform, 0.2f, 1.1f);
    }

    public void Select()
    {
        ChoiceManager.Instance.Choose(id);
    }

    private string FitText(string str)
    {
        int currStartIndex = startIndex;

        if (currStartIndex >= str.Length)
            return str;

        int spaceIndex = str.IndexOf(" ", currStartIndex);

        while (spaceIndex >= startIndex)
        {

            str = str.Insert(spaceIndex, "\n");

            currStartIndex += startIndex;

            if (currStartIndex >= str.Length)
            {
                break;
            }

            spaceIndex = str.IndexOf(" ", currStartIndex);

            //			print (spaceIndex);

            if (startIndex >= 100)
            {
                break;
            }
        }

        return str;
    }

    private Type GetBubbleType(string str)
    {
        Type bubbleType = (Type)0;

        foreach (var bubblePhrase in ChoiceManager.Instance.bubblePhrases)
        {
            if (str.EndsWith(bubblePhrase))
                return bubbleType;

            ++bubbleType;
        }

        return bubbleType;

    }
}
