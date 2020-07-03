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

    public override void UpdateImage()
    {
        base.UpdateImage();

        text.text = SkillManager.jobNames[apparenceItem.id];
    }

    public override void Select()
    {
        base.Select();

        Crews.playerCrew.captain.MemberID.SetJob((Job)apparenceItem.id);

        SoundManager.Instance.PlaySound(SoundManager.Sound.Select_Small);
    }

}
