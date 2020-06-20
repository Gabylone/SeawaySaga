using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapParameters
{
    public int id = 0;

    public string mapName = "";

    public int boatAmount = 100;

    public int endFightLevel = 10;

    public int numberOfCluesBeforeTreasure = 3;

    public int islandPerCol = 0;

    public int mapScale_X = 100;
    public int mapScale_Y = 100;
}
