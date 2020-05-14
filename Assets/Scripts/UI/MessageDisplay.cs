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

	public delegate void OnValidate ();
	public static OnValidate onValidate;

	void Awake () {
		Instance = this;

        onValidate = null;
	}

    private void Start()
    {
        CloseDelay();
    }

    public void Show (string str) {

		group.SetActive (true);

		uiText.text = str;

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.4f);


    }

	public void Validate () {

		Close ();

		if ( onValidate != null ) {
			onValidate ();
		}

		onValidate = null;
	}
	public void Cancel() {
		Close ();
		onValidate = null;
	}

	void Close ()
	{
        canvasGroup.DOFade(0f, 0.4f);
        Invoke("CloseDelay", 0.4f);
        
	}

    void CloseDelay()
    {
        group.SetActive(false);
    }

}
