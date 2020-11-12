using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NavigationTrigger : MonoBehaviour {

	public static bool anyTargeted = false;

    private BoxCollider _boxCollider;

	public Directions direction;

	private Transform _transform;

    private bool selected = false;

    public float outOfMapFeedbackDuration = 1f;

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _transform = GetComponent<Transform>();

        NavigationManager.Instance.onUpdateCurrentChunk += HandleOnUpdateCurrentChunk;

        WorldTouch.onPointerDown += HandleOnTouchWorld;

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
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

	/*void HandleOnSwipe (Directions direction)
	{
        Deselect();

        Coords targetCoords = Boats.Instance.playerBoatInfo.coords + NavigationManager.Instance.getNewCoords(direction);

        if ( direction == this.direction ) {

            if (targetCoords.x < 0 || targetCoords.x > MapGenerator.Instance.MapScale - 1 || targetCoords.y < 0 || targetCoords.y > MapGenerator.Instance.MapScale - 1)
            {
                OutOfMapFeedback();
                return;
            }

            Vector3 corner = NavigationManager.Instance.GetCornerPosition(direction);
            Vector3 p = corner + (corner - PlayerBoat.Instance.getTransform.position).normalized * 2f;

            PlayerBoat.Instance.SetTargetPos(p);

			Select ();
		}
	}*/

	void OnTriggerStay ( Collider other ) {

		if (other.tag == "Player" && selected ) {
			NavigationManager.Instance.ChangeChunk (direction);
            Deselect();
		}
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SoundManager.Instance.PlaySound("change chunk");
            SoundManager.Instance.PlayRandomSound("ting");
            SoundManager.Instance.PlaySound("Magic Chimes 05");

            Transitions.Instance.ScreenTransition.FadeIn(0.5f);
            Invoke("ChangeChunk", 0.5f);
        }
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

        //_boxCollider.enabled = false;
    }

}