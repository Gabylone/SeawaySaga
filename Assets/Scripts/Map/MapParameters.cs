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

    public int islandPerCol = 0;

    private int mapScale_X = 100;
    private int mapScale_Y = 100;

    public int MapScale_X
    {
        get
        {
            return mapScale_X;
        }
    }

    public int MapScale_Y
    {
        get
        {
            return mapScale_Y;
        }
    }

    public void SetMapScale_X (int x)
    {
        mapScale_X = x;
    }

    public void SetMapScale_Y (int y)
    {
        mapScale_Y = y;
    }
}
