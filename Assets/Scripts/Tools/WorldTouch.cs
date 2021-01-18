using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WorldTouch : MonoBehaviour
{
    public static WorldTouch Instance;

    public delegate void OnTouchWorld();
    public static OnTouchWorld onPointerExit;

    public delegate void OnPointerDownEvent();
    public static OnPointerDownEvent onPointerDown;

    public bool debugTouch = false;

    public bool touching = false;

    public bool swipped = false;

    float timer = 0f;
    float timeToTouch = 0.25f;

	public Image testimage;

    public bool isEnabled = false;

    Vector2 oldPosition;

    public bool locked = false;

    bool invoking = false;

    public float invokeDelay = 0.5f;

    private void Awake()
    {
        Instance = this;

        onPointerDown = null;
        onPointerExit = null;
    }

    public void Enable()
    {
        isEnabled = true;
        invoking = false;
    }

    public void Disable()
    {
        isEnabled = false;
    }

    public void Lock()
    {
        
        locked = true;
    }

    public void Unlock()
    {
        locked = false;
    }

    public void HandleOnSwipe()
    {
        touching = false;
        swipped = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isEnabled == false)
        {
            if (!IsPointerOverUIObject() && !invoking)
            {
                invoking = true;

                CancelInvoke("Enable");
                Invoke("Enable", invokeDelay);
            }
        }
        else
        {
            if ( debugTouch)
            {

                /*RaycastHit hit;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit , 1000f ))
                {
                    Debug.Log("raycast hitting : " + hit.collider.name);
                }*/
            }

            if (IsPointerOverUIObject())
            {
                Disable();
            }
        }

    }

    public bool IsEnabled ()
    {
        return isEnabled;
        //return isEnabled;
    }

    public void OnMouseDown()
    {
        if (locked)
        {
            return;
        }

        if (!IsEnabled())
        {
            return;
        }

        if ( IsPointerOverUIObject())
        {
            return;
        }

        touching = true;
        
        if (onPointerDown != null)
        {
            onPointerDown();
        }

    }

    private void OnMouseUp()
    {
        if (locked)
        {
            if (debugTouch)
            {
                Debug.Log("! Locked !");
            }

            return;
        }

        if (!IsEnabled())
        {
            if (debugTouch)
            {
                Debug.Log("! Disabled !");
            }

            return;
        }

        if (!touching)
        {
            if (debugTouch)
            {
                Debug.Log("! Not touching !");
            }

            return;
        }

        touching = false;

        if (onPointerExit != null)
        {
            onPointerExit();
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                Vector2 v = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

                eventDataCurrentPosition.position = v;
                oldPosition = v;
            }
            else
            {
                eventDataCurrentPosition.position = oldPosition;
            }
        }
        else
        {
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);


        }

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        if ( debugTouch && results.Count > 0)
        {
            Debug.Log("UI : results : " + results[0].gameObject.name + " (out of " + results.Count + ")");
        }

        return results.Count > 0;
    }

}
