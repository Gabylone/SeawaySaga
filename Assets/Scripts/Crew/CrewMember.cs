using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CrewMember {

	// SELECTED MEMBER IN INVENTORY
	static CrewMember selectedMember;
	public static CrewMember GetSelectedMember {
		get {

			if (selectedMember == null) {
                if ( Crews.playerCrew == null)
                {
                    return null;
                }
				if (Crews.playerCrew.CrewMembers.Count != 0) {
					return Crews.playerCrew.captain;
				} else {
					return Crews.playerCrew.captain;
					//Debug.LogError("no captain, merde chier");
                    return null;
				}
			}

			return selectedMember;
		}
	}

	public static void SetSelectedMember (CrewMember crewMember) {

        selectedMember = crewMember;

	}

	// STATS
	public int maxLevel
	{
		get
		{
			return MapGenerator.mapParameters.endFightLevel;
        }
	}

	// EXPERIENCE
	public int xpToLevelUp {
		get {
			return 100 + (5 * (Level-1) );
		}
	}

    // COMPONENTS
    public Crews.Side side;
	private Member memberID;

    // HUNGER
    public float minHungerDamage = 5;
    public float maxHungerDamage = 15;
    public int hungerDamage
    {
        get
        {
            float l = (float)GetStat(Stat.Constitution) / 6;

            float damage = Mathf.Lerp(maxHungerDamage, minHungerDamage , l);
            return (int)damage;
        }
    }

	// JOB
	public Job job {
		get {
			return memberID.job;
		}
	}

	public List<Skill> DefaultSkills = new List<Skill> ();
	public List<Skill> SpecialSkills = new List<Skill>();
	public int[] charges;
	public void ResetSkills() {

		DefaultSkills.Clear ();
		DefaultSkills.Add (SkillManager.GetDefaultAttackSkill (this));
		DefaultSkills.Add (SkillManager.getSkill(Skill.Type.Defend));
		DefaultSkills.Add (SkillManager.getSkill(Skill.Type.Flee));
        DefaultSkills.Add (SkillManager.getSkill(Skill.Type.SkipTurn));

		SpecialSkills.Clear ();

		foreach (var item in memberID.specialSkillsIndexes) {
			Skill newSkill = new Skill ();
			newSkill = SkillManager.skills [item];
			SpecialSkills.Add (newSkill);
		}

		charges = new int[8] { 0, 0, 0, 0 , 0 , 0 , 0 , 0};
//		foreach (var item in SpecialSkills) {
//			item.currentCharge = 0;
//		}
//
//		foreach (var item in DefaultSkills) {
//			item.currentCharge = 0;
//		}

	}

	public void AddSkill (Skill skill)
	{
		MemberID.specialSkillsIndexes.Add ( SkillManager.getSkillIndex(skill) );
		ResetSkills ();
	}

	public Skill GetSkill (Skill.Type type)
	{
		Skill skill = SpecialSkills.Find(x => x.type == type);

		if ( skill == null)
			skill = DefaultSkills.Find(x => x.type == type);

		return skill;
	}

	/// <summary>
	/// energy
	/// </summary>
	public int energy = 0;
	public int EnergyPerTurn {
        get {
            return 5 + GetStat(Stat.Constitution);
        }
    }
	public int MaxEnergy {
        get {
			return 12;
        }
    }

	public void AddEnergy (int _energy)
	{
		energy += _energy;
		energy = Mathf.Clamp (energy, 0, MaxEnergy);
	}

	// ICON
	public MemberIcon memberIcon;

	// CONSTRUCTOR
	public CrewMember (Member _memberID, Crews.Side _side , MemberIcon memberIcon )
	{
		memberID = _memberID;

		side = _side;

		this.memberIcon = memberIcon;

		this.memberIcon.SetMember (this);

		ResetSkills ();
	}

	#region level
	public void AddXP ( int _xp ) {

		if ( Level == maxLevel ) {
			return;
		}

		CurrentXp += _xp;

		if ( CurrentXp >= xpToLevelUp ) {
			LevelUp ();
		}

	}

	public delegate void OnLevelUpStat (CrewMember member);
	public OnLevelUpStat onLevelUpStat;
	public void HandleOnLevelUpStat (Stat stat)
	{
		int newValue = GetStat (stat) + 1;

		SetStat(stat, newValue);

		--SkillPoints;

		if (onLevelUpStat != null) {
			
			onLevelUpStat (this);
		}

	}

	public delegate void OnLevelUp (CrewMember member);
	public OnLevelUp onLevelUp;
    public void LevelUp()
    {
        ++Level;

        CurrentXp = CurrentXp - xpToLevelUp;

        SkillPoints += 2;

        if (Level == maxLevel)
        {
            CurrentXp = 0;
        }

        DisplayCrewMemberLevelUp.Instance.Display(this);

        if (onLevelUp != null)
        {
            onLevelUp(this);
        }
    }
	public delegate void OnWrongLevel();
	public static OnWrongLevel onWrongLevel;
	public bool CheckLevel ( int lvl ) {

		if (lvl > Level) {
//			LootManager.Instance.OnWrontLevel ();
			if (onWrongLevel != null)
				onWrongLevel ();
			return false;
		}

		return true;

	}
    #endregion

    #region in inventory
    public void ShowInInventory()
    {
        SetSelectedMember(this);

        foreach (var item in Crews.playerCrew.CrewMembers)
        {
            if (item != this)
            {
                item.Icon.MoveToPoint(Crews.PlacingType.Portraits);
            }
        }

        Icon.MoveToPoint(Crews.PlacingType.Inventory);

        if ( InGameMenu.Instance.onDisplayCrewMember != null)
        {
            InGameMenu.Instance.onDisplayCrewMember(this);
        }

    }
    #endregion

    #region health
    public float GetDamage ( float incomingAttack ) {
		//		
		
		float maxAttack = 124f;

		incomingAttack = Mathf.Clamp (incomingAttack, 0, maxAttack);

		float dif = ((float)Defense - incomingAttack);

		float lerp = 1 - (dif + maxAttack) / (maxAttack*2);

		float maxDamage = 40f;
		float minDamage = 2f;

		float damageTaken = minDamage + ((maxDamage - minDamage) * lerp);

        //float damageTaken = maxAttack / hits;
		//damageTaken = Mathf.Clamp(damageTaken, 1f, maxAttack);

		if ( side == Crews.Side.Player) {
            //
            damageTaken = Crews.Instance.reducedDamage * damageTaken / 100;
        }

		int roundedDamage = Mathf.RoundToInt(damageTaken);

			/*//Debug.Log ("attack : " + incomingAttack);
			//Debug.Log ("defense : " + Defense);
			//Debug.Log ("dif : " + dif);
			//Debug.Log ("lerp : " + lerp);
			//Debug.Log ("damageTaken : " + damageTaken);*/

		return roundedDamage;
	}
	public void AddHealth (float f) {
		Health += Mathf.RoundToInt(f);
	}
	public void RemoveHealth (float f) {
		Health -= Mathf.RoundToInt(f);
	}

	public void Kill () {

        Crews.Instance.KillMember(this);

        //if (this == selectedMember)
        ////Debug.Log ("le membre mourrant est bel et bien le séléctionné");

        SetSelectedMember(null);

		Crews.getCrew(side).RemoveMember (this);

	}
    #endregion

    #region states
    public delegate void OnAddHunger();
    public OnAddHunger onAddHunger;

	public void AddHunger () {

		++CurrentHunger;

		if ( CurrentHunger >= MaxHunger) {

			RemoveHealth(hungerDamage);

			if (Health <= 0) {
                DieOfHunger();
			}

		}

        if (onAddHunger != null)
        {
            onAddHunger();
        }

	}

    public void DieOfHunger()
    {
        if (Health - hungerDamage <= 0)
        {
            MessageDisplay.Instance.onValidate += HandleOnValidate;
            MessageDisplay.Instance.Display("After " + daysOnBoard + " days on board, " + MemberName + " tragically dies of hunger.");
        }

        SoundManager.Instance.PlaySound("Defeat");
        SoundManager.Instance.PlayRandomSound("ui_deny");
        SoundManager.Instance.PlaySound("ui_deny");

        Kill();
    }

    void HandleOnValidate()
    {
        if ( Crews.playerCrew.CrewMembers.Count == 0)
        {
            GameManager.Instance.BackToMenu();
        }
    }

    void HandleOnValidateDelay()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    public bool HasHunger()
    {
        return CurrentHunger > 0;
    }

    public bool Hungry()
    {
        return CurrentHunger >= MaxHunger;
    }

	public int daysOnBoard {
		get {
			return memberID.daysOnBoard;
		}
		set {
			memberID.daysOnBoard = value;
		}
	}
    #endregion

    #region parameters
    public delegate void OnChangeHealth();
    public OnChangeHealth onChangeHealth;
    
    public bool HasMaxHealth()
    {
        return Health == memberID.maxHealth;
    }

	public int Health {
		get {
			return memberID.health;
		}
		set
        {
			memberID.health = Mathf.Clamp (value , 0 , memberID.maxHealth);

            if ( onChangeHealth != null)
            {
                onChangeHealth();
            }

//			if (memberID.health <= 0)
//				Kill ();
		}
	}

	public string MemberName {
		get {
			return memberID.Name;
		}
	}

	public bool Male {
        get { return false; }
	}
	#endregion

	#region stats
	public int Attack {
		get {

			int i = GetStat(Stat.Strenght) * 4;

            if (GetEquipment (EquipmentPart.Weapon) != null) {

				if ( GetEquipment(EquipmentPart.Weapon).weaponType == Item.WeaponType.Distance ){
					i = GetStat (Stat.Dexterity) * 4;
				}
				return i + GetEquipment (EquipmentPart.Weapon).value;
			}

			return i;
		}
	}

	public int Defense {
		get {

			int i = GetStat(Stat.Constitution) * 4;

			if (GetEquipment (EquipmentPart.Clothes) != null)
				return i + GetEquipment (EquipmentPart.Clothes).value;

			return i;
		}
	}

	public int GetStat (Stat stat) {
		return memberID.stats [(int)stat];
	}

	public void SetStat (Stat stat, int value) {
		memberID.stats [(int)stat] = value;
	}

    public bool HasMeleeWepon()
    {
        return GetEquipment(CrewMember.EquipmentPart.Weapon) == null || GetEquipment(CrewMember.EquipmentPart.Weapon).weaponType != Item.WeaponType.Distance;
    }
	public bool HasDistanceWeapon()
	{
		return GetEquipment(CrewMember.EquipmentPart.Weapon) == null || GetEquipment(CrewMember.EquipmentPart.Weapon).weaponType == Item.WeaponType.Distance;
	}
	#endregion

	#region icon
	public MemberIcon Icon {
		get {
			return memberIcon;
		}
	}
	public int GetIndex {
		get {
            return Crews.getCrew(side).CrewMembers.FindIndex(x => x.memberID.SameAs(this.memberID));
		}
	}
	#endregion

	#region properties
	public Member MemberID {
		get {
			return memberID;
		}
	}
	#endregion

	#region equipment
	public enum EquipmentPart {
		Weapon,
		Clothes,

		None
	}

	public void RemoveEquipment ( EquipmentPart part ) {
		switch (part) {
		case EquipmentPart.Weapon:
			memberID.equipedWeapon = null;
			break;
		case EquipmentPart.Clothes:
			memberID.equipedCloth = null;
			break;
		}
	}

	#region equipment
	public void SetEquipment (Item item ) {

		switch (item.EquipmentPart) {
		case EquipmentPart.Weapon:
			memberID.equipedWeapon = item;
			break;
		case EquipmentPart.Clothes:
			memberID.equipedCloth = item;
			break;
		}
	}

	public Item GetEquipment ( EquipmentPart part ) {
		switch (part) {
		case EquipmentPart.Weapon:
			return memberID.equipedWeapon;
		case EquipmentPart.Clothes:
			return memberID.equipedCloth;
		default:
			return memberID.equipedWeapon;
		}
	}
	#endregion

	public bool CanUseSkills () {
		
		foreach (var item in DefaultSkills) {

			if (item.type == Skill.Type.SkipTurn)
				continue;

			if (energy >= item.energyCost) {
                return true;
			}

		}

        // pourquoi 3 ? on sait pas mais TOUT REPOSE SUR CA
		int a = 3;
		foreach (var item in SpecialSkills) {
			
			if (energy >= item.energyCost && charges[a] == 0) {
				return true;
			}

			++a;
		}

		return false;

	}
	#endregion

	#region states properties
	public int CurrentHunger {
		get {
			return memberID.currentHunger;
		}
		set {
			memberID.currentHunger = Mathf.Clamp (value, 0, MaxHunger);
		}
	}
    public int MaxHunger
    {
        get
        {
            float lerp = (float)GetStat(Stat.Constitution) / 6;

            int maxHunger = (int)Mathf.Lerp(Crews.maxHunger_MinConstitution, Crews.maxHunger_MaxConstitution, lerp);

            ////Debug.Log("max hunger of " + MemberName + " : " + maxHunger);

            return maxHunger;
        }
    }
	#endregion

	#region level
	public int Level {
		get {
			return memberID.Lvl;
		}
		set {
			memberID.Lvl = value;
		}
	}
	public int CurrentXp {
		get {
			return memberID.xp;
		}
		set {
			memberID.xp = value;
		}
	}

	public int SkillPoints {
		get {
			return memberID.skillPoints;
		}
		set {
			memberID.skillPoints = value;
		}
	}
	#endregion

	public Color GetLevelColor () {

//		if (Crews.playerCrew.captain == null)
//			return;
//
		float dif = Level - Crews.playerCrew.captain.Level;

		float l = ( (dif+9) / 18f);

		if ( l < 0.5f )
			return Color.Lerp ( Color.green , Color.white , l*2 );
		else
			return Color.Lerp ( Color.white , Color.red , (l-0.5f) * 2f );

	}
}

public enum Stat {
	Strenght,
	Dexterity,
	Trickery,
	Constitution
}

public enum Job {
	Brute,
	Surgeon,
	Cook,
    Cannoneer,
	Gambler,

	None,
}