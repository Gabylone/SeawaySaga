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

    private bool canSpawn = false;

    private bool locked = false;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    public virtual void Start()
    {
        _transform = GetComponent<Transform>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        NavigationManager.Instance.onUpdateCurrentChunk += HandleOnUpdateCurrentChunk;

        CombatManager.Instance.onFightStart += Lock;
        CombatManager.Instance.onFightEnd += Unlock;

        Invoke("StartDelay", 2f);
    }

    void StartDelay()
    {
        canSpawn = true;
    }

    void Lock()
    {
        locked = true;
    }

    void Unlock()
    {
        locked = false;
    }

    public virtual void HandleOnUpdateCurrentChunk()
    {
        canTrigger = true;
    }

    public bool CanSpawn()
    {
        if (!canSpawn)
        {
            return false;
        }

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
        spriteRenderer.color = Color.clear;
        spriteRenderer.DOColor(Color.white, 0.2f);
        
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
        spriteRenderer.DOFade(0f, disappearDelay);

        //_transform.DOScale(0f, disappearDelay).SetEase(Ease.InBounce);

        Invoke("Hide", disappearDelay);
    }
}
