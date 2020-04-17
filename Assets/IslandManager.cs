using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandManager : MonoBehaviour
{
    public static IslandManager Instance;

    public Island[] islands;

    public Island currentIsland;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        islands = GetComponentsInChildren<Island>();

        for (int i = 0; i < islands.Length; i++)
        {
            islands[i].id = i;
        }
    }
}
