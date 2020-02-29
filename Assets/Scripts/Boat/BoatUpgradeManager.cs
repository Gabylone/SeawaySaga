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

	[Header("Crew")]
	[SerializeField]
	private GameObject[] crewIcons;
    public GameObject[] crewIconCloseButtons;
    public Text crewPriceText;

    public Button upgradeCrewButton;

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

        if (onOpenBoatUpgrade != null)
			onOpenBoatUpgrade ();
	}

	void Hide () {
		menuObj.SetActive (false);
	}
    public void Close()
    {
        InGameMenu.Instance.Hide();

        opened = false;

        if (trading == true)
        {
            StopTrading();

        }

        Hide();

    }
    #endregion

    public int GetPrice ( UpgradeType upgradeType ) {

		switch (upgradeType) {
		case UpgradeType.Crew:
			return Boats.playerBoatInfo.crewCapacity * 100;
		case UpgradeType.Cargo:
			return Boats.playerBoatInfo.cargoLevel * 150;
		case UpgradeType.Longview:
			return Boats.playerBoatInfo.shipRange * 200;
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
			Boats.playerBoatInfo.cargoLevel++;
			break;
		case UpgradeType.Longview:
			Boats.playerBoatInfo.shipRange++;
			break;
		}

		++currentLevel;

		UpdateInfo ();

		if (onUpgradeBoat != null)
			onUpgradeBoat (upgradeType);

        SoundManager.Instance.PlaySound(upgradeSound);

    }


    public void UpdateInfo () {

		nameTextUI.text = Boats.playerBoatInfo.Name;

        boatUpgradeButtons[0].UpdateUI( Boats.playerBoatInfo.shipRange );
        boatUpgradeButtons[1].UpdateUI( Boats.playerBoatInfo.cargoLevel);

        UpgradeCrewButton();
	}

    private void UpgradeCrewButton()
    {
        // starts at one to leave captain alone
        for (int i = 1; i < crewIcons.Length; ++i)
        {
            if (i < Crews.playerCrew.CrewMembers.Count)
            {
                crewIcons[i].GetComponentInChildren<Image>().color = Color.white;
                crewIconCloseButtons[i].SetActive(true);
            }
            else if (i < Crews.playerCrew.CurrentMemberCapacity)
            {
                crewIcons[i].GetComponentInChildren<Image>().color = Color.gray;
                crewIconCloseButtons[i].SetActive(false);
            }
            else
            {
                crewIcons[i].GetComponentInChildren<Image>().color = Color.black;
                crewIconCloseButtons[i].SetActive(false);
            }
        }


        if (trading)
        {

            upgradeCrewButton.gameObject.SetActive(true);

            if (Crews.playerCrew.CurrentMemberCapacity == crewIcons.Length)
            {
                upgradeCrewButton.interactable = false;
                crewPriceText.text = "MAX";
            }
            else
            {
                upgradeCrewButton.interactable = true;
                crewPriceText.text = "" + GetPrice(UpgradeType.Crew);
            }
        }
        else
        {
            upgradeCrewButton.gameObject.SetActive(false);
        }
    }

    public void StartTrading()
    {
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

        MessageDisplay.Instance.Show("Abandonner " + Crews.playerCrew.CrewMembers[i].MemberName + " ?");
        MessageDisplay.onValidate += ConfirmRemoveMember;
    }

    public void ConfirmRemoveMember()
    {
        Crews.playerCrew.RemoveMember(memberToRemove);

    }

    
}
