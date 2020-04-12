using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Directions dir;

    /*private void OnMouseEnter()
    {
        if ( WorldTouch.Instance.touching && WorldTouch.Instance.IsEnabled () )
        {
            Swipe.Instance.TriggerSwipe(dir);
        }
    }

    private void OnMouseDown()
    {
        if ( WorldTouch.Instance.IsEnabled())
        {
            Swipe.Instance.TriggerSwipe(dir);
        }
    }

    private void OnMouseExit()
    {
        if( InputManager.Instance.OnInputStay() && WorldTouch.Instance.IsEnabled())
        {
            WorldTouch.Instance.Enable();
            WorldTouch.Instance.OnMouseDown();

        }
    }*/
}
