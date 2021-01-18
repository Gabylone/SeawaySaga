using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public float logoDuration = 4f; 

    // Start is called before the first frame update
    void Start()
    {
        Transitions.Instance.ScreenTransition.FadeOut(0.5f);

        Invoke("GoToMenu", logoDuration);

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    void GoToMenu()
    {
        Transitions.Instance.ScreenTransition.FadeIn(0.5f);

        Invoke("GoToMenuDelay", logoDuration);

    }

    void GoToMenuDelay()
    {
        LoadingScene.sceneToLoad = "Menu";
        SceneManager.LoadScene("Loading");
    }
}
