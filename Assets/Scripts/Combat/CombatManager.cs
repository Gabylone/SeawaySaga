using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

	public static CombatManager Instance;

    public bool fighting = false;

    public Color selectionColor_Enemies;
    public Color selectionColor_Allies;
    public Color selectionColor_Self;

    public delegate void OnChangeState(States currState, States prevState);
    public OnChangeState onChangeState;

    // stsatesyes ?
    public enum States {
		
		None,

		CombatStart,
		PlayerMemberChoice,
		EnemyMemberChoice,
		StartTurn,
		PlayerActionChoice,
		EnemyActionChoice,

		PlayerAction,
		EnemyAction,

	}

	private States previousState = States.None;
	private States currentState = States.None;
	public delegate void UpdateState();
	private UpdateState updateState;
	private float timeInState = 0f;

    private FightOutcome currentFightOutCome = FightOutcome.None;

	/// <summary>
	/// The fighters
	/// </summary>
	private int memberIndex = 0;
	List<Fighter> fighters = new List<Fighter> ();

	[Header("Fighter Objects")]
	public GameObject playerFighters_Parent;
	public GameObject enemyFighters_Parent;
	public Fighter[] initPlayerFighters;
	private Fighter[] initEnemyFighters;

	public List<Fighter> currPlayerFighters = new List<Fighter>();
	public List<Fighter> currEnemyFighters = new List<Fighter>();
	public List<Fighter> getCurrentFighters (Crews.Side side) {
		return side == Crews.Side.Player ? currPlayerFighters : currEnemyFighters;
	}
