using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryInput : MonoBehaviour {

	public static StoryInput Instance;

	bool waitForInput = false;

    public delegate void OnPressInput();
    public OnPressInput onPressInput;

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

			if (InputManager.Instance.OnInputDown ()) {
				PressInput ();
			}
		}
	}
//
	public bool locked = false;

	public void WaitForInput () {

        CancelInvoke("WaitForInputDelay");
		Invoke ("WaitForInputDelay", 0.1f);
    }
	void WaitForInputDelay () {
		waitForInput = true;
    }

    void PressInput()
    {
        if (locked)
        {
            return;
        }


        waitForInput = false;

        if (onPressInput != null)
        {
            onPressInput();
        }



    }

}
