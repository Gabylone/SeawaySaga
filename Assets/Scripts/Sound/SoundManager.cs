using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SoundManager : MonoBehaviour {

	public enum Sound {
		Select_Small,
		Select_Big,
	}

	public static SoundManager Instance;

	[SerializeField]
	private AudioSource soundSource;

	[SerializeField]
	private AudioSource ambianceSource;

	[SerializeField]
	private AudioClip[] clips;

	bool enableSound = true;

	[Header("Inventory sounds")]
	[SerializeField] private AudioClip eatSound;
	[SerializeField] private AudioClip equipSound;
	[SerializeField] private AudioClip sellSound;
	[SerializeField] private AudioClip lootSound;

	[Header ("Time sounds")]
	[SerializeField] private AudioClip rainSound;
	[SerializeField] private AudioClip daySound;
	[SerializeField] private AudioClip nightSound;

	[Header("Sound menu")]
	[SerializeField]
	private Image soundImage;
	[SerializeField]
	private Sprite sprite_SoundOn;

	[SerializeField]
	private Sprite sprite_SoundOff;
	void Start () {
		
		EnableSound = true;

		LootUI.useInventory += HandleUsePlayerInventory;
		NavigationManager.Instance.EnterNewChunk += HandleChunkEvent;

		StoryFunctions.Instance.getFunction += HandleGetFunction;

		UpdateAmbiance ();
	}

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.SetWeather:
		case FunctionType.ChangeTimeOfDay:
			UpdateAmbiance ();
			break;
		default:
			break;
		}
	}

	#region time
	void HandleChunkEvent ()
	{
	}

	void UpdateAmbiance ()
	{
		AudioClip ambiantClip;
		if (TimeManager.Instance.raining)
			ambiantClip = rainSound;
		else if (TimeManager.Instance.dayState == TimeManager.DayState.Night)
			ambiantClip = nightSound;
		else
			ambiantClip = daySound;

		PlayAmbiance (ambiantClip);
	}
	#endregion

	#region inventory
	void HandleUsePlayerInventory (InventoryActionType actionType)
	{
		switch (actionType) {
		case InventoryActionType.Eat:
			PlaySound (eatSound);
			break;
		case InventoryActionType.Equip:
		case InventoryActionType.PurchaseAndEquip:
		case InventoryActionType.Unequip:
			PlaySound (equipSound);
			break;
		case InventoryActionType.Throw:
			PlaySound (equipSound);
			break;
		case InventoryActionType.Sell:
			PlaySound (sellSound);
			break;
		case InventoryActionType.Buy:
			PlaySound (lootSound);
			break;
		case InventoryActionType.PickUp:
			PlaySound (lootSound);
			break;
		default:
			throw new System.ArgumentOutOfRangeException ();
		}
	}
	#endregion

	void Awake () {
		Instance = this;
	}

	public void PlayRandomSound (AudioClip[] clips) {
		PlaySound (clips [Random.Range (0, clips.Length)]);
	}

	public void PlaySound ( Sound sound ) {
		PlaySound (clips [(int)sound]);
	}

	public void PlaySound ( AudioClip clip ) {

		if ( clip == null ) {
			Debug.LogError ("unassigned clip");
			return;
		}

		soundSource.clip = clip;
		soundSource.Play ();
	}

	public void PlayAmbiance ( AudioClip clip ) {
		ambianceSource.clip = clip;
		ambianceSource.Play ();
	}

	public AudioSource AmbianceSource {
		get {
			return ambianceSource;
		}
	}

	public void SwitchEnableSound () {
		EnableSound = !EnableSound;
	}

	public bool EnableSound {
		get {
			return enableSound;
		}
		set {
			enableSound = value;

			soundSource.enabled = value;
			ambianceSource.enabled = value;
			if (value) {
				ambianceSource.Play ();
			}

			soundImage.sprite = value ? sprite_SoundOn : sprite_SoundOff;
		}
	}
}
