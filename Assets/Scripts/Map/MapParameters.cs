using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapParameters
{
    public int id = 0;

    public string mapName = "";

    public int numberOfNumberBoats = 100;

    public int endFightLevel = 10;

    public int numberOfCluesBeforeTreasure = 3;

    public int islandPerCol = 0;

    private int mapScale = 100;

    public int GetScale()
    {
        return mapScale;
    }

    public void SetScale ( int value)
    {
        mapScale = value;
    }
}
