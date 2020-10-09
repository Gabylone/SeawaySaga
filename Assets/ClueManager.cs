using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueManager : MonoBehaviour
{
    [SerializeField]
    private string[] clues;

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
            case FunctionType.GiveClue:
                GiveClue(cellParameters);
                break; 
        }
    }


    public void GiveClue(string cellParams)
    {
        // choper un decal
        int clueIndex = StoryReader.Instance.CurrentStoryHandler.GetDecal();

        // pas de decal enregistré
        if (clueIndex < 0)
        {
            clueIndex = Random.Range(0, clues.Length);

            StoryReader.Instance.CurrentStoryHandler.SaveDecal(clueIndex);
        }

        // set dialogue
        if ( cellParams.Length > 2)
        {
            DialogueManager.Instance.SetDialogue(clues[clueIndex], Crews.playerCrew.captain);
        }
        else
        {
            DialogueManager.Instance.SetDialogue(clues[clueIndex], Crews.enemyCrew.captain);
        }

        StoryInput.Instance.WaitForInput();
    }
}
