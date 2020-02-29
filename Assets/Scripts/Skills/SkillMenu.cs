using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenu : MonoBehaviour {

    public static SkillMenu Instance;

	public GameObject group;

	public bool opened = false;

    public GameObject showSkillMenuButton;

    private void Awake()
    {
        Instance = this;

        onShowSkillMenu = null;
        onHideSkillMenu = null;
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
	public delegate void OnShowCharacterStats();
	public static OnShowCharacterStats onShowSkillMenu;
	public void Show () {

        InGameMenu.Instance.Open();
        //DisplayCrew.Instance.Show(CrewMember.GetSelectedMember);

        showSkillMenuButton.SetActive(false);

        DisplayCrew.Instance.ShowSkillMenu();

        LootUI.Instance.Hide();

        opened = true;

		group.SetActive (true);

		if (onShowSkillMenu != null)
			onShowSkillMenu ();
	}
	public delegate void OnHideCharacterStats ();
	public static OnHideCharacterStats onHideSkillMenu;
	public void Close () {

        showSkillMenuButton.SetActive(true);

        //InGameMenu.Instance.Hide();
        //DisplayCrew.Instance.Hide();

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
