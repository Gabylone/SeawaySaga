using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQuest : MonoBehaviour {

    public static DisplayQuest Instance;

    public GameObject group;

    public Text nameText;

    public Text gold_Text;
    public Text experience_Text;

    public ScrollRect ScrollRect;

    public Text description_Text;

    public GameObject achievedFeedback;
    public GameObject giveUpButtonObj;

    public GameObject infoGroup;

    Quest currentQuest;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();
    }

    public void Display ( Quest quest)
    {
        Show();

        currentQuest = quest;
        nameText.text = quest.Story.displayName;

        //gold_Text.text = "Reward : " + quest.goldValue + " / XP : " + quest.experience;
        gold_Text.text = "" + quest.goldValue;
        experience_Text.text = "" + quest.experience;
        description_Text.text = "Given by " + quest.giver.Name + "\n";

        if (quest.level == 10)
        {
            description_Text.text = "<i>(most suited for level " + quest.level + ")</i>" +
                "\n";
        }
        else
        {
            description_Text.text = "<i>(most suited for level " + quest.level + " or more)</i>" +
                "\n";
        }

        description_Text.text += "\n";

        for (int i = 1; i < 3; i++)
        {
            description_Text.text += "''" + quest.Story.content[0][i].Remove(0,13) + "''";
        }

        giveUpButtonObj.SetActive(true);

        ScrollRect.verticalNormalizedPosition = 1f;

        infoGroup.SetActive(true);

        if (quest.accomplished)
        {
            giveUpButtonObj.SetActive(false);
            achievedFeedback.SetActive(true);
        }
        else
        {
            achievedFeedback.SetActive(false);
            giveUpButtonObj.SetActive(true);
        }
    }

    public void ShowCurrentQuestOnMap()
    {
        currentQuest.ShowOnMap();
    }

    #region main quest
    public void DisplayMainQuest()
    {
        Show();

        nameText.text = "The treasure of " + MapGenerator.Instance.treasureName;

        DisplayFormulasInDescription();

        experience_Text.text = "";
        gold_Text.text = "";
        achievedFeedback.SetActive(false);
        giveUpButtonObj.SetActive(false);

        infoGroup.SetActive(false);

    }

    private void DisplayFormulasInDescription()
    {
        string str = "";

        bool foundOne = FormulaManager.Instance.formulas.Find(x=> x.found == true ) != null;

        if (foundOne == false)
        {
            str += "No clues yet";
        }
        else
        {
            str += "<b>Strange words I heard about the treasure : </b>\n\n";

            foreach (var form in FormulaManager.Instance.formulas)
            {
                if (form.found == true)
                {
                    str += form.name.ToUpper() + "\n";
                }
            }
        }

        if (FormulaManager.Instance.clueIndexesFound.Count > 0)
        {
            str += "\n\n";
            str += "<b>Useful rumors I heard about the treasure : </b>\n\n";

            foreach (var clueIndex in FormulaManager.Instance.clueIndexesFound)
            {
                string clue_str = ClueManager.Instance.GetClue(clueIndex).Replace('*', '\n');
                clue_str = NameGeneration.CheckForKeyWords(clue_str);
                str += clue_str + "\n\n";
            }
        }

        description_Text.text = str;
    }
    #endregion

    public void Show()
    {
        group.SetActive(true);
    }
    
    public void Hide()
    {
        group.SetActive(false);
    }

    public void ShowOnMap()
    {
        
    }

    public void GiveUp()
    {
        MessageDisplay.Instance.onValidate += HandleOnValidate;

        MessageDisplay.Instance.Show("Abandon quest ?");
    }

    public void HandleOnValidate()
    {
        QuestManager.Instance.GiveUpQuest(currentQuest);
    }
}
