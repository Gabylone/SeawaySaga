using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IslandTrigger : MonoBehaviour {

    public Island island;

    public Collider collider;

    public NavMeshObstacle navMeshObstacle;

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player")
        {
            Island.Instance.CollideWithPlayer();
        }
    }

    public void DeactivateCollider()
    {
        collider.enabled = false;

        navMeshObstacle.enabled = true;
    }

    public void ActivateCollider()
    {
        collider.enabled = true;

        navMeshObstacle.enabled = false;

    }
}
