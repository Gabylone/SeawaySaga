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

    public RectTransform scrollView_RectTransform;

    public Text description_Text;

    public float fullScale = 260f;
    public float normalScale = 200f;

    public RectTransform content_RectTransform;

    public GameObject achievedFeedback;
    public GameObject giveUpButtonObj;

    public GameObject infoGroup;

    private Quest currentQuest;

    public RectTransform[] layoutGroups_RectTransforms;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateLayout();
        }
    }

    public void Display ( Quest quest)
    {
        Show();

        currentQuest = quest;
        nameText.text = quest.Story.displayName;

        // remove feedback on menu
        quest.updated = false;

        gold_Text.text = "" + quest.goldValue;
        experience_Text.text = "" + quest.experience;

        description_Text.text = "Given by " + quest.giver.Name + " in some " + quest.GetOriginIslandData().storyManager.storyHandlers[0].Story.displayName;

        description_Text.text += "\n";
        description_Text.text += "\n";

        if (quest.level == 10)
        {
            description_Text.text += "<i>(most suited for level " + quest.level + ")</i>";
        }
        else
        {
            description_Text.text += "<i>(most suited for level " + quest.level + " or more)</i>";
        }

        description_Text.text += "\n";
        description_Text.text += "\n";

        for (int i = 1; i < 3; i++)
        {
            description_Text.text += "''" + quest.Story.content[0][i].Remove(0,13) + "''";
        }

        giveUpButtonObj.SetActive(true);

        scrollView_RectTransform.sizeDelta = new Vector2(scrollView_RectTransform.sizeDelta.x, normalScale);

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

        UpdateLayout();
    }

    void UpdateLayout()
    {
        foreach (var item in layoutGroups_RectTransforms)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
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

        QuestMenu.Instance.mainQuestUpdated = false;

        ScrollRect.verticalNormalizedPosition = 1f;

        scrollView_RectTransform.sizeDelta = new Vector2(scrollView_RectTransform.sizeDelta.x , fullScale);

        experience_Text.text = "";
        gold_Text.text = "";
        achievedFeedback.SetActive(false);
        giveUpButtonObj.SetActive(false);

        infoGroup.SetActive(false);

        UpdateLayout();
    }

    private void DisplayFormulasInDescription()
    {
        string str = "";

        bool foundOne = FormulaManager.Instance.formulas.Find(x=> x.found == true ) != null;

        if (foundOne == false)
        {
            str += "No clues yet ";
        }
        else
        {
            
            str += "<b>Strange words I heard about the treasure : </b>\n\n";

            foreach (var form in FormulaManager.Instance.formulas)
            {
                if (form.found == true)
                {
                    if(form.verified)
                    {
                        str += "<color=green>" + form.name.ToUpper() + "</color>" + "\n";
                    }
                    else
                    {
                        str += "<color=blue>" + form.name.ToUpper() + "</color>" + "\n";
                    }
                }
            }
        }

        bool colored = false;
        if (FormulaManager.Instance.clueIndexesFound.Count > 0)
        {
            str += "\n\n";
            str += "<b>Useful rumors I heard about the treasure : </b>\n\n";

            foreach (var clueIndex in FormulaManager.Instance.clueIndexesFound)
            {
                string clue_str = ClueManager.Instance.GetClue(clueIndex).Replace('*', '\n');
                clue_str = NameGeneration.CheckForKeyWords(clue_str);

                if (colored)
                {
                    str += "<color=olive>" + clue_str + "</color>" + "\n\n";
                }
                else
                {
                    str += "<color=brown>" + clue_str + "</color>" + "\n\n";
                }

                colored = !colored;
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

        MessageDisplay.Instance.Display("Abandon quest ?");
    }

    public void HandleOnValidate()
    {
        MessageDisplay.Instance.onValidate -= HandleOnValidate;
        QuestManager.Instance.GiveUpQuest(currentQuest);
    }
}
