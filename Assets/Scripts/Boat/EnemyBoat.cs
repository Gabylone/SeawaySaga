using UnityEngine;
using System.Collections;

public class EnemyBoat : Boat {

	public static EnemyBoat Instance;

	private OtherBoatInfo otherBoatInfo;

	public bool followPlayer = false;

    private bool reachedPlayer = false;

	private bool metPlayer = false;

    private bool visible = false;

    private bool exitingScreen = false;

	public float leavingSpeed = 20f;
	public float followPlayer_Speed= 15f;

    public float decalToCenter = 5f;

    public float timeToShowBoat = 1.5f;

    BoxCollider boxCollider;

    public GameObject group;

	void Awake (){
		Instance = this;
	}

	public override void Start ()
	{
		base.Start();

		StoryLauncher.Instance.onPlayStory += HandlePlayStoryEvent;
		StoryLauncher.Instance.onEndStory += HandleEndStoryEvent;

		NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;

		boxCollider = GetComponent<BoxCollider> ();

        Hide();

    }

	void HandleChunkEvent ()
	{
        Hide();
    }

	void HandlePlayStoryEvent ()
	{
		EndMovenent ();
	}

	void HandleEndStoryEvent ()
	{
		if (StoryReader.Instance.CurrentStoryHandler.storyType == StoryType.Boat)
        {
            reachedPlayer = false;
            followPlayer = false;

            ExitScreen();
            SetSpeed(leavingSpeed);
        }
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
                Leave();
            }

		}
        else 
        {
            SetTargetPos(PlayerBoat.Instance.transform.position);
        }
    }

    void Leave()
    {
        otherBoatInfo.MoveToOtherChunk();

        DisplayMinimap.Instance.UpdateOtherBoatsMinimapIcon();

        Hide();
    }

	public void Show ( OtherBoatInfo boatInfo ) 
	{
        this.otherBoatInfo = boatInfo;

        CancelInvoke("ShowDelay");
        Invoke("ShowDelay", timeToShowBoat);

    }

    void ShowDelay()
    {
        exitingScreen = false;
        boxCollider.enabled = true;

        Visible = true;

        UpdatePositionOnScreen();

        if (otherBoatInfo.storyManager.CurrentStoryHandler.Story.param == 0)
        {
            // go about
            ExitScreen();

            SetSpeed(startSpeed);

        }
        else
        {
            SetSpeed(followPlayer_Speed);

            SetTargetPos(PlayerBoat.Instance.getTransform.position);
        }
    }

    void ExitScreen()
    {
        Vector3 corner = NavigationManager.Instance.GetCornerPosition(otherBoatInfo.currentDirection);
        //Debug.Log("target position : " + otherBoatInfo.currentDirection);

        Vector3 p = corner + (corner - Vector3.zero).normalized * decalToCenter;

        SetTargetPos(p);

        exitingScreen = true;
    }

	public void Hide () {
        CancelInvoke("ShowDelay");

        Visible = false;
		OtherBoatInfo = null;
	}

	public override void UpdatePositionOnScreen ()
	{
		base.UpdatePositionOnScreen ();

        Vector3 corner = NavigationManager.Instance.GetOppositeCornerPosition(otherBoatInfo.currentDirection);

        getTransform.position = corner;

		metPlayer = false;
	}

	#region world
	void OnTriggerEnter (Collider other) {

		if (metPlayer == false) {

            if (other.tag == "Player")
            {
                Enter();
            }

		}
        else
        {
            print("met player");
        }
	}
	#endregion

	#region story
	public void Enter () {

        Tween.Bounce(transform);

        // if he met the player once IN THE SCREEN
        metPlayer = true;

        // if he met the player in the WHOLE story
        otherBoatInfo.alreadyMet = true;

		reachedPlayer = true;

        boxCollider.enabled = false;

        SetSpeed(0);

		StoryLauncher.Instance.PlayStory (OtherBoatInfo.storyManager, StoryLauncher.StorySource.boat);
	}

    private void OnMouseDown()
    {
        Tween.Bounce(transform);

        PlayerBoat.Instance.SetTargetPos(transform.position);
    }
    #endregion

    #region properties
    public OtherBoatInfo OtherBoatInfo {
		get {
			return otherBoatInfo;
		}
		set {
			otherBoatInfo = value;
		}
	}

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
