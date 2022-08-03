using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour {

	public static DialogueManager Instance;

	[Header("UI Element")]
	[SerializeField] private GameObject bubble_Obj;
	[SerializeField] private RectTransform bubble_Image;
	[SerializeField] private Text bubble_Text;

	[Header("Sound")]
	public AudioSource source;
	public List<Voice> voices = new List<Voice>();
	[System.Serializable]
	public class Voice
    {
		public enum Type
        {
			Greetings,
			Conversations,
			Hurts,
			Deaths,
        }
		
		public AudioClip GetRandomVoice(Type type)
        {
            switch (type)
            {
                case Type.Greetings:
					return greetings[Random.Range(0, greetings.Count)];
                case Type.Conversations:
					return conversations[Random.Range(0, conversations.Count)];
				case Type.Hurts:
					return hurts[Random.Range(0, hurts.Count)];
				case Type.Deaths:
					return deaths[Random.Range(0, deaths.Count)];
				default:
                    break;
            }

			return null;
        }

		public List<AudioClip> greetings = new List<AudioClip>();
		public List<AudioClip> conversations = new List<AudioClip>();
		public List<AudioClip> hurts = new List<AudioClip>();
		public List<AudioClip> deaths = new List<AudioClip>();
    }

	private Transform target;

	private bool DisplayingText = false;

	[Header("Parameters")]
	[SerializeField]
	private Vector3 speaker_Decal = Vector3.up * 2f;
	private CrewMember talkingMember;
	private IconVisual currentIconVisual;

	[SerializeField] private Vector2 bubbleBounds = new Vector2();

	[SerializeField]
	private int maxCharactersPerLine = 20;

	private int TextIndex = 0;

	public string[] Texts;

	public float DisplayTime = 2f;
	private float CurrentTime = 0f;

	bool timed = false;

    public delegate void OnEndDialogue();
    public OnEndDialogue onEndDialogue;

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
        Invoke("HandleOnPressInputDelay", 0.0001f);
    }

    void HandleOnPressInputDelay()
    {
        if (DisplayingText)
        {
            SoundManager.Instance.PlayRandomSound("click_med");

            if (TextIndex == Texts.Length - 1)
            {
                EndDialogue();
                //StoryReader.Instance.ContinueStory();
            }
            else
            {
                StoryInput.Instance.WaitForInput();
                ContinueDialogue();
            }

        }
    }

    void HandleGetFunction (FunctionType func, string cellParameters)
	{
		switch (func) {
		case FunctionType.OtherSpeak:
                OtherSpeak_Story(cellParameters.Remove(0, 2));
                break;

            case FunctionType.PlayerSpeak:
                PlayerSpeak_Story(cellParameters.Remove(0, 2));
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

    #region story dialogue
    public void OtherSpeak(string text)
    {
        Crews.playerCrew.UpdateCrew(Crews.PlacingType.World);
        Crews.enemyCrew.UpdateCrew(Crews.PlacingType.World);

        SetDialogueInput(text, Crews.enemyCrew.captain);
    }
    public void PlayerSpeak(string text)
    {
        Crews.playerCrew.UpdateCrew(Crews.PlacingType.World);
        SetDialogueInput(text, Crews.playerCrew.captain);
    }
    public void OtherSpeak_Story(string text)
    {
        Crews.playerCrew.UpdateCrew(Crews.PlacingType.World);
        Crews.enemyCrew.UpdateCrew(Crews.PlacingType.World);

        SetDialogueInput(text, Crews.enemyCrew.captain);

        onEndDialogue += HandleOnEndDialogue;
    }
    public void PlayerSpeak_Story(string text)
    {
        Crews.playerCrew.UpdateCrew(Crews.PlacingType.World);
        SetDialogueInput(text, Crews.playerCrew.captain);

        onEndDialogue += HandleOnEndDialogue;
    }
    public void HandleOnEndDialogue()
    {
        if (!LootUI.Instance.preventAdvanceStory)
        {
            StoryReader.Instance.ContinueStory();
        }

        onEndDialogue -= HandleOnEndDialogue;
    }
    #endregion

    #region set dialogue
    public void SetDialogueTimed (string phrase, Transform _target) {

        timed = true;

        SetDialogue(phrase, _target);

	}
    public void SetDialogueTimed(string phrase, CrewMember crewMember)
    {
        SetDialogueTimed(phrase, crewMember.Icon.dialogueAnchor);
    }


    // INPUT
    public void SetDialogueInput(string phrase, CrewMember crewMember)
    {
        StoryInput.Instance.WaitForInput();
        SetDialogue(phrase, crewMember);
    }

    public void SetDialogue (string phrase, CrewMember crewMember) {
		crewMember.Icon.animator.SetTrigger ("speak");
        crewMember.Icon.animator.SetInteger("speakIndex", Random.Range(0, 3));

		talkingMember = crewMember;

		SpeakSound();

		SetDialogue (phrase, crewMember.Icon.dialogueAnchor);
	}
	public void SetDialogue (string phrase, Transform _target) {

		phrase = NameGeneration.CheckForKeyWords (phrase);

        Texts = phrase.Split('*');

		target = _target;

		StartDialogue ();
	}
	#endregion

	#region dialogue states
	private void StartDialogue () {

		bubble_Obj.SetActive (true);

		// reset text
		TextIndex = 0;

		bubble_Text.text = Texts[TextIndex];

		CurrentTime = DisplayTime;

		DisplayingText = true;

		UpdateBubblePosition ();
		UpdateBubbleScale ();

//		if ( talkingMember != null )
//			SoundManager.Instance.PlaySound ( speakSounds[talkingMember.MemberID.voiceID] );
//	
	}

    public void ContinueDialogue()
    {
		SpeakSound();

		++TextIndex;
        bubble_Text.text = Texts[TextIndex];

        UpdateBubbleScale();
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

        if ( onEndDialogue != null)
        {
            onEndDialogue();
            //onEndDialogue = null;
        }
	}
	#endregion

	void SpeakSound()
    {
		if ( talkingMember == null)
        {
            Debug.Log("no crew member for sound speak");
			return;
        }

		if (Random.value <= 0.3f)
		{
			PlayRandomVoice(talkingMember, Voice.Type.Conversations);
		}
	}

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

    int faceIndex = 0;
	public void PlayRandomVoice(CrewMember crewMember, Voice.Type type)
    {
		PlayRandomVoice(crewMember, crewMember.Icon.iconVisual, type);
    }

	public void PlayRandomVoice(CrewMember crewMember, IconVisual iconVisual, Voice.Type type)
    {
		talkingMember = crewMember;
		currentIconVisual = iconVisual;

        source.clip = GetRandomVoice(crewMember, type);

        source.Play();

		// enfait peut être pas d'oeils tout cours.
		int id = crewMember.MemberID.GetCharacterID(ApparenceType.eyes);
		// id 3 = eye patch, ça fait bizarre si il disparait / réaparait :)
		if ( id != 3)
        {
			currentIconVisual.SetRandomSprite(ApparenceType.eyes);
			Tween.Bounce(currentIconVisual.GetImage(ApparenceType.eyes).transform);
		}

		currentIconVisual.SetRandomSprite(ApparenceType.eyebrows);
		Tween.Bounce(currentIconVisual.GetImage(ApparenceType.eyebrows).transform);
		currentIconVisual.SetRandomSprite(ApparenceType.mouth);
		Tween.Bounce(currentIconVisual.GetImage(ApparenceType.mouth).transform);

		Debug.Log("playing random voice");

		CancelInvoke("PlayRandomVoiceDelay");
		Invoke("PlayRandomVoiceDelay", 0.35f);
    }

	void PlayRandomVoiceDelay()
    {
		if ( talkingMember == null)
        {
			Debug.Log("voice delay : crew member is null");
			return;
		}

		currentIconVisual.InitVisual();

	}

	public AudioClip GetRandomVoice( CrewMember crewMember, Voice.Type type)
    {
		return voices[crewMember.MemberID.GetCharacterID(ApparenceType.voiceType)].GetRandomVoice(type);
    }


	/*public AudioClip[] clips_tmp;

	public void LoadSounds()
    {
		voices.Clear();
		string voiceName = "";
        foreach (var item in clips_tmp)
        {
            string str = item.name.Remove(7);

			if ( voiceName != str)
            {
				voiceName = str;
                Voice newVoice = new Voice();

				voices.Add(newVoice);
            }

			Voice voice = voices[voices.Count - 1];
			if (item.name.EndsWith("g"))
			{
				// greetings
				voice.greetings.Add(item);
			}
			else if (item.name.EndsWith("h"))
			{
				// hurt
				voice.hurts.Add(item);
			}
			else if (item.name.EndsWith("d"))
			{
				// death
				voice.deaths.Add(item);
			}
			else
			{
				voice.conversations.Add(item);
			}
		}
    }*/
}
