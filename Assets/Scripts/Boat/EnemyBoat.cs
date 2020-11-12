using UnityEngine;
using System.Collections;

public class EnemyBoat : Boat
{
    public int id = 0;

    public OtherBoatInfo boatInfo;

    public bool followPlayer = false;

    private bool reachedPlayer = false;

    public float decalToCenter = -2f;

    public float distanceToExitScreen = 0.5f;

    private bool metPlayer = false;

    private bool visible = false;

    public Renderer renderer;

    private bool exitingScreen = false;

    public float distanceFromOtherPlayer = 5f;

    public enum MovementType
    {
        FollowPlayer,
        MoveAround,
    }

    public MovementType movementType;

    public float leavingSpeed = 20f;
    public float followPlayer_Speed = 15f;

    public float timeToShowBoat = 1.5f;

    private BoxCollider boxCollider;

    public GameObject group;

    public override void Start()
    {
        minimapBoat = DisplayMinimap.Instance.CreateMinimapBoat(DisplayMinimap.Instance.enemyBoatIconPrefab, GetTransform, GetBoatInfo());

        base.Start();

        boxCollider = GetComponent<BoxCollider>();

        Hide();

    }

    public override void SetTargetPos(Vector3 p)
    {
        base.SetTargetPos(p);
    }

    public override void Update()
    {
        base.Update();

        if (exitingScreen)
        {
            if (Vector2.Distance(targetPos, GetTransform.position) < distanceToExitScreen)
            {
                ExitToOtherChunk();
            }

        }
        else
        {
            if (moving)
            {
                SetTargetPos(PlayerBoat.Instance.GetTransform.position);
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
    public void Show(OtherBoatInfo boatInfo)
    {
        this.boatInfo = boatInfo;

        exitingScreen = false;
        boxCollider.enabled = true;

        EndMovenent();

        Visible = true;

        GetMinimapBoat.Show(boatInfo);

        UpdatePositionOnScreen();
        UpdateMastColor();

        CancelInvoke("ShowDelay");
        Invoke("ShowDelay", timeToShowBoat * id);
    }

    public void Hide()
    {
        GetMinimapBoat.Hide();

        CancelInvoke("ShowDelay");

        Visible = false;
    }

    void UpdateMastColor()
    {
        renderer.materials[4] = Boats.Instance.boatMaterials[boatInfo.storyManager.CurrentStoryHandler.Story.param];
    }

    void ShowDelay()
    {
        switch (boatInfo.storyManager.CurrentStoryHandler.Story.param)
        {
            case 0:
                movementType = MovementType.FollowPlayer;
                break;
            case 1:
                movementType = MovementType.FollowPlayer;
                break;
            case 2:
                movementType = MovementType.MoveAround;
                break;
            case 3:
                movementType = MovementType.MoveAround;
                break;
            default:
                break;
        }

        if (StoryLauncher.Instance.PlayingStory)
        {

        }
        else
        {
            GoToTargetDestination();
        }

    }
    #endregion

    void GoToTargetDestination()
    {
        if (exitingScreen || movementType == MovementType.MoveAround)
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
        exitingScreen = true;

        Vector3 corner = NavigationManager.Instance.GetCornerPosition(boatInfo.currentDirection);

        Vector3 p = corner + (corner - Vector3.zero).normalized * decalToCenter;

        SetTargetPos(p);

    }

    public override void UpdatePositionOnScreen()
    {
        base.UpdatePositionOnScreen();

        Vector3 corner = NavigationManager.Instance.GetOppositeCornerPosition(boatInfo.currentDirection);

        Vector3 dir = (corner - Vector3.zero).normalized;

        Vector3 p = corner + dir * 25f;

        GetTransform.position = p;

        SetTargetPos(p);

        metPlayer = false;

        GetMinimapBoat.MoveToCoords(boatInfo.coords);
    }

    #region world
    void OnTriggerEnter(Collider other)
    {

        if (other.tag == "Player")
        {
            MeetPlayer();
        }
    }
    #endregion

    #region story
    public void MeetPlayer()
    {

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
        if (Boats.Instance.pausingBoats)
        {
            return;
        }

        Boats.Instance.currentEnemyBoat = this;

        Tween.Bounce(GetTransform);

        Boats.Instance.WithdrawBoats();

        // if he met the player once IN THE SCREEN
        metPlayer = true;

        // if he met the player in the WHOLE story
        boatInfo.alreadyMet = true;

        reachedPlayer = true;

        boxCollider.enabled = false;

        SetSpeed(0);

        SoundManager.Instance.PlaySound("enter port");

        StoryLauncher.Instance.PlayStory(boatInfo.storyManager, StoryLauncher.StorySource.boat);
    }

    public void LeaveBoat()
    {

        reachedPlayer = false;
        followPlayer = false;

        SoundManager.Instance.PlaySound("leave port");

        ExitScreen();
        SetSpeed(leavingSpeed);
    }

    public void Withdraw()
    {
        EndMovenent();

        Vector3 dirFromPlayer = (PlayerBoat.Instance.GetTransform.position - GetTransform.position).normalized;
        Vector3 posAwayFromPos = GetTransform.position + (dirFromPlayer * distanceFromOtherPlayer);

        //SetTargetPos(posAwayFromPos);

    }

    public void Resume()
    {
        GoToTargetDestination();
    }

    private void OnMouseDown()
    {
        Tween.Bounce(GetTransform);

        PlayerBoat.Instance.SetTargetPos(GetTransform.position);

        SoundManager.Instance.PlayRandomSound("button_big");
    }
    #endregion

    #region properties
    public bool Visible
    {
        get
        {
            return visible;
        }
        set
        {

            gameObject.SetActive(value);

            visible = value;

            reachedPlayer = false;

            group.SetActive(value);


        }
    }
    #endregion

    public override BoatInfo GetBoatInfo()
    {
        return boatInfo;
    }

    private void OnDrawGizmosSelected()
    {
        if (exitingScreen)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.blue;
        }

        Gizmos.DrawWireSphere(targetPos, distanceToExitScreen);
    }
}
