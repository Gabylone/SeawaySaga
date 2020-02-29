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

	#region get touch & click
	/// <summary>
	/// Raises the input down event.
	/// </summary>
	public bool OnInputDown () {
		return OnInputDown (0,ScreenPart.Any);
	}
	public bool OnInputDown (int id, ScreenPart screenPart) {

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
	public bool OnInputStay () {
		return OnInputStay (0,ScreenPart.Any);
	}
	public bool OnInputStay (int id, ScreenPart screenPart) {

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
	public bool OnInputExit () {
		return OnInputExit (0,ScreenPart.Any);
	}
	public bool OnInputExit (int id, ScreenPart screenPart) {

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