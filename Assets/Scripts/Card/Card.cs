using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Card : MonoBehaviour {

	public static Card previouslySelectedCard;

		// components
	private Transform _transform;

    [Header("UI Elements")]
    [SerializeField]
    private GameObject cardObject;

    public Transform barGroup;

	[SerializeField]
	private Image targetFeedbackImage;

	[SerializeField]
	private RectTransform healthBackground;
	[SerializeField]
	private RectTransform healthFillDelay;
	[SerializeField]
	private RectTransform healthFill;

	[SerializeField]
	private GameObject energyGroup;
	[SerializeField]
	private GameObject jobGroup;
	[SerializeField]
	private GameObject[] energyPoints;

	[SerializeField]
	private GameObject heartGroup;

	[SerializeField]
	private Image levelImage;

	[SerializeField]
	private Text levelText;

	public Fighter linkedFighter;

	public Image jobImage;

	bool playingTurn = false;

	public Transform endTurnFeedback;
	float endTurnFeedbackDuration = 0.7f;

//	void Awake () {
	public void Init() {

		linkedFighter.onReset += HandleOnInit;
		linkedFighter.onSetAsTarget += HandleOnSetAsTarget;
		linkedFighter.onSetTurn += HandleOnSetTurn;
		linkedFighter.onEndTurn += HandleOnEndTurn;

		linkedFighter.onShowInfo += HandleOnShowInfo;
		linkedFighter.onGetHit += HandleOnGetHit;

		linkedFighter.onChangeState += HandleOnChangeState;

		linkedFighter.onSetPickable += HandleOnSetPickable;

		LootUI.useInventory+= HandleUseInventory;

		HideTargetFeedback ();
		HideEndTurnFeedback ();

		energyGroup.SetActive (false);
		jobGroup.SetActive (false);

	}

	void HandleOnSetPickable (bool pickable)
	{
		if (pickable) {

            ShowTargetFeedback(Color.red);
        }
        else {

			if ( playingTurn ) {
				ShowTargetFeedback (Color.white);
			} else {
				HideTargetFeedback ();
			}
		}
	}

	void ShowTargetFeedback(Color color) {

        foreach (var item in linkedFighter.GetComponentsInChildren<SpriteOutline>())
        {
            item.enabled = true;
            item.color = color;
        }

		/*targetFeedbackImage.color = color;

		targetFeedbackImage.gameObject.SetActive (true);
		Tween.Bounce (targetFeedbackImage.transform);*/
	}

	void HideTargetFeedback () {

        foreach (var item in linkedFighter.GetComponentsInChildren<SpriteOutline>())
        {
            item.enabled = false;
        }
        /*targetFeedbackImage.GetComponent<Animator>().SetBool("bouncing", false);
        targetFeedbackImage.gameObject.SetActive(false);*/

    }

	void HandleOnChangeState (Fighter.states currState, Fighter.states prevState)
	{
		if (currState == Fighter.states.triggerSkill) {
			UpdateEnergyBar (linkedFighter.crewMember);

			Tween.Bounce (energyGroup.transform);
		}
	}


	void HandleOnEndTurn ()
	{
		Tween.Scale (barGroup, 0.2f, 1f);

		HideTargetFeedback ();

		endTurnFeedback.gameObject.SetActive (true);

		endTurnFeedback.localEulerAngles = Vector3.zero;
        endTurnFeedback.DOLocalRotate(Vector3.forward * 89f, endTurnFeedbackDuration).SetEase(Ease.InOutQuad);
		Tween.Bounce (endTurnFeedback, 1f , 1.5f);

		energyGroup.SetActive (false);

		playingTurn = false;

		Invoke ("HideEndTurnFeedback", endTurnFeedbackDuration);
	}

	void HideEndTurnFeedback () {
		endTurnFeedback.gameObject.SetActive (false);
	}

	void HandleOnInit () {

		UpdateMember (linkedFighter.crewMember);
	}

	void HandleOnGetHit ()
	{
		UpdateMember ();
	}

	void HandleOnShowInfo ()
	{
		energyGroup.SetActive (true);
		jobGroup.SetActive (true);

		CancelInvoke ("HideInfo");
		Invoke ("HideInfo",2f);

		Tween.Bounce (transform);
	}

	public void HideInfo ()
	{
		energyGroup.SetActive (false);
		jobGroup.SetActive (false);
	}

	void HandleOnSetAsTarget ()
	{
		UpdateMember ();
	}

	void HandleOnSetTurn ()
	{
		playingTurn = true;

		UpdateMember ();

		ShowTargetFeedback (Color.white);

		energyGroup.SetActive (true);

		Tween.Scale (barGroup, 0.2f, 1.15f);
	}

	void HandleUseInventory (InventoryActionType actionType)
	{
		UpdateMember (CrewMember.GetSelectedMember);
	}

	void UpdateMember() {
		UpdateMember (linkedFighter.crewMember);
	}

	public virtual void UpdateMember ( CrewMember member ) {

		levelText.text = member.Level.ToString ();

		if( member.side == Crews.Side.Enemy ) {
			
			levelImage.color = member.GetLevelColor ();
		}

		// HEALTH
		float l = (float)member.Health / (float)member.MemberID.maxHealth;
		float health_Width = -healthBackground.rect.width + healthBackground.rect.width * l;

		Vector2 v = new Vector2 (health_Width, 0f);

		float dur = 0.15f;

        healthFill.DOSizeDelta(v, dur);
        healthFillDelay.DOSizeDelta(v, dur * 3f).SetEase(Ease.OutCirc).SetDelay(dur * 3f);

		// STATS
		/*attackText.text = member.Attack.ToString ();
		defenceText.text = member.Defense.ToString ();*/

		if (SkillManager.jobSprites.Length <= (int)member.job)
			print ("skill l : " + SkillManager.jobSprites.Length + " / member job " + (int)member.job);
		jobImage.sprite = SkillManager.jobSprites[(int)member.job];

		UpdateEnergyBar (member);

		Tween.Bounce (transform);

	}

	public Color energyColor_Full;
	public Color energyColor_Empty;
	int currentEnergy = 0;
	public Text energyText;
	void UpdateEnergyBar(CrewMember member) {

		float scaleAmount = 0.8f;

		float dur = 0.5f;

		int a = 0;

		energyText.text = "" + member.energy;

//		foreach (var item in energyPoints) {
//			
//			if (a < member.energy) {
//				
//				item.transform.localScale = Vector3.one * scaleAmount;
//
//				HOTween.To ( item.transform , dur , "localScale" , Vector3.one );
//				HOTween.To ( item.GetComponent<Image>() , dur , "color" , energyColor_Full);
//
////				item.SetActive (true);
//			} else {
//
//				HOTween.To ( item.transform , dur , "localScale" , Vector3.one * scaleAmount);
//				HOTween.To ( item.GetComponent<Image>() , dur , "color" , energyColor_Empty);
//
////				item.SetActive (false);
//			}
//			++a;
//		}

//		currentEnergy = member.energy;
	}

	public void ShowCard () {
		cardObject.SetActive (true);

	}
	public void HideCard () {
		cardObject.SetActive (false);
	}


}
