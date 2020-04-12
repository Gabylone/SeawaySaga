using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenu : MonoBehaviour {

    public static SkillMenu Instance;

	public GameObject group;

	public bool opened = false;

    public GameObject showSkillMenuButton;

    public delegate void OnHideCharacterStats();
    public OnHideCharacterStats onHideSkillMenu;

    public delegate void OnShowCharacterStats();
    public OnShowCharacterStats onShowSkillMenu;

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

        if (onShowSkillMenu != null)
            onShowSkillMenu();
    }

	public void Close () {

        showSkillMenuButton.SetActive(true);

        InGameMenu.Instance.Hide();
        DisplayCrew.Instance.Hide();

        DisplayCrew.Instance.HideSkillMenu();

        opened = false;

		Hide ();

		if (onHideSkillMenu != null)
			onHideSkillMenu ();
	}
	void Hide () {
		group.SetActive (false);
	}
	#endregion
}
