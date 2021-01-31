using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class MessageDisplay : MonoBehaviour {

	public static MessageDisplay Instance;

	[SerializeField]
	private Text uiText;

	[SerializeField]
	private GameObject group;

    public CanvasGroup canvasGroup;

    public GameObject cancelButtonObj;

	public delegate void OnValidate ();
	public OnValidate onValidate;

	void Awake () {
		Instance = this;
	}

    private void Start()
    {
        CloseDelay();
    }

    public void Display (string str) {

		group.SetActive (true);

        str = NameGeneration.CheckForKeyWords(str);

		uiText.text = str;

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.4f);
    }

    public void HideCancelButton()
    {
        cancelButtonObj.SetActive(false);
    }

	public void Validate () {

        SoundManager.Instance.PlayRandomSound("button_tap_light");

        Close();

		if ( onValidate != null ) {
			onValidate ();
		}

		onValidate = null;
	}
	public void Cancel() {

        SoundManager.Instance.PlayRandomSound("button_tap_light");

        Close();
		onValidate = null;
	}

	void Close ()
	{
        canvasGroup.DOFade(0f, 0.4f);
        Invoke("CloseDelay", 0.4f);



        if (DisplayFastTravelInfo.Instance != null && DisplayFastTravelInfo.Instance.visible)
        {
            DisplayFastTravelInfo.Instance.Hide();
        }

    }

    void CloseDelay()
    {
        group.SetActive(false);
    }

}
