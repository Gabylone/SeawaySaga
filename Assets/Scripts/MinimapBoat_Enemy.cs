using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapBoat_Enemy : MinimapBoat
{
    public EnemyBoat EnemyBoat;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public void OnPointerDown()
    {
        return;

        Tween.Bounce(local_RectTransform);

        IslandInfo.Instance.DisplayBoatInfo(EnemyBoat.boatInfo);
        IslandInfo.Instance.ShowAtTransform(local_RectTransform);
    }
}
