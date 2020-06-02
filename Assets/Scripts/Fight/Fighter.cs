using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Fighter : MonoBehaviour {

	public bool killed = false;
	public bool escaped = false;

	public enum states {

		dead,

		none,

		moveToTarget,
		moveBack,
		triggerSkill,
		getHit,
		guard,
		blocked

	}
	private states currentState = states.moveToTarget;

	public states CurrentState {
		get {
			return currentState;
		}
	}

	private states previousState;

	public CombatFeedback combatFeedback;

	float timeInState = 0f;

	private delegate void UpdateState ();
	private UpdateState updateState;

	public GameObject ui_Group;

	[Header ("Components")]
	[SerializeField]
	private Transform bodyTransform;
	public Animator animator;
	public CrewMember crewMember;
	public Transform arrowAnchor;
    public StatusGroup statusGroup;

    public IconVisual iconVisual;
    public CanvasGroup canvasGroup;

	[Header ("Move")]
	public float moveToTargetDuration = 0.79f;
	public float moveBackDuration = 0.79f;

	public enum Direction {
		Right,
		Left
	}

	[SerializeField]
	private Direction direction;

	int dir = 0;

	[Header ("Hit")]
	[SerializeField]
	private float hit_Duration = 0.5f;
	public float Hit_Duration { get { return hit_Duration; } }

	[SerializeField]
	private float hit_TimeToEnableCollider = 0.5f;
	public float Hit_TimeToEnableCollider {
		get {
			return hit_TimeToEnableCollider;
		}
	}

	[SerializeField]
	private GameObject impactEffect;

	// get hit
	[Header ("Get Hit")]
	[SerializeField]
	private float getHit_Duration = 0.3f;

	// blocked
	[Header ("Blocked")]
	[SerializeField]
	private float blocked_Duration = 0.5f;

	[Header("Dodge")]
	[SerializeField]
	private float maxDodgeChance = 20;

	private Vector3 initPos = Vector3.zero;

	[Header("Sounds")]
	[SerializeField] private AudioClip hitSound;
	[SerializeField] private AudioClip hurtSound;

	[Header ("Target")]
	private Fighter targetFighter;
	[SerializeField]
	private float stopDistance = 1f;

	private bool pickable = false;
	public delegate void OnSetPickable ( bool b );
	public OnSetPickable onSetPickable;
	public bool Pickable {
		get {
			return pickable;
		}
		set {
			if (pickable == value)
				return;

			pickable = value;

			if (onSetPickable != null)
				onSetPickable (value);
		}
	}

	// Use this for initialization
	void Start () {

		transform.parent.GetComponentInChildren<Card> ().Init ();

		dir = direction == Direction.Left ? 1 : -1;

		initPos = transform.position;

		Hide ();

	}


	// Update is called once per frame
	void FixedUpdate () {

		if ( updateState != null )
			updateState ();

		timeInState += Time.deltaTime;


	}

	#region initalization
	public delegate void OnInit ();
	public OnInit onReset;
	public void Reset ( CrewMember crewMember, int id )
	{
		this.crewMember = crewMember;

		Show ();

		ChangeState (states.none);

		killed = false;
		escaped = false;

		animator.SetBool("dead",false);

		// animation
		transform.position = initPos;

		// energy & status
		crewMember.energy = 0;

		onSkillDelay = null;

		for (int i = 0; i < statusCount.Length; i++) {
			statusCount [i] = 0;
		}

        // member sprites
        iconVisual.InitVisual(crewMember.MemberID);

		// event
		if ( onReset != null )
			onReset ();
	}

	public delegate void OnSetTurn ();
	public OnSetTurn onSetTurn;
	public void SetTurn () {

		Tween.Bounce (transform);

		ChangeState (Fighter.states.none);

        if ( HasStatus(Status.PreparingAttack) ) {

			if (onSkillDelay != null) {
				onSkillDelay (this);
			}
             
			RemoveStatus (Status.PreparingAttack);
			return;
			//
		}

		/// knocked out
		if ( HasStatus(Status.KnockedOut) ) {
			RemoveStatus (Status.KnockedOut);
			combatFeedback.Display ("zzz", Color.white);

			EndTurn ();
			CombatManager.Instance.NextTurn ();
			return;
			//
		}

		CheckStatus ();

		crewMember.AddEnergy (crewMember.energyPerTurn);

		for (int i = 0; i < crewMember.charges.Length; i++) {
			if ( crewMember.charges [i] > 0 )
				crewMember.charges [i] -= 1;
		}

		CombatManager.Instance.StartActions ();

		if (onSetTurn != null) {
			onSetTurn (); 
		}

	}

	public delegate void OnEndTurn ();
	public OnEndTurn onEndTurn;
	public void EndTurn ()
	{

		if (onEndTurn != null)
			onEndTurn ();
	}

	public delegate void OnSetAsTarget ();
	public OnSetAsTarget onSetAsTarget;
	public void SetAsTarget () {

		if ( HasStatus(Status.Provoking) ) {
			RemoveStatus (Status.Provoking);
		}
			

		CombatManager.Instance.currentFighter.TargetFighter = this;
//		targetFighter = CombatManager.Instance.currentFighter;

		Tween.Bounce (transform);

		if (onSetAsTarget != null)
			onSetAsTarget ();
	}

	public void Hide ()
	{
		ChangeState (states.none);

		ui_Group.SetActive (false);
		gameObject.SetActive (false);

	}

    public void Fade()
    {

        ChangeState(states.none);

        canvasGroup.DOFade(0f, 0.5f);

        ui_Group.SetActive(false);
    }

	void Show () {

        canvasGroup.alpha = 1f;

		ui_Group.SetActive (true);
		gameObject.SetActive (true);
	}

	public virtual void Die () {

			// self
		ChangeState (states.dead);
		animator.SetBool ("dead", true);

		killed = true;

		Fade ();
		CombatManager.Instance.DeleteFighter (this);

	}
	#endregion

	#region move to target
	public Fighter TargetFighter {
		get {
			return targetFighter;
		}
		set {
			targetFighter = value;
		}
	}

	public delegate void OnReachTarget ();
	public OnReachTarget onReachTarget;

	public virtual void MoveToTarget_Start () {

		animator.SetBool ("move", true);

        Vector3 dir = new Vector3 ( TargetFighter.crewMember.side == Crews.Side.Enemy ? -1 : 1 , 0 , 0 ); 

		Vector3 targetPos = TargetFighter.transform.position + dir * stopDistance;

        transform.DOMove(targetPos, moveToTargetDuration);

	}
	public virtual void MoveToTarget_Update () {
		
		if (timeInState > moveToTargetDuration ) {

			ChangeState (states.none);

		}

	}

	public virtual void MoveToTarget_Exit () {
		
		animator.SetBool ("move", false);

		if (onReachTarget != null) {
			onReachTarget ();
		}

	}
	#endregion

	#region move back
	public virtual void MoveBack_Start () {

		animator.SetBool ("move", true);

        transform.DOMove(initPos, moveBackDuration);

	}

	public virtual void MoveBack_Update () {

		if (timeInState > moveToTargetDuration) {
			ChangeState (states.none);
		}
	}

	public virtual void MoveBack_Exit () {
		animator.SetBool ("move", false);
    }
	#endregion

	#region hit
	public virtual void triggerSkill_Start () {

	}

	public virtual void triggerSkill_Update () {

	}
	public virtual void hit_Exit () {

	}
	#endregion

	#region guard
	public virtual void guard_Start () {
        //animator.SetBool ( "guard" , true);
    }

    public virtual void guard_Update () {

	}
	public virtual void guard_Exit () {
        //animator.SetBool ( "guard" , false);
    }
    #endregion

    #region blocked
    public virtual void blocked_Start () {
		
		//animator.SetBool ( "blocked" , true);
	}

	public virtual void blocked_Update () {

		if (timeInState > blocked_Duration) {
			ChangeState (states.none);
		}

	}
	public void blocked_Exit () {
        //animator.SetBool ( "blocked" , false);
    }
    #endregion

    #region dead
    void dead_Start ()
	{
		
	}
	void dead_Update ()
	{
		
	}
	void dead_Exit ()
	{
		
	}
	#endregion

	public delegate void OnShowInfo();
	public OnShowInfo onShowInfo;
	public void ShowInfo () {

		if ( Pickable ) {
			CombatManager.Instance.ChoseTargetMember (this);
			Tween.Bounce (transform);
			return;
		}

		if (CombatManager.Instance.currentFighter == this && CombatManager.Instance.currentFighter.crewMember.side == Crews.Side.Player ) {
			return;
		}

		if (onShowInfo != null)
			onShowInfo ();
	}

	public void Speak (string txt)
	{
		DialogueManager.Instance.SetDialogueTimed (txt, arrowAnchor);
	}

	#region get hit
	public virtual void getHit_Start () {

		animator.SetTrigger ("getHit");
	}
	public virtual void getHit_Update () {

		if (timeInState > getHit_Duration)
			ChangeState (states.none);
	}
	public virtual void getHit_Exit () {
		//
	}

	void HitEffect () {
		
		SoundManager.Instance.PlaySound (hitSound);

		impactEffect.SetActive (false);
		impactEffect.SetActive (true);

		animator.SetTrigger("getHit");

		impactEffect.transform.position = BodyTransform.position;
	}

	float GetDamage (Fighter otherFighter, float attack, float mult)
	{
		float damage = 0f;

		damage = crewMember.GetDamage (attack);
		damage *= mult;

		// reduced damage
		if ( otherFighter.HasStatus(Status.Cussed) ) {
			damage = damage * 0.5f;
		}

		if ( otherFighter.HasStatus(Status.Toasted) ) {
			damage = damage * 1.5f;
		}

		if ( HasStatus(Status.Protected) ) {
			damage = damage * 0.5f;
		}

		damage = Mathf.Round (damage);

		return damage;
	}

	public delegate void OnGetHit ();
	public OnGetHit onGetHit;
	public void GetHit (Fighter otherFighter, float attack, float mult) {

		if (SucceedDodge() == true) {
			return;
		}

		HitEffect ();

		float damage = GetDamage (otherFighter, attack,mult);

		if ( CombatManager.Instance.debugKill ) {
			damage = 100;
		}

		combatFeedback.Display (damage.ToString() , Color.red);
		crewMember.RemoveHealth (damage);

		if (onGetHit != null)
			onGetHit ();

		if (crewMember.Health <= 0) {

			if (otherFighter.crewMember.side == Crews.Side.Player) {
				int xpPerMember = 25;

				otherFighter.crewMember.AddXP (xpPerMember);
				otherFighter.combatFeedback.Display ("" + xpPerMember, Color.white);
			}

			crewMember.Kill ();
			Die ();
		}
	}

	public void CheckContact (Fighter otherFighter) {
		if (HasStatus (Status.BearTrapped)) {

			RemoveStatus (Status.BearTrapped, 1);

			otherFighter.Hurt (30);
		}
	}


	public void Hurt (float amount) {

		combatFeedback.Display ("" + amount , Color.red);

		crewMember.RemoveHealth (amount);

		if (onGetHit != null)
			onGetHit ();

		if (crewMember.Health <= 0) {
			crewMember.Kill ();
		}

	}

	public void Heal (float amount) {

		crewMember.AddHealth (amount);

		combatFeedback.Display ("" + amount , Color.green);

		if (onGetHit != null)
			onGetHit ();
	}

	public bool SucceedDodge () {

		// DODGE
		float dodgeChange = Random.value * 100f;

		float dodgeSkill = (float)crewMember.GetStat(Stat.Dexterity) / 6f;
		dodgeSkill *= maxDodgeChance;

		if ( dodgeChange < dodgeSkill ) {
			animator.SetTrigger("dodge");
			combatFeedback.Display ("Missed !", Color.red);
			return true;
		}

		return false;

	}
	#endregion

	#region getters
	public Transform BodyTransform {
		get {
			return bodyTransform;
		}
	}
	#endregion

	#region state machine
	public delegate void OnChangeState (states currState,states prevState);
	public OnChangeState onChangeState;
	public void ChangeState ( states targetState ) {

		previousState = currentState;
		currentState = targetState;

		timeInState = 0f;

		if (onChangeState != null)
			onChangeState (currentState, previousState);

		ExitState ();
		EnterState ();
	}
	private void EnterState () {
		switch (currentState) {

		case states.none:
			updateState = null;
			break;

		case states.moveToTarget:
			updateState = MoveToTarget_Update;
			MoveToTarget_Start();
			break;
		case states.moveBack:
			updateState = MoveBack_Update;
			MoveBack_Start();
			break;
		case states.triggerSkill:
			updateState = triggerSkill_Update;
			triggerSkill_Start();
			break;
		case states.getHit:
			updateState = getHit_Update;
			getHit_Start ();
			break;
		case states.guard:
			updateState = guard_Update;
			guard_Start ();
			break;
		case states.blocked:
			updateState = blocked_Update;
			blocked_Start ();
			break;
		case states.dead:
			updateState = dead_Update;
			dead_Start ();
			break;
		}
	}



	private void ExitState () {
		switch (previousState) {

		case states.none:
			//
			break;

		case states.moveToTarget:
			MoveToTarget_Exit();
			break;
		case states.moveBack:
			MoveBack_Exit();
			break;
		case states.triggerSkill:
			hit_Exit ();
			break;
		case states.getHit:
			getHit_Exit ();
			break;
		case states.guard:
			guard_Exit ();
			break;
		case states.blocked:
			blocked_Exit ();
			break;
		case states.dead:
			dead_Exit ();
			break;
		}
	}
	#endregion

	#region fight states
	/// <summary>
	///  la machine à état est continuée dans le combat manager, au bout d'une seconde, donc là on la saute.
	/// </summary>

	int[] statusCount = new int[11];

	public delegate void OnSkillDelay (Fighter fighter);
	public OnSkillDelay onSkillDelay;

	void CheckStatus () {

		if ( animator.GetBool("uncounscious") ) {
			animator.SetBool ("uncounscious", false);
		}

		if ( HasStatus(Status.Cussed) ) {
			RemoveStatus (Status.Cussed);
		}

		if ( HasStatus(Status.Toasted) ) {
			RemoveStatus (Status.Toasted);
		}

		if ( HasStatus(Status.Protected) ) {
			RemoveStatus (Status.Protected);
		}

		if ( HasStatus(Status.Poisonned) ) {
			RemoveStatus (Status.Poisonned);
			Hurt (10);
		}

		if ( HasStatus(Status.Jagged) ) {
			RemoveStatus (Status.Jagged);
			Heal (10);
		}

		if ( HasStatus(Status.Enraged) ) {
			RemoveStatus (Status.Enraged);
		}

//		if ( HasStatus(Status.PreparingAttack) ) {
//
//			if (onSkillDelay != null) {
//				onSkillDelay (this);
//			}
//
//			RemoveStatus (Status.PreparingAttack);
//			//
//		}
	}

	public delegate void OnAddStatus (Status status, int count);
	public OnAddStatus onAddStatus;
	public void AddStatus (Status status) {
		AddStatus (status, 1);
	}

	public void AddStatus (Status status, int count) {

		switch (status) {
		case Status.KnockedOut:
			animator.SetBool ("uncounscious", true);
			break;
		default:
			break;
		}

		combatFeedback.Display (status, Color.white);

		statusCount[(int)status] = count;
		statusCount [(int)status] = Mathf.Clamp (statusCount [(int)status], 0, 10);

        statusGroup.HandleOnAddStatus(status, statusCount[(int)status]);

		if (onAddStatus != null)
			onAddStatus (status, statusCount[(int)status]);
	}

	public delegate void OnRemoveStatus (Status status, int count);
	public OnRemoveStatus onRemoveStatus;

	public void RemoveStatus (Status status) {
		RemoveStatus (status, 1);
	}

	public void RemoveStatus (Status status, int valueToRemove) {

        if (statusCount[(int)status] == 0)
        {
            return;
        }

		statusCount[(int)status] -= valueToRemove;
		statusCount [(int)status] = Mathf.Clamp (statusCount [(int)status], 0, 10);

        statusGroup.HandleOnRemoveStatus(status, statusCount[(int)status]);

		if (onRemoveStatus != null)
			onRemoveStatus ( status, statusCount[(int)status] );
	}

	public bool HasStatus ( Status status ) {

		return statusCount [(int)status] > 0;
	}

	public enum Status {

		KnockedOut,

		PreparingAttack,

		Enraged,

		Jagged,

		Poisonned,

		Provoking,

//		Parrying,

		Protected,

		Toasted,

		BearTrapped,

		Cussed,

		None

	}
	#endregion
}
