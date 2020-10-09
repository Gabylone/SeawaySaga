using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class InGameBackGround : MonoBehaviour {

    public static InGameBackGround Instance;

    public enum Type
    {
        Island,
        House,
        Tavern,
        Cave,
        Forest,
        Village,
        Boat,
    }

	public GameObject group;

    private bool visible = false;

    public Color darkColor;

	public Image image;
    public Sprite[] sprites;

    public float fadeDuration = 1f;

    public Type currentType;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        StoryFunctions.Instance.getFunction += HandleGetFunction;

        CombatManager.Instance.onFightStart += SetDark;
        CombatManager.Instance.onFightEnd += SetWhite;

        Hide();
	}

    private void HandleGetFunction(FunctionType func, string cellParameters)
    {
        if (func == FunctionType.SetBG)
        {
            string backgroundName = cellParameters.Remove(0, 1);

            backgroundName = backgroundName.Remove(backgroundName.Length - 1);

            Type backgroundType = Type.Island;

            bool paramIsValid = Enum.TryParse(backgroundName, out backgroundType);

            if ( !paramIsValid)
            {
                Debug.LogError("Unparsable background type : " + backgroundName + " going with island");
            }

            if (currentType != backgroundType)
            {
                currentType = backgroundType;

                Transitions.Instance.FadeScreen();

                StoryReader.Instance.NextCell();
                StoryReader.Instance.Wait(Transitions.Instance.defaultTransition + 0.5f);

                Invoke("SetSpriteDelay", Transitions.Instance.defaultTransition);
            }
            else
            {
                StoryReader.Instance.NextCell();
                StoryReader.Instance.UpdateStory();
            }

        }
    }

    void SetSpriteDelay()
    {
        SetSprite(currentType);
    }

    public void ShowBackground()
    {
        Show();

        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.boat)
        {
            SetSprite(Type.Boat);
        }
        else
        {
            SetSprite(Type.Island);
        }
    }

    void SetSprite( Type type )
    {
        currentType = type;

        image.sprite = sprites[(int)type];

        SoundManager.Instance.UpdateAmbianceSound();
    }

    public void SetDark()
    {
        image.DOColor(darkColor, 0.5f);
    }

    public void SetWhite()
    {
        image.DOColor(Color.white, 0.5f);
    }

    public void Show()
    {
        group.SetActive(true);
        visible = true;
    }

	public void Hide ()
    {
        group.SetActive(false);
        visible = false;
	}
}
