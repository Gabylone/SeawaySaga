using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour {

	public Fighter fighter;
	public Fighter preferedTarget;

	public string skillName = "";
	public string description = "";
	public int energyCost = 5;
	public float animationDelay = 0.6f;
	public int initCharge = 0;

	public bool playAnim = true;
	public SkillManager.AnimationType animationType;

	public bool hasTarget = false;
	public bool goToTarget = true;
	public bool canTargetSelf = true;
	public TargetType targetType;
	public int priority = 0;

    public float timeToMoveBack = 1f;

	public Job linkedJob;

	public enum TargetType {
		Other,
		Self
	}

	public Type type;

	void UseEnergy () {

        fighter.crewMember.energy -= energyCost;

        Skill skill = CombatManager.Instance.currentFighter.crewMember.GetSkill(type);

		if (skill != null) {
			fighter.crewMember.charges [GetSkillIndex(fighter.crewMember)] = initCharge;
		}
	}

	public virtual void Trigger (Fighter fighter) {

		this.fighter = fighter;

        fighter.iconVisual.bodyVisual.onApplyEffect = null;
        fighter.iconVisual.bodyVisual.onApplyEffect += HandleOnApplyEffect;

		if (hasTarget) {
			SetTarget ();
		} else {
			SkipTarget ();
		}

	}

	/// <summary>
	/// no target
	/// </summary>
	void SkipTarget (){

		if (CombatManager.Instance.currentFighter.crewMember.side == Crews.Side.Enemy) { 
			CombatManager.Instance.ChangeState (CombatManager.States.EnemyAction);
		} else {
			CombatManager.Instance.ChangeState (CombatManager.States.PlayerAction);
		}

		InvokeSkill ();
	}

	/// <summary>
	/// target
	/// </summary>
	void SetTarget ()
	{
		if (CombatManager.Instance.currentFighter.crewMember.side == Crews.Side.Enemy) { 

			CombatManager.Instance.GoToTargetSelection (Crews.Side.Enemy, this);

		} else {
			
			CombatManager.Instance.GoToTargetSelection (Crews.Side.Player, this);

		}

		CombatManager.Instance.onChangeState += HandleOnChangeState;
	}



	void HandleOnChangeState (CombatManager.States currState, CombatManager.States prevState)
	{
		CombatManager.Instance.onChangeState -= HandleOnChangeState;

		if ( currState == CombatManager.States.PlayerAction || currState == CombatManager.States.EnemyAction ) {

			OnSetTarget ();

		}
	}

	public virtual void OnSetTarget () {

		if ( animationType == SkillManager.AnimationType.CloseAttack && goToTarget) {
			fighter.onReachTarget += HandleOnReachTarget;
			fighter.ChangeState (Fighter.states.moveToTarget);
		} else {
			InvokeSkill ();
		}
	}

	void HandleOnReachTarget ()
	{
		fighter.onReachTarget -= HandleOnReachTarget;
		InvokeSkill ();
	}

	public virtual void InvokeSkill () {

		UseEnergy ();

		fighter.ChangeState (Fighter.states.triggerSkill);

		StartAnimation ();
	}

	public virtual void StartAnimation () {

		if (!playAnim)
			return;
		//
	}

	/// <summary>
	/// Triggers the skill.
	/// </summary>
	public virtual void HandleOnApplyEffect ()
	{
		if (goToTarget) {

			fighter.TargetFighter.CheckContact (fighter);

			Invoke ("InvokeMoveBack",timeToMoveBack);
		}
	}

	void InvokeMoveBack()  {
		fighter.ChangeState (Fighter.states.moveBack);
	}

	/// <summary>
	/// end skill
	/// </summary>
	public virtual void EndSkill () {

		if (goToTarget) {
			Invoke ("EndSkillDelay",fighter.moveBackDuration + 1f);
		} else {
			EndSkillDelay ();
		}
	}

	void EndSkillDelay () {

		if (fighter.crewMember.side == Crews.Side.Player) {

			if ( fighter.crewMember.CanUseSkills() == false ) {
                fighter.EndTurn();
				CombatManager.Instance.NextTurn ();
				return;
			}

			CombatManager.Instance.ChangeState (CombatManager.States.PlayerActionChoice);

		} else {
			CombatManager.Instance.ChangeState (CombatManager.States.EnemyActionChoice);
		}

	}

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
			skillIndex = member.SpecialSkills.FindIndex (x => x.type == type) + 3;
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