//
	public Fighter currentFighter {
		get {
			return fighters [memberIndex];
		}
	}

	public CrewMember currentMember { get { return fighters [memberIndex].crewMember; } }

	int crewValue = 0;

	/// <summary>
	/// action
	/// </summary>
	[SerializeField]
	private CategoryContent categoryFightContent;

	// EVENTS
	public delegate void FightStarting ();
	public FightStarting onFightStart;

	public delegate void FightEnding ();
	public FightEnding onFightEnd;

    public Skill currentSkill;

    public GameObject cancelPlayerMemberChoiceButton;
    public Transform cancelPlayerMemberChoice_Transform;

    public bool debugKill = false;

    void Awake () {
		Instance = this;
	}

	// Use this for initialization
	void Start () {

		InitFighters ();

        cancelPlayerMemberChoiceButton.SetActive(false);
        cancelPlayerMemberChoice_Transform = cancelPlayerMemberChoiceButton.transform;

        StoryFunctions.Instance.getFunction += HandleGetFunction;
	}


    void Update()
    {

        if (updateState != null)
        {
            updateState();
            timeInState += Time.deltaTime;
        }
    }


    void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.LaunchCombat:
			StartFight ();
			break;

		}
	}

    #region Combat Start
    private void CombatStart_Start () {

		foreach (CrewMember member in Crews.enemyCrew.CrewMembers)
        {
            member.Icon.overable = true;
        }

        currentFightOutCome = FightOutcome.None;

        SortFighters ();
		ShowFighters ();

		onFightStart ();

		CrewMember.SetSelectedMember (currentMember);

		ChangeState (States.StartTurn);
	}

	private void CombatStart_Update () {}
	private void CombatStart_Exit () {}
	#endregion

	#region StartTurn
	private void StartTurn_Start () {
		currentFighter.SetTurn ();
	}
	public void StartActions ()
	{
		States state = currentMember.side == Crews.Side.Player ? States.PlayerActionChoice : States.EnemyActionChoice;
		ChangeState (state);
	}

	private void StartTurn_Update () {

	}

	private void StartTurn_Exit () {}

	public void NextTurn () {

		if (currentFightOutCome != FightOutcome.None) {
			return;
		}

		NextMember ();

		if ( currentFighter.killed )
        {
			NextTurn ();
			return;
		}

		if ( currentFighter.escaped )
        {
            NextTurn();
			return;
		}

		Invoke ("StartNewTurn" , 1f);

	}

	void StartNewTurn () {

		ChangeState (States.StartTurn);
		//
	}

	public enum FightOutcome
	{
        None,

        PlayerCrewKilled,
		PlayerCrewEscaped,
		EnemyCrewKilled,

	}

	
	void NoMorePlayersDelay()
    {
		
	
	}
	void NoMoreEnnemiesDelay()
    {
		

	}
	#endregion

	#region member choice
	public void GoToTargetSelection ( Crews.Side side , Skill skill ) {

		currentSkill = skill;

		if (side == Crews.Side.Player) {
			ChangeState (States.PlayerMemberChoice);
		} else {
			ChangeState (States.EnemyMemberChoice);
		}

	}
	public void ChoseTargetMember (Fighter fighter) {

		fighter.SetAsTarget ();

		ChangeState (States.PlayerAction);
	}

	public void DisablePickable () {
		foreach (Fighter fighter in getCurrentFighters (Crews.Side.Player)) {
			fighter.Pickable = false;
		}
		foreach (Fighter fighter in getCurrentFighters (Crews.Side.Enemy)) {
			fighter.Pickable = false;
		}
	}

	public void ChoosingTarget (Crews.Side side) {

		if (side == Crews.Side.Enemy) {
			
			bool provoking = false;

			foreach (Fighter fighter in getCurrentFighters (side)) {
				if ( fighter.HasStatus(Fighter.Status.Provoking) ) {
					provoking = true;
					break;
				}
			}

			if (provoking) {

                //print ("y'a eu provocation apparemment, donc y'en a qui sont pas pickable ?");

                foreach (Fighter fighter in getCurrentFighters (side)) {
					if (fighter.HasStatus (Fighter.Status.Provoking)) {
						fighter.Pickable = true;
					}
				}

			} else {

				foreach (Fighter fighter in getCurrentFighters (side)) {
					fighter.Pickable = true;
				}

			}
		} else {

			foreach (Fighter fighter in getCurrentFighters (side)) {
				if (!currentSkill.canTargetSelf) {

					if ( fighter != currentFighter )
						fighter.Pickable = true;

				} else {
					fighter.Pickable = true;
				}
			}

		}
	}
	#endregion

	#region Player Action Choice
	private void PlayerActionChoice_Start () {



	}
	private void PlayerActionChoice_Update () {}
	private void PlayerActionChoice_Exit () {}
	#endregion

	#region Player Member Choice 
	private void PlayerMemberChoice_Start () {

		cancelPlayerMemberChoiceButton.SetActive (true);

        SkillDescription.Instance.Show(currentSkill);

		Tween.Bounce (cancelPlayerMemberChoice_Transform);

		if (currentSkill.targetType == Skill.TargetType.Self) {
			ChoosingTarget (Crews.Side.Player);
		} else {
			ChoosingTarget (Crews.Side.Enemy);
		}


    }
    private void PlayerMemberChoice_Update ()
    {

    }
	private void PlayerMemberChoice_Exit ()
    {
		DisablePickable ();
        SkillDescription.Instance.Hide();

		cancelPlayerMemberChoiceButton.SetActive (false);

    }
	public void CancelPlayerMemberChoice ()
    {
		ChangeState (States.PlayerActionChoice);
	}
	#endregion

	#region Player Action
	private void PlayerAction_Start () {

	}
	private void PlayerAction_Update () {

    }
	private void PlayerAction_Exit () {

    }
	#endregion

	#region Enemy Action Choice
	private void EnemyActionChoice_Start () {

		Skill skill = SkillManager.RandomSkill (currentMember);
		skill.Trigger (CombatManager.Instance.currentFighter);

	}
	private void EnemyActionChoice_Update () {}
	private void EnemyActionChoice_Exit () {}
	#endregion

	#region Enemy Member Choice 
	private void EnemyMemberChoice_Start ()
    {
		if ( currentSkill.preferedTarget != null )
        {
			currentSkill.preferedTarget.SetAsTarget ();
			currentSkill.preferedTarget = null;
			return;
		}

		if (currentSkill.targetType == Skill.TargetType.Self)
        {

			// attention au pledge of feast.
			List<Fighter> targetFighters = currEnemyFighters;

			if ( currentSkill.canTargetSelf == false )
				targetFighters.Remove (currentFighter);

			int randomIndex = Random.Range (0, targetFighters.Count);

			targetFighters [randomIndex].SetAsTarget ();

		}
        else
        {

			Fighter weakestFighter = currPlayerFighters [0];

			foreach (var item in currPlayerFighters) {

				if (item.HasStatus (Fighter.Status.Provoking)) {
					item.SetAsTarget ();
					return;
				}

				if (item.HasStatus (Fighter.Status.Protected))
					continue;

				if ( item.crewMember.Health < weakestFighter.crewMember.Health ) {
					weakestFighter = item;
				}
			}

			weakestFighter.SetAsTarget ();

		}

	}
	private void EnemyMemberChoice_Update () {

		if (timeInState >= 0.5f) {

			ChangeState (States.EnemyAction);

		}

	}
	private void EnemyMemberChoice_Exit () {}
	#endregion

	#region Enemy Action
	private void EnemyAction_Start () {

	}
	private void EnemyAction_Update () {}
	private void EnemyAction_Exit () {}
	#endregion

	#region loot & xp
	public void ShowLoot () {
        OtherInventory.Instance.StartLooting (true);
	}
	#endregion 

	#region fight end
    private void DisplayEndFightMessage()
    {
        switch (currentFightOutCome)
        {
		case FightOutcome.PlayerCrewKilled:
                HandleDefeat();
                break;
		case FightOutcome.PlayerCrewEscaped:
                HandleEscape();
                break;
		case FightOutcome.EnemyCrewKilled:
                HandleVictory();
                break;
            case FightOutcome.None:
			break;
            default:
			break;
		}

        /*
         * switch (currentFightOutCome)
        {
		case FightOutcome.PlayerCrewKilled:
        HideFight();
			break;
		case FightOutcome.PlayerCrewEscaped:
			Escape ();
			Invoke ("ExitFight", 1f);
			break;
		case FightOutcome.EnemyCrewKilled:
			ReceiveXp ();
			Invoke ("ExitFight", 1f);
			Invoke ("ShowLoot", 1.5f);
			break;
		case FightOutcome.None:
			break;
		default:
			break;
		}
        */
    }

    #region escape
    private void HandleEscape()
    {
        string str = "Cowardly, CAPITAINE and his crew escape from the fight. They better not come back until they match the strenght of their foes";
        if (Crews.playerCrew.CrewMembers.Count == 1)
        {
            str = "Cowardly, CAPITAINE escapes from the fight. He better not come back until he matches the strenght of his foes";
        }

        DisplayCombatResults.Instance.Display("ESCAPE !", str);
        DisplayCombatResults.Instance.onConfirm += HandleOnConfirm_Escape;

        SoundManager.Instance.PlayRandomSound("swipe");
    }
    private void HandleOnConfirm_Escape()
    {
        Invoke("Escape", 1f);
        Invoke("HideFight", 1f);
    }
    void Escape()
    {
        // ici seulement pendant la FUITE, parce que le loot des morts l'ouvre auto.
        // pour régler bug : menu droit n'apparrait pas quand on fuit
        StoryLauncher.Instance.EndStory();

        Invoke("EscapeDelay", 1f);

    }
    void EscapeDelay()
    {
        InGameMenu.Instance.ShowMenuButtons();
    }
    #endregion

    #region defeat
    private void HandleDefeat()
    {
        int goldReceived = crewValue * Random.Range(10, 15);
        GoldManager.Instance.AddGold(goldReceived);

        string str = "CAPITAINE didn't manage too bring his crew to victory. Maybe in an another life, he'll find wealth and reknown.";
        if (Crews.playerCrew.CrewMembers.Count == 1)
        {
            str = "CAPITAINE didn't manage too emerge victorious from the fight. Maybe in an another life, he'll find wealth and reknown.";
        }

        DisplayCombatResults.Instance.Display("DEFEAT !", str);
        DisplayCombatResults.Instance.onConfirm += HandleOnConfirm_Defeat;

        SoundManager.Instance.PlaySound("Defeat");
        SoundManager.Instance.PlayRandomSound("ui_deny");
        SoundManager.Instance.PlaySound("ui_deny");
    }
    private void HandleOnConfirm_Defeat()
    {
        Transitions.Instance.ScreenTransition.FadeIn(1f);

        Invoke("HandleOnConfirm_DefeatDelay", 1f);
    }
    private void HandleOnConfirm_DefeatDelay()
    {
        GameManager.Instance.BackToMenu();
    }
    #endregion

    #region victory
    private void HandleVictory()
    {
        // xp 
        int xpPerMember = 40;
        foreach (var item in currPlayerFighters)
        {
            item.combatFeedback.Display("" + xpPerMember, Color.white);
            item.crewMember.AddXP(xpPerMember);
        }

        // gold
        int goldReceived = crewValue * Random.Range(10, 15);
        GoldManager.Instance.AddGold(goldReceived);

        string str = "CAPITAINE and his crew sucessfly defeated their foes. Loot, gold and experience await them";

        if ( Crews.playerCrew.CrewMembers.Count == 1)
        {
            str = "CAPITAINE sucessfly defeated his foes. Loot, gold and experience await him";
        }

        str = NameGeneration.CheckForKeyWords(str);

        DisplayCombatResults.Instance.Display("VICTORY !", str);

        DisplayCombatResults.Instance.DisplayResults(goldReceived, xpPerMember);
        DisplayCombatResults.Instance.onConfirm += HandleOnConfirm_Victory;
    }

    private void HandleOnConfirm_Victory()
    {
        if (DisplayCrewMemberLevelUp.Instance.crewMembersToDisplay.Count > 0)
        {
            DisplayCrewMemberLevelUp.Instance.DisplayLastCrewMember();
        }
        else
        {
            HandleOnConfirm_Victory_Continue();
        }

    }

    public void HandleOnConfirm_Victory_Continue()
    {
        Invoke("ShowLoot", 0.8f);
        Invoke("HideFight", 0.4f);
    }
    #endregion

    public void HideFight()
    {
        fighting = false;

        ChangeState(States.None);

        HideFighters(Crews.Side.Player);
        HideFighters(Crews.Side.Enemy);

        if (onFightEnd != null)
        {
            onFightEnd();
        }

        UIBackground.Instance.ShowBackGround();

        Invoke("HideFightDelay", UIBackground.Instance.duration);
    }
    void HideFightDelay()
    {
        InGameBackGround.Instance.SetWhite();
    }
	#endregion

	#region fighters
	void InitFighters ()
	{
		enemyFighters_Parent.SetActive (true);
		playerFighters_Parent.SetActive (true);

		initPlayerFighters = playerFighters_Parent.GetComponentsInChildren<Fighter> (true);
		initEnemyFighters = enemyFighters_Parent.GetComponentsInChildren<Fighter> (true);
	}
	void SortFighters ()
	{
		memberIndex = 0;

		fighters.Clear ();
		currPlayerFighters.Clear ();
		currEnemyFighters.Clear ();

		for (int fighterIndex = 0; fighterIndex < Crews.playerCrew.CrewMembers.Count; fighterIndex++) {
			fighters.Add (initPlayerFighters[fighterIndex]);
			currPlayerFighters.Add (initPlayerFighters [fighterIndex]);
		}

		for (int fighterIndex = 0; fighterIndex < Crews.enemyCrew.CrewMembers.Count; fighterIndex++) {
			fighters.Add (initEnemyFighters[fighterIndex]);
			currEnemyFighters.Add (initEnemyFighters [fighterIndex]);
		}
	}
	private void ShowFighters () {

		foreach ( Crews.Side side in Crews.Instance.Sides ) {
			
			Crews.getCrew(side).UpdateCrew ( Crews.PlacingType.Hidden );

			Fighter[] fighters = side == Crews.Side.Player ? initPlayerFighters : initEnemyFighters;

			for (int fighterIndex = 0; fighterIndex < Crews.getCrew(side).CrewMembers.Count; fighterIndex++) {
				
				fighters [fighterIndex].Reset (Crews.getCrew(side).CrewMembers[fighterIndex],fighterIndex);
			}
		}

		foreach (CrewMember mem in Crews.enemyCrew.CrewMembers)
			crewValue += mem.Level;
	}
	private void HideFighters (Crews.Side side) {

		Fighter[] fighters = side == Crews.Side.Player ? initPlayerFighters : initEnemyFighters;

		foreach (Fighter f in fighters)
			f.Hide ();
	}
    public void DeleteFighter(Fighter fighter)
    {

        if (fighter.crewMember.side == Crews.Side.Player)
        {
            currPlayerFighters.Remove(fighter);
        }
        else
        {
            currEnemyFighters.Remove(fighter);
        }

        DetermineFightOutcome();
        if (currentFightOutCome != FightOutcome.None)
        {
            Invoke("DisplayEndFightMessage", 1f);
        }
    }
    private void DetermineFightOutcome()
    {
        if (currEnemyFighters.Count == 0)   
        {
            currentFightOutCome = FightOutcome.EnemyCrewKilled;
            return;
        }

        if (currPlayerFighters.Count == 0)
        {

            if (Crews.playerCrew.CrewMembers.Count == 0)
            {
                currentFightOutCome = FightOutcome.PlayerCrewKilled;
                return;
            }

            currentFightOutCome = FightOutcome.PlayerCrewEscaped;
            return;
        }

        currentFightOutCome = FightOutcome.None;
    }
    #endregion

    #region StateMachine
    public void ChangeState ( States newState ) {

		previousState = currentState;
		currentState = newState;

		if (onChangeState != null) {
			onChangeState (currentState, previousState);
		}

		ExitState ();
		EnterState ();

		timeInState = 0f;

	}
	private void EnterState () {
		switch (currentState) {
		case States.CombatStart:
			updateState = CombatStart_Update;
			CombatStart_Start ();
			break;
		case States.StartTurn:
			updateState = StartTurn_Update;
			StartTurn_Start ();
			break;
		case States.PlayerActionChoice:
			updateState = PlayerActionChoice_Update;
			PlayerActionChoice_Start ();
			break;

		case States.PlayerMemberChoice:
			updateState = PlayerMemberChoice_Update;
			PlayerMemberChoice_Start ();
			break;
		case States.PlayerAction:
			updateState = PlayerAction_Update;
			PlayerAction_Start ();
			break;
		case States.EnemyActionChoice:
			updateState = EnemyActionChoice_Update;
			EnemyActionChoice_Start ();
			break;
		case States.EnemyMemberChoice:
			updateState = EnemyMemberChoice_Update;
			EnemyMemberChoice_Start();
			break;
		case States.EnemyAction:
			updateState = EnemyAction_Update;
			EnemyAction_Start ();
			break;

		case States.None:
			updateState = null;
			break;
		}
	}
	private void ExitState () {
		switch (previousState) {
		case States.CombatStart:
			CombatStart_Exit ();
			break;
		case States.StartTurn:
			StartTurn_Exit ();
			break;
		case States.PlayerActionChoice:
			PlayerAction_Exit();
			break;
		case States.PlayerMemberChoice:
			PlayerMemberChoice_Exit();
			break;
		case States.PlayerAction:
			PlayerAction_Exit();
			break;
		case States.EnemyActionChoice:
			EnemyAction_Exit();
			break;
		case States.EnemyMemberChoice:
			EnemyMemberChoice_Exit();
			break;
		case States.EnemyAction:
			EnemyAction_Exit();
			break;
			//

		case States.None:
			break;
		}
	}
	#endregion

	#region properties
	public void StartFight () {

		Crews.enemyCrew.managedCrew.hostile = true;

		fighting = true;

		ChangeState (States.CombatStart);

	}

	void NextMember () {
		
		++memberIndex;

		if ( memberIndex >= fighters.Count )
			memberIndex = 0;
	}
	#endregion


}
