using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour {

	public static DialogueManager Instance;

	[Header("UI Element")]
	[SerializeField] private GameObject bubble_Obj;
	[SerializeField] private RectTransform bubble_Image;
	[SerializeField] private Text bubble_Text;

	private Transform target;

	private bool DisplayingText = false;

	[Header("Parameters")]
	[SerializeField]
	private Vector3 speaker_Decal = Vector3.up * 2f;
	private CrewMember talkingMember;

	[SerializeField] private Vector2 bubbleBounds = new Vector2();

	[SerializeField]
	private int maxCharactersPerLine = 20;

	private int TextIndex = 0;

	public string dialogueText = "";

	public float DisplayTime = 2f;
	private float CurrentTime = 0f;

	[Header("Sounds")]
	[SerializeField] private AudioClip[] speakSounds;

	bool timed = false;

	void Awake () {
		Instance = this;
	}

	void Start () {
		StoryFunctions.Instance.getFunction+= HandleGetFunction;

		InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		InGameMenu.Instance.onCloseMenu += HandleCloseInventory;

        StoryInput.Instance.onPressInput += HandleOnPressInput;

    }

    bool previousActive = false;
	void HandleOpenInventory ()
	{
		if (bubble_Obj.activeSelf) {
			previousActive = true;
			bubble_Obj.SetActive (false);
		}
	}

	void HandleCloseInventory ()
	{
		if (previousActive) {
			bubble_Obj.SetActive (true);
			previousActive = false;
		}
	}

	void HandleOnPressInput ()
	{
		if ( DisplayingText)
        {
            EndDialogue();
            StoryReader.Instance.ContinueStory();
        }
    }

	void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.OtherSpeak:

                Crews.enemyCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Discussion);
                Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Discussion);
                SetDialogue(cellParameters.Remove (0, 2), Crews.enemyCrew.captain);
                StoryInput.Instance.WaitForInput();
                break;

            case FunctionType.PlayerSpeak:

                Crews.playerCrew.captain.Icon.MoveToPoint(Crews.PlacingType.Discussion);
			    SetDialogue (cellParameters.Remove (0, 2), Crews.playerCrew.captain);
                StoryInput.Instance.WaitForInput();
                break;
        }
	}

	void Update() 
	{
		if (DisplayingText == true) {
			UpdateDialogue ();
			UpdateBubblePosition ();
		}

	}

	#region set dialogue
	// TIMED
	public void SetDialogueTimed (string phrase, Transform _target) {

        timed = true;

        SetDialogue(phrase, _target);

	}
    public void SetDialogueTimed(string phrase, CrewMember crewMember)
    {
        SetDialogueTimed(phrase, crewMember.Icon.dialogueAnchor);
    }


	// INPUT
	public void SetDialogue (string phrase, CrewMember crewMember) {
		crewMember.Icon.animator.SetTrigger ("speak");
		crewMember.Icon.animator.SetInteger ("speakIndex",Random.Range(0,3) );
		SetDialogue (phrase, crewMember.Icon.dialogueAnchor);
	}
	public void SetDialogue (string phrase, Transform _target) {

		phrase = NameGeneration.CheckForKeyWords (phrase);

		dialogueText = phrase;

		target = _target;

		StartDialogue ();
	}
	#endregion

	#region dialogue states
	private void StartDialogue () {

		bubble_Obj.SetActive (true);

		// reset text
		TextIndex = 0;

		bubble_Text.text = dialogueText;

		CurrentTime = DisplayTime;

		DisplayingText = true;

		UpdateBubblePosition ();
		UpdateBubbleScale ();

//		if ( talkingMember != null )
//			SoundManager.Instance.PlaySound ( speakSounds[talkingMember.MemberID.voiceID] );
//	
	}

	private void UpdateDialogue () {

		if (target == null) {
			EndDialogue ();
			return;
		}

		if (CurrentTime > 0)
		{
			
			if (timed) {
				CurrentTime -= Time.deltaTime;
			}

		}
		else
		{
            EndDialogue();
		}
	}
	public void EndDialogue ()
	{
		DisplayingText = false;

		bubble_Obj.SetActive (false);

		timed = false;
	}
	#endregion

	void UpdateBubbleScale ()
	{
		// scale
		Vector3 scale = Vector3.one;

		float f = target.position.x < 0 ? -1 : 1;
		bubble_Image.localScale = new Vector3(f ,1 ,1 );
		bubble_Text.transform.localScale = new Vector3 (f,1,1);

		Tween.Bounce ( bubble_Image.transform , 0.2f , bubble_Image.localScale , 1.05f );	
	}

	private void UpdateBubblePosition ()
	{
		// position
		bubble_Image.transform.position = target.position;
	}

	public AudioClip[] SpeakSounds {
		get {
			return speakSounds;
		}
	}
}
