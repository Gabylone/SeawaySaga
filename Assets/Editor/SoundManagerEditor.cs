using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundManager),true)]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SoundManager soundManager = (SoundManager)target;

        if (GUILayout.Button("Load Sounds"))
        {
            soundManager.LoadSounds();
        }

        base.OnInspectorGUI();
    }

}