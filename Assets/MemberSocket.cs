using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class MemberSocket : MonoBehaviour
{
    Transform tr;

    public CanvasGroup canvasGroup;

    public GameObject kickOut_Group;

    public Outline outline;

    public float fadeDuration = 0.2f;

    public bool visible = false;
    public GameObject group;

    public Image image;

    private void Start()
    {
        tr = transform;

        Hide();
    }

    public void Show()
    {
        if (visible)
        {
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, fadeDuration);

        group.SetActive(true);

        visible = true;
    }

    public void Hide()
    {
        /*if (!visible)
        {
            return;
        }*/

        group.SetActive(false);

        canvasGroup.DOFade(0f, fadeDuration);
        kickOut_Group.SetActive(false);

        visible = false;
    }

    public void Select()
    {
        image.color = Color.grey;

        outline.effectColor = Color.red;

        outline.effectDistance = new Vector2( 2f,2f );

        if ( DisplayCrew.Instance.visible && Crews.playerCrew.CrewMembers.Count > 1)
        {
            kickOut_Group.SetActive(true);
        }
        else
        {
            kickOut_Group.SetActive(false);
        }
    }

    public void Deselect()
    {
        outline.effectDistance = new Vector2(1.5f,1.5f);

        outline.effectColor = Color.black;
        image.color = Color.white;
        kickOut_Group.SetActive(false);
    }

    public void OnPointerClick()
    {
        if (Crews.playerCrew.CrewMembers.Count <= 1 )
        {
            return;
        }

        Tween.Bounce(tr);

        MessageDisplay.Instance.onValidate += HandleOnValidate;
        MessageDisplay.Instance.Display("Are you sure you want to remove " + CrewMember.GetSelectedMember.MemberName + " from the crew?", true);

    }

    void HandleOnValidate()
    {
        DisplayCombatResults.Instance.Display(CrewMember.GetSelectedMember.MemberName + " leaves the crew", "The captain dismisses " + CrewMember.GetSelectedMember.MemberName + " from the crew, wishes them good luck, and everyone waves goodbye to them.");

        Crews.playerCrew.RemoveMember(CrewMember.GetSelectedMember);

        CrewMember.SetSelectedMember(Crews.playerCrew.CrewMembers[0]);

        Crews.playerCrew.UpdateCrew(Crews.PlacingType.Portraits);
    }
}
