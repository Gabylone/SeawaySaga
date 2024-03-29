﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

    public enum Language
    {
        _fr,
        _en,
    }

    public static Language language = Language._en;

	public bool loadOnStart = false;
	public bool saveOnStart = false;
	public bool hideMemberCreation = false;

	void Start () {

		Application.targetFrameRate = 60;

		Instance = this;

		InitializeGame ();
	}

	public void InitializeGame () {

		ItemLoader.Instance.Init ();

		FormulaManager.Instance.Init ();

		Crews.Instance.Init ();


        if (loadOnStart) {
			
			KeepOnLoad.dataToLoad = 0;
            KeepOnLoad.Instance.mapName = "Default";
			SaveManager.Instance.LoadGame ();

		} else if (KeepOnLoad.dataToLoad >= 0) {

			SaveManager.Instance.LoadGame ();

		} else {

			MapGenerator.Instance.CreateNewMap ();

			LootManager.Instance.CreateNewLoot ();

			Crews.Instance.RandomizePlayerCrew ();

			Boats.Instance.RandomizeBoats ();

			GoldManager.Instance.InitGold ();

			TimeManager.Instance.Reset ();

        }

		InGameMenu.Instance.Init ();

		WeightManager.Instance.Init ();

		QuestMenu.Instance.Init ();

		if (KeepOnLoad.dataToLoad < 0) {

            MemberCreator.Instance.Show();

		}

	}

	public void BackToMenu () {

        Transitions.Instance.ScreenTransition.FadeIn(1f);

        Invoke("BackToMenuDelay", 1f);

    }
    private void BackToMenuDelay()
    {
        SceneManager.LoadScene ("Menu");
	}

    public void QuitGame()
    {
        Transitions.Instance.ScreenTransition.FadeIn(1);
        Invoke("QuitGameDelay" , 1f);
    }

    void QuitGameDelay()
    {
        Application.Quit();
    }
}
