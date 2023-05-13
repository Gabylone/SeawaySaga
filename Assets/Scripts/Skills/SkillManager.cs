using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    public static SkillManager Instance;

	public static Skill[] skills;

	public Sprite[] skillSprites;
	public Sprite[] statusSprites;
	public static Sprite[] jobSprites;
    public Sprite[] statSprites;
    public Color[] statColors;
	public Color[] skillColors;

    public delegate void OnLevelUpStat();
    public OnLevelUpStat onLevelUpStat;

	public static string[] jobNames = new string[5] {
		"Brute",
		"Medic",
		"Cook",
		"Cannoneer",
		"Crook"
	};

	public TextAsset skillData;

    public Skill currentSkill;

    private void Awake()
    {
        Instance = this;

        SkillButton_Inventory.onUnlockSkill = null;
        StatusFeedback.onTouchStatusFeedback = null;
    }

    // Use this for initialization
    void Start () {
		
		skills = GetComponentsInChildren<Skill> ();

		string[] rows = skillData.text.Split ('\n');

		int skillIndex = 0;
		int rowIndex = 2;
		foreach (var item in skills) {

			string[] cells = rows [rowIndex].Split (';');

			item.type = (Skill.Type)skillIndex;

			if (cells.Length <= 1)
				print (item.type);

			item.skillName = cells [1];
			item.energyCost = int.Parse ( cells[3] );
			item.initCharge = int.Parse ( cells[4] );
			item.priority = int.Parse ( cells[5] );
			item.description = cells [6];

			++skillIndex;
			++rowIndex;
		}

		jobSprites = Resources.LoadAll<Sprite> ("Graph/JobSprites");


	}

	public static Skill getSkill ( Skill.Type type ) {

		Skill skill = System.Array.Find (skills, x => x.type == type);

		if (skill == null)
			print ("getting skill : " + type + " is null");

		return skill;
	}

	public static int getSkillIndex ( Skill skill ) {

		int skillIndex = System.Array.FindIndex (skills, x => x.type == skill.type);

		return skillIndex;
	}

	public static bool CanUseSkill ( int energy ) {

		foreach (var item in skills) {
			if (energy >= item.energyCost) {
				return true;
			}
		}

		return false;

	}

	public static List<Skill> getJobSkills ( Job job ) {

		List<Skill> jobSkills = new List<Skill> ();

		foreach (var skill in skills) {

			if (skill.linkedJob == job) {
				
				jobSkills.Add (skill);

			}

		}

		if ( jobSkills.Count == 0 )
			print ("skills were not found .... !!!! for job : " + job.ToString());

		return jobSkills;

	}

	public static Skill GetDefaultAttackSkill (CrewMember member) {
		if ( member.HasMeleeWepon() )
			return getSkill(Skill.Type.CloseAttack);
        else
            return getSkill (Skill.Type.DistanceAttack);
	}

	public static Skill RandomSkill ( CrewMember member ) {

		List<Skill> fittingSkills = new List<Skill> ();

		int priority = 0;

		List<Skill> memberSkills = new List<Skill>();
		foreach (var item in member.DefaultSkills) {
//			print ("all skills : " + item.name);
			memberSkills.Add (item);
		}
		foreach (var item in member.SpecialSkills) {
//			print ("all skills : " + item.name);
			memberSkills.Add (item);
		}

		// dans tous les skills du membre
		foreach (var item in memberSkills) {

			if ( item.MeetsConditions(member) == false ) {
//				print (item.name + " ne rempli pas les conditions");
				continue;
			}

			if ( item.MeetsRestrictions(member) == false ) {
//				print (item.name + " ne rempli pas les resstrictions");
				continue;
			}

//			print (item.name + " rempli les conditions");

			if (item.priority == priority) {
//				print (item.name + " a une priorité égale (prio : " + item.priority + ")");
				fittingSkills.Add (item);
			}

			if ( item.priority > priority ) {

//				print (item.name + " a une priorité supérieure");
				fittingSkills.Clear ();
				fittingSkills.Add (item);
				priority = item.priority;

			}


		}

		int skillIndex = Random.Range(0,fittingSkills.Count);

        if ( fittingSkills.Count == 0)
        {
            //Debug.Log("no fitting skills : skipping turn");

            return getSkill(Skill.Type.SkipTurn);
        }

		Skill skill = fittingSkills[skillIndex];

        //		foreach (var item in fittingSkills) {
        //			print ("skill possibles : " + item.name);
        //		}
        //
        //		print(" skill choisi : "+ skill.name);

		return skill;
	}

	public enum AnimationType {

		CloseAttack,
		Shoot,

	}
}
