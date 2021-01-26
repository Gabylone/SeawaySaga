using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using DG.Tweening;

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

    public enum Result
    {
        CriticalFailure,
        Failure,
        Success,

        None
    }

    public Result result;

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

    public CrewMember thrower;
    Stat currentStat;

    public bool waitingForThrowerSelection = false;

	[SerializeField]
	private Dice[] dices;

	private Throw currentThrow;

	private int highestResult = 0;

    public GameObject backgroundObj;
    public CanvasGroup backgroundCanvasGroup;
    public Text backgroundText;

	private bool throwing = false;
	private float timer = 0f;

    public Color color_CriticalFailure;
    public Color color_Failure;
    public Color color_Success;
    // no critical sucess, <= 5 is failure
    //public Color color_CriticalSuccess;

    void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {
		
		InitDice ();
		ResetDice ();

        backgroundObj.SetActive(false);


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
    void ShowDice()
    {
        foreach (var item in dices)
        {
            item.Show();
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

        ResetDice();

        ShowDice();

        Transitions.Instance.actionTransition.FadeIn(0.5f);

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Dice Multiple");
        SoundManager.Instance.PlayRandomSound("Dice Multiple");

        SoundManager.Instance.PlayLoop("dice_wait");

        result = Result.None;


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
            Dice dice = dices[diceIndex];

            dice.TurnToDirection(dice.targetResult);

            yield return new WaitForSeconds(
                dice.tweenDuration
                +
                dice.quickTweenDuration
                );

            Tween.Bounce(dice.bodyTransform);

            if (dice.targetResult == 6)
            {
                dice.SettleUp();

                for (int i = 0; i < diceIndex; i++)
                {
                    Dice previousDice = dices[i];

                    if (previousDice.targetResult == 1)
                    {
                        SoundManager.Instance.PlaySound("ui_wrong");
                        SoundManager.Instance.PlayRandomSound("Magic Chimes");

                        //dice.SettleDown();

                        dice.targetResult = -1;
                        previousDice.targetResult = -1;

                        yield return new WaitForSeconds(dice.tweenDuration);

                        SoundManager.Instance.PlaySound("ui_wrong");
                        SoundManager.Instance.PlayRandomSound("Whoosh");

                        dice.Fade();
                        previousDice.Fade();

                        break;
                    }
                }

            }
            else
            {
                if ( dice.targetResult == 1)
                {
                    for (int i = 0; i < diceIndex; i++)
                    {
                        Dice previousDice = dices[i];

                        if (previousDice.targetResult == 6)
                        {
                            SoundManager.Instance.PlaySound("ui_wrong");
                            SoundManager.Instance.PlayRandomSound("Magic Chimes");

                            dice.SettleDown();

                            dice.targetResult = -1;
                            previousDice.targetResult = -1;

                            yield return new WaitForSeconds(dice.tweenDuration);

                            SoundManager.Instance.PlaySound("ui_wrong");
                            SoundManager.Instance.PlayRandomSound("Whoosh");

                            dice.Fade();
                            previousDice.Fade();

                            break;
                        }
                    }
                }

                dice.SettleDown();
            }

            yield return new WaitForSeconds(dice.tweenDuration);
        }

        for (int i = 0; i < currentThrow.diceAmount; i++)
        {
            Dice item = dices[i];

            if ( item.targetResult == 6)
            {
                result = Result.Success;
                break;
            }

            if ( item.targetResult == 1)
            {
                result = Result.CriticalFailure;
                break;
            }

            result = Result.Failure;
        }

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
    private void ShowingHighest_Start()
    {

        SoundManager.Instance.PlayRandomSound("Tribal");
        SoundManager.Instance.PlayRandomSound("ting");

        Throwing = false;

        foreach (var item in dices)
        {
            item.Fade();
        }

        

    }
    private void ShowingHighest_Update () {
		
		if (timeInState > settlingDuration + 0.1f) {
			
			ChangeState (states.none);
		}
	}
    private void ShowingHighest_Exit()
    {
        Transitions.Instance.actionTransition.FadeOut(0.5f);

        switch (result)
        {
            case Result.CriticalFailure:
                DisplayDiceResult.Instance.Display("Critical Failure !");
                break;
            case Result.Failure:
                DisplayDiceResult.Instance.Display("Failure !");
                break;
            case Result.Success:
                DisplayDiceResult.Instance.Display("Success!");
                break;
            case Result.None:
                break;
            default:
                break;
        }
    }

    public void EndThrow()
    {
        Invoke("EndThrowDelay", 0.5f);
    }

    void EndThrowDelay()
    {
        ResetDice();

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
    void ThrowDiceAmount()
    {
        string cellParams = StoryFunctions.Instance.cellParams;
        int amount = int.Parse(cellParams);

        ThrowDirection = 1;
        ThrowDice(DiceTypes.CHA, amount);

        onEndThrow += ThrowDiceAmount_HandleOnEndThrow;
    }

    void ThrowDiceAmount_HandleOnEndThrow()
    {
        onEndThrow -= ThrowDiceAmount_HandleOnEndThrow;

        int decal = result == Result.Success ? 0 : 1;

        StoryReader.Instance.NextCell();

        StoryReader.Instance.SetDecal(decal);

        StoryReader.Instance.UpdateStory();
    }
    #endregion throw dice amount

    #region check stat
    private void CheckStat () {

        int decal = StoryReader.Instance.CurrentStoryHandler.GetDecal();

        // decal has already been done
        if (decal >= 0)
        {
            if ( decal == 0)
            {
                DialogueManager.Instance.PlayerSpeak("I already nailed this !");
            }
            else
            {
                DialogueManager.Instance.PlayerSpeak("I already blew this, no need retrying !");
            }

            DialogueManager.Instance.onEndDialogue += HandleOnEndDialogue;
            return;
        }

        string cellParams = StoryFunctions.Instance.cellParams;
        string color_html = "";

        switch (cellParams)
        {
            case "STR":
                currentStat = Stat.Strenght;
                color_html = "<color=red>";
                break;
            case "DEX":
                currentStat = Stat.Dexterity;
                color_html = "<color=green>";
                break;
            case "CHA":
                color_html = "<color=magenta>";
                currentStat = Stat.Trickery;
                break;
            case "CON":
                color_html = "<color=lightblue>";
                currentStat = Stat.Constitution;
                break;
            default:
                Debug.LogError("PAS DE Dé " + cellParams + " : lancé de force");
                currentStat = Stat.Strenght;
                break;
        }

        Crews.playerCrew.UpdateCrew(Crews.PlacingType.Portraits);

        foreach (var item in Crews.playerCrew.CrewMembers)
        {
            item.Icon.ShowDiceStats(currentStat);
        }

        waitingForThrowerSelection = true;

        backgroundObj.SetActive(true);
        backgroundCanvasGroup.alpha = 0f;
        backgroundCanvasGroup.DOFade(1f, 0.2f);

        backgroundText.text = "Who will throw the dice for " + color_html + currentStat + " </color>";

    }

    void HandleOnEndDialogue()
    {
        DialogueManager.Instance.onEndDialogue -= HandleOnEndDialogue;

        int decal = StoryReader.Instance.CurrentStoryHandler.GetDecal();
        StoryReader.Instance.NextCell();
        StoryReader.Instance.SetDecal(decal);
        StoryReader.Instance.UpdateStory();
    }

    public void SelectThrower(CrewMember member)
    {
        waitingForThrowerSelection = false;

        foreach (var item in Crews.playerCrew.CrewMembers)
        {
            item.Icon.HideDiceStats();
        }

        Crews.playerCrew.UpdateCrew(Crews.PlacingType.World);

        backgroundCanvasGroup.DOFade(0f, 0.2f);

        thrower = member;

        CancelInvoke("OnSelectThrower");
        Invoke("OnSelectThrower", 0.5f);

        CancelInvoke("SelectThrowerDelay");
        Invoke("SelectThrowerDelay", 0.2f);
    }

    void SelectThrowerDelay()
    {
        backgroundObj.SetActive(false);
    }

    void OnSelectThrower()
    {
        onEndThrow += CheckStat_HandleOnEndThrow;

        thrower.memberIcon.animator.SetTrigger("throw dice");

        ThrowDirection = 1;

        string cellParams = StoryFunctions.Instance.cellParams;

        switch (cellParams)
        {
            case "STR":
                ThrowDice(DiceTypes.STR, thrower.GetStat(Stat.Strenght));
                break;
            case "DEX":
                ThrowDice(DiceTypes.DEX, thrower.GetStat(Stat.Dexterity));
                break;
            case "CHA":
                ThrowDice(DiceTypes.CHA, thrower.GetStat(Stat.Trickery));
                break;
            case "CON":
                ThrowDice(DiceTypes.CON, thrower.GetStat(Stat.Constitution));
                break;
            default:
                Debug.LogError("PAS DE Dé " + cellParams + " : lancé de force");
                ThrowDice(DiceTypes.STR, thrower.GetStat(Stat.Strenght));
                break;
        }
        
    }

    void CheckStat_HandleOnEndThrow()
    {
        onEndThrow -= CheckStat_HandleOnEndThrow;

        int decal = result == Result.Success ? 0 : 1;

        // story
        StoryReader.Instance.CurrentStoryHandler.SaveDecal(decal);

        StoryReader.Instance.NextCell();

        StoryReader.Instance.SetDecal(decal);

        StoryReader.Instance.UpdateStory();
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
