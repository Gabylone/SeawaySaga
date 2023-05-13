using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCrewMemberLevelUp : Displayable
{
    public static DisplayCrewMemberLevelUp Instance;

    // content
    public Text uiText_Title;
    public Text uiText_Content;

    public delegate void OnConfirm();
    public OnConfirm onConfirm;

    public IconVisual iconVisual;

    public List<CrewMember> crewMembersToDisplay = new List<CrewMember>();

    private void Awake()
    {
        Instance = this;
    }

    public override void Start()
    {
        base.Start();
    }

    public void Display(CrewMember crewMember)
    {
        crewMembersToDisplay.Add(crewMember);

        if (CombatManager.Instance.fighting)
        {
            ////Debug.Log( crewMember.MemberName + " leveled up but it's a fight, so we'll show at the end");
        }
        else
        {
            DisplayLastCrewMember();
        }
    }
    
    public void DisplayLastCrewMember()
    {
        StoryInput.Instance.Lock();

        Show();

        SoundManager.Instance.PlaySound("Big Tap");
        SoundManager.Instance.PlaySound("Mystick Tap");
        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Tribal");

        CrewMember crewMember = crewMembersToDisplay[0];

        iconVisual.InitVisual(crewMember.MemberID);

        uiText_Title.text = crewMember.MemberName.ToUpper();
        uiText_Content.text = "is now level " + crewMember.Level;
    }

    public void Confirm()
    {
        crewMembersToDisplay.RemoveAt(0);

        if (crewMembersToDisplay.Count > 0)
        {
            DisplayLastCrewMember();
        }
        else
        {
            Hide();
            StoryInput.Instance.Unlock();
        }
    }

    public override void HideDelay()
    {
        base.HideDelay();

        if (CombatManager.Instance.fighting)
        {
            CombatManager.Instance.HandleOnConfirm_Victory_Continue();
        }
    }
}
