using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandArrow : MonoBehaviour
{
    Transform tr;

    private void Start()
    {
        tr = transform;
    }

    // Update is called once per frame
    void Update()
    {
        tr.LookAt(CamBehavior.Instance._transform.position, -Vector3.up);

    }
}
