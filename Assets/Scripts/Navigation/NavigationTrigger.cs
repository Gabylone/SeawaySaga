using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NavigationTrigger : MonoBehaviour {

	public static bool anyTargeted = false;

	public GameObject arrowGroup;
    public Image arrowImage;

    BoxCollider _boxCollider;

	public Directions direction;

	Transform _transform;

    bool selected = false;

    public float outOfMapFeedbackDuration = 1f;

    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _transform = GetComponent<Transform>();
        arrowImage = arrowGroup.GetComponentInChildren<Image>();

        NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;
        Swipe.onSwipe += HandleOnSwipe;

        WorldTouch.onPointerDown += HandleOnTouchWorld;

        StoryLauncher.Instance.onPlayStory += HandlePlayStoryEvent;

        Deselect();

    }

	void HandlePlayStoryEvent ()
	{
        Deselect();
    }

	void HandleOnTouchWorld ()
	{
        Deselect();
    }

	void HandleChunkEvent ()
	{
        Deselect();
    }

	void HandleOnSwipe (Directions direction)
	{
        Deselect();

        Coords targetCoords = Boats.playerBoatInfo.coords + NavigationManager.Instance.getNewCoords(direction);

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
	}

	void OnTriggerStay ( Collider other ) {
		if (other.tag == "Player" && selected ) {
			NavigationManager.Instance.ChangeChunk (direction);
            Deselect();
		}
	}
//
	void Select ()
	{
        CancelInvoke("OutOfMapFeedbackDelay");

        selected = true;

        arrowGroup.SetActive(true);

        arrowImage.color = Color.white;

        Tween.Bounce(arrowGroup.transform);

        _boxCollider.enabled = true;

    }

    void Deselect()
    {
        selected = false;

        arrowGroup.SetActive(false);

        _boxCollider.enabled = false;
    }

    void OutOfMapFeedback()
    {
        DialogueManager.Instance.SetDialogueTimed("Il n'y a rien par là", Crews.playerCrew.captain);
        Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Discussion);

        arrowGroup.SetActive(true);

        arrowImage.color = Color.red;

        Tween.Bounce(arrowGroup.transform);

        CancelInvoke("OutOfMapFeedbackDelay");
        Invoke("OutOfMapFeedbackDelay", outOfMapFeedbackDuration);
    }

    void OutOfMapFeedbackDelay()
    {
        Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Map);

        arrowGroup.SetActive(false);
    }

}