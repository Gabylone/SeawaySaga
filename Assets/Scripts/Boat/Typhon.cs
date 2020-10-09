using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class Typhon : RandomPlacable
{
    public float delay = 1f;

    public float endDelay = 1f;

    public int damage = 15;

    public override void Start()
    {
        base.Start();
    }

    public override void HandleOnEnterNewChunk()
    {
        base.HandleOnEnterNewChunk();

        CanSpawn();

        transform.localScale = Vector3.one;
    }

    public override void Trigger()
    {
        base.Trigger();

        // sound
        SoundManager.Instance.PlayRandomSound("Sword");
        SoundManager.Instance.PlayRandomSound("Blunt");
        SoundManager.Instance.PlaySound("Fury");

        PlayerBoat.Instance.EndMovenent();
        PlayerBoat.Instance.GetTransform.DOMove(transform.position, delay);
        PlayerBoat.Instance.animator.SetTrigger( "Typhon" );

        Invoke("TriggerDelay", delay);

        WorldTouch.Instance.Lock();
    }

    void TriggerDelay()
    {
        foreach (var crewMember in Crews.playerCrew.CrewMembers)
        {
            crewMember.RemoveHealth(damage);
            crewMember.Icon.hungerIcon.DisplayHealthAmount(damage);

            if (crewMember.Health <= 0)
            {
                crewMember.Health = 2;
            }
        }

        Disappear();

        Invoke("TriggerDelayDelay" , endDelay);
    }

    void TriggerDelayDelay()
    {
        WorldTouch.Instance.Unlock();
    }
}
