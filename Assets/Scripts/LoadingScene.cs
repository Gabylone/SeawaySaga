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

    public bool activateAtStart = true;

    float currentTimer = 0f;
    private float timerSpeed = 0.2f;

    private float limit = 0.9f;
    
    public static string sceneToLoad = "Menu";

    void Start()
    {
        Transitions.Instance.ScreenTransition.FadeOut(1f);
        Invoke("StartDelay", 1f);

        UpdateUI(0);
    }

    private void LateUpdate()
    {
        UpdateUI(currentTimer);

        if ( currentTimer < limit)
        {
            currentTimer += Time.deltaTime * timerSpeed;
        }
    }

    void StartDelay()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        if ( MainMenuManager.debugLoading_ActivateSceneAtTheEnd)
        {
            asyncOperation.allowSceneActivation = false;
        }
        else
        {
            asyncOperation.allowSceneActivation = true;
        }


        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                currentTimer = asyncOperation.progress;
                limit = 100f;

                UpdateUI(asyncOperation.progress);

                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    void UpdateUI(float f)
    {
        m_Text.text = "" + (int)(f * 100) + " %";

        float w = f * rectTransform_Background.rect.width;
        Vector2 scale = new Vector2(w, rectTransform_Fill.sizeDelta.y);
        rectTransform_Fill.sizeDelta = scale;
    }
}
