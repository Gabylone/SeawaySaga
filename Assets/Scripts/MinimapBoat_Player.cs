using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBoat_Player : MinimapBoat
{
    public static MinimapBoat_Player Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();

        Vector3 dir = targetBoatTransform.forward;
        dir = new Vector3(dir.x, dir.z, 0f);

        local_RectTransform.up = dir;
    }
}
