using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Member {

	public static int globalID = 0;
	public int id = 0;

	// name
	public string Name = "";

	public int health = 100;
	public int maxHealth = 100;

	public int currentHunger = 0;

	// lvl
	public int Lvl 		= 0;
	public int xp 		= 0; 
	public int skillPoints = 0;
	public List<int> specialSkillsIndexes = new List<int> ();

	// stats
	public int[] stats = new int[4] {
		1,1,1,1
	};

	public int daysOnBoard = 0;

	public Item equipedWeapon;
	public Item equipedCloth;

	public Item equipedWeaponID;
	public Item equipedClothID;

	public Member () {
        
	}

	public void SetJob ( Job _job ) {
		
		this.job = _job;


		specialSkillsIndexes.Clear();

		// special
		List<Skill> jobSkills = SkillManager.getJobSkills (job);

		if (Lvl >= 0) {
			specialSkillsIndexes.Add (SkillManager.getSkillIndex (jobSkills [0]));
		}

		if (Lvl >= 3) {
			specialSkillsIndexes.Add (SkillManager.getSkillIndex (jobSkills [1]));
		}

		if (Lvl >= 5) {
			specialSkillsIndexes.Add (SkillManager.getSkillIndex (jobSkills [2]));
		}

		if (Lvl >= 7) {
			specialSkillsIndexes.Add (SkillManager.getSkillIndex (jobSkills [3]));
		}



		Item cloth = ItemLoader.Instance.GetRandomItemOfCertainLevel (ItemCategory.Clothes, Lvl);
		equipedCloth = cloth;

		if (job == Job.Flibuster) {
			Item anyGun = System.Array.Find(ItemLoader.Instance.getItems(ItemCategory.Weapon), x => x.weaponType == Item.WeaponType.Distance && x.level == Lvl);
			equipedWeapon = anyGun;
		} else if (job == Job.Brute) {
			Item anySword = System.Array.Find(ItemLoader.Instance.getItems(ItemCategory.Weapon), x => x.weaponType == Item.WeaponType.Stick && x.level == Lvl);
			equipedWeapon = anySword;
		} else {
			Item anyWeapon = ItemLoader.Instance.GetRandomItemOfCertainLevel (ItemCategory.Weapon, Lvl);
			equipedWeapon = anyWeapon;
			//
		}

	}

	public Member (CrewParams crewParams) {

		// ID
		id = globalID;
		globalID++;

		// LEVEL
		if (crewParams.level > 0) {
			Lvl = crewParams.level;
		} else {

			Lvl = Random.Range (Crews.playerCrew.captain.Level - 3, Crews.playerCrew.captain.Level + 3);
			if ( StoryReader.Instance.CurrentStoryHandler.storyType == StoryType.Quest ) {
				Lvl = QuestManager.Instance.Coords_CheckForTargetQuest.level;
			}
		}

		Lvl = Mathf.Clamp ( Lvl , 1 , 10 );

        // JOB & SKILLS
        List<int> possibleIDs = new List<int>();

        if ( Crews.Instance.startJob != Job.None)
        {
            SetJob(Crews.Instance.startJob);
        }
        else
        {
            SetJob((Job)Random.Range(0, 5));
        }

		// STATS
		int statAmount = Lvl - 1;

		while ( statAmount > 0 )  {
			++stats [Random.Range (0, 4)];
			--statAmount;
		}

        foreach (var item in CrewCreator.Instance.apparenceGroups)
        {
            List<ApparenceItem> possibleItems = item.items.FindAll(x => x.locked == false);
            int randomID = possibleItems[Random.Range(0, possibleItems.Count)].id;

            if ( item.items[0].apparenceType == ApparenceType.job)
            {
                if ( Crews.Instance.startJob != Job.None)
                {
                    randomID = (int)Crews.Instance.startJob;
                }
            }

            characterIDS.Add(randomID);
        }
        
        // NAME
        if (Random.value < 0.5f)
        {
            Name = CrewCreator.Instance.manNames[Random.Range(0, CrewCreator.Instance.manNames.Length)];
        }
        else
        {
            Name = CrewCreator.Instance.womanNames[Random.Range(0, CrewCreator.Instance.womanNames.Length)];
        }
    }

    // icon index
	public Job job;

    public List<int> characterIDS = new List<int>();
    public int GetCharacterID ( ApparenceType type)
    {
        return characterIDS[(int)type];
    }
    public void SetCharacterID(ApparenceType type, int i)
    {
        characterIDS[(int)type] = i;
    }

    public bool SameAs (Member member)
    {
        return id == member.id;
    }

}