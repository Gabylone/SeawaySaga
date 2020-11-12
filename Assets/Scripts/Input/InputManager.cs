using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	public static InputManager Instance;

	public enum ScreenPart {
		Any,
		Left,
		Right
	}

	void Awake () {
		Instance = this;
	}

    public delegate void OnPointerDown();
    public OnPointerDown onPointerDown;

    private void Update()
    {
        if ( Touch_Down ())
        {
            if (onPointerDown != null)
            {
                onPointerDown();
            }
        }
    }

    #region get touch & click
    /// <summary>
    /// Raises the input down event.
    /// </summary>
    public bool Touch_Down () {
		return Touching_Down (0,ScreenPart.Any);
	}
	public bool Touching_Down (int id, ScreenPart screenPart) {

		bool rightSideOfScreen = GetInputPosition ().x > 0;
		if (screenPart == ScreenPart.Left)
			rightSideOfScreen = GetInputPosition ().x <= Screen.width / 2;
		if (screenPart == ScreenPart.Right)
			rightSideOfScreen = GetInputPosition ().x > Screen.width / 2;

		if (OnMobile) {
			
			if (Input.touches.Length <= 0)
				return false;
			
			return Input.GetTouch (id).phase == TouchPhase.Began && rightSideOfScreen;
		}
		else
			return Input.GetMouseButtonDown (id) && rightSideOfScreen;
	}

	/// <summary>
	/// Raises the input stay event.
	/// </summary>
	public bool Touch_Stay () {
		return Touch_Stay (0,ScreenPart.Any);
	}
	public bool Touch_Stay (int id, ScreenPart screenPart) {

		bool rightSideOfScreen = GetInputPosition ().x > 0;
		if (screenPart == ScreenPart.Left)
			rightSideOfScreen = GetInputPosition ().x <= Screen.width / 2;
		if (screenPart == ScreenPart.Right)
			rightSideOfScreen = GetInputPosition ().x > Screen.width / 2;

		if (OnMobile) {
			if (Input.touches.Length <= 0)
				return false;

			return (Input.GetTouch (id).phase == TouchPhase.Stationary || Input.GetTouch (id).phase == TouchPhase.Moved) && rightSideOfScreen;
		} else
			return Input.GetMouseButton (id) && rightSideOfScreen;
	}

	/// <summary>
	/// Raises the input exit event.
	/// </summary>
	public bool Touch_Exit () {
		return Touch_Exit (0,ScreenPart.Any);
	}
	public bool Touch_Exit (int id, ScreenPart screenPart) {

		bool rightSideOfScreen = GetInputPosition ().x > 0;
		if (screenPart == ScreenPart.Left)
			rightSideOfScreen = GetInputPosition ().x <= Screen.width / 2;
		if (screenPart == ScreenPart.Right)
			rightSideOfScreen = GetInputPosition ().x > Screen.width / 2;

		if (OnMobile) {
			
			if (Input.touches.Length <= 0)
				return false;

			return (Input.GetTouch (id).phase == TouchPhase.Ended) && rightSideOfScreen;
		}
		else
			return Input.GetMouseButtonUp (id) && rightSideOfScreen;
	}

	/// <summary>
	/// Gets the input position.
	/// </summary>
	/// <returns>The input position.</returns>
	public Vector2 GetInputPosition () {
		return GetInputPosition (0);
	}
	public Vector2 GetInputPosition (int id) {
		if (OnMobile) {
			if ( Input.touches.Length <=0 ) {
				return Vector3.zero;
			}
			return Input.GetTouch (id).position;
		} else {
			return Input.mousePosition;
		}
	}
	#endregion

	public bool OnMobile {
		get {
			return Application.isMobilePlatform;
		}
	}
}