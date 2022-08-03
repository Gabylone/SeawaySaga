using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NavigationTrigger : MonoBehaviour {

	public static bool anyTargeted = false;

    private BoxCollider _boxCollider;

	public Directions direction;

	private Transform _transform;

    private bool selected = false;

    public GameObject select_Feedback;

    public float outOfMapFeedbackDuration = 1f;

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _transform = GetComponent<Transform>();

        NavigationManager.Instance.onUpdateCurrentChunk += HandleOnUpdateCurrentChunk;

        WorldTouch.onPointerDown += HandleOnTouchWorld;

        WorldTouch.Instance.onSelectSomething += Deselect;

        // decomment this to activate swipe
        //Swipe.onSwipe += HandleOnSwipe;
        Deselect();

    }

	void HandleOnTouchWorld ()
	{
        Deselect();
    }

	void HandleOnUpdateCurrentChunk ()
	{
        Coords targetCoords = Boats.Instance.playerBoatInfo.coords + NavigationManager.Instance.getNewCoords(direction);

        if (targetCoords.x < 0 || targetCoords.x > MapGenerator.Instance.GetMapHorizontalScale - 1 || targetCoords.y < 0 || targetCoords.y > MapGenerator.Instance.GetMapVerticalScale - 1)
        {
            gameObject.SetActive(false);
            return;
        }

        if ( Chunk.GetChunk(targetCoords).state == ChunkState.DiscoveredVoid
            ||
            Chunk.GetChunk(targetCoords).state == ChunkState.UndiscoveredVoid)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);


    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            CamBehavior.Instance.followPlayer = false;

            SoundManager.Instance.PlaySound("change chunk");
            SoundManager.Instance.PlayRandomSound("ting");
            SoundManager.Instance.PlaySound("Magic Chimes 05");

            Tween.Bounce(_transform);

            Transitions.Instance.ScreenTransition.FadeIn(0.5f);
            Invoke("ChangeChunk", 0.5f);
        }
    }

    private void OnMouseDown()
    {
        if (StoryLauncher.Instance.PlayingStory)
        {
            return;
        }

        if (!WorldTouch.Instance.IsEnabled())
        {
            return;
        }

        WorldTouch.Instance.onSelectSomething();

        Tween.Bounce(_transform);

        Flag.Instance.Hide();
        PlayerBoat.Instance.SetTargetPos(_transform.position);
        select_Feedback.SetActive(true);

        selected = true;
        Debug.Log("mouse down");
    }

    void ChangeChunk()
    {
        SoundManager.Instance.PlayRandomSound("Swipe");

        Transitions.Instance.ScreenTransition.FadeOut(0.5f);
        NavigationManager.Instance.ChangeChunk(direction);
    }

    void Deselect()
    {
        selected = false;

        select_Feedback.SetActive(false);
    }

}