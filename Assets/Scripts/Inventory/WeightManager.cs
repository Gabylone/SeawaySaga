using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class WeightManager : MonoBehaviour {

	public static WeightManager Instance;

	[Header("UI Elements")]
	[SerializeField]
	private GameObject weightGroup;
	[SerializeField]
	private Image weightImage;
	[SerializeField]
	private Text currentWeightText;
    [SerializeField]
    private Text weightCapacityText;

    [SerializeField]
	private float feedbackDuration = 0.3f;
	[SerializeField]
	private float feedbackBounceDuration = 0.3f;
	[SerializeField]
	private float feedbackScaleAmount = 1.3f;

	[Header("Sound")]
	[SerializeField] 
	private AudioClip noRoomSound;

	void Awake () {
		Instance = this;

        onTooMuchWeight = null;
	}

	public void Init () {
//
		BoatUpgradeManager.onUpgradeBoat += HandleOnUpgradeBoat;

		LootUI.useInventory += UpdateDisplay;

		LootManager.Instance.updateLoot += UpdateDisplay;

		UpdateDisplay ();
	}

	void HandleOnUpgradeBoat (BoatUpgradeManager.UpgradeType upgradeType)
	{
		if (upgradeType == BoatUpgradeManager.UpgradeType.Cargo)
			UpdateDisplay ();
	}

	#region weight control
	public delegate void OnTooMuchWeight ();
	public static OnTooMuchWeight onTooMuchWeight;
	public bool CheckWeight ( int amount ) {

        Tween.Bounce(weightGroup.transform);

		if ( CurrentWeight + amount > CurrentCapacity ) {

			if (onTooMuchWeight != null)
				onTooMuchWeight ();

            weightImage.color = Color.red;
            currentWeightText.color = Color.red;

            SoundManager.Instance.PlaySound("ui_deny");

            Invoke("HideFeedback", feedbackBounceDuration);
			return false;
        }

        weightImage.color = Color.white;
        currentWeightText.color = Color.black;

        return true;

	}
	public void UpdateDisplay (InventoryActionType inventoryActionType) {
		UpdateDisplay ();
	}
	public void UpdateDisplay () {
        currentWeightText.text = "" + CurrentWeight;
        weightCapacityText.text = " / " + CurrentCapacity;
	}
	#endregion

	#region feedback
	public void HideFeedback () {
        weightImage.color = Color.white;
        currentWeightText.color = Color.black;
    }
	#endregion

	#region properties
	public int CurrentWeight {
		get {
			return LootManager.Instance.getLoot (Crews.Side.Player).weight;
		}
	}

	public int CurrentCapacity {
		get {
			return Boats.Instance.playerBoatInfo.GetCargoCapacity();
		}
	}
	#endregion
}
