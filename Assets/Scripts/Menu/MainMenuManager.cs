using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour {

    public GameObject mapsGroup;

	[SerializeField]
	private GameObject quitButton;


    public GameObject HandObj;

    [SerializeField]
	private GameObject loadButton;

	[SerializeField]
	private GameObject playButton;

	[SerializeField]
	private GameObject loadMenu;

	public float transitionDuration = 1.2f;

	public bool loadMenuOpened = false;

    public float mapsAppearDuration = 0.5f;

    public Animator mapAnimator;

	void Start () {
		Transitions.Instance.ScreenTransition.FadeOut (0.5f);

		Screen.orientation = ScreenOrientation.Landscape;

        mapsGroup.SetActive(false);
        MenuObj.SetActive(false);


	}

	public void NewGameButton () {

		Tween.Bounce (playButton.transform);

        mapsGroup.SetActive(true);
        mapAnimator.SetBool("Opened", true);

        CancelInvoke("HideMapsDelay");

        MenuObj.SetActive(false);

        SoundManager.Instance.PlayRandomSound("Magic Chimes");
        SoundManager.Instance.PlayRandomSound("Bag");

    }

    public void HideMaps ()
    {

        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Swipe");
        SoundManager.Instance.PlayRandomSound("Bag");

        Invoke("HideMapsDelay", 0.5f);

        mapAnimator.SetBool("Opened", false);

    }

    void HideMapsDelay()
    {
        MenuObj.SetActive(true);

        mapsGroup.SetActive(false);
    }

    void HandleOnValidate ()
	{
		Transitions.Instance.ScreenTransition.FadeIn (transitionDuration);
		Invoke ("NewGameDelay" , transitionDuration);
        mapAnimator.SetBool("Opened", false);

        SoundManager.Instance.PlayRandomSound("button_tap_light");
    }
	private void NewGameDelay () {
		KeepOnLoad.dataToLoad = -1;
		SceneManager.LoadScene ("Loading");
	}
	public void QuitButton () {

        SoundManager.Instance.PlayRandomSound("button_tap_light");

        Tween.Bounce (quitButton.transform);

		Transitions.Instance.ScreenTransition.FadeIn (transitionDuration);
		Invoke ("QuitDelay" , transitionDuration);
	}
	private void QuitDelay () {
		Application.Quit ();
		//
	}

	public void Load () {

        

        Tween.Bounce (loadButton.transform);

		KeepOnLoad.dataToLoad = 0;
		Transitions.Instance.ScreenTransition.FadeIn (transitionDuration);
		Invoke ("LoadDelay" , transitionDuration);
	}
	private void LoadDelay () {
		SceneManager.LoadScene ("Loading");
    }

	public void OpenLoadMenu () {
		loadMenu.SetActive (true);
		loadButton.SetActive (false);

		Tween.Bounce (loadMenu.transform, 0.1f , 1.05f );
	}

	public void CloseLoadMenu () {
		loadMenu.SetActive (false);
		loadButton.SetActive (true);
	}

    public GameObject MenuObj;
    public void OnTapBackground()
    {
        SoundManager.Instance.PlayRandomSound("button_tap_light");
        SoundManager.Instance.PlayRandomSound("Whoosh");
        SoundManager.Instance.PlayRandomSound("Swipe");

        MenuObj.SetActive(true);
        HandObj.SetActive(false);
    }
}
