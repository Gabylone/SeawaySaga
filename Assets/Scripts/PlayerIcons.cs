using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class PlayerIcons : MonoBehaviour {

    public static PlayerIcons Instance;

    public MemberSocket[] memberSockets;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {

        memberSockets = GetComponentsInChildren<MemberSocket>();
    }

    public void Show()
    {
        int i = 0;

        foreach (var item in memberSockets)
        {

            if (i < Crews.playerCrew.CrewMembers.Count)
            {
                item.Show();
            }
            else
            {
                item.Hide();
            }

            i++;

        }
    }

    public void Hide()
    {
        foreach (var item in memberSockets)
        {
            item.Hide();
        }
    }

    public void HandleOpenInventory ()
	{
        Show();

        foreach (var item in memberSockets)
        {
            item.Deselect();
        }

        int id = Crews.playerCrew.CrewMembers.FindIndex(x => x.MemberID.SameAs(CrewMember.GetSelectedMember.MemberID));
        memberSockets[id].Select();
    }

    public void HandleCloseInventory()
    {
        foreach (var item in memberSockets)
        {
            item.Deselect();
        }
    }
}
