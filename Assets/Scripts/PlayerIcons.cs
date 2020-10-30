using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class PlayerIcons : MonoBehaviour {

    public static PlayerIcons Instance;

    public Image[] images;
    public GameObject[] deleteMember_Objs;

    public CanvasGroup canvasGroup;

    public float fadeDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

		Crews.playerCrew.onChangeCrewMembers += HandleOnChangeCrewMembers;

        InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		InGameMenu.Instance.onCloseMenu += HandleOnCloseInventory;

        HandleOnChangeCrewMembers();

        canvasGroup.alpha = 0f;
	}

    public void FadeIn()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration);
    }

    public void FadeOut()
    {
        canvasGroup.DOFade(0f, fadeDuration);
    }

    public Image GetImage ( int id)
    {
        return images[id];
    }

    private void HandleOnCloseInventory()
    {
        FadeOut(); 

        foreach (var item in images)
        {
            item.color = Color.white;
        }
    }

    void HandleOpenInventory ()
	{
        Invoke( "HandleOpenInventoryDelay" , 0.001f );
	}

    void HandleOpenInventoryDelay()
    {
        FadeIn();

        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = Color.grey;
            deleteMember_Objs[i].SetActive(false);
        }

        int id = Crews.playerCrew.CrewMembers.FindIndex(x => x.MemberID.SameAs(CrewMember.GetSelectedMember.MemberID));

        images[id].color = Color.white;

        if ( Crews.playerCrew.CrewMembers.Count > 1)
        {
            deleteMember_Objs[id].SetActive(true);
        }
    }

    void HandleOnChangeCrewMembers ()
	{
		int i = 0;

		foreach (var item in images) {

			if (i < Crews.playerCrew.CrewMembers.Count) {
				item.enabled = true;
			} else {
				item.enabled = false;
			}

			i++;

		}
	}
}
