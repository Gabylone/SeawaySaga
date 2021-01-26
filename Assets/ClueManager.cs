using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    public static ClueManager Instance;

    [SerializeField]
    private string[] clues;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StoryFunctions.Instance.getFunction += HandleGetFunction;

        string path = MapGenerator.mapParameters.mapName + " Clues";
        TextAsset tipsTextAsset = Resources.Load("Clues/" + path) as TextAsset;
        clues = tipsTextAsset.text.Split('\n');
    }

    void HandleGetFunction(FunctionType func, string cellParameters)
    {
        switch (func)
        {
            case FunctionType.TellClue:
                TellClue(cellParameters);
                break; 
        }
    }


    public void TellClue(string cellParams)
    {
        // choper un decal
        int clueIndex = StoryReader.Instance.CurrentStoryHandler.GetDecal();

        string str = "";

        bool alreadyKnown = false;

        // pas de decal enregistré
        if (clueIndex < 0)
        {
            List<int> tmpClueIndexes = new List<int>();

            for (int i = 0; i < clues.Length; i++)
            {
                // if the player hasn't found the clue before add it
                if (!FormulaManager.Instance.clueIndexesFound.Contains(i))
                {
                    tmpClueIndexes.Add(i);
                }
            }


            if ( tmpClueIndexes.Count == 0)
            {
                // this means there's no rumors left
                clueIndex = Random.Range(0, clues.Length);

                str = clues[clueIndex];

                alreadyKnown = true;

                // save quand meme, pour pas que ce soit la meme rumeur qui se répète
                StoryReader.Instance.CurrentStoryHandler.SaveDecal(clueIndex);
            }
            else
            {
                // this means the plaer juste discovered a new rumor
                clueIndex = tmpClueIndexes[Random.Range(0, tmpClueIndexes.Count)];

                str = clues[clueIndex];

                StoryReader.Instance.CurrentStoryHandler.SaveDecal(clueIndex);

                FormulaManager.Instance.clueIndexesFound.Add(clueIndex);

                QuestFeedback.Instance.HandleOnNewRumor();
            }

        }
        else
        {
            str = clues[clueIndex];
        }

        // set dialogue
        if ( cellParams.Length > 2)
        {
            if ( cellParams == "Player")
            {
                DialogueManager.Instance.PlayerSpeak_Story(str);
            }
            else
            {
                str = str.Replace("*", " ");

                if ( alreadyKnown)
                {
                    str += ">>. But CAPITAINE clearly already knew about it... He also had the feeling he knew enough about the treasure to just go and follow the leads he pick up here and there...";
                    Narrator.Instance.ShowNarratorInput("<<" + str);
                }
                else
                {
                    Narrator.Instance.ShowNarratorInput("<<" + str + ">>");
                }

            }
        }
        else
        {
            if ( alreadyKnown)
            {
                str += "*But you clearly already know this*And it seems you know everything about the treasure, too...*So you just go find follow the leads people gave you...";
            }

            DialogueManager.Instance.OtherSpeak_Story(str);
        }
    }

    public string GetClue( int i)
    {
        return clues[i];
    }
}
