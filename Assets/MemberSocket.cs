using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberSocket : MonoBehaviour
{
    Transform tr;

    private void Start()
    {
        tr = transform;
    }

    public void OnPointerClick()
    {
        Tween.Bounce(tr);

        MessageDisplay.Instance.onValidate += HandleOnValidate;
        MessageDisplay.Instance.Show("Are you sure you want to remove " + CrewMember.GetSelectedMember.MemberName + " from the crew ?!");

    }

    void HandleOnValidate()
    {
        DisplayCombatResults.Instance.Display(CrewMember.GetSelectedMember.MemberName + " leaves the crew", "The captain discards " + CrewMember.GetSelectedMember.MemberName + " from the boat, wishes him good luck, and everybody waves at him goodbye.");

        Crews.playerCrew.RemoveMember(CrewMember.GetSelectedMember);

        Crews.playerCrew.UpdateCrew(Crews.PlacingType.Portraits);

    }
}
