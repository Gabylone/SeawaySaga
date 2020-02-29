using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class RandomPlacable : MonoBehaviour
{
    public float minX = 0f;
    public float maxX = 0f;
    public float maxY = 0f;
    public float minY = 0f;

    public float chanceAppearing = 15f;

    public float disappearDelay = 0.3f;

    public float minDistanceToIsland = 5f;
    public float minDistanceToPlayerBoat = 5f;

    private bool canTrigger = true;

    private bool locked = false;

    // Start is called before the first frame update
    public virtual void Start()
    {
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

    public void CheckProximityWithPlayer()
    {
        Hide();

        if (Chunk.currentChunk.IslandData != null)
        {
            return;
        }

        if ( NavigationManager.Instance.chunksTravelled < 2)
        {
            return;
        }

        if (Random.value * 100 < chanceAppearing)
        {
            ResetPosition();

            if (Vector3.Distance(transform.position, Island.Instance.transform.position) < minDistanceToIsland)
            {
                return;
            }

            if (Vector3.Distance(transform.position, PlayerBoat.Instance.transform.position) < minDistanceToPlayerBoat)
            {
                return;
            }

            Show();
        }
    }

    void ResetPosition()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);

        transform.position = new Vector3(x, 0f, y);
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

        Tween.Bounce(transform);

        Flag.Instance.Hide();
        PlayerBoat.Instance.SetTargetPos(transform.position);
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

    void Hide()
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

        Tween.Bounce(PlayerBoat.Instance.getTransform);
    }

    public void Disappear()
    {
        transform.DOScale(0f, disappearDelay).SetEase(Ease.InBounce);

        Invoke("Hide", disappearDelay);
    }
}
