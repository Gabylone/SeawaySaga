using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class SkillMenu : MonoBehaviour {

    public static SkillMenu Instance;

	public GameObject group;

	public bool opened = false;

    public GameObject showSkillMenuButton;

    public delegate void OnHideCharacterStats();
    public OnHideCharacterStats onHideSkillMenu;

    public delegate void OnShowCharacterStats();
    public OnShowCharacterStats onShowSkillMenu;

    public float lootTransition_Duration = 1f;
    public float lootTransition_Decal = 1f;

    public Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    void Start () {
		Hide ();

		RayBlocker.onTouchRayBlocker += HandleOnTouchRayBlocker;
	}

	void HandleOnTouchRayBlocker ()
	{
		if ( opened ) {
			Close ();
		}
	}

	#region character stats
	public void Show () {

        Show(CrewMember.GetSelectedMember);
	}
    public void Show (CrewMember member)
    {
        // basic open the menu stuff
        InGameMenu.Instance.Open();

        DisplayCrew.Instance.Show(member);

        showSkillMenuButton.SetActive(false);

        DisplayCrew.Instance.ShowSkillMenu();

        opened = true;

        group.SetActive(true);

        LerpIn();

        if (onShowSkillMenu != null)
            onShowSkillMenu();
    }

	public void Close () {

        showSkillMenuButton.SetActive(true);

        InGameMenu.Instance.Hide();
        DisplayCrew.Instance.Hide();

        DisplayCrew.Instance.HideSkillMenu();

        opened = false;

        LerpOut();

        CancelInvoke("Hide");
        Invoke("Hide" , lootTransition_Duration);

		if (onHideSkillMenu != null)
			onHideSkillMenu ();
	}
	void Hide () {
		group.SetActive (false);
	}
    #endregion

    public void LerpIn()
    {
        transform.position = Vector3.right * lootTransition_Decal;

        transform.DOMove(Vector3.zero, lootTransition_Duration);
    }

    public void LerpOut()
    {
        transform.DOMove(Vector3.right * lootTransition_Decal, lootTransition_Duration);
    }
}
