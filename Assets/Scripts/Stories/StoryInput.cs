using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInput : MonoBehaviour {

	public static StoryInput Instance;

	private bool waitForInput = false;

    public delegate void OnPressInput();
    public OnPressInput onPressInput;

    public bool activateInputDebug = false;

    void Awake()
    {
        Instance = this;
    }

	void Start () {

		InGameMenu.Instance.onOpenMenu += HandleOpenInventory;
		InGameMenu.Instance.onCloseMenu += HandleCloseInventory;;

		//WorldTouch.onPointerDown += HandlePointerDownEvent;

	}

	/*void HandlePointerDownEvent ()
	{
		if ( waitForInput ) {
			PressInput ();
		}
	}*/

	void HandleCloseInventory ()
	{
        UnlockDelay();
	}

    public void UnlockDelay()
    {
        CancelInvoke("Unlock");
		Invoke ("Unlock", 0.2f);
    }

	public void Unlock () {
		locked = false;
	}

	void HandleOpenInventory ()
	{
		Lock();
	}

	public void LockFromMember () {
        UnlockDelay();
	}

    public void Lock()
    {
        locked = true;
    }

	// Update is called once per frame
	void Update () {
		 
		if ( waitForInput ) {

			if (InputManager.Instance.Touch_Down ()) {
				PressInput ();
			}
		}
	}
//
	public bool locked = false;

	public void WaitForInput () {

        if (activateInputDebug)
        {
            Debug.Log("trigger wait for input");
        }

        CancelInvoke("WaitForInputDelay");
		Invoke ("WaitForInputDelay", 0.1f);
    }
	void WaitForInputDelay () {

        if (activateInputDebug)
        {
            Debug.Log("waiting input delay");
        }

        waitForInput = true;
    }

    void PressInput()
    {
        if (locked)
        {
            return;
        }

        if (activateInputDebug)
        {
            Debug.Log("pressing input");
        }

        waitForInput = false;


        CancelInvoke("PressInputDelay");
        Invoke("PressInputDelay", 0.001f);
    }

    void PressInputDelay()
    {
        if (onPressInput != null)
        {
            onPressInput();
        }

        if (activateInputDebug)
        {
            Debug.Log("pressing input delay");
        }
    }

}
