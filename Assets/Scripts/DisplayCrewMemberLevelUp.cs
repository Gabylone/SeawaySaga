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
        Show();

        crewMembersToDisplay.Add(crewMember);

        DisplayLastCrewMember();
    }
    
    private void DisplayLastCrewMember()
    {
        CrewMember crewMember = crewMembersToDisplay[0];

        iconVisual.InitVisual(crewMember.MemberID);

        uiText_Title.text = crewMember.MemberName.ToUpper();
        uiText_Content.text = "is now level " + crewMember.Level;
    }

    public void Confirm()
    {
        Hide();
    }

    public override void HideDelay()
    {
        base.HideDelay();
        
        if ( crewMembersToDisplay.Count > 0)
        {
            crewMembersToDisplay.RemoveAt(0);
            DisplayLastCrewMember();
        }
    }
}
