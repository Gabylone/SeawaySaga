using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class OptionManager : MonoBehaviour {

	[SerializeField]
	private GameObject quitFeedback;

	[SerializeField]
	private GameObject openButton;
	[SerializeField]
	private GameObject menuGroup;

	[SerializeField]
	private GameObject saveButton;

	bool quit_Confirmed = false;

    bool opened = false;

	void Start () {
		Hide ();

        RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;

    }

    private void HandleOnTouchRayBlocker()
    {
        if (opened)
            Close();
    }

    public void Open()
    {
        InGameMenu.Instance.Open();

        opened = true;

        menuGroup.SetActive(true);

        Tween.Bounce(menuGroup.transform, 0.2f, 1.1f);

        Tween.Bounce(openButton.transform);

    }

    public void Close () {

        InGameMenu.Instance.Hide();

        opened = false;

        Hide ();

		quit_Confirmed = false;

	}
	void Hide (){
		menuGroup.SetActive (false);
	}

	#region buttons
	public void SaveButton () {

		Tween.Bounce (saveButton.transform);

		MessageDisplay.Instance.Show ("Sauvegarder partie ?");
		MessageDisplay.onValidate += HandleOnValidate;
	}

	void HandleOnValidate ()
	{
//		SaveManager.Instance.SaveOverallGame ();
	}
	public void QuitButton () {

		Tween.Bounce (transform);
		Invoke ("Quit",Tween.defaultDuration);
	}
	void Quit () {
		Application.Quit ();
	}
	#endregion
}