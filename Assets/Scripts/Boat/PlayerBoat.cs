using UnityEngine;
using System.Collections;
using System;

public class PlayerBoat : Boat {

	public static PlayerBoat Instance;

    public LayerMask layerMask;

    void Awake()
    {
        Instance = this;
    }

    public override void Start()
    {
        base.Start();


        WorldTouch.onPointerExit += HandleOnPointerExit;

        NavigationManager.Instance.onUpdateCurrentChunk += HandleOnUpdateChunk;

        Invoke("StartDelay", 0.001f);

    }

    void StartDelay()
    {
        minimapBoat = DisplayMinimap.Instance.CreateMinimapBoat(DisplayMinimap.Instance.playerBoatIconPrefab, GetTransform, GetBoatInfo());

        DisplayMinimap.Instance.CenterOnBoat_Quick();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void SetTargetPos(Vector3 p)
    {
        if (!moving)
        {
            SoundManager.Instance.PlaySound("click_med 05");
        }

        base.SetTargetPos(p);

        Flag.Instance.SetPos(p);
        
    }

    public void HandleOnUpdateChunk ()
	{
        UpdatePositionOnScreen();

        GetMinimapBoat.TweenToCoords(GetBoatInfo().coords);

        CamBehavior.Instance.RefreshCamOnPlayer();
	}

    #region events
    private void HandleOnPointerExit()
    {
        Tween.Bounce(GetTransform);
    }
    #endregion

    public override void EndMovenent ()
	{
		base.EndMovenent ();

        if (moving)
        {
            SoundManager.Instance.PlaySound("click_light 03");
        }

        Tween.Bounce(GetTransform);

        WorldTouch.Instance.touching = false;

        //SetTargetPos(GetTransform.position);

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
        }

        moving = false;

        Flag.Instance.HandleOnEndMovement();
	}

    public override void UpdatePositionOnScreen()
    {
        base.UpdatePositionOnScreen();

        GetTransform.position = NavigationManager.Instance.GetOppositeCornerPosition(GetBoatInfo().currentDirection);

        SetTargetPos( GetTransform.position );
    }

    void OnTriggerEnter (Collider collider) {

		if (collider.tag == "Flag" && !WorldTouch.Instance.touching)
        {
			EndMovenent ();
		}
	}

    public override BoatInfo GetBoatInfo()
    {
        return Boats.Instance.playerBoatInfo;
    }
}
