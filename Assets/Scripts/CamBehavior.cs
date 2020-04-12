using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CamBehavior : MonoBehaviour
{
    public static CamBehavior Instance;

    public Vector3 decal_Zoomed;
    public Vector3 decal_Init;

    public float distanceToIsland = 10f;

    public float zoomDuration = 1f;

    public float speed_Zoomed = 1f;
    public float speed_Init = 1f;

    private Transform _transform;

    bool zoomed = false;

    private Vector3 initPos;
    private Vector3 initRot;

    public float targetDistance = 2f;

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        _transform = transform;

        initPos = _transform.position;
        initRot = _transform.forward;

        StoryLauncher.Instance.onEndStory += UnZoom;
    }

    private void Update()
    {
        if ( zoomed)
        {
            Vector3 targetPos = Island.Instance.transform.position;

            if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.boat)
            {
                targetPos = Boats.Instance.currentEnemyBoat.GetTransform.position;
            }

            Vector3 p = targetPos + decal_Zoomed;

            _transform.position = Vector3.Lerp(_transform.position, p, speed_Zoomed * Time.deltaTime);
            _transform.forward = Vector3.Lerp(_transform.forward, (targetPos - p).normalized, speed_Zoomed * Time.deltaTime);
        }
        else
        {
            Vector3 p =  PlayerBoat.Instance.transform.position + (Flag.Instance.transform.position - PlayerBoat.Instance.transform.position).normalized * targetDistance;
            Vector3 targetPos = p + decal_Init;

            if (!PlayerBoat.Instance.moving)
            {
                targetPos = PlayerBoat.Instance.transform.position + decal_Init;
            }

            _transform.position = Vector3.Lerp(_transform.position, targetPos, speed_Init * Time.deltaTime);
            _transform.forward = Vector3.Lerp(_transform.forward, initRot, speed_Init * Time.deltaTime);
        }
    }

    public void RefreshCamOnPlayer()
    {
        Flag.Instance.SetPos(PlayerBoat.Instance.transform.position);

         Vector3 targetPos = PlayerBoat.Instance.transform.position + decal_Init;
        _transform.position = targetPos;

    }

    public void Zoom()
    {
        //Vector3 dirFromIsland = Island.Instance.transform.position - decal;

        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.other)
            return;

        zoomed = true;
    }

    public void UnZoom()
    {
        if (StoryLauncher.Instance.CurrentStorySource == StoryLauncher.StorySource.other)
            return;

        zoomed = false;
    }
}
