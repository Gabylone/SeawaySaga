using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum DiceTypes {
	STR,
	DEX,
	CHA,
	CON,
}

public class DiceManager : MonoBehaviour {
		
	// singleton
	public static DiceManager Instance;

		// STATES
	public enum states {

		none,

		throwing,
		settling,
		showingHighest,
	}
	private states previousState;
	private states currentState;

	float timeInState = 0f;

    public float timeBetweenSettles = 0.3f;

	private delegate void UpdateState ();
	UpdateState updateState;

	public int outcome = -1;

		// STATES

	[Header("Dice")]
	[SerializeField]
	public float settlingDuration = 0.5f;

	[SerializeField]
	private Color[] diceColors;

	public float throwDuration;
	private int throwDirection = 1;

	[SerializeField]
	private Dice[] dices;

	private Throw currentThrow;

	private int highestResult = 0;

	private bool throwing = false;
	private float timer = 0f;

	void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		
		InitDice ();
		ResetDice ();


		StoryFunctions.Instance.getFunction += HandleGetFunction;
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.CheckStat:
			CheckStat ();
			break;
		case FunctionType.ThrowDice:
			ThrowDiceAmount ();
			break;
		}
	}
	
	// Update is called once per frame
	void Update () {

		if ( updateState != null ) {
			updateState ();
			timeInState += Time.deltaTime;

		}

        /*if (Input.GetKeyDown(KeyCode.D))
        {
            ThrowDice(DiceTypes.CHA, 6);
        }*/

//		print ("QUICK THROW RESULT : " + QuickThrow (1));
	}

	#region init
	private void InitDice () {
		foreach ( Dice die in dices) {
			die.Init ();
		}
	}
	private void ResetDice () {
		foreach ( Dice die in dices) {
			die.Reset ();
		}
	}
	#endregion

	#region throwing
	public int QuickThrow (int diceAmount) {

		int result = 0;

		int[] quickDices = new int[diceAmount];
		for (int i = 0; i < diceAmount; i++) {
			if (outcome < 1) {
				quickDices [i] = Random.Range (1, 7);
			} else {
				quickDices [i] = outcome;
				//
			}
		}

		for (int diceIndex = 0; diceIndex < diceAmount; diceIndex++) {
			if (quickDices[diceIndex] > result) {
				result = quickDices [diceIndex];
			}
		}

		return result;
	}



	public void ThrowDice (DiceTypes type, int diceAmount) {

        Transitions.Instance.actionTransition.FadeIn(0.5f);

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Dice Multiple");
        SoundManager.Instance.PlayRandomSound("Dice Multiple");

        SoundManager.Instance.PlayLoop("dice_wait");

		ResetDice ();

		Throwing = true;

		currentThrow = new Throw (diceAmount, type);

		ChangeState (states.throwing);

	}
	private void Throwing_Start () {
		
		PaintDice (currentThrow.diceType);
		
		for (int i = 0; i < currentThrow.diceAmount ; ++i) {
			dices[i].Throw ();
		}
	}
	private void Throwing_Update () {

//		if ( InputManager.Instance.OnInputDown() ) {
//			ChangeState (states.showingHighest);
//			return;
//		}

		if ( timeInState > settlingDuration) {
			ChangeState (states.settling);
		}
	}
	private void Throwing_Exit () {

        SoundManager.Instance.StopLoop("dice_wait");


    }
    #endregion

    #region settling
    private void Settling_Start () {

        SoundManager.Instance.PlaySound("Dice Settle");

        StartCoroutine(Settling_Coroutine());
        
        /*for (int diceIndex = 0; diceIndex < currentThrow.diceAmount; diceIndex++) {
			dices[diceIndex].TurnToDirection (dices[diceIndex].result);
		}*/
	}

    IEnumerator Settling_Coroutine()
    {
        for (int diceIndex = 0; diceIndex < currentThrow.diceAmount; diceIndex++)
        {
            dices[diceIndex].TurnToDirection(dices[diceIndex].result);

            yield return new WaitForSeconds(
                dices[diceIndex].tweenDuration
                +
                dices[diceIndex].quickTweenDuration
                );
        }

        yield return new WaitForSeconds(timeBetweenSettles);

        ChangeState(states.showingHighest);
    }

    private void Settling_Update () {

		/*if (timeInState > settlingDuration)
			ChangeState (states.showingHighest);*/
	}
	private void Settling_Exit () {
	
        
	}
	#endregion

	#region showing highest
	public delegate void OnEndThrow ();
	public OnEndThrow onEndThrow;
	private void ShowingHighest_Start () {

        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlayRandomSound("ting");

        Dice highestDie = dices [0];

		highestResult = 0;

        int highestIndex = 0;

        for (int diceIndex = 0; diceIndex < CurrentThrow.diceAmount; diceIndex++) {

			if (dices[diceIndex].result > highestResult) {
				highestResult = dices [diceIndex].result;
                highestDie = dices[diceIndex];
                highestIndex = diceIndex;
			}
		}

        int a = 0;
        foreach (var die in dices)
        {
            if ( a != highestIndex)
            dices[a].SettleDown();

            ++a;
        }

        highestDie.SettleUp ();
		Throwing = false;
	}
	private void ShowingHighest_Update () {
		
		if (timeInState > settlingDuration + 0.1f) {
			
			ChangeState (states.none);
		}
	}
	private void ShowingHighest_Exit () {

        foreach (var item in dices)
        {
            item.Fade();
        }

        Invoke("ResetDice" , 1f);

        EndThrow();
	}

    void EndThrow()
    {
        Transitions.Instance.actionTransition.FadeOut(0.5f);

        if (onEndThrow != null)
            onEndThrow();
    }
	#endregion

	#region paint dice
	public Color DiceColors (DiceTypes type) {
		return diceColors [(int)type];
	}

	private void PaintDice ( DiceTypes type ) {
		foreach ( Dice dice in dices ) {
			dice.Paint (type);
		}
	}
	#endregion

	#region properties
	public float ThrowDuration {
		get {
			return throwDuration;
		}
	}

	public int ThrowDirection {
		get {
			return throwDirection;
		}
		set {
			throwDirection = value;
		}
	}

	public Throw CurrentThrow {
		get {
			return currentThrow;
		}
		set {
			currentThrow = value;
		}
	}

	public bool Throwing {
		get {
			return throwing;
		}
		set {
			throwing = value;
		}
	}
	public int HighestResult {
		get {
			return highestResult;
		}

	}
	#endregion


	#region states
	public void ChangeState ( states newState ) {
		previousState = currentState;
		currentState = newState;

		switch (previousState) {
		case states.throwing :
			Throwing_Exit ();
			break;
		case states.settling:
			Settling_Exit();
			break;
		case states.showingHighest :
			ShowingHighest_Exit ();
			break;
		case states.none :
			// nothing
			break;
		}

		switch (currentState) {
		case states.throwing :
			updateState = Throwing_Update;
			Throwing_Start ();
			break;
		case states.showingHighest :
			updateState = ShowingHighest_Update;
			ShowingHighest_Start ();
			break;

		case states.settling:
			updateState = Settling_Update;
			Settling_Start ();
			break;

		case states.none :
			updateState = null;
			break;
		}

		timeInState = 0f;
	}
	#endregion

	#region dice
	void ThrowDiceAmount ()
	{
		string cellParams = StoryFunctions.Instance.CellParams;
		int amount = int.Parse ( cellParams );

		StartCoroutine (ThrowDiceAmount_Coroutine (amount));
	}
	private void CheckStat () {

		int decal = StoryReader.Instance.CurrentStoryHandler.GetDecal ();

		if (decal < 0) {

			StartCoroutine (CheckStat_Coroutine ());

		} else {

			StoryReader.Instance.NextCell ();

			StoryReader.Instance.SetDecal (decal);

			StoryReader.Instance.UpdateStory ();

		}

	}

	IEnumerator CheckStat_Coroutine () {

        Crews.playerCrew.captain.memberIcon.animator.SetTrigger("throw dice");

		ThrowDirection = 1;

		string cellParams = StoryFunctions.Instance.CellParams;

		switch (cellParams) {
		case "STR":
			ThrowDice (DiceTypes.STR, Crews.playerCrew.captain.GetStat(Stat.Strenght));
			break;
		case "DEX":
			ThrowDice (DiceTypes.DEX, Crews.playerCrew.captain.GetStat(Stat.Dexterity));
			break;
		case "CHA":
			ThrowDice (DiceTypes.CHA, Crews.playerCrew.captain.GetStat(Stat.Charisma));
			break;
		case "CON":
			ThrowDice (DiceTypes.CON, Crews.playerCrew.captain.GetStat(Stat.Constitution));
			break;
		default:
			Debug.LogError ("PAS DE Dé " + cellParams + " : lancé de force");
			ThrowDice (DiceTypes.STR, Crews.playerCrew.captain.GetStat(Stat.Strenght));
			break;
		}


		while (Throwing)
			yield return null;

		int decal = HighestResult == 6 ? 0 : 1;

        // story
		StoryReader.Instance.CurrentStoryHandler.SaveDecal (decal);

		StoryReader.Instance.NextCell ();

		StoryReader.Instance.SetDecal (decal);

		StoryReader.Instance.UpdateStory ();
	}

	IEnumerator ThrowDiceAmount_Coroutine (int amount) {

		ThrowDirection = 1;

		ThrowDice (DiceTypes.CHA, amount);

		while (Throwing)
			yield return null;

		int decal = HighestResult == 6 ? 0 : 1;

		StoryReader.Instance.NextCell ();

		StoryReader.Instance.SetDecal (decal);

		StoryReader.Instance.UpdateStory ();
	}


	#endregion

}



public class Throw {

	public int diceAmount = 0;

	public DiceTypes diceType;

	public int highestResult = 0;

	public Throw ( int _amount , DiceTypes type ) {
		diceAmount = _amount;
		diceType = type;
	}

}
