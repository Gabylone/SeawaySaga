using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class MemberCreator : MonoBehaviour {

	public static MemberCreator Instance;

    public CanvasGroup canvasGroup;

	public enum CreationStep {
		CaptainName,
		//BoatName,
		Appearance,
		Job
    }

    public Image fadeImage;

	public CreationStep currentStep;

    public MemberCreation_NextStepArrow nextStepArrow;
    public MemberCreation_PreviousStepArrow previousStepArrow;

    public Transform iconTargetParent;
    public Transform iconInitParent;

	[SerializeField]
	private GameObject overall;

	public Color initColor;

    public string[] apparenceType_Names;

	public GameObject[] stepObjs;
	public GameObject GetStep ( CreationStep step ) {
		return stepObjs [(int)step];
	}

    public Text jobDescription_Text;

	public Sprite femaleSprite;
	public Sprite maleSprite;

	public GameObject memberCreatorButtonParent;
	public MemberCreatorButton[] memberCreatorButtons;

	public float tweenDuration = 0.7f;

	public Image rayblocker;

    public Animator animator;

	void Awake () {
		Instance = this;
	}

	void Start () {

		Hide ();
	}

	void Hide ()
	{
		overall.SetActive (false);

		foreach (var item in stepObjs) {
			item.SetActive (false);
		}

	}

	public void HideStep (CreationStep step) {
        GetStep(step).GetComponent<RectTransform>().DOAnchorPos(Vector2.up * -1000f, tweenDuration);
	}
    
	public void ShowStep ( CreationStep step ) {

        previousStepArrow.Hide();

        // TWEEN NEXT STEP
        GetStep(step).SetActive (true);
		GetStep (step).GetComponent<RectTransform>().anchoredPosition = new Vector3 (0f , 1000f , 0f);

        GetStep(step).GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, tweenDuration);

        Invoke("ShowStepDelay", tweenDuration);

    }

    void ShowStepDelay () {

        if ( currentStep == 0)
        {
            previousStepArrow.HideDelay();
        }
        else
        {
            previousStepArrow.Show();
        }

        nextStepArrow.Show();
	}

    private void NextStep()
    {
        ++currentStep;

        if (currentStep > CreationStep.CaptainName)
        {
            HideStep(currentStep - 1);
        }

        ShowStep(currentStep);

       
    }

    public void PreviousStep()
    {
        --currentStep;

        HideStep(currentStep + 1);
        ShowStep(currentStep);
    }

    public void Show ()
	{
		currentStep = CreationStep.CaptainName;

        InGameMenu.Instance.canOpen = false;

        Crews.playerCrew.captain.Icon.MoveToPoint (Crews.PlacingType.MemberCreation);

        overall.SetActive(true);

		ShowStep(currentStep);

        iconInitParent = CrewCreator.Instance.crewParent_Player;
        Crews.playerCrew.captain.Icon.transform.SetParent(iconTargetParent);
        Crews.playerCrew.captain.Icon.transform.localScale = Vector3.one;
    }

    public void Confirm () {

		if ( currentStep == CreationStep.Job ) {

			EndMemberCreation ();

            SoundManager.Instance.PlaySound("Confirm_Big");
            SoundManager.Instance.PlaySound("button_heavy 01");

        } else {

            NextStep();

            SoundManager.Instance.PlayRandomSound("Swipe");
            SoundManager.Instance.PlaySound("click_med 01");
        }

    }

    void EndMemberCreation () {

        HideStep(CreationStep.Job);

        previousStepArrow.Hide();
        nextStepArrow.Hide();

        Transitions.Instance.ScreenTransition.FadeIn(tweenDuration);

        Invoke("EndMemberCreationDelay", tweenDuration);
    }

    void EndMemberCreationDelay()
    {
        Crews.playerCrew.captain.Icon.transform.SetParent(iconInitParent);
        Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Portraits);

        // pour régler un bug avec les skills là j'en peux plus c'est un fix sale
        foreach (var crewMember in Crews.playerCrew.CrewMembers)
        {
            crewMember.ResetSkills();
        }

        Invoke("EndMemberCreationDelay2", tweenDuration);
    }

    void EndMemberCreationDelay2 () {

        Transitions.Instance.ScreenTransition.FadeOut(tweenDuration);

        InGameMenu.Instance.canOpen = true;

        Hide();

		SaveManager.Instance.SaveGameData ();

        // ici je mets le premiere island data, parce que le ID 0 est lié à la maison en l'occurance. Mais quel bordel nom de dieu.
        IslandManager.Instance.islands[0].Enter();
	}

    public void ChangeApparence ( ApparenceType apparence , int id) {
        Crews.playerCrew.captain.Icon.InitVisual (Crews.playerCrew.captain.MemberID);
        SoundManager.Instance.PlaySound("click_med 01");
    }

    public void UpdateDescriptionText(int i)
    {
        jobDescription_Text.text = CrewCreator.Instance.jobDescriptions[i];
    }

}
