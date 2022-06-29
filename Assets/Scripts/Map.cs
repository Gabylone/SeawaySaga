using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class Map : MonoBehaviour, IPointerClickHandler {

    public MapParameters mapParameters;

    public Image mapImage;

    public GameObject padlockGroup;

    public int progression = 0;
    int max = 0;

    public Image progression_FillImage;
    public Image progression_BackgroundImage;

    public Text pearls_UiText;

    public Text mapName_UIText;

    public GameObject loadGroup;

    public GameObject[] membersGroups;
    public IconVisual[] targetIconVisuals;
    public Text saveName_Text;
    public Text[] captainUITexts;
    public Text[] levelUITexts;

    public GameObject newGameGroup;

    public GameObject finishedGroup;

    public GameObject confirmNewGame_Group;
    public RectTransform validateNewGame_RectTransform;

    private bool load = false;

    public bool retry = false;

    // Use this for initialization
    void Start()
    {
        UpdateUI();

        max = System.Enum.GetValues(typeof(TutorialStep)).Length;

        confirmNewGame_Group.SetActive(false);
    }

    public void UpdateUI()
    {
        loadGroup.SetActive(false);

        // remove return
        //mapName_UIText.text = mapParameters.mapName.Replace(' ', '\n');
        mapName_UIText.text = mapParameters.mapName;

        string currentMap_data = PlayerPrefs.GetString("map_data" + mapParameters.id, mapParameters.id == 0 ? "unlocked": "locked");

        if (retry)
        {
            padlockGroup.SetActive(false);
        }
        else
        {
            if (currentMap_data == "locked")
            {
                padlockGroup.SetActive(true);
                newGameGroup.SetActive(false);
            }
            else if (currentMap_data == "finished")
            {
                finishedGroup.SetActive(true);
                padlockGroup.SetActive(false);
                newGameGroup.SetActive(false);
                load = false;

                HideMemberIcons();
                return;
            }
            else
            {
                padlockGroup.SetActive(false);
            }
        }

        

        if (SaveTool.Instance.FileExists(mapParameters.mapName, "game data"))
        {
            loadGroup.SetActive(true);

            load = true;

            GameData gameData = SaveTool.Instance.LoadFromSpecificPath(mapParameters.mapName, "game data.xml", "GameData") as GameData;

            string str = gameData.playerCrew.MemberIDs[0].Name + ", captain of \n" +
                gameData.playerBoatInfo.Name + ", and its crew";
            saveName_Text.text = str;

            HideMemberIcons();

            for (int i = 0; i < gameData.playerCrew.MemberIDs.Count; i++)
            {
                Member member = gameData.playerCrew.MemberIDs[i];

                membersGroups[i].SetActive(true);


                targetIconVisuals[i].InitVisual(member);


                //captainUITexts[i].text = member.Name;

                levelUITexts[i].text = member.Lvl.ToString();

                /*Sprite[] jobSprites = Resources.LoadAll<Sprite>("Graph/JobSprites");
                jobImage.sprite = jobSprites[(int)captain.job];*/
            }

            newGameGroup.SetActive(false);

        }
        else
        {
            load = false;

            HideMemberIcons();

            newGameGroup.SetActive(currentMap_data != "locked");
        }
    }

    void HideMemberIcons()
    {
        foreach (var item in membersGroups)
        {
            item.SetActive(false);
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
        string currentMapKey = "map_data" + mapParameters.id;
        string currentMap_data = PlayerPrefs.GetString(currentMapKey, mapParameters.id == 0 ? "unlocked" : "locked");

        if (currentMap_data == "finished"&& !retry)
        {
            return;
        }

        Tween.Bounce(transform, 0.1f , 1.01f);


        if (currentMap_data == "locked")
        {
            SoundManager.Instance.PlayRandomSound("button_tap_light");
            SoundManager.Instance.PlaySound("ui_wrong");
            //DisplayPurchase.Instance.Display(apparenceItem, transform);
            return;
        }


        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Swipe");

        confirmNewGame_Group.SetActive(true);
    }

    public void ConfirmLaunchGame()
    {
        Tween.Bounce(validateNewGame_RectTransform);

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

        if (mapParameters.id == 0 && !load)
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

        

        Invoke("LaunchMapDelay", 1f);
    }

    void LaunchMapDelay()
    {
        LoadingScene.sceneToLoad = "Main";
        SceneManager.LoadScene("Loading");
    }

    public void EraseMap()
    {
        MessageDisplay.Instance.Display("Erase game ?", true);

        MessageDisplay.Instance.onValidate += ConfirmEraseMap;

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlayRandomSound("Bag");
    }

    void ConfirmEraseMap()
    {
        MessageDisplay.Instance.onValidate -= ConfirmEraseMap;

        SaveTool.Instance.DeleteFolder(mapParameters.mapName);

        Tween.Bounce(transform);

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlayRandomSound("Writing");
        SoundManager.Instance.PlayRandomSound("Page");

        UpdateUI();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LaunchMap();
    }
}
