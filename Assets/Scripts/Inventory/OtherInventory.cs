using UnityEngine;
using System.Collections;
using DG.Tweening;

public class OtherInventory : MonoBehaviour {

	public static OtherInventory Instance;

	public enum Type {
		None,

		Loot,
		Trade,
	}

	public Type type = Type.None;

    public float lootTransition_Duration = 0.5f;
    public float lootTransition_Decal = 100f;

	void Awake () {
		Instance = this;
	}

	void Start () {

		StoryFunctions.Instance.getFunction += HandleGetFunction;
		LootUI.useInventory += HandleUseInventory;
    }

    // 
    public void SwitchToPlayer ()
    {
        StartCoroutine(SwitchSideCoroutine());

        SoundManager.Instance.PlaySound("Whoosh 08");
    }

    //
    public void SwitchToOther()
    {
        StartCoroutine(SwitchSideCoroutine());

        SoundManager.Instance.PlaySound("Whoosh 09");

    }

    public void LerpIn()
    {
        LootUI.Instance.transform.position = Vector3.right * lootTransition_Decal;

        LootUI.Instance.HideAllSwitchButtons();

        LootUI.Instance.transform.DOMove(Vector3.zero, lootTransition_Duration);

        CancelInvoke("ShowButtons");
        Invoke("ShowButtons",lootTransition_Duration);
    }

    public void LerpOut()
    {
        CancelInvoke("ShowButtons");

        LootUI.Instance.transform.DOMove(Vector3.right * lootTransition_Decal, lootTransition_Duration);

        LootUI.Instance.HideAllSwitchButtons();
    }

    void SwitchInventorySide()
    {
        LootUI.Instance.currentSide = LootUI.Instance.currentSide == Crews.Side.Player ? Crews.Side.Enemy : Crews.Side.Player;
        ShowLoot();
    }

    void ShowButtons()
    {
        LootUI.Instance.InitButtons();
    }

    IEnumerator SwitchSideCoroutine()
    {
        LerpOut();

        ///
        yield return new WaitForSeconds(lootTransition_Duration);
        ///

        LerpIn();

        SwitchInventorySide();
    }


	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.Loot:
			StartLooting (false);
			break;
		case FunctionType.Trade:
			StartTrade ();
			break;

		}
	}

    void ShowLoot()
    {
        if (LootUI.Instance.currentSide == Crews.Side.Player)
        {
            switch (type)
            {
                case Type.None:
                    break;

                case Type.Loot:
                    LootUI.Instance.Show(CategoryContentType.PlayerLoot, Crews.Side.Player);
                    break;
                case Type.Trade:
                    LootUI.Instance.Show(CategoryContentType.PlayerTrade, Crews.Side.Player);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (type)
            {
                case Type.None:
                    break;

                case Type.Loot:
                    LootUI.Instance.Show(CategoryContentType.OtherLoot, Crews.Side.Enemy);
                    break;
                case Type.Trade:
                    LootUI.Instance.Show(CategoryContentType.OtherTrade, Crews.Side.Enemy);
                    break;
                default:
                    break;
            }
        }
    }

	void HandleUseInventory (InventoryActionType actionType)
	{
		switch (actionType) {
		case InventoryActionType.Buy:
			PurchaseItem ();
			break;
		case InventoryActionType.PickUp:
			PickUp (LootUI.Instance.SelectedItem);
			break;
		default:
			break;
		}
	}

	#region trade
	public void StartTrade () {

			// get loot
		Loot loot = LootManager.Instance.GetIslandLoot (2, false);

		if ( loot.IsEmpty () ) {
			DialogueManager.Instance.OtherSpeak ("Looks like you already bought everything from me !");
			return;
        }

        SoundManager.Instance.PlayRandomSound("Coins");
        SoundManager.Instance.PlaySound("Trade");
        SoundManager.Instance.PlayRandomSound("Bag");

        LootUI.Instance.currentSide = Crews.Side.Enemy;
		LootManager.Instance.SetLoot ( Crews.Side.Enemy, loot);

		type = Type.Trade;

        ShowLoot();
	}
	#endregion

	#region looting
	public void StartLooting (bool fightingLoot) {

		Loot loot = LootManager.Instance.GetIslandLoot (1, fightingLoot);

		if ( loot.IsEmpty () ) {
			DialogueManager.Instance.PlayerSpeak ("There was something but now nothing's left !");
			return;
        }

        SoundManager.Instance.PlayRandomSound("Bag");
        SoundManager.Instance.PlayRandomSound("button_big");

        LootUI.Instance.currentSide = Crews.Side.Enemy;
        LootManager.Instance.SetLoot ( Crews.Side.Enemy, loot);

		type = Type.Loot;

        ShowLoot();

    }
	#endregion

	public void PurchaseItem () {

		if (!GoldManager.Instance.CheckGold (LootUI.Instance.SelectedItem.price)) {
			return;
		}

		if (!WeightManager.Instance.CheckWeight (LootUI.Instance.SelectedItem.weight)) {
			return;
		}

		GoldManager.Instance.RemoveGold (LootUI.Instance.SelectedItem.price);

		LootManager.Instance.PlayerLoot.AddItem (LootUI.Instance.SelectedItem);
		LootManager.Instance.OtherLoot.RemoveItem (LootUI.Instance.SelectedItem);

        SoundManager.Instance.PlayRandomSound("Bag");
        SoundManager.Instance.PlayRandomSound("Coins");

	}

    public void PickUp ( Item item ) {
		
		if (!WeightManager.Instance.CheckWeight (item.weight))
			return;

        SoundManager.Instance.PlayRandomSound("Foley Armour");
        SoundManager.Instance.PlayRandomSound("Bag");

        LootManager.Instance.PlayerLoot.AddItem (item);
		LootManager.Instance.OtherLoot.RemoveItem (item);
	}
}
