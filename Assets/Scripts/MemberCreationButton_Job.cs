using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemberCreationButton_Job : MemberCreatorButton {

    public Text text;

    public override void Start()
    {
        base.Start();
    }

    public override void Select()
    {
        if ( apparenceItem.locked)
        {
            return;
        }

        base.Select();

        backgroundImage.color = Color.gray;
    }

    public override void Deselect()
    {
        base.Deselect();

        backgroundImage.color = Color.white;

    }

    public override void UpdateImage()
    {
        base.UpdateImage();

        text.text = SkillManager.jobNames[apparenceItem.id];
    }

    public override void OnPointerUp()
    {
        base.OnPointerUp();

        if (apparenceItem.locked)
        {
            return;
        }

        MemberCreator.Instance.UpdateDescriptionText( apparenceItem.id );

        Crews.playerCrew.captain.MemberID.SetJob((Job)apparenceItem.id);
        Crews.playerCrew.captain.memberIcon.InitVisual(Crews.playerCrew.captain.MemberID);
    }

}
