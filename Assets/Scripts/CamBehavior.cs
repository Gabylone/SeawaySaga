using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CamBehavior : MonoBehaviour
{
    public Vector3 decal;

    public float distanceToIsland = 10f;

    public float zoomDuration = 1f;

    public float speed = 1f;

    Transform _transform;


    bool zoomed = false;

    Vector3 initPos;
    Vector3 initRot;

    // Use this for initialization
    void Start()
    {
        _transform = transform;

        initPos = _transform.position;
        initRot = _transform.forward;

        StoryLauncher.Instance.onPlayStory += Zoom;
        StoryLauncher.Instance.onEndStory += UnZoom;
    }

    private void Update()
    {
        if ( zoomed)
        {
            Vector3 targetPos = Island.Instance.transform.position;

            if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.boat)
            {
                targetPos = EnemyBoat.Instance.getTransform.position;
            }

            Vector3 p = targetPos + decal;

            _transform.position = Vector3.Lerp(_transform.position, p, speed * Time.deltaTime);
            _transform.forward = Vector3.Lerp(_transform.forward, (targetPos - p).normalized, speed * Time.deltaTime);
        }
        else
        {
            _transform.position = Vector3.Lerp(_transform.position, initPos, speed * Time.deltaTime);
            _transform.forward = Vector3.Lerp(_transform.forward, initRot, speed * Time.deltaTime);
        }
    }

    void Zoom()
    {
        //Vector3 dirFromIsland = Island.Instance.transform.position - decal;

        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.other)
            return;

        zoomed = true;
    }

    void UnZoom()
    {
        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.other)
            return;

        zoomed = false;
    }
}
