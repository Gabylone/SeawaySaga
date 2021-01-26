using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipManager : MonoBehaviour
{
    [SerializeField]
    private string[] tips;

    // Start is called before the first frame update
    void Start()
    {
        StoryFunctions.Instance.getFunction += HandleGetFunction;

        string path = MapGenerator.mapParameters.mapName + " Tips";
        TextAsset tipsTextAsset = Resources.Load("Tips/" + path) as TextAsset;
        tips = tipsTextAsset.text.Split('\n');
    }

    void HandleGetFunction(FunctionType func, string cellParameters)
    {
        switch (func)
        {
            case FunctionType.GiveTip:
                GiveRandomTip();
                break;
        }
    }

    public void GiveRandomTip()
    {
        DialogueManager.Instance.OtherSpeak_Story(tips[Random.Range(0, tips.Length)]);
    }
}
