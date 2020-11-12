using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Map : MonoBehaviour {

    public MapParameters mapParameters;

    public Image mapImage;

    public GameObject padlockGroup;

    public int progression = 0;
    int max = 0;

    public Image progression_FillImage;
    public Image progression_BackgroundImage;

    public Text pearls_UiText;

    public Text mapName_UIText;

    public GameObject iconVisualGroup;
    public IconVisual targetIconVisual;
    public Text captainUIText;
    public Text levelUIText;
    public Image jobImage;

    public GameObject newGameGroup;

    public GameObject finishedGroup;

    private bool load = false;

    public ApparenceItem apparenceItem;

    // Use this for initialization
    void Start()
    {
        UpdateUI();

        max = System.Enum.GetValues(typeof(TutorialStep)).Length;
    }

    public void UpdateUI()
    {
        apparenceItem = CrewCreator.Instance.GetApparenceItem(apparenceItem.apparenceType, apparenceItem.id);

        mapName_UIText.text = mapParameters.mapName.Replace(' ', '\n');

        if (apparenceItem.locked)
        {
            padlockGroup.SetActive(true);
            mapImage.color = Color.black;
            newGameGroup.SetActive(false);
        }
        else if (apparenceItem.finished)
        {
            finishedGroup.SetActive(true);
            padlockGroup.SetActive(false);
            mapImage.color = Color.white;
            newGameGroup.SetActive(false);
            load = false;
            iconVisualGroup.SetActive(false);
            return;
        }
        else
        {
            padlockGroup.SetActive(false);
            mapImage.color = Color.white;
        }

        if (SaveTool.Instance.FileExists(mapParameters.mapName, "game data"))
        {
            load = true;

            iconVisualGroup.SetActive(true);

            GameData gameData = SaveTool.Instance.LoadFromSpecificPath(mapParameters.mapName, "game data.xml", "GameData") as GameData;

            Member captain = gameData.playerCrew.MemberIDs[0];

            targetIconVisual.InitVisual(captain);

            captainUIText.text = captain.Name;
            levelUIText.text = captain.Lvl.ToString();

            Sprite[] jobSprites = Resources.LoadAll<Sprite>("Graph/JobSprites");
            jobImage.sprite = jobSprites[(int)captain.job];

            newGameGroup.SetActive(false);

        }
        else
        {
            load = false;
            iconVisualGroup.SetActive(false);

            newGameGroup.SetActive(!apparenceItem.locked);
        }
    }

    void UpdateProgressionBar()
    {

        float w = progression_BackgroundImage.rectTransform.rect.width;
        float l1 = (float)(progression - 1) / (float)max;
        float l2 = (float)progression / (float)max;

        progression_FillImage.rectTransform.sizeDelta = new Vector2(-(w) + (l1 * w), 0);

    }

    public void LaunchMap()
    {
        if (apparenceItem.finished)
        {
            return;
        }

        Tween.Bounce(transform);


        if (apparenceItem.locked)
        {
            SoundManager.Instance.PlayRandomSound("button_tap_light");
            SoundManager.Instance.PlaySound("ui_wrong");
            //DisplayPurchase.Instance.Display(apparenceItem, transform);
            return;
        }

        MapGenerator.mapParameters = this.mapParameters;

        Transitions.Instance.ScreenTransition.FadeIn(1f);

        if (load)
        {
            KeepOnLoad.dataToLoad = 0;
        }
        else
        {
            KeepOnLoad.dataToLoad = -1;
        }

        if ( mapParameters.id == 0)
        {
            KeepOnLoad.displayTuto = true;
        }
        else
        {
            KeepOnLoad.displayTuto = false;
        }

        KeepOnLoad.Instance.mapName = mapParameters.mapName;

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Swipe");
        SoundManager.Instance.PlayRandomSound("Writing");
        SoundManager.Instance.PlayRandomSound("Page");

        SaveTool.Instance.CreateDirectories();

        Invoke("LaunchMapDelay", 1f);

    }

    void LaunchMapDelay()
    {
        SceneManager.LoadScene("Loading");
    }

    public void EraseMap()
    {
        MessageDisplay.Instance.Show("Erase game ?");

        MessageDisplay.Instance.onValidate += ConfirmEraseMap;

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlayRandomSound("Bag");
    }

    void ConfirmEraseMap()
    {
        SaveTool.Instance.DeleteFolder(mapParameters.mapName);

        Tween.Bounce(transform);

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlayRandomSound("Writing");
        SoundManager.Instance.PlayRandomSound("Page");

        UpdateUI();
    }

}
