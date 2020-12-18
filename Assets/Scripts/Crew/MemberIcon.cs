using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MemberIcon : MonoBehaviour {

		// components
	[Header ("Components")]
	public GameObject group;

	public Animator animator;

    private RectTransform rectTransform;

	public bool overable = true;

	public float moveDuration = 1f;

    public int indexInList = 0;

    public float bodyScale = 1f;

    public bool visible = true;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.2f;

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

        if ( currentPlacingType != Crews.PlacingType.Portraits)
        {
            return;
        }

        if (!InGameMenu.Instance.canOpen)
        {
            print("cannot open player loot");
            return;
        }

        CrewMember.SetSelectedMember(member);

        if ( DisplayCrew.Instance.onSkills)
        {
            SkillMenu.Instance.Show();
        }
        else
        {
            if (LootUI.Instance.visible)
            {
                DisplayCrew.Instance.Show(CrewMember.GetSelectedMember);
                LootUI.Instance.UpdateLootUI();
            }
            else
            {
                LootUI.Instance.OpenInventory();
            }
        }
    }
    #endregion

    #region movement
    public void MoveToPoint ( Crews.PlacingType targetPlacingType ) {

		previousPlacingType = currentPlacingType;
		currentPlacingType = targetPlacingType;

        if (targetPlacingType == Crews.PlacingType.Hidden)
        {
            Hide();
            return;
        }

        Vector3 targetPos = Crews.getCrew(member.side).CrewAnchors [(int)targetPlacingType].position;

        if (currentPlacingType == Crews.PlacingType.Portraits) {
			targetPos = Crews.getCrew (member.side).inventoryAnchors [member.GetIndex].position;
		}
        else if (currentPlacingType == Crews.PlacingType.World)
        {
            int index = indexInList;
            rectTransform.SetParent(Crews.getCrew(member.side).worldAnchord[index]);
            targetPos = Crews.getCrew(member.side).worldAnchord[index].position;
        }

        Show();

        rectTransform.DOMove(targetPos, moveDuration);

        switch (currentPlacingType) {

            case Crews.PlacingType.Portraits:
            case Crews.PlacingType.Hidden:
            case Crews.PlacingType.None:
                HideBody();
                break;
            case Crews.PlacingType.MemberCreation:
            case Crews.PlacingType.Inventory:
            case Crews.PlacingType.World:
                ShowBody();
                if ( member.side == Crews.Side.Player)
                {
                    hungerIcon.HideInfo();
                }
                break;
            default:

                break;

		}
    }
    #endregion

    #region visibility
    // pour l'instant tout ça c'est avec déplacements, le perso est juste hors de l'écran
    // ça donne cet effet dynamique mais peut être à changer
    public void Show()
    {
        if (visible)
        {
            return;
        }

        CancelInvoke("HideDelay");

        group.SetActive(true);

        visible = true;

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration);
    }

    public void Hide()
    {
        if (!visible)
        {
            return;
        }

        visible = false;

        canvasGroup.DOFade(0f, fadeDuration);

        CancelInvoke("HideDelay");
        Invoke("HideDelay", fadeDuration);
    }

    public void HideDelay()
    {
        group.SetActive(false);
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


        iconVisual.rectTransform.DOScale(targetScale, moveDuration);

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

        iconVisual.rectTransform.DOScale(targetScale,moveDuration);

    }

	public void InitVisual (Member memberID)
	{
		iconVisual.InitVisual (memberID);
    }
	#endregion
}