using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Fighter : MonoBehaviour {

	public bool killed = false;
	public bool escaped = false;
    public bool dodged = false;

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

    public Transform dialogueAnchor;

    public StatusGroup statusGroup;

    public IconVisual iconVisual;
    public CanvasGroup canvasGroup;

    public Transform GetTransform;

	[Header ("Move")]
	public float moveToTargetDuration = 0.79f;
	public float moveBackDuration = 0.79f;

	public enum Direction {
		Right,
		Left
	}

    public Card card;

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
	// get hit
	[Header ("Get Hit")]
	public float getHit_Duration = 0.3f;
    public float getHit_ScaleDuration = 0.2f;
    public float getHit_ScaleAmount = 1.2f;

    public delegate void OnGetHit();
    public OnGetHit onGetHit;

    // blocked
    [Header ("Blocked")]
	[SerializeField]
	private float blocked_Duration = 0.5f;

	[Header("Dodge")]
	[SerializeField]
	private float maxDodgeChance = 20;

	private Vector3 initPos = Vector3.zero;

	[Header ("Target")]
	private Fighter targetFighter;
	public float stopDistance = 1f;

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

        GetTransform = GetComponent<Transform>();

        card = GetTransform.parent.GetComponentInChildren<Card>();
        card.Init ();

		dir = direction == Direction.Left ? 1 : -1;

		initPos = GetTransform.position;

		Hide ();

	}

    // Update is called once per frame
    void FixedUpdate()
    {

        if (updateState != null)
            updateState();

        timeInState += Time.deltaTime;

        


    }

	#region initalization
	public delegate void OnInit ();
	public OnInit onReset;
	public void Reset ( CrewMember crewMember, int id )
	{
		this.crewMember = crewMember;

        crewMember.energy = 0;

        Show();

		ChangeState (states.none);

		killed = false;
		escaped = false;

		// animation
		GetTransform.position = initPos;

		onSkillDelay = null;

		for (int i = 0; i < statusCount.Length; i++) {
			statusCount [i] = 0;
		}

        // reset animations
        animator.SetBool("dead", false);
        animator.SetBool("preparingToLeap", false);

        // reset visuals
        iconVisual.InitVisual(crewMember.MemberID);
        iconVisual.RemoveDeadEyes();
        iconVisual.ResetSkinColor();
        iconVisual.RemoveMadFace();
        iconVisual.ResetEffects();

        // energy & status
        crewMember.energy = 0;

        // event
        if ( onReset != null )
			onReset ();
	}

	public delegate void OnSetTurn ();
	public OnSetTurn onSetTurn;
	public void SetTurn () {

		Tween.Bounce (GetTransform);

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

            SoundManager.Instance.PlaySound("Wake Up");
            SoundManager.  Instance.PlaySound("Tribal 01");

			return;
			//
		}

		CheckStatus ();

        if ( killed || escaped)
        {
            return;
        }

		if (HasStatus(Status.Enraged))
        {
            crewMember.AddEnergy(10);
        }
        else
        {
            crewMember.AddEnergy(6);
        }

        for (int i = 0; i < crewMember.charges.Length; i++) {
			if ( crewMember.charges [i] > 0 )
				crewMember.charges [i] -= 1;
		}

		CombatManager.Instance.StartActions ();

		if (onSetTurn != null) {
			onSetTurn (); 
		}

        SoundManager.Instance.PlaySound("New Turn");

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
			

		CombatManager.Instance.GetCurrentFighter.TargetFighter = this;
