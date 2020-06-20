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

        NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;

        WorldTouch.onPointerDown += HandleOnTouchWorld;

        // decomment this to activate swipe
        //Swipe.onSwipe += HandleOnSwipe;
        Deselect();

    }

	void HandleOnTouchWorld ()
	{
        Deselect();
    }

	void HandleChunkEvent ()
	{
        Coords targetCoords = Boats.Instance.playerBoatInfo.coords + NavigationManager.Instance.getNewCoords(direction);

        if (targetCoords.x < 0 || targetCoords.x > MapGenerator.Instance.MapScale_X - 1 || targetCoords.y < 0 || targetCoords.y > MapGenerator.Instance.MapScale_Y - 1)
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
            Transitions.Instance.ScreenTransition.FadeIn(0.5f);
            Invoke("ChangeChunk", 0.5f);
        }
    }

    void ChangeChunk()
    {
        Transitions.Instance.ScreenTransition.FadeOut(0.5f);
        NavigationManager.Instance.ChangeChunk(direction);
    }
    //
    void Select ()
	{
        CancelInvoke("OutOfMapFeedbackDelay");

        selected = true;

        _boxCollider.enabled = true;

    }

    void Deselect()
    {
        selected = false;

        //_boxCollider.enabled = false;
    }

    void OutOfMapFeedback()
    {
        DialogueManager.Instance.SetDialogueTimed("Nothing to see in this direction", Crews.playerCrew.captain);
        Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.World);

        CancelInvoke("OutOfMapFeedbackDelay");
        Invoke("OutOfMapFeedbackDelay", outOfMapFeedbackDuration);
    }

    void OutOfMapFeedbackDelay()
    {
        Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Map);
    }

}