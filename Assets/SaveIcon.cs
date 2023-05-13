using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveIcon : Displayable
{
    public float duration = 2f;

    public override void Start()
    {
        base.Start();
     
        SaveManager.Instance.onSaveGameData += HandleOnSaveGameData;

        HideDelay();
    }

    private void HandleOnSaveGameData()
    {
        Show();

        CancelInvoke("Delay");
        Invoke("Delay", duration);
    }

    void Delay()
    {
        Hide();
    }
}
