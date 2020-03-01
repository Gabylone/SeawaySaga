﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class UIBackground : MonoBehaviour {

	RectTransform rectTransform;

    public Vector2 initPos;
	public float skillX = 0f;
	public float hiddenX = 0f;

	public float duration = 0.3f;

	public GameObject uiGroup;
    public GameObject playerIconsObj;

	// Use this for initialization
	void Start () {
		rectTransform = GetComponent<RectTransform> ();

        CombatManager.Instance.onChangeState += HandleOnChangeState;

        CombatManager.Instance.onFightStart += HideBackground;
		CombatManager.Instance.onFightEnd += HandleFightEnding;

		initPos = rectTransform.anchoredPosition;
	}

    private void HandleOnChangeState(CombatManager.States currState, CombatManager.States prevState)
    {
        if (!CombatManager.Instance.fighting)
        {
            ShowBackGround();
            return;
        }
        switch (currState)
        {
            case CombatManager.States.CombatStart:
            case CombatManager.States.PlayerMemberChoice:
            case CombatManager.States.EnemyMemberChoice:
            case CombatManager.States.StartTurn:
            case CombatManager.States.EnemyActionChoice:
            case CombatManager.States.PlayerAction:
            case CombatManager.States.EnemyAction:
                HideBackground();
                break;


            case CombatManager.States.PlayerActionChoice:
                MoveBackGround();
                break;

            default:
                break;
        }
    }

    void HandleFightEnding ()
	{
		Invoke("HandleFightEndingDelay" , duration);
        ShowBackGround();
    }

    void HandleFightEndingDelay () {

        Crews.playerCrew.UpdateCrew (Crews.PlacingType.Map);
	}

	void ShowBackGround ()
	{
        rectTransform.DOAnchorPos(initPos, duration);

        playerIconsObj.SetActive(true);
		//uiGroup.SetActive (true);
    }

	void MoveBackGround ()
	{
        rectTransform.DOAnchorPos(new Vector2(skillX, initPos.y), duration);

		//uiGroup.SetActive (false);
        playerIconsObj.SetActive(false);
	}

	void HideBackground ()
	{
        rectTransform.DOAnchorPos(new Vector2(hiddenX, initPos.y), duration);

        playerIconsObj.SetActive(false);
		//uiGroup.SetActive (false);

    }
}