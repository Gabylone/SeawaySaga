using UnityEngine;
using System.Collections;

public class EnemyBoat : Boat {

    public int id = 0;

	public OtherBoatInfo boatInfo;

	public bool followPlayer = false;

    private bool reachedPlayer = false;

	private bool metPlayer = false;

    private bool visible = false;

    private bool exitingScreen = false;

    public enum MovementType
    {
        FollowPlayer,
        MoveAround,
    }

    public MovementType movementType;

	public float leavingSpeed = 20f;
	public float followPlayer_Speed= 15f;

    public float decalToCenter = 5f;

    public float timeToShowBoat = 1.5f;

    BoxCollider boxCollider;

    public GameObject group;

	public override void Start ()
	{
		base.Start();

        Boats.Instance.onMeetPlayer += HandleOnMeetPlayer;
        Boats.Instance.onLeavePlayer += HandleOnLeavePlayer;

		boxCollider = GetComponent<BoxCollider> ();

        Hide();

    }

    public override void SetTargetPos(Vector3 p)
    {
        base.SetTargetPos(p);
    }

    public override void Update ()
	{
		base.Update ();

		if  (exitingScreen ) {

            if (Vector2.Distance(targetPos, transform.position) < 0.5f)
            {
                ExitToOtherChunk();
            }

		}
        else 
        {
            if( moving)
            {
                SetTargetPos(PlayerBoat.Instance.transform.position);
            }
        }
    }

    void ExitToOtherChunk()
    {
        boatInfo.MoveToOtherChunk();

        DisplayMinimap.Instance.UpdateOtherBoatsMinimapIcon();

        Hide();
    }

    #region boat appears
    public void Show ( OtherBoatInfo boatInfo ) 
	{
        this.boatInfo = boatInfo;

        exitingScreen = false;
        boxCollider.enabled = true;

        EndMovenent();

        Visible = true;

        UpdatePositionOnScreen();

        CancelInvoke("ShowDelay");
        Invoke("ShowDelay", timeToShowBoat * id);
    }

    void ShowDelay()
    {
        if (boatInfo.storyManager.CurrentStoryHandler.Story.param == 0)
        {
            movementType = MovementType.MoveAround;
        }
        else
        {
            movementType = MovementType.FollowPlayer;
        }

        GoToTargetDestination();
    }
    #endregion

    void GoToTargetDestination()
    {
        if (movementType == MovementType.MoveAround)
        {
            // go about
            ExitScreen();

            SetSpeed(startSpeed);

        }
        else
        {
            SetSpeed(followPlayer_Speed);

            SetTargetPos(PlayerBoat.Instance.GetTransform.position);
        }
    }

    void ExitScreen()
    {
        Vector3 corner = NavigationManager.Instance.GetCornerPosition(boatInfo.currentDirection);
        //Debug.Log("target position : " + otherBoatInfo.currentDirection);

        Vector3 p = corner + (corner - Vector3.zero).normalized * decalToCenter;

        SetTargetPos(p);

        exitingScreen = true;
    }

	public void Hide () {

        CancelInvoke("ShowDelay");

        Visible = false;
	}

	public override void UpdatePositionOnScreen ()
	{
		base.UpdatePositionOnScreen ();

        Vector3 corner = NavigationManager.Instance.GetOppositeCornerPosition(boatInfo.currentDirection);

        Vector3 dir = (corner - Vector3.zero).normalized;

        Vector3 p = corner + dir * 25f;

        GetTransform.position = p;
        SetTargetPos(p);

		metPlayer = false;
	}

	#region world
	void OnTriggerEnter (Collider other) {

        if (other.tag == "Player")
        {
            MeetPlayer();
        }
	}
	#endregion

	#region story
	public void MeetPlayer () {

        // a story is currently going
        if (StoryLauncher.Instance.PlayingStory)
        {
            return;
        }

        // the boat has already met the player
        if (metPlayer)
        {
            return;
        }
        // the boat is not actually idle
        if (!moving)
        {
            return;
        }
        // another boat is meeting the player
        if (Boats.Instance.meetingPlayer)
        {
            return;
        }

        Boats.Instance.currentEnemyBoat = this;

        Tween.Bounce(transform);

        Boats.Instance.MeetPlayer();

        // if he met the player once IN THE SCREEN
        metPlayer = true;

        // if he met the player in the WHOLE story
        boatInfo.alreadyMet = true;

		reachedPlayer = true;

        boxCollider.enabled = false;

        SetSpeed(0);

        SoundManager.Instance.PlayLoop("enter port");

		StoryLauncher.Instance.PlayStory (boatInfo.storyManager, StoryLauncher.StorySource.boat);
	}

    public void LeavePlayer()
    {
        Boats.Instance.LeaveOtherBoat();

        reachedPlayer = false;
        followPlayer = false;

        SoundManager.Instance.PlayLoop("leave port");

        ExitScreen();
        SetSpeed(leavingSpeed);
    }

    void HandleOnMeetPlayer()
    {
        EndMovenent();
    }

    void HandleOnLeavePlayer()
    {
        GoToTargetDestination();
    }

    private void OnMouseDown()
    {
        Tween.Bounce(transform);

        PlayerBoat.Instance.SetTargetPos(transform.position);

        SoundManager.Instance.PlayRandomSound("button_big");
    }
    #endregion

    #region properties

	public bool Visible {
		get {
			return visible;
		}
		set {

            gameObject.SetActive(value);

            visible = value;

			reachedPlayer = false;

            group.SetActive(value);


        }
	}
	#endregion
}
