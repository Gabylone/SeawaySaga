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
        if (LootUI.Instance.currentSide == Crews.Side.Player)
        {
            return;
        }

        StartCoroutine(SwitchSideCoroutine());

        SoundManager.Instance.PlaySound("Whoosh 08");
    }

    //
    public void SwitchToOther()
    {
        if (LootUI.Instance.currentSide == Crews.Side.Enemy)
        {
            return;
        }

        StartCoroutine(SwitchSideCoroutine());

        SoundManager.Instance.PlaySound("Whoosh 09");
    }

    public void LerpIn()
    {
        LootUI.Instance.transform.position = Vector3.right * lootTransition_Decal;

        LootUI.Instance.transform.DOMove(Vector3.zero, lootTransition_Duration);

        LootUI.Instance.closeButton_canvasGroup.alpha = 0f;
        LootUI.Instance.closeButton_canvasGroup.DOFade(1f, 0.2f).SetDelay(lootTransition_Duration);

    }

    public void LerpOut()
    {
        LootUI.Instance.transform.DOMove(Vector3.right * lootTransition_Decal, lootTransition_Duration);

        LootUI.Instance.closeButton_canvasGroup.alpha = 0f;

    }

    void SwitchInventorySide()
    {
        LootUI.Instance.currentSide = LootUI.Instance.currentSide == Crews.Side.Player ? Crews.Side.Enemy : Crews.Side.Player;
        ShowLoot();
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
		Loot loot = LootManager.Instance.GetIslandLoot (GetCellMult(2), false);

		if ( loot.IsEmpty () ) {

            string[] strs = new string[5]
            {
                "Looks like you already bought everything from me!",
                "Sorry my friend, I’m afraid there’s nothing left to buy!",
                "I’m out-of-stock, sorry! Shop is empty but my purse is full, that’s for sure!",
                "Looks like I sold everything! I need to restock as soon as possible.",
                "Today was a good day, all my merchandise was sold.Couldn’t be happier!"
            };

            string str = strs[Random.Range(0, strs.Length)];

            DialogueManager.Instance.OtherSpeak_Story (str);
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
        
        Loot loot = LootManager.Instance.GetIslandLoot (GetCellMult(1), fightingLoot);

		if ( loot.IsEmpty () ) {

            string[] strs = new string[5]
            {
            "There was something but now nothing's left!",
            "There’s nothing left that’s worth taking.",
            "Seems like there’s nothing interesting here.",
            "I don’t see anything of value here! Let’s keep moving.",
            "I can’t find anything valuable that’s left."
            };

            string str = strs[Random.Range(0, strs.Length)];

            DialogueManager.Instance.PlayerSpeak_Story (str);
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

    private int GetCellMult(int mult)
    {
        if ( StoryFunctions.Instance.cellParams.Length > 0)
        {
            int tmpMult = 0;

            string lastCharacter = "" + StoryFunctions.Instance.cellParams[StoryFunctions.Instance.cellParams.Length - 1];

            if (int.TryParse(lastCharacter, out tmpMult))
            {
                StoryFunctions.Instance.cellParams = StoryFunctions.Instance.cellParams.Remove(StoryFunctions.Instance.cellParams.Length - 1);
                return tmpMult;
            }
            else
            {
                return mult;
            }
        }

        return mult;
    }

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
