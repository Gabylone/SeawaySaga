using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCombatResults : Displayable
{
    public static DisplayCombatResults Instance;

    // content
    public Text uiText_Title;
    public Text uiText_Content;

    public delegate void OnConfirm();
    public OnConfirm onConfirm;

    [Header("Fight Results")]
    public GameObject fightResults_Group;
    public Text fightResults_Gold_Text;
    public Text fightResults_XP_Text;

    private void Awake()
    {
        Instance = this;
    }

    public override void Start()
    {
        base.Start();
    }

    public void Display(string title, string content)
    {
        Show();

        fightResults_Group.SetActive(false);

        uiText_Title.text = title;
        uiText_Content.text = content;
    }

    public void DisplayResults(int gold, int xp)
    {
        fightResults_Group.SetActive(true);

        fightResults_Gold_Text.text = "" + gold;
        fightResults_XP_Text.text = "" + xp + "\n" + "xp each";
    }

    public void Confirm()
    {
        if (onConfirm != null)
        {
            onConfirm();
        }
    }
}
