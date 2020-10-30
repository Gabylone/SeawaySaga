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

	public GameObject confirmButtonObj;
    //public GameObject previousButtonObj;

    public MemberCreation_NextStepArrow nextStepArrow;

    public Transform iconTargetParent;
    public Transform iconInitParent;

	[SerializeField]
	private GameObject overall;

	public Color initColor;

    public string[] boat_Names;
    public string[] boat_Adjectives;

	public GameObject[] stepObjs;
	public GameObject GetStep ( CreationStep step ) {
		return stepObjs [(int)step];
	}

    public Text jobDescription_Text;

	public Sprite femaleSprite;
	public Sprite maleSprite;

	[SerializeField]
	InputField captainName;

	[SerializeField]
	InputField boatName;

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

        //previousButtonObj.SetActive(false);

        // TWEEN NEXT STEP
        GetStep(step).SetActive (true);
		GetStep (step).GetComponent<RectTransform>().anchoredPosition = new Vector3 (0f , 1000f , 0f);

        GetStep(step).GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, tweenDuration);

        Invoke("ShowStepDelay", tweenDuration);

    }

    void ShowStepDelay () {

        if ( currentStep == 0)
        {
            //previousButtonObj.SetActive(false);
        }
        else
        {
            //previousButtonObj.SetActive(true);
        }

        nextStepArrow.Show();
	}

    private void NextStep()
    {
        ++currentStep;

        if (currentStep > CreationStep.CaptainName)
        {
            GetStep(currentStep - 1).SetActive(false);
        }

        ShowStep(currentStep);

       
    }

    public void PreviousStep()
    {
        --currentStep;

        HideStep(currentStep + 1);
        GetStep(currentStep + 1).SetActive(false);

        ShowStep(currentStep);
    }

    public void Show ()
	{
		currentStep = CreationStep.CaptainName;

        InGameMenu.Instance.canOpen = false;

        Crews.playerCrew.captain.Icon.MoveToPoint (Crews.PlacingType.MemberCreation);

        overall.SetActive(true);

		ShowStep(currentStep);

        SetRandomBoatName();

        Crews.playerCrew.captain.MemberID.Name = "The Captain";
		captainName.text = Crews.playerCrew.captain.MemberID.Name;

        iconInitParent = CrewCreator.Instance.crewParent_Player;
        Crews.playerCrew.captain.Icon.transform.SetParent(iconTargetParent);
        Crews.playerCrew.captain.Icon.transform.localScale = Vector3.one;
    }

    public void SetRandomBoatName()
    {
        string boat_name = CrewCreator.Instance.boatNames[Random.Range(0, CrewCreator.Instance.boatNames.Length)];
        string boat_adjective = CrewCreator.Instance.boatAdjectives[Random.Range(0, CrewCreator.Instance.boatAdjectives.Length)];
        string boat_fulName = "The " + boat_adjective + " " + boat_name;

        Boats.Instance.playerBoatInfo.Name = boat_fulName;
        boatName.text = boat_fulName;

        SoundManager.Instance.PlayRandomSound("click_med");

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

        HideStep(CreationStep.Appearance);

		//previousButtonObj.SetActive (false);

        Transitions.Instance.ScreenTransition.FadeIn(tweenDuration);

        Invoke("EndMemberCreationDelay", tweenDuration);
    }

    void EndMemberCreationDelay()
    {
        Crews.playerCrew.captain.Icon.transform.SetParent(iconInitParent);
        Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Portraits);

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

	public void ChangeBoatName () {

		Tween.Bounce ( boatName.transform , 0.2f , 1.05f);

        if (boatName.text.StartsWith("The "))
        {

        }
        else
        {
            boatName.text = "The " + boatName.text;
        }

		Boats.Instance.playerBoatInfo.Name = boatName.text;

            SoundManager.Instance.PlaySound("click_med 03");
    }

	public void ChangeCaptainName () {

		Tween.Bounce ( captainName.transform , 0.2f , 1.05f);

		Crews.playerCrew.captain.MemberID.Name = captainName.text;

            SoundManager.Instance.PlaySound("click_med 03");

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
