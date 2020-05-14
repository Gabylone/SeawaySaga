using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CrewCreator : MonoBehaviour
{
    public static CrewCreator Instance;

    private Crews.Side targetSide;

    [Header("General")]
    [SerializeField]
    private Transform crewParent;
    [SerializeField]
    private GameObject[] memberIconPrefabs;

    public Sprite[] weaponSprites;
    public Sprite handSprite;

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

    void Awake()
    {   
        Instance = this;

        MemberCreatorButton.lastSelected = null;
    }

    public List<ApparenceGroup> apparenceGroups = new List<ApparenceGroup>();

    private void Start()
    {
        UpdateApparenceItems();

        LoadTextData();
    }

    private void LoadTextData()
    {
        TextAsset textAsset = Resources.Load("JobDescriptions") as TextAsset;
        jobDescriptions = textAsset.text.Split('\n');

        textAsset = Resources.Load("Names/Boat Names") as TextAsset;
        boatNames = textAsset.text.Split('\n');

        textAsset = Resources.Load("Names/Boat Adjectives") as TextAsset;
        boatAdjectives = textAsset.text.Split('\n');

        textAsset = Resources.Load("Names/Man Names") as TextAsset;
        manNames = textAsset.text.Split('\n');

        textAsset = Resources.Load("Names/Woman Names") as TextAsset;
        womanNames = textAsset.text.Split('\n');

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
        iconObj.transform.SetParent(crewParent);
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
    job,
    genre,
    map,
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

    public bool locked = false;

    public int id = 0;

    public int price = 10;

    public void Init (int i)
    {
        id = i;

        if ( locked && PlayerInfo.Instance.apparenceItems.Find(x => this.id == x.id && this.apparenceType == x.apparenceType) != null)    
        {
            locked = false;
        }
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
    
}