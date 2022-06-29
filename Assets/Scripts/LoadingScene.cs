using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    public Text m_Text;

    public RectTransform rectTransform_Background;
    public RectTransform rectTransform_Fill;

    float currentTimer = 0f;
    private float timerSpeed = 0.05f;

    private float limit = 0f;

    public static string sceneToLoad = "Menu";

    public GameObject loadingBar_Go;
    public GameObject playButton_Go;

    public bool loadScene = false;

    void Start()
    {
        Transitions.Instance.ScreenTransition.FadeOut(1f);
        Invoke("StartDelay", 1f);

        loadScene = false;

        loadingBar_Go.SetActive(true);
        playButton_Go.SetActive(false);

        UpdateUI(0);
    } 

    void StartDelay()
    {
        if (sceneToLoad== "Main")
        {
            SaveTool.Instance.CreateDirectories();
        }

        Invoke("StartDelay2", 1f);
        UpdateUI(0.1f);

    }

    void StartDelay2()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        asyncOperation.allowSceneActivation = false;

        /*if ( MainMenuManager.debugLoading_ActivateSceneAtTheEnd)
        {
            asyncOperation.allowSceneActivation = false;
        }
        else
        {
            asyncOperation.allowSceneActivation = true;
        }*/


        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            if (loadScene)
            {
                asyncOperation.allowSceneActivation = true;
            }

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                UpdateUI(1f);

                playButton_Go.SetActive(true);
            }
            else
            {
                UpdateUI(Mathf.Lerp(0.1f, 1f, asyncOperation.progress));
            }
            

            yield return null;
        }


    }

    public void PressPlay()

    {
        Tween.Bounce(playButton_Go.transform);

        Transitions.Instance.ScreenTransition.FadeIn(1f);

        Invoke("PressPlayDelay", 1f);
    }

    void PressPlayDelay()
    {
        loadScene = true;
    }

    void UpdateUI(float f)
    {
        m_Text.text = "" + (int)(f * 100) + " %";

        float w = f * rectTransform_Background.rect.width;
        Vector2 scale = new Vector2(w, rectTransform_Fill.sizeDelta.y);
        rectTransform_Fill.sizeDelta = scale;
    }
}
