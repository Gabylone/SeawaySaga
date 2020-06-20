using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemberCreationButton_Apparence : MemberCreatorButton, IPointerClickHandler {

    public override void Start()
    {
        base.Start();
    }

    public override void Select()
    {
        base.Select();

        if (apparenceItem.locked)
            return;

        Crews.playerCrew.captain.MemberID.SetCharacterID(apparenceItem.apparenceType, apparenceItem.id);
        SoundManager.Instance.PlaySound(SoundManager.Sound.Select_Small);

        Crews.playerCrew.captain.Icon.InitVisual(Crews.playerCrew.captain.MemberID);
    }

    public override void Deselect()
    {
        base.Deselect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }
}
