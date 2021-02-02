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
        Dark
    }

	public GameObject group;

    private bool visible = false;

    public Color darkColor;
    public Color blackColor;

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

        Hide();
	}

    private void HandleGetFunction(FunctionType func, string cellParameters)
    {
        if (func == FunctionType.SetBG)
        {
            string backgroundName = cellParameters.Remove(0, 1);

            backgroundName = backgroundName.Remove(backgroundName.Length - 1);

            Crews.playerCrew.UpdateCrew(Crews.PlacingType.Hidden);
            Crews.enemyCrew.UpdateCrew(Crews.PlacingType.Hidden);

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

        SetSprite(Type.Boat);

        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.boat)
        {
        }
        else
        {
            //SetSprite(Type.Island);
        }
    }

    void SetSprite(Type type)
    {
        currentType = type;

        if (type == Type.Dark)
        {
            image.color = blackColor;
        }
        else
        {
            image.sprite = sprites[(int)type];
            image.color = Color.white;
        }

        switch (type)
        {
            case Type.Island:
            case Type.Forest:
            case Type.Village:
            case Type.Boat:
                if (TimeManager.Instance.raining)
                {
                    TimeManager.Instance.ShowRain();
                }

                if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
                {
                    TimeManager.Instance.ShowNight();
                }
                break;
            case Type.House:
            case Type.Tavern:
            case Type.Cave:
                if (TimeManager.Instance.raining)
                {
                    TimeManager.Instance.HideRain();
                }

                if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
                {
                    TimeManager.Instance.HideNight();
                }
                break;
            default:
                break;
        }

        SoundManager.Instance.UpdateAmbianceSound();

        if ( currentType == Type.Tavern)
        {
            MusicManager.Instance.PlayTavernMusic();
        }
    }

    public bool IsInterior()
    {
        if (!StoryLauncher.Instance.PlayingStory)
        {
            return false;
        }

        switch (currentType)
        {
            case Type.Island:
            case Type.Forest:
            case Type.Village:
            case Type.Boat:
                return false;
                break;
            case Type.House:
            case Type.Tavern:
            case Type.Cave:
                return true;
                break;
            default:
                break;
        }

        return false;
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
