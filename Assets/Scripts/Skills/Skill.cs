using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {

	public Fighter fighter;
	public Fighter preferedTarget;

	public string skillName = "";
	public string description = "";
	public int energyCost = 5;
	public int initCharge = 0;

    public int value = 10;

	public bool hasTarget = false;
	public bool goToTarget = true;
	public bool canTargetSelf = true;
	public TargetType targetType;
	public int priority = 0;

    public float timeToMoveBack = 1f;

    public bool triggerOnAnim = false;

	public Job linkedJob;

	public enum TargetType {
		Other,
		Self
	}

	public Type type;

    public virtual void Start()
    {
        
    }

    // first step of the skill / when it is pressed / chosen
    #region step 1
    public virtual void Trigger (Fighter fighter) {

        SkillManager.Instance.currentSkill = this;

		this.fighter = fighter;

        if (triggerOnAnim)
        {
            fighter.iconVisual.bodyVisual.onApplyEffect = null;
            fighter.iconVisual.bodyVisual.onApplyEffect += ApplyEffect_Anim;
        }
        else
        {
            Invoke("HandleOnApplyEffect", 0.5f);
        }

		if (hasTarget) {
			SetTarget ();
		} else {
			SkipTarget ();
		}

	}
    #endregion

    #region select target
    /// <summary>
    /// no target
    /// </summary>
    void SkipTarget()
    {
        if (CombatManager.Instance.GetCurrentFighter.crewMember.side == Crews.Side.Enemy)
        {
            CombatManager.Instance.ChangeState(CombatManager.States.EnemyAction);
        }
        else
        {
            CombatManager.Instance.ChangeState(CombatManager.States.PlayerAction);
        }

        InvokeSkill();
    }

    /// <summary>
    /// has target
    /// </summary>
    void SetTarget()
    {
        if (CombatManager.Instance.GetCurrentFighter.crewMember.side == Crews.Side.Enemy)
        {
            CombatManager.Instance.GoToTargetSelection(Crews.Side.Enemy, this);
        }
        else
        {
            CombatManager.Instance.GoToTargetSelection(Crews.Side.Player, this);
        }

        CombatManager.Instance.onChangeState += HandleOnChangeState;
    }

    void HandleOnChangeState(CombatManager.States currState, CombatManager.States prevState)
    {
        CombatManager.Instance.onChangeState -= HandleOnChangeState;

        if (currState == CombatManager.States.PlayerAction || currState == CombatManager.States.EnemyAction)
        {
            OnSetTarget();
        }
    }

    public virtual void OnSetTarget()
    {
        //if ( animationType == SkillManager.AnimationType.CloseAttack && goToTarget) {
        if (goToTarget)
        {
            fighter.onReachTarget += HandleOnReachTarget;

            TriggerFighterMoveToTarget();
        }
        else
        {
            InvokeSkill();
        }
    }

    public virtual void TriggerFighterMoveToTarget()
    {
        fighter.ChangeState(Fighter.states.moveToTarget);
    }

    public virtual void HandleOnReachTarget()
    {
        fighter.onReachTarget -= HandleOnReachTarget;
        InvokeSkill();
    }
    #endregion

    #region step 2
    // start of animation
    public virtual void InvokeSkill () {

		UseEnergy ();

		fighter.ChangeState (Fighter.states.triggerSkill);

		StartAnimation ();
	}
    #endregion

    #region animations
    public virtual void StartAnimation()
    {

    }
    public virtual void AnimationEvent_1()
    {

    }
    public virtual void AnimationEvent_2()
    {

    }
    #endregion

    /// <summary>
    /// Triggers the skill.
    /// </summary>
    #region effects
    void ApplyEffect_Anim()
    {
        HandleOnApplyEffect();

    }
    public virtual void HandleOnApplyEffect ()
	{

        if (goToTarget) {
            Invoke("InvokeMoveBack", timeToMoveBack);
        }
	}
    #endregion

    #region come back
    void InvokeMoveBack()  {

        if (fighter.TargetFighter != null && fighter.TargetFighter.HasStatus(Fighter.Status.BearTrapped))
        {
            fighter.TargetFighter.RemoveStatus(Fighter.Status.BearTrapped, 1);
            fighter.combatFeedback.Display("TRAPPED !", Color.red);

            fighter.TargetFighter.iconVisual.bearTrap_Transform.GetComponent<Animator>().SetBool("opened", false);
            Invoke("HideBearTrap", timeToMoveBack);

            SoundManager.Instance.PlaySound("beartrap_close");

            int damage = SkillManager.getSkill(Skill.Type.BearTrap).value;
            fighter.Hurt(damage);

            if (fighter.killed && fighter.crewMember.side == Crews.Side.Player)
            {
                fighter.EndTurn();
                CombatManager.Instance.NextTurn();
            }
            else
            {
                Invoke("InvokeMoveBack", timeToMoveBack);
            }
            return;
        }


        fighter.ChangeState(Fighter.states.moveBack);
    }
    void HideBearTrap()
    {
        fighter.TargetFighter.iconVisual.bearTrap_Transform.gameObject.SetActive(false);
    }
    #endregion

    public virtual void ContinueSkill()
    {

    }

    /// <summary>
    /// end skill
    /// </summary>
    #region end skill
    public virtual void EndSkill () {

		if (goToTarget) {
			Invoke ("EndSkillDelay",fighter.moveBackDuration + 1f);
		} else {
			EndSkillDelay ();
		}
	}

	public virtual void EndSkillDelay () {

        if (fighter.crewMember.CanUseSkills() == false)
        {
            fighter.EndTurn();
            CombatManager.Instance.NextTurn();
            return;
        }

        if (fighter.crewMember.side == Crews.Side.Player) {
			CombatManager.Instance.ChangeState (CombatManager.States.PlayerActionChoice);

		} else {
			CombatManager.Instance.ChangeState (CombatManager.States.EnemyActionChoice);
		}

	}
    #endregion

    #region energys
    void UseEnergy()
    {
        fighter.crewMember.energy -= energyCost;

        Skill skill = CombatManager.Instance.GetCurrentFighter.crewMember.GetSkill(type);

        if (skill != null)
        {
            int skillIndex = GetSkillIndex(fighter.crewMember);
            fighter.crewMember.charges[skillIndex] = initCharge;
        }
    }
    #endregion

    public virtual bool MeetsRestrictions ( CrewMember member ) {

		if ( canTargetSelf == false && targetType == TargetType.Self ) {
			return Crews.enemyCrew.CrewMembers.Count > 1;
		}

		return true;
	}
    

    public virtual bool MeetsConditions (CrewMember member) {

		if (member.energy < energyCost ) {
			return false;
		}

		if (member.charges [GetSkillIndex (member)] > 0) {
			return  false;
		}

		return true;
	}

	public int GetSkillIndex ( CrewMember member )  {
		
		int skillIndex = member.DefaultSkills.FindIndex (x => x.type == type);

		if (skillIndex < 0) {
			skillIndex = member.SpecialSkills.FindIndex (x => x.type == type) + member.DefaultSkills.Count;
		}

		if (skillIndex < 0) {
			Debug.LogError ("pas trouvé l'index de " + skillName);
		}

		return skillIndex;
	}

	// ENUM //
	public enum Type {

		// NORMAL
		CloseAttack,
		DistanceAttack,
        Defend,
		Flee,
        SkipTurn,

		// BRUTE
		Cosh,
		Wallop,
		Leap,
		Fury,

		// SURGEON
		BistouryBlow,
		Jag,
		RatPoison,
		RhumRound,

		// COOK
		HelpMate,
		Goad,
		ToastUp,
		PledgeOfFeast,

		// FILIBUSTER
		HeadShot,
		GrapeShot,
		BearTrap,
		Dynamite,

		// PLAYER
		HeadsOrTails,
		Robbery,
		Cuss,
		DoubleTalk, 

	}
}