//		targetFighter = CombatManager.Instance.currentFighter;

		Tween.Bounce (GetTransform);

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

        CancelInvoke("Hide");
        Invoke("Hide", 0.5f);
    }

	void Show () {

        canvasGroup.alpha = 1f;

		ui_Group.SetActive (true);
		gameObject.SetActive (true);
	}

	public virtual void Die () {

        SoundManager.Instance.PlaySound("Death");

			// self
		ChangeState (states.dead);

        iconVisual.SetDeadEyes();

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

        Vector3 targetPos = TargetFighter.GetTransform.position + GetTargetDirection() * stopDistance;

        GetTransform.DOMove(targetPos, moveToTargetDuration);

	}

    public Vector3 GetTargetDirection()
    {
        Vector3 dir;

        if (crewMember.side == TargetFighter.crewMember.side)
        {
            if (TargetFighter.crewMember.side == Crews.Side.Enemy)
            {
                dir = new Vector3(1f, 0f, 0f);
            }
            else
            {
                dir = new Vector3(-1f, 0f, 0f);
            }
        }
        else
        {
            if (TargetFighter.crewMember.side == Crews.Side.Enemy)
            {
                dir = new Vector3(-1f, 0f, 0f);
            }
            else
            {
                dir = new Vector3(1, 0f, 0f);
            }
        }

        return dir;
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

        GetTransform.DOMove(initPos, moveBackDuration);

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

	public void Select () {


		if ( Pickable ) {
			CombatManager.Instance.ChoseTargetMember (this);
			Tween.Bounce (GetTransform);

            SoundManager.Instance.PlaySound( "button_heavy 01" );
			return;
		}

		if (CombatManager.Instance.GetCurrentFighter == this && CombatManager.Instance.GetCurrentFighter.crewMember.side == Crews.Side.Player ) {
			return;
        }

        card.HandleOnShowInfo();
	}

	public void Speak (string txt)
	{
		DialogueManager.Instance.SetDialogueTimed (txt, dialogueAnchor);
	}

	#region get hit
	public virtual void getHit_Start () {

	}
	public virtual void getHit_Update () {

		if (timeInState > getHit_Duration)
        {
            ChangeState(states.none);
        }
    }
	public virtual void getHit_Exit () {
		
        //iconVisual.RemoveDeadEyes();
	}

    void HitEffect()
    {
        animator.SetTrigger("getHit");

        iconVisual.TaintOnce(Color.red);

        iconVisual.hitEffect_Anim.gameObject.SetActive(true);
        iconVisual.hitEffect_Anim.SetTrigger("Trigger");
        iconVisual.hitEffect_Transform.position = GetTransform.position - Vector3.up * 3f + (Vector3)Random.insideUnitCircle * 1f;

        Tween.Bounce(GetTransform, getHit_ScaleDuration , getHit_ScaleAmount);
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

        if (HasStatus(Status.Parrying))
        {
            damage = damage * 0.5f;
            RemoveStatus(Status.Parrying);
        }

		damage = Mathf.Round (damage);

		return damage;
	}

	public void GetHit (Fighter otherFighter, float attack, float mult) {

		if (SucceedDodge() == true) {
            dodged = true;
		}

        dodged = false;

		float damage = GetDamage (otherFighter, attack,mult);

		if ( CombatManager.Instance.debugKill ) {
			damage = 200;
		}

        if (crewMember.Health - damage <= 0)
        {
            if (otherFighter.crewMember.side == Crews.Side.Player)
            {
                int xpPerMember = 25;

                otherFighter.crewMember.AddXP(xpPerMember);
                otherFighter.combatFeedback.Display("" + xpPerMember, Color.cyan);
            }
        }

        if (otherFighter.CriticalHit())
        {
            otherFighter.combatFeedback.Display("CRITICAL !", Color.red, 2f);
            Hurt(damage*2f);
        }
        else
        {
            Hurt(damage);
        }
    }

    public bool CriticalHit()
    {
        float l = (float)crewMember.GetStat(Stat.Trickery) / 6f;

        float maxChance = 20f;

        float chance = l * maxChance;

        return (Random.value*100f) < chance;
    }

	public void Hurt (float amount) {

        HitEffect();

        combatFeedback.Display(amount.ToString(), Color.red);
        crewMember.RemoveHealth(amount);

        SoundManager.Instance.PlayRandomSound("Blunt");

        if (onGetHit != null)
            onGetHit();

        if (crewMember.Health <= 0)
        {
            crewMember.Kill();
            Die();
        }

    }

	public void Heal (float amount) {

        SoundManager.Instance.PlayRandomSound("Magic");
        SoundManager.Instance.PlayRandomSound("Alchemy");
        SoundManager.Instance.PlayRandomSound("Potion");

        Tween.Bounce(GetTransform, getHit_ScaleDuration, getHit_ScaleAmount);

        crewMember.AddHealth (amount);

        iconVisual.healEffect_Anim.gameObject.SetActive(true);
        iconVisual.healEffect_Anim.SetTrigger("Trigger");

        iconVisual.TaintOnce(Color.green);

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
            Dodge();
            return true;
		}

		return false;

	}

    public bool SkippingTurn()
    {
        return 
            HasStatus(Status.KnockedOut)
            ||
            HasStatus(Status.PreparingAttack);
    }

    public void CancelDelayedAttack()
    {
        iconVisual.RemoveAimingEyes();
        iconVisual.RemoveMadFace();

        RemoveStatus(Status.PreparingAttack);
        onSkillDelay = null;
    }

    public void KnockOut()
    {
        SoundManager.Instance.PlayRandomSound("Blunt");
        SoundManager.Instance.PlayRandomSound("Punch");
        SoundManager.Instance.PlayRandomSound("slash");

        SoundManager.Instance.PlaySound("knockout");

        if (HasStatus(Status.PreparingAttack))
        {
            CancelDelayedAttack();
        }

        combatFeedback.Display("Knocked Out !", Color.red);

        AddStatus(Fighter.Status.KnockedOut);

        crewMember.energy = 0;
    }

    void Dodge()
    {
        animator.SetTrigger("dodge");
        combatFeedback.Display("Missed !", Color.red);

        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlaySound("Dodge");

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
		if ( HasStatus(Status.Cussed) ) {
			RemoveStatus (Status.Cussed);
		}

		if ( HasStatus(Status.Toasted) ) {
			RemoveStatus (Status.Toasted);
		}

		if ( HasStatus(Status.Protected) ) {
			RemoveStatus (Status.Protected);
		}

        if (HasStatus(Status.Parrying))
        {
            RemoveStatus(Status.Parrying);
        }

        if (HasStatus(Status.Poisonned))
        {
            RemoveStatus(Status.Poisonned);
            iconVisual.poisonEffect_Obj.SetActive(true);
            combatFeedback.Display("POISON !", Color.red);
            int damage = SkillManager.getSkill(Skill.Type.RatPoison).value;
            Hurt(damage);

            if (killed)
            {
                EndTurn();
                CombatManager.Instance.NextTurn();
                return;
            }
        }

		if ( HasStatus(Status.Jagged) ) {
			RemoveStatus (Status.Jagged);
			Heal (10);
		}

		if ( HasStatus(Status.Enraged) ) {
			RemoveStatus (Status.Enraged);
		}

	}

    public void AttachItemToHand (Transform _transform)
    {
        SoundManager.Instance.PlayRandomSound("Bag");

        _transform.SetParent(iconVisual.bodyVisual.itemAnchor);
        _transform.localPosition = Vector3.zero;
        _transform.up = iconVisual.bodyVisual.itemAnchor.up;
        _transform.gameObject.SetActive(true);
        Tween.Bounce(_transform);
    }

	public delegate void OnAddStatus (Status status, int count);
	public OnAddStatus onAddStatus;
	public void AddStatus (Status status) {
		AddStatus (status, 1);
	}

	public void AddStatus (Status status, int count) {

		switch (status) {
            case Status.Toasted:
                iconVisual.SetHappyFace();
                break;
            case Status.Cussed:
                iconVisual.SetSadFace();
                break;
            case Status.Protected:
                iconVisual.SetHappyFace();
                break;
            case Status.Poisonned:
                iconVisual.poisonPuddle_Obj.gameObject.SetActive(true);
                Tween.Bounce(iconVisual.poisonPuddle_Obj.transform);
                Color poisonedColor = Color.Lerp(iconVisual.GetColor(ApparenceType.skinColor), Color.green , 0.3f);
                iconVisual.OverrideSkinColor(poisonedColor);
                break;
            case Status.Parrying:
                animator.SetBool("defending", true);
                break;
            case Status.KnockedOut:
                iconVisual.SetDeadEyes();
			    animator.SetBool ("uncounscious", true);
			break;
            case Status.Enraged:
                iconVisual.OverrideSkinColor(Color.red);
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

        if ( statusCount[(int)status] == 0 ){
            switch (status)
            {
                case Status.PreparingAttack:
                    animator.SetBool("aiming", false);
                    animator.SetBool("preparingToLeap", false);
                    break;
                case Status.Toasted:
                    iconVisual.RemoveHappyFace();
                    break;
                case Status.Cussed:
                    iconVisual.RemoveSadFace();
                    break;
                case Status.Protected:
                    iconVisual.RemoveHappyFace();
                    break;

                case Status.Parrying:
                    animator.SetBool("defending", false);
                    break;
                case Status.Poisonned:
                    iconVisual.poisonEffect_Obj.SetActive(false);
                    iconVisual.poisonPuddle_Obj.SetActive(false);
                    iconVisual.ResetSkinColor();
                    break;
                case Status.KnockedOut:
                    iconVisual.RemoveDeadEyes();
                    animator.SetBool("uncounscious", false);
                    break;
                case Status.Enraged:
                    iconVisual.ResetSkinColor();
                    iconVisual.RemoveMadFace();
                    break;
                default:
                    break;
            }
        }
        

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

		Parrying,

		Protected,

		Toasted,

		BearTrapped,

		Cussed,

		None

	}
	#endregion
}