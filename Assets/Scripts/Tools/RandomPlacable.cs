﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class RandomPlacable : MonoBehaviour
{
    private protected Transform _transform;

    public float chanceAppearing = 15f;

    public float disappearDelay = 0.3f;

    public float minDistanceToIsland = 5f;
    public float minDistanceToPlayerBoat = 5f;

    private bool canTrigger = true;

    private bool locked = false;

    // Start is called before the first frame update
    public virtual void Start()
    {
        _transform = GetComponent<Transform>();

        NavigationManager.Instance.EnterNewChunk += HandleOnEnterNewChunk;

        CombatManager.Instance.onFightStart += Lock;
        CombatManager.Instance.onFightEnd += Unlock;
    }

    void Lock()
    {
        locked = true;
    }

    void Unlock()
    {
        locked = false;
    }

    public virtual void HandleOnEnterNewChunk()
    {
        canTrigger = true;
    }

    public bool CanSpawn()
    {
        if ( NavigationManager.Instance.chunksTravelled < 2)
        {
            return false;
        }

        if (Random.value * 100 > chanceAppearing)
        {
            return false;
        }

        ResetPosition();

        for (int i = 0; i < Chunk.currentChunk.islandDatas.Length; i++)
        {
            if (Vector3.Distance(_transform.position, IslandManager.Instance.islands[i]._transform.position) < minDistanceToIsland)
            {
                return false;
            }
        }

        if (Vector3.Distance(_transform.position, PlayerBoat.Instance.GetTransform.position) < minDistanceToPlayerBoat)
        {
            return false;
        }

        return true;
    }

    void ResetPosition()
    {
        float x = Random.Range(NavigationManager.Instance.minX, NavigationManager.Instance.maxX);
        float y = Random.Range(NavigationManager.Instance.minY, NavigationManager.Instance.maxY);

        _transform.position = new Vector3(x, 0f, y);
    }

    public virtual void OnMouseDown()
    {
        if ( locked )
        {
            return;
        }

        if ( !canTrigger )
        {
            return;
        }

        if (StoryLauncher.Instance.PlayingStory)
        {
            return;
        }

        if (!WorldTouch.Instance.IsEnabled())
        {
            return;
        }

        Tween.Bounce(_transform);

        Flag.Instance.Hide();
        PlayerBoat.Instance.SetTargetPos(_transform.position);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && canTrigger)
        {
            Trigger();
        }
    }

    public virtual void Trigger()
    {
        canTrigger = false;

        Boats.Instance.WithdrawBoats();

        Tween.Bounce(PlayerBoat.Instance.GetTransform);
    }

    public void Disappear()
    {
        _transform.DOScale(0f, disappearDelay).SetEase(Ease.InBounce);

        Invoke("Hide", disappearDelay);
    }
}
