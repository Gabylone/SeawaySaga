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
    public GameObject previousButtonObj;

    public Transform iconTargetParent;
    public Transform iconInitParent;

	[SerializeField]
	private GameObject overall;

	public Color initColor;

	public GameObject[] stepObjs;
	public GameObject GetStep ( CreationStep step ) {
		return stepObjs [(int)step];
	}

    public Text jobDescription_Text;
    private string[] jobDescriptions_Str;

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

        LoadJobTexts();
	}

    private void LoadJobTexts()
    {
        TextAsset textAsset = Resources.Load("JobDescriptions") as TextAsset;
        jobDescriptions_Str = textAsset.text.Split('\n');
    }

    public string[] boatNames;
	public string[] captainNames;

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

		confirmButtonObj.SetActive (false);
        previousButtonObj.SetActive(false);

        // TWEEN NEXT STEP
        GetStep(step).SetActive (true);
		GetStep (step).GetComponent<RectTransform>().anchoredPosition = new Vector3 (0f , 1000f , 0f);

        GetStep(step).GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, tweenDuration);

        Invoke("ShowStepDelay", tweenDuration);

    }

    void ShowStepDelay () {

        if ( currentStep == 0)
        {
            previousButtonObj.SetActive(false);
        }
        else
        {
            //previousButtonObj.SetActive(true);
        }

        confirmButtonObj.SetActive (true);
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
		Transitions.Instance.ActionTransition.FadeIn (0.5f);

		currentStep = CreationStep.CaptainName;

        InGameMenu.Instance.canOpen = false;

        Crews.playerCrew.captain.Icon.MoveToPoint (Crews.PlacingType.MemberCreation);

        overall.SetActive(true);

		ShowStep(currentStep);

		int ID = Random.Range ( 0, boatNames.Length );

		Boats.playerBoatInfo.Name = captainNames[ID];
		boatName.text = captainNames[ID];

		Crews.playerCrew.captain.MemberID.Name = boatNames [ID];
		captainName.text = Crews.playerCrew.captain.MemberID.Name;

        Crews.playerCrew.captain.Icon.transform.SetParent(iconTargetParent);

	}

	public void Confirm () {

		if ( currentStep == CreationStep.Job ) {

			EndMemberCreation ();

		} else {

            NextStep();

		}

		SoundManager.Instance.PlaySound (SoundManager.Sound.Select_Big);

	}

    void EndMemberCreation () {

        HideStep(CreationStep.Appearance);

		confirmButtonObj.SetActive (false);
		previousButtonObj.SetActive (false);

        canvasGroup.DOFade(0f , tweenDuration);

        Invoke("EndMemberCreationDelay",tweenDuration);

	}
	void EndMemberCreationDelay () {

        Crews.playerCrew.captain.Icon.transform.SetParent(iconInitParent);
        Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Map);
        InGameMenu.Instance.canOpen = true;

        Hide();

		SaveManager.Instance.SaveGameData ();
		StoryLauncher.Instance.PlayStory (Chunk.currentChunk.IslandData.storyManager, StoryLauncher.StorySource.island);
	}

	public void ChangeBoatName () {

		Tween.Bounce ( boatName.transform , 0.2f , 1.05f);

		Boats.playerBoatInfo.Name = boatName.text;
		SoundManager.Instance.PlaySound (SoundManager.Sound.Select_Big);
	}

	public void ChangeCaptainName () {

		Tween.Bounce ( captainName.transform , 0.2f , 1.05f);

		Crews.playerCrew.captain.MemberID.Name = captainName.text;
		SoundManager.Instance.PlaySound (SoundManager.Sound.Select_Big);
	}

	public void ChangeApparence ( ApparenceType apparence , int id) {

		SoundManager.Instance.PlaySound (SoundManager.Sound.Select_Small);

        Crews.playerCrew.captain.Icon.InitVisual (Crews.playerCrew.captain.MemberID);

	}

    public void UpdateDescriptionText(int i)
    {
        jobDescription_Text.text = jobDescriptions_Str[i];
    }

}
