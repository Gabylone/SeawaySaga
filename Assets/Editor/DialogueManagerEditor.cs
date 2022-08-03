using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueManager), true)]
public class DialogueManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DialogueManager dm = (DialogueManager)target;

        if (GUILayout.Button("Load Sounds"))
        {
            //dm.LoadSounds();
        }

        base.OnInspectorGUI();
    }
}
