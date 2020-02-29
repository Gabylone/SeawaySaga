using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageDisplay : MonoBehaviour {

	public static MessageDisplay Instance;

	[SerializeField]
	private Text uiText;

	[SerializeField]
	private GameObject group;

	public delegate void OnValidate ();
	public static OnValidate onValidate;

	void Awake () {
		Instance = this;

        onValidate = null;
	}

    private void Start()
    {
        Close();
    }

    public void Show (string str) {

		group.SetActive (true);

		uiText.text = str;

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
		group.SetActive (false);
	}

}
