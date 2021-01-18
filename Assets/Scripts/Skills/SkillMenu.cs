using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class SkillMenu : MonoBehaviour {

    public static SkillMenu Instance;

    public GameObject group;

    private Transform _transform;

	public bool visible = false;

    public delegate void OnShowCharacterStats();
    public OnShowCharacterStats tutoEvent;

    public CanvasGroup closeButton;

    public float lootTransition_Duration = 1f;
    public float lootTransition_Decal = 1f;

    public Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {

        _transform = GetComponent<Transform>();

		HideDelay ();

		RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;
	}

	void HandleOnTouchRayBlocker ()
	{
		if ( visible ) {
			Close ();
		}
	}

	#region character stats
	public void Show () {
        Show(CrewMember.GetSelectedMember);
	}
    public void Show(CrewMember member)
    {
        SoundManager.Instance.PlaySound("Open Chest");

        visible = true;

        group.SetActive(true);

        DisplayCrew.Instance.Show(CrewMember.GetSelectedMember);
        DisplayCrew.Instance.OnSwitchSkills();

        LerpIn();

        // tuto
        if (tutoEvent != null)
            tutoEvent();
    }

	public void Close () {

        if (!visible)
        {
            return;
        }

        InGameMenu.Instance.Hide();
        DisplayCrew.Instance.Hide();

        Hide();

	}

    public void Hide()
    {
        LerpOut();

        visible = false;

        SoundManager.Instance.PlaySound("Close Chest");

        CancelInvoke("HideDelay");
        Invoke("HideDelay", lootTransition_Duration);
    }

	void HideDelay () {
		group.SetActive (false);
	}
    #endregion

    public void LerpIn()
    {
        _transform.position = Vector3.right * lootTransition_Decal;

        closeButton.alpha = 0f;
        closeButton.DOFade(1f, 0.2f).SetDelay(lootTransition_Duration);

        _transform.DOMove(Vector3.zero, lootTransition_Duration);
    }

    public void LerpOut()
    {
        _transform.DOMove(Vector3.right * lootTransition_Decal, lootTransition_Duration);

        closeButton.alpha = 0f;
    }
}
