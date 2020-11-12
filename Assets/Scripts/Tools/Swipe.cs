using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swipe : MonoBehaviour {

    public static Swipe Instance;

	public delegate void OnSwipe (Directions direction);
	public static OnSwipe onSwipe;

	Vector2 prevPoint;

	public Transform test;

	public float minimumDistance = 0.1f;

	public float minimumTime = 0.5f;

    public bool enableSwipe = true;

	bool swiping = false;

	public float timer = 0f;

    private void Awake()
    {
        Instance = this;

        onSwipe = null;
    }

    // Use this for initialization
    void Start () {
		if (enableSwipe)
        {
            WorldTouch.onPointerDown += HandlePointerDownEvent;
        }
	}

	void HandlePointerDownEvent ()
	{
		Swipe_Start ();
	}
	
	// Update is called once per frame
	void Update () {

        /*if (InputManager.Instance.OnInputDown())
        {
            Swipe_Start();
        }*/
	
		if (swiping)
        {
			Swipe_Update ();
		}

	}

	void Swipe_Start() {

		swiping = true;
		timer = 0f;

		prevPoint = InputManager.Instance.GetInputPosition ();
	}

	void Swipe_Update() {

		float dis = Vector2.Distance ( prevPoint , InputManager.Instance.GetInputPosition() );

		if (dis > minimumDistance && timer < minimumTime) {

			Vector2 dir = (Vector2)InputManager.Instance.GetInputPosition () - prevPoint;
			Directions direction = NavigationManager.Instance.getDirectionFromVector (dir);

            TriggerSwipe(direction);

            Swipe_Exit ();
		}

		timer += Time.deltaTime;

//		prevPoint = InputManager.Instance.GetInputPosition ();

		if ( InputManager.Instance.Touch_Exit() ) {
			Swipe_Exit ();
		}
	}

    public void TriggerSwipe (Directions dir)
    {
        Flag.Instance.Hide();

        WorldTouch.Instance.HandleOnSwipe();

        if (onSwipe != null)
        {
            onSwipe(dir);
        }
    }

	void Swipe_Exit () {
		swiping = false;
	}
}
