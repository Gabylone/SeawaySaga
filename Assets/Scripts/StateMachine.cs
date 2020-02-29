using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StateMachine : MonoBehaviour {

	// states
	public enum States {
		None,
		State1,
		State2,
		State3,
		State4,
		State5,
		State6,
		State7,
	}
	private States previousState = States.None;
	private States currentState = States.None;

	public delegate void UpdateState();
	UpdateState updateState;

	private float timeInState = 0f;

	// Use this for initialization
	void Start () {
		ChangeState (States.State1);
	}

	// Update is called once per frame
	void Update () {
		if ( updateState != null ) {
			updateState ();
			timeInState += Time.deltaTime;
		}
	}

	#region State1
	private void State1_Start () {
		
	}
	private void State1_Update () {

	}
	private void State1_Exit () {
		//
	}
	#endregion

	#region State2
	private void State2_Start () {

	}
	private void State2_Update () {
		
	}
	private void State2_Exit () {
		//
	}
	#endregion

	#region State3
	private void State3_Start () {

	}
	private void State3_Update () {
		
	}
	private void State3_Exit () {
		//
	}
	#endregion

	#region State4
	private void State4_Start () {

	}
	private void State4_Update () {
		
	}
	private void State4_Exit () {
		//
	}
	#endregion

	#region State5
	private void State5_Start () {

	}
	private void State5_Update () {

	}
	private void State5_Exit () {
		
	}
	#endregion

	#region State6
	private void State6_Start () {
		
	}
	private void State6_Update () {

	}
	private void State6_Exit () {
		//
	}
	#endregion

	#region State7
	private void State7_Start () {

	}
	private void State7_Update () {

	}
	private void State7_Exit () {
		//
	}
	#endregion

	#region StateMachine
	public void ChangeState ( States newState ) {
		previousState = currentState;
		currentState = newState;

		ExitState ();
		EnterState ();

		timeInState = 0f;

	}
	private void EnterState () {
		switch (currentState) {
		case States.State1:
			updateState = State1_Update;
			State1_Start ();
			break;
		case States.State2:
			updateState = State2_Update;
			State2_Start ();
			break;
		case States.State3:
			updateState = State3_Update;
			State3_Start ();
			break;
		case States.State4:
			updateState = State4_Update;
			State4_Start ();
			break;
		case States.State5:
			updateState = State5_Update;
			State5_Start ();
			break;
		case States.State6:
			updateState = State6_Update;
			State6_Start ();
			break;
		case States.State7 :
			updateState = State7_Update;
			State7_Start ();
			break;
		}
	}
	private void ExitState () {
		switch (previousState) {
		case States.State1:
			State1_Exit ();
			break;
		case States.State2:
			State2_Exit ();
			break;
		case States.State3:
			State2_Exit ();
			break;
		case States.State4:
			State4_Exit ();
			break;
		case States.State5:
			State5_Exit ();
			break;
		case States.State6:
			State6_Exit ();
			break;
		case States.State7 :
			State7_Exit ();
			break;
		}
	}
	#endregion

	#region properties
	public States CurrentState {
		get {
			return currentState;
		}
		set {
			currentState = value;
		}
	}
	#endregion

}
