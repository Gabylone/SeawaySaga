using UnityEngine;
using System;
using System.IO;

[Serializable()]
public class Item {

	public int ID = 0;

    public string name
    {
        get
        {
            return names[(int)GameManager.language];
        }
    }

    public string[] names;
	public int 		value 		= 0;
	public int 		price 		= 0;
	public int 		weight 		= 0;
	public int 		level 		= 0;
	public int		spriteID 	= 0;

    public enum WeaponType
    {
        Distance,
        Stick,
        Sword,
        Club,
    }
    public WeaponType weaponType;

	public ItemCategory category;
		
	public Item () {
		//
	}

	public CrewMember.EquipmentPart EquipmentPart {
		get {
			switch (category) {
			case ItemCategory.Weapon:
				return CrewMember.EquipmentPart.Weapon;
			case ItemCategory.Clothes:
				return CrewMember.EquipmentPart.Clothes;
			default:
				return CrewMember.EquipmentPart.None;
			}
		}
	}
}
