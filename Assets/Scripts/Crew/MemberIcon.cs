using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MemberIcon : MonoBehaviour {

		// components
	[Header ("Components")]
	public GameObject group;

	public Animator animator;

    RectTransform rectTransform;

	public bool overable = true;

	public float moveDuration = 1f;

    public int indexInList = 0;

	public float bodyScale = 1f;

	public float initScale;

	public Crews.PlacingType currentPlacingType = Crews.PlacingType.None;
	public Crews.PlacingType previousPlacingType  = Crews.PlacingType.None;

    public Vector2 decal = Vector2.zero;

	public Transform dialogueAnchor;

	public CrewMember member;

    public DisplayHunger_Icon hungerIcon;

    public IconVisual iconVisual;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    void Start () {
        LootUI.useInventory += HandleUseInventory;
    }

    void OnDestroy()
    {
        LootUI.useInventory -= HandleUseInventory;
    }

    void HandleUseInventory(InventoryActionType actionType)
    {
        switch (actionType)
        {
            case InventoryActionType.Equip:
            case InventoryActionType.PurchaseAndEquip:
            case InventoryActionType.Unequip:

                if (LootUI.Instance.SelectedItem.category == ItemCategory.Weapon)
                {
                    iconVisual.UpdateWeaponSprite(CrewMember.GetSelectedMember.MemberID);
                }

                break;
            default:
                break;
        }
    }

    public void SetMember (CrewMember member) {

		this.member = member;

		animator = GetComponentInChildren<Animator> ();
	
		HideBody ();

		InitVisual (member.MemberID);
	}

    #region overing
    public void OnPointerDown()
    {
        if (member.side == Crews.Side.Enemy)
        {
            StoryInput.Instance.LockFromMember();
            GetComponentInChildren<StatGroup>().Display(member);
            return;
        }

        if (!InGameMenu.Instance.canOpen)
        {
            print("cannot open player loot");
            return;
        }

        if ( SkillMenu.Instance.opened)
        {
            SkillMenu.Instance.Show(member);
        }
        else
        {
            LootUI.Instance.OpenMemberLoot(member);
        }
    }
    #endregion

    #region movement
    public void MoveToPoint ( Crews.PlacingType targetPlacingType ) {

		previousPlacingType = currentPlacingType;
		currentPlacingType = targetPlacingType;

		Vector3 targetPos = Crews.getCrew(member.side).CrewAnchors [(int)targetPlacingType].position;

        if (currentPlacingType == Crews.PlacingType.Portraits) {
			targetPos = Crews.getCrew (member.side).inventoryAnchors [member.GetIndex].position;
		}
        else if (currentPlacingType == Crews.PlacingType.World)
        {
            int index = indexInList;
            transform.SetParent(Crews.getCrew(member.side).worldAnchord[index]);
            targetPos = Crews.getCrew(member.side).worldAnchord[index].position;
        }

        rectTransform.DOMove(targetPos, moveDuration);

		switch (currentPlacingType) {

            case Crews.PlacingType.Portraits:

                HideBody();

                break;

            case Crews.PlacingType.Hidden:

                HideBody();

                break;

            case Crews.PlacingType.None:

                HideBody();

                break;

            case Crews.PlacingType.MemberCreation:

                ShowBody();

                break;

            case Crews.PlacingType.Inventory:

                ShowBody();

                if ( member.side == Crews.Side.Player)
                {
                    hungerIcon.HideInfo();
                }

                break;

            case Crews.PlacingType.World:

                ShowBody();

                if (member.side == Crews.Side.Player)
                {
                    hungerIcon.HideInfo();
                }

                break;

            default:

                break;

		}
    }
    #endregion

    #region body
    public void HideBody () {

        foreach (var item in iconVisual.bodyParts)
        {
            item.SetActive(false);
        }

		animator.SetBool ("enabled", false);

		Vector3 targetScale = Vector3.one * initScale;
		if (member.side == Crews.Side.Player)
			targetScale.x = -targetScale.x;

        group.transform.DOScale(targetScale, moveDuration);

	}
	public void ShowBody () {

        foreach (var item in iconVisual.bodyParts)
        {
            item.SetActive(true);
        }

        animator.SetBool ("enabled", true);

        Vector3 targetScale = Vector3.one * bodyScale;
        if (member.side == Crews.Side.Player)
            targetScale.x = -targetScale.x;

        group.transform.DOScale(targetScale,moveDuration);

    }

	public void InitVisual (Member memberID)
	{
		iconVisual.InitVisual (memberID);
    }
	#endregion
}