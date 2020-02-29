using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayQuest : MonoBehaviour {

    public static DisplayQuest Instance;

    public GameObject group;

    public Text nameText;

    public Text rewardText;
    public Text levelText;

    public Text descriptionText;

    public GameObject achievedFeedback;

    public GameObject giveUpButtonObj;

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
        nameText.text = quest.Story.name;

        rewardText.text = "Récompense : " + quest.goldValue + " / XP : " + quest.experience;

        descriptionText.text = "Donnée par " + quest.giver.Name + "\n";

        descriptionText.text += "\n";

        for (int i = 1; i < 3; i++)
        {
            descriptionText.text += "''" + quest.Story.content[0][i].Remove(0,13) + "''";
        }

        giveUpButtonObj.SetActive(true);

        currentQuest.ShowOnMap();

        if (quest.accomplished)
        {
            levelText.gameObject.SetActive(false);
            achievedFeedback.SetActive(true);
        }
        else
        {
            levelText.text = "Niveau conseillé : " + quest.level.ToString();
            levelText.gameObject.SetActive(true);
            achievedFeedback.SetActive(false);
        }
    }

    #region main quest
    public void DisplayMainQuest()
    {
        Show();

        nameText.text = "Le Trésor";

        DisplayFormulasOnDescription();

        giveUpButtonObj.SetActive(false);

        levelText.text = "";
        rewardText.text = "";

        achievedFeedback.SetActive(false);
        
    }

    private void DisplayFormulasOnDescription()
    {
        string str = "";

        bool foundOne = false;

        int formulaIndex = 0;
        foreach (var form in FormulaManager.Instance.formulas)
        {
            if (form.found == true)
            {

                str += form.name.ToUpper() + "\n";

                foundOne = true;

            }

            formulaIndex++;

        }

        if (foundOne == false)
        {
            str = "Aucun indices";
        }

        descriptionText.text = str;
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
        MessageDisplay.onValidate += HandleOnValidate;

        MessageDisplay.Instance.Show("Abandonner quête ?");
    }

    public void HandleOnValidate()
    {
        QuestManager.Instance.GiveUpQuest(currentQuest);
    }
}
