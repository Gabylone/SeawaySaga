using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

using DG.Tweening;

public class OptionManager : MonoBehaviour {

	[SerializeField]
	private GameObject quitFeedback;

	[SerializeField]
	private GameObject openButton;
    [SerializeField]
    private GameObject menuGroup;

    

	[SerializeField]
	private GameObject saveButton;

    public CanvasGroup canvasGroup;

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
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f);

        InGameMenu.Instance.Open();

        opened = true;

        menuGroup.SetActive(true);
    }

    public void Close () {

        canvasGroup.DOFade(0f, 0.5f);

        InGameMenu.Instance.Hide();

        opened = false;

        CancelInvoke("Hide");
        Invoke("Hide", 0.5f);

		quit_Confirmed = false;

	}
	void Hide (){
		menuGroup.SetActive (false);
	}

	#region buttons
	public void QuitButton () {

        Transitions.Instance.ScreenTransition.FadeIn(0.5f);

		Invoke ("Quit",0.6f);
	}
	void Quit () {
        SceneManager.LoadScene("Menu");
	}
	#endregion
}