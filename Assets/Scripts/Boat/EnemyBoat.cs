using UnityEngine;
using System.Collections;

public class EnemyBoat : Boat
{
    public int id = 0;

    public OtherBoatInfo boatInfo;

    public bool followPlayer = false;

    public float decalToCenter = -2f;

    public float distanceToExitScreen = 0.5f;

    private bool metPlayer = false;

    private bool visible = false;

    public Renderer renderer;

    private bool exitingScreen = false;

    public float distanceFromOtherPlayer = 5f;

    public GameObject[] meshes_Objs;

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

    public GameObject select_feedback;

    public override void Start()
    {
        

        base.Start();

        boxCollider = GetComponent<BoxCollider>();

        Hide();

        WorldTouch.Instance.onSelectSomething += Deselect;

        
    }

    private void OnDestroy()
    {
        WorldTouch.onPointerDown -= Deselect;

    }

    public override MinimapBoat GetMinimapBoat
    {
        get
        {
            if (minimapBoat== null)
            minimapBoat = DisplayMinimap.Instance.CreateMinimapBoat(DisplayMinimap.Instance.enemyBoatIconPrefab, GetTransform, GetBoatInfo());

            return minimapBoat;
        }
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

        if (boatInfo.storyManager.CurrentStoryHandler.Story.id == 3)
        {
            meshes_Objs[0].SetActive(false);
            meshes_Objs[1].SetActive(true);
        }
        else
        {
            meshes_Objs[0].SetActive(true);
            meshes_Objs[1].SetActive(false);
        }

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

        Invoke("UpdatePositionOnScreenDelay", 0.1f);
    }

    void UpdatePositionOnScreenDelay()
    {
        foreach (TrailRenderer renderer in trailRenderers)
        {
            renderer.emitting = true;
        }
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
            //Debug.Log("enemy boat break : story playing");
            return;
        }

        // the boat has already met the player
        /*if (metPlayer)
        {
            //Debug.Log("enemy boat break : already met");
            return;
        }*/

        // the boat is not actually idle
        if (!moving)
        {
            //Debug.Log("enemy boat break : not moving");
            return;
        }
        // another boat is meeting the player
        if (Boats.Instance.pausingBoats)
        {
            //Debug.Log("enemy boat break : pausing boats ( another boat is meeting the player )");
            return;
        }

        Boats.Instance.currentEnemyBoat = this;

        Tween.Bounce(GetTransform);

        Boats.Instance.WithdrawBoats();

        // if he met the player once IN THE SCREEN
        metPlayer = true;

        // if he met the player in the WHOLE story
        boatInfo.alreadyMet = true;

        boxCollider.enabled = false;

        SetSpeed(0);
        EndMovenent();

        SoundManager.Instance.PlaySound("enter port");

        StoryLauncher.Instance.PlayStory(boatInfo.storyManager, StoryLauncher.StorySource.boat);

        Deselect();
    }

    public void LeaveBoat()
    {

        Deselect();

        followPlayer = false;

        SoundManager.Instance.PlaySound("leave port");

        ExitScreen();
        SetSpeed(leavingSpeed);

        boxCollider.enabled = true;

    }

    public void Withdraw()
    {
        if ( movementType == MovementType.MoveAround || exitingScreen)
        {
            //Debug.Log("not withdrawing : moving around / exiting screen");
            return;
        }


        

        Invoke("WithdrawDelay", 0.1f);
    }

    void WithdrawDelay() {

        //Debug.Log("withdrawing boat");

        EndMovenent();
        SetSpeed(0);

        Vector3 dirFromPlayer = (PlayerBoat.Instance.GetTransform.position - GetTransform.position).normalized;

        float x = Random.Range(NavigationManager.Instance.minX, NavigationManager.Instance.maxX);
        float y = Random.Range(NavigationManager.Instance.minY, NavigationManager.Instance.maxY);

        Vector3 posAwayFromPos = new Vector3(x, 0f, y);

        SetTargetPos(posAwayFromPos);
    }

    public override void EndMovenent()
    {
        base.EndMovenent();

    }

    public void Resume()
    {
        //Debug.Log("resuming boat");
        GoToTargetDestination();
    }

    private void OnMouseDown()
    {
        Select();
    }
    public void Select()
    {
        // deselect everything
        WorldTouch.Instance.onSelectSomething();

        Tween.Bounce(GetTransform);

        PlayerBoat.Instance.SetTargetPos(GetTransform.position);

        Flag.Instance.Hide();

        PlayerBoat.Instance.targetingBoat = true;
        PlayerBoat.Instance.boatTarget = GetTransform;

        SoundManager.Instance.PlayRandomSound("button_big");

        select_feedback.SetActive(true);
    }
    public void Deselect()
    {
        PlayerBoat.Instance.targetingBoat = false;
        PlayerBoat.Instance.boatTarget = null;
        select_feedback.SetActive(false);
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
