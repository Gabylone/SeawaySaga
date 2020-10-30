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

        Invoke("HandleOnEnterNewChunkDelay", 0.1f);
    }

    void HandleOnEnterNewChunkDelay()
    {
        Hide();

        if (CanSpawn())
        {
            _transform.localScale = Vector3.one;

            Show();
        }
    }

    public override void Trigger()
    {
        base.Trigger();

        // sound
        SoundManager.Instance.PlayRandomSound("Sword");
        SoundManager.Instance.PlayRandomSound("Blunt");
        SoundManager.Instance.PlaySound("Fury");

        PlayerBoat.Instance.EndMovenent();
        PlayerBoat.Instance.GetTransform.DOMove(_transform.position, delay);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistanceToIsland);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, minDistanceToPlayerBoat);

    }
}
