using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class GoldManager : MonoBehaviour {

	public static GoldManager Instance;

	[Header ("UI Elements")]
	[SerializeField] private GameObject goldGroup;
	[SerializeField] private Text goldText;
	[SerializeField] private Image goldImage;

	[SerializeField]
	private float feedbackDuration = 1.5f;
	[SerializeField]
	private float feedbackScaleAmount = 1.5f;
	[SerializeField]
	private float feedbackBounceDuration = 0.3f;

	[Header ("Amounts")]
	[SerializeField]
	private int startValue = 200;
	public int goldAmount = 0;

	[Header("Sound")]
	[SerializeField] private AudioClip noGoldSound;
	[SerializeField] private AudioClip buySound;

    Color initTextColor;

	void Awake () {
		Instance = this;
        onChangeGold = null;
        onCheckGold = null;
	}

	void Start () {

        initTextColor = goldText.color;

		StoryFunctions.Instance.getFunction += HandleGetFunction;

		LootUI.useInventory+=HandleUseInventory;
	}

    /*void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddGold( 100 );
        }
    }*/

    void HandleUseInventory (InventoryActionType actionType)
	{
		switch (actionType) {
		case InventoryActionType.Sell:
//		case InventoryActionType.Buy:
//		case InventoryActionType.PurchaseAndEquip:
			Tween.Bounce (goldGroup.transform);
			break;
		default:
			break;
		}	
	}

	public void InitGold ()
	{
		goldAmount = startValue;

		UpdateUI ();
	}

	public void LoadGold ()
	{
		goldAmount = SaveManager.Instance.GameData.playerGold;

		UpdateUI ();
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.CheckGold:
			SetGoldDecal ();
			break;
		case FunctionType.RemoveGold:
			RemoveGold (cellParameters);
			break;
		case FunctionType.AddGold:
			AddGold (cellParameters);
			break;
		}
	}

	#region timed feedback
	
	private void DisplayFeedback () {

		Tween.Bounce (goldGroup.transform);

	}
	#endregion

	public void SetGoldDecal () {

		int amount = 0;

		if ( StoryFunctions.Instance.CellParams == "THIRD" ) {
			amount = (int)((float)goldAmount / 3f);
		} else {
			amount = int.Parse (StoryFunctions.Instance.CellParams);
		}

		if (CheckGold (amount)) {
			StoryReader.Instance.NextCell ();
		} else {
			StoryReader.Instance.NextCell ();
			StoryReader.Instance.SetDecal (1);
		}

		DisplayFeedback ();

		StoryReader.Instance.UpdateStory ();
	}

	public delegate void OnCheckGold ( bool enoughtGold );
	public static OnCheckGold onCheckGold;

	public bool CheckGold ( float amount ) {

		DisplayFeedback ();

        Tween.Bounce(goldGroup.transform);

        if ( amount > goldAmount ) {

			if (onCheckGold != null)
				onCheckGold (false);

			goldImage.color = Color.red;
			goldText.color = Color.red;

            SoundManager.Instance.PlaySound("ui_deny");

            Invoke("HideFeedback", feedbackBounceDuration);
			return false;
		}


		goldImage.color = Color.white;
		goldText.color = initTextColor;

        return true;
	}

    private void HideFeedback()
    {
        goldImage.color = Color.white;
        goldText.color = initTextColor;
    }

    public void UpdateUI () {
		goldText.text = goldAmount.ToString ();
	}

	void RemoveGold(string cellParams) {
		
		int amount = 0;

		if ( StoryFunctions.Instance.CellParams == "THIRD" ) {
			amount = (int)((float)goldAmount / 3f);
		} else {
			amount = int.Parse (StoryFunctions.Instance.CellParams);
		}

		RemoveGold(amount);

        SoundManager.Instance.PlayRandomSound("Coins");
        SoundManager.Instance.PlayRandomSound("Coins");

		StoryReader.Instance.Wait ( 0.3f );

		DisplayFeedback ();

	}
	void AddGold(string cellParams) {

		int amount = 0;

		if ( StoryFunctions.Instance.CellParams == "THIRD" ) {
			amount = (int)((float)goldAmount / 3f);
		} else {
			amount = int.Parse (StoryFunctions.Instance.CellParams);
		}

        SoundManager.Instance.PlayRandomSound("Bag");
        SoundManager.Instance.PlayRandomSound("Coins");
        SoundManager.Instance.PlayRandomSound("Coins");

        AddGold (amount);

		StoryReader.Instance.Wait (0.3f);

		DisplayFeedback ();

	}

	public delegate void OnChangeGold (int value);
	public static OnChangeGold onChangeGold;
	public void AddGold ( int i ) {
		goldAmount += i;
		UpdateUI ();
		if (onChangeGold != null)
			onChangeGold (i);
	}
	public void RemoveGold ( int i ) {
		goldAmount -= i;
		UpdateUI ();
		if (onChangeGold != null)
			onChangeGold (-i);
	}
}
