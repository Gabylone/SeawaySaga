using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IslandManager : MonoBehaviour
{
    public static IslandManager Instance;

    public Island[] islands;

    public Sprite[] icons;

    public Island currentIsland;

    public NavMeshObstacle NavMeshObstacle;

    public float minDistanceBetweenIslands = 9f;

    // NE CORRIGE PAS TU ASSUME LE TYPO SINON TU VAS NIQUER L INSPECTEUR
    public Vector3[] possiblePosisitions;
    // STP NE CORRIGE PAS TU AS FAIS UNE FAUTE TU AURAIS DU LECRIRE BIEN MAINTENANT LAISSE LINSPECTEUR TRANQUILE

    public List<Vector3> tmpPossiblePositions = new List<Vector3>();

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

    public IslandData GetCurrentIslandData()
    {
        int currentIndex = IslandManager.Instance.currentIsland.id;
        IslandData currentIslandData = Chunk.currentChunk.GetIslandData(currentIndex);

        return currentIslandData;
    }

    public IslandData GetRandomIslandDataForQuest()
    {
        List<IslandData> potentialIslands = new List<IslandData>();

        foreach (var chunk in Chunk.chunks.Values)
        {
            if ( !chunk.HasIslands())
            {
                continue;
            }

            foreach (var islandData in chunk.islandDatas)
            {
                // the island is the same as the current
                // PEUT BEUGUER PARCE que serialization bizarre
                if (islandData == GetCurrentIslandData())
                {
                    //Debug.Log("QUEST : Island is current island");
                    continue;
                }

                Quest islandQuest = QuestManager.Instance.currentQuests.Find(x => x.GetTargetIslandData() != null && chunk.coords == x.GetTargetIslandData().coords);

                // the island has already a quest
                if (islandQuest != null)
                {
                    //Debug.Log("QUEST : Island already has a quest");
                    continue;
                }

                // adding island to potential quest islands
                potentialIslands.Add(islandData);
            }
        }

        // no potential island, returning house ( et on verra )
        if (potentialIslands.Count == 0)
        {
            Debug.LogError("no potential island for quest : returning current");
            return GetCurrentIslandData();
        }

        return potentialIslands[Random.Range(0, potentialIslands.Count)];
    }

    #region island positions
    public void ResetIslandPositions()
    {
        tmpPossiblePositions.Clear();

        foreach (var item in possiblePosisitions)
        {
            tmpPossiblePositions.Add(item);
        }
    }
    public Vector2 GetNewIslandPosition()
    {
        int index = Random.Range(0, tmpPossiblePositions.Count);

        Vector3 v = tmpPossiblePositions[index];

        tmpPossiblePositions.RemoveAt(index);

        return new Vector2(v.x, v.z);

    }
    #endregion

    public Sprite GetIcon(int i)
    {
        return icons[i];
    }
}
