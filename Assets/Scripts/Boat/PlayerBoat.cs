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

    public override void Start ()
	{
		base.Start();

		WorldTouch.onPointerExit += HandleOnPointerExit;

		StoryLauncher.Instance.onPlayStory += EndMovenent;
		StoryLauncher.Instance.onEndStory += EndMovenent;

		NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;

		NavigationManager.Instance.EnterNewChunk += UpdatePositionOnScreen;

	}

    public override void Update()
    {
        base.Update();
    }

    void HandleChunkEvent ()
	{
        //SetTargetPos(NavigationManager.Instance.GetAnchor(Directions.None));
	}

	void HandleOnTouchIsland ()
	{
		SetTargetPos (Island.Instance.transform.position);
	}

    #region events
    private void HandleOnPointerExit()
    {
        Tween.Bounce(getTransform);
    }
    #endregion

    public override void EndMovenent ()
	{
		base.EndMovenent ();

        Tween.Bounce(transform);

        WorldTouch.Instance.touching = false;

        SetTargetPos(transform.position);

        agent.isStopped = true;

        moving = false;

        Flag.Instance.HandleOnEndMovement();
	}

	public override void UpdatePositionOnScreen ()
	{
		base.UpdatePositionOnScreen ();

        getTransform.position = NavigationManager.Instance.GetOppositeCornerPosition(Boats.playerBoatInfo.currentDirection);

	}

	void OnTriggerEnter (Collider collider) {

		if (collider.tag == "Flag" && !WorldTouch.Instance.touching)
        {
			EndMovenent ();
		}
	}
}
