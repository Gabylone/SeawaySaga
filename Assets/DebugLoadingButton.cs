using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DebugLoadingButton : MonoBehaviour, IPointerClickHandler
{
    public bool skipLoading = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        /*Tween.Bounce(transform);

        if ( skipLoading)
        {
            MainMenuManager.debugLoading_SkipLoading = true;
        }
        else
        {
            MainMenuManager.debugLoading_ActivateSceneAtTheEnd = true;
        }

        GetComponent<Image>().color = Color.green;*/
    }
}
