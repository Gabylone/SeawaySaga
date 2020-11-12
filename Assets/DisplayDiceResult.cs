using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDiceResult : Displayable
{
    public static DisplayDiceResult Instance;

    public Text uiText;

    private void Awake()
    {
        Instance = this;
    }

    public void Display(string txt)
    {
        Show();

        uiText.text = txt;

        InputManager.Instance.onPointerDown += HandleOnPointerDown;
    }

    void HandleOnPointerDown()
    {
        InputManager.Instance.onPointerDown -= HandleOnPointerDown;

        Hide();

        DiceManager.Instance.EndThrow();
    }
}
