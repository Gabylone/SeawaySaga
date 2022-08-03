using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CrewCreator : MonoBehaviour
{
    public static CrewCreator Instance;

    private Crews.Side targetSide;

    [Header("General")]
    public Transform crewParent_Player;
    public Transform crewParent_Enemy;
    [SerializeField]
    private GameObject[] memberIconPrefabs;

    public Sprite[] weaponSprites;
    public Sprite[] handSprites;

    public Sprite[] zombie_NoseSprite;
    public Sprite[] zombie_MouthSprite;
    public Sprite[] zombie_EyesSprite;
    public Color[]  zombie_SkinColors;

    [Header("Colors")]
    [SerializeField]
    private Color darkHair;
    [SerializeField] private Color beige;
    [SerializeField]
    public Color[] hairColors = new Color[7] {
        Color.red,
        Color.white,
        Color.black,
        Color.yellow,
        Color.gray,
        Color.gray,
        Color.gray,
    };

    /// <summary>
    /// text data
    /// </summary>
    public string[] jobDescriptions;
    public string[] womanNames;
    public string[] manNames;
    public string[] boatNames;
    public string[] boatAdjectives;

    public Sprite noImage_Sprite;

    [Header("Knocked Out")]
    public Sprite deadEyes_Sprite;
    [Header("Mad")]
    public Sprite madEyes_Sprite;
    public Sprite madEyebrows_Sprite;
    public Sprite madMouth_Sprite;

    [Header("Happy")]
    public Sprite happyEyes_Sprite;
    public Sprite happyEyebrows_Sprite;
    public Sprite happyMouth_Sprite;

    [Header("Sad")]
    public Sprite sadEyes_Sprite;
    public Sprite sadEyebrows_Sprite;
    public Sprite sadMouth_Sprite;

    [Header("Aiming")]
    public Sprite aimingEyes_Sprite;

    public BodySet[] bodySets;

    void Awake()
    {   
        Instance = this;
    }

    public List<ApparenceGroup> apparenceGroups = new List<ApparenceGroup>();

    private void Start()
    {
        UpdateApparenceItems();

        LoadTextData();
    }

    public BodySet GetBodySet (int id)
    {
        return bodySets[id];
    }

    private void LoadTextData()
    {
        TextAsset textAsset = Resources.Load("JobDescriptions") as TextAsset;
        jobDescriptions = textAsset.text.Split('\n');

        // boat names
        textAsset = Resources.Load("Names/Boat Names") as TextAsset;
        boatNames = textAsset.text.Split('\n');
        boatNames = Clean(boatNames);

        // boat adjectives
        textAsset = Resources.Load("Names/Boat Adjectives") as TextAsset;
        boatAdjectives = textAsset.text.Split('\n');
        boatAdjectives = Clean(boatAdjectives);

        // man names
        textAsset = Resources.Load("Names/Man Names") as TextAsset;
        manNames = textAsset.text.Split('\n');
        manNames = Clean(manNames);

        // woman names
        textAsset = Resources.Load("Names/Woman Names") as TextAsset;
        womanNames = textAsset.text.Split('\n');
        womanNames = Clean(womanNames);
    }

    public string[] Clean( string[] strings)
    {
        for (int i = 0; i < strings.Length; i++)
        {
            strings[i] = strings[i].TrimEnd('\r','\n');
            strings[i] = strings[i].TrimStart('\r','\n');
        }

        return strings;
    }

    public void UpdateApparenceItems()
    {
        foreach (var item in apparenceGroups)
        {
            int i = 0;

            foreach (var item2 in item.items)
            {
                item2.Init(i);

                ++i;
            }
        }
    }

    public ApparenceItem GetApparenceItem(ApparenceType type, int id)
    {
        if ( (int) type >= apparenceGroups.Count)
        {
            Debug.LogError("ATTENTION : le type : " + type + " (" + ((int)type).ToString () + ") dépasse apparence groups count (l " + apparenceGroups.Count + ")");
            return apparenceGroups[0].items[0];
        }
        if ( id >= apparenceGroups[(int)type].items.Count)
        {
            Debug.LogError("ATTENTION : l'id : " + id + " du type : " + type + " dépasse apparence groups items count ( l " + apparenceGroups[(int)type].items.Count + ")");
            return apparenceGroups[0].items[0];
        }
        return apparenceGroups[(int)type].items[id];
    }

    public CrewMember NewMember(Member memberID)
    {

        CrewMember crewMember = new CrewMember(

            memberID,

            // side
            targetSide,

            NewIcon(memberID)

        );

        return crewMember;
    }

    #region icons
    public MemberIcon NewIcon(Member memberID)
    {

        GameObject iconObj = Instantiate(memberIconPrefabs[(int)targetSide]) as GameObject;
        MemberIcon icon = iconObj.GetComponent<MemberIcon>();

        // set object transform
        if (targetSide == Crews.Side.Player)
        {
            iconObj.transform.SetParent(crewParent_Player);
        }
        else
        {
            iconObj.transform.SetParent(crewParent_Enemy);
        }

        iconObj.transform.localScale = Vector3.one;
        iconObj.transform.position = Crews.getCrew(targetSide).CrewAnchors[(int)Crews.PlacingType.Hidden].position;

        //		Vector3 scale = new Vector3 ( TargetSide == Crews.Side.Enemy ? 1 : -1 , 1 , 1);
        //
        //		icon.group.transform.localScale = scale;

        return iconObj.GetComponent<MemberIcon>();
    }
    #endregion

    public Crews.Side TargetSide
    {
        get
        {
            return targetSide;
        }
        set
        {
            targetSide = value;
        }
    }

    [System.Serializable]
    public class BodySet
    {
        public Sprite[] sprites;
        public BodyVisual.BodyID[] layerOrder
            = new BodyVisual.BodyID[7]
            {
                BodyVisual.BodyID.LeftArm,
                BodyVisual.BodyID.Skin,
                BodyVisual.BodyID.Shoes,
                BodyVisual.BodyID.Pants,
                BodyVisual.BodyID.Top,
                BodyVisual.BodyID.RightArm,
                BodyVisual.BodyID.Face
            };
    }

}

public enum ApparenceType
{
    // apparence
    beard,
    eyebrows,
    eyes,
    hair,
    mouth,
    nose,
    hairColor,

    //jobs
    job, // TOTALEMENT OBSOLEE EN FAIT
    genre,
    map,

    bodyType,
    skinColor,
    topColor,
    pantColor,
    shoesColor,

    faceType,

    voiceType,
}

[System.Serializable]
public class ApparenceGroup
{
    public List<ApparenceItem> items = new List<ApparenceItem>();
}

[System.Serializable]
public class ApparenceItem
{
    public ApparenceType apparenceType;

    [SerializeField]
    Sprite sprite;

    public  Color color;

    public bool locked = false;

    public int id = 0;

    public void Init (int i)
    {
        id = i;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
    
}