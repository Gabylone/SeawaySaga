using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Pin
{
    public enum ColorType
    {
        Red,
        Blue,
        Green,
        Yellow,

        None,
    }

    public ColorType colorType;

    public string content;
    public float save_X = 0;
    public float save_Y = 0;
}
