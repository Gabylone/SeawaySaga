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
	private Color weightImage_Color;
	[SerializeField]
	private Text currentWeightText;
	public Image fillImage;
    public Color text_InitColor;
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

    void Awake()
    {
        Instance = this;

		weightImage_Color = weightImage.color;

        onTooMuchWeight = null;

        text_InitColor = currentWeightText.color;
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

        weightImage.color = weightImage_Color;
        currentWeightText.color = text_InitColor;

        return true;

	}
	public void UpdateDisplay (InventoryActionType inventoryActionType) {
		UpdateDisplay ();
	}
	public void UpdateDisplay () {

		float f = (float)CurrentWeight / (float)CurrentCapacity;

		fillImage.DOFillAmount(f, 0.2f).SetEase(Ease.OutBounce);

		currentWeightText.text = "" + Mathf.Clamp(CurrentWeight, 0, CurrentCapacity);
        weightCapacityText.text = " / " + CurrentCapacity;
	}
	#endregion

	#region feedback
	public void HideFeedback () {
        weightImage.color = weightImage_Color;
        currentWeightText.color = text_InitColor;
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
