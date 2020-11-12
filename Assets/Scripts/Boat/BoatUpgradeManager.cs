using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class BoatUpgradeManager : MonoBehaviour {

	public static BoatUpgradeManager Instance;

	public bool trading = false;

	public bool opened = false;

    public delegate void OnOpenBoatUpgrade();
    public OnOpenBoatUpgrade onOpenBoatUpgrade;

    public Animator animator;

    public enum UpgradeType {
		Crew,
		Cargo,
		Longview
	}

	[Header("Name & Level")]
	[SerializeField]
	private Text nameTextUI;
	[SerializeField]
	private Text levelTextUI;

	private int currentLevel = 1;
	public int cargoAugmentation = 50;

	[Header("UI Groups")]
	[SerializeField]
	private GameObject menuObj;

    [Header("Sounds")]
    [SerializeField] private AudioClip upgradeSound;

    public BoatUpgradeButton[] boatUpgradeButtons;

    void Awake()
    {
        Instance = this;
    }

    void Start () {

		StoryFunctions.Instance.getFunction += HandleGetFunction;

		RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;

		Hide ();

	}

	void HandleOnTouchRayBlocker ()
	{
		if (opened)
			Close ();
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		if ( func == FunctionType.BoatUpgrades ) {
            StartTrading();
		}
	}

	#region show / hide
	public void Show () {

        InGameMenu.Instance.Open();

        menuObj.SetActive (true);

		UpdateInfo ();

		opened = true;

        animator.SetBool("opened", true);

        if (onOpenBoatUpgrade != null)
			onOpenBoatUpgrade ();
	}

    public void Close()
    {
        if (!opened)
        {
            return;
        }


        animator.SetBool("opened", false);

        opened = false;

        if (trading == true)
        {
            StopTrading();
        }

        Invoke( "Hide" , 0.5f );
    }

    void Hide()
    {
        InGameMenu.Instance.Hide();
        menuObj.SetActive(false);
    }
    #endregion

    public int GetPrice ( UpgradeType upgradeType ) {

		switch (upgradeType) {
		case UpgradeType.Crew:
			return Boats.Instance.playerBoatInfo.crewCapacity * 100;
		case UpgradeType.Cargo:
			return Boats.Instance.playerBoatInfo.cargoLevel * 150;
		case UpgradeType.Longview:
			return Boats.Instance.playerBoatInfo.shipRange * 200;
		default:
			return -1;
		}

	}

	public delegate void OnUpgradeBoat ( UpgradeType upgradeType );
	public static OnUpgradeBoat onUpgradeBoat;
    public void Upgrade(int i)
    {
        Upgrade((UpgradeType)i);
    }
	public void Upgrade ( UpgradeType upgradeType )
    {
        if ( !GoldManager.Instance.CheckGold ( GetPrice(upgradeType) ))
			return;

		GoldManager.Instance.RemoveGold (GetPrice(upgradeType));

		switch ( upgradeType ) {
		case UpgradeType.Crew:
			Crews.playerCrew.CurrentMemberCapacity++;
			break;
		case UpgradeType.Cargo:
			Boats.Instance.playerBoatInfo.cargoLevel++;
			break;
		case UpgradeType.Longview:
			Boats.Instance.playerBoatInfo.shipRange++;
			break;
		}

		++currentLevel;

        SoundManager.Instance.PlayRandomSound("Anvil");
        SoundManager.Instance.PlayRandomSound("Workshop");

        UpdateInfo ();

		if (onUpgradeBoat != null)
			onUpgradeBoat (upgradeType);
    }


    public void UpdateInfo () {

		nameTextUI.text = Boats.Instance.playerBoatInfo.Name;

        boatUpgradeButtons[0].UpdateUI( Boats.Instance.playerBoatInfo.shipRange );
        boatUpgradeButtons[1].UpdateUI( Boats.Instance.playerBoatInfo.cargoLevel);
        boatUpgradeButtons[2].UpdateUI( Boats.Instance.playerBoatInfo.crewCapacity);
	}


    public void StartTrading()
    {
        SoundManager.Instance.PlayRandomSound("Workshop");

        trading = true;

        Show();

        RayBlocker.Instance.Show();

        InGameMenu.Instance.Lock();

    }

    public void StopTrading()
    {
        StoryReader.Instance.NextCell();
        StoryReader.Instance.UpdateStory();

        trading = false;

        RayBlocker.Instance.Hide();

        InGameMenu.Instance.Unlock();

    }

    int memberToRemove = -1;
    public void RemoveMember(int i)
    {
        memberToRemove = i;

        MessageDisplay.Instance.Show("Abandon " + Crews.playerCrew.CrewMembers[i].MemberName + " ?");
        MessageDisplay.Instance.onValidate += ConfirmRemoveMember;
    }

    public void ConfirmRemoveMember()
    {
        Crews.playerCrew.RemoveMember(memberToRemove);

    }

    
}
